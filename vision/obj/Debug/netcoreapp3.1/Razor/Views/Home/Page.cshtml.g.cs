#pragma checksum "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\Home\Page.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e7f282446eaea1c3c6d0b10419a4f9c8f364dae2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Page), @"mvc.1.0.view", @"/Views/Home/Page.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\_ViewImports.cshtml"
using Vision;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\_ViewImports.cshtml"
using Vision.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e7f282446eaea1c3c6d0b10419a4f9c8f364dae2", @"/Views/Home/Page.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3c029e0582d750bb1125ebd1bd790db1024a4483", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Page : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<(Page _Page, Record _Record)>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\Home\Page.cshtml"
  
    ViewData["Title"] = Model._Page.Title.Split('#')[0] ;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>");
#nullable restore
#line 6 "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\Home\Page.cshtml"
Write(Model._Page.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n\r\n");
#nullable restore
#line 8 "G:\ASP.NET Sites\Acamedia 3.1\Vision\Views\Home\Page.cshtml"
Write(new Microsoft.AspNetCore.Html.HtmlString(ViewBag.HtmlPage));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<div class=\"placeholder\"><p>&nbsp;</p></div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<(Page _Page, Record _Record)> Html { get; private set; }
    }
}
#pragma warning restore 1591
