using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Web;
using CefSharp.Handler;
using AeroEpubViewer.Epub;
namespace AeroEpubViewer
{
    public class AeroEpubSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "aeroepub";
        Assembly assembly = Assembly.GetExecutingAssembly();


        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            Log.log(request.Url);
            switch (uri.Host)
            {
                case "book":
                    {
                        if (uri.Query.Contains("footnote"))
                        {
                            return ResourceHandler.FromString("");
                        }
                        var epubItemPath = uri.AbsolutePath;
                        if (epubItemPath[0] == '/') epubItemPath = epubItemPath.Substring(1);
                        ManifestItem i = Program.epub.GetItem(epubItemPath);
                        if (i != null)
                        {
                            //To-do:注入样式，js
                            if (i.href.EndsWith("html"))
                            {
                                string content = (i.GetData() as TextEpubItemFile).text;
                                content = HtmlHack.Hack(content);
                                return ResourceHandler.FromString(content);
                            }
                            if (i.href.EndsWith("css"))
                            {
                                //html里的其实也应该处理……
                                string content = (i.GetData() as TextEpubItemFile).text;
                                content = CssHack.Hack(content);
                                return ResourceHandler.FromString(content, null, true, i.mediaType);
                            }
                            if (uri.Query.Contains("warm"))
                            {
                                if (!i.mediaType.Contains("image")) throw new Exception("Should be image.");

                                var d = ImageHack.Warmer(i.GetData().GetBytes());
                                return ResourceHandler.FromByteArray(d, "image/bmp");


                            }
                            return ResourceHandler.FromByteArray(i.GetData().GetBytes(), i.mediaType);
                        }
                        else
                        {
                            Log.log("[Error]Cannot get "+uri);
                        }

                    }
                    break;
                case "viewer":
                    {
                        var filename = uri.AbsolutePath.Substring(1).Replace("/", ".");
                        Stream fs = assembly.GetManifestResourceStream("AeroEpubViewer.Res." + filename);
                        if (filename.EndsWith("css"))
                        {
                            string content = new StreamReader(fs).ReadToEnd();
                            content = CssHack.Hack(content);
                            return ResourceHandler.FromString(content, null, true, "text/css");
                        }
                        string mime = Util.GetMimeType(filename);
                        if (mime != null)
                            return ResourceHandler.FromStream(fs, mime);
                        return ResourceHandler.FromStream(fs);
                    } //break;
                case "app":
                    {
                        string[] args = uri.AbsolutePath.Substring(1).Split('/');
                        switch (args[0])
                        {
                            case "pos":
                                ResizeManage.SetPara(args);
                                return ResourceHandler.FromString("OK");
                            case "bookfontsize":
                                UserSettings.bookFontSize = int.Parse(args[1]);
                                EpubViewer.chromium.LoadingStateChanged += EpubViewer.SendDataWhenLoad;
                                EpubViewer.chromium.Reload(true);
                                return ResourceHandler.FromString("OK");
                            case "screentest":
                                CssHack.SetScreenTest(args);
                                return ResourceHandler.FromString("OK");
                            case "inspector":
                                EpubViewer.chromium.ShowDevTools();
                                return ResourceHandler.FromString("OK");
                            case "theme":
                               UserSettings.theme= args[1];
                                return ResourceHandler.FromString("OK");
                            case "ImageQuickView":
                                return ResourceHandler.FromString(ImageQuickView.GetHTML());
                        }


                    }
                    break;


            }
            return null;
        }
    }
}
