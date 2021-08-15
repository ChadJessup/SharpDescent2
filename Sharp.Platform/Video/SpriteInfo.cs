using Veldrid;

namespace Sharp.Platform.Video
{
    public struct SpriteInfo
    {
        public SpriteInfo(Texture texture, QuadVertex quad)
        {
            this.Texture = texture;
            this.Quad = quad;
            this.SpriteName = string.Empty;
        }

        public SpriteInfo(string spriteName, QuadVertex quad)
        {
            this.SpriteName = spriteName;
            this.Quad = quad;
            this.Texture = null;
        }

        public Texture? Texture { get; init; }
        public string SpriteName { get; init; }
        public QuadVertex Quad { get; init; }

        public override bool Equals(object? obj)
        {
            if (obj is not SpriteInfo other)
            {
                return false;
            }

            return this.SpriteName.Equals(other.SpriteName, StringComparison.OrdinalIgnoreCase)
                && this.Quad.Equals(other.Quad);
        }

        public override int GetHashCode() => HashCode.Combine(this.SpriteName, this.Quad);
        public override string ToString() => $"{this.SpriteName}-{this.Quad}";
    }
}
