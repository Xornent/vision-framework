using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    public class ArrayData : DataSection {
        public ArrayData() {
            this.Type = DataSectionType.Array;
        }

        public List<List<Section>> Value { get; set; } = new List<List<Section>>();

        // 输入 (a) (b) (c:...)

        public static ArrayData Parse(string raw) {
            ArrayData data = new ArrayData();
            data.Content = raw.Trim();
            data.Raw ="("+ raw+")";
            Dictionary<string, string> unparsed = new Dictionary<string, string>();

            int bracket = 0;
            int col = 0;
            bool reversed = false;
            string content = "";
            foreach (char ch in raw) {
                if (ch == '`') { reversed = true; col++; continue; }
                if (!reversed) {
                    if (ch == '(') {
                        bracket++; col++;
                        if (bracket > 1)
                            content = content + '(';
                        continue;
                    }
                    if (ch == ')') {
                        bracket--; col++;

                        // 关闭一级标签

                        if (bracket == 0) {
                            unparsed.Add(unparsed.Count.ToString(), content.Trim());
                            content = "";
                        } else { content = content + ")"; }

                        continue;
                    }
                }

                // 这里将字符串附加到当前读取器上

                if(bracket >= 1) {
                    content = content + ch;
                } 

                if (reversed) reversed = false;
                col++;
            }

            ElementSection elem = new ElementSection();
            elem.ParseRawDictionary(unparsed);

            foreach (var item in elem.Parameters) {
                data.Value.Add(item.Value);
            }
            return data;
        }

        public override string ToString() {
            string s = "";
            foreach(var list in this.Value) {
                foreach(var item in list) {
                    s = s + item.ToString();
                }
            }
            return s;
        }

        public static IntegerData count(ArrayData me) {
            string s = me.Value.Count.ToString();
            return new IntegerData() { Content = s, Raw = s, Value = me.Value.Count };
        }

        public static DataSection get(ArrayData me, IntegerData id) {
            if(me.Value[id.Value].Count == 1) {
                if (me.Value[id.Value][0].IsData) {
                    switch ((me.Value[id.Value][0] as DataSection).Type) {
                        case DataSectionType.Array: return (me.Value[id.Value][0] as ArrayData);
                        case DataSectionType.Boolean: return (me.Value[id.Value][0] as BooleanData);
                        case DataSectionType.DateTime: return (me.Value[id.Value][0] as DateTimeData);
                        case DataSectionType.Enumeration: return (me.Value[id.Value][0] as EnumerationData);
                        case DataSectionType.Float: return (me.Value[id.Value][0] as FloatData);
                        case DataSectionType.Integer: return (me.Value[id.Value][0] as IntegerData);
                        case DataSectionType.Text: return (me.Value[id.Value][0] as TextData);
                    }
                    return new TextData("");
                } else
                    return new TextData(me.Value[id.Value][0].Raw);
            } else {
                ArrayData arr = new ArrayData();
                string content = "";
                foreach (var item in me.Value[id.Value]) {
                    content = content + " (" + item.Raw + ")";
                    arr.Value.Add(new List<Section>() { item });
                }
                arr.Content = content;
                arr.Raw = content;
                return arr;
            }
        }

        public static BooleanData contains(ArrayData me, DataSection data, BooleanData includechildren) {
            bool flag = false;
            if (includechildren.Value) {
                foreach (var item in me.Value) {
                    foreach (var i in item) {
                        if (i.IsData)
                            if (DataSection.Equal((i as DataSection), data))
                                flag = true;
                    }
                }
            } else {
                foreach (var item in me.Value) {
                    if (item[0].IsData)
                        if (DataSection.Equal((item[0] as DataSection), data))
                            flag = true;
                }
            }
            return new BooleanData(flag) { Content = flag.ToString(), Raw = flag.ToString() };
        }

        public static ArrayData append (ArrayData me, DataSection data) {
            if (data.Type == DataSectionType.Array) {
                ArrayData arr = ArrayData.Parse(me.Content + data.Content );
                return arr;
            } else {
                ArrayData arr = ArrayData.Parse(me.Content + "(" + data.Content + ")");
                return arr;
            }
        }

        public static ArrayData add(ArrayData me, DataSection data) {
            ArrayData arr = ArrayData.Parse(me.Content + "(" + data.Content + ")");
            return arr;
        }
    }
}
