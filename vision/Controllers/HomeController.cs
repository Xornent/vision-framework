using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vision.Data;
using Vision.Difference.Builder;
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
    ///  
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();

        private readonly PageContext _ctx_pages;
        private readonly RecordContext _ctx_records;
        private readonly CategoryContext _ctx_categories;
        private readonly UserContext _ctx_users;

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

            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";
        }

        #region Index
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index() {
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
                return ManagedPageNotFoundError(Cryptography.MD5.Encrypt(id)+" ("+id+")");

            ViewBag.Namespace = "";

            // 其二，我们在 Edit 提交后应该立即读取生成的 Diff 数据，从而直接生成当前历史版本的文本
            //       Vision 标记语言。 加入 Record.Body 中没有数据（这件事情本不应发生，但以方万一）
            //       我们就重新使用 Record.History 和 Record.Use 属性生成 Body 属性；反之，我们就直接
            //       使用 Record.Body.
            //
            // 注意：在管理员审核过程中如果更改了 Use 属性，一定要重新生成，否则本次检查也无法生成正确
            //       版本的界面内容。 如果一定不想在当时生成，可以将 Record.Bosy 设成空，或空格字符
            //       以强制执行此方法。

            if (!string.IsNullOrWhiteSpace(pageItem.Record.Body))
                ViewBag.HtmlPage = MarkupParser.FromSource(pageItem.Record.Body, pageItem.Result, pageItem.Record,
                    null, ViewBag.Namespace, _ctx_pages, _ctx_users);
            else {
                pageItem.Record.Body = DifferParser.Build(pageItem.Record.History, pageItem.Record.Use);
                _ctx_records.Update(pageItem.Record);
                await _ctx_records.SaveChangesAsync();
                ViewBag.HtmlPage = MarkupParser.FromSource(pageItem.Record.Body, pageItem.Result, pageItem.Record,
                    null, ViewBag.Namespace, _ctx_pages, _ctx_users);
            }

            ViewData["IsPage"] = true;
            ViewData["PageId"] = pageItem.Result.Id;
            ViewData["PageTitle"] = pageItem.Result.Title;
            if (pageItem.Result.Title.StartsWith("Talk:"))
                ViewData["PageTitle"] = pageItem.Result.Title.Remove(0,5);

            return View((pageItem.Result, pageItem.Record));
        }
        #endregion

        #region Privacy
        [ResponseCache(Duration =3600, Location = ResponseCacheLocation.Any)]
        public IActionResult Privacy() {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }
        #endregion

        #region About
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult About() {
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
            if (id != fdata.Id) return ManagedPageNotFoundError(id);

            var pageItem = Utilities.Query.GetPageAndRecordById(_ctx_pages, _ctx_records, id);
            if (!pageItem.Success) return ManagedPageNotFoundError(id);

            // 我们从 FormData 中获得了最新修改的内容 Body，需要和旧版本的 History 进行比对，
            // 然后得出新版本的差异报告，附加在 History 之后。

            string diff = DifferParser.Generate(pageItem.Record.Body, fdata.History);
            int v = DifferParser.GetVersion(pageItem.Record.History) + 1;
            string addHist = "\n:" + v + "\n" +
                "@user " + ViewBag.User + "\n" +
                "@datetime " + DateTime.Now.ToString() + "\n" +
                "@summary " + "\n" +
                diff;

            pageItem.Record.History = pageItem.Record.History + addHist;
            pageItem.Record.Body = DifferParser.Build(pageItem.Record.History);
            pageItem.Record.Category = fdata.Tag;

            if (true) {
                try {
                    _ctx_pages.Update(pageItem.Result);
                    _ctx_records.Update(pageItem.Record);
                    await _ctx_pages.SaveChangesAsync();
                    await _ctx_records.SaveChangesAsync();
                } catch { }
            }
            return Redirect("../Page/" + HttpUtility.UrlEncode(pageItem.Result.Title, Encoding.UTF8).Replace("+","%20"));
        }
        #endregion

        #region Create
        public IActionResult Create() {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Create([Bind("Hash,Id,Title,Level")] Page page) {

            // 从输入的 Form Data 中获取文件名（和命名空间名）
            // 这两个名称是直接存储在 Page.Title 中的。（详见文档注释说明）

            page.Hash = Cryptography.MD5.Encrypt(page.Title);
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

            if (page.Title.StartsWith("Talk:")) {
                return IllegalNamespaceError(page.Title);
            }

            Models.Page pageTalk = new Page()
            {
                Title = "Talk: " + page.Title,
                Level = 0,
                Hash = Cryptography.MD5.Encrypt("Talk: " + page.Title)
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
            _ctx_pages.Page.Remove(pageItem.Result);
            _ctx_records.Record.Remove(pageItem.Record);
            await _ctx_records.SaveChangesAsync();
            await _ctx_pages.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Debug
        public IActionResult Debug() {
            ViewData["IsPage"] = false;
            ViewData["PageId"] = 0;
            ViewData["PageTitle"] = "";

            return View(_ctx_pages.Page);
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

        public IActionResult ManagedPageNotFoundError(int id) => ManagedError(
            "找不到请求的界面",

            "你正在请求 Id = <code>"+id+"</code> 的界面数据，但是现有的数据库中不存在相对应的 Page 或 Record 对象。");

        public IActionResult ManagedPageNotFoundError(string id) => ManagedError(
            "找不到请求的界面",

            "你正在请求 Md5 = <code>" + id + "</code> 的界面数据，但是现有的数据库中不存在相对应的 Page 或 Record 对象。");

        public IActionResult IllegalNamespaceError(string id) => ManagedError(
            "你不能创建含命名空间的界面，只能分配特定的命名空间",

            "你试图创建手动的讨论界面");

        public IActionResult ManagedPageNoInputError() => ManagedError(
            "你使用了不合要求的 Page 查找语法",

            "你调用了 ~/Home/Page/ 页，但是我们要求你提供查找界面的 UTF8 格式 Title 值。\n " +
            "正确的语法是 ~/Home/Page/(utf8 string Title)");

        public IActionResult ManagedPageRegistryFailed() => ManagedError(
            "已存储的对象没有找到",

            "程序异常，之前存储的对象缺不能查询得到");
        #endregion
    }
}
