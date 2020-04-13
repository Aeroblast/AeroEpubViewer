var PD = this.parent.document;
window.onmousewheel = function (e) {
    PD.Wheel(e);
}
document.addEventListener("keydown", function (e) { PD.keydown(e);});
document.addEventListener('contextmenu', event => event.preventDefault());
document.body.onclick = PD.FrameMouseUp;
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