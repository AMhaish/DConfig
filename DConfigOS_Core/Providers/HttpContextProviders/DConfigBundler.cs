using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Web.Routing;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public class Bundler
    {
        protected static string BuildHtmlStringFrom(object htmlAttributes)
        {
            // Try and safely cast
            var routeHtmlAttributes = htmlAttributes as IDictionary<string, object> ?? new RouteValueDictionary(htmlAttributes);

            var attributeBuilder = new StringBuilder();

            foreach (var attribute in routeHtmlAttributes)
            {
                attributeBuilder.AppendFormat(" {0}=\"{1}\"", attribute.Key, attribute.Value);
            }

            return attributeBuilder.ToString();
        }
    }

    public class Scripts : Bundler
    {
        public static IHtmlString Render(string path, object htmlAttributes=null)
        {
            var result= System.Web.Optimization.Scripts.Render(path + DConfigRequestContext.Current.ContextId);
            return result;
//            var attributes = BuildHtmlStringFrom(htmlAttributes);
//            string completedTag = string.Empty;
//#if DEBUG
//            var originalHtml = System.Web.Optimization.Scripts.Render(path + DConfigRequestContext.Current.ContextId).ToHtmlString();
//            completedTag = originalHtml.Replace("/>", attributes + "/>");
//#else
//            completedTag = string.Format(
//                "<script src=\"{0}\" {1} />",
//                Scripts.Url(path + DConfigRequestContext.Current.ContextId), attributes);
//#endif
//            return MvcHtmlString.Create(completedTag);
        }
    }

    public class Styles : Bundler
    {
        public static IHtmlString Render(string path, object htmlAttributes=null)
        {
            var result = System.Web.Optimization.Styles.Render(path + DConfigRequestContext.Current.ContextId);
            return result;
            //            var attributes = BuildHtmlStringFrom(htmlAttributes);
            //            string completedTag = string.Empty;
            //#if DEBUG
            //            var originalHtml = System.Web.Optimization.Styles.Render(path + DConfigRequestContext.Current.ContextId).ToHtmlString();
            //            completedTag = originalHtml.Replace("/>", attributes + "/>");
            //#else
            //            completedTag = string.Format(
            //                "<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\"{1} />",
            //                Styles.Url(path + DConfigRequestContext.Current.ContextId), attributes);
            //#endif
            //            return MvcHtmlString.Create(completedTag);
        }
    }
}
