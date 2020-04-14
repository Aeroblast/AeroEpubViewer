var PD = this.parent.document;
window.onmousewheel = function (e) {
    PD.Wheel(e);
}
document.addEventListener("keydown", function (e) { PD.keydown(e); });
document.addEventListener('contextmenu', event => event.preventDefault());

var selectionFlag = false;
document.body.onmouseup = function (e) {
    if (e.button == 0)//left click
    {
        let tryclose = PD.TryCloseFootnote() || PD.TryCloseContextMenu();
        if (tryclose) return;
        let sel = window.getSelection();
        if (sel.type == 'Range') {
            selectionFlag = true;
            return;
        }
        if (selectionFlag) {
            selectionFlag = false;
            return;
        }
        PD.MenuSwitch();
    } else
        if (e.button == 2)//right
        {
            console.log(e.target.tagName)
            if (e.target.tagName == "IMG")
            {
                PD.ContextMenu(e.target, e.pageX, e.pageY, window.frameElement);
            }
        }

};
var fontSizeStyle = document.createElement("style");
fontSizeStyle.innerHTML = "body{font-size:" + PD.userSettings.bookFontSize + "px}";
document.head.appendChild(fontSizeStyle);
var style = document.createElement("style");
style.innerHTML = PD.direction.injectStyle;
document.head.appendChild(style);
var themeStyle = document.createElement("style");
themeStyle.innerHTML = PD.theme.frameStyle;
document.head.appendChild(themeStyle);
document.addEventListener("touchstart", function (e) { PD.OnFrameTouchStart(e.touches[0].screenX, e.touches[0].screenY); });
document.addEventListener("touchend", function (e) { PD.OnFrameTouchEnd(); });
document.addEventListener("touchmove", function (e) { PD.OnFrameTouchMove(e.touches[0].screenX, e.touches[0].screenY); });
[].forEach.call(document.getElementsByTagName("img"), function (e) { e.src = e.src + "?" + PD.theme.name; });
[].forEach.call(document.getElementsByTagName("image"), function (e) { e.setAttribute("xlink:href", e.getAttribute("xlink:href") + "?" + PD.theme.name); });
function Href(e) {
    if (e.getAttribute("epub:type") == "noteref") {
        let noteid = e.href.split('#')[1];
        let note = document.getElementById(noteid);
        PD.PopupFootnote(note.innerHTML, e.offsetLeft, e.offsetTop, this.frameElement);
        return false;
    }
    PD.Link(e.href);
}
document.body.oncopy = function (e) {
    e.preventDefault();
    let sel = window.getSelection();
    if (sel.type == "Range") {
        let str_html = "";
        let str_txt = "";
        let flag_ruby = false;
        if (sel.anchorNode.nodeType == Node.ELEMENT_NODE) {

        }
        else if (sel.anchorNode.nodeType == Node.TEXT_NODE) {
            str_html += "<" + sel.anchorNode.parentNode.nodeName + ">";
            let t = sel.anchorNode.nodeValue.substr(sel.anchorOffset);
            str_html += t;
            str_txt += t;
        }
        let last = sel.anchorNode;
        do {
            let next;
            if (last.nodeType == Node.ELEMENT_NODE) {
                next = last.firstChild;
            } else {
                next = last.nextSibling;
                while (next == null) {
                    last = last.parentNode;
                    next = last.nextSibling;
                    str_html += "</" + last.nodeName + ">";
                    switch (last.nodeName) {
                        case 'P': case 'DIV':
                            str_txt += '\n'; break;
                        case 'RT':
                            flag_ruby = false;
                            break;
                    }
                }
            }
            last = next;
            if (last == sel.focusNode) { break; }
            if (last.nodeType == Node.ELEMENT_NODE) {
                switch (last.nodeName) {
                    case 'RT':
                        flag_ruby = true;
                        break;
                }
                str_html += "<" + last.nodeName + ">";
            }
            else if (last.nodeType == Node.TEXT_NODE) {
                let t = last.nodeValue;
                str_html += t;
                if (!flag_ruby) str_txt += Trim(t);
            }
        } while (true);
        if (sel.focusNode.nodeType == Node.ELEMENT_NODE) {

            str_html += sel.focusNode.outerHTML;
        }
        else if (sel.focusNode.nodeType == Node.TEXT_NODE) {
            let t = sel.focusNode.nodeValue.substr(0, sel.focusOffset);
            str_html += t;
            if (!flag_ruby) str_txt += Trim(t);
            str_html += "</" + sel.focusNode.parentNode.nodeName + ">";
        }
        e.clipboardData.setData("text/html", str_html);
        e.clipboardData.setData("text/plain", str_txt);
    }

}
function Trim(t) {
    let start = 0, end = t.length - 1;
    while ((t[start] == ' ' || t[start] == '\n' || t[start] == '\t' || t[start] == '\r') && start < t.length) start++;
    if (end >= start)
        while (t[end] == ' ' || t[end] == '\n' || t[end] == '\t' || t[end] == '\r') end--;
    else return "";
    return t.substr(start, end - start + 1);
}