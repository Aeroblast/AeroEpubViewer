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
        public static string Hack(string css,Control window) 
        {
            Match m=reg_vh.Match(css);
            while (m.Success) 
            {
                int vh=int.Parse(m.Groups[1].Value);
                int px = window.Height * vh / 100;
                var builder = new StringBuilder();
                builder.Append(css.Substring(0, m.Index));
                builder.Append(string.Format(": {0}px",px));
                builder.Append(css.Substring(m.Index + m.Length));
                css= builder.ToString();
                m = reg_vh.Match(css);
            }
             m = reg_vw.Match(css);
            while (m.Success)
            {
                int vw = int.Parse(m.Groups[1].Value);
                int px = window.Width * vw / 100;
                var builder = new StringBuilder();
                builder.Append(css.Substring(0, m.Index));
                builder.Append(string.Format(": {0}px", px));
                builder.Append(css.Substring(m.Index + m.Length));
                css = builder.ToString();
                m = reg_vw.Match(css);
            }
            return css;
        }
    }
}
