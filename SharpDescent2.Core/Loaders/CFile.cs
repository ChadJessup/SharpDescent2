using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpDescent2.Core.Loaders;

public static class CFile
{
    public static T[] cfread<T>(int number, Span<byte> bytes)
        where T : struct
    {
        return MemoryMarshal.Cast<byte, T>(bytes).ToArray();
    }

    public static T cfread<T>(Stream stream)
        where T : struct
    {
        var totalSize = Unsafe.SizeOf<T>();

        using var buffer = MemoryPool<byte>.Shared.Rent(minBufferSize: totalSize);
        stream.Read(buffer.Memory.Span.Slice(0, totalSize));

        return MemoryMarshal.Cast<byte, T>(buffer.Memory.Span.Slice(0, totalSize)).ToArray().First();
    }

    public static T[] cfread<T>(int number, Stream stream)
        where T : struct
    {
        var size = Unsafe.SizeOf<T>();

        var totalSize = size * number;
        using var buffer = MemoryPool<byte>.Shared.Rent(minBufferSize: totalSize);
        stream.Read(buffer.Memory.Span.Slice(0, totalSize));

        return MemoryMarshal.Cast<byte, T>(buffer.Memory.Span.Slice(0, totalSize))
            .ToArray();
    }
}
