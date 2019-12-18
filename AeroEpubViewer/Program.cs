﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CefSharp;
using CefSharp.Web;
using CefSharp.WinForms;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    public static class Program
    {

        public static EpubFile epub;
        public static string cachePath = Path.GetTempPath() + "AEVCache";
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string []args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (args.Length > 0)
                if (File.Exists(args[0])) 
                {
                    epub = new EpubFile(args[0]);
                    var settings = new CefSettings();
                    if(epub.language!="")
                    settings.Locale = epub.language;

                    settings.RegisterScheme(new CefCustomScheme
                    {
                        SchemeName = "aeroepub",
                        SchemeHandlerFactory = new AeroEpubSchemeHandlerFactory()
                    });
                    settings.CachePath =cachePath ;
                    Cef.Initialize(settings);
                    Cef.EnableHighDPISupport();
                    Application.Run(new EpubViewer());
                }
            Util.DeleteDir(cachePath);
        }
    }

 
}
