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
            if (Program.epub.version == "2.0") Parse2();
        }
        static string ncxPath;
        public static void Parse2()
        {
            var f = Program.epub.toc.GetFile() as TextEpubItemFile;
            ncxPath = f.fullName;
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
                            pt.url = Util.ReferPath(ncxPath, e.Attributes["src"].Value);
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
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(f.text);
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
