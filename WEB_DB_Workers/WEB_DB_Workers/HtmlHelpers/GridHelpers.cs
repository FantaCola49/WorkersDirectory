using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WEB_DB_Workers.Models;

namespace WEB_DB_Workers.HtmlHelpers
{
    public static class GridHelpers
    {
        // Отобразил страницы в виде 1 ... 3 4 5 ... Last
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            if (pagingInfo.currentPage > pagingInfo.pagesSide + 1)
            {
                // первая страница
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");

                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(1));
                tag.InnerHtml = "1";

                li.InnerHtml = tag.ToString();
                result.Append(li.ToString());
            }
            int page1 = pagingInfo.currentPage - pagingInfo.pagesSide;
            int page2 = pagingInfo.currentPage + pagingInfo.pagesSide;
            if (page1 < 1)
            {
                page2 = page2 - page1 + 1;
                page1 = 1;
            }
            if (page2 > pagingInfo.totalPages) page2 = pagingInfo.totalPages;
            if (page1 > 2)
            {// ...
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");

                TagBuilder tag = new TagBuilder("span");
                tag.InnerHtml = "...";
                tag.AddCssClass("page-item");
                tag.AddCssClass("disabled");

                li.InnerHtml = tag.ToString();
                result.Append(li.ToString());
            }
            for (int i = page1; i <= page2; i++)
            {// страницы
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (i == pagingInfo.currentPage) li.AddCssClass("active");

                TagBuilder tag = new TagBuilder("a");
                tag.AddCssClass("page-link");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();

                li.InnerHtml = tag.ToString();
                result.Append(li.ToString());
            }
            if (page2 < pagingInfo.totalPages)
            {// ... и последняя страница
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");

                TagBuilder tag = new TagBuilder("span");
                tag.InnerHtml = "...";
                tag.AddCssClass("page-item");
                tag.AddCssClass("disabled");
                li.InnerHtml = tag.ToString();
                result.Append(li.ToString());

                li = new TagBuilder("li");
                li.AddCssClass("page-item");

                tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(pagingInfo.totalPages));
                tag.InnerHtml = pagingInfo.totalPages.ToString();

                li.InnerHtml = tag.ToString();
                result.Append(li.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static IHtmlString SortIdentifier(this HtmlHelper htmlHelper, string sortOrder, string field)
        {
            if (string.IsNullOrEmpty(sortOrder) || (sortOrder.Trim() != field && sortOrder.Replace("_desc", "").Trim() != field)) return null;
            string glyph = "glyphicon glyphicon-chevron-up";
            if (sortOrder.ToLower().Contains("desc"))
            {
                glyph = "glyphicon glyphicon-chevron-down";
            }
            var span = new TagBuilder("span");
            span.Attributes["class"] = glyph;
            return MvcHtmlString.Create(span.ToString());
        }

        public static RouteValueDictionary ToRouteValueDictionary(this NameValueCollection collection, string newKey, string newValue)
        {
            var routeValueDictionary = new RouteValueDictionary();
            foreach (var key in collection.AllKeys)
            {
                if (key == null) continue;
                if (routeValueDictionary.ContainsKey(key))
                    routeValueDictionary.Remove(key);
                routeValueDictionary.Add(key, collection[key]);
            }
            if (string.IsNullOrEmpty(newValue))
            {
                routeValueDictionary.Remove(newKey);
            }
            else
            {
                if (routeValueDictionary.ContainsKey(newKey))
                    routeValueDictionary.Remove(newKey);
                routeValueDictionary.Add(newKey, newValue);
            }
            return routeValueDictionary;
        }
    }
}