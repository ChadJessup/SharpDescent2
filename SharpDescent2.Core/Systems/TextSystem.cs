using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems
{
    public class TextSystem
    {
        private static class Constants
        {
            public const int NumberOfTextStrings = 649;
            public const byte BITMAP_TBL_XOR = 0xD3;

        }

        private readonly ILogger<TextSystem> logger;
        private readonly ILibraryManager library;

        public TextSystem(
            ILogger<TextSystem> logger,
            ILibraryManager library)
        {
            this.logger = logger;
            this.library = library;

            this.Text_string = new(Constants.NumberOfTextStrings);
        }

        public List<string> Text_string { get; }

        public void LoadEncryptedText(Span<byte> bytes)
        {
            int length = bytes.Length;
            int idx = 0;

            int newLineIdx = 0;
            for (int i = 0; i < Constants.NumberOfTextStrings; i++)
            {
                newLineIdx = bytes.IndexOf((byte)'\n');
                var encryptedLine = bytes[0..newLineIdx];

                for (int j = 0; j < encryptedLine.Length; j++)
                {
                    encryptedLine[j] = this.EncodeRotateLeft(encryptedLine, j);
                    encryptedLine[j] ^= Constants.BITMAP_TBL_XOR;
                    encryptedLine[j] = this.EncodeRotateLeft(encryptedLine, j);
                }

                var str = Encoding.ASCII.GetString(encryptedLine);

                this.Text_string.Add(str.Replace("\\n", "\n").Replace("\\t", "\t"));

                bytes = bytes[(newLineIdx + 1)..];
            }
        }

        private byte EncodeRotateLeft(Span<byte> encoded, int idx)
        {
            bool found = false;
            if ((encoded[idx] & 0x80) != 0)
            {
                found = true;
            }

            encoded[idx] = (byte)(encoded[idx] << 1);

            if (found)
            {
                encoded[idx] |= 0x01;
            }

            return encoded[idx];
        }
    }
}
