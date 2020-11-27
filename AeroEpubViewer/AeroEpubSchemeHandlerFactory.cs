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
using AeroEpub;
namespace AeroEpubViewer
{
    public class AeroEpubSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "aeroepub";
        Assembly assembly = Assembly.GetExecutingAssembly();

        string imagePageSizeAttribute;
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            Log.log(request.Url);
            var route = uri.AbsolutePath.Split('/')[1];
            switch (route)
            {
                case "book":
                    {
                        if (uri.Query.Contains("footnote"))
                        {
                            return ResourceHandler.FromString("");
                        }
                        var epubItemPath = uri.AbsolutePath.Substring("/book/".Length);
                        if (epubItemPath[0] == '/') epubItemPath = epubItemPath.Substring(1);
                        Item i = Program.epub.GetItem(epubItemPath);
                        if (i != null)
                        {
                            if (i.mediaType == "application/xhtml+xml")
                            {
                                string content = (i.GetFile() as TextEpubFileEntry).text;
                                content = HtmlHack.Hack(content);
                                return ResourceHandler.FromString(content);
                            }
                            if (i.mediaType == "text/css")
                            {
                                //html里的其实也应该处理……
                                string content = (i.GetFile() as TextEpubFileEntry).text;
                                content = CssHack.Hack(content);
                                return ResourceHandler.FromString(content, null, true, i.mediaType);
                            }
                            if (i.mediaType.StartsWith("image"))
                            {
                                //Image warm color process
                                if (uri.Query.Contains("warm"))
                                {
                                    switch (i.mediaType)
                                    {
                                        case "image/heic":
                                        case "image/webp":
                                            byte[] imageData = i.GetFile().GetBytes();
                                            var decoded = ImageHack.TryDecode(imageData);
                                            if (decoded != null)
                                            {
                                                return ResourceHandler.FromByteArray(ImageHack.Warmer(decoded), "image/bmp");
                                            }
                                            else//local decoder not found 
                                            {
                                                return ResourceHandler.FromByteArray(imageData, i.mediaType);
                                            }
                                        default:
                                            return ResourceHandler.FromByteArray(ImageHack.Warmer(i.GetFile().GetBytes()), "image/bmp");
                                    }
                                }
                                //do not return image but  a html page 
                                if (uri.Query.Contains("page"))
                                {
                                    if (imagePageSizeAttribute == null)
                                    {
                                        if (Program.epub.spine.pageProgressionDirection == "rtl")
                                        { imagePageSizeAttribute = "height=\"100%\""; }
                                        else
                                        { imagePageSizeAttribute = "width=\"100%\""; }
                                    }
                                    return ResourceHandler.FromString($"<html><head><link href=\"aeroepub://viewer/viewer-inject.css\" rel=\"stylesheet\" type=\"text/css\"/></head><body><img {imagePageSizeAttribute} src={("aeroepub://book" + uri.AbsolutePath)}><script src=\"aeroepub://viewer/viewer-inject.js\"></script></body></html>");
                                }
                                //normally return image data. Decode use system decoder for some format
                                switch (i.mediaType)
                                {
                                    case "image/heic":
                                        byte[] imageData = i.GetFile().GetBytes();
                                        var decoded = ImageHack.TryDecode(imageData);
                                        if (decoded != null)
                                        {
                                            return ResourceHandler.FromByteArray(decoded, "image/bmp");
                                        }
                                        else//local decoder not found 
                                        {
                                            return ResourceHandler.FromByteArray(imageData, i.mediaType);
                                        }
                                    default:
                                        return ResourceHandler.FromByteArray(i.GetFile().GetBytes(), i.mediaType);

                                }

                            }
                            return ResourceHandler.FromByteArray(i.GetFile().GetBytes(), i.mediaType);

                        }
                        else
                        {
                            Log.log("[Error]Cannot get " + uri);
                        }

                    }
                    break;
                case "viewer":
                    {
                        var filename = uri.AbsolutePath.Substring("/viewer/".Length).Replace("/", ".");
                        Stream fs = assembly.GetManifestResourceStream("AeroEpubViewer.Res." + filename);
                        string mime = Util.GetMimeType(filename);
                        if (mime != null)
                            return ResourceHandler.FromStream(fs, mime);
                        return ResourceHandler.FromStream(fs);
                    } //break;
                case "app":
                    {
                        string[] args = uri.AbsolutePath.Substring("/app/".Length).Split('/');
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
                                UserSettings.theme = args[1];
                                return ResourceHandler.FromString("OK");
                            case "ImageQuickView":
                                return ResourceHandler.FromString(SpecialPageService.ImageQuickView());
                            case "BookInfo":
                                return ResourceHandler.FromString(SpecialPageService.BookInfo());
                            case "StartSearch":
                                {
                                    var t = uri.AbsolutePath.Substring("/app/".Length);
                                    int i = t.IndexOf('/');
                                    var word = Uri.UnescapeDataString(t.Substring(i + 1));
                                    SearchService.Start(word);
                                    Log.log(word);
                                    return ResourceHandler.FromString("OK");
                                }
                            case "CheckSearchResult":
                                return ResourceHandler.FromString(SearchService.GetResult(int.Parse(args[1])));
                            case "CopyImage":
                                {
                                    string path = uri.AbsolutePath.Substring("/app/CopyImage/".Length);
                                    var f = Program.epub.GetFile(path);
                                    using (var stm = new MemoryStream(f.GetBytes()))
                                    using (var img = System.Drawing.Image.FromStream(stm))
                                        System.Windows.Forms.Clipboard.SetImage(img);
                                }
                                return ResourceHandler.FromString("OK");
                            case "UserBookCss":
                                return ResourceHandler.FromString(UserSettings.userBookCssContent, null, true, "text/css");
                            case "UserBookCssRtl":
                                return ResourceHandler.FromString(UserSettings.userBookCssContent_rtl, null, true, "text/css");
                            case "UserBookCssLtr":
                                return ResourceHandler.FromString(UserSettings.userBookCssContent_ltr, null, true, "text/css");
                            case "External":
                                System.Diagnostics.Process.Start("explorer.exe", Uri.UnescapeDataString(args[1]));
                                return ResourceHandler.FromString("OK");
                        }


                    }
                    break;


            }
            return null;
        }
    }
}
