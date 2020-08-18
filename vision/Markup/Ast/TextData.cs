using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization.Internal;
using System.Reflection;

namespace Vision.Markup.Ast {

    public class TextData : DataSection {
        
        public TextData(string con) {
            this.Content = con;
            this.Raw = con;
            this.Type = DataSectionType.Text;
        }

        // 由语义规定，合法的变量引用只能出现在 TextData 中，形式为 @{variableName}，因此
        // 在构造 TextData 时我们一并传入环境变量域，查找到所有的变量引用，并直接替换，得到新的
        // TextData 对象 @{month} ddd @{day}

        public void UpdateVariables(MarkupDocument doc) {
            string input = this.Raw;
            string pattern = @"@\{(.*?)\}";

            while (Regex.IsMatch(this.Content, pattern)) {
                string result = Regex.Replace(this.Content, pattern, (match) => {
                    string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 3, 1);

                    // 这时获取到的可以是变量属性组，其标志是 | 和 # 字符

                    if (val.Contains("#")) {
                        return GetOperationVariables(val, doc).Raw;
                    }

                    DataSection data = doc.GetVariable(val);
                    if (data != null) {
                        if (data is TextData)
                            return data.Content;
                        else return data.Raw;
                    } else return "<code>" + val + " 未声明</code>";
                });

                this.Content = result;
            }
        }

        // 变量组是一个关于某变量，提供一个方法名和一组参数变量，而返回计算结果的语句 （参见语言标准）

        // 常规下，一个变量组也是由 @{ ... } 包裹，而在里面写出的不是一个变量名，而是一组变量名，
        // 所有关于变量取值和变量组解析的步骤全部见 TextData 的静态方法 GetVariables(string, Dictionary),
         
        // 变量组的格式为 @{ object # method | parametersList }，假设我们为某种变量定义方法 A， 其在
        // C# 语言的定义是 static DataSection A (DataSection obj, DataSection obj_b, DataSection obj_c)
        // 它可以写作： @{ obj # A | obj_b, obj_c }

        public static DataSection GetOperationVariables(string operation, MarkupDocument doc) {
            operation = operation
                .Replace("`#", "$<sharp>")
                .Replace("`|", "$<pipe>")
                .Replace("`,", "$<comma>")
                .Replace("`:", "$<colon>");

            if (operation.Contains(":")) {
                string[] paramSections = operation.Split(':');
                string[] props = paramSections[0].Split('#');
                string[] param = paramSections[1].Split(',');

                // 获取当前对象的指定名称的静态方法
                // 在 VML 中，所有的方法名是全小写的，如果没有也会被自动转换为小写

                List<DataSection> par = new List<DataSection>();
                List<Type> paramtypes = new List<Type>();
                var type = typeof(DataSection);

                if (!string.IsNullOrWhiteSpace(props[0])) {
                    string varname = props[0].Trim()
                        .Replace("$<sharp>", "#")
                        .Replace("$<pipe>","|")
                        .Replace("$<comma>", ",")
                        .Replace("$<colon>", ":");
                    DataSection obj = doc.GetVariable(varname);
                    par.Add(obj);
                    type = obj.GetType();
                    paramtypes.Add(obj.GetType());
                }

                foreach (var name in param) {
                    par.Add(DataSection.Parse( doc.GetVariable(name.Trim()).Content.Trim()));
                    paramtypes.Add(par.Last().GetType());
                }

                MethodInfo method;
                if (type != typeof(IntegerData) && type != typeof(FloatData))
                    method = type.GetMethod(props[1].Trim().ToLower());
                else
                    method = type.GetMethod(props[1].Trim().ToLower(), paramtypes.ToArray());
                DataSection result = (DataSection)method.Invoke(null, par.ToArray());

                return result;
            } else {
                string[] props = operation.Split('#');

                List<DataSection> par = new List<DataSection>();
                List<Type> paramtypes = new List<Type>();
                var type = typeof(DataSection);

                if (!string.IsNullOrWhiteSpace(props[0])) {
                    DataSection obj = doc.GetVariable(props[0].Trim());
                    par.Add(obj);
                    type = obj.GetType();
                    paramtypes.Add(obj.GetType());
                }

                MethodInfo method;
                if (type != typeof(IntegerData) && type != typeof(FloatData))
                    method = type.GetMethod(props[1].Trim().ToLower());
                else
                    method = type.GetMethod(props[1].Trim().ToLower(), paramtypes.ToArray());

                if (!string.IsNullOrWhiteSpace(props[0]))
                    return (DataSection)method.Invoke(null, par.ToArray());
                else return (DataSection)method.Invoke(null, null);
            }
        }

        public override string ToString() {
            return this.Content;
        }

        public static TextData remove(TextData me, IntegerData start, IntegerData offset) {
            return new TextData(me.Content.Remove(start.Value, offset.Value));
        }

        public static TextData removefrom(TextData me, IntegerData start) {
            return new TextData(me.Content.Remove(start.Value));
        }

        public static TextData replace(TextData me, TextData src, IntegerData dest) {
            return new TextData(me.Content.Replace(src.Content, dest.Content));
        }

        public static BooleanData contains(TextData me, TextData cont) {
            return new BooleanData(me.Content.Contains(cont.Content));
        }

        public static BooleanData startwith(TextData me, TextData check) {
            return new BooleanData(me.Content.StartsWith(check.Content));
        }

        public static BooleanData endwith(TextData me, TextData check) {
            return new BooleanData(me.Content.EndsWith(check.Content));
        }

        public static TextData trimstart(TextData me, TextData check) {
            return new TextData(me.Content.TrimStart(check.Content[0]));
        }

        public static TextData trimend(TextData me, TextData check) {
            return new TextData(me.Content.TrimEnd(check.Content[0]));
        }

        public static TextData trimstartspace(TextData me) {
            return new TextData(me.Content.TrimStart());
        }

        public static TextData trimendspace(TextData me) {
            return new TextData(me.Content.TrimEnd());
        }

        public static TextData trim(TextData me) {
            return new TextData(me.Content.Trim());
        }

        public static TextData upper(TextData me) {
            return new TextData(me.Content.ToUpper());
        }

        public static TextData lower(TextData me) {
            return new TextData(me.Content.ToLower());
        }

        public static TextData upperinvariant(TextData me) {
            return new TextData(me.Content.ToUpperInvariant());
        }

        public static TextData lowerinvariant(TextData me) {
            return new TextData(me.Content.ToLowerInvariant());
        }

        public static TextData get(TextData me, IntegerData i) {
            return new TextData(new string(new char[1] { me.Content[i.Value] }));
        }

        public static TextData substring (TextData me, IntegerData start, IntegerData offset) {
            return new TextData(me.Content.Substring(start.Value, offset.Value));
        }

        public static TextData substringfrom(TextData me, IntegerData start) {
            return new TextData(me.Content.Substring(start.Value));
        }

        public static IntegerData count(TextData me) {
            return new IntegerData(me.Content.Length);
        }

        public static IntegerData length(TextData me) {
            return new IntegerData(me.Content.Length);
        }

        public static TextData splitat(TextData me, TextData splitter, IntegerData count, TextData defaults) {
            try {
                string[] spl = me.Content.Split(splitter.Content[0]);
                return new TextData(spl[count.Value]);
            } catch { return new TextData(defaults.Content); }
        }

        public static BooleanData isempty(TextData me) {
            return new BooleanData(string.IsNullOrWhiteSpace(me.Content));
        }
    }
}
