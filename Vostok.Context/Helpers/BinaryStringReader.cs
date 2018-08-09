using System.Text;

namespace Vostok.Context.Helpers
{
    internal class BinaryStringReader
    {
        private readonly byte[] buffer;
        private int position;

        public BinaryStringReader(byte[] buffer, int position = 0)
        {
            this.buffer = buffer;
            this.position = position;
        }

        public bool HasDataLeft => position < buffer.Length;

        public unsafe string Read()
        {
            int size;

            fixed (byte* ptr = &buffer[position])
                size = *(int*)ptr;

            var result = Encoding.UTF8.GetString(buffer, position + sizeof(int), size);

            position += sizeof(int) + size;

            return result;
        }
    }
}