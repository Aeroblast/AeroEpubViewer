using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    class TocManage
    {
        public static TocItem tocTree;
        public static void Parse()
        {
            if (Program.epub.toc == null) return;
            if (Program.epub.version == "2.0") Parse2();
            if (Program.epub.version == "3.0") Parse3();
        }
        static string tocPath;
        public static void Parse2()
        {
            var f = Program.epub.toc.GetFile() as TextEpubItemFile;
            tocPath = f.fullName;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(f.text);
            var root = xml.GetElementsByTagName("navMap")[0];
            tocTree = new TocItem();
            tocTree.children = new List<TocItem>();
            Parse2Helper(root, tocTree);

        }
        static void Parse2Helper(XmlNode px, TocItem pt)
        {
            foreach (XmlNode e in px.ChildNodes)
            {
                switch (e.Name)
                {
                    case "navLabel":
                        {
                            pt.name = e.InnerText;
                        }
                        break;
                    case "content":
                        {
                            pt.url = Util.ReferPath(tocPath, e.Attributes["src"].Value);
                        }
                        break;
                    case "navPoint":
                        {
                            var n = pt.AddChild();
                            Parse2Helper(e, n);
                        }
                        break;
                }
            }
        }
        public static void Parse3()
        {
            if (Program.epub.toc.mediaType == "application/x-dtbncx+xml")
            {
                Parse2(); return;
            }
            var f = Program.epub.toc.GetFile() as TextEpubItemFile;

            tocPath = f.fullName;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(f.text);
            foreach (XmlElement nav in xml.GetElementsByTagName("nav"))
            {
                if (nav.GetAttribute("epub:type") == "toc")
                {
                    tocTree = new TocItem();
                    tocTree.children = new List<TocItem>();
                    var root = nav.GetElementsByTagName("ol")[0];
                    Parse3Helper(root, tocTree);
                    break;
                }
            }
        }
        static void Parse3Helper(XmlNode px, TocItem pt)
        {
            foreach (XmlNode e in px.ChildNodes)
                if (e.Name == "li")
                {
                    var node = pt.AddChild();
                    foreach (XmlNode a in e.ChildNodes)
                    {
                        if (a.Name == "a" && node.name == "")
                        {
                            node.name = a.InnerText;
                            node.url = Util.ReferPath(tocPath, ((XmlElement)a).GetAttribute("href"));
                            continue;
                        }
                        if (a.Name == "ol")
                        {
                            Parse3Helper(a, node);
                        }
                    }
                }
        }
    }

    class TocManager
    {
        public TocItem GetPosition(string path, DocPoint p)
        {
            int i = 0;
            foreach (SpineItemref itemref in Program.epub.spine)
            {
                if (itemref.href == path) { return GetPosition(i, p); }
                i++;
            }
            throw new Exception("Shouldn't be here");
        }
        public TocItem GetPosition(int docIndex, DocPoint p)
        {
            if (TocManage.tocTree == null) return new TocItem();
            this.p = p;
            var r = SearchToc(TocManage.tocTree, docIndex);
            if (r == null) //the point is before the first toc record
            { r = new TocItem(); }
            return r;
        }
        public string[] GetPlainStruct()
        {
            if (TocManage.tocTree == null) return null;
            List<string> urls = new List<string>();
            foreach (SpineItemref i in Program.epub.spine)
            {
                if (!i.linear) continue;
                urls.Add(i.href);
            }
            string[] plain = new string[urls.Count];
            GetPlainStructHelper(urls, TocManage.tocTree, ref plain);
            return plain;
        }
        public string GetPlainStructJSON()
        {
            if (TocManage.tocTree == null) return "[]";
            StringBuilder r = new StringBuilder();
            r.Append("[\"");
            r.Append(string.Join("\",\"", GetPlainStruct()));
            r.Append("\"]");
            return r.ToString();
        }
        void GetPlainStructHelper(List<string> urls, TocItem p, ref string[] plain, string intro = "")
        {
            foreach (TocItem i in p.children)
            {
                string u = i.url.Split('#')[0];
                int index = urls.IndexOf(u);
                if (index >= 0)
                {
                    if (plain[index] == null)
                        plain[index] = intro+i.name;
                }
                if (i.children != null)
                    GetPlainStructHelper(urls, i, ref plain, intro + i.name + " > ");
            }
        }
        DocPoint p;
        TocItem last;
        TocItem SearchToc(TocItem c, int i)
        {
            foreach (var e in c.children)
            {
                if (e.children == null)
                {
                    int com = Compare(e.docIndex, e.position, i, p);
                    if (com == 0) return e;
                    if (com > 0) return last;
                    last = e;
                }
                else
                {
                    var r = SearchToc(e, i);
                    if (r != null) return r;
                }
            }
            return last;
        }
        int Compare(int i1, DocPoint p1, int i2, DocPoint p2)
        {
            if (i1 != i2) return i1.CompareTo(i2);
            if (p1 == null)
                if (p2 == null) return 0;
                else return -1;
            else
                if (p2 == null) return 1;

            return p1.CompareTo(p2);
        }

    }

    class TocItem
    {
        public List<TocItem> children;
        public TocItem parent;
        public string name = "";
        public string _url;
        public string url
        {
            set
            {
                _url = value;
                int i = 0;
                var spl = _url.Split('#');
                var path = spl[0];
                var id = "";
                if (spl.Length > 1) id = spl[1];
                foreach (SpineItemref itemref in Program.epub.spine)
                {
                    if (itemref.href == path)
                    {
                        docIndex = i;
                        if (id != "")
                        {
                            var f = itemref.item.GetFile() as TextEpubItemFile;
                            var xml = Xhtml.Load(f.text);
                            var x = xml.SelectSingleNode($"//*[@id='{id}']");//Cannot use GetElementById because of DTD not always in  epub
                            if (x != null)
                            {
                                position = new DocPoint(x, 0);
                            }
                        }
                        return;
                    }
                    i++;
                }
                throw new EpubErrorException("Error at parse toc");
            }
            get { return _url; }
        }
        public DocPoint position;
        public int docIndex;
        public TocItem AddChild()
        {
            if (children == null) children = new List<TocItem>();
            TocItem n = new TocItem();
            n.parent = this;
            children.Add(n);
            return n;
        }
        public override string ToString()
        {
            string s = name;
            if (parent != null)
            {
                var t = parent.ToString();
                if (t.Length > 0)
                    s = parent.ToString() + " > " + s;
            }
            return s;
        }
    }
}
