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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Interpreter {
  /// <summary>
  /// A wrapper around Scope exposing it as a string-object dictionary. Used to expose Globals dictionary from Main scope
  /// </summary>
  public class ScopeValuesDictionary : IDictionary<string, object> {
    ScopeBase _scope; 

    internal ScopeValuesDictionary(ScopeBase scope) {
      _scope = scope;
    }

    public void Add(string key, object value) {
      var slot = _scope.Info.GetSlot(key);
      if (slot == null)
        slot = _scope.AddSlot(key);
      _scope.SetValue(slot.Index, value);
    }

    public bool ContainsKey(string key) {
      return _scope.Info.GetSlot(key) != null;      
    }

    public ICollection<string> Keys {
      get { return _scope.Info.GetNames(); }
    }

    //We do not remove the slotInfo (you can't do that, slot set can only grow); instead we set the value to null
    // to indicate "unassigned"
    public bool Remove(string key) {
      this[key] = null;
      return true;
    }

    public bool TryGetValue(string key, out object value) {
      value = null;
      SlotInfo slot = _scope.Info.GetSlot(key);
      if (slot == null)
        return false;
      value = _scope.GetValue(slot.Index);
      return true;
    }

    public ICollection<object> Values {
      get {return _scope.GetValues(); }
    }

    public object this[string key] {
      get {
        object value;
        TryGetValue(key, out value);
        return value;        
      }
      set {
        Add(key, value);
      }
    }

    public void Add(KeyValuePair<string, object> item) {
      Add(item.Key, item.Value);
    }

    public void Clear() {
      var values = _scope.GetValues();
      for (int i = 0; i < values.Length; i++)
        values[i] = null; 
    }

    public bool Contains(KeyValuePair<string, object> item) {
      return _scope.Info.GetSlot(item.Key) != null;
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
      throw new NotImplementedException();
    }

    public int Count {
      get { return _scope.Info.GetSlotCount(); }
    }

    public bool IsReadOnly {
      get { return true; }
    }

    public bool Remove(KeyValuePair<string, object> item) {
      return Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
      var slots = _scope.Info.GetSlots(); //make local copy
      foreach (var slot in slots)
        yield return new KeyValuePair<string, object>(slot.Name, _scope.GetValue(slot.Index));
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator(); 
    }
  }
}
