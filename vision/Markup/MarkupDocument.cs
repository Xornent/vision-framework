using Microsoft.AspNetCore.Razor.Language.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Markup.Ast;
using Vision.Markup.Categories;
using Vision.Markup.Enumerations;
using Vision.Models;
using Vision.Utilities;

namespace Vision.Markup {

    public class MarkupDocument : SegmentSection {
        public override string ToString() {
            string html = "<document>";

            if (this.HasChildren)
                foreach (var v in this.Children) {
                    html = html + "\n" + v.ToString();
                }
            return html + "\n</document>";
        }

        public static int EmbedMaximum = 200;
        public int MaximumEmbed { get; set; } = 1;

        public List<string> TemplateKeys = new List<string>();
        public List<string> TemplateDefinitions = new List<string>();
        public List<string> CrashedTemplates = new List<string>();
        public List<string> NotFoundTemplates = new List<string>();

        public List<CategoryItem> Categories = new List<CategoryItem>();
        public Dictionary<string, string> References = new Dictionary<string, string> ();
        public Dictionary<string, string> Footnotes = new Dictionary<string, string>();
        public List<ExternalLink> ExternalLinks = new List<ExternalLink>();

        public List<List<Section>> Models = new List<List<Section>>();
        public List<string> StringifyModels = new List<string>();

        // 这里预定义了所有系统变量和别称表
        // 参见语言标准：预定义的变量

        /*   @{currentweek}              | @{week}           | <int> 在本年度中的周序数 30
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
         */

        public Dictionary<string, DataSection> Variables = new Dictionary<string, DataSection>()
        {
            { "currentweek"     , new IntegerData() },
            { "currentmonth"    , new IntegerData() },
            { "currentday"      , new IntegerData() },
            { "currentdayofweek", new TextData("") },
            { "currentyear"     , new IntegerData() },
            { "currenttime"     , new TextData("") },
            { "currentmonthname", new TextData("") },
            { "currentdayname"  , new TextData("") },
            { "datetime"        , new DateTimeData() },
            { "title"           , new TextData("") },
            { "namespace"       , new TextData("") },
            { "revisionid"      , new IntegerData() },
            { "page"            , new TextData("") },
            { "numberofpages"   , new IntegerData() },
            { "numberofusers"   , new IntegerData() },
            { "revisionuser"    , new TextData("") },
            { "true"            , new BooleanData(true) },
            { "false"           , new BooleanData(false) }
        };

        public DataSection GetVariable (string name) {

            // 值得注意的是，这里的 name 也可以是变量属性组

            string find = name;
            switch (name) {
                case "week": find = "currentweek"; break;
                case "month": find = "currentmonth"; break;
                case "day": find = "currentday"; break;
                case "dow": find = "currentdayofweek"; break;
                case "year": find = "currentyear"; break;
                case "time": find = "currenttime"; break;
                case "monthname": find = "currentmonthname"; break;
                case "dayname": find = "currentdayname"; break;
                case "ns": find = "namespace"; break;
                case "rid": find = "revisionid"; break;
                case "nop": find = "numberofpages"; break;
                case "nou": find = "numberofusers"; break;
                case "ru": find = "revisionuser"; break;
                default:
                    break;
            }
            return Variables[find];
        }

        public void SetVariable (string name, DataSection value) {
            if (Variables.ContainsKey(name))
                Variables[name] = value;
            else
                Variables.Add(name, value);
        }

        public void SetVariable(string name, string value) {
            if (Variables.ContainsKey(name)) {
                if (value.Contains("(")) {
                    Variables[name] = ArrayData.Parse(value);
                } else {
                    Variables[name] = DataSection.Parse(value);
                }
            }else {
                if (value.Contains("(")) {
                    Variables.Add(name, ArrayData.Parse(value));
                } else {
                    Variables.Add(name, DataSection.Parse(value));
                }
            }
        }

        // 用当前的（解析文档过程之前）的环境重新刷新系统变量的值

        public void UpdateSystemVariables(Page page, Record record, User user, string namespaces, PageContext ctx_pages, UserContext ctx_users) {
            SetVariable("currentweek", (DateTime.Today.DayOfYear / 7).ToString());
            SetVariable("currentmonth", DateTime.Today.Month.ToString());
            SetVariable("currentday", DateTime.Now.Day.ToString());

            // 值得注意的是，根据规定，星期日是一周的第一天，此后依次为周一，周二直到周六.
            // 然而在欧洲和亚洲的大部分地区，特别是中国，人们习惯以周一为第一天。 因此，我们将此 Enum 改写，
            // 令周日为第7天。

            SetVariable("currentdayofweek", (((int)(DateTime.Today.DayOfWeek) == 0) ? 7 : (int)(DateTime.Today.DayOfWeek)).ToString());
            SetVariable("currentyear", DateTime.Today.Year.ToString());
            SetVariable("currenttime", DateTime.Now.ToString());
            SetVariable("currentmonthname", DateTime.Today.ToString("MMMM", System.Globalization.CultureInfo.CurrentUICulture));
            SetVariable("currentdayname", DateTime.Today.ToString("DDDD", System.Globalization.CultureInfo.CurrentUICulture));
            SetVariable("datetime", DateTime.Now.ToString());
            SetVariable("title", page.Title);
            SetVariable("namespace", namespaces);
            SetVariable("revisionid", ((record.Use==0)? new Difference.DifferParser().GetVersion(record.History) : record.Use).ToString());
            SetVariable("page", "~/home/page/" + page.Title.Replace(" ", "%20"));
            SetVariable("numberofpages", ctx_pages.Page.Count().ToString());
            SetVariable("numberofusers", ctx_users.User.Count().ToString());
        }

        // 隐式依赖于 ViewBag.Namespace

        public void Execute(Page page, Record record, User user, string namespaces, PageContext ctx_pages, UserContext ctx_users) {
            UpdateSystemVariables(page, record, user, namespaces, ctx_pages, ctx_users);
            ExecuteProgrammables(this.Children, 1);
            foreach (var item in this.Models) {
                ExecuteProgrammables(item, 1);
            }
            ExecuteExplicitBreaks();
            ExecuteCommands();

            foreach (var item in this.Models) {
                string s = "";
                foreach (var child in item) {
                    s = s + child.ToString();
                }
                StringifyModels.Add(s);
            }
        }

        public void ExecuteProgrammables(List<Section> sect, int level = 1) {

            // 沙盒保护：

            // 参数 Level 代表这个方法被调用的层次结构，在许多情况下，如果出现死循环或循环过大将会导致
            // 程序卡死的情况，对服务器造成阻塞。 而这样的卡死在很大程度上是由于编辑的失误引起的。 例如
            // 在模板中自己调用自己而造成的死循环。

            // 我们使用一组保护性沙盒参数来确保不陷入无法退出的循环

            if (this.MaximumEmbed < level) this.MaximumEmbed = level;
            if (level > EmbedMaximum) return;

            int total = sect.Count;
            for(int count = 0;count< total;count++) {
                var item = sect[count];
                Type t = item.GetType();
                if (t == typeof(ParagraphSection)) {
                    ExecuteProgrammables((item as ParagraphSection).Children, level + 1);
                } else if (t == typeof(HeadlineSection)) {
                    ExecuteProgrammables((item as HeadlineSection).Children);
                } else if (t == typeof(ArrayData)) {
                    foreach (var list in (item as ArrayData).Value)
                        ExecuteProgrammables(list, level + 1);
                } else if (t == typeof(ElementSection)) {

                    // 第一步时，我们需要找出所有的程序语句，并执行该程序语句
                    // 这时，调用当前文档的变量列表可以获取所有可用的变量和声明。

                    ElementSection elem = item as ElementSection;
                    elem.Name = elem.Name.Trim();
                    switch (elem.Name.ToLower()) {
                        case "ifbranch":

                            // 语法： (ifbranch: (evaluation: <bool>)
                            //        (content: ...) (otherwise: ...))

                            bool success = false;

                            try {
                                DataSection data = TextData.GetOperationVariables(elem.Parameters["evaluation"].First().Raw, this);
                                if (data is TextData) {
                                    (data as TextData).UpdateVariables(this);
                                    data = DataSection.Parse(data.Content);
                                }
                                if (data.Type == DataSection.DataSectionType.Boolean)
                                    success = (data as BooleanData).Value;
                                else {

                                    // 所有在 evaluation 位置的非布尔值对象就像在 python 中运行的那样
                                    // 全部返回 true

                                    success = true;
                                }
                            } catch (Exception ex) {
                                success = false;
                            }

                            if (success) {
                                List<Section> sections = elem.Parameters["content"];
                                int delta = sections.Count - 1;
                                sect.RemoveAt(count);
                                if (sections.Any())
                                    sect.InsertRange(count, sections);
                                total += delta;
                                count--;
                            } else {
                                List<Section> sections = elem.Parameters["otherwise"];
                                int delta = sections.Count - 1;
                                sect.RemoveAt(count);
                                if (sections.Any())
                                    sect.InsertRange(count, sections);
                                total += delta;
                                count--;
                            }
                            break;

                        case "for":

                            // 语法： (for: (define: define_name) (from: ...) (to: ...) (step: ...) (content: ...))
                            // define 标签向变量库中注册了一个变量，它只能是 int 或 float 类型

                            DataSection from = elem.Parameters["from"].First() as DataSection;
                            DataSection to = elem.Parameters["to"].First() as DataSection;
                            DataSection step = elem.Parameters["step"].First() as DataSection;

                            if (from is TextData) {
                                (from as TextData).UpdateVariables(this);
                                from = DataSection.Parse(from.Content);
                            }
                            if (to is TextData) {
                                (to as TextData).UpdateVariables(this);
                                to = DataSection.Parse(to.Content);
                            }
                            if (step is TextData) {
                                (step as TextData).UpdateVariables(this);
                                step = DataSection.Parse(step.Content);
                            }
                            float f = 0;
                            if (from is IntegerData || from is FloatData)
                                if (to is IntegerData || to is FloatData)
                                    if (step is IntegerData || step is FloatData) {
                                        f = float.Parse(from.Raw);
                                        float _t = float.Parse(to.Raw);
                                        float _s = float.Parse(step.Raw);

                                        string varname = elem.Parameters["define"].First().Raw;
                                        SetVariable(varname, new FloatData());

                                        List<Section> for_replacements = new List<Section>();
                                        for (float temp = f; temp <= _t; temp += _s) {
                                            List<Section> for_item = CopySections(elem.Parameters["content"]);

                                            SetVariable(varname, temp.ToString());
                                            ExecuteProgrammables(for_item, level + 1);
                                            for_replacements.AddRange(for_item);
                                        }
                                        Variables.Remove(varname);

                                        int delta = for_replacements.Count - 1;
                                        sect.RemoveAt(count);
                                        if (for_replacements.Any())
                                            sect.InsertRange(count, for_replacements);
                                        total += delta;
                                        count += delta;
                                    }

                            break;
                        case "template":

                            // 模板声明语句，语法： (template: (name: ...) (content: ...))

                            // 注意：在解析器中，所有声明的模板（无论位置）都是全局可用的，
                            //       在读取解析之后，直接将其元素删除，加到注册表中。

                            string name = (elem.Parameters["name"].First() as DataSection).Raw;
                            this.TemplateDefinitions.Add(name);
                            if (Templating.TemplateRegistry.Registry.ContainsKey(name))
                                this.CrashedTemplates.Add(name);
                            else {
                                Templating.TemplateRegistry.Registry.Add(name, elem.Parameters["content"]);
                                sect.RemoveAt(count);
                                total--; count--;
                            }

                            break;

                        case "set":

                            // 语法： (set: (variable: ... ) (value: ...))

                            // 如果 set 获得的变量名是存在的，则覆盖其值，若不存在，则创建一个新的
                            // 这样 set 语句总不会报错，也省略了 set 和 create 的区分

                            string setvar = elem.Parameters["variable"].First().Raw;
                            DataSection val = null;
                            var valvar = elem.Parameters["value"];
                            ExecuteProgrammables(valvar, level + 1);

                            if (valvar.Count == 1)
                                val = valvar.First() as DataSection;
                            else if (valvar.Count > 1) {

                                // 这是实现的断言（我们考虑在标准中加上）

                                ArrayData arr = new ArrayData();
                                string raw = "";
                                foreach (var obj in valvar) {
                                    arr.Value.Add(new List<Section>() { obj });
                                    raw = raw + "(" + obj.Raw + ")";
                                }
                                arr.Raw = "(" + raw + ")";
                                val = arr;
                            }

                            SetVariable(setvar, val);

                            break;

                        /*   toc                         | -                 | 生成目录
                         *   referencelist               | reflist           | 列举出本页中所有的参考
                         *   externallist                | extlist           | 列举出本页中所有的外部链接
                         *   footnotelist                | footlist          | 列举出本页所有的脚注
                         *   sign                        | -                 | 编者和编辑时间签名
                         *   break                       | br                | 命令文本换行符
                         *   
                         *   这些是其他的命令项，在此不做解析
                         */

                        case "toc":
                            string tocraw = "(toccontainer: (content: <div class='toctitle'>章节目录</div>";
                            List<CategoryItem> categories = new List<CategoryItem>();
                            foreach (var title in this.Children) {
                                if (title is HeadlineSection) {
                                    var ttl = (title as HeadlineSection);
                                    ElementSection tempelem = new ElementSection();
                                    tempelem.Parse("(content: " + ttl.Raw + ")");
                                    var contents = tempelem.Parameters["content"];
                                    if (ttl.Level == 2)
                                        contents.Insert(0, new TextData((categories.Count + 1).ToString() + " "));
                                    else if (ttl.Level == 3)
                                        contents.Insert(0, new TextData((categories.Count).ToString() + "." + (categories.Last().Children.Count + 1).ToString() + " "));
                                    else if (ttl.Level == 3)
                                        contents.Insert(0, new TextData((categories.Count).ToString() + "." + (categories.Last().Children.Count).ToString() +
                                            "." + (categories.Last().Children.Last().Children.Count + 1).ToString() + " "));

                                    if (ttl.Level == 2)
                                        categories.Add(new CategoryItem()
                                        {
                                            Content = contents,
                                            Location = (categories.Count + 1).ToString()
                                        });
                                    else if (ttl.Level == 3)
                                        categories.Last().Children.Add(new CategoryItem()
                                        {
                                            Content = contents,
                                            Location = (categories.Count).ToString() + "_" + (categories.Last().Children.Count + 1).ToString()
                                        });
                                    else if (ttl.Level == 4)
                                        categories.Last().Children.Last().Children.Add(new CategoryItem()
                                        {
                                            Content = contents,
                                            Location = (categories.Count).ToString() + "_" + (categories.Last().Children.Count).ToString() +
                                            "_" + (categories.Last().Children.Last().Children.Count + 1).ToString()
                                        });
                                }
                            }

                            foreach (var cat in categories) {
                                tocraw = tocraw + "" + cat.ToString("tocitem");
                            }
                            tocraw = tocraw + "))";

                            ElementSection temptoc = new ElementSection();
                            temptoc.Parse("(content: " + tocraw + ")");
                            List<Section> toc = temptoc.Parameters["content"];

                            int __delta = toc.Count - 1;
                            sect.RemoveAt(count);
                            if (toc.Any())
                                sect.InsertRange(count, toc);
                            total += __delta;
                            count--;

                            break;

                        case "referencelist":
                            string rawlist = "<div class='ref-list'><ol>";
                            string rawitem = "";
                            List<string> htmllist = new List<string>();
                            int i = 0;
                            foreach (var refs in this.References) {
                                if (!refs.Key.Contains("_")) {
                                    if (!string.IsNullOrWhiteSpace(rawitem)) {
                                        htmllist.Add(rawitem);
                                    }
                                    rawitem = "<li id='cite_item-"+refs.Key+"'><span><sup><b><a href='#cite_ref-" + refs.Key + "'>[0]</a>";
                                    i = 1;
                                } else {
                                    rawitem = rawitem + "<a href='#cite_ref-" + refs.Key + "'>[" + i + "]</a>";
                                    i++;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(rawitem)) {
                                htmllist.Add(rawitem);
                            }

                            foreach (var refs in this.References) {
                                if (!refs.Key.Contains("_")) {
                                    int pc = int.Parse(refs.Key) - 1;
                                    htmllist[pc] = htmllist[pc] + "</b></sup></span><span>" +
                                        refs.Value + "</span></li>";
                                }
                            }

                            foreach (var refs in htmllist) {
                                rawlist = rawlist + refs;
                            }
                            rawlist = rawlist + "</ol></div>";

                            ElementSection el = new ElementSection();
                            el.Parse("(content: " + rawlist + ")");
                            List<Section> l = el.Parameters["content"];

                            int _delta = l.Count - 1;
                            sect.RemoveAt(count);
                            if (l.Any())
                                sect.InsertRange(count, l);
                            total += _delta;
                            count--;

                            break;

                        case "externallist": break;

                        case "footnotelist":
                            rawlist = "<div class='note-list'><ol>";
                            rawitem = "";
                            htmllist = new List<string>();
                            i = 0;
                            foreach (var refs in this.Footnotes) {
                                if (!refs.Key.Contains("_")) {
                                    if (!string.IsNullOrWhiteSpace(rawitem)) {
                                        htmllist.Add(rawitem);
                                    }
                                    rawitem = "<li id='note_item-"+ MarkupParser.ToRomanLower(int.Parse(refs.Key)) + "'><span><sup><b><a href='#note_ref-" + MarkupParser.ToRomanLower(int.Parse(refs.Key)) + "'>[0]</a>";
                                    i = 1;
                                } else {

                                    rawitem = rawitem + "<a href='#note_ref-" + refs.Key + "'>[" + i + "]</a>";
                                    i++;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(rawitem)) {
                                htmllist.Add(rawitem);
                            }

                            foreach (var refs in this.Footnotes) {
                                if (!refs.Key.Contains("_")) {
                                    int pc = int.Parse(refs.Key) - 1;
                                    htmllist[pc] = htmllist[pc] + "</b></sup></span><span>" +
                                        refs.Value + "</span></li>";
                                }
                            }

                            foreach (var refs in htmllist) {
                                rawlist = rawlist + refs;
                            }
                            rawlist = rawlist + "</ol></div>";

                            el = new ElementSection();
                            el.Parse("(content: " + rawlist + ")");
                            l = el.Parameters["content"];

                            _delta = l.Count - 1;
                            sect.RemoveAt(count);
                            if (l.Any())
                                sect.InsertRange(count, l);
                            total += _delta;
                            count--;

                            break;

                        case "sign": break;
                        case "break": break;

                        case "model":
                            #region
                            TextData mdName = (TextData) elem.Parameters["name"][0];
                            mdName.UpdateVariables(this);
                            string rawmd = "";
                            var list_md = elem.Parameters["content"];
                            foreach (var li in list_md) {
                                rawmd = rawmd + li.Raw;
                            }

                            switch(mdName.Content.ToLower()) {
                                case "info":
                                    string _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/info.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "warn":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-warning'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/warning.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "warnsevere":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-warningsevere'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/warning.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "page":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/page.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "system":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/system.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "history":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/history.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                case "category":
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/category.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                                default:
                                    _inf = @"
    <table class='box-Current plainlinks metadata ambox ambox-notice'>
          <tbody>
              <tr>
                  <td class='mbox-image'>
                    <div style = 'width: 52px; margin-left: 10px;'>
                    <img src='/images/page-markers/info.png' alt='' width='48' data-file-width='48' data-file-height='48' /></div>
                </td>
                <td class='mbox-text'>
                    <div class='mbox-text-span'>" + rawmd + @"</div>
                </td>
            </tr>
        </tbody>
    </table> ";
                                    this.Models.Add(new List<Section>() { new TextData(_inf) });
                                    break;
                            }
                            #endregion

                            break;

                        default:

                            // 其他的模板替代

                            if (!this.TemplateKeys.Contains(elem.Name.ToLower()))
                                this.TemplateKeys.Add(elem.Name.ToLower());

                            if (Templating.TemplateRegistry.Registry.ContainsKey(elem.Name.ToLower())) {

                                // 添加命名变量

                                // Issue 1 2020.08.06

                                // 这里我们使用保留键，为了应对如下情况：
                                // (a: ... （注册变量x）
                                //      ... (a: ... （重复注册变量x），此处将原来的 x 覆盖，此处保留键
                                //              ...
                                //          ) 此处将 x 变量直接移除
                                //          Solve 1 在此处加回保留键
                                //      ... Issue 1.1 再此处使用父元素的 x 时，发现找不到 x，因为已经被移除
                                // ) Issue 1.2 无法移除 x

                                // 我们在进入每一个模板时，检查有没有重名的键，有就临时储存起来，
                                // 这样，在模板执行完时加回，就恢复了父元素的运行状态。

                                Dictionary<string, DataSection> reserved = new Dictionary<string, DataSection>();
                                foreach (var vars in elem.Parameters) {
                                    if (this.Variables.ContainsKey(vars.Key)) {
                                        reserved.Add(vars.Key, Variables[vars.Key]);
                                    }
                                }

                                foreach (var vars in elem.Parameters) {
                                    if (vars.Value.Count == 1) {
                                        DataSection vari = vars.Value.First() as DataSection;
                                        if (vari is TextData) (vari as TextData).UpdateVariables(this);
                                        this.SetVariable(vars.Key, vari);
                                    } else if (vars.Value.Count > 1) {

                                        // 这是实现的断言（我们考虑在标准中加上）

                                        ArrayData arr = new ArrayData();
                                        string raw = "";
                                        foreach (var obj in vars.Value) {
                                            if (obj is TextData) (obj as TextData).UpdateVariables(this);
                                            arr.Value.Add(new List<Section>() { obj });
                                            raw = raw + "(" + obj.Raw + ")";
                                        }
                                        arr.Raw = "(" + raw + ")";
                                        this.SetVariable(vars.Key, arr);
                                    }
                                }

                                var template = Templating.TemplateRegistry.Registry.GetValueOrDefault(elem.Name.ToLower());

                                // TextData 激活化

                                List<Section> templ = CopySections(template);
                                ExecuteProgrammables(templ, level + 1);

                                foreach (var vars in elem.Parameters) {
                                    this.Variables.Remove(vars.Key);
                                }

                                // Issue Solve 1: 加回保留键

                                foreach (var vars in reserved) {
                                    this.SetVariable(vars.Key, vars.Value);
                                }

                                int delta = templ.Count - 1;
                                sect.RemoveAt(count);
                                if (templ.Any())
                                    sect.InsertRange(count, templ);
                                total += delta;
                                count += delta;

                            } else this.NotFoundTemplates.Add(elem.Name.ToLower());
                            break;
                    }
                } else if (t == typeof(TextData)) {
                    var text = item as TextData;
                    text.UpdateVariables(this);

                    if (text.Content.Contains("(") ||
                        text.Content.Contains("#")) {

                        // 事实上，每个变量都可以是一个数组或一个组合物。因此可以达到传递
                        // 批量参数的效果。 这也导致了许多字符串在进行替换之后不再是一个字符串
                        // 例如下面的例子，替换后引入了非字符串元素，因此我们需要检测并重新解析

                        // 源： vision is @{group} encyclopedia.
                        // 其中 @{group} = ((free) (that) (everyone can edit))

                        string src = "(content:" + text.Content + ")";
                        ElementSection elem = new ElementSection();
                        elem.Parse(src);
                        var replacement = elem.Parameters["content"];
                        int delta = replacement.Count - 1;

                        if (delta == 0 && replacement[0] is TextData) {

                        } else {
                            sect.RemoveAt(count);
                            if (replacement.Any())
                                sect.InsertRange(count, replacement);

                            total += delta;
                            count--;
                        }
                    }
                }
            }
        }

        public void ExecuteExplicitBreaks() {

            // 我们设计显式换行符的目的是在使用命令和逻辑语句时保留默认的换行即分段的功能。
            // 相当于在第一层次下，
            //
            // 源代码：  1   some text
            //           2
            //           3   (for: (define: i) (from: 0) (to: 4) (step:2) (content: 
            //           4       procedure @{i}
            //           5   ) and other texts.
            //
            // 等价于：  1   some text
            //           2
            //           3       procedure 0
            //           4       procedure 2
            //           4       procedure 4 and other texts
            //
            // 而源代码：1   some text
            //           2
            //           3   (for: (define: i) (from: 0) (to: 4) (step:2) (content: 
            //           4       procedure @{i} (break:)
            //           5   ) and other texts.
            //
            // 等价于：  1   some text
            //           2   
            //           3       procedure 0
            //           4
            //           5       procedure 2
            //           6 
            //           7       procedure 4
            //           8
            //           9   and other texts
            // 
            // 注意： break 标签只在一级嵌套中有用

            int total = this.Children.Count;
            for(int i = 0;i<total;i++) {
                var parag = this.Children[i];

                if(parag is ParagraphSection) {
                    List<ParagraphSection> replacement = new List<ParagraphSection>();
                    ParagraphSection temp = new ParagraphSection();
                    foreach (var item in (parag as ParagraphSection).Children) {
                        if (item is ElementSection)
                            if ((item as ElementSection).Name == "br" ||
                               (item as ElementSection).Name == "break") { 
                                replacement.Add(temp);
                                temp = new ParagraphSection();
                                continue; 
                            }
                        temp.Children.Add(item);
                    }

                    if (replacement.Count > 1) {
                        this.Children.RemoveAt(i);
                        this.Children.InsertRange(i, replacement);
                        total += (replacement.Count - 1);
                        i += (replacement.Count - 1);
                    }
                }
            }

        }

        public void ExecuteCommands() {

        }

        public List<Section> CopySections(List<Section> sections) {
            List<Section> dest = new List<Section>();
            for(int i = 0; i < sections.Count; i++) {
                var item = sections[i];
                var t = item.GetType();
                
                if(t == typeof(ArrayData)) {
                    dest.Add(ArrayData.Parse((item as ArrayData).Content));
                } else if(t==typeof(DateTimeData)) {
                    dest.Add(DataSection.Parse((item as DateTimeData).Raw));
                } else if (t == typeof(EnumerationData)) {
                    dest.Add(DataSection.Parse((item as EnumerationData).Raw));
                } else if (t == typeof(IntegerData)) {
                    dest.Add(DataSection.Parse((item as IntegerData).Raw));
                } else if (t == typeof(FloatData)) {
                    dest.Add(DataSection.Parse((item as FloatData).Raw));
                } else if (t == typeof(TextData)) {
                    dest.Add(DataSection.Parse((item as TextData).Raw));
                } else if(t==typeof(ElementSection)) {
                    ElementSection e = new ElementSection();
                    var c = item as ElementSection;
                    e.Name = c.Name;
                    e.Parse(c.Content);
                    dest.Add(e);
                }
            }
            return dest;
        }
    }
}
