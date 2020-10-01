using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebUI.Html
{
    public static class HtmlPrefixScopeExtensions
    {
        private const string idsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";

        public static IDisposable BeginCollectionItem(
          this HtmlHelper html,
          string collectionName)
        {
            Queue<string> idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
            string str = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();
            html.ViewContext.Writer.WriteLine("<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />", collectionName, html.Encode(str));
            return html.BeginHtmlFieldPrefixScope(string.Format("{0}[{1}]", collectionName, str));
        }

        public static IDisposable BeginHtmlFieldPrefixScope(
          this HtmlHelper html,
          string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
        }

        private static Queue<string> GetIdsToReuse(
          HttpContextBase httpContext,
          string collectionName)
        {
            string str1 = "__htmlPrefixScopeExtensions_IdsToReuse_" + collectionName;
            Queue<string> stringQueue = (Queue<string>)httpContext.Items[str1];
            if (stringQueue == null)
            {
                httpContext.Items[str1] = stringQueue = new Queue<string>();
                string str2 = httpContext.Request[collectionName + ".index"];
                if (!string.IsNullOrEmpty(str2))
                {
                    string str3 = str2;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str4 in str3.Split(chArray))
                        stringQueue.Enqueue(str4);
                }
            }
            return stringQueue;
        }

        private class HtmlFieldPrefixScope : IDisposable
        {
            private readonly TemplateInfo templateInfo;
            private readonly string previousHtmlFieldPrefix;

            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                this.templateInfo = templateInfo;
                previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            public void Dispose()
            {
                templateInfo.HtmlFieldPrefix = previousHtmlFieldPrefix;
            }
        }
    }
}