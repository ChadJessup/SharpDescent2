#pragma warning disable xUnit1004 // Test methods should not be skipped
using SharpDescent2.Core.Loaders;
using SixLabors.ImageSharp;
using Xunit;

namespace SharpDescent2.Core.Tests.Loaders
{
    public class PCXTests
    {
        [Fact]
        public async Task DecodesPCX()
        {
            var hogPath = @"C:\Descent 2\descent2.hog";
            var hog = HOGArchive.LoadFile(hogPath);

            var pcx = hog.FileHeaders.First(fh => fh.FileName.EndsWith("pcx"));
            var file = await hog.ReadFile(pcx);

            var bmp = new grs_bitmap();
            PCX.pcx_read_bitmap(file.Span, ref bmp, BM.LINEAR, null, null);

            Assert.Equal(320, bmp.bm_data.Width);
            Assert.Equal(200, bmp.bm_data.Height);
        }

        [Fact(Skip = "Dumps all pcx files")]
        public async Task WriteAllPCXFromHOGs()
        {
            var hogPath = @"C:\Descent 2\descent2.hog";
            var hog = HOGArchive.LoadFile(hogPath);
            Directory.CreateDirectory(@"C:\temp\pcxs");

            var pcxs = hog.FileHeaders
                .Where(fh => fh.FileName.EndsWith("pcx"))
                .ToList();

            foreach (var pcx in pcxs)
            {
                var file = await hog.ReadFile(pcx);

                var bmp = new grs_bitmap();
                PCX.pcx_read_bitmap(file.Span, ref bmp, BM.LINEAR, null, null);

                bmp.bm_data.Save(@$"C:\temp\pcxs\{Path.GetFileNameWithoutExtension(pcx.FileName)}.png");
            }
        }
    }
}

#pragma warning restore xUnit1004 // Test methods should not be skipped
