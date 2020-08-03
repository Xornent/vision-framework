using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Markup.Enumerations;

namespace Vision.Markup.Ast {

    public class DataSection : Section {
        public string Content { get; set; }
        public override bool IsData { get => true ; set => base.IsData = value; }

        /// <summary>
        /// 将字符串表达式解析成 Data Section （不包含数组！）
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static DataSection Parse(string content) {

            // 检测是否是 boolean

            string lt = content.ToLower().Trim();
            if (lt == "__true__") return new BooleanData(true) { Raw = content, Content = lt };
            if (lt == "__false__") return new BooleanData(false) { Raw = content, Content = lt };

            // 特殊的，请不要删除上面三行注释，这是解析 bool 的一种思路，但它和使用时的另一种思路冲突了
            // 我们在系统默认变量表中规定 true 为一个真实例， false 为一个伪实例，使用变量组可以
            // 写出直接对 boolean 的引用，例如 @{a # compare | true}， 我们应该使用 @{true} 表示 true 实例，而不是 true.

            // 最开始尝试 int 匹配，再尝试 float 匹配，其中有小数点的强制为 float，
            // 没有小数点的强制为 int， 再尝试 datetime 匹配， 再尝试 enum 匹配，
            // 如果所有的尝试均失败，则转化为 string.

            string trimmed = content.Trim();
            int t_int;
            float t_float;
            DateTime t_datetime;

            bool s_int = int.TryParse(trimmed, out t_int);
            if (s_int) {
                if (trimmed.Contains(".")) return new FloatData() { Raw = trimmed, Value = t_int };
                else return new IntegerData() { Raw = trimmed, Value = t_int };
            }

            bool s_float = float.TryParse(trimmed, out t_float);
            if (s_float) return new FloatData() { Raw = trimmed, Value = t_float };

            bool s_datetime = DateTime.TryParse(trimmed, out t_datetime);
            if (s_datetime) return new DateTimeData() { Raw = trimmed, Value = t_datetime };

            if (trimmed.Contains("|")) {
                string[] enums = trimmed.Split('|');
                if (enums.Count() > 2) return new TextData(trimmed);

                Type t;
                bool success = RegisteredEnumerations.TryGetValue(enums[0].Trim().ToLower(), out t);
                if (success) {
                    object val = null;
                    bool get_val = Enum.TryParse(t, enums[1].Trim().ToUpper(), true, out val);
                    if (get_val) {
                        return new EnumerationData() { Raw = trimmed, Enumeration = t, Value = Convert.ToInt32(val) };
                    }
                }

                return new TextData(trimmed);
            } else {
                return new TextData(trimmed);
            }
        }

        public DataSectionType Type { get; set; } = DataSectionType.Undefined;

        public enum DataSectionType {
            Undefined,
            Array,
            Boolean,
            DateTime,
            Enumeration,
            Integer,
            Float,
            Text
        }

        public static Dictionary<string, Type> RegisteredEnumerations = new Dictionary<string, Type>()
        {
            { "color", typeof(System.Drawing.KnownColor) },
            { "toggle", typeof(Toggle) },
            { "orientionh", typeof(HorizontalOriention) }
        };

        public static bool Equal(DataSection variable, DataSection value) {
            if (variable is BooleanData || value is BooleanData)
                return (variable as BooleanData).Value == (value as BooleanData).Value;

            if (variable is IntegerData ||
               variable is FloatData) {
                if (value is IntegerData ||
                   value is FloatData) {
                    return float.Parse(variable.Raw) == float.Parse(value.Raw);
                } else return false;
            }

            if (variable is DateTimeData && value is DateTimeData) {
                return DateTime.Parse(variable.Raw) == DateTime.Parse(value.Raw);
            }

            if (variable is EnumerationData && value is EnumerationData) {
                if ((variable as EnumerationData).Enumeration ==
                   (value as EnumerationData).Enumeration)
                    return (variable as EnumerationData).Value == (value as EnumerationData).Value;
            }

            if (variable is TextData && value is TextData) {
                if ((variable as TextData).Content.Trim() == (value as TextData).Content.Trim())
                    return true;
            }

            return false;
        }

        public static FloatData sin(FloatData f) {
            return new FloatData((float)Math.Sin(f.Value));
        }
    }
}
