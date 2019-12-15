using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Web;
using CefSharp.Handler;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    public partial class EpubViewer : Form
    {
        ChromiumWebBrowser chromium;

        public EpubViewer()
        {
            chromium = new ChromiumWebBrowser("aeroepub://viewer/rtl/viewer.html");
            chromium.BrowserSettings.WebSecurity = CefState.Disabled; ;
            this.Controls.Add(chromium);
            chromium.Dock = DockStyle.Fill;
            chromium.IsBrowserInitializedChanged += OnLoad;

            chromium.LoadingStateChanged += SendUrlListWhenLoad;

            InitializeComponent();
            this.Text = string.Format("AeroEpubViewer - {0}", Program.epub.title);
        }
        private void OnLoad(Object sender, EventArgs e)
        {
            chromium.ShowDevTools();
        }

        void SendUrlListWhenLoad(Object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == true) return;
                string jsCmd = "";
            foreach (var i in Program.epub.spine)
            {
                jsCmd += string.Format(",'{0}'", "aeroepub://book/" + i.ToString());
            }
            jsCmd = string.Format("Init([{0}])", jsCmd.Substring(1));
            chromium.ExecuteScriptAsync(jsCmd);
            chromium.LoadingStateChanged -= SendUrlListWhenLoad;

        }
    }


}