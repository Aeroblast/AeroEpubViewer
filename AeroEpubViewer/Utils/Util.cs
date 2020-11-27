using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

class Util
{
    public static void Packup(string src, string outputfullpath)
    {
        if (File.Exists(outputfullpath))
        {
            File.Delete(outputfullpath);
        }
        ZipFile.CreateFromDirectory(src, outputfullpath);
        Log.log("Saved:" + outputfullpath);
    }
    public static void Unzip(string archive_path, string output_dir)
    {
        if (Directory.Exists(output_dir))
        {
            Directory.CreateDirectory(output_dir);
        }
        ZipArchive archive = ZipFile.OpenRead(archive_path);
        archive.ExtractToDirectory(output_dir);
    }
    public static void DeleteDir(string path)
    {
        if (!Directory.Exists(path)) return;
        foreach (string p in Directory.GetFiles(path)) File.Delete(p);
        foreach (string p in Directory.GetDirectories(path)) DeleteDir(p);
        Directory.Delete(path);
    }
    public static void DeleteEmptyDir(string path)
    {
        if (!Directory.Exists(path)) return;
        foreach (string p in Directory.GetDirectories(path)) DeleteEmptyDir(p);
        if (Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0)
            Directory.Delete(path);
    }


    public static string ReferPath(string filename, string refPath)
    {
        string r = Path.GetDirectoryName(filename).Replace('\\', '/');
        string[] parts = refPath.Replace('\\', '/').Split('/');
        foreach (string p in parts)
        {
            if (p == "") continue;

            if (p == "..") { r = Path.GetDirectoryName(r); continue; }
            r = Path.Combine(r + "/", p);
        }
        if (r.StartsWith("/")) { return r.Substring(1); }
        return r;
    }
    public static string Trim(string str)
    {
        int s = 0, e = str.Length - 1;
        for (; s < str.Length; s++) { if (str[s] == ' ' || str[s] == '\t' || str[s] == '\n' || str[s] == '\r') { } else break; }
        for (; e >= 0; e--) { if (str[e] == ' ' || str[e] == '\t' || str[e] == '\n' || str[e] == '\r') { } else break; }
        if (s <= e) return str.Substring(s, e - s + 1);
        else return "";
    }
    public static string Number(int number, int length = 4)
    {
        string r = number.ToString();
        for (int j = length - r.Length; j > 0; j--) r = "0" + r;
        return r;
    }
    public static bool Contains(string[] c, string s) { if (c != null) foreach (string x in c) if (x == s) return true; return false; }


    static Dictionary<string, string> mimeMap = new Dictionary<string, string>
        {
            { ".css","text/css"},
            { ".js","text/javascript"}
        };
    public static string GetMimeType(string filename)
    {
        string a;
        bool r = mimeMap.TryGetValue(Path.GetExtension(filename).ToLower(), out a);
        if (r) return a;
        else return null;

    }
    static Random random = new Random();
    public static int RandomRange(int start = 0, int end = int.MaxValue)
    {
        return random.Next(start, end);
    }
    public static string TrimLanguageCode(string s)
    {
        s = s.ToLower();
        switch (s)
        {
            case "zh-tw":
            case "zh-hant":
                return "zh-TW";
            case "zh-cn":
            case "zh-hans":
            case "zh-hans-cn":
            case "zh":
                return "zh-CN";
            default:
                return s.Substring(0, 2);
        }
    }
    public static string[] LanguageCodes = new string[] { "en", "zh-CN", "zh-TW", "ja" };
    public static bool isLanguageCode(string s)
    {
        foreach (var code in LanguageCodes)
            if (s == code) return true;
        return false;
    }
}

