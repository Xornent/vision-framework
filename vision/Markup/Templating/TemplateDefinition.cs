using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Templating {

    public static class TemplateDefinition {
        public static string System = @"
(template: (name: table) (params: orient, title, description, items, row, column) (content:
(ifbranch: (evaluation:#equal:orient, $[toriention|right]) (content:<table class='table box-right') (otherwise:)) 
(ifbranch: (evaluation:#equal:orient, $[toriention|left]) (content:<table class='table box-left') (otherwise:)) 
(ifbranch: (evaluation:#equal:orient, $[toriention|fill]) (content:<table class='table box-fill') (otherwise:)) 
><tbody>
(ifbranch: (evaluation: title#isempty ) (otherwise:
<tr class='table-title'><td colspan='@{column}'>@{title}</td></tr>
) (content: ))
(ifbranch: (evaluation: description#isempty ) (otherwise:
<tr><td colspan='@{column}'>@{description}</td></tr>
) (content: ))
(for: (define: i_c) (from: 1) (to: @{items#count}) (step: 1) 
(content: 
    (set: (variable: c_1) (value: @{i_c#decrement}))
    (set: (variable: t_row) (value: @{items#get:c_1}))
    <tr>
        (for: (define: i_d) (from: 1) (to: @{t_row#count}) (step: 1)
        (content: 
            (set: (variable: c_2) (value: @{i_d#decrement}))
            (set: (variable: raw_content) (value: @{t_row#get:c_2}))
            (set: (variable: raw_position) (value: @{raw_content#splitat:$[@],$[1],$[1;1]}))
            <td colspan='@{raw_position#splitat:$[;],$[0],$[_]}' rowspan='@{raw_position#splitat:$[;],$[1],$[_]}'>@{raw_content#splitat:$[@],$[0],$[1;1]}</td> ))
    </tr>))
</tbody></table>
))

(template: (name: list) (params: items)
(content: <ul>
    (for: (define: i) (from: 1) (to: @{items#count}) (step: 1) 
    (content: <li> 
        (set: (variable: c_1) (value: @{i#decrement}))
        @{items#get:c_1} </li> )) </ul> ))

(template: (name: link) (params: redirect, display)
(content: <a href='/home/page/@{redirect}' alt=@{display}>@{display}</a> ))

(template: (name: externallink) (params: redirect, display)
(content: <a href='@{redirect}' alt=@{display}>@{display}</a> ))

(template: (name: image) (params: orient, alt, redirect, width, height, message)
(content:
    (ifbranch: (evaluation: #equal:orient, $[orientionh|right]) 
        (content: <div class='box-padding box-right'> ) 
        (otherwise: <div class='box-padding box-left'> ) )
            <div>
              <img alt='@{alt}' src='@{redirect}' width='@{width}' height='@{height}'/>
              <div class='box-caption' style='width:@{width#minus:$[4]}px'>@{message}</div>
            </div>
          </div>
))

(template: (name: vgallery) (params: orient, redirect, alt, width, height, message)
(content:
    (ifbranch: (evaluation: #equal:orient, $[orientionh|left]) 
        (content: <div class='box-padding box-right'> ) 
        (otherwise: <div class='box-padding box-left'> ) )
            <div>
            <table><tbody>
              (for: (define: i) (from: 1) (to: @{redirect#count}) (step: 1) (content:
              (set: (variable: c_2) (value: @{i#decrement}))
              <tr><td><img alt='@{alt#get:c_2}' src='@{redirect#get:c_2}' width='@{width}' height='auto'/></td></tr>
              ))
            </tbody></table>
              <div class='box-caption' style='width:@{width#minus:$[4]}px'>@{message}</div>
            </div>
    </div>
))

(template: (name: hgallery) (params: orient, redirect, alt, width, height, message)
           (content:
    (ifbranch: (evaluation: #equal:orient, $[orientionh|right]) 
        (content: <div class='box-padding box-right'> ) 
        (otherwise: <div class='box-padding box-left'> ) )
            <div>
            <table><tbody><tr>
              (for: (define: i) (from: 1) (to: @{redirect#count}) (step: 1) (content:
              (set: (variable: c_2) (value: @{i#decrement}))
              <td><img alt='@{alt#get:c_2}' src='@{redirect#get:c_2}' width='auto' height='@{height}'/></td>
              ))
            </tr></tbody></table>
              <div class='box-caption' style='width: auto'>@{message}</div>
            </div>
          </div>
))

(template: (name: code) (params: content) (content: <code>@{content}</code>))

(template: (name: tablequote) (params: orient, width, content)
(content: <table (ifbranch: (evaluation:#equal:orient, $[orientionh|right]) (content:class='quote quote-right') (otherwise:)) 
          (ifbranch: (evaluation:#equal:orient, $[orientionh|left]) (content:class='quote quote-left') (otherwise:)) 
          (ifbranch: (evaluation:#equal:orient, $[orientionh|middle]) (content:class='quote quote-middle') (otherwise:)) 
          style='width: @{width}!important'>
          <tbody> <tr>
            <td valign='top' rowspan='2' class='quote-marker-top'>“</td>
            <td class='quote-body'>@{content}</td>   
            <td valign='bottom' rowspan='2' class='quote-marker-bottom'>”</td>
          </tr> </tbody> </table> )) 

(template: (name: quote) (params: content) (content:
(tablequote: (orient: orientionh|middle) (width: 500px) (content: @{content}))  
))

(template: (name: reference) (params: id)
(content: <sup id='cite_ref-@{id}' class='reference'>
          <a href='#cite_item-@{id}'>[@{id}]</a> </sup> ))

(template: (name: footnote) (params: id)
(content: <sup id='note_ref-@{id}' class='footnote'>
          <a href='#note_item-@{id}'>[@{id}]</a> </sup> ))

(template: (name: indent) (params: content)
(content: <div class='indent'>@{content}</div>))

(template: (name: tocitem) (params: content)
(content: <div class='indent'>@{content}</div>))

(template: (name: toccontainer) (params: content)
(content: <div class='toccontainer'>@{content}</div>))

(template: (name: syntax) (params: language, content)
(content: <pre name='code' class='prettyprint lang-@{language} linenums'>@{content}</pre>))

(template: (name: stub) (params:)
(content: (model: (name: page) (content: 本界面是[[System:短页面|短页面]]（可读内容小于 500 词），需要贡献者添加更详细的引用和文本)) ))
";
    }
}
