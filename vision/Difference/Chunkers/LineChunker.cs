using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Chunkers {

    public class LineChunker : IChunker {
        private readonly string[] lineSeparators = new[] { "\r\n", "\r", "\n" };
        public static LineChunker Instance { get; } = new LineChunker();

        public string[] Chunk(string text) {
            return text.Split(lineSeparators, StringSplitOptions.None);
        }
    }
}
