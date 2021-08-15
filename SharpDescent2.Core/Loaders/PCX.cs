using System.Reflection.PortableExecutable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Buffers.Binary;

using static SharpDescent2.Core.Loaders.CFile;
using SixLabors.ImageSharp.ColorSpaces;
using System.Runtime.InteropServices;

namespace SharpDescent2.Core.Loaders;

public enum PCX_ERROR
{
    NONE = 0,
    OPENING = 1,
    NO_HEADER = 2,
    WRONG_VERSION = 3,
    READING = 4,
    NO_PALETTE = 5,
    WRITING = 6,
    MEMORY = 7,
}

public enum BM
{
    LINEAR = 0,
    MODEX = 1,
    SVGA = 2,
    RGB15 = 3,   //5 bits each r,g,b stored at 16 bits
    SVGA15 = 4,
}

public struct PCXHeader
{
    public byte Manufacturer;
    public byte Version;
    public byte Encoding;
    public byte BitsPerPixel;
    public short Xmin;
    public short Ymin;
    public short Xmax;
    public short Ymax;
    public short Hdpi;
    public short Vdpi;
    public Rgb[] ColorMap;
    public byte Reserved;
    public byte Nplanes;
    public short BytesPerLine;
    public byte[] filler;//[60];
}

public class PCX
{
    public static PCX_ERROR pcx_read_bitmap(Span<byte> bytes, ref grs_bitmap bmp, BM bitmap_type, Span<byte> palette)
    {
        int count = 0;
        var fileName = $@"C:\temp\test_{count}.pcx";

        while (File.Exists(fileName))
        {
            fileName = $@"C:\temp\test_{++count}.pcx";
        }

        File.WriteAllBytes(fileName, bytes.ToArray());

        var header = new PCXHeader
        {
            Manufacturer = bytes[0],
            Version = bytes[1],
            Encoding = bytes[2],
            BitsPerPixel = bytes[3],
            Xmin = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(4, sizeof(short))),
            Ymin = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(6, sizeof(short))),
            Xmax = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(8, sizeof(short))),
            Ymax = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(10, sizeof(short))),
            Hdpi = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(12, sizeof(short))),
            Vdpi = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(14, sizeof(short))),
            Reserved = bytes[64],
            Nplanes = bytes[65],
            BytesPerLine = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(66, sizeof(short))),
            filler = bytes.Slice(74, 54).ToArray(),
        };

        var colorMap = bytes.Slice(16, 48);
        header.ColorMap = new Rgb[16];

        for (int i = 0; i < 16; i++)
        {
            var pixel = colorMap.Slice(i * 3, 3);
            header.ColorMap[i] = new Rgb(pixel[0], pixel[1], pixel[2]);
        }

        return PCX_ERROR.NONE;
    }
}
