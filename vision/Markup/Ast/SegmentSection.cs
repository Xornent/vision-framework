using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class SegmentSection : Section {
        public List<Section> Children = new List<Section>();

        public bool HasChildren {
            get {
                return Children.Count > 0;
            }
        }
    }
}
