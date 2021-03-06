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
using AeroEpub;
namespace AeroEpubViewer
{
    public class EpubViewer : Form
    {
        public static ChromiumWebBrowser chromium;

        public EpubViewer()
        {
            if (UserSettings.viewMode == "paged")
                chromium = new ChromiumWebBrowser("aeroepub://domain/viewer/viewer-paged.html");
            else
                chromium = new ChromiumWebBrowser("aeroepub://domain/viewer/viewer.html");
            chromium.BrowserSettings.WebSecurity = CefState.Disabled;
            Controls.Add(chromium);
            chromium.Dock = DockStyle.Fill;
            chromium.IsBrowserInitializedChanged += OnLoad;
            chromium.LoadingStateChanged += SendDataWhenLoad;

            {
                string[] temp;
                if (UserSettings.fontFamilySettings.TryGetValue(Util.TrimLanguageCode(Program.epub.language), out temp))
                {
                    chromium.BrowserSettings.SerifFontFamily = temp[1];
                    chromium.BrowserSettings.SansSerifFontFamily = temp[2];
                }
            }

            ResizeEnd += (e, arg) =>
            {
                if (Size.Equals(ResizeManage.lastSize)) return;
                chromium.Reload(true); chromium.LoadingStateChanged += SendDataWhenLoad;
                ResizeManage.lastSize = Size;
            };
            Resize += (e, arg) =>
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    chromium.Reload(true); chromium.LoadingStateChanged += SendDataWhenLoad;
                    ResizeManage.lastSize = Size;
                }
            };
            InitializeComponent();
            this.Text = string.Format("AeroEpubViewer - {0}", Program.epub.title);
        }
        private void OnLoad(Object sender, EventArgs e)
        {
            //chromium.ShowDevTools();

            refreshForm = new RefreshForm(() =>
            {
                Activate();
                Focus();
            });
            Invoke(refreshForm);
        }
        public delegate void RefreshForm();
        public RefreshForm refreshForm;


        public static void SendDataWhenLoad(Object sender, LoadingStateChangedEventArgs e)
        {
#if !DEBUG
            try
            {
#endif
            if (e.IsLoading == true) return;
            if (Program.epub.spine.pageProgressionDirection == "rtl")
            {
                chromium.ExecuteScriptAsync("direction = direction_rtl;");
            }
            string userDataCmd = string.Format("LoadUserSettings({0});", UserSettings.GetJson());
            string initCmd = "";
            string lengthDataCmd = "";
            foreach (Itemref i in Program.epub.spine)
            {
                if (!i.linear) continue;
                initCmd += string.Format(",'{0}'", "aeroepub://domain/book/" + i.ToString());
                int l;
                if (i.item.mediaType == "application/xhtml+xml")
                {
                    l = (i.item.GetFile() as TextEpubFileEntry).text.Length;
                }
                else if (i.item.mediaType.Contains("image")) { l = 10; }
                else
                {
                    throw new Exception("Cannot Handle type in spine:" + i.item.mediaType);
                }

                lengthDataCmd += "," + l;
            }
            if (lengthDataCmd.Length == 0) throw new Exception("Spine is empty.");
            lengthDataCmd = $"LoadScrollBar([{ lengthDataCmd.Substring(1)}],{new TocManager().GetPlainStructJSON()});";
            initCmd = string.Format("Init([{0}],{1},{2});", initCmd.Substring(1), ResizeManage.index, ResizeManage.percent);
            chromium.ExecuteScriptAsync(userDataCmd + lengthDataCmd + initCmd);

            if (Program.epub.toc != null)
            {
                switch (Program.epub.toc.mediaType)
                {
                    case "application/x-dtbncx+xml":
                        {
                            string toc = (Program.epub.toc.GetFile() as TextEpubFileEntry).text;
                            Match m = Regex.Match(toc, "<navMap>([\\s\\S]*?)</navMap>");
                            if (m.Success)
                            {
                                chromium.ExecuteScriptAsync("LoadTocNcx", m.Groups[1], Path.GetDirectoryName(Program.epub.toc.href));
                            }
                            else
                            {
                                Log.log("[Error]at TOC loading:" + Program.epub.toc);
                            }
                        }
                        break;
                    case "application/xhtml+xml":
                        {
                            string toc = (Program.epub.toc.GetFile() as TextEpubFileEntry).text;
                            toc = toc.Replace(" href=\"", " hraf=\"");
                            Match m = Regex.Match(toc, "<body[\\s\\S]*?>([\\s\\S]*?)</body>");
                            if (m.Success)
                            {
                                chromium.ExecuteScriptAsync("LoadTocNav", m.Groups[1], Path.GetDirectoryName(Program.epub.toc.href).Replace('\\', '/'));
                            }
                            else
                            {
                                Log.log("[Error]at TOC loading:" + Program.epub.toc);
                            }
                        }
                        break;

                }

            }
            chromium.LoadingStateChanged -= SendDataWhenLoad;
#if !DEBUG
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                Application.Exit();
            }
#endif
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EpubViewer));
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
            // 
            // EpubViewer
            // 
            this.ClientSize = size;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EpubViewer";
            this.ResumeLayout(false);
            ResizeManage.lastSize = Size;

        }
    }


}