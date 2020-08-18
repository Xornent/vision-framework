#region Vision Markup 语言标准

#region 版本 1.0
/*
 * ==================================================================================================
 * 
 *                                Vision Markup 文档：语言标准
 *                                
 *                                版本： 1.0
 *                                发布： 2020.07.23
 *                                
 * ==================================================================================================
 * 目录
 * 
 * I.    概述
 *   I.I    基础语法
 *   I.II   解析类型
 * II.   抽象语法树
 * --------------------------------------------------------------------------------------------------
 * I. 概述
 *     
 *     为了在站点上描述一个网页的内容，并以较少的文字呈现出丰富而规范的网页排版和内容，使用 HTML 作
 * 为描述语言显然是麻烦的。 也根据数十年前的项目 Wikipedia 的经验，我们需要设计一个简单的描述性语言。
 * 
 * I.I 基础语法
 * 
 *     在基础上，我们采用形式类似 Lisp 的语法，用括号包围的函数名和参数标识内容。然而，Lisp 在其间并
 * 没有间隔符，为了观看的方便，我们在函数名（宏和模板名）与参数之间加上 ':'（英文冒号），并以此翻译
 * 和扩充 Html 的语法。 这里面说“函数”，其实只是借用它的位置。这些标识不同括号中嵌套内容的名称我们
 * 叫做标签。 而这些括号和其包裹起来的部分称为元素。 特殊的，当一个元素没有指定名称，它可以不需要冒号，
 * 它内部的内容将被自动翻译成文本类型。
 * 
 *     每个元素的标题冒号之后的部分是它的参数区。 每个参数由一对括号包围。 参数区中除去所有参数的剩余
 * 部分将被自动解析成它的默认内容。 对于某些元素，默认内容与参数名为 content 的参数是语义上等价的，而
 * content 若不被使用，这些内容直接视为无效。
 * 
 *     注释是语言中被刻意视为无效的部分，它可以使用 (; 内容) 表示。不要故意使用 content 无效的元素代替
 * 注释，那是不规范的。实际上，因为这个语言只是描述界面内容，你可以几乎不用注释；就像没有人写文章还要
 * 故意写出很多给出版者看，而不给读者看的段落一样。
 * 
 *     e.g. 1  (paragraph: 
 *                 (content: 段落的内容)
 *             )
 *     
 *             这是一个元素，它的标签是 paragraph
 *             
 *     e.g. 2  (paragraph: 段落的内容)
 *             
 *             这和 e.g. 1 是等价的，我们推荐使用这种语法，因为它更简单。
 *             
 *     e.g. 3  (link: (redirect: https://xornent.github.io/vision/)
 *                    (content: Vision 项目))
 *             
 *             或 (link: (redirect: https://xornent.github.io/vision/) Vision 项目)
 *             
 *             link 元素有两个参数，content 是 'Vision 项目'
 *             
 *             (link: Visi(redirect: https://xornent.github.io/vision/)on 项目) 
 *
 *             事实上， 按照语法规定，这样也能到达正确的结果。但是请不要这样做，不是所有人都要
 *             像解析器那样专注于标准的细节。
 *             
 *     各元素之间是互相嵌套的，破坏嵌套结构会导致当前初级元素解析失败。而每个元素在语义上规定拥有一个
 * content 子元素和一个 params 子元素。 其中 content 是数据类型， params 是数组类型。 params 可以是空
 * 数组。
 *             
 *     对于任何熟悉打字的人来说，英语的输入其实与相同语义的中文输入的难度相当，甚至更小，我们使用英语
 * 为各元素标签取名。 在默认的编辑器中，我们还支持自动补全。
 * 
 * I.II 解析类型
 * 
 *     Vision 标记语言源码是纯文本，我们将读取范围内的所有内容去除首尾两端的空格，视作一个元素内的
 * 可用内容。 然后，我们会尝试将改内容翻译成预设置的基本类型。 预设置的基本类型如下：
 * 
 * I.II.I 浮点数类型
 * 
 *     可用内容形如 233.14, 910.0219930103920100399201930940291039021 等，必须包含小数点
 *     
 *     未命名的浮点数 (233.14)
 *     命名的浮点数 (foo: 233.14)
 *     
 * I.II.II 整数类型
 * 
 *     可用类型形如 891, 100, 85 等，必须不包含小数点
 *     
 * I.II.III 日期类型
 * 
 *     可用 DateTime.Parse 解析的字符串
 *     
 * I.II.IV 枚举类型
 * 
 *     字符串的枚举标记规定了枚举和枚举常量。 它的语法是 (E`枚举类型: 枚举常量)，对于枚举类型和枚举常量
 * 的范围和限制，在后文预定义类型中有做出规范，例如：
 * 
 *     (color| red)，
 *     
 *     枚举前标号 E` 中不区分大小写。你可以写作 (COLOR|RED),(color|Red) 等等，这些都是等效的，但是，我们
 * 推荐将枚举类型和枚举常量名使用全小写，规范为 (color| red)
 * 
 * I.II.V 字符串类型
 *     
 *     如果内容不是其他类型，则统一解析为无前导/尾随空格的字符串
 *     
 *     以上五种解析类型被称为数据类型，除此之外，还有数组类型。
 *     
 * I.II.VI 数组类型
 * 
 *     使用括号包括的多个数据类型被称为一个数组。 多余的括号是存在语义的，但我们需要做出区别：
 *     
 *     ((12) (34.15))    (foo: (12) (34.15))
 *     
 *     ============================================================================================
 *      源代码                                               | 抽象语法树
 *     --------------------------------------------------------------------------------------------
 *      (content)                                            | <string> 'content'
 *     --------------------------------------------------------------------------------------------
 *      ((233.12) (90) (2020/03/03 12:23:00) (e`color:red))  | <array>
 *                                                           |     [0]  : <float>     233.12
 *                                                           |     [1]  : <int>       90
 *                                                           |     [2]  : <datetime>  2020.03.03
 *                                                           |                        12:23:00
 *                                                           |     [3]  : <e`color>   red
 *     --------------------------------------------------------------------------------------------
 *      (model: (e`color: red) ((0) (100)) the content )     | <element> model
 *                                                           |     params:  <array>
 *                                                           |         [0] : <color>  red
 *                                                           |         [1] : <array>
 *                                                           |              [0]: <int> 0
 *                                                           |              [1]: <int> 100
 *                                                           |         [2] : <string> (named content) 
 *                                                           |              'the content'
 *     =============================================================================================
 *     
 * II. 抽象语法树
 * 
 * II.I 元素 <element>
 *     
 *     一个元素有一个元素标签，一个 params 数据。 其中 params 数据类型为数组。params 中的值可以有
 * 一个，多个或者没有元素。记作：
 *     
 *     (元素标签: (...) (...) (...) ...)
 *     
 *     这时，参数数组中的内容会被自动分配编号，这些参数分别叫 #1, #2, #3。 一个元素的 params 参数
 * 是可以有名称的，这样，参数不写完也能指定内容。 在模板的书写过程中，可以使用 @@(#1)，或者 
 * @@(参数名称) 调用参数代表的数据。
 *     
 *     (元素标签: (参数名1: ...) (参数名2: ...) ...) 例如：
 *     
 *     ((background: color | red) (range: (start: 0)        | 
 *     (end: 100)) the content )                            | <array>  
 *                                                          |         [0] : <e`color>  red
 *                                                          |         [1] : <array> (named range)
 *                                                          |              [0]: <int> (named start) 0 
 *                                                          |              [1]: <int> (named end) 100
 *                                                          |         [2] : <string> (named content) 
 *                                                          |              'the content'
 *                                                          
 *      假设这是一个模板的传入参数，那么
 *      
 *      ===========================================================================================
 *       调用                                            | 值
 *      -------------------------------------------------------------------------------------------
 *       @@(#1)                                          | <e`color>  red
 *       @@(range)                                       | <array>    ((start: 0) (end: 100))
 *       @@(range>start)                                 | <int>      0
 *       @@(range>end)                                   | <int>      100
 *       @@(content)                                     | <string>   'the content'
 *      ===========================================================================================
 *      注： 我们虽然没有显式声明 content，但解析器自动分配了 content 的名字。 但是略微修改的表达式
 *      可能导致相对应的序号发生变化，例如 '((range: (start: 0) (end: 100)) content)' 中的 @@(#1) 值
 *      就为 ((start: 0) (end: 100)) 了，我们推荐在模板的创建和使用的过程中，为所有的属性分配名称。
 *      
 * II.II 解析类型： 同 I.II
 * 
 * II.III 服务器端语句
 * 
 *     在可编程性的 Vision Markup 中，我们指定了一组关键字和逻辑，用于动态生成模板和参数的显示内容。
 * 我们支持以条件 (if... eif.... else...)，分支(switch case... default...)，循环(for...) 来选择性的
 * 显示和隐藏部分的内容。
 * 
 * II.III.I 条件语句
 * 
 *     语法： (ifbranch: (variable: <string> name) (condition: <enum> ifcondition|...) (value: ...)
 *                       (content: ...) (otherwise: ...))
 *     
 *     其中， 枚举 ifcondition 指示比较类型，参见内置类型。
 *     
 *     例如，我们有一段代码（它所在的模板名为 test）：
 *         
 *         <p>(ifbranch:
 *                 (condition: ifcondition|more)
 *                 (variable: type) (value: 8)
 *                 (content:
 *                     the type is more than eight. <b>`(8, +oo`) )
 *                 (otherwise: the type is less than three. <b>`(-oo, 8]))</b></p>
 *             
 *     ============================================================================================
 *      调用                                      | 结果
 *     --------------------------------------------------------------------------------------------
 *      (test:                                    | <p>the type is more than eight. <b>(8, +oo)
 *          (type: 13))                           | </b></p>
 *     ============================================================================================
 *     
 * II.III.II 循环语句
 * 
 *     语法： (for: (define: define_name) (from: ...) (to: ...) (step: ...) ...)
 *     
 *     例如，在模板(test)中，有这样一段代码：
 *     
 *         (for: (define: i) (from: 0) (to: 10) (step:@@(step)) 
 *             <p>this is @@(i)</p>
 *         )
 *     
 *     ============================================================================================
 *      调用                                      | 结果
 *     --------------------------------------------------------------------------------------------
 *      (test: (step: 2))                         | <p>this is 0</p>
 *                                                | <p>this is 2</p>
 *                                                | <p>this is 4</p>
 *                                                | <p>this is 6</p>
 *                                                | <p>this is 8</p>
 *                                                | <p>this is 10</p>
 *     --------------------------------------------------------------------------------------------
 *      (test: (step: 3))                         | <p>this is 0</p>
 *                                                | <p>this is 3</p>
 *                                                | <p>this is 6</p>
 *                                                | <p>this is 9</p>
 *     ============================================================================================
 *     
 * II.IV 转义
 * 
 *     使用 '`' 字符是后面紧跟的一个字符转义成原字符的含义，而不赋予特殊的语义，在英语 () 和 : 中
 * 极为常见。 在你确定的没有 Vision 标记语言的英语段落中，全选替换为转义符号，在中文语段中，一律
 * 使用中文的括号和冒号防止混淆 （）：
 * 
 * II.V 客户端脚本的系统模板
 * 
 *     假如一个编者想让某一块内容折叠展开，或者点击某超链接使某元素变成红色，这样的功能是动态界面
 * 的范畴。为了条目的稳定性，我们不允许在标记语言中显式调用 ECMA Script 等客户端脚本，只将合法的内容
 * 包装成特殊的元素共开发这调用。 我们在 III.I 中会列出参考。
 * 
 * II.VI 预处理符号
 * 
 *     我们支持预处理符号将代码分节，便于指定解析器的操作
 *     
 *     预处理符号必须顶格写出（或者前面只有白空格）。
 *     
 *     ============================================================================================
 *      预处理符号                               | 功能
 *     --------------------------------------------------------------------------------------------
 *      #disable                                 | 从这一行开始不通过解析，所有的文本都照原样进行
 *      #enable                                  | 从这一行开始重新启用解析
 *     ============================================================================================
 * 
 * III. 列表项
 * 
 * III.I 客户端脚本的系统模板
 * 
 * III.I.I 折叠展开子元素 expand
 * 
 *     语法： (expand: (default: toggle| ...) ...)
 *     
 *     出现一个区域，单击边缘的箭头可以使它的全部子元素显现或消失。
 * 
 * III.II 内置类型表
 * 
 *  ================================================================================================
 *   类型全名                    | 缩写              | 对应的 C# 语言对象
 *  ------------------------------------------------------------------------------------------------
 *   int                         | 不用写出          | System.Int32
 *   float                       | 不用写出          | System.Float
 *   string                      | 不用写出          | System.String
 *   datetime                    | 不用写出          | System.DateTime
 *   array                       | 不用写出          | System.Object[]
 *   color                       | -                 | System.Drawing.Color
 *   ifcondition                 | icond             | Vision.Markup.Enumerations.IfCondition
 *   boolean                     | bool              | Vision.Markup.Enumerations.@Boolean
 *   toogle                      | -                 | Vision.Markup.Enumerations.Toogle
 *   orientionh                  | orih              | Vision.Markup.Enumerations.HorizontalOriention
 *  ================================================================================================
 * 
 * III.III 系统命名表
 * 
 *  ================================================================================================
 *   全名                        | 缩写              | 注释
 *  ------------------------------------------------------------------------------------------------
 *   系统模板（当前不支持缩写！）
 *  ------------------------------------------------------------------------------------------------
 *   model                       | -                 |
 *   link                        | l                 |
 *   externallink                | lext              |
 *   reference                   | ref               |
 *   user                        | -                 |
 *   image                       | img               |
 *   gallary                     | gal               |
 *   footnote                    | f                 |
 *   table                       | tbl               |
 *   trow                        | tr                |
 *   theader                     | th                |
 *   quote                       | q                 |
 *   code                        | c                 |
 *   forced                      | -                 |
 *   math                        | m                 |
 *   xt                          | -                 | 表示错误的文本
 *   vt                          | -                 | 表示正确的文本
 *   x                           | -                 | 打一个叉
 *   v                           | -                 | 打一个勾
 *   strike                      | s                 | 
 *   cite                        | -                 | 
 *  ------------------------------------------------------------------------------------------------
 *   系统命令
 *  ------------------------------------------------------------------------------------------------
 *   toc                         | -                 | 生成目录
 *   referencelist               | reflist           | 列举出本页中所有的参考
 *   externallist                | extlist           | 列举出本页中所有的外部链接
 *   footnotelist                | footlist          | 列举出本页所有的脚注
 *   sign                        | -                 | 编者和编辑时间签名
 *   template                    | t                 | 定义模板
 *   break                       | br                | 命令文本换行符
 *   element                     | elem              | 基类
 *  ------------------------------------------------------------------------------------------------
 *   可编程性
 *  ------------------------------------------------------------------------------------------------
 *   ifbranch                    | ifb               | If 块分支
 *   condition                   | cond              |
 *   variable                    | var               |
 *   value                       | -                 |
 *   otherwise                   | -                 |
 *   for                         | -                 | For 块分支
 *   define                      | -                 |
 *   from                        | -                 |
 *   to                          | -                 |
 *   step                        | -                 |
 *  ------------------------------------------------------------------------------------------------
 *   预声明的变量 （假设今天是 2020.7.29 星期三,你在浏览 Help 界面）
 *  ------------------------------------------------------------------------------------------------
 *   @{currentweek}              | @{week}           | <int> 在本年度中的周序数 30
 *   @{currentmonth}             | @{month}          | <int> 本年度中月序数     7
 *   @{currentday}               | @{day}            | <int> 本月中的日序数     29
 *   @{currentdayofweek}         | @{dow}            | <int> 星期               3
 *   @{currentyear}              | @{year}           | <int>                    2020
 *   @{currenttime}              | @{time}           | <string>                 10:45
 *   @{currentmonthname}         | @{monthname}      | <string>                 July
 *   @{currentdayname}           | @{dayname}        | <string>                 Wendnesday
 *   @{datetime}                 | -                 | <datetime>               2020.07.29 10:45:00
 *   @{title}                    | -                 | <string>                 Help
 *   @{namespace}                | @{ns}             | <string>                 Vision
 *   @{revisionid}               | @{rid}            | <int>                    1
 *   @{page}                     | -                 | <string>                 /home/page/(id)
 *   @{numberofpages}            | @{nop}            | <int>                    ...
 *   @{numberofusers}            | @{nou}            | <int>                    ...
 *   @{revisionuser}             | @{ru}             | <string>                 admin
 *  ================================================================================================
 * 
 * III.IV Html 元素
 * 
 * IV. 模板
 * 
 * IV.I 模板中的语法
 * 
 *     一个标准的模板（以默认模板 link 为例）：
 *     
 *     (template: (name: link) (content:
 *         <a alt='@@(alt)' href='~/home/page/@@(redirect)' style='@@(style)'>
 *         @@(content) </a>
 *     ))
 *     
 * ==================================================================================================
 */
#endregion

#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Vision.Data;
using Vision.Markup.Ast;
using Vision.Markup.Categories;
using Vision.Models;

namespace Vision.Markup {

    public class MarkupParser {
        public MarkupParser() {

        }

        public (string Page, MarkupDocument Document) FromSource(string markup, Page page, Record record, User user, string ns, PageContext ctxpage, UserContext ctxuser) {

            // 使用 <novml> ... </novml> 可以使部分内容直接以原文显示，
            // 这不同于一般的 HTML 标签。

            string pattern = @"<novml>[\s\S\n\t\r]*?</novml>";
            markup = markup.Replace("\n", "$<break-n>");
            Dictionary<Guid, string> replacer = new Dictionary<Guid, string>();
            while (Regex.IsMatch(markup, pattern)) {
                markup = Regex.Replace(markup, pattern, (match) => {
                    Guid guid = Guid.NewGuid();
                    replacer.Add(guid, match.Value.Remove(0,7).Remove(match.Value.Length - 15,8));
                    return "$$:::" + guid.ToString().ToUpper() + ":::";
                });
            }
            markup = markup.Replace("$<break-n>", "\n");
            MarkupDocument doc = Parse(markup.Replace("`(", "$<br-left>").Replace("`)","$<br-right>"));
            doc.Execute(page, record, user, ns, ctxpage, ctxuser);
            string s = doc.ToString().Replace("$<br-left>","(").Replace("$<br-right>",")");
            foreach (var item in replacer) {
                s = s.Replace("$$:::" + item.Key.ToString().ToUpper() + ":::", item.Value.Replace("$<break-n>", "\n"));
            }

            return (s, doc);
        }

        public (string Page, MarkupDocument Document) FromSource(string markup, PageContext ctxpage, UserContext ctxuser) {
            string pattern = @"<novml>[\s\S\n\t\r]*?</novml>";
            markup = markup.Replace("\n", "$<break-n>");
            Dictionary<Guid, string> replacer = new Dictionary<Guid, string>();
            while (Regex.IsMatch(markup, pattern)) {
                markup = Regex.Replace(markup, pattern, (match) => {
                    Guid guid = Guid.NewGuid();
                    replacer.Add(guid, match.Value.Remove(0, 7).Remove(match.Value.Length - 15, 8));
                    return "$$:::" + guid.ToString().ToUpper() + ":::";
                });
            }
            markup = markup.Replace("$<break-n>", "\n");
            MarkupDocument doc = Parse(markup.Replace("`(", "$<br-left>").Replace("`)", "$<br-right>"));
            doc.Execute(ctxpage, ctxuser);
            string s = doc.ToString().Replace("$<br-left>", "(").Replace("$<br-right>", ")");
            foreach (var item in replacer) {
                s = s.Replace("$$:::" + item.Key.ToString().ToUpper() + ":::", item.Value.Replace("$<break-n>", "\n"));
            }

            return (s, doc);
        }

        public MarkupDocument NotExecuteFromSource(string markup) {
            string pattern = @"<novml>[\s\S\n\t\r]*?</novml>";
            markup = markup.Replace("\n", "$${break-n}");
            Dictionary<Guid, string> replacer = new Dictionary<Guid, string>();
            while (Regex.IsMatch(markup, pattern)) {
                markup = Regex.Replace(markup, pattern, (match) => {
                    Guid guid = Guid.NewGuid();
                    replacer.Add(guid, match.Value.Remove(0, 7).Remove(match.Value.Length - 15, 8));
                    return "$$:::" + guid.ToString().ToUpper() + ":::";
                });
            }
            markup = markup.Replace("$${break-n}", "\n");
            MarkupDocument doc = Parse(markup.Replace("`(", "$${br-left}").Replace("`)", "$${br-right}"));
            return doc;
        }

        public MarkupDocument Parse(string markup) {
            MarkupDocument doc = new MarkupDocument();
            int major = 0;
            int minor = 0;
            int small = 0;
            markup = ParseAbbrevation(markup, doc);
            if (!markup.EndsWith('\n')) markup = markup + "\n";
            int bracket = 0;
            List<string> lines = markup.Split('\n').ToList();
            bool enabled = true;

            bool listmode = false;
            ListItem list = new ListItem();
            var appendee = list;
            int lastlevel = 1;
            bool skipped = false;

            bool codemode = false;

            int count = lines.Count;
            string raw = "";
            for(int i = 0; i < count; i++) {
                string line = lines[i].Replace("\t", "").Replace("\r", "");
                int col = 1;
                string trimmed = line.Trim();
                string lowered = trimmed.ToLower();

                // 整行检测，用于检测标题和非括号段落,以及预处理指令
                // 它只在没有括号嵌套时才检测

                if (bracket == 0) {
                    if (lowered.StartsWith("######") && enabled) {
                        doc.Children.Add(new HeadlineSection(6, trimmed.Remove(0, 6).Trim(),"")); continue;
                    } else if (lowered.StartsWith("#####") && enabled) {
                        doc.Children.Add(new HeadlineSection(5, trimmed.Remove(0, 5).Trim(),"")); continue;
                    } else if (lowered.StartsWith("####") && enabled) {
                        small++;
                        doc.Children.Add(new HeadlineSection(4, trimmed.Remove(0, 4).Trim(),major+"_"+minor+"_"+small)); continue;
                    } else if (lowered.StartsWith("###") && enabled) {
                        minor++; small = 0;
                        doc.Children.Add(new HeadlineSection(3, trimmed.Remove(0, 3).Trim(), major +"_"+minor)); continue;
                    } else if (lowered.StartsWith("##") && enabled) {
                        major++; minor = 0; small = 0;
                        doc.Children.Add(new HeadlineSection(2, trimmed.Remove(0, 2).Trim(), major.ToString())); continue;
                    } else if(lowered.StartsWith("#")) {

                        // 注意：只有一个 # 开头的时预编译指令

                        // 这和标题语法并不矛盾，因为在页面最上端，系统会自动生成一级标题，
                        // 而一个页面只能拥有一个以及标题，所以我们直接禁用了其表示标题的意义。

                        string command = lowered.Remove(0, 1).Trim();
                        switch (command) {
                            case "enable":
                                enabled = true;
                                break;
                            case "disable":
                                enabled = false;
                                break;
                            default:
                                break;
                        }
                        continue;
                    } else if(lowered.StartsWith("```")) {
                        if (listmode) { 
                            listmode = false;
                            doc.Children.Add(new TextData(list.ToString()));
                            list = new ListItem();
                            appendee = list;
                            lastlevel = 1;
                            skipped = false;
                        }
                        else if (!listmode) {
                            if (lowered.Replace("```", "").Trim() == "list") listmode = true;
                        }
                        continue;
                    }

                    if (listmode) {
                        string tag = lowered.Split(' ')[0];
                        string contentlist = trimmed.Replace(tag, "").Trim();
                        if ((lowered.StartsWith(":") || lowered.StartsWith("*"))) {
                            int level = tag.Count();
                            if (level - lastlevel > 1 ) {
                                skipped = true;
                            } else skipped = false;

                            if (!skipped) {
                                char currenttag = tag.Last();
                                if (level > lastlevel) {
                                    appendee = appendee.Children.Last();
                                    lastlevel++;
                                } else if (level == lastlevel) {

                                } else if (level < lastlevel) {
                                    while (level < lastlevel) {
                                        appendee = appendee.Parent;
                                        lastlevel--;
                                    }
                                }

                                if (currenttag == '*')
                                    appendee.Children.Add(new ListItem()
                                    {
                                        Content = new List<Section>() { new TextData(contentlist) },
                                        Numbered = false,
                                        Parent = appendee
                                    });
                                else if (currenttag == ':')
                                    appendee.Children.Add(new ListItem()
                                    {
                                        Content = new List<Section>() { new TextData(contentlist) },
                                        Numbered = true,
                                        Parent = appendee
                                    });
                            }
                        } else {
                            if (!skipped) {
                                appendee.Children.Last().Content.Add(new TextData(contentlist));
                            }
                        }
                        continue;
                    }

                    // 在所有括号均已关闭时，检测到空行，立即结束当前节

                    if (string.IsNullOrWhiteSpace(trimmed) && (!string.IsNullOrWhiteSpace(raw))) {
                        doc.Children.Add(new ParagraphSection() { Raw = raw });
                        raw = ""; continue;
                    }
                }

                if (enabled)
                    raw = raw + "" + trimmed.Replace("\t", "").Replace("\n", "");
            }

            if(!string.IsNullOrWhiteSpace(raw)) {
                doc.Children.Add(new ParagraphSection() { Raw = raw });
            }

            for (int p = 0; p < doc.Children.Count; p++) {
                if (doc.Children[p] is ParagraphSection) {
                    var parag = doc.Children[p] as ParagraphSection;
                    string src = "(content: " + parag.Raw + ")";
                    ElementSection elem = new ElementSection();
                    elem.Parse(src);

                    List<Section> val = new List<Section>();
                    bool success = elem.Parameters.TryGetValue("content", out val);
                    parag.Children = val;
                }
            }

            return doc;
        }

        public string ParseAbbrevation(string markup, MarkupDocument doc) {

            // Abbrevation 是 VML 支持的行内非模板缩略语法

            // VML 的模板需要满足一种类 Lisp 的语法，如果不是大段的代码段，其实很难减少实际代码量，
            // 这里，我们像 Markdown 或 Sematic Wiki 一样，也支持将一些极为常用的行内元素转化成
            // 简单的缩写，由服务器解析器预执行成相应的 Html 代码。

            // 注意：缩略语法也分为行内和整行两种，整行在解析流程上也分为段落行和标题行两种，
            //       但是在这个方法中，我们先只解析行内缩略，整行缩略由 Parse（上文）方法解析

            // 链接：     语法：  [[redirect]] 显示原始界面 redirect 的链接
            //                    [[redirect|display]] 显示为 display 导向 redirect 的链接
            // 如果 redirect 不是指向站内的链接，它将转化为外部链接

            // 它等价于元素 link
            // 使用 VML 标准模板 link 有： (link: (redirect: ...) (display: ...))
            // 如果它是外部链接：          (externallink: (redirect: ...) (display: ...))

            // 图片引用： 语法：  [[I: redirect|orient]] 显示 redirect 所指示的图片
            //                    [[I: redirect|orient|alt]] 显示 redirect 所指示的图片，悬停显示 alt
            //                    [[I: redirect|orient|alt|message]] 显示 redirect 所指示的图片，悬停显示 alt，有说明文字 message.
            //                    [[I: redirect|orient|alt|message #width,auto]] 指定大小加 #width(auto),height(auto)

            // 它等价于元素 image
            // 使用 VML 标准模板 image 有： (image: (redirect: ...) (alt: ...) (message: ...) (oriention: ...))

            markup = Regex.Replace(markup, @"'''''([\s\S\n\t\r]*?)'''''", (match) => {
                string val = match.Value.Remove(0, 5).Remove(match.Value.Length - 10, 5).Replace("\n", "");
                return "<b><i>" + val + "</i></b>";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            markup = Regex.Replace(markup, @"'''([\s\S\n\t\r]*?)'''", (match) => {
                string val = match.Value.Remove(0, 3).Remove(match.Value.Length - 6, 3).Replace("\n", "");
                return "<i>" + val + "</i>";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            markup = Regex.Replace(markup, @"''([\s\S\n\t\r]*?)''", (match) => {
                string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n", "");
                return "<b>" + val + "</b>";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string[] urls = Startup.WebRoot.Split(";");
            markup = Regex.Replace(markup, @"\[\[([\s\S\n\t\r]*?)\]\]", (match) => {
                string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n", "");
                val = Regex.Replace(val, @" +", " ");
                val = val.Replace(": ", ":").Replace(" :", ":").Replace(" : ", ":");

                if (val.StartsWith("I:") || val.StartsWith("i:")) {
                    string expr = val.Remove(0, 2).Trim().Replace("\n","");
                    string[] sizing = expr.Split('#');
                    string elem = "(redirect: ~/images/page-markers/warning.png) (alt: Default Image) (message:) (oriention: middle)";
                    string size = "(width: auto) (height: auto)";
                    string[] msg = sizing[0].Split('|');
                    if(sizing.Count() == 1) {
                        
                    } else {
                        string[] wh = sizing[1].Trim().Split(',');
                        size = "(width: " + wh[0].Trim() + ") (height: " + wh[1].Trim() + ")";
                    }

                    if(msg.Count() == 2) {
                        elem = "(redirect: " + msg[0].Trim() + ") (oriention: " + msg[1].Trim() + ") (alt:) (message:)";
                    } else if (msg.Count() == 3) {
                        elem = "(redirect: " + msg[0].Trim() + ") (oriention: " + msg[1].Trim() + ") (alt: "+ msg[2].Trim() +") (message:)";
                    } else if(msg.Count() == 4) {
                        elem = "(redirect: " + msg[0].Trim() + ") (oriention: " + msg[1].Trim() + ") (alt: " + msg[2].Trim() + ") (message: " + msg[3].Trim() + ")";
                    }
                    return "(image: " + elem + size + ")";
                } else {
                    string href = ""; string display = "";
                    string[] vals = val.Split('|');

                    if (vals.Count() > 1) display = vals[1];
                    else display = vals[0];
                    if (vals[0].Contains("/")) {
                        bool isinside = false;
                        foreach (var url in urls) {
                            if (vals[0].Trim().StartsWith(url) ||
                               vals[0].Trim().StartsWith(url.Replace("https://", "").Replace("http://", ""))) {
                                isinside = true;
                            }
                        }
                        if (isinside) return "(link: (redirect: " + vals[0] + ") (display: " + display + "))";
                        else return "(externallink: (redirect: " + vals[0] + ") (display: " + display + "))";
                    } else {
                        return "(link: (redirect:" + HttpUtility.UrlEncode(vals[0].Trim(), Encoding.UTF8).Replace("+", "%20") + ") (display: " + display + "))";
                    }
                }
            });

            // 图片组：   语法：  {[ redirect|alt !
            //                       redirect|alt ! ... 
            //                       redirect|alt ! orient ! message #width,height ]}

            // 它等价于元素 gallery
            // 使用 VML 标准模板 gallery 有： (gallery: (redirect: <array>) (alt: <array>) (message: ...) (width: ...) (height: ...) (oriention: ...))

            markup = Regex.Replace(markup, @"{\[([\s\S\n\t\r]*?)\]}", (match) => {
                string expr = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n","");
                string[] sizing = expr.Split('#');
                string size = "(width: auto) (height: auto)";
                List<string> array = sizing[0].Split('!').ToList();
                if (sizing.Count() == 1) {

                } else {
                    string[] wh = sizing[1].Trim().Split(',');
                    size = "(width: " + wh[0].Trim() + ") (height: " + wh[1].Trim() + ")";
                }

                string msg = array.Last().Trim();
                array.RemoveAt(array.Count - 1);
                string orient = array.Last().Trim();
                array.RemoveAt(array.Count - 1);
                string redir = ""; string alts = "";
                foreach(var item in array) {
                    string[] prop = item.Split('|');
                    redir = redir + " (" + prop[0].Trim() + ")";
                    alts = alts + " (" + prop[1].Trim() + ")";
                }
                return "(gallery: (redirect: (" + redir + ")) (alt: (" + alts + ")) (message: " + msg + ") (oriention: " + orient + ") " + size + " )";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 引文：     语法： %%quote%%
            //                   (quote: (content: ...))

            markup = Regex.Replace(markup, @"%%([\s\S\n\t\r]*?)%%", (match) => {
                string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n","");
                return "(quote: (content: " + val + "))";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 代码：     语法： ^^code^^
            //                   (code: (content: ...))

            markup = Regex.Replace(markup, @"\^\^([\s\S\n\t\r]*?)\^\^", (match) => {
                string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n","");
                return "(code: (content: " + val + "))";
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 引用：     语法： {{{id}}}
            //                   {{{id|message}}}
            //                   (reference: (id: ...) (message: ...))

            markup = Regex.Replace(markup, @"{{{([\s\S\n\t\r]*?)}}}", (match) => {
                string val = match.Value.Remove(0, 3).Remove(match.Value.Length - 6, 3).Replace("\n","");
                string mark = "";
                string msg = "";
                if (val.Contains("|")) {
                    string[] param = val.Split('|');
                   
                    SetDictionary(doc.References, param[0].Trim(), param[1].Trim());
                    return "(reference: (id: " + param[0].Trim() + ") (message: " + param[1].Trim() + "))";
                } else {
                    SetDictionary(doc.References, val.Trim(), "");
                    return "(reference: (id: " + val.Trim() + ") (message: ))";
                }
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 脚注：     语法： {{id}}
            //                   {{id|message}}
            //                   (footnote: (id: ...) (message: ...))

            markup = Regex.Replace(markup, @"{{([\s\S\n\t\r]*?)}}", (match) => {
                string val = match.Value.Remove(0, 2).Remove(match.Value.Length - 4, 2).Replace("\n","");
                if (val.Contains("|")) {
                    string[] param = val.Split('|');
                    string disp = ToRomanLower(int.Parse(param[0].Trim()));
                    SetDictionary(doc.Footnotes, param[0].Trim(), param[1].Trim());
                    return "(footnote: (id: " + disp + ") (message: " + param[1].Trim() + "))";
                } else {
                    SetDictionary(doc.Footnotes, val.Trim(), "");
                    return "(footnote: (id: " + ToRomanLower(int.Parse(val.Trim())) + ") (message: ))";
                }
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 值得注意的是，由于列表在其他语法中的简化与 VML 标准语法的复杂度之间差异不大，在许多情况下甚至 VML 更简单
            // 我们不对表格进行特殊语法规定，而统一使用标准 VML 数组嵌套数组来表示二维表格：

            // (table: (title: ...) (type: ...) (row: ...) (column: ...) (orient: <torient>)
            //         (description: ...) 
            //         (data: 
            //             (( (( 1-1 ) ( 1-2 ) ( 1-3 ) ( 1-4 ))
            //                (( 2-1 ) ( 2-2 ) ( 2-3 ) ( 2-4 ))
            //                (( 34-123 #3,2 )         ( 3-4 ))
            //                (                        ( 4-4 )) )) ) )

            // <<< title ! subtitle ! 

            // 其中 #3,2 意味着本单元格横向吞并 3 格，纵向吞并 2 格

            return markup;
        }

        public static string ToRomanLower(int num) {
            string[,] mapping = new string[4, 10] {
                {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"},
                {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"},
                {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"},
                 {"", "M", "MM", "MMM", ""  , "",  ""  , ""   , ""    , "" }
            };

            return (mapping[3, num / 1000 % 10]  
                    + mapping[2, num / 100 % 10]
                    + mapping[1, num / 10 % 10] 
                    + mapping[0, num % 10]).ToLower();
        }

        public void SetDictionary(Dictionary<string, string> dict, string key, string value) {
            if (dict.ContainsKey(key)) {
                int i = 1;
                while(dict.ContainsKey(key+"_"+i)) {
                    i++;
                }
                dict.Add(key + "_" + i, value);
            }
            else dict.Add(key, value);
        }
    }
}
