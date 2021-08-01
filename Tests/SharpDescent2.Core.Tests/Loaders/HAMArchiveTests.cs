using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDescent2.Core.Loaders;
using Xunit;

namespace SharpDescent2.Core.Tests.Loaders
{
    public class HAMArchiveTests
    {
        [Fact]
        public void LoadHAMArchiveTest()
        {
            var hamPath = @"C:\Descent 2\descent2.ham";
            var ham = HAMArchive.LoadFile(hamPath);

            // Assert.Equal(97, ham.NumberOfFiles);

        }
    }
}
