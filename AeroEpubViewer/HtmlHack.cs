using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AeroEpubViewer
{
    class HtmlHack
    {
        static string cssInject = "<link href=\"aeroepub://viewer/viewer-inject.css?random={0}\" rel=\"stylesheet\" type=\"text/css\"/>";
        static string jsInject = "<script src=\"aeroepub://viewer/viewer-inject.js?random={0}\"></script>";
        static Regex regLink = new Regex("(<link +href=\".*?)(\".*?>)");
        //static Regex regHref = new Regex("(<.*? href=\".*?\")(.*?>)");
        static Regex regScript = new Regex("<script.*?/>");
        public static string Hack(string html)
        {
            int random = Util.RandomRange();
            
            html = regScript.Replace(html, "");
            html = regLink.Replace(html, "$1?r=" + random + "$2");
            //html = regHref.Replace(html, "$1 onclick=\"Href(this);event.stopPropagation();return false;\"$2");
            cssInject = string.Format(cssInject, random);
            jsInject = string.Format(jsInject, random);
            html = html.Replace("</head>", cssInject + "\n</head>").Replace("</body>", jsInject + "\n</body>");
            return html;
        }
        public static void LoadUser()
        {
            if (UserSettings.userBookCssContent != null) cssInject += "<link href=\"aeroepub://app/UserBookCss\" rel=\"stylesheet\" type=\"text/css\"/>";
            if (Program.epub.spine.pageProgressionDirection == "rtl")
            {
                if (UserSettings.userBookCssContent_rtl != null) cssInject += "<link href=\"aeroepub://app/UserBookCssRtl\" rel=\"stylesheet\" type=\"text/css\"/>";
            }
            else
            {
                if (UserSettings.userBookCssContent_ltr != null) cssInject += "<link href=\"aeroepub://app/UserBookCssLtr\" rel=\"stylesheet\" type=\"text/css\"/>";
            }
        }
    }
}
