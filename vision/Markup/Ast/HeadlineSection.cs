using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class HeadlineSection : SegmentSection {
        public HeadlineSection(int level, string text, string loc) {
            this.Level = level;
            this.Raw = text;
            this.Children.Add(new TextData(text));
            this.Location = loc;
        }

        public int Level { get; set; }
        public string Location { get; set; }

        public override string ToString() {
            string html;
            if (!string.IsNullOrEmpty(Location))
                html = "<h" + Level + " id='title-" + Location + "'>";
            else
                html = "<h" + Level + ">";
            if (this.HasChildren)
                foreach (var v in this.Children) {
                    html = html + "\n" + v.ToString();
                }
            return html + "\n</h" + Level + ">";
        }
    }
}
