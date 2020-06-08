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
        public static string cachePath = Path.GetTempPath() + "AEVCache";
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
                if (File.Exists(args[0]))
                {
                    ReadBook(args[0]);
                }
                else { MessageBox.Show("文件不存在:" + args[0]); }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Title = "请选择书";
                dialog.Filter = "ePUB电子书(*.epub)|*.epub";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ReadBook(dialog.FileName);
                }
            }
            try
            {
                Util.DeleteDir(cachePath);
            }
            catch (Exception) { }

        }
        static void ReadBook(string bookPath)
        {
            try
            {
                epub = new EpubFile(bookPath);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("该文件无法打开，可能已被其他程序打开：" + bookPath);
                return;
            }
            catch (EpubErrorException e)
            {

                MessageBox.Show("读取EPUB时发生错误:" + bookPath + "\n" + e.Message);
                return;
            }
            try
            {
                TocManage.Parse();
            }
            catch (EpubErrorException e)
            {
                Console.WriteLine(e.Message);
            }
            UserSettings.ReadSettings();
            var settings = new CefSettings();
            if (epub.language != "")
                settings.Locale = Util.TrimLanguageCode(epub.language);

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "aeroepub",
                SchemeHandlerFactory = new AeroEpubSchemeHandlerFactory()
            });
            settings.CachePath = cachePath;
            Cef.Initialize(settings);
            Cef.EnableHighDPISupport();
            Application.Run(new EpubViewer());
        }
    }



}
