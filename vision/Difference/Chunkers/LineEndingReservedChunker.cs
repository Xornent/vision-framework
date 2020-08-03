using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Chunkers {

    public class LineEndingsPreservingChunker : IChunker {
        private readonly string[] emptyArray = new string[0];
        public static LineEndingsPreservingChunker Instance { get; } = new LineEndingsPreservingChunker();

        public string[] Chunk(string text) {
            if (string.IsNullOrEmpty(text))
                return emptyArray;

            var output = new List<string>();
            var lastCut = 0;
            for (var currentPosition = 0; currentPosition < text.Length; currentPosition++) {
                char ch = text[currentPosition];
                switch (ch) {
                    case '\n':
                    case '\r':
                        currentPosition++;
                        if (ch == '\r' && currentPosition < text.Length && text[currentPosition] == '\n') {
                            currentPosition++;
                        }
                        var str = text.Substring(lastCut, currentPosition - lastCut);
                        lastCut = currentPosition;
                        output.Add(str);
                        break;
                    default:
                        continue;
                }
            }

            if (lastCut != text.Length) {
                var str = text.Substring(lastCut, text.Length - lastCut);
                output.Add(str);
            }

            return output.ToArray();
        }
    }
}
