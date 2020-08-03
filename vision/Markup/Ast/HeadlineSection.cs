﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class HeadlineSection : SegmentSection {
        public HeadlineSection(int level, string text) {
            this.Level = level;
            this.Children.Add(new TextData(text));
        }

        public int Level { get; set; }

        public override string ToString() {
            string html = "<h" + Level + ">";

            if (this.HasChildren)
                foreach (var v in this.Children) {
                    html = html + "\n" + v.ToString();
                }
            return html + "\n</h" + Level + ">";
        }
    }
}
