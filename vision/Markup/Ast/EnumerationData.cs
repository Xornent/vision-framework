using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class EnumerationData : DataSection {
        public EnumerationData() {
            this.Type = DataSectionType.Enumeration;
        }

        public Type Enumeration { get; set; }
        public int Value { get; set; }

        public override string ToString() {
            return "";
        }
    }
}
