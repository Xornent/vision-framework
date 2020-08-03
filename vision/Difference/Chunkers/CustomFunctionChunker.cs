using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Chunkers {

    public class CustomFunctionChunker : IChunker {
        private readonly Func<string, string[]> customChunkerFunc;

        public CustomFunctionChunker(Func<string, string[]> customChunkerFunc) {
            if (customChunkerFunc == null) throw new ArgumentNullException(nameof(customChunkerFunc));
            this.customChunkerFunc = customChunkerFunc;
        }

        public string[] Chunk(string text) {
            return customChunkerFunc(text);
        }
    }
}
