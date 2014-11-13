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
using System.Threading;

namespace Irony.Interpreter {

  public class ScopeBase {
    public ScopeInfo Info;
    public volatile object[] Values;

    public ScopeBase(ScopeInfo scopeInfo) : this(scopeInfo, null) {}
    public ScopeBase(ScopeInfo scopeInfo, object[] values) {
      Info = scopeInfo;
      Values = values; 
      if (Values == null)
        Values = new object[scopeInfo.ValuesCount];
    }

    public SlotInfo AddSlot(string name) {
      var slot = Info.AddSlot(name, SlotType.Value);
      if (slot.Index >= Values.Length)
        Resize(Values.Length + 4);
      return slot; 
    }

    public object[] GetValues() {
      return Values; 
    }

    public object GetValue(int index) {
      try {
        var tmp = Values;
        // The following line may throw null-reference exception (tmp==null), if resizing is happening at the same time
        // It may also throw IndexOutOfRange exception if new variable was added by another thread in another frame(scope)
        // but this scope and Values array were created before that, so Values is shorter than #slots in SlotInfo. 
        // But in this case, it does not matter, result value is null (unassigned)
        return tmp[index]; 
      } catch (NullReferenceException) {
        Thread.Sleep(0); // Silverlight does not have Thread.Yield; 
        // Thread.Yield(); // maybe SpinWait.SpinOnce?
        return GetValue(index); //repeat attempt
      } catch (IndexOutOfRangeException) {
        return null; //we do not resize here, value is unassigned anyway.
      }

    }//method

    public void SetValue(int index, object value) {
      try {
        var tmp = Values;
        // The following line may throw null-reference exception (tmp==null), if resizing is happening at the same time
        // It may also throw IndexOutOfRange exception if new variable was added by another thread in another frame(scope)
        // but this scope and Values array were created before that, so Values is shorter than #slots in SlotInfo 
        tmp[index] = value; 
        //Now check that tmp is the same as Values - if not, then resizing happened in the middle, 
        // so repeat assignment to make sure the value is in resized array.
        if (tmp != Values)
          SetValue(index, value); // do it again
      } catch (NullReferenceException) {
        Thread.Sleep(0); // it's  OK to Sleep intead of SpinWait - it is really rare event, so we don't care losing a few more cycles here. 
        SetValue(index, value); //repeat it again
      } catch (IndexOutOfRangeException) {
        Resize(Info.GetSlotCount());
        SetValue(index, value); //repeat it again
      }
    }//method

    // Disabling warning: 'Values: a reference to a volatile field will not be treated as volatile'
    // According to MSDN for CS0420 warning (see http://msdn.microsoft.com/en-us/library/4bw5ewxy.aspx),
    // this does NOT apply to Interlocked API - which we use here.
    #pragma warning disable 0420
    protected void Resize(int newSize) {
      lock (this.Info.LockObject) {
        if (Values.Length >= newSize) return; 
        object[] tmp = Interlocked.Exchange(ref Values, null); 
        Array.Resize(ref tmp, newSize);
        Interlocked.Exchange(ref Values, tmp);
      }
    }

    public IDictionary<string, object> AsDictionary() {
      return new ScopeValuesDictionary(this);
    }

    public override string ToString() {
      return Info.ToString();
    }


  }//class


}
