#pragma checksum "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "023520b049263fc93983128e740eedc3a8c92f6d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Register), @"mvc.1.0.view", @"/Views/Account/Register.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Account/Register.cshtml", typeof(AspNetCore.Views_Account_Register))]
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
#line 1 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/_ViewImports.cshtml"
using LibRes.App;

#line default
#line hidden
#line 2 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/_ViewImports.cshtml"
using LibRes.App.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"023520b049263fc93983128e740eedc3a8c92f6d", @"/Views/Account/Register.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b8b1b2a11219abb87ed9877651e2de56e3cf3864", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Register : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<LibRes.App.Models.RegisterModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
  
    ViewBag.Title = "Register";

#line default
#line hidden
            BeginContext(80, 4, true);
            WriteLiteral("<h2>");
            EndContext();
            BeginContext(85, 13, false);
#line 5 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
Write(ViewBag.Title);

#line default
#line hidden
            EndContext();
            BeginContext(98, 8, true);
            WriteLiteral(".</h2>\r\n");
            EndContext();
#line 6 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
 using (Html.BeginForm("Register", "Account", FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
{
    

#line default
#line hidden
            BeginContext(230, 23, false);
#line 8 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
            EndContext();
            BeginContext(255, 48, true);
            WriteLiteral("    <h4>Create a new account.</h4>\r\n    <hr />\r\n");
            EndContext();
            BeginContext(308, 58, false);
#line 11 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
Write(Html.ValidationSummary("", new { @class = "text-danger" }));

#line default
#line hidden
            EndContext();
            BeginContext(368, 38, true);
            WriteLiteral("    <div class=\"form-group\">\r\n        ");
            EndContext();
            BeginContext(407, 70, false);
#line 13 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
   Write(Html.LabelFor(m => m.Email, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
            EndContext();
            BeginContext(477, 47, true);
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
            EndContext();
            BeginContext(525, 62, false);
#line 15 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
       Write(Html.TextBoxFor(m => m.Email, new { @class = "form-control" }));

#line default
#line hidden
            EndContext();
            BeginContext(587, 68, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        ");
            EndContext();
            BeginContext(656, 73, false);
#line 19 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
   Write(Html.LabelFor(m => m.Password, new { @class = "col-md-2 control-label" }));

#line default
#line hidden
            EndContext();
            BeginContext(729, 47, true);
            WriteLiteral("\r\n        <div class=\"col-md-10\">\r\n            ");
            EndContext();
            BeginContext(777, 66, false);
#line 21 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
       Write(Html.PasswordFor(m => m.Password, new { @class = "form-control" }));

#line default
#line hidden
            EndContext();
            BeginContext(843, 215, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <div class=\"col-md-offset-2 col-md-10\">\r\n            <input type=\"submit\" class=\"btn btn-default\" value=\"Register\" />\r\n        </div>\r\n    </div>\r\n");
            EndContext();
#line 29 "/Users/dneychev/Projects/LibRes/LibRes.App/Views/Account/Register.cshtml"
}

#line default
#line hidden
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<LibRes.App.Models.RegisterModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
