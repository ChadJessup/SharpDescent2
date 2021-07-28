using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.SPIRV;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Sharp.Platform.Video
{
    public class SpriteRenderer
    {
        private readonly List<SpriteInfo> _draws = new List<SpriteInfo>();

        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _textBuffer;
        private DeviceBuffer _orthoBuffer;
        private ResourceLayout _orthoLayout;
        private ResourceSet _orthoSet;
        private ResourceLayout _texLayout;
        private Pipeline _pipeline;
        protected GraphicsDevice gd;

        public List<SpriteInfo> DrawCalls { get; protected set; }
        private Dictionary<SpriteInfo, (Texture, TextureView, ResourceSet)> _loadedImages
            = new Dictionary<SpriteInfo, (Texture, TextureView, ResourceSet)>();
        private ResourceSet _textSet;

        public SpriteRenderer(GraphicsDevice gd)
        {
            this.DrawCalls = this._draws;

            ResourceFactory factory = gd.ResourceFactory;

            this._vertexBuffer = factory.CreateBuffer(new BufferDescription(1000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            this._textBuffer = factory.CreateBuffer(new BufferDescription(QuadVertex.VertexSize, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            this._orthoBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            this._orthoLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("OrthographicProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            this._orthoSet = factory.CreateResourceSet(new ResourceSetDescription(this._orthoLayout, this._orthoBuffer));

            this._texLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SpriteTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SpriteSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            this._pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleStrip,
                new ShaderSetDescription(
                    new VertexLayoutDescription[]
                    {
                        new VertexLayoutDescription(
                            QuadVertex.VertexSize,
                            1,
                            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                            new VertexElementDescription("Size", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                            new VertexElementDescription("Tint", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4_Norm),
                            new VertexElementDescription("Rotation", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1))
                    },
                    factory.CreateFromSpirv(
                        new ShaderDescription(ShaderStages.Vertex, this.LoadShaderBytes("sprite.vert.spv"), "main"),
                        new ShaderDescription(ShaderStages.Fragment, this.LoadShaderBytes("sprite.frag.spv"), "main"),
                        this.GetCompilationOptions(factory))),
                new[] { this._orthoLayout, this._texLayout },
                gd.MainSwapchain.Framebuffer.OutputDescription));

            this.gd = gd;
        }

        private CrossCompileOptions GetCompilationOptions(ResourceFactory factory)
        {
            return new CrossCompileOptions(false, false, new SpecializationConstant[]
            {
                new SpecializationConstant(0, false)
            });
        }

        private byte[] LoadShaderBytes(string name) => VeldridVideoManager.ReadEmbeddedAssetBytes(name);

        public void AddSprite(SixLabors.ImageSharp.Point loc, Texture texture, string spriteName, RgbaByte? tint = null, float rotation = 0f)
            => this.AddSprite(new Rectangle(loc.X, loc.Y, (int)texture.Width, (int)texture.Height), texture, spriteName, tint, rotation);

        public void AddSprite(Rectangle rectangle, Texture texture, string spriteName, RgbaByte? tint = null, float rotation = 0f)
        {
            if (tint is null)
            {
                tint = RgbaByte.White;
            }

            this.DrawCalls.Add(new SpriteInfo()
            {
                SpriteName = spriteName,
                Texture = texture,
                Quad = new QuadVertex(rectangle.ToPoint(), new(rectangle.Size.Width, rectangle.Size.Height), tint.Value, rotation),
            });
        }

        private ResourceSet Load(GraphicsDevice gd, SpriteInfo spriteInfo)
        {
            if (!this._loadedImages.TryGetValue(spriteInfo, out (Texture, TextureView, ResourceSet) ret))
            {
                Texture? tex = spriteInfo.Texture;

                if (!string.IsNullOrWhiteSpace(spriteInfo.SpriteName) && spriteInfo.Texture is null)
                {
                    string texPath = Path.Combine(AppContext.BaseDirectory, "Assets", spriteInfo.SpriteName);
                    tex = new ImageSharpTexture(texPath, false).CreateDeviceTexture(gd, gd.ResourceFactory);
                }

                TextureView view = gd.ResourceFactory.CreateTextureView(tex);
                ResourceSet set = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                    this._texLayout,
                    view,
                    gd.PointSampler));

                ret = (tex, view, set);
                this._loadedImages.Add(spriteInfo, ret);
            }

            return ret.Item3;
        }

        public virtual void Draw(GraphicsDevice gd, CommandList cl, bool clearCalls = true)
        {
            if (this.DrawCalls.Count == 0)
            {
                return;
            }

            float width = gd.MainSwapchain.Framebuffer.Width;
            float height = gd.MainSwapchain.Framebuffer.Height;
            gd.UpdateBuffer(
                this._orthoBuffer,
                0,
                Matrix4x4.CreateOrthographicOffCenter(0, width, 0, height, 0, 1));

            this.EnsureBufferSize(gd, (uint)this.DrawCalls.Count * QuadVertex.VertexSize);
            MappedResourceView<QuadVertex> writemap = gd.Map<QuadVertex>(this._vertexBuffer, MapMode.Write);
            for (int i = 0; i < this.DrawCalls.Count; i++)
            {
                writemap[i] = this.DrawCalls[i].Quad;
            }

            gd.Unmap(this._vertexBuffer);

            cl.SetPipeline(this._pipeline);
            cl.SetVertexBuffer(0, this._vertexBuffer);
            cl.SetGraphicsResourceSet(0, this._orthoSet);

            for (int i = 0; i < this.DrawCalls.Count;)
            {
                uint batchStart = (uint)i;

                ResourceSet rs;

                string spriteName = this.DrawCalls[i].SpriteName;
                rs = this.Load(gd, this.DrawCalls[i]);

                cl.SetGraphicsResourceSet(1, rs);
                uint batchSize = 0;
                do
                {
                    i += 1;
                    batchSize += 1;
                }
                while (i < this.DrawCalls.Count && this.DrawCalls[i].SpriteName == spriteName);

                cl.Draw(4, batchSize, 0, batchStart);
            }

            if (clearCalls)
            {
                this.DrawCalls.Clear();
            }
        }

        public virtual void RenderText(GraphicsDevice gd, CommandList cl, TextureView textureView, Vector2 pos)
        {
            cl.SetPipeline(this._pipeline);
            cl.SetVertexBuffer(0, this._textBuffer);
            cl.SetGraphicsResourceSet(0, this._orthoSet);

            if (this._textSet == null)
            {
                this._textSet = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(this._texLayout, textureView, gd.PointSampler));
            }

            cl.SetGraphicsResourceSet(1, this._textSet);
            Texture target = textureView.Target;
            cl.UpdateBuffer(this._textBuffer, 0, new QuadVertex(pos, new Vector2(target.Width, target.Height)));
            cl.Draw(4, 1, 0, 0);
        }

        private void EnsureBufferSize(GraphicsDevice gd, uint size)
        {
            if (this._vertexBuffer.SizeInBytes < size)
            {
                this._vertexBuffer.Dispose();
                this._vertexBuffer = gd.ResourceFactory.CreateBuffer(
                    new BufferDescription(size, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }
        }

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
}
