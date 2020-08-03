using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class IntegerData : DataSection {
        public IntegerData() {
            this.Type = DataSectionType.Integer;
        }

        public IntegerData(int i) {
            this.Value = i;
            this.Raw = i.ToString();
            this.Content = i.ToString();
            this.Type = DataSectionType.Integer;
        }

        public int Value { get; set; }

        public override string ToString() {
            return this.Value.ToString();
        }

        public static IntegerData increment(IntegerData me) {
            return new IntegerData(me.Value + 1);
        }

        public static IntegerData decrement(IntegerData me) {
            return new IntegerData(me.Value - 1);
        }

        public static IntegerData add(IntegerData me, IntegerData data) {
            return new IntegerData(me.Value + data.Value);
        }

        public static FloatData add(IntegerData me, FloatData data) {
            return new FloatData(me.Value + data.Value);
        }

        public static IntegerData negative (IntegerData me) {
            return new IntegerData(-me.Value);
        }

        public static IntegerData minus(IntegerData me, IntegerData data) {
            return new IntegerData(me.Value - data.Value);
        }

        public static FloatData minus(IntegerData me, FloatData data) {
            return new FloatData(me.Value - data.Value);
        }

        public static IntegerData multiply(IntegerData me, IntegerData data) {
            return new IntegerData(me.Value * data.Value);
        }

        public static FloatData multiply(IntegerData me, FloatData data) {
            return new FloatData(me.Value * data.Value);
        }

        public static IntegerData dividequo(IntegerData me, IntegerData data) {
            return new IntegerData(me.Value / data.Value);
        }

        public static IntegerData dividequo(IntegerData me, FloatData data) {
            return new IntegerData((int)Math.Floor(me.Value / data.Value));
        }

        public static FloatData divide(IntegerData me, FloatData data) {
            return new FloatData(me.Value / data.Value);
        }

        public static FloatData divide(IntegerData me, IntegerData data) {
            return new FloatData(me.Value / (float)data.Value);
        }
    }
}
