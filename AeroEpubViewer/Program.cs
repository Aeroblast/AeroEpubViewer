using System;
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
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string []args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Cef.EnableHighDPISupport();
            var settings = new CefSettings();

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "aeroepub",
                SchemeHandlerFactory = new AeroEpubSchemeHandlerFactory()
            });
            settings.CachePath = Path.GetTempPath()+"AEVCache";
            Cef.Initialize(settings);
            if (args.Length > 0)
                if (File.Exists(args[0])) 
                {
                    epub = new EpubFile(args[0]);
                    Application.Run(new EpubViewer());
                }
            Util.DeleteDir(settings.CachePath);
        }
    }

 
}
