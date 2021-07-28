using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SharpDescent2.Core.Systems
{
    public class TextSystem
    {
        private readonly ILogger<TextSystem> logger;

        public TextSystem(ILogger<TextSystem> logger)
        {
            this.logger = logger;
        }
    }
}
