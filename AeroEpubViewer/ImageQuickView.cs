using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroEpubViewer.Epub;

namespace AeroEpubViewer
{
    public class ImageQuickView
    {
        public static string GetHTML() 
        {
            
            StringBuilder r=new StringBuilder();
            r.Append("<html><head><style>img{max-height:95vh;max-width:90vw}</style></head><body>");
            foreach (var i in Program.epub.manifest) 
            {
                if (i.Value.mediaType.Contains("image")) 
                {
                    
                    r.Append("<div><img src=\"aeroepub://book/");
                    r.Append(i.Value.href);
                    r.Append("\"/></div>");
                }
            }
            r.Append("</body>");

            return r.ToString() ; 
        }
    }
}
