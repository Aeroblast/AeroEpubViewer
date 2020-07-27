using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Xml.Resolvers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroEpubViewer
{
    class Xhtml
    {
        public static XmlDocument Load(string xhtml)
        {
            xhtml = xhtml.Replace("<!DOCTYPE html>", "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            XmlDocument d = new XmlDocument();
            using (var rdr = new XmlTextReader(new StringReader(xhtml)))
            {
                rdr.DtdProcessing = DtdProcessing.Parse;
                rdr.XmlResolver = new XhtmlEntityResolver();
                d.Load(rdr);
            }
            return d;
        }
    }
    class XhtmlEntityResolver : XmlResolver
    {
        static Assembly ass = Assembly.GetExecutingAssembly();
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            Stream fs = ass.GetManifestResourceStream("AeroEpubViewer.Res.xhtml.dtd");
            return fs;
        }
    }
}
