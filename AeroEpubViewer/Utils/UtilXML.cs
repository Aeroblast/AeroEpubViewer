using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AeroEpubViewer
{
    //System.Xml太抠规范了，还是自己简易糊个吧。
    public class XFragment
    {
        int indexInSource;
        public List<XPart> parts = new List<XPart>();
        public XELement root;
        public int originalLength;
        public int IndexInSource(int partIndex)
        {
            int r = 0;
            for (int i = 0; i < partIndex; i++)
            {
                r += parts[i].originalText.Length;
            }
            return r + indexInSource;
        }
        public XFragment(string text, int start)
        {
            if (text[start] != '<') throw new XMLException("XFragment Error:Unexpect Start.");
            indexInSource = start;
            Regex reg_tag = new Regex("<[\\s\\S]*?>");
            int count = 0, pos = start;
            Match m;
            do
            {
                m = reg_tag.Match(text, pos);
                if (!m.Success) { new XMLException("XFragment Error:Unexpect end."); }
                XTag tag = new XTag(text,m);
                if (tag.type == PartType.tag_start) count++;
                if (tag.type == PartType.tag_end) count--;
                if (m.Index > pos) { parts.Add(new XText(text.Substring(pos, m.Index - pos))); }
                parts.Add(tag);
                pos = m.Index + tag.originalText.Length;
            }
            while (count > 0);
            originalLength = m.Index - start + m.Value.Length;
            root = new XELement(this, 0);
        }
        public static XFragment FindFragment(string tagName, string text)
        {
            Regex reg = new Regex("<" + tagName + "[^a-zA-Z]");
            Match m = reg.Match(text);
            if (m.Success)
            {
                XFragment f = new XFragment(text, m.Index);
                return f;
            }
            return null;
        }
        public void Apply(ref string text)
        {
            text = text.Remove(indexInSource, originalLength);
            text = text.Insert(indexInSource, root.ToString());

        }

    }

    public class XELement
    {

        //不包括自己
        public XELement GetElementById(string id)
        {
            foreach (var x in childs)
            {
                if (x.tag.GetAttribute("id") == id) { return x; }
                var r = x.GetElementById(id);
                if (r != null) return r;
            }
            return null;
        }
        public XTag tag { get { return (XTag)doc.parts[tagStartRef]; } }
        string tagname { get { return tag.tagname; } }

        List<XAttribute> attributes { get { return tag.attributes; } }
        XFragment doc;
        public int tagStartRef, tagEndRef = -1;

        public List<XELement> childs = new List<XELement>();
        public XELement parent;
        public XELement(XFragment frag, int start)
        {
            doc = frag;
            this.tagStartRef = start;
            if (doc.parts[tagStartRef].type == PartType.tag_single)
            {
                this.tagEndRef = start;
                return;
            }
            for (int i = start + 1; i < doc.parts.Count; i++)
            {
                if (doc.parts[i].type == PartType.tag_start|| doc.parts[i].type == PartType.tag_single)
                {
                    XELement ele = new XELement(doc, i);
                    ele.parent = this;
                    childs.Add(ele);
                    i = ele.tagEndRef ;
                    continue;
                }
                if (doc.parts[i].type == PartType.tag_end)
                {
                    if (((XTag)doc.parts[i]).tagname == ((XTag)doc.parts[start]).tagname)
                    {
                        tagEndRef = i; break;
                    }
                    else
                    {
                        throw new XMLException("dismatched end tag:"+doc.parts[start]+"..."+doc.parts[i]);
                    }
                }
            }
            if (tagEndRef == -1) throw new XMLException("Failure when close tag.");
        }
        public string innerXHTML
        {
            get
            {
                string r = "";
                for (int i = tagStartRef + 1; i < tagEndRef; i++)
                {
                    r += doc.parts[i].ToString();
                }
                return r;
            }
        }
        public string outerXHTML
        {
            get
            {
                if (doc.parts[tagStartRef].type == PartType.tag_single) return doc.parts[tagStartRef].ToString();
                else
                    return doc.parts[tagStartRef].ToString() + innerXHTML + doc.parts[tagEndRef].ToString();
            }
        }
        public override string ToString()
        {
            return outerXHTML;
        }

    }

    public abstract class XPart
    {
        public PartType type;
        public string originalText;
    }

    public enum PartType
    {
        text, tag_start, tag_end, tag_single
    }
    public class XText : XPart
    {
        string text;
        public XText(string s)
        {
            originalText = s;
            text = s;
            type = PartType.text;
        }
        override public string ToString()
        {
            return text;
        }
    }
    public class XTag : XPart
    {

        /// <param name="pos">找到Tag之后，Tag的起始位置</param>
        public static XTag FindTag(string tagName, string text, ref int pos)
        {
            Regex reg1 = new Regex("<" + tagName + "[\\s]");
            Regex reg2 = new Regex("<" + tagName + "[\\s\\S]*?>");
            Match m1 = reg1.Match(text, pos);
            if (m1.Success)
            {
                Match m2 = reg2.Match(text, m1.Index);
                XTag tag = new XTag(text,m2);
                pos = m2.Index;
                return tag;
            }
            return null;
        }
        public static XTag FindTag(string tagName, string text)
        {
            int i = 0;
            return FindTag(tagName, text, ref i);
        }

        public string tagname;
        public List<XAttribute> attributes;
        public bool isComment = false;
        public string GetAttribute(string name)
        {
            foreach (var att in attributes)
                if (att.name == name)
                {
                    return att.value;
                }
            return "";
        }
        public void SetAttribute(string name, string value)
        {
            foreach (var att in attributes)
                if (att.name == name)
                {
                    att.value = value;
                    return;
                }
            attributes.Add(new XAttribute(name, value));
        }
        public bool RemoveAttribute(string name)
        {
            XAttribute a = null;
            foreach (var att in attributes)
                if (att.name == name)
                {
                    a = att;
                    break;
                }
            if (a == null) return false;
            attributes.Remove(a);
            return true;

        }

        public string[] GetClassNames()
        {
            foreach (var att in attributes)
                if (att.name == "class")
                {
                    return att.value.Split(' ');
                }
            return null;
        }
        public void AddClassName(string classname)
        {
            string[] old = GetClassNames();
            if (!Util.Contains(old, classname))
            {
                if (old == null) { SetAttribute("class", classname); }
                else { SetAttribute("class", GetAttribute("class") + " " + classname); }
            }
        }

        public XTag(string text,Match m)
        {
            originalText = m.Value;

            if (originalText.StartsWith("<!--"))
            {
                Regex reg_comm = new Regex("<!--[\\S\\s]*?-->");
                m = reg_comm.Match(text, m.Index);
                originalText = m.Value;
                isComment = true;
                type = PartType.tag_single;
            }
            else 
            {
                attributes = new List<XAttribute>();
                string intag = originalText.Substring(1, originalText.Length - 2);
                if (intag[intag.Length - 1] == '/')
                {
                    type = PartType.tag_single;
                    intag = intag.Substring(0, intag.Length - 1);
                }
                string[] x = intag.Split(' ');
                tagname = x[0];
                string t = "";
                for (int i = 1; i < x.Length; i++)
                {
                    x[i] = Util.Trim(x[i]);
                    if (x[i].Length == 0) continue;
                    t += x[i];
                    if (_CountSep(t) != 2) { t += ' '; continue; }
                    attributes.Add(new XAttribute(t));
                    t = "";
                }
                if (tagname[0] == '/') { type = PartType.tag_end; tagname = tagname.Substring(1); }
                else if (type != PartType.tag_single) type = PartType.tag_start;
            }


        }
        private int _CountSep(string s){int c=0;int p=0;while(p<s.Length){if(s[p]=='"')c++;p++;}return c;}
        public override string ToString()
        {
            string r = "<";
            if (type == PartType.tag_end) r += "/";
            r += tagname;
            if (attributes != null)
                foreach (var att in attributes)
                {
                    r += string.Format(" {0}=\"{1}\"", att.name, att.value);
                }
            if (type == PartType.tag_single) r += "/";
            r += ">";
            return r;
        }


    }
    public class XAttribute
    {
        public string name, value;
        public XAttribute(string n, string v) { name = n; value = v; }
        public XAttribute(string s)
        {
            int e = s.IndexOf("=");
            name = s.Substring(0, e);
            value = s.Substring(e + 2, s.Length - name.Length - 3);
        }
    }
    public class XMLException : System.Exception
    {
        public XMLException(string s) : base(s) { }
    }
}