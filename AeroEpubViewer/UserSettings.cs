using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroEpubViewer
{
    class UserSettings
    {
        public static int bookFontSize = 18;
        public static string viewerStyle = "warm";

        static string jsonTemplate = "\"bookFontSize\":{0},\"viewerTheme\":\"{1}\"";
        public static string GetJson() 
        {
            return "{"+string.Format(jsonTemplate,bookFontSize,viewerStyle)+"}";
        }
    }
}
