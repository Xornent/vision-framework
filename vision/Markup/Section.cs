using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup {

    public class Section {
        public string Raw { get; set; }
        public virtual bool IsData { get; set; } = false;
    }
}
