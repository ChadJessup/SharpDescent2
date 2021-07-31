#pragma warning disable xUnit1004 // Test methods should not be skipped
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDescent2.Core.Loaders;
using Xunit;

namespace SharpDescent2.Core.Tests.Loaders
{
    public class HOGArchiveTests
    {

        [Fact]
        public void LoadHOGArchiveTest()
        {
            var hogPath = @"C:\Descent 2\descent2.hog";
            var hog = HOGArchive.LoadFile(hogPath);

            Assert.Equal(97, hog.NumberOfFiles);
        }

        [Fact(Skip = "Use to write data files")]
        public async Task WriteAllFilesToDirectory()
        {
            var hogPath = @"C:\Descent 2\descent2.hog";
            var hog = HOGArchive.LoadFile(hogPath);
            var outputDirectory = @"C:\D2Temp";

            foreach (var hogFile in hog.FileHeaders)
            {
                await hog.WriteFile(hogFile, outputDirectory);
            }
        }
    }
}

#pragma warning restore xUnit1004 // Test methods should not be skipped
