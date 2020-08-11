using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Ast {

    // 一个 Element Section 代表一个拥有子项的嵌套结构，它不允许有默认项，在语义上，
    // 它可以分为 Command Section， Template Section 两种

    // 例如： (template: (name: link)
    //                   (array: ((redirect)
    //                            (style)
    //                            (alt)) )
    //                   (content: 
    //                       click this to follow: (h-a: (href: @@(redir)) 
    //                       (alt: @@(alt)) (style: @@(style)) (content: @@(content)))
    //                   ))
    //
    //        (template: (name: (h-a))
    //                   (array: ((href) (alt) (style)) )
    //                   (content: <a href='@@(href)' style='@@(style)' alt='@@(alt)'> @@(content) </a> ))
    //
    // 解析成纯 ElementSection 列为：
    // 
    //     CommandSection
    //     |- Name = 'template'
    //     |- Parameters 
    //        |- [name] :  [0] <StringData> 'link'
    //        |- [array] : [0] <ArrayData> ...
    //        |                |- [0] <StringData> 'redirect'
    //        |                |- [1] <StringData> 'style'
    //        |                |- [2] <StringData> 'alt'
    //        |- [content] : [0] TextSection 'click this to follow: '
    //                       [1] TemplateSection
    //                           |- Name: 'h-a'
    //                           |- Parameters
    //                              |- [href] : <Param>
    //                              |- [alt] : <Param>
    //                              |- [style] : <Param>
    //                              |- [content] : <Param>

    public class ElementSection : Section {
        public string Name { get; set; }
        public string Content { get; set; }

        public Dictionary<string, List<Section>> Parameters { get; set; }
         = new Dictionary<string, List<Section>>();

        public void Parse(string raw) {
            this.Content = raw;
            if (this.Name != "") this.Raw = "(" + this.Name + ": " + raw + ")";

            // 在上述的例子中，本级所解析的 raw 字符串可以为：

            // '(name: link)(array: ((redirect)(style)(alt)))(content: ...)' 

            // 因此，我们需要解析的是将一组多括号组解析到 Parameters 的字典中，
            // 例如将 name 键值设为 raw 值为 'link' 的未解析元素

            Dictionary<string, string> unparsed = new Dictionary<string, string>();

            int bracket = 0;
            int col = 0;
            bool reversed = false;

            bool hascolon = false;
            string content = "";
            string name = "";
            foreach (char ch in raw) {
                if (ch == '`') { reversed = true; col++; continue; }
                if (!reversed) {
                    if (ch == '(') { bracket++; col++;
                        if (bracket > 1)
                            content = content + '(';
                        continue; }
                    if (ch == ')') {
                        bracket--; col++;

                        // 关闭一级标签

                        if (bracket == 0) {
                            unparsed.Add(name, content.Trim());
                            name = "";
                            content = "";
                            hascolon = false;
                        } else { content = content + ')'; }

                        continue;
                    }
                }

                // 这里将字符串附加到当前读取器上

                if (bracket >= 1 &&
                   string.IsNullOrWhiteSpace(content) &&
                   hascolon == false) {
                    if (ch == ':') {
                        hascolon = true;
                    } else {
                        name = name + ch;
                    }
                } else if (bracket >= 1 &&
                    hascolon) {
                    content = content + ch;
                }

                if (reversed) reversed = false;
                col++;
            }

            bracket = 0; col = 0; reversed = false;

            // 到这一步，我们已经获得了未解析字典，它的键就是属性的名称，而
            // 它的键值是一个没有解析，并且将要被解析成 List<Section> 的字符串，
            // 在上述的例子中，到这一步字典形为：
            // 
            // ['name']          : 'link'
            // ['array']         : '((redirect)(style)(alt))'
            // ['content']       : 'click this to follow: (h-a: (href: @@(redir)) (alt: @@(alt)) (style: @@(style)) (content: @@(content)))'

            // 现在，我们检测里面有无嵌套结构，如果没有()，直接解析成基础类型

            ParseRawDictionary(unparsed);
        }

        public void ParseRawDictionary(Dictionary<string, string> unparsed) {
            int bracket = 0;
            int col = 0;
            bool reversed = false;

            foreach (var item in unparsed) {
                string value = item.Value;
                bool hasbracket = false;

                // 检测有无括号

                bool rev = false;
                foreach (char ch in value) {
                    if (ch == '`') {
                        rev = true;
                        continue;
                    }

                    // 我们默认括号是成对的

                    if (ch == '(' && rev == false) {
                        hasbracket = true;
                        break;
                    }

                    if (hasbracket) hasbracket = false;
                }

                if (hasbracket) {

                    // 含有括号时，我们需要考虑它时复合结构，数组还是元素。

                    // 数组形如 ((...) (...) (...))
                    // 元素形如 (...: ...)
                    // 复合结构形如 ... ((...) (...)) ... (...: ...) ...

                    string zerolevel = "";
                    string firstlevel = "";
                    bool hcolon = false;

                    string elemname = "";
                    string elembody = "";

                    // 我们定义两个层级的总字符，第零层级（没有括号的），和第一层级

                    int parrallelbrackets = 0;
                    foreach (char ch in value) {
                        if (ch == '`') { reversed = true; continue; }
                        if (!reversed) {
                            if (ch == '(') { 
                                bracket++;
                                if (bracket > 1) {
                                    elembody = elembody + "(";
                                }
                                continue; 
                            }
                            if (ch == ')') {
                                bracket--;
                                if(bracket>0) {
                                    elembody = elembody + ")";
                                } else if(bracket == 0) {
                                    parrallelbrackets++;
                                }
                                continue;
                            }
                        }

                        if (bracket == 0) {
                            zerolevel = zerolevel + ch;
                        } else if (bracket == 1) {
                            firstlevel = firstlevel + ch;
                        }

                        if (bracket >= 1) {
                            if (ch == ':' && hcolon == false) {
                                hcolon = true;
                                continue;
                            }
                            if (hcolon == false &&
                                bracket == 1) {
                                elemname = elemname + ch;
                            } else if (hcolon == true && bracket >= 1) {
                                elembody = elembody + ch;
                            }
                        }

                        if (reversed) reversed = false;
                    }

                    if (string.IsNullOrWhiteSpace(zerolevel) && parrallelbrackets == 1) {
                        if (string.IsNullOrWhiteSpace(firstlevel)) {
                            this.Parameters.Add(item.Key, new List<Section>()
                                {
                                    ArrayData.Parse(value.Remove(0,1).Remove(value.Length-2,1))
                                });
                        } else {
                            if (value.Remove(0, 1).Remove(value.Length - 2, 1).Contains(":"))
                                this.Parameters.Add(item.Key, new List<Section>()
                                {
                                     ElementSection.Parse(elembody,elemname)
                                });
                        }
                    } else {

                        // 这里是混合类型

                        // 因为其实现思路有较大的不同，这里我们把前面进行过的步骤重做一遍：

                        List<Section> child = new List<Section>();
                        string temp_text = "";

                        bracket = 0; col = 0; reversed = false;
                        foreach (char ch in value) {
                            if (ch == '`') { reversed = true; continue; }
                            if (!reversed) {
                                if (ch == '(') {
                                    bracket++;

                                    if (bracket == 1) {

                                        // 括号 0 -> 1 ： 前面的均为 text.

                                        child.Add(new TextData(temp_text));
                                        temp_text = "";
                                    } else { temp_text = temp_text + ch; }

                                    continue;
                                }
                                if (ch == ')') {
                                    bracket--;

                                    if (bracket == 0) {

                                        // 括号 1 -> 0，前面的可能为 Array 也可能为 Element
                                        // 对于 temp_text, 我们需要对此做出辨别

                                        elembody = ""; elemname = "";
                                        hcolon = false;
                                        rev = false; int br = 0;

                                        foreach (char c in temp_text) {
                                            if (c == '`') { rev = true; continue; }
                                            if (!rev) {
                                                if (c == '(') { 
                                                    br++;
                                                    elembody = elembody + "(";
                                                    continue; }
                                                if (c == ')') {
                                                    br--;
                                                    elembody = elembody + ")";
                                                    continue;
                                                }
                                            }

                                            if (br >= 0) {
                                                if (c == ':' && hcolon == false) {
                                                    hcolon = true;
                                                    if (rev) rev = false;
                                                    continue;
                                                }
                                                if (hcolon == false) {
                                                    elemname = elemname + c;
                                                } else if (hcolon == true) {
                                                    elembody = elembody + c;
                                                }
                                            }

                                            if (rev) rev = false;
                                        }

                                        if (temp_text.Trim().StartsWith("("))
                                            child.Add(ArrayData.Parse(temp_text));
                                        else if (temp_text.Contains(":"))
                                            child.Add(ElementSection.Parse(elembody, elemname));
                                        else child.Add(DataSection.Parse(temp_text));
                                        temp_text = "";
                                    } else { temp_text = temp_text + ch; }

                                    continue;
                                }
                            }

                            temp_text = temp_text + ch;
                            if (reversed) reversed = false;
                        }

                        if (!string.IsNullOrWhiteSpace(temp_text)) {
                            if (temp_text.Trim().StartsWith("("))
                                child.Add(ArrayData.Parse(temp_text));
                            else child.Add(DataSection.Parse(temp_text));
                            temp_text = "";
                        }

                        this.Parameters.Add(item.Key, child);
                    }
                } else {

                    // 直接解析成基础类型

                    this.Parameters.Add(item.Key, new List<Section>()
                    {
                        DataSection.Parse(value)
                    });
                }
            }
        }

        public static ElementSection Parse(string text, string name) {
            ElementSection e = new ElementSection();
            e.Name = name;
            e.Parse(text);
            e.Raw = "(" + name + ": " + text + ")";
            return e;
        }

        public override string ToString() {
            return "";
        }
    }
}
