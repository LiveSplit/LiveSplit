using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
#pragma warning disable 1591

// Note: Please be careful when modifying this because it could break existing components!

namespace LiveSplit.ComponentUtil
{
    public class SignatureScanner
    {
        private byte[] _memory;
        private Process _process;
        private IntPtr _address;
        private int _size;

        public IntPtr Address
        {
            get { return _address; }
            set {
                _memory = null;
                _address = value;
            }
        }

        public int Size
        {
            get { return _size; }
            set {
                _memory = null;
                _size = value;
            }
        }

        public Process Process
        {
            get { return _process; }
            set {
                _memory = null;
                _process = value;
            }
        }

        public SignatureScanner(Process proc, IntPtr addr, int size)
        {
            if (proc == null)
                throw new ArgumentNullException(nameof(proc));
            if (addr == IntPtr.Zero)
                throw new ArgumentException("addr cannot be IntPtr.Zero.", nameof(addr));
            if (size <= 0)
                throw new ArgumentException("size cannot be less than zero.", nameof(size));

            _process = proc;
            _address = addr;
            _size = size;
            _memory = new byte[1];
        }

        public IntPtr Scan(SigScanTarget target)
        {
            if (_memory == null || _memory.Length != _size)
            {
                _memory = new byte[_size];

                byte[] bytes;

                if (!_process.ReadBytes(_address, _size, out bytes))
                {
                    _memory = null;
                    return IntPtr.Zero;
                }

                _memory = bytes;
            }

            foreach (SigScanTarget.Signature sig in target.Signatures)
            {
                IntPtr ptr = FindPattern(sig.Pattern, sig.Mask, sig.Offset);
                if (ptr != IntPtr.Zero)
                {
                    if (target.OnFound != null)
                        ptr = target.OnFound(_process, this, ptr);
                    return ptr;
                }
            }

            return IntPtr.Zero;
        }

        unsafe IntPtr FindPattern(byte[] sig, bool[] mask, int finalOffset)
        {
            if (sig.Length != mask.Length)
                throw new ArgumentException("sig length is not equal to mask length.", nameof(sig));

            fixed (byte* mem = _memory, s = sig)
            fixed (bool* m = mask)
            {
                int sigLen = sig.Length;
                int memLen = _memory.Length;

                for (int addr = 0; addr < memLen; addr++)
                {
                    for (int i = 0; i < sigLen; i++)
                    {
                        if (m[i])
                            continue;

                        if (addr + i >= memLen || s[i] != mem[addr + i])
                            goto next;
                    }

                    return _address + (addr + finalOffset);

                next:
                    ;
                }
            }

            return IntPtr.Zero;
        }
    }

    public class SigScanTarget
    {
        public struct Signature
        {
            public byte[] Pattern;
            public bool[] Mask;
            public int Offset;
        }

        public delegate IntPtr OnFoundCallback(Process proc, SignatureScanner scanner, IntPtr ptr);
        public OnFoundCallback OnFound { get; set; }

        private List<Signature> _sigs;
        public ReadOnlyCollection<Signature> Signatures
        {
            get { return _sigs.AsReadOnly(); }
        }

        public SigScanTarget()
        {
            _sigs = new List<Signature>();
        }

        public SigScanTarget(int offset, params string[] signature)
        {
            _sigs = new List<Signature>();
            AddSignature(offset, signature);
        }

        public SigScanTarget(int offset, byte[] binary)
        {
            _sigs = new List<Signature>();

            var emptyMask = new bool[binary.Length];
            _sigs.Add(new Signature { Pattern = binary, Mask = emptyMask, Offset = offset });
        }

        public void AddSignature(int offset, params string[] signature)
        {
            string sigStr = string.Join(string.Empty, signature).Replace(" ", string.Empty);
            if (sigStr.Length % 2 != 0)
                throw new ArgumentException();

            var sigBytes = new List<byte>();
            var sigMask = new List<bool>();

            for (int i = 0; i < sigStr.Length; i += 2)
            {
                byte b;
                if (byte.TryParse(sigStr.Substring(i, 2), NumberStyles.HexNumber, null, out b))
                {
                    sigBytes.Add(b);
                    sigMask.Add(false);
                }
                else
                {
                    sigBytes.Add(0);
                    sigMask.Add(true);
                }
            }

            _sigs.Add(new Signature {
                Pattern = sigBytes.ToArray(),
                Mask = sigMask.ToArray(),
                Offset = offset
            });
        }
    }
}
