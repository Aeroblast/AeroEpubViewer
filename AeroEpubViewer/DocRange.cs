using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace AeroEpubViewer
{
    class DocRange
    {
        public DocPoint start, end;
        public string preview, href;
        public TocItem tocPosition;
        public DocRange(XmlNode startNode, int startOffset, XmlNode endNode, int endOffset, string href)
        {
            start = new DocPoint(startNode, startOffset);
            end = new DocPoint(endNode, endOffset);
            preview = startNode.Value.Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", " ");
            tocPosition = new TocManager().GetPosition(href, start);
            this.href = href;
        }
        public string ToJSON()
        {
            return $"{{\"start\":{start.ToJSON()},\"end\":{end.ToJSON()},\"href\":\"{href}\",\"preview\":\"{preview}\",\"toc\":\"{tocPosition.ToString()}\"}}";
        }
    }
    class DocPoint : IComparable<DocPoint>
    {
        public string selector;
        public int nodeOffset, textOffset;
        public List<int> values = new List<int>();
        public DocPoint(XmlNode node, int offset)
        {
            if (node.ParentNode == null)
            {
                selector = "html";
                values.Add(0);
            }
            else
            {
                if (node.NodeType == XmlNodeType.Text)
                    selector = GetSelector(node.ParentNode);
                else
                    selector = GetSelector(node);
            }
            values.TrimExcess();
            var c = node.ParentNode.ChildNodes;
            for (int i = 0; i < c.Count; i++)
                if (c[i] == node)
                { nodeOffset = i; break; }
            textOffset = offset;
        }
        string GetSelector(XmlNode e)
        {
            if (e.Name == "html") return "html";
            var c = e.ParentNode.ChildNodes;
            int elementCount = 0;
            foreach (XmlNode n in c)
            {
                if (n == e)
                {
                    values.Add(elementCount + 1);
                    return GetSelector(e.ParentNode) + ">:nth-child(" + (elementCount + 1) + ")";
                }
                if (n.NodeType == XmlNodeType.Element) elementCount++;
            }
            throw new XMLException("Parent Node??");
        }

        public string ToJSON()
        {
            return $"{{\"selector\":\"{selector}\",\"nodeOffset\":{nodeOffset},\"textOffset\":{textOffset}}}";
        }


        public int CompareTo(DocPoint other)
        {
            int i = 0;
            int r = 0;
            while (i < values.Count && i < other.values.Count)
            {
                r = values[i].CompareTo(other.values[i]);
                if (r != 0) break;
                i++;
            }
            return r;
        }

        public static bool operator >(DocPoint operand1, DocPoint operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        public static bool operator <(DocPoint operand1, DocPoint operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        public static bool operator >=(DocPoint operand1, DocPoint operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        public static bool operator <=(DocPoint operand1, DocPoint operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
    }
}
