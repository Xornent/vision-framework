using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class DateTimeData : DataSection {
        public DateTimeData() {
            this.Type = DataSectionType.DateTime;
        }

        public DateTime Value { get; set; }

        public override string ToString() {
            return Value.ToString();
        }
    }
}
