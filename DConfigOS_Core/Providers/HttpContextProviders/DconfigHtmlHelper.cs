using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Globalization;
using System.IO;
using System.Web.Mvc.Properties;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc.Html;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public static class DconfigHtmlHelper
    {
        public static MvcHtmlString RenderCapatchaScript(this HtmlHelper htmlHelper)
        {
            StringBuilder result = new StringBuilder();
            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.MergeAttribute("src", "https://www.google.com/recaptcha/api.js?render=6Lc_VIYUAAAAAKCnwV5UkdzRNw1INoNGT7DG_6_N");
            result.AppendLine(scriptBuilder.ToString(TagRenderMode.Normal));
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderCapatchaCheckerScript(this HtmlHelper htmlHelper,string formName)
        {
            string script = @"<script>
                grecaptcha.ready(function() {
                    grecaptcha.execute('6Lc_VIYUAAAAAKCnwV5UkdzRNw1INoNGT7DG_6_N', {action: '" + formName + @"'})
                    .then(function(token) {
                        // Verify the token on the server.
                        // Set `token` in a hidden form input.
                        document.getElementById('captcha').value = token;
                    });
                });
            </script> ";
            StringBuilder html = new StringBuilder();
            html.AppendLine(script);
            return new MvcHtmlString(html.ToString());
        }

        public static MvcHtmlString RenderCapatchaCheckerInput(this HtmlHelper htmlHelper)
        {
            StringBuilder result = new StringBuilder();
            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("id", "captcha");
            inputBuilder.MergeAttribute("name", "captcha");
            inputBuilder.MergeAttribute("type", "hidden");
            inputBuilder.MergeAttribute("value", "");
            result.AppendLine(inputBuilder.ToString(TagRenderMode.SelfClosing));
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderContentMetaTags(this HtmlHelper htmlHelper, DConfigModel model)
        {
            StringBuilder result = new StringBuilder();
            var titleBuilder = new TagBuilder("title");
            var descriptionBuilder = new TagBuilder("meta");
            var keywordsBuider = new TagBuilder("meta");
            if (model.ActiveContentInstance.Title != null)
            {
                titleBuilder.SetInnerText(model.ActiveContentInstance.Title);
                result.AppendLine(titleBuilder.ToString(TagRenderMode.Normal));
            }
            descriptionBuilder.MergeAttribute("name", "description");
            keywordsBuider.MergeAttribute("name", "keywords");
            if (model.ActiveContentInstance.MetaDescription != null)
            {
                descriptionBuilder.MergeAttribute("content", model.ActiveContentInstance.MetaDescription);
                result.AppendLine(descriptionBuilder.ToString(TagRenderMode.SelfClosing));
            }
            if (model.ActiveContentInstance.MetaKeywords != null)
            {
                keywordsBuider.MergeAttribute("content", model.ActiveContentInstance.MetaKeywords);
                result.AppendLine(keywordsBuider.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderStringData(this HtmlHelper htmlHelper, DConfigModel model, int fieldId)
        {
            StringBuilder result = new StringBuilder();
            if (htmlHelper.ViewBag.EditingMode == true)
            {
                var parentBuilder = new TagBuilder("span");
                parentBuilder.MergeAttribute("data-ng-controller", "TextEditor");
                parentBuilder.MergeAttribute("style", "position:relative;");
                var spanBuilder = new TagBuilder("span");
                var saveBtnBuilder = new TagBuilder("input");
                parentBuilder.MergeAttribute("data-field-id", fieldId.ToString());
                parentBuilder.MergeAttribute("data-content-id", model.ActiveContentInstance.Id.ToString());
                saveBtnBuilder.MergeAttribute("type", "button");
                saveBtnBuilder.MergeAttribute("data-ng-click", "save()");
                saveBtnBuilder.MergeAttribute("style", "position:absolute;bottom:-30px;right:-60px;");
                saveBtnBuilder.MergeAttribute("value", "Save");
                saveBtnBuilder.MergeAttribute("data-ng-show", "changed==true");
                spanBuilder.MergeAttribute("id", "editablecontent" + fieldId.ToString());
                spanBuilder.MergeAttribute("contenteditable", "true");
                if (model[fieldId] != null)
                {
                    spanBuilder.InnerHtml=model[fieldId];

                }
                parentBuilder.InnerHtml = spanBuilder.ToString(TagRenderMode.Normal) + saveBtnBuilder.ToString(TagRenderMode.SelfClosing);
                result.AppendLine(parentBuilder.ToString(TagRenderMode.Normal));
            }
            else
            {
                var spanBuilder = new TagBuilder("span");
                if (model[fieldId] != null)
                {
                    spanBuilder.SetInnerText(model[fieldId]);
                }
                result.AppendLine(spanBuilder.ToString(TagRenderMode.Normal));
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderRichTextBoxData(this HtmlHelper htmlHelper, DConfigModel model, int fieldId)
        {
            StringBuilder result = new StringBuilder();
            if (model[fieldId] != null)
            {
                result.AppendLine(model[fieldId]);
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderImageData(this HtmlHelper htmlHelper, DConfigModel model, int fieldId)
        {
            StringBuilder result = new StringBuilder();
            var imageBuilder = new TagBuilder("img");
            if (model[fieldId] != null)
            {
                imageBuilder.MergeAttribute("src", model[fieldId]);
                result.AppendLine(imageBuilder.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RenderCustomData(this HtmlHelper htmlHelper, DConfigModel model, int fieldId)
        {
            string value = model[fieldId];
            if (htmlHelper.ViewBag.EditingMode == true)
            {
                StringBuilder result = new StringBuilder();
                var parentBuilder = new TagBuilder("span");
                parentBuilder.MergeAttribute("data-ng-controller", "CustomEditor");
                parentBuilder.MergeAttribute("style", "position:relative;");
                var spanBuilder = new TagBuilder("span");
                var saveBtnBuilder = new TagBuilder("input");
                parentBuilder.MergeAttribute("data-field-id", fieldId.ToString());
                parentBuilder.MergeAttribute("data-content-id", model.ActiveContentInstance.Id.ToString());
                parentBuilder.MergeAttribute("data-intent", model.ActiveContent.ViewType.ViewFields.Single(m => m.Id==fieldId).TypeObj.IntentName);
                saveBtnBuilder.MergeAttribute("type", "button");
                saveBtnBuilder.MergeAttribute("data-ng-click", "runIntentSelector()");
                saveBtnBuilder.MergeAttribute("style", "position:absolute;bottom:-30px;right:-60px;");
                saveBtnBuilder.MergeAttribute("value", "Update");
                if (!String.IsNullOrEmpty(value))
                {
                    string[] values = value.Split(';');
                    if (values.Count() == 3)
                    {
                        string controller = values[0];
                        string action = values[1];
                        string id = values[2];
                        spanBuilder.InnerHtml = htmlHelper.Action(action, controller, new { Id = id }).ToHtmlString();
                    }
                }
                parentBuilder.InnerHtml = spanBuilder.ToString(TagRenderMode.Normal) + saveBtnBuilder.ToString(TagRenderMode.SelfClosing);
                result.AppendLine(parentBuilder.ToString(TagRenderMode.Normal));
                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                if (!String.IsNullOrEmpty(value))
                {
                    string[] values = value.Split(';');
                    if (values.Count() == 3)
                    {
                        string controller = values[0];
                        string action = values[1];
                        string id = values[2];
                        return htmlHelper.Action(action, controller, new { Id = id });

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static MvcHtmlString RenderContentUDCommands(this HtmlHelper htmlHelper, DConfigModel model,int contentId)
        {
            return null;
        }

        public static MvcHtmlString RenderChildContentCreateCommand(this HtmlHelper htmlHelper,DConfigModel model,int contentId)
        {
            return null;
        }

        public static MvcHtmlString LangSwitcher(this UrlHelper url, string Name, RouteData routeData, string lang)
        {
            var liTagBuilder = new TagBuilder("li");
            var aTagBuilder = new TagBuilder("a");
            var routeValueDictionary = new RouteValueDictionary(routeData.Values);
            if (routeValueDictionary.ContainsKey("lang"))
            {
                if (routeData.Values["lang"] as string == lang)
                {
                    liTagBuilder.AddCssClass("active");
                }
                else
                {
                    routeValueDictionary["lang"] = lang;
                }
            }
            aTagBuilder.MergeAttribute("href", url.RouteUrl(routeValueDictionary));
            aTagBuilder.SetInnerText(Name);
            liTagBuilder.InnerHtml = aTagBuilder.ToString();
            return new MvcHtmlString(liTagBuilder.ToString());
        }

        //public static string ValidationSummary(this HtmlHelper htmlHelper, string message, IDictionary<string, object> htmlAttributes)
        //{
        //    if (htmlHelper.ViewData.ModelState.IsValid)
        //    {
        //        return null;
        //    }
        //    string messageSpan;
        //    if (!String.IsNullOrEmpty(message))
        //    {
        //        TagBuilder spanTag = new TagBuilder("span");
        //        spanTag.MergeAttributes(htmlAttributes);
        //        spanTag.MergeAttribute("class", HtmlHelper.ValidationSummaryCssClassName);
        //        spanTag.SetInnerText(message);
        //        messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
        //    }
        //    else
        //    {
        //        messageSpan = null;
        //    }

        //    StringBuilder htmlSummary = new StringBuilder();
        //    TagBuilder unorderedList = new TagBuilder("ul");
        //    unorderedList.MergeAttributes(htmlAttributes);
        //    unorderedList.MergeAttribute("class", HtmlHelper.ValidationSummaryCssClassName);

        //    foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
        //    {
        //        foreach (ModelError modelError in modelState.Errors)
        //        {
        //            string errorText = GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, null /* modelState */);
        //            if (!String.IsNullOrEmpty(errorText))
        //            {
        //                TagBuilder listItem = new TagBuilder("li");
        //                listItem.SetInnerText(errorText);
        //                htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
        //            }
        //        }
        //    }
        //}


        //public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, DConfigFormModel model)
        //{
        //    StringBuilder result = new StringBuilder();
        //    var summary = new TagBuilder("span");
        //    if (!String.IsNullOrEmpty(model.ErrorSummary))
        //    {
        //        summary.SetInnerText(model.ErrorSummary);
        //        summary.MergeAttribute("class", "validation-summary-errors");
        //        result.AppendLine(summary.ToString(TagRenderMode.Normal));
        //    }
        //    return MvcHtmlString.Create(result.ToString());
        //}

        //public static MvcHtmlString FieldValidationMessage(this HtmlHelper htmlHelper, DConfigFormModel model, int FieldId)
        //{
        //    StringBuilder result = new StringBuilder();
        //    var summary = new TagBuilder("span");
        //    if (model.PageFormFieldsErrors.ContainsKey(FieldId))
        //    {
        //        summary.SetInnerText(model.PageFormFieldsErrors[FieldId]);
        //        summary.MergeAttribute("class", "validation-summary-errors");
        //        result.AppendLine(summary.ToString(TagRenderMode.Normal));
        //    }
        //    return MvcHtmlString.Create(result.ToString());
        //}


    }
}
