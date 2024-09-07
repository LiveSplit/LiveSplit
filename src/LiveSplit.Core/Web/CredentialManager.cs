/*

MIT License

Copyright(c) 2017 Gérald Barré

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace LiveSplit.Web;

public static class CredentialManager
{
    public static Credential ReadCredential(string applicationName)
    {
        bool read = CredRead(applicationName, CredentialType.Generic, 0, out IntPtr nCredPtr);
        if (read)
        {
            using var critCred = new CriticalCredentialHandle(nCredPtr);
            CREDENTIAL cred = critCred.GetCredential();
            return ReadCredential(cred);
        }

        return null;
    }

    private static Credential ReadCredential(CREDENTIAL credential)
    {
        string applicationName = Marshal.PtrToStringUni(credential.TargetName);
        string userName = Marshal.PtrToStringUni(credential.UserName);
        string secret = null;
        if (credential.CredentialBlob != IntPtr.Zero)
        {
            secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);
        }

        return new Credential(credential.Type, applicationName, userName, secret);
    }

    public static void WriteCredential(string applicationName, string userName, string secret)
    {
        byte[] byteArray = secret == null ? null : Encoding.Unicode.GetBytes(secret);
        // XP and Vista: 512;
        // 7 and above: 5*512
        if (Environment.OSVersion.Version < new Version(6, 1) /* Windows 7 */)
        {
            if (byteArray != null && byteArray.Length > 512)
            {
                throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 512 bytes.");
            }
        }
        else
        {
            if (byteArray != null && byteArray.Length > 512 * 5)
            {
                throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 2560 bytes.");
            }
        }

        var credential = new CREDENTIAL
        {
            AttributeCount = 0,
            Attributes = IntPtr.Zero,
            Comment = IntPtr.Zero,
            TargetAlias = IntPtr.Zero,
            Type = CredentialType.Generic,
            Persist = (uint)CredentialPersistence.LocalMachine,
            CredentialBlobSize = (uint)(byteArray == null ? 0 : byteArray.Length),
            TargetName = Marshal.StringToCoTaskMemUni(applicationName),
            CredentialBlob = Marshal.StringToCoTaskMemUni(secret),
            UserName = Marshal.StringToCoTaskMemUni(userName ?? Environment.UserName)
        };

        bool written = CredWrite(ref credential, 0);
        Marshal.FreeCoTaskMem(credential.TargetName);
        Marshal.FreeCoTaskMem(credential.CredentialBlob);
        Marshal.FreeCoTaskMem(credential.UserName);

        if (!written)
        {
            int lastError = Marshal.GetLastWin32Error();
            throw new Exception(string.Format("CredWrite failed with the error code {0}.", lastError));
        }
    }

    public static bool CredentialExists(string applicationName)
    {
        return ReadCredential(applicationName) != null;
    }

    public static void DeleteCredential(string applicationName)
    {
        if (applicationName == null)
        {
            throw new ArgumentNullException(nameof(applicationName));
        }

        if (!CredentialExists(applicationName))
        {
            return;
        }

        bool success = CredDelete(applicationName, CredentialType.Generic, 0);
        if (!success)
        {
            int lastError = Marshal.GetLastWin32Error();
            throw new Win32Exception(lastError);
        }
    }

    [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

    [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredDelete(string target, CredentialType type, int reservedFlag);

    [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

    [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
    private static extern bool CredFree([In] IntPtr cred);

    private enum CredentialPersistence : uint
    {
        Session = 1,
        LocalMachine,
        Enterprise
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public uint Flags;
        public CredentialType Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    private sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public CriticalCredentialHandle(IntPtr preexistingHandle)
        {
            SetHandle(preexistingHandle);
        }

        public CREDENTIAL GetCredential()
        {
            if (!IsInvalid)
            {
                var credential = (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL));
                return credential;
            }

            throw new InvalidOperationException("Invalid CriticalHandle!");
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                CredFree(handle);
                SetHandleAsInvalid();
                return true;
            }

            return false;
        }
    }
}

public enum CredentialType
{
    Generic = 1,
    DomainPassword,
    DomainCertificate,
    DomainVisiblePassword,
    GenericCertificate,
    DomainExtended,
    Maximum,
    MaximumEx = Maximum + 1000,
}

public class Credential
{
    public CredentialType CredentialType { get; }

    public string ApplicationName { get; }

    public string UserName { get; }

    public string Password { get; }

    public Credential(CredentialType credentialType, string applicationName, string userName, string password)
    {
        ApplicationName = applicationName;
        UserName = userName;
        Password = password;
        CredentialType = credentialType;
    }

    public override string ToString()
    {
        return string.Format("CredentialType: {0}, ApplicationName: {1}, UserName: {2}, Password: {3}", CredentialType, ApplicationName, UserName, Password);
    }
}
