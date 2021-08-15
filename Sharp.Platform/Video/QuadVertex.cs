using System.Numerics;
using Veldrid;

namespace Sharp.Platform.Video
{
    public struct QuadVertex
    {
        public const uint VertexSize = 24;

        public Vector2 Position;
        public Vector2 Size;
        public RgbaByte Tint;
        public float Rotation;

        public QuadVertex(Vector2 position, Vector2 size) : this(position, size, RgbaByte.White, 0f) { }
        public QuadVertex(Vector2 position, Vector2 size, RgbaByte tint, float rotation)
        {
            this.Position = position;
            this.Size = size;
            this.Tint = tint;
            this.Rotation = rotation;
        }

        public override int GetHashCode() => HashCode.Combine(this.Position, this.Size, this.Tint, this.Rotation);
        public override bool Equals(object? obj)
        {
            if (obj is not QuadVertex quad)
            {
                return false;
            }

            return this.Position.Equals(quad.Position)
                && this.Size.Equals(quad.Size)
                && this.Tint.Equals(quad.Tint)
                && this.Rotation.Equals(quad.Rotation);
        }

        public override string ToString() => $"{this.Position}:{this.Size}";
    }
}
