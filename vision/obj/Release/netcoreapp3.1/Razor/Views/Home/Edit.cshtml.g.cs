#pragma checksum "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "be0bb5461817d964437f5597a9a46852b01622f1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Edit), @"mvc.1.0.view", @"/Views/Home/Edit.cshtml")]
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
#line 1 "G:\Github\source\xornent\projects\vision-framework\vision\Views\_ViewImports.cshtml"
using Vision;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "G:\Github\source\xornent\projects\vision-framework\vision\Views\_ViewImports.cshtml"
using Vision.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"be0bb5461817d964437f5597a9a46852b01622f1", @"/Views/Home/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3c029e0582d750bb1125ebd1bd790db1024a4483", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<(Page _Page, Record _Record)>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/editor/min/vs/loader.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
  
    ViewData["Title"] = "编辑";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>编辑： ");
#nullable restore
#line 6 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
   Write(Html.Raw(Model._Page.Title.Split('#')[0]));

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n<div id=\"monaco-div\" style=\"width: 100%;height: 450px; margin-top: 15px; margin-bottom:15px\">\r\n</div>\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "be0bb5461817d964437f5597a9a46852b01622f14625", async() => {
                WriteLiteral("\r\n    <input style=\"display:none\" name=\"id\" id=\"id-id\"");
                BeginWriteAttribute("value", " value=\"", 317, "\"", 340, 1);
#nullable restore
#line 11 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
WriteAttributeValue("", 325, Model._Page.Id, 325, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n    <input style=\"display:none\" name=\"tags\" id=\"id-tags\"");
                BeginWriteAttribute("value", " value=\"", 402, "\"", 433, 1);
#nullable restore
#line 12 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
WriteAttributeValue("", 410, Model._Record.Category, 410, 23, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n    <textarea style=\"display:none\" name=\"history\" id=\"id-hist\">");
#nullable restore
#line 13 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
                                                          Write(Model._Record.Body);

#line default
#line hidden
#nullable disable
                WriteLiteral("</textarea>\r\n    <p><b>此页面的标识：</b><br /></p>\r\n    <div class=\"tagsinput-primary\">\r\n        <input name=\"tagsinput\" id=\"tagsinputval\" class=\"tagsinput\" data-role=\"tagsinput\"");
                BeginWriteAttribute("value", " value=\"", 693, "\"", 701, 0);
                EndWriteAttribute();
                WriteLiteral(@" placeholder=""输入后回车"" />
    </div>
    <p><b>编辑和更改摘要：</b><br></p>
    <div class=""form-group""><textarea style=""margin: 0px 12px; height: 80px;"" name=""summary"" id=""id-summary"" placeholder=""用简短的几句话概括本次编辑的主要内容，填写编辑摘要（可选的）"" class=""form-control""></textarea></div>

    <input type=""submit"" value=""提交对文档的更改"" style=""width:180px; height:30px; margin-top:20px;"" onclick=""return loadup()"" />
");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "be0bb5461817d964437f5597a9a46852b01622f18059", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
<script>

    var meditor = null;

    // Language Services
    require.config({ paths: { 'vs': '../../editor/min/vs' } });
    require(['vs/editor/editor.main'], function () {

        // Register a new language
        monaco.languages.register({ id: 'vmkup' });

        // Register a tokens provider for the language
        monaco.languages.setMonarchTokensProvider('vmkup', {
            tokenizer: {
                root: [
                    [/\([^)]*\:/gm, ""cmd""],
                    [/=====[^)]*=====/gm, ""h5""],
                    [/====[^)]*====/gm, ""h4""],
                    [/===[^)]*===/gm, ""h3""],
                    [/==[^)]*==/gm, ""h2""],
                    [/\{\{\{[^)]*\}\}\}/gm, ""template""],
                    [/\{\{[^)]*\}\}/gm, ""reference""],
                    [/\[\[[^)]*\]\]/gm, ""link""],
                    [/\[\[\[[^)]*\]\]\]/gm, ""weblink""],
                    [/\{\[[^)]*\]\}/gm, ""table""],
                    [/\<\<\<[^)]*\>\>\>/gm, ""gallery""],
               ");
            WriteLiteral(@"     [/\<\:[^)]*\:\>/gm, ""tag""],
                    [/;;;;;;/gm, ""showref""]
                ]
            }
        });

        // Define a new theme that contains only rules that match this language
        monaco.editor.defineTheme('vmkupTheme', {
            base: 'vs',
            inherit: false,
            rules: [
                { token: 'h2', foreground: '993333' },
                { token: 'h3', foreground: '993366' },
                { token: 'h4', foreground: '993399' },
                { token: 'h5', foreground: '9933cc' },
                { token: 'cmd', foreground: '9933ff' },
                { token: 'reference', foreground: '006600' },
                { token: 'template', foreground: 'ff00cc' },
                { token: 'weblink', foreground: '0033ff' },
                { token: 'link', foreground: '0033ff' },
                { token: 'table', foreground: '0033ff' },
                { token: 'image', foreground: 'ff6600' },
                { token: 'gallery', foregroun");
            WriteLiteral(@"d: '006666' },
                { token: 'tag', foreground: 'cc6600' },
                { token: 'showref', foreground: '990033' }
            ]
        });

        // Register a completion item provider for the new language
        monaco.languages.registerCompletionItemProvider('vmkup', {
            provideCompletionItems: () => {
                var suggestions = [{
                    label: 'p',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '::${1:Paragraph}::'
                }, {
                    label: 'para',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '::${1:Paragraph}::'
                }, {
                    label: 'link',
                    kind: monaco.langu");
            WriteLiteral(@"ages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '[[${1:Link}|${2:Display}]]'
                }, {
                    label: 'weblink',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '[[[${1:Link}|${2:Display}]]]'
                }, {
                    label: 'h2',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '==${1:Title}=='
                }, {
                    label: 'h3',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    i");
            WriteLiteral(@"nsertText: '===${1:Title}==='
                }, {
                    label: 'h4',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '====${1:Title}===='
                }, {
                    label: 'h5',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '=====${1:Title}====='
                }, {
                    label: 'h6',
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                    insertText: '======${1:Title}======'
                }
                ];
                return { suggestions: suggestions };
            }
        });

        // impo");
            WriteLiteral("rt * as monaco from \'../editor/esm/vs/editor/editor.api.js\';\r\n        meditor = monaco.editor.create(document.getElementById(\"monaco-div\"), {\r\n            value: \"");
#nullable restore
#line 135 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
               Write(Html.Raw(Model._Record.Body.Replace("\r", @"\r").Replace("\t", @"\t").Replace("\n", @"\n").Replace("\"",@"\"+"\"")));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""",
            language: ""vmkup"",

            lineNumbers: ""on"",
            roundedSelection: false,
            scrollBeyondLastLine: true,
            readOnly: false,
            theme: ""vmkupTheme"",
        });
    });

        var initialized = false;

    function loadup() {
        try {
            var ibd = document.getElementById(""id-hist"");
            var str = meditor.getValue();
            ibd.value = str;

            var itg = document.getElementById(""id-tags"");
            var tgs = $(""#tagsinputval"").val();
            itg.value = tgs;

            if (!initialized) {
                $(""#tagsinputval"").tagsinput(""add"", """);
#nullable restore
#line 159 "G:\Github\source\xornent\projects\vision-framework\vision\Views\Home\Edit.cshtml"
                                                Write(Html.Raw(Model._Record.Category));

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n                initialized = true;\r\n            }\r\n            return true;\r\n        } catch (ex) { return false; }\r\n    }\r\n\r\n    setTimeout(loadup, 800);\r\n</script>\r\n");
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
