using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroEpubViewer
{
    class UserSettings
    {
        public static string settingsPath = System.AppDomain.CurrentDomain.BaseDirectory + "UserSettings\\";

        //path
        static string generalSetting = Path.Combine(settingsPath, "general.txt");
        static string fontSetting = Path.Combine(settingsPath, "font.txt");

        //general
        public static string theme = "warm";
        public static string warmColor = "#ffe6a0";
        public static string viewMode = "flow";

        //font
        public static int bookFontSize = 18;
        public static Dictionary<string, string[]> fontFamilySettings = new Dictionary<string, string[]>();

        //user files
        public static string userBookCss = Path.Combine(settingsPath, "user-book.css");
        public static string userBookCssContent = null;
        public static string userBookCss_ltr = Path.Combine(settingsPath, "user-book-ltr.css");
        public static string userBookCssContent_ltr = null;
        public static string userBookCss_rtl = Path.Combine(settingsPath, "user-book-rtl.css");
        public static string userBookCssContent_rtl = null;

        public static string GetJson()
        {
            return "{" +
                $"\"bookFontSize\":{bookFontSize}," +
                $"\"viewerTheme\":\"{theme}\"," +
                $"\"warmColor\":\"{warmColor}\""
                + "}";
        }
        public static void ReadSettings()
        {
            if (File.Exists(fontSetting))
            {
                string line;
                using (var file = new StreamReader(fontSetting))
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] para = GetPara(line);
                        if (para.Length == 2 && para[0] == "BookFontSize") { if (!int.TryParse(para[1], out bookFontSize)) bookFontSize = 18; }
                        if (para.Length >= 3)
                        {
                            string code = Util.TrimLanguageCode(para[0]);
                            if (Util.isLanguageCode(code))
                            {
                                if (!fontFamilySettings.ContainsKey(code))
                                    fontFamilySettings.Add(code, para);
                            }
                        }
                    }
            }
            if (File.Exists(generalSetting))
            {
                string line;
                using (var file = new StreamReader(generalSetting))
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] para = GetPara(line);
                        if (para.Length == 2)
                            switch (para[0])
                            {
                                case "Theme":
                                    theme = para[1].ToLower();
                                    break;
                                case "WarmColor":
                                    warmColor = para[1];
                                    ImageHack.SetWarmColor(warmColor);
                                    break;
                                case "ViewMode":
                                    viewMode = para[1];
                                    break;
                            }
                    }
            }
            if (File.Exists(userBookCss))
            {
                userBookCssContent = File.ReadAllText(userBookCss);
            }
            if (File.Exists(userBookCss_ltr))
            {
                userBookCssContent_ltr = File.ReadAllText(userBookCss_ltr);
            }
            if (File.Exists(userBookCss_rtl))
            {
                userBookCssContent_rtl = File.ReadAllText(userBookCss_rtl);
            }
            HtmlHack.LoadUser();
        }
        public static void WriteSettings()
        {
            if (!Directory.Exists(settingsPath)) Directory.CreateDirectory(settingsPath);
        }
        static string[] GetPara(string line)
        {
            string[] r = line.Split(',');
            for (int i = 0; i < r.Length; i++)
                r[i] = Util.Trim(r[i]);
            return r;
        }
    }
}
