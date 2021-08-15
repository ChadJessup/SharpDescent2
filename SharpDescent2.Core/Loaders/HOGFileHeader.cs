namespace SharpDescent2.Core.Loaders;

public readonly struct HOGFileHeader
{
    public readonly string FileName { get; init; }
    public readonly int Length { get; init; }
    public readonly long Offset { get; init; }

    public override string ToString() => $"{this.FileName} @ {this.Offset}";
}
