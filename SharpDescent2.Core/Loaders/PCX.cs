using System.Reflection.PortableExecutable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Buffers.Binary;

using static SharpDescent2.Core.Loaders.CFile;
using SixLabors.ImageSharp.ColorSpaces;
using System.Runtime.InteropServices;
using SharpDescent2.Core.Systems;
using System.Diagnostics;

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
    public static PCX_ERROR pcx_read_bitmap(Span<byte> bytes, ref grs_bitmap bmp, BM bitmap_type, Span<byte> palette, GraphicsSystem gr)
    {
        var header = ExtractHeader(bytes);

        TryExtractExtendedPalette(bytes, out Rgba32[] extendedPalette);

        if ((header.Manufacturer != 10)
            || (header.Encoding != 1)
            || (header.Nplanes != 1)
            || (header.BitsPerPixel != 8)
            || (header.Version != 5))
        {
            return PCX_ERROR.WRONG_VERSION;
        }

        // Find the size of the image
        int xsize = header.Xmax - header.Xmin + 1;
        int ysize = header.Ymax - header.Ymin + 1;
        int uncompressedIndex = 0;
        byte data;
        int length;

        Span<Rgba32> uncompressedPixels = new Rgba32[xsize * ysize + 1];
        var rawCompressedImage = bytes[(74 + 54)..^768];

        if (bitmap_type == BM.LINEAR)
        {
            if (bmp.bm_data is null)
            {
                bmp.bm_data = new Image<Rgba32>(xsize, ysize);
                bmp.bm_w = bmp.bm_rowsize = xsize;
                bmp.bm_h = ysize;
                bmp.bm_type = bitmap_type;
            }
        }

        if (bmp.bm_type == BM.LINEAR)
        {
            for (int offset = 0; offset < rawCompressedImage.Length;)
            {
                data = rawCompressedImage[offset];

                // RLE
                if ((data & 0xC0) == 0xC0)
                {
                    length = data & 0x3F;
                    data = rawCompressedImage[offset + 1];

                    var pixel = extendedPalette[data];
                    var rleSpan = uncompressedPixels.Slice(uncompressedIndex, length);

                    rleSpan.Fill(pixel);

                    offset += 2;
                    uncompressedIndex += length;
                }
                else
                {
                    var pixel = extendedPalette[data];
                    uncompressedPixels[uncompressedIndex++] = pixel;

                    offset++;
                }
            }
        }
        else
        {
            for (int index = 0; index < rawCompressedImage.Length; index++)
            {
                data = rawCompressedImage[index++];

                if ((data & 0xC0) == 0xC0)
                {
                    length = data & 0x3F;
                    data = rawCompressedImage[index++];

                    for (int i = 0; i < length; i++)
                    {
                        // gr.bm_pixel(ref bmp, col + i, index, data);
                    }

                    uncompressedIndex += length;
                }
                else
                {
                    // gr.bm_pixel(ref bmp, col, index, data);
                    // gr_bm_pixel(bmp, col, row, data);
                    // col++;
                }
            }
        }

        // int linearIdx = 0;
        // for (int rowIdx = 0; rowIdx < bmp.bm_data.Height; rowIdx++)
        // {
        //     var row = bmp.bm_data.GetPixelRowSpan(rowIdx);
        // 
        //     for (int col = 0; col < bmp.bm_data.Width; col++)
        //     {
        //         row[col] = extendedPalette[uncompressedBytes[linearIdx++]];
        //     }
        // }
        var idx = 0;
        for (int y = 0; y < bmp.bm_data.Height; y++)
        {
            var pixelSpan = bmp.bm_data.GetPixelRowSpan(y);
            var pixels = uncompressedPixels.Slice(idx, pixelSpan.Length);
            pixels.CopyTo(pixelSpan);

            idx += pixelSpan.Length;
        }

        return PCX_ERROR.NONE;
    }

    private static bool TryExtractExtendedPalette(Span<byte> bytes, out Rgba32[] extendedPalette)
    {
        extendedPalette = new Rgba32[256];

        if (bytes[^769] == 12)
        {
            var extendedPaletteSpan = bytes[^768..];
            for (int i = 0; i < 256; i++)
            {
                var pixel = extendedPaletteSpan.Slice(i * 3, 3);

                extendedPalette[i] = new Rgba32(pixel[0], pixel[1], pixel[2]);
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    private static PCXHeader ExtractHeader(Span<byte> bytes)
    {
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

        return header;
    }
}
