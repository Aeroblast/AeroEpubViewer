using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace AeroEpubViewer
{
    class CssHack
    {
        //Regex reg_block = new Regex("{[\\s\\S!{].*?}");
        //Regex reg_attr = new Regex("[a-zA-Z\\s].*?:[a-zA-Z\\s].*?");
        static Regex reg_vh = new Regex(":[\\s]*([0-9]+)vh");
        static Regex reg_vw = new Regex(":[\\s]*([0-9]+)vw");
        static int logicalWidth=500;
        static int logicalHeight=500;
        static bool screenTested = false;
        public static string Hack(string css) 
        {
            if (!screenTested) Log.log("[Error]Hacking CSS when Screen Untested.");
            if (Program.epub.spine.pageProgressionDirection == "rtl")
            {
                Match m = reg_vw.Match(css);
                while (m.Success)
                {
                    int vw = int.Parse(m.Groups[1].Value);
                    int px = logicalWidth* vw / 100;
                    var builder = new StringBuilder();
                    builder.Append(css.Substring(0, m.Index));
                    builder.Append(string.Format(": {0}px", px));
                    builder.Append(css.Substring(m.Index + m.Length));
                    css = builder.ToString();
                    m = reg_vw.Match(css);
                }
            }
            else 
            {
                Match m = reg_vh.Match(css);
                while (m.Success)
                {
                    int vh = int.Parse(m.Groups[1].Value);
                    int px = logicalHeight* vh / 100;
                    var builder = new StringBuilder();
                    builder.Append(css.Substring(0, m.Index));
                    builder.Append(string.Format(": {0}px", px));
                    builder.Append(css.Substring(m.Index + m.Length));
                    css = builder.ToString();
                    m = reg_vh.Match(css);
                }
            }
            return css;
        }

        public static void SetScreenTest(string []args) 
        {
            logicalWidth = int.Parse(args[1]);
            logicalHeight = int.Parse(args[2]);
            screenTested = true;
        }
    }
}
