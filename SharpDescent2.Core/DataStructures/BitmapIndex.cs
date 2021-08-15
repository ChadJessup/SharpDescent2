namespace SharpDescent2.Core.DataStructures;

public readonly struct BitmapIndex
{
    public readonly ushort Index { get; init; }

    public override string ToString() => this.Index.ToString();
}
