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
        public string version;
        string idref;
        public MetaRecord uniqueIdentifier;


        public class MetaRecord
        {
            public string name;
            public string value;
            public string id;
            public List<MetaRecord> refines = new List<MetaRecord>();
            public MetaRecord(XELement e)
            {
                name = e.tag.tagname;
                value = e.innerXHTML;
                id = e.tag.GetAttribute("id");
            }
            public MetaRecord() { }
            public void AddIfExist(XELement e, string property_name)
            {
                string t = e.tag.GetAttribute(property_name);
                if (t != "")
                {
                    int pre = property_name.IndexOf(':');
                    if (pre > 0) { property_name = property_name.Substring(pre + 1); }
                    var a = new MetaRecord();
                    a.name = property_name;
                    a.value = t;
                    refines.Add(a);
                }
            }
            public MetaRecord GetRefines(string name)
            {
                foreach (var a in refines) { if (name == a.name) return a; }
                return null;
            }
        }
        public string title
        {
            get { if (dc_titles == null) ReadMeta(); return dc_titles[0].value; }
        }
        public string language
        {
            get
            {
                if (dc_language == null) ReadMeta();
                if (dc_language.Count > 0)
                    return dc_language[0].value;
                else
                    return xml_lang;
            }
        }

        public string xml_lang;
        public List<MetaRecord> dc_titles;
        public List<MetaRecord> dc_creators;
        public List<MetaRecord> dc_language;
        public List<MetaRecord> dc_identifier;
        public List<MetaRecord> others;
        public List<MetaRecord> meta;
        public string cover_img = "";
        public ManifestItem toc;

        public void ReadMeta()
        {
            var packge_tag = XTag.FindTag("package", OPF.text);
            idref = packge_tag.GetAttribute("unique-identifier");
            version = packge_tag.GetAttribute("version");
            xml_lang = packge_tag.GetAttribute("xml:lang");//bookwalker
            dc_titles = new List<MetaRecord>();
            dc_creators = new List<MetaRecord>();
            dc_language = new List<MetaRecord>();
            dc_identifier = new List<MetaRecord>();
            others = new List<MetaRecord>();
            meta = new List<MetaRecord>();
            switch (version)
            {
                case "3.0": ReadMeta3(); break;
                default: ReadMeta2(); break;
            }

        }

        void ReadMeta2()
        {
            XFragment f = XFragment.FindFragment("metadata", OPF.text);

            foreach (var e in f.root.childs)
            {
                switch (e.tag.tagname)
                {
                    case "dc:title":
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "opf:file-as");
                            dc_titles.Add(t);
                        }
                        break;
                    case "dc:creator":
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "opf:file-as");
                            t.AddIfExist(e, "opf:role");
                            dc_creators.Add(t);
                        }
                        break;
                    case "dc:language":
                        {
                            var t = new MetaRecord(e);
                            dc_language.Add(t);
                        }
                        break;
                    case "dc:identifier":
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "opf:scheme");
                            dc_identifier.Add(t);
                        }
                        break;
                    case "dc:contributor":
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "opf:file-as");
                            t.AddIfExist(e, "opf:role");
                            others.Add(t);
                        }
                        break;
                    case "dc:date":
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "opf:event");
                            others.Add(t);
                        }
                        break;
                    case "meta":
                        {
                            var t = new MetaRecord();
                            t.name = e.tag.GetAttribute("name");
                            t.value = e.tag.GetAttribute("content");
                            meta.Add(t);
                        }
                        break;
                    default:
                        {
                            var t = new MetaRecord(e);
                            others.Add(t);
                        }
                        break;
                }
            }
            foreach (var a in meta)
            {
                if (a.name == "cover")
                {
                    string id = a.value;
                    if (manifest.ContainsKey(id))
                    {
                        cover_img = manifest[id].href;
                    }
                    break;
                }
            }
            toc = spine.toc;
        }
        void ReadMeta3()
        {
            XFragment f = XFragment.FindFragment("metadata", OPF.text);
            List<MetaRecord> primary = new List<MetaRecord>();
            foreach (var e in f.root.childs)
            {
                switch (e.tag.tagname)
                {

                    case "dc:language":
                    case "dc:identifier":
                        {
                            var t = new MetaRecord(e);
                            primary.Add(t);
                        }
                        break;
                    case "meta":
                        {
                            string name = e.tag.GetAttribute("name");
                            if (name != "")
                            {
                                var t = new MetaRecord();
                                t.name = name;
                                t.value = e.tag.GetAttribute("content");
                                meta.Add(t);
                                continue;
                            }
                            string refines = e.tag.GetAttribute("refines");
                            if (refines != "")
                            {
                                if (refines.StartsWith("#") && refines.Length > 1)
                                {
                                    string id = refines.Substring(1);
                                    var t = new MetaRecord(e);
                                    t.name = e.tag.GetAttribute("property");
                                    t.AddIfExist(e, "scheme");
                                    foreach (var r in primary)
                                    { //要是refine在primary前面我可不管……
                                        if (r.id == id)
                                        {
                                            r.refines.Add(t);
                                            break;
                                        }
                                    }
                                    continue;
                                }
                            }
                            string property = e.tag.GetAttribute("property");
                            if (property != "")
                            {
                                var t = new MetaRecord(e);
                                t.name = property;
                                meta.Add(t);
                                continue;
                            }
                        }
                        break;
                    default:
                        {
                            var t = new MetaRecord(e);
                            t.AddIfExist(e, "xml:lang");
                            t.AddIfExist(e, "dir");
                            primary.Add(t);
                        }
                        break;
                }
            }
            foreach (var a in primary)
            {
                switch (a.name)
                {
                    case "dc:title": dc_titles.Add(a); break;
                    case "dc:creator": dc_creators.Add(a); break;
                    case "dc:identifier": dc_identifier.Add(a); break;
                    case "dc:language": dc_language.Add(a); break;
                    default: others.Add(a); break;
                }
            }
            foreach (var a in dc_identifier)
            {
                if (idref == a.id) { uniqueIdentifier = a; break; }
            }
            foreach (var a in manifest)
            {
                switch (a.Value.properties)
                {
                    case "nav": toc = a.Value; break;
                    case "cover-image": cover_img = a.Value.href; break;
                }
            }
            if (toc == null) toc = spine.toc;
            //check
            //if (dc_titles.Count == 0 || dc_identifier.Count == 0 || dc_language.Count == 0) { throw new EpubErrorException("Lack of some metadata."); }
        }
        Spine _spine;
        Dictionary<string, ManifestItem> _manifest;
        public Spine spine
        {
            get
            {
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
                var i = new ManifestItem(e, this);
                _manifest.Add(i.id, i);
            }
            foreach (var a in _manifest)
            {
                if (a.Value.href[0] != '/')
                {
                    a.Value.href = Path.GetDirectoryName(OPF.fullName) + "/" + a.Value.href;
                }
            }

            f = XFragment.FindFragment("spine", OPF.text);
            _spine = new Spine(f, _manifest);
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
            throw new EpubErrorException("Cannot find file by filename:" + fullName);
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
                                if (s != "application/epub+zip") throw new EpubErrorException("The mimetype of epub should be 'application/epub+zip'. Current:" + s);
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
        //http://idpf.org/epub/30/spec/epub30-publications.html#sec-item-elem
        public string href, id, mediaType;
        public string properties;
        //public string fallback, mediaOverlay;
        EpubFile belongTo;
        public ManifestItem(XELement e, EpubFile belongTo)
        {
            this.belongTo = belongTo;
            XTag tag = e.tag;
            href = tag.GetAttribute("href");
            id = tag.GetAttribute("id");
            mediaType = tag.GetAttribute("media-type");
            properties = tag.GetAttribute("properties");
        }
        public EpubItemFile GetData()
        {
            return belongTo.GetFile(Uri.UnescapeDataString(href));
        }
    }
    public class Spine : IEnumerable
    {
        //http://idpf.org/epub/30/spec/epub30-publications.html#sec-spine-elem
        List<SpineItemref> items = new List<SpineItemref>();
        public ManifestItem toc;//For EPUB2
        public string pageProgressionDirection;
        public string id;
        public Spine(XFragment spine, Dictionary<string, ManifestItem> items)
        {
            string toc = spine.root.tag.GetAttribute("toc");
            string id = spine.root.tag.GetAttribute("id");
            if (toc != "")
            {
                this.toc = items[toc];
            }
            pageProgressionDirection = spine.root.tag.GetAttribute("page-progression-direction");
            foreach (var e in spine.root.childs)
            {
                if (e.tag.tagname != "itemref") continue;
                this.items.Add(new SpineItemref(e, items));
            }
        }
        public int Count { get { return items.Count; } }
        public SpineItemref this[int index]
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
    public class SpineItemref
    {
        //http://idpf.org/epub/30/spec/epub30-publications.html#sec-itemref-elem
        public ManifestItem item;
        public string properties;
        public string id;
        public bool linear = true;
        public SpineItemref(XELement itemref, Dictionary<string, ManifestItem> items)
        {
            this.item = items[itemref.tag.GetAttribute("idref")];
            properties = itemref.tag.GetAttribute("properties");
            id = itemref.tag.GetAttribute("id");
            if (itemref.tag.GetAttribute("linear") == "no") linear = false;
        }
        public string href { get { return item.href; } }
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

    public class EpubErrorException : System.Exception
    {
        public EpubErrorException(string s) : base(s) { }
    }

}