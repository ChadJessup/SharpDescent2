namespace SharpDescent2.Core;

public struct grs_bitmap
{
    public short bm_x;           // Offset from parent's origin
    public short bm_y;
    public short bm_w;           // width
    public short bm_h;           // height
    public byte bm_type;         // 0=Linear, 1=ModeX, 2=SVGA
    public byte bm_flags;        // bit 0 on means it has transparency.
                                 // bit 1 on means it has supertransparency
                                 // bit 2 on means it doesn't get passed through lighting.
    public short bm_rowsize;     // unsigned char offset to next row
    public Memory<byte> bm_data;      // ptr to pixel data...
                                 //   Linear = *parent+(rowsize*y+x)
                                 //   ModeX = *parent+(rowsize*y+x/4)
                                 //   SVGA = *parent+(rowsize*y+x)
    public ushort bm_handle;     // for application.  initialized to 0
    public byte avg_color;       // Average color of all pixels in texture map.
    public byte unused;          // to 4-byte align.
}
