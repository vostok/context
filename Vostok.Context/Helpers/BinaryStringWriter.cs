using System;
using System.Text;

namespace Vostok.Context.Helpers
{
    internal class BinaryStringWriter
    {
        private byte[] buffer;
        private int offset;
        private int length;

        public BinaryStringWriter(int initialCapacity)
        {
            buffer = new byte[initialCapacity];
        }

        public bool IsEmpty => length == 0;

        public void Clear()
        {
            offset = 0;
            length = 0;
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(buffer, 0, length);
        }

        public unsafe void Write(string value)
        {
            EnsureCapacity(Encoding.UTF8.GetMaxByteCount(value.Length) + sizeof(int));

            var byteCount = Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset + sizeof(int));

            fixed (byte* ptr = &buffer[offset])
                *(int*) ptr = byteCount;

            offset += sizeof(int) + byteCount;

            if (offset > length)
                length = offset;
        }

        private void EnsureCapacity(int neededBytes)
        {
            var remainingBytes = buffer.Length - offset;
            if (remainingBytes >= neededBytes)
                return;

            var newCapacity = buffer.Length + Math.Max(neededBytes - remainingBytes, buffer.Length);
            var newBuffer = new byte[newCapacity];

            Buffer.BlockCopy(buffer, 0, newBuffer, 0, length);

            buffer = newBuffer;
        }
    }
}
