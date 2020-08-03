using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference {

    public interface IChunker {
        string[] Chunk(string text);
    }
}
