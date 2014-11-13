#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution.
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
// This file and all functionality of dynamic assembly reloading was contributed by Alexey Yakovlev (yallie)
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Irony.Parsing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Irony.GrammarExplorer {
  /// <summary>
  /// Maintains grammar assemblies, reloads updated files automatically.
  /// </summary>
  class GrammarLoader {
    private TimeSpan _autoRefreshDelay = TimeSpan.FromMilliseconds(1000);
    private static HashSet<string> _probingPaths = new HashSet<string>();
    private Dictionary<string, CachedAssembly> _cachedGrammarAssemblies = new Dictionary<string, CachedAssembly>();
    private static Dictionary<string, Assembly> _loadedAssembliesByNames = new Dictionary<string, Assembly>();
    private static HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();
    private static bool _enableBrowsingForAssemblyResolution = false;

    static GrammarLoader() {
      AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => _loadedAssembliesByNames[args.LoadedAssembly.FullName] = args.LoadedAssembly;
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => FindAssembly(args.Name);
    }

    static Assembly FindAssembly(string assemblyName) {
      if (_loadedAssembliesByNames.ContainsKey(assemblyName))
        return _loadedAssembliesByNames[assemblyName];
      // ignore resource assemblies
      if (assemblyName.ToLower().Contains(".resources, version="))
        return _loadedAssembliesByNames[assemblyName] = null;
      // use probing paths to look for dependency assemblies
      var fileName = assemblyName.Split(',').First() + ".dll";
      foreach (var path in _probingPaths) {
        var fullName = Path.Combine(path, fileName);
        if (File.Exists(fullName)) {
          try {
            return LoadAssembly(fullName);
          }
          catch {
            // the file seems to be bad, let's try to find another one
          }
        }
      }
      // the last chance: try asking user to locate the assembly
      if (_enableBrowsingForAssemblyResolution) {
        fileName = BrowseFor(assemblyName);
        if (!string.IsNullOrWhiteSpace(fileName))
          return LoadAssembly(fileName);
      }
      // assembly not found, don't search for it again
      return _loadedAssembliesByNames[assemblyName] = null;
    }

    static string BrowseFor(string assemblyName) {
      var fileDialog = new OpenFileDialog {
        Title = "Please locate assembly: " + assemblyName,
        Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*"
      };
      using (fileDialog) {
        if (fileDialog.ShowDialog() == DialogResult.OK)
          return fileDialog.FileName;
      }
      return null;
    }

    class CachedAssembly {
      public long FileSize;
      public DateTime LastWriteTime;
      public FileSystemWatcher Watcher;
      public Assembly Assembly;
      public bool UpdateScheduled;
    }

    public event EventHandler AssemblyUpdated;

    public GrammarItem SelectedGrammar { get; set; }

    public Grammar CreateGrammar() {
      if (SelectedGrammar == null)
        return null;

      // resolve dependencies while loading and creating grammars
      _enableBrowsingForAssemblyResolution = true;
      try {
        var type = SelectedGrammarAssembly.GetType(SelectedGrammar.TypeName, true, true);
        return Activator.CreateInstance(type) as Grammar;
      }
      finally {
        _enableBrowsingForAssemblyResolution = false;
      }
    }

    Assembly SelectedGrammarAssembly {
      get {
        if (SelectedGrammar == null)
          return null;

        // create assembly cache entry as needed
        var location = SelectedGrammar.Location;
        if (!_cachedGrammarAssemblies.ContainsKey(location)) {
          var fileInfo = new FileInfo(location);
          _cachedGrammarAssemblies[location] =
            new CachedAssembly {
              LastWriteTime = fileInfo.LastWriteTime,
              FileSize = fileInfo.Length,
              Assembly = null
            };

          // set up file system watcher
          _cachedGrammarAssemblies[location].Watcher = CreateFileWatcher(location);
        }

        // get loaded assembly from cache if possible
        var assembly = _cachedGrammarAssemblies[location].Assembly;
        if (assembly == null) {
          assembly = LoadAssembly(location);
          _cachedGrammarAssemblies[location].Assembly = assembly;
        }

        return assembly;
      }
    }

    private FileSystemWatcher CreateFileWatcher(string location) {
      var folder = Path.GetDirectoryName(location);
      var watcher = new FileSystemWatcher(folder);
      watcher.Filter = Path.GetFileName(location);

      watcher.Changed += (s, args) => {
        if (args.ChangeType != WatcherChangeTypes.Changed)
          return;

        lock (this) {
          // check if assembly file was changed indeed since the last event
          var cacheEntry = _cachedGrammarAssemblies[location];
          var fileInfo = new FileInfo(location);
          if (cacheEntry.LastWriteTime == fileInfo.LastWriteTime && cacheEntry.FileSize == fileInfo.Length)
            return;

          // reset cached assembly and save last file update time
          cacheEntry.LastWriteTime = fileInfo.LastWriteTime;
          cacheEntry.FileSize = fileInfo.Length;
          cacheEntry.Assembly = null;

          // check if file update is already scheduled (work around multiple FileSystemWatcher event firing)
          if (!cacheEntry.UpdateScheduled) {
            cacheEntry.UpdateScheduled = true;
            // delay auto-refresh to make sure the file is closed by the writer
            ThreadPool.QueueUserWorkItem(_ => {
              Thread.Sleep(_autoRefreshDelay);
              cacheEntry.UpdateScheduled = false;
              OnAssemblyUpdated(location);
            });
          }
        }
      };

      watcher.EnableRaisingEvents = true;
      return watcher;
    }

    private void OnAssemblyUpdated(string location) {
      if (AssemblyUpdated == null || SelectedGrammar == null || SelectedGrammar.Location != location)
        return;
      AssemblyUpdated(this, EventArgs.Empty);
    }

    public static Assembly LoadAssembly(string fileName) {
      // normalize the filename
      fileName = new FileInfo(fileName).FullName;
      // save assembly path for dependent assemblies probing
      var path = Path.GetDirectoryName(fileName);
      _probingPaths.Add(path);
      // try to load assembly using the standard policy
      var assembly = Assembly.LoadFrom(fileName);
      // if the standard policy returned the old version, force reload
      if (_loadedAssemblies.Contains(assembly)) {
          assembly = Assembly.Load(File.ReadAllBytes(fileName));
      }
      // cache the loaded assembly by its location
      _loadedAssemblies.Add(assembly);
      return assembly;
    }
  }
}
