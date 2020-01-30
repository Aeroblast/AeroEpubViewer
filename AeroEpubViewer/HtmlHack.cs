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
        static Regex regHref = new Regex("(<.*? href=\".*?\")(.*?>)");
        public static string Hack(string html) 
        {
            int random = Util.RandomRange();
            html = regLink.Replace(html, "$1?r="+random+"$2");
            html = regHref.Replace(html, "$1 onclick=\"Href(this);event.stopPropagation();return false;\"$2");
            cssInject = string.Format(cssInject, random);
            jsInject = string.Format(jsInject, random);
            html = html.Replace("</head>", cssInject + "\n</head>").Replace("</body>", jsInject + "\n</body>");
            return html;
        }
    }
}
