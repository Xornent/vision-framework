using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Builder {

    public class TwoSidedDiff {
        public DiffPane OldText { get; }
        public DiffPane NewText { get; }

        public TwoSidedDiff() {
            OldText = new DiffPane();
            NewText = new DiffPane();
        }
    }
}
