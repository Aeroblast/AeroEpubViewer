using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;
namespace AeroEpubViewer.Epub
{
    public class EpubFile
    {
        public string filename;
        public string path;
        public List<EpubItemFile> items;
        TextEpubItemFile _OPF = null;
        public TextEpubItemFile OPF
        {
            get
            {
                if (_OPF == null)
                {
                    TextEpubItemFile i = GetFile<TextEpubItemFile>("META-INF/container.xml");
                    if (i == null) { throw new EpubErrorException("Cannot find META-INF/container.xml"); }
                    Regex reg = new Regex("<rootfile .*?>");
                    XTag tag = XTag.FindTag("rootfile", i.text);
                    string opf_path = tag.GetAttribute("full-path");
                    _OPF = GetFile<TextEpubItemFile>(opf_path);
                    if (_OPF == null) { throw new EpubErrorException("Cannot find opf file"); }
                }
                return _OPF;
            }
        }

        string _title = null;
        public string title { get { if (_title == null) ReadMeta(); return _title; } }
        string _creator = null;
        public string creator { get { if (_title == null) ReadMeta(); return _creator; } }
        string _language=null;
        public string language { get { if (_language == null) ReadMeta(); return _language; } }
        public void ReadMeta()
        {
            XFragment f = XFragment.FindFragment("metadata", OPF.text);
            _creator = "";
            _title = "";
            _language = "";

            foreach (var e in f.root.childs)
            {
                switch (e.tag.tagname)
                {
                    case "dc:title": _title = e.innerXHTML; break;
                    case "dc:creator": _creator += e.innerXHTML + ","; break;
                    case "dc:language":_language = e.innerXHTML.ToLower();break;
                }
            }
            if (_creator.EndsWith(",")) _creator = _creator.Substring(0, _creator.Length - 1);
        }
        Spine _spine;
        Dictionary<string,ManifestItem> _manifest;
        public Spine spine 
        {
            get {
                if (_spine == null) ReadSpine();
                return _spine;
            
            }
        }
        public Dictionary<string, ManifestItem> manifest
        { 
            get 
            {
                if (_manifest == null) ReadSpine();
                return _manifest;
            } 
        
        }


        void ReadSpine()
        {

            XFragment f = XFragment.FindFragment("manifest", OPF.text);
            _manifest = new Dictionary<string, ManifestItem>();
            foreach (var e in f.root.childs)
            {
                if (e.tag.tagname != "item") continue;
                var i = new ManifestItem(e,this);
                _manifest.Add(i.id,i);            
            }
            foreach (var a in _manifest) 
            {
                if (a.Value.href[0] != '/') 
                {
                    a.Value.href =Path.GetDirectoryName(OPF.fullName) +"/"+ a.Value.href;
                }
            }

            f = XFragment.FindFragment("spine", OPF.text);
            _spine = new Spine(f,_manifest);



        }

        public void DeleteEmpty()//只查一层……谁家epub也不会套几个文件夹
        {
            List<EpubItemFile> tobedelete = new List<EpubItemFile>();
            foreach (var item in items)
            {
                if (item.fullName.EndsWith("/"))
                {
                    bool refered = false;
                    foreach (var item2 in items)
                    {
                        if (item2.fullName != item.fullName && item2.fullName.StartsWith(item.fullName))
                        {
                            refered = true;
                            break;
                        }
                    }
                    if (!refered) tobedelete.Add(item);
                }

            }
            foreach (var a in tobedelete) items.Remove(a);
        }

        public EpubItemFile GetFile(string fullName)
        {
            foreach (var i in items) if (i.fullName == fullName) return i;
            throw new EpubErrorException("Cannot find file by filename:"+fullName);
        }
        public T GetFile<T>(string fullName) where T : EpubItemFile
        {
            EpubItemFile r = null;
            foreach (var i in items) if (i.fullName == fullName) r = i;
            if (r == null || r.GetType() != typeof(T)) return null;
            return (T)r;
        }
        public ManifestItem GetItem(string href)
        {

            foreach (var i in manifest) if (i.Value.href == href) return i.Value;
            return null;
        }
        public void Save(string path, FileMode fileMode = FileMode.Create)
        {
            string filepath = path;
            if (!path.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
            {
                filepath = Path.Combine(filepath, filename + ".epub");
            }
            using (FileStream zipToOpen = new FileStream(filepath, fileMode))
            {

                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (var item in items) { item.PutInto(archive); }
                }
            }
            Log.log("[Info]Saved " + filepath);
        }
        public EpubFile(string path)
        {
            this.path = path;
            filename = Path.GetFileNameWithoutExtension(path);
            items = new List<EpubItemFile>();
            using (FileStream zipToOpen = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string ext = Path.GetExtension(entry.Name).ToLower();
                        if (entry.FullName == "mimetype")
                        {
                            using (var stm = entry.Open())
                            using (StreamReader r = new StreamReader(stm))
                            {
                                string s = Util.Trim(r.ReadToEnd());
                                if (s != "application/epub+zip") throw new EpubErrorException("The mimetype of epub should be 'application/epub+zip'. Current:"+s);
                                var i = new MIMETypeItem();
                                items.Insert(0, i);
                            }

                        }
                        else
                            switch (ext)
                            {
                                case ".xhtml":
                                case ".html":
                                case ".xml":
                                case ".css":
                                case ".opf":
                                case ".ncx":
                                case ".svg":
                                case ".js":

                                    using (var stm = entry.Open())
                                    using (StreamReader r = new StreamReader(stm))
                                    {
                                        string s = r.ReadToEnd();
                                        var i = new TextEpubItemFile(entry.FullName, s);
                                        items.Add(i);
                                    }
                                    break;
                                default:
                                    using (var stm = entry.Open())

                                    {
                                        byte[] d = new byte[entry.Length];
                                        if (entry.Length < int.MaxValue)
                                        {
                                            stm.Read(d, 0, (int)entry.Length);
                                            var i = new EpubItemFile(entry.FullName, d);
                                            items.Add(i);
                                        }
                                        else { throw new EpubErrorException("File size exceeds the limit."); }
                                    }
                                    break;
                            }


                    }
                }
            }
            if (items.Count == 0) throw new EpubErrorException("Cannot find files in epub");
            if (items[0].GetType() != typeof(MIMETypeItem)) throw new EpubErrorException("Cannot find mimetype in epub");
        }
    }
    public class ManifestItem
    {
        public string href,id,mediaType;
        EpubFile belongTo;
        public ManifestItem(XELement e,EpubFile belongTo) 
        {
            this.belongTo = belongTo;
            XTag tag = e.tag;
            href=tag.GetAttribute("href");
            id = tag.GetAttribute("id");
            mediaType = tag.GetAttribute("media-type");
        }
        public EpubItemFile GetData() 
        {
            return belongTo.GetFile(Uri.UnescapeDataString(href));
        }
    }
    public class Spine : IEnumerable
    {
        List<SpineItem> items=new List<SpineItem>();
        public ManifestItem toc;
        public string pageProgressionDirection;
        public Spine(XFragment spine, Dictionary<string, ManifestItem> items) 
        {
            string toc = spine.root.tag.GetAttribute("toc");
            if (toc != "") 
            {
               this.toc= items[toc];
            }
            pageProgressionDirection = spine.root.tag.GetAttribute("page-progression-direction");
            foreach (var e in spine.root.childs) 
            {
                if (e.tag.tagname != "itemref") continue;
                this.items.Add(new SpineItem(e,items));
            }
        }
        public int Count { get { return items.Count; } }
        public SpineItem this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }
    public class SpineItem
    {
        public ManifestItem item;
        public string properties;
        public bool linear = true;
        public SpineItem(XELement itemref, Dictionary<string, ManifestItem> items)
        {
            this.item = items[itemref.tag.GetAttribute("idref")];
            properties = itemref.tag.GetAttribute("properties");
            if (itemref.tag.GetAttribute("linear") == "no") linear = false;
        }
        public string href{ get { return item.href; } }
        public override string ToString()
        {
            return href;
        }
    }


    public class TextEpubItemFile : EpubItemFile
    {
         public string text;

        public TextEpubItemFile(string fullName, string data)
        {
            this.fullName = fullName;
            this.text = data;

        }
        public override void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry(fullName);
            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write(text);
            }

        }
        public override byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(text);
        }
    }
    public class EpubItemFile
    {
        public string fullName;
        byte[] data;
        public EpubItemFile() { }
        public EpubItemFile(string fullName, byte[] data)
        {
            this.fullName = fullName;
            this.data = data;

        }
        public virtual void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry(fullName);
            using (Stream stream = entry.Open())
            {
                stream.Write(data, 0, data.Length);
            }

        }
        public virtual byte[] GetBytes()
        {
            return data;
        }
    }
    public class MIMETypeItem : EpubItemFile
    {
        public MIMETypeItem() { fullName = "mimetype"; }
        public override void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);//没啥意义，还是Deflate，不是Store

            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write("application/epub+zip");
            }

        }
        public override byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes("mimetype");
        }
    }

    public class EpubErrorException : System.Exception {
        public EpubErrorException(string s) : base(s) { }
    }

}