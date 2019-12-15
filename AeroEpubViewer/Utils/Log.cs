using System;
using System.IO;

public class Log
{
    static string t = "";
    public static string level = "";
    public static void log(string s)
    {
        t += level+s + "\r\n";
        if(s.StartsWith("[Warn")){Console.ForegroundColor=ConsoleColor.Yellow;}
        if(s.StartsWith("[Error")){Console.ForegroundColor=ConsoleColor.Red;}
        if(s.StartsWith("[Info")){Console.ForegroundColor=ConsoleColor.Green;}
        Console.WriteLine(level+s);
        Console.ForegroundColor=ConsoleColor.White;
    }
    public static void Save(string path)
    {
        File.WriteAllText(path, t);
    }

}




