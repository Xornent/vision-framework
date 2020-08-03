using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class FloatData : DataSection {
        public FloatData() {
            this.Type = DataSectionType.Float;
        }

        public FloatData(float i) {
            this.Value = i;
            this.Raw = i.ToString();
            this.Content = i.ToString();
            this.Type = DataSectionType.Integer;
        }

        public float Value { get; set; }

        public override string ToString() {
            return this.Value.ToString();
        }

        public static FloatData increment(FloatData me) {
            return new FloatData(me.Value + 1);
        }

        public static FloatData decrement(FloatData me) {
            return new FloatData(me.Value - 1);
        }

        public static FloatData add(FloatData me, IntegerData data) {
            return new FloatData(me.Value + data.Value);
        }

        public static FloatData add(FloatData me, FloatData data) {
            return new FloatData(me.Value + data.Value);
        }

        public static FloatData negative(FloatData me) {
            return new FloatData(-me.Value);
        }

        public static FloatData minus(FloatData me, IntegerData data) {
            return new FloatData(me.Value - data.Value);
        }

        public static FloatData minus(FloatData me, FloatData data) {
            return new FloatData(me.Value - data.Value);
        }

        public static FloatData multiply(FloatData me, IntegerData data) {
            return new FloatData(me.Value * data.Value);
        }

        public static FloatData multiply(FloatData me, FloatData data) {
            return new FloatData(me.Value * data.Value);
        }

        public static IntegerData dividequo(FloatData me, IntegerData data) {
            return new IntegerData((int)Math.Floor(me.Value / data.Value));
        }

        public static IntegerData dividequo(FloatData me, FloatData data) {
            return new IntegerData((int)Math.Floor(me.Value / data.Value));
        }

        public static FloatData divide(FloatData me, FloatData data) {
            return new FloatData(me.Value / data.Value);
        }

        public static FloatData divide(FloatData me, IntegerData data) {
            return new FloatData(me.Value / (float)data.Value);
        }
    }
}
