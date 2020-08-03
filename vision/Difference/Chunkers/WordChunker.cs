using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Chunkers {

    public class WordChunker : DelimiterChunker {
        private static char[] WordSeparaters { get; } = { ' ', '\t', '.', '(', ')', '{', '}', ',', '!', '?', ';' };
        public static WordChunker Instance { get; } = new WordChunker();

        public WordChunker() : base(WordSeparaters) { }
    }
}
