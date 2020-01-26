using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroEpubViewer.Epub;

namespace AeroEpubViewer
{
    public class SpecialPageService
    {
        public static string ImageQuickView()
        {

            StringBuilder r = new StringBuilder();
            r.Append("<html><head><style>img{max-height:95vh;max-width:90vw}</style></head><body>");
            foreach (var i in Program.epub.manifest)
            {
                if (i.Value.mediaType.Contains("image"))
                {

                    r.Append("<div><img src=\"aeroepub://book/");
                    r.Append(i.Value.href);
                    r.Append("\"/></div>");
                }
            }

            r.Append("<script src=\"aeroepub://viewer/sp-page.js\"></script>");
            r.Append("</body>");

            return r.ToString();
        }

        //http://www.loc.gov/marc/relators/
        static Dictionary<string, string> opf_role = new Dictionary<string, string>() {
{"abr","Abridger"},
{"acp","Art copyist"},
{"act","Actor"},
{"adi","Art director"},
{"adp","Adapter"},
{"aft","Author of afterword, colophon, etc."},
{"anl","Analyst"},
{"anm","Animator"},
{"ann","Annotator"},
{"ant","Bibliographic antecedent"},
{"ape","Appellee"},
{"apl","Appellant"},
{"app","Applicant"},
{"aqt","Author in quotations or text abstracts"},
{"arc","Architect"},
{"ard","Artistic director"},
{"arr","Arranger"},
{"art","Artist"},
{"asg","Assignee"},
{"asn","Associated name"},
{"ato","Autographer"},
{"att","Attributed name"},
{"auc","Auctioneer"},
{"aud","Author of dialog"},
{"aui","Author of introduction, etc."},
{"aus","Screenwriter"},
{"aut","Author"},
{"bdd","Binding designer"},
{"bjd","Bookjacket designer"},
{"bkd","Book designer"},
{"bkp","Book producer"},
{"blw","Blurb writer"},
{"bnd","Binder"},
{"bpd","Bookplate designer"},
{"brd","Broadcaster"},
{"brl","Braille embosser"},
{"bsl","Bookseller"},
{"cas","Caster"},
{"ccp","Conceptor"},
{"chr","Choreographer"},
{"-clb","Collaborator"},
{"cli","Client"},
{"cll","Calligrapher"},
{"clr","Colorist"},
{"clt","Collotyper"},
{"cmm","Commentator"},
{"cmp","Composer"},
{"cmt","Compositor"},
{"cnd","Conductor"},
{"cng","Cinematographer"},
{"cns","Censor"},
{"coe","Contestant-appellee"},
{"col","Collector"},
{"com","Compiler"},
{"con","Conservator"},
{"cor","Collection registrar"},
{"cos","Contestant"},
{"cot","Contestant-appellant"},
{"cou","Court governed"},
{"cov","Cover designer"},
{"cpc","Copyright claimant"},
{"cpe","Complainant-appellee"},
{"cph","Copyright holder"},
{"cpl","Complainant"},
{"cpt","Complainant-appellant"},
{"cre","Creator"},
{"crp","Correspondent"},
{"crr","Corrector"},
{"crt","Court reporter"},
{"csl","Consultant"},
{"csp","Consultant to a project"},
{"cst","Costume designer"},
{"ctb","Contributor"},
{"cte","Contestee-appellee"},
{"ctg","Cartographer"},
{"ctr","Contractor"},
{"cts","Contestee"},
{"ctt","Contestee-appellant"},
{"cur","Curator"},
{"cwt","Commentator for written text"},
{"dbp","Distribution place"},
{"dfd","Defendant"},
{"dfe","Defendant-appellee"},
{"dft","Defendant-appellant"},
{"dgg","Degree granting institution"},
{"dgs","Degree supervisor"},
{"dis","Dissertant"},
{"dln","Delineator"},
{"dnc","Dancer"},
{"dnr","Donor"},
{"dpc","Depicted"},
{"dpt","Depositor"},
{"drm","Draftsman"},
{"drt","Director"},
{"dsr","Designer"},
{"dst","Distributor"},
{"dtc","Data contributor"},
{"dte","Dedicatee"},
{"dtm","Data manager"},
{"dto","Dedicator"},
{"dub","Dubious author"},
{"edc","Editor of compilation"},
{"edm","Editor of moving image work"},
{"edt","Editor"},
{"egr","Engraver"},
{"elg","Electrician"},
{"elt","Electrotyper"},
{"eng","Engineer"},
{"enj","Enacting jurisdiction"},
{"etr","Etcher"},
{"evp","Event place"},
{"exp","Expert"},
{"fac","Facsimilist"},
{"fds","Film distributor"},
{"fld","Field director"},
{"flm","Film editor"},
{"fmd","Film director"},
{"fmk","Filmmaker"},
{"fmo","Former owner"},
{"fmp","Film producer"},
{"fnd","Funder"},
{"fpy","First party"},
{"frg","Forger"},
{"gis","Geographic information specialist"},
{"-grt","Graphic technician"},
{"his","Host institution"},
{"hnr","Honoree"},
{"hst","Host"},
{"ill","Illustrator"},
{"ilu","Illuminator"},
{"ins","Inscriber"},
{"inv","Inventor"},
{"isb","Issuing body"},
{"itr","Instrumentalist"},
{"ive","Interviewee"},
{"ivr","Interviewer"},
{"jud","Judge"},
{"jug","Jurisdiction governed"},
{"lbr","Laboratory"},
{"lbt","Librettist"},
{"ldr","Laboratory director"},
{"led","Lead"},
{"lee","Libelee-appellee"},
{"lel","Libelee"},
{"len","Lender"},
{"let","Libelee-appellant"},
{"lgd","Lighting designer"},
{"lie","Libelant-appellee"},
{"lil","Libelant"},
{"lit","Libelant-appellant"},
{"lsa","Landscape architect"},
{"lse","Licensee"},
{"lso","Licensor"},
{"ltg","Lithographer"},
{"lyr","Lyricist"},
{"mcp","Music copyist"},
{"mdc","Metadata contact"},
{"med","Medium"},
{"mfp","Manufacture place"},
{"mfr","Manufacturer"},
{"mod","Moderator"},
{"mon","Monitor"},
{"mrb","Marbler"},
{"mrk","Markup editor"},
{"msd","Musical director"},
{"mte","Metal-engraver"},
{"mtk","Minute taker"},
{"mus","Musician"},
{"nrt","Narrator"},
{"opn","Opponent"},
{"org","Originator"},
{"orm","Organizer"},
{"osp","Onscreen presenter"},
{"oth","Other"},
{"own","Owner"},
{"pan","Panelist"},
{"pat","Patron"},
{"pbd","Publishing director"},
{"pbl","Publisher"},
{"pdr","Project director"},
{"pfr","Proofreader"},
{"pht","Photographer"},
{"plt","Platemaker"},
{"pma","Permitting agency"},
{"pmn","Production manager"},
{"pop","Printer of plates"},
{"ppm","Papermaker"},
{"ppt","Puppeteer"},
{"pra","Praeses"},
{"prc","Process contact"},
{"prd","Production personnel"},
{"pre","Presenter"},
{"prf","Performer"},
{"prg","Programmer"},
{"prm","Printmaker"},
{"prn","Production company"},
{"pro","Producer"},
{"prp","Production place"},
{"prs","Production designer"},
{"prt","Printer"},
{"prv","Provider"},
{"pta","Patent applicant"},
{"pte","Plaintiff-appellee"},
{"ptf","Plaintiff"},
{"pth","Patent holder"},
{"ptt","Plaintiff-appellant"},
{"pup","Publication place"},
{"rbr","Rubricator"},
{"rcd","Recordist"},
{"rce","Recording engineer"},
{"rcp","Addressee"},
{"rdd","Radio director"},
{"red","Redaktor"},
{"ren","Renderer"},
{"res","Researcher"},
{"rev","Reviewer"},
{"rpc","Radio producer"},
{"rps","Repository"},
{"rpt","Reporter"},
{"rpy","Responsible party"},
{"rse","Respondent-appellee"},
{"rsg","Restager"},
{"rsp","Respondent"},
{"rsr","Restorationist"},
{"rst","Respondent-appellant"},
{"rth","Research team head"},
{"rtm","Research team member"},
{"sad","Scientific advisor"},
{"sce","Scenarist"},
{"scl","Sculptor"},
{"scr","Scribe"},
{"sds","Sound designer"},
{"sec","Secretary"},
{"sgd","Stage director"},
{"sgn","Signer"},
{"sht","Supporting host"},
{"sll","Seller"},
{"sng","Singer"},
{"spk","Speaker"},
{"spn","Sponsor"},
{"spy","Second party"},
{"srv","Surveyor"},
{"std","Set designer"},
{"stg","Setting"},
{"stl","Storyteller"},
{"stm","Stage manager"},
{"stn","Standards body"},
{"str","Stereotyper"},
{"tcd","Technical director"},
{"tch","Teacher"},
{"ths","Thesis advisor"},
{"tld","Television director"},
{"tlp","Television producer"},
{"trc","Transcriber"},
{"trl","Translator"},
{"tyd","Type designer"},
{"tyg","Typographer"},
{"uvp","University place"},
{"vac","Voice actor"},
{"vdg","Videographer"},
{"-voc","Vocalist"},
{"wac","Writer of added commentary"},
{"wal","Writer of added lyrics"},
{"wam","Writer of accompanying material"},
{"wat","Writer of added text"},
{"wdc","Woodcutter"},
{"wde","Wood engraver"},
{"win","Writer of introduction"},
{"wit","Witness"},
{"wpr","Writer of preface"},
{"wst","Writer of supplementary textual content"}};
        static Dictionary<string, string> langcode = new Dictionary<string, string>() {
            {"zh-cn","中文-大陆" },
           {"ja","日本語" },
            {"zh","中文" },
            { "zh-tw","中文-台灣"},
            { "zh-Hans","中文-简体"},
            {"zh-Hant","中文-繁體"}
        };
        public static string BookInfo()
        {
            XFragment x = XFragment.FindFragment("metadata", Program.epub.OPF.text);
            StringBuilder metatemp = new StringBuilder();
            string creators = "";
            string contributers = "";
            string cover_href = "";
            foreach (var e in x.root.childs)
            {
                switch (e.tag.tagname)
                {
                    case "dc:identifier":
                        {
                            string a = e.tag.GetAttribute("opf:scheme");
                            if (a != "") { a = "" + a + " : "; }
                            metatemp.Append("<tr><td>Book Identifier</td><td><data-item>" + a + e.innerXHTML + "</data-item></td></tr>");
                        }
                        break;
                    case "dc:language":
                        string lang = Util.Trim(e.innerXHTML);
                        if (langcode.ContainsKey(lang)) lang = langcode[lang];
                        metatemp.Append("<tr><td>Language</td><td><data-item>" + lang + "</data-item></td></tr>");
                        break;
                    case "dc:title": break;
                    case "dc:creator":
                        {
                            string a = e.tag.GetAttribute("opf:role");
                            if (a != "")
                            {
                                if (opf_role.ContainsKey(a))
                                    a = opf_role[a];
                                a = " (" + a + ")";
                            }
                            creators += e.innerXHTML + a + ", ";
                        }
                        break;
                    case "dc:contributor":
                        {
                            string a = e.tag.GetAttribute("opf:role");
                            if (a != "")
                            {
                                if (opf_role.ContainsKey(a))
                                    a = opf_role[a];
                                a = " (" + a + ")";
                            }
                            contributers += e.innerXHTML + a + ", ";
                        }
                        break;
                    case "dc:date":
                        {
                            string a = e.tag.GetAttribute("opf:event");
                            if (a != "")
                            {
                                a = "Date of " + a;
                            }
                            else { a = "Date"; }
                            metatemp.Append("<tr><td>" + a + "</td><td><data-item>" + e.innerXHTML + "</data-item></td></tr>");
                        }
                        break;
                    case "meta":
                        string meta_name = e.tag.GetAttribute("name");
                        string meta_content = e.tag.GetAttribute("content");
                        if (meta_name == "cover" && Program.epub.manifest.ContainsKey(meta_content))
                        {
                            cover_href = Program.epub.manifest[meta_content].href;
                            break;
                        }
                        metatemp.Append("<tr><td>" + meta_name + "</td><td><data-item>" + meta_content + "</data-item></td></tr>");
                        break;
                    default:
                        metatemp.Append("<tr><td>" + e.tag.tagname + "</td><td><data-item>" + e.innerXHTML + "</data-item></td></tr>");
                        break;
                }

            }
            StringBuilder r = new StringBuilder();
            r.Append("<html><head><style>img{max-height:95vh;max-width:90vw}data-item{font-weight:bold;}</style></head><body>");
            r.Append("<table border=\"0\">");
            r.Append("<tr><td>Title</td><td><data-item>" + Program.epub.title + "</data-item></td></tr>");
            if (creators != "") r.Append("<tr><td>Creator(s)</td><td><data-item>" + creators.Substring(0, creators.Length - 2) + "</data-item></td></tr>");
            r.Append("</table>");
            if (cover_href != "") r.Append("<img src=\"aeroepub://book/" + cover_href + "\"/>");
            r.Append("<table border=\"0\">");
            if (contributers != "") r.Append("<tr><td>Contributer(s)</td><td><data-item>" + contributers.Substring(0, contributers.Length - 2) + "</data-item></td></tr>");
            r.Append(metatemp);
            r.Append("</table>");
            r.Append("<script src=\"aeroepub://viewer/sp-page.js\"></script>");
            r.Append("</body>");

            return r.ToString();
        }
    }
}
