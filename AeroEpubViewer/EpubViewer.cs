﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Web;
using CefSharp.Handler;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    public class EpubViewer : Form
    {
        public static ChromiumWebBrowser chromium;

        public EpubViewer()
        {
            chromium = new ChromiumWebBrowser("aeroepub://viewer/viewer.html");
            chromium.BrowserSettings.WebSecurity = CefState.Disabled; ;
            this.Controls.Add(chromium);
            chromium.Dock = DockStyle.Fill;
            chromium.IsBrowserInitializedChanged += OnLoad;
            chromium.LoadingStateChanged += SendDataWhenLoad;
            ResizeEnd += (e, arg) => {
                if (Size.Equals(ResizeManage.lastSize)) return;
                chromium.Reload(true); chromium.LoadingStateChanged += SendDataWhenLoad;
                ResizeManage.lastSize = Size;
            };
            InitializeComponent();
            this.Text = string.Format("AeroEpubViewer - {0}", Program.epub.title);
            
        }
        private void OnLoad(Object sender, EventArgs e)
        {
            chromium.ShowDevTools();
        }

   

        public static void SendDataWhenLoad(Object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == true) return;
            string jsCmd = "";
            if (Program.epub.spine.pageProgressionDirection == "rtl")
            {
                chromium.ExecuteScriptAsync("direction = direction_rtl;");
            }
            foreach (var i in Program.epub.spine)
            {
                jsCmd += string.Format(",'{0}'", "aeroepub://book/" + i.ToString());
            }
            jsCmd = string.Format("Init([{0}],{1},{2})", jsCmd.Substring(1),ResizeManage.index,ResizeManage.percent);
            chromium.ExecuteScriptAsync(jsCmd);
            if (Program.epub.spine.toc != null) 
            {
                string toc = (Program.epub.spine.toc.GetData() as TextEpubItemFile).text;
               Match m= Regex.Match(toc, "<navMap>([\\s\\S]*?)</navMap>");
                if (m.Success)
                {
                    chromium.ExecuteScriptAsync("LoadToc", m.Groups[1],Path.GetDirectoryName(Program.epub.spine.toc.href));
                }
                else 
                {
                    Log.log("[Error]at TOC loading:"+ Program.epub.spine.toc);
                }
            }
            chromium.LoadingStateChanged -= SendDataWhenLoad;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            var size = new System.Drawing.Size();
            if (Program.epub.spine.pageProgressionDirection == "rtl")
            {
                size.Width = Screen.PrimaryScreen.WorkingArea.Width * 4 / 5;
                size.Height = Screen.PrimaryScreen.WorkingArea.Height * 4 / 5;
            }
            else
            {
                size.Height = Screen.PrimaryScreen.WorkingArea.Height * 4 / 5;
                size.Width = size.Height * 4 / 5;
            }
            ResizeManage.lastSize = size;
            this.ClientSize = size;
            this.Name = "EpubViewer";
            this.ResumeLayout(false);

        }
    }


}