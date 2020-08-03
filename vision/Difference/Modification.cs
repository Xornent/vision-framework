using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference {

    public class Modification {
        public Modification(string str) {
            RawData = str;
        }

        public int[] HashedPieces { get; set; }
        public string RawData { get; }
        public bool[] Modifications { get; set; }
        public string[] Pieces { get; set; }
    }
}
