using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class ParagraphSection : SegmentSection {
        public override string ToString() {
            string html = "<p>";

            if (this.HasChildren)
                foreach (var v in this.Children) {
                    html = html + "\n" + v.ToString();
                }
            return html + "\n</p>";
        }
    }
}
