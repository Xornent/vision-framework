using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference.Builder {

    public class DiffPane {
        public List<DiffPiece> Lines { get; }

        public bool HasDifferences {
            get { return Lines.Any(x => x.Type != ChangeType.Unchanged); }
        }

        public DiffPane() {
            Lines = new List<DiffPiece>();
        }
    }
}
