using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Chunkers {

    public class CharacterChunker : IChunker {
        public static CharacterChunker Instance { get; } = new CharacterChunker();

        public string[] Chunk(string text) {
            var s = new string[text.Length];
            for (int i = 0; i < text.Length; i++) s[i] = text[i].ToString();
            return s;
        }
    }
}
