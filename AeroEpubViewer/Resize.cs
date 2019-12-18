using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Web;
using CefSharp.Handler;
using System.Drawing;
namespace AeroEpubViewer
{
    class ResizeManage
    {
        public static string index="0";
        public static float percent=0;
        public static Size lastSize;
        public static void SetPara(string[] args) 
        {
            if (args.Length == 4) 
            {
                index = args[1];
                percent = -(int.Parse(args[2])/(float)int.Parse(args[3]));
            }
        }

    }
}
