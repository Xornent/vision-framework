using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference {

    public enum Edit {
        None,
        DeleteRight,
        DeleteLeft,
        InsertDown,
        InsertUp
    }

    public class EditLengthResult {
        public int EditLength { get; set; }

        public int StartX { get; set; }
        public int EndX { get; set; }
        public int StartY { get; set; }
        public int EndY { get; set; }

        public Edit LastEdit { get; set; }
    }
}
