using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vision.Data;
using Vision.Difference.Builder;
using Vision.Markup;
using Vision.Models;

namespace Vision.Controllers {

    /// <summary>
    /// 使用到的 ViewBag 资源：
    ///   1.  HtmlPage (string)
    ///   
    /// 各视图使用的模型对象： （泛型类使用 Basic 语法，因为文档注释中不允许中括号）
    ///    
    ///  View                   Model
    /// ------------------------------------------------------
    ///  About                  null
    ///  Create                 null
    ///  Debug                  IEnumerable(of Page)
    ///  Delete                 Page
    ///  Edit                   Tuple (of Page, of Record)
    ///  Error                  ManagedError
    ///  Index                  null
    ///  Page                   Tuple (of Page, of Record)
    ///  Privacy                null
    ///  
    /// View Page 约定：
    /// 
    ///  View Data                说明
    /// ---------------------------------------------------------
    ///  ViewData["IsPage"]       本页是否为内容页
    ///  ViewData["PageId"]       如果是内容页，获取Id
    ///  ViewData["PageTitle"]    获取页标题
    ///  ViewData["IsCategory"]
    ///  ViewData["CategoryId"]
    ///  
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();

        private readonly PageContext _ctx_pages;
        private readonly RecordContext _ctx_records;
        private readonly CategoryContext _ctx_categories;
        private readonly UserContext _ctx_users;

        private readonly string CurrentUserName = "";
        private bool IsUser = false;

        private readonly Markup.MarkupParser MarkupParser = new Markup.MarkupParser();
        private readonly Difference.DifferParser DifferParser = new Difference.DifferParser();

        public HomeController(ILogger<HomeController> logger,
            PageContext ctxPages,
            RecordContext ctxRecord,
            CategoryContext ctxCategory,
            UserContext ctxUser) {
            _logger = logger;

            _ctx_pages = ctxPages;
            _ctx_categories = ctxCategory;
            _ctx_records = ctxRecord;
            _ctx_users = ctxUser;

            // 加载系统预设的模板

            MarkupParser.FromSource(Markup.Templating.TemplateDefinition.System, _ctx_pages, _ctx_users);

            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";
        }

        #region Index
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }
        #endregion

        #region Page

        // 我们在页的视图中尝试两次缓存操作：
        //
        // 其一，就是在请求中设置五分钟的服务器内容缓存。但本缓存只针对于 Page 的读操作；其写操作
        //       详见 Edit 方法是即时更新的。

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Page(string? id) {
            if (id == null) return ManagedPageNoInputError();

            var pageItem = Utilities.Query.GetPageAndRecordByMd5(_ctx_pages, _ctx_records, Cryptography.MD5.Encrypt(id));
            if (!pageItem.Success)
                return ManagedPageNotFoundError(Cryptography.MD5.Encrypt(id) + " (" + id + ")");

            ViewBag.Namespace = "";

            // 其二，我们在 Edit 提交后应该立即读取生成的 Diff 数据，从而直接生成当前历史版本的文本
            //       Vision 标记语言。 加入 Record.Body 中没有数据（这件事情本不应发生，但以方万一）
            //       我们就重新使用 Record.History 和 Record.Use 属性生成 Body 属性；反之，我们就直接
            //       使用 Record.Body.
            //
            // 注意：在管理员审核过程中如果更改了 Use 属性，一定要重新生成，否则本次检查也无法生成正确
            //       版本的界面内容。 如果一定不想在当时生成，可以将 Record.Bosy 设成空，或空格字符
            //       以强制执行此方法。

            if (!string.IsNullOrWhiteSpace(pageItem.Record.Body)) {
                (string p, MarkupDocument doc) res = MarkupParser.FromSource(pageItem.Record.Body, pageItem.Result, pageItem.Record,
                    null, ViewBag.Namespace, _ctx_pages, _ctx_users);
                ViewBag.HtmlPage = res.p;
                pageItem.Result.Models = res.doc.StringifyModels;
            } else {
                pageItem.Record.Body = DifferParser.Build(pageItem.Record.History, pageItem.Record.Use);
                _ctx_records.Update(pageItem.Record);
                await _ctx_records.SaveChangesAsync();
                (string p, MarkupDocument doc) res = MarkupParser.FromSource(pageItem.Record.Body, pageItem.Result, pageItem.Record,
                    null, ViewBag.Namespace, _ctx_pages, _ctx_users);
                pageItem.Result.Models = res.doc.StringifyModels;
                ViewBag.HtmlPage = res.p;
            }

            pageItem.Record.Changes = DifferParser.GetChanges(pageItem.Record.History);

            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = true;
            ViewData["PageId"] = pageItem.Result.Id;
            ViewData["PageTitle"] = pageItem.Result.Title;
            if (pageItem.Result.Title.StartsWith("Talk:"))
                ViewData["PageTitle"] = pageItem.Result.Title.Remove(0, 5);

            return View((pageItem.Result, pageItem.Record));
        }
        #endregion

        #region Privacy
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult Privacy() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }
        #endregion

        #region About
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult About() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }
        #endregion

        #region Edit
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Edit(int? id) {
            if (id == null) return ManagedPageNoInputError();

            var pageItem = Utilities.Query.GetPageAndRecordById(_ctx_pages, _ctx_records, (int)id);
            if (!pageItem.Success) return ManagedPageNotFoundError((int)id);

            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";
            return View((pageItem.Result, pageItem.Record));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Edit(int id, IFormCollection formData) {
            var fdata = new EditFormData()
            {
                History = formData["history"],
                Id = Convert.ToInt32(formData["id"]),
                Tag = formData["tags"]
            };
            string summary = formData["summary"];
            summary = summary.Replace("<", "&lt;").Replace(">", "&gt;").Replace(" ", "&nbsp;").Replace("\n", "<br/>");
            if (id != fdata.Id) return ManagedPageNotFoundError(id);

            var pageItem = Utilities.Query.GetPageAndRecordById(_ctx_pages, _ctx_records, id);
            if (!pageItem.Success) return ManagedPageNotFoundError(id);

            // 我们从 FormData 中获得了最新修改的内容 Body，需要和旧版本的 History 进行比对，
            // 然后得出新版本的差异报告，附加在 History 之后。

            string diff = DifferParser.Generate(pageItem.Record.Body, fdata.History);
            int v = DifferParser.GetVersion(pageItem.Record.History) + 1;

            string user = Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            try {
                string temp = User.Claims.Where(claim => claim.Type == "display").ToList()[0].Value;
                user = temp;
            } catch { }
            string addHist = "\n:" + v + "\n" +
                "@user " + user + "\n" +
                "@datetime " + DateTime.Now.ToString() + "\n" +
                "@summary " + summary + "\n" +
                diff;

            pageItem.Record.History = pageItem.Record.History + addHist;

            // 这里我们得到了当前版本的内容，我们还要生成历史一版本的内容，然后我们构建两个 VML 文档，
            // 并对比两个文档的 Category 差异

            pageItem.Record.Body = DifferParser.Build(pageItem.Record.History);
            string historyVersion = DifferParser.Build(pageItem.Record.History, v - 1);

            MarkupDocument present = MarkupParser.NotExecuteFromSource(pageItem.Record.Body);
            MarkupDocument past = MarkupParser.NotExecuteFromSource(historyVersion);
            List<string> p_cats = present.DetectCategories();
            List<string> h_cats = past.DetectCategories();

            foreach (var item in p_cats) {
                if (!h_cats.Contains(item)) {
                    var cat = Utilities.Query.GetCategoryByName(_ctx_categories, item);
                    cat.AddPage(pageItem.Result.Namespace + ":" + pageItem.Result.Title);
                    _ctx_categories.Update(cat);
                }
            }

            foreach (var item in h_cats) {
                if (!p_cats.Contains(item)) {
                    var cat = Utilities.Query.GetCategoryByName(_ctx_categories, item);
                    cat.DeletePage(pageItem.Result.Namespace + ":" + pageItem.Result.Title);
                    _ctx_categories.Update(cat);
                }
            }

            await _ctx_categories.SaveChangesAsync();
            pageItem.Record.Category = fdata.Tag;

            if (true) {
                try {
                    _ctx_pages.Update(pageItem.Result);
                    _ctx_records.Update(pageItem.Record);
                    await _ctx_pages.SaveChangesAsync();
                    await _ctx_records.SaveChangesAsync();
                } catch { }
            }

            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            return Redirect("../Page/" + pageItem.Result.Namespace + ":" + HttpUtility.UrlEncode(pageItem.Result.Title, Encoding.UTF8).Replace("+", "%20"));
        }
        #endregion

        #region Edit Category
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult EditCategory(int? id) {
            if (id == null) return ManagedPageNoInputError();

            var pageItem = Utilities.Query.GetCategoryById(_ctx_categories, (int)id);
            if (pageItem == null) return ManagedPageNotFoundError((int)id);

            pageItem.AliasBody = DifferParser.Build(pageItem.Alias);

            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";
            return View(pageItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> EditCategory(int id, IFormCollection formData) {
            var fdata = new EditFormData()
            {
                History = formData["alias"],
                Id = Convert.ToInt32(formData["id"])
            };
            if (id != fdata.Id) return ManagedPageNotFoundError(id);

            var pageItem = Utilities.Query.GetCategoryById(_ctx_categories, id);
            if (pageItem == null) return ManagedPageNotFoundError(id);

            if (string.IsNullOrEmpty(pageItem.AliasBody))
                pageItem.AliasBody = DifferParser.Build(pageItem.Alias);
            string diff = DifferParser.Generate(pageItem.AliasBody, fdata.History);
            int v = DifferParser.GetVersion(pageItem.Alias) + 1;

            string user = Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            try {
                string temp = User.Claims.Where(claim => claim.Type == "display").ToList()[0].Value;
                user = temp;
            } catch { }
            string addHist = "\n:" + v + "\n" +
                "@user " + user + "\n" +
                "@datetime " + DateTime.Now.ToString() + "\n" +
                "@summary " + "\n" +
                diff;

            pageItem.Alias = pageItem.Alias + addHist;

            // 这里我们得到了当前版本的内容，我们还要生成历史一版本的内容，然后我们构建两个 VML 文档，
            // 并对比两个文档的 Category 差异

            pageItem.AliasBody = DifferParser.Build(pageItem.Alias);
            string historyVersion = DifferParser.Build(pageItem.Alias, v - 1);

            MarkupDocument present = MarkupParser.NotExecuteFromSource(pageItem.AliasBody);
            MarkupDocument past = MarkupParser.NotExecuteFromSource(historyVersion);
            List<string> p_cats = present.DetectCategories();
            List<string> h_cats = past.DetectCategories();

            foreach (var item in p_cats) {
                if (!h_cats.Contains(item)) {
                    var cat = Utilities.Query.GetCategoryByName(_ctx_categories, item);
                    cat.AddCategory(pageItem.Name);
                    _ctx_categories.Update(cat);
                }
            }

            foreach (var item in h_cats) {
                if (!p_cats.Contains(item)) {
                    var cat = Utilities.Query.GetCategoryByName(_ctx_categories, item);
                    cat.DeleteCategory(pageItem.Name);
                    _ctx_categories.Update(cat);
                }
            }

            await _ctx_categories.SaveChangesAsync();

            if (true) {
                try {
                    _ctx_categories.Update(pageItem);
                    await _ctx_categories.SaveChangesAsync();
                } catch { }
            }

            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            return Redirect("../Portal/" + HttpUtility.UrlEncode(pageItem.Name, Encoding.UTF8).Replace("+", "%20") + "|1-1");
        }
        #endregion

        #region Create Category
        public IActionResult CreateCategory() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> CreateCategory([Bind("Id,Name,Pages,Alias")] Category cat) {
            cat.Name = cat.Name.Trim();
            cat.Pages = "";
            cat.Alias = "";
            var check = Utilities.Query.GetCategoryByName(_ctx_categories, cat.Name);
            if(check != null) {
                ManagedError err = new ManagedError()
                {
                    Title = "已存在相应的词条（或界面）",
                    Details = "要创建的标题已存在，请导航至已存在的界面：" +
                    "<a href='../Home/Category/" + check.Name + "|1-1'>" + check.Name + "</a>"
                };
                return View("Error", err);
            }

            _ctx_categories.Category.Add(cat);
            await _ctx_categories.SaveChangesAsync();
            return Redirect("/Home/Portal/" + HttpUtility.UrlEncode(cat.Name, Encoding.UTF8).Replace("+", "%20") + "|1-1");
        }
        #endregion

        #region Create
        public IActionResult Create() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Create([Bind("Hash,Id,Title,Namespace,Level")] Page page) {

            if (page.Namespace == "系统") page.Namespace = "System";
            if (page.Namespace == "特殊") page.Namespace = "Special";
            if (page.Namespace == "分类") page.Namespace = "Category";
            if (page.Namespace == "不可见") page.Namespace = "Invisible";

            // 从输入的 Form Data 中获取文件名（和命名空间名）
            // 这两个名称是直接存储在 Page.Title 中的。（详见文档注释说明）

            page.Hash = Cryptography.MD5.Encrypt(page.Namespace + ":" + page.Title);
            page.Level = 0;

            // 查找标题是否与已有的重复

            var check = Utilities.Query.GetPageByMd5(_ctx_pages, page.Hash);
            if (check.Success) {
                ManagedError err = new ManagedError()
                {
                    Title = "已存在相应的词条（或界面）",
                    Details = "要创建的标题（和对应的 MD5 摘要）已存在，请导航至已存在的界面：" +
                    "<a href='../Home/Page/" + check.Result.Title + "'>" + check.Result.Title + "</a>"
                };
                return View("Error", err);
            }

            // 每一个 Page 对象一定有一个对应的 Record 对象，现在我们创建一个与之 Id 值
            // 相同的 Record 对象。 （详见 Record 文档注释说明）

            // 对于 Record 类中 History, Category, Use 的描述，详见 Record 文档注释说明

            Record record = new Record()
            {
                Id = page.Id,
                Body = "",
                Category = "",
                History = "",
                Use = 0
            };

            _ctx_records.Add(record);
            _ctx_pages.Add(page);

            // 现在，我们为每个界面创建它所有的 Talk 界面，如果标题中前导 Talk:
            // 我们会报错，因为用户不能创建命名空间的界面

            if (page.Title.ToLower().StartsWith("talk:")) {
                return IllegalNamespaceError(page.Title);
            }

            Models.Page pageTalk = new Page()
            {
                Title = page.Title,
                Level = 0,
                Namespace = "Talk",
                Hash = Cryptography.MD5.Encrypt("Talk:" + page.Title)
            };

            Record recordTalk = new Record()
            {
                Body = "",
                Category = "",
                History = "",
                Use = 0
            };

            _ctx_records.Add(recordTalk);
            _ctx_pages.Add(pageTalk);

            await _ctx_records.SaveChangesAsync();
            await _ctx_pages.SaveChangesAsync();

            // 点击 “创建界面” 按钮后，用户被自动转到新建的界面的编辑器中。

            check = Utilities.Query.GetPageByMd5(_ctx_pages, page.Hash);
            if (check.Success)
                return Redirect("../Home/Edit/" + check.Result.Id);
            else return ManagedPageRegistryFailed();
        }
        #endregion

        #region Delete
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Delete(int? id) {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            if (id == null) return ManagedPageNoInputError();

            var result = Utilities.Query.GetPageById(_ctx_pages, (int)id);
            if (result.Success)
                return View(result.Result);
            return ManagedPageNotFoundError((int)id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var pageItem = Utilities.Query.GetPageAndRecordById(_ctx_pages, _ctx_records, id);
            pageItem.Result.Namespace = "Deleted";
            var talkPage = Utilities.Query.GetPageAndRecordByMd5(_ctx_pages, _ctx_records, Cryptography.MD5.Encrypt("Talk:" + pageItem.Result.Title));
            talkPage.Result.Namespace = "DeletedTalk";

            int v = DifferParser.GetVersion(pageItem.Record.History) + 1;

            string user = Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            try {
                string temp = User.Claims.Where(claim => claim.Type == "display").ToList()[0].Value;
                user = temp;
            } catch { }
            string addHist = "\n:" + v + "\n" +
                "@user " + user + "\n" +
                "@datetime " + DateTime.Now.ToString() + "\n" +
                "@summary " + "删除了界面" +
                "@delete";
            pageItem.Record.History = pageItem.Record.History + addHist;

            v = DifferParser.GetVersion(talkPage.Record.History) + 1;
            string addHist2 = "\n:" + v + "\n" +
                "@user " + user + "\n" +
                "@datetime " + DateTime.Now.ToString() + "\n" +
                "@summary " + "删除了当前界面的讨论界面" +
                "@delete";
            talkPage.Record.History = talkPage.Record.History + addHist2;

            _ctx_pages.Update(pageItem.Result);
            _ctx_pages.Update(talkPage.Result);
            _ctx_records.Update(pageItem.Record);
            _ctx_records.Update(talkPage.Record);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Debug
        public IActionResult Debug() {
            ViewData["IsCategory"] = false;
            ViewData["CategoryId"] = 0;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View(_ctx_pages.Page);
        }
        #endregion

        #region Portal
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None)]
        public IActionResult Portal(string? id) {
            ViewData["IsCategory"] = true;
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            string pagestr = id.Split('|')[1].Trim();
            int catpage = int.Parse(pagestr.Split('-')[0]);
            int pagepage = int.Parse(pagestr.Split('-')[1]);
            Category cat = Utilities.Query.GetCategoryByName(_ctx_categories, id.Split('|')[0].Trim());

            ViewData["CategoryId"] = cat.Id;

            List<string> lines = cat.Pages.Split('\n').ToList();
            cat.PageList.Clear();
            cat.SubCategoryList.Clear();
            cat.CurrentCategory = catpage;
            cat.CurrentPage = pagepage;

            char lastpage = ' ';
            char lastcat = ' ';

            List<string> sorted = new List<string>();
            foreach (var item in lines) {
                if (!string.IsNullOrWhiteSpace(item)) {
                    sorted.Add(item);
                }
            }

            sorted.Sort((left, right) => {
                string eva_left = "";
                if(left.StartsWith("@page")) {
                    eva_left = left.Remove(0, 5).Trim();
                    if (eva_left.Contains(":")) eva_left = eva_left.Split(':')[1].Trim();
                } else if (left.StartsWith("@cat")) {
                    eva_left = left.Remove(0, 4).Trim();
                    if (eva_left.Contains(":")) eva_left = eva_left.Split(':')[1].Trim();
                }

                string eva_right = "";
                if (right.StartsWith("@page")) {
                    eva_right = right.Remove(0, 5).Trim();
                    if (eva_right.Contains(":")) eva_right = eva_right.Split(':')[1].Trim();
                } else if (left.StartsWith("@cat")) {
                    eva_right = right.Remove(0, 4).Trim();
                    if (eva_right.Contains(":")) eva_right = eva_right.Split(':')[1].Trim();
                }

                eva_left = Vision.Utilities.Encoding.GetCapitalSpellCode(eva_left.Substring(0, 1)).ToUpper() + eva_left;
                eva_right = Vision.Utilities.Encoding.GetCapitalSpellCode(eva_right.Substring(0, 1)).ToUpper() + eva_right;

                return eva_right.CompareTo(eva_left);
            });

            foreach (var item in lines) {
                if (item.StartsWith("@page")) {
                    string t = item.Remove(0, 5).Trim();
                    string first = Vision.Utilities.Encoding.GetCapitalSpellCode(t.Substring(0, 1)).ToUpper();
                    if (t.Contains(":")) first = Vision.Utilities.Encoding.GetCapitalSpellCode(t.Split(':')[1].Substring(0, 1)).ToUpper();
                    if (t[0] == lastpage) {
                        cat.PageList.Add("<li><a href='/Home/Page/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "'>" + t + "</a></li>");
                    } else {
                        if (lastpage == ' ') {
                            cat.PageList.Add("<h3>" + first + "</h3>");
                            cat.PageList.Add("<li><a href='/Home/Page/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "'>" + t + "</a></li>");
                            lastpage = first[0];
                        } else {
                            cat.PageList.Add("<h3>" + first + "</h3>");
                            cat.PageList.Add("<li><a href='/Home/Page/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "'>" + t + "</a></li>");
                            lastpage = first[0];
                        }
                    }
                } else if (item.StartsWith("@cat")) {
                    string t = item.Remove(0, 4).Trim();
                    string first = Vision.Utilities.Encoding.GetCapitalSpellCode(t.Substring(0, 1)).ToUpper();
                    if(t.Contains(":")) first = Vision.Utilities.Encoding.GetCapitalSpellCode(t.Split(':')[1].Substring(0, 1)).ToUpper();
                    if (t[0] == lastcat) {
                        cat.SubCategoryList.Add("<li><a href='/Home/Portal/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "|1-1'>" + t + "</a></li>");
                    } else {
                        if (lastcat == ' ') {
                            cat.SubCategoryList.Add("<h3>" + first + "</h3>");
                            cat.SubCategoryList.Add("<li><a href='/Home/Portal/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "'|1-1>" + t + "</a></li>");
                            lastcat = first[0];
                        } else {
                            cat.SubCategoryList.Add("<h3>" + first + "</h3>");
                            cat.SubCategoryList.Add("<li><a href='/Home/Portal/" + HttpUtility.UrlEncode(t, Encoding.UTF8).Replace("+", "%20") + "'|1-1>" + t + "</a></li>");
                            lastcat = first[0];
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(cat.AliasBody))
                cat.AliasBody = DifferParser.Build(cat.Alias);
            (string p, MarkupDocument doc) res = MarkupParser.FromSource(cat.AliasBody, _ctx_pages, _ctx_users);
            cat.AliasHtml = res.p;

            return View(cat);
        }

        public IActionResult CategoryPager(IFormCollection form) {
            int id = int.Parse(form["id"]);
            int catpage = int.Parse(form["catpage"]);
            int pagepage = int.Parse(form["pagepage"]);
            Category cat = Utilities.Query.GetCategoryById(_ctx_categories, id);
            return Redirect("/Home/Portal/" + HttpUtility.UrlEncode(cat.Name, Encoding.UTF8).Replace("+", "%20") + "|" + catpage + "-" + pagepage);
        }
        #endregion

        public IActionResult Error(ManagedError error) {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View(error);
        }

        #region Auxiliary
        private bool Exists(int id) {
            return _ctx_pages.Page.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View(new RequestError { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ManagedError(string title, string description) {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View("Error", new ManagedError()
            {
                Title = title,
                Details = description
            });
        }

        public IActionResult ManagedPageNotFoundError(int id) {
            return ManagedError(
"找不到请求的界面",

"你正在请求 Id = <code>" + id + "</code> 的界面数据，但是现有的数据库中不存在相对应的 Page 或 Record 对象。");
        }

        public IActionResult ManagedPageNotFoundError(string id) {
            return ManagedError(
"找不到请求的界面",

"你正在请求 Md5 = <code>" + id + "</code> 的界面数据，但是现有的数据库中不存在相对应的 Page 或 Record 对象。");
        }

        public IActionResult IllegalNamespaceError(string id) {
            return ManagedError(
"你不能创建含命名空间的界面，只能分配特定的命名空间",

"你试图创建手动的讨论界面");
        }

        public IActionResult ManagedPageNoInputError() {
            return ManagedError(
"你使用了不合要求的 Page 查找语法",

"你调用了 ~/Home/Page/ 页，但是我们要求你提供查找界面的 UTF8 格式 Title 值。\n " +
"正确的语法是 ~/Home/Page/(utf8 string Title)");
        }

        public IActionResult ManagedPageRegistryFailed() {
            return ManagedError(
"已存储的对象没有找到",

"程序异常，之前存储的对象缺不能查询得到");
        }
        #endregion
    }
}
