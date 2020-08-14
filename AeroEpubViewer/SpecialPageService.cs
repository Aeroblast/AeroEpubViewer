using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Xml;
using AeroEpubViewer.Epub;

namespace AeroEpubViewer
{
    public class SpecialPageService
    {
        public static string ImageQuickView()
        {
            var filename = "image-quick-view.html".Replace("/", ".");
            Stream fs = Assembly.GetExecutingAssembly().GetManifestResourceStream("AeroEpubViewer.Res." + filename);
            string content = new StreamReader(fs).ReadToEnd();
            StringBuilder r = new StringBuilder();
            int i = 0;
            foreach (SpineItemref a in Program.epub.spine)
            {
                var item = a.item.GetFile() as TextEpubItemFile;
                if (item == null) continue;
                var text = item.text;
                var xml = Xhtml.Load(text);
                var rs = xml.GetElementsByTagName("img");
                var rs2 = xml.GetElementsByTagName("image");//svg
                var tocm = new TocManager();
                foreach (XmlNode n in rs)
                {
                    string src = Util.ReferPath(a.href, n.Attributes["src"].Value);
                    DocPoint p = new DocPoint(n, 0);
                    var toc = tocm.GetPosition(i, p);
                    string record = $"<div class='item' onclick=\"Direct('{a.href}','{p.selector}')\"><div><img src='aeroepub://book/{src}'></div><div>{toc.ToString()}</div></div>";
                    r.Append(record);
                }
                foreach (XmlNode n in rs2)
                {
                    string src = Util.ReferPath(a.href, n.Attributes["xlink:href"].Value);
                    DocPoint p = new DocPoint(n, 0);
                    var toc = tocm.GetPosition(i, p);
                    string record = $"<div class='item' onclick=\"Direct('{a.href}','{p.selector}')\"><div><img src='aeroepub://book/{src}'></div><div>{toc.ToString()}</div></div>";
                    r.Append(record);
                }
                i++;
            }
            return content.Replace("{0}", r.ToString());
        }



        public static string BookInfo()
        {
            StringBuilder r = new StringBuilder();
            r.Append("<html><head><style>img{max-height:55vh;max-width:90vw}data-item{font-weight:bold;}table{max-width:95%;margin-left:4%;border:none;}</style></head><body>");
            r.Append("<h1>" + Program.epub.title + "</h1>");
            if (Program.epub.cover_img != "") r.Append("<img src=\"aeroepub://book/" + Program.epub.cover_img + "\"/>");
            r.Append("<table>");
            string creators = "";
            foreach (var a in Program.epub.dc_creators)
            {
                string t = a.value;
                var role = a.GetRefines("role");
                if (role != null && Dics.marc2name.ContainsKey(role.value))
                {
                    t += " (" + Dics.marc2name[role.value] + ")";
                }
                if (creators != "") creators += ", ";
                creators += t;
            }
            r.Append("<tr><td>Creator(s)</td><td><data-item>" + creators + "</data-item></td></tr>");
            string lang = "";
            foreach (var a in Program.epub.dc_language)
            {
                if (Dics.langcode.ContainsKey(a.value.ToLower()))
                {
                    lang += Dics.langcode[a.value.ToLower()] + " ";
                }
                else
                {
                    lang += a.value + " ";
                }
            }
            if (lang == "")
            {
                if (Dics.langcode.ContainsKey(Program.epub.language))
                {
                    lang += Dics.langcode[Program.epub.language];
                }
                else
                {
                    lang += Program.epub.language;
                }
            }
            r.Append("<tr><td>Language</td><td><data-item>" + lang + "</data-item></td></tr>");
            foreach (var a in Program.epub.others)
            {
                string name = a.name;
                if (name.IndexOf(':') > 0) name = name.Substring(name.IndexOf(':') + 1);
                name = Char.ToUpper(name[0]) + name.Substring(1);
                string value = a.value;
                switch (a.name)
                {
                    case "dc:contributor":
                        {
                            var role = a.GetRefines("role");
                            if (role != null && Dics.marc2name.ContainsKey(role.value))
                            {
                                value += " (" + Dics.marc2name[role.value] + ")";
                            }
                        }
                        break;
                    case "dc:date":
                        {
                            var dateEvent = a.GetRefines("event");
                            if (dateEvent != null)
                                name += $" ({dateEvent.value})";
                        }
                        break;
                }

                r.Append("<tr><td>" + name + "</td><td><data-item>" + value + "</data-item></td></tr>");
            }
            r.Append("<tr><td>File</td><td><data-item>" + Program.epub.path + "</data-item></td></tr>");
            r.Append("</table><p>Other metadata:</p><table>");
            foreach (var a in Program.epub.dc_identifier)
            {
                string v = a.value;
                var t = a.GetRefines("identifier-type");
                if (t != null) v = t.GetRefines("scheme").value + ":" + v;
                t = a.GetRefines("scheme");
                if (t != null) v = t.value + ":" + v;
                r.Append("<tr><td>Identifier</td><td><data-item>" + v + "</data-item></td></tr>");
            }
            foreach (var a in Program.epub.meta)
            {
                r.Append("<tr><td>" + a.name + "</td><td><data-item>" + a.value + "</data-item></td></tr>");
            }
            r.Append("</table>");
            r.Append("<script src=\"aeroepub://viewer/sp-page.js\"></script>");
            r.Append("</body>");

            return r.ToString();
        }

    }
}
