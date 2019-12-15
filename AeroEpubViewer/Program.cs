using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            Cef.Initialize(settings);
            epub =new  EpubFile(args[0]);
            Application.Run(new EpubViewer());
      




        }
    }

 
}
