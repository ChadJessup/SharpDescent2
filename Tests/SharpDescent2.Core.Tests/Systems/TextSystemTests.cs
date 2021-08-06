using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;
using SharpDescent2.Core.Systems;
using Xunit;

namespace SharpDescent2.Core.Tests.Systems
{
    public class TextSystemTests
    {
        [Fact]
        public async Task LoadEncryptedText()
        {
            var hogPath = @"C:\Descent 2\descent2.hog";
            var hog = HOGArchive.LoadFile(hogPath);

            var descentTxb = hog.FileHeaders.First(fh => fh.FileName.Equals("descent.txb", StringComparison.OrdinalIgnoreCase));
            var txbStream = await hog.ReadFile(descentTxb);

            var ts = new TextSystem(
                Mock.Of<ILogger<TextSystem>>(),
                Mock.Of<ILibraryManager>());

            ts.LoadEncryptedText(txbStream.Span);
        }
    }
}
