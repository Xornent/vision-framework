using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {
    public class BooleanData : DataSection {
        public BooleanData(bool data) {
            this.Value = data;
            this.Type = DataSectionType.Boolean;
            this.Raw = "__" + data.ToString().ToLower() + "__";
            this.Content = "__" + data.ToString().ToLower() + "__";
        }

        public bool Value { get; set; }

        public static BooleanData not(BooleanData me) {
            return new BooleanData(!me.Value);
        }

        public static BooleanData or(BooleanData me, BooleanData value) {
            return new BooleanData(me.Value || value.Value);
        }

        public static BooleanData nand(BooleanData me, BooleanData value) {
            return new BooleanData(!(me.Value && value.Value));
        }

        public static BooleanData xor(BooleanData me, BooleanData value) {
            if (me.Value == value.Value) return new BooleanData(false);
            else return new BooleanData(true);
        }

        public static BooleanData xnor(BooleanData me, BooleanData value) {
            if (me.Value == value.Value) return new BooleanData(true);
            else return new BooleanData(false);
        }
    }
}
