using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace LiveSplit.Model.RunFactories
{
    public class LlanfairRunFactory : IRunFactory
    {
        public String Path { get; set; }

        public LlanfairRunFactory(String path = null)
        {
            Path = path;
        }
        private void Empty(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
                file.Delete();

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }

        protected String Unescape(String text)
        {
            return text.Replace(@"\.", @",").Replace(@"\\", @"\");
        }

        private string GetJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    return FindJavaInRegistry(baseKey);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    return FindJavaInRegistry(baseKey);
                }
            }
        }

        protected String FindJavaInRegistry(RegistryKey baseKey)
        {
            var software = baseKey.OpenSubKey("SOFTWARE");
            var javasoft = software.OpenSubKey("JavaSoft");
            var jre = javasoft.OpenSubKey("Java Runtime Environment");
            //string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
            using (jre)
            {
                string currentVersion = jre.GetValue("CurrentVersion").ToString();
                using (RegistryKey key = jre.OpenSubKey(currentVersion))
                {
                    return key.GetValue("JavaHome").ToString();
                }
            }
        }

        protected IRun ReadFromLlanfairTextFile(Stream stream, IComparisonGeneratorsFactory factory)
        {
            var run = new Run(factory);

            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();
                var titleInfo = line.Split(',');
                run.GameName = Unescape(titleInfo[0]);
                run.CategoryName = Unescape(titleInfo[1]);
                run.AttemptCount = Int32.Parse(Unescape(titleInfo[2]));
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        var splitInfo = line.Split(',');
                        Time pbSplitTime = default(Time);
                        Time goldTime = default(Time);

                        try
                        {
                            pbSplitTime.RealTime = TimeSpan.Parse(Unescape(splitInfo[1]));
                        }
                        catch
                        {
                            try
                            {
                                pbSplitTime.RealTime = TimeSpan.Parse(Unescape("0:" + splitInfo[1]));
                            }
                            catch
                            {
                                pbSplitTime.RealTime = TimeSpan.Parse(Unescape("0:0:" + splitInfo[1]));
                            }
                        }

                        try
                        {
                            goldTime.RealTime = TimeSpan.Parse(Unescape(splitInfo[2]));
                        }
                        catch
                        {
                            try
                            {
                                goldTime.RealTime = TimeSpan.Parse(Unescape("0:" + splitInfo[2]));
                            }
                            catch
                            {
                                goldTime.RealTime = TimeSpan.Parse(Unescape("0:0:" + splitInfo[2]));
                            }
                        }

                        if (pbSplitTime.RealTime == TimeSpan.Zero)
                            pbSplitTime.RealTime = null;

                        if (goldTime.RealTime == TimeSpan.Zero)
                            goldTime.RealTime = null;

                        var realIconPath = "";
                        if (splitInfo.Length > 3)
                            realIconPath = Unescape(splitInfo[3]);
                        Image icon = null;
                        if (realIconPath.Length > 0)
                        {
                            try
                            {
                                using (var imageStream = File.OpenRead(realIconPath))
                                {
                                    icon = Image.FromStream(imageStream);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                        run.AddSegment(Unescape(splitInfo[0]), pbSplitTime, goldTime, icon);
                    }
                }
            }

            return run;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            using (var stream = File.OpenRead(Path))
            {
                var data = new byte[4];
                stream.Read(data, 0, 4);
                if (data[0] != 0xac
                    || data[1] != 0xed
                    || data[2] != 0x00
                    || data[3] != 0x05)
                    throw new NotSupportedException();
            }

            var liveSplitBasePath = System.IO.Path.GetTempPath() + "LiveSplit";
            var splitsBasePath = liveSplitBasePath + @"\Splits";
            var splitsFilePath = splitsBasePath + @"\splits";
            var loaderPath = liveSplitBasePath + @"\loader.jar";
            var javaPath = GetJavaInstallationPath();

            if (!Directory.Exists(liveSplitBasePath))
                Directory.CreateDirectory(liveSplitBasePath);

            if (!Directory.Exists(splitsBasePath))
                Directory.CreateDirectory(splitsBasePath);

            if (File.Exists(loaderPath) && File.GetCreationTimeUtc(loaderPath) < new DateTime(2013, 12, 13, 20, 0, 0, 0, DateTimeKind.Utc))
            {
                File.Delete(loaderPath);
            }

            if (!File.Exists(loaderPath))
            {
                File.Create(loaderPath).Close();
                using (var memoryStream = new MemoryStream(Resources.LlanfairLoader))
                {
                    using (var stream = File.Open(loaderPath, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write))
                    {
                        memoryStream.CopyTo(stream);
                    }
                }
            }

            Empty(new DirectoryInfo(splitsBasePath));

            var process = Process.Start(System.IO.Path.Combine(javaPath, "bin\\javaw.exe"), String.Format("-jar \"{0}\" \"{1}\" \"{2}\"", loaderPath, Path, splitsFilePath));
            process.WaitForExit();
            process.Close();

            using (var stream = File.OpenRead(splitsFilePath))
            {
                return ReadFromLlanfairTextFile(stream, factory);
            }
        }
    }
}
