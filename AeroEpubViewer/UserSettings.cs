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
        public static int bookFontSize = 18;
        public static string theme = "warm";
        public static string settingsPath = System.AppDomain.CurrentDomain.BaseDirectory + "UserSettings\\";
        public static Dictionary<string, string[]> fontFamilySettings = new Dictionary<string, string[]>();
        static string jsonTemplate = "\"bookFontSize\":{0},\"viewerTheme\":\"{1}\"";
        static string fontSetting = Path.Combine(settingsPath, "font.txt");
        public static string GetJson()
        {
            return "{" + string.Format(jsonTemplate, bookFontSize, theme) + "}";
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
