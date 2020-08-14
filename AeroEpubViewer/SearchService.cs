using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    class SearchService
    {
        static string word;
        static Thread workThread;
        static int matchPos = 0;
        static List<DocRange> matches = new List<DocRange>();

        public static void Start(string word)
        {
            if (workThread != null)
            {
                workThread.Abort();
            }
            SearchService.word = word;
            matchPos = 0;

            matches.Clear();
            workThread = new Thread(SearchWorker);
            workState = WorkState.run;
            workThread.Start();
        }
        public static string GetResult(int count)
        {
            string arr = "[]";
            if (end)
            {
                if (count < matches.Count)
                {
                    arr = GetRestResult(count);
                }
                return $"{{\"cmd\":\"Stop\",\"list\":{arr}}}";
            }
            //searching
            workState = WorkState.pause;
            while (working) ;
            if (count < matches.Count)
            {
                arr = GetRestResult(count);
            }
            workState = WorkState.run;
            return $"{{\"cmd\":\"Wait\",\"list\":{arr}}}";
        }
        static string GetRestResult(int count)
        {
            StringBuilder arr = new StringBuilder();
            arr.Append("[");
            for (int i = count; i < matches.Count - 1; i++)
            {
                arr.Append(matches[i].ToJSON());
                arr.Append(',');
            }
            arr.Append(matches[matches.Count - 1].ToJSON());
            arr.Append(']');
            return arr.ToString();
        }
        static string currentHref;
        enum WorkState { idle, run, pause }
        static WorkState workState = WorkState.idle;
        static bool working = false;
        static bool end = true;
        public static void SearchWorker()
        {
            end = false;
            foreach (SpineItemref a in Program.epub.spine)
            {
                if (!a.linear) continue;
                currentHref = a.href;
                var text = (a.item.GetFile() as TextEpubItemFile).text;
                if (text != null)
                    SearchDoc(text);
            }
            end = true;
        }
        static void WorkCheck()
        {
            if (workState == WorkState.pause)
                working = false;
            while (workState == WorkState.pause) ;
            working = true;
        }
        static XmlNode currentStartNode;
        static int currentStartOffset;
        public static void SearchDoc(string text)
        {
            try
            {
                var xml = Xhtml.Load(text);
                SearchNode(xml.GetElementsByTagName("body")[0]);
            }
            catch (XmlException)
            { Log.log("[Error]XML Error"); }

        }
        public static void SearchNode(XmlNode node)
        {
            WorkCheck();
            if (node.NodeType == XmlNodeType.Element)
            {
                switch (node.Name)
                {
                    case "p":
                    case "div":
                        foreach (XmlNode a in node.ChildNodes)
                            SearchNode(a);
                        matchPos = 0;
                        break;
                    case "ruby":
                        foreach (XmlNode a in node.ChildNodes)
                        {
                            if (a.Name == "rt")
                            {
                                foreach (XmlNode b in a.ChildNodes)
                                {
                                    if (b.NodeType == XmlNodeType.Text)
                                    {
                                        string t = b.InnerText;
                                        var i = t.IndexOf(word);
                                        if (i >= 0)
                                            matches.Add(new DocRange(b, i, b, i + word.Length, currentHref));
                                    }
                                }

                            }
                            else { SearchNode(a); }
                        }
                        break;
                    default:
                        foreach (XmlNode a in node.ChildNodes)
                            SearchNode(a);
                        break;

                }
            }
            else if (node.NodeType == XmlNodeType.Text)
            {
                int i = 0;
                string s = node.Value;
                for (; i < s.Length; i++)
                {
                    if (word[matchPos] == s[i])
                    {
                        if (matchPos == 0)
                        {
                            currentStartNode = node;
                            currentStartOffset = i;
                        }
                        matchPos++;
                        if (matchPos == word.Length)
                        {
                            matches.Add(new DocRange(currentStartNode, currentStartOffset, node, i, currentHref));
                            matchPos = 0;
                        }
                    }
                    else { matchPos = 0; }
                }

            }

        }
    }


}
