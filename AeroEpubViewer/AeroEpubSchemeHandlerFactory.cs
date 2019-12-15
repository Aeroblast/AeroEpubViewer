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
        
        static AeroEpubSchemeHandlerFactory()
        {

        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            Log.log(request.Url);
            switch (uri.Host) 
            {
                case "book": 
                    {
                        var epubItemPath = uri.AbsolutePath;
                        if (epubItemPath[0] == '/') epubItemPath = epubItemPath.Substring(1);
                        ManifestItem i = Program.epub.GetItem(epubItemPath);
                        if (i != null)
                        {
                            //To-do:注入样式，js
                            if (i.href.EndsWith("html")) 
                            {
                                string cssInject = "<link href=\"aeroepub://viewer/rtl/viewer-inject.css\" rel=\"stylesheet\" type=\"text/css\"/>";
                                string jsInject = "<script src=\"aeroepub://viewer/rtl/viewer-inject.js\"></script>";
                                string content = (i.GetData() as TextEpubItemFile).text;
                                content = content.Replace("</head>", cssInject + "\n</head>").Replace("</body>",jsInject+"\n</body>");
                                return ResourceHandler.FromString(content);


                            }
                            return ResourceHandler.FromByteArray(i.GetData().GetBytes(), i.mediaType);
                        }

                    }
                    break;
                case "viewer": 
                    {
                        var filename = uri.AbsolutePath.Substring(1).Replace("/",".") ;
                        Stream fs = assembly.GetManifestResourceStream("AeroEpubViewer.Res."+filename);
                        string mime=Util.GetMimeType(filename);
                        if(mime!=null)
                            return ResourceHandler.FromStream(fs,mime);
                        return ResourceHandler.FromStream(fs);
                    } //break;
            
            
            }
            return null;
        }
    }
}
