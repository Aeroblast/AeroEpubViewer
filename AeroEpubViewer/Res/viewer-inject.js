var PD = this.parent.document;
window.onmousewheel = function (e) {
    PD.Scroll(Math.sign(e.wheelDelta) * 40);
}
document.addEventListener("keydown", PD.keydown);
document.addEventListener('contextmenu', event => event.preventDefault());
document.body.onclick = PD.FrameMouseUp;
var fontSizeStyle = document.createElement("style");
fontSizeStyle.innerHTML = "body{font-size:" + PD.userSettings.bookFontSize + "px}";
document.head.appendChild(fontSizeStyle);
var style = document.createElement("style");
style.innerHTML = PD.direction.injectStyle;
document.head.appendChild(style);
var themeStyle = document.createElement("style");
style.innerHTML = PD.theme.frameStyle;
document.head.appendChild(style);
document.addEventListener("touchstart", function (e) { PD.OnFrameTouchStart(e.touches[0].screenX, e.touches[0].screenY); });
document.addEventListener("touchend", function (e) { PD.OnFrameTouchEnd(); });
document.addEventListener("touchmove", function (e) { PD.OnFrameTouchMove(e.touches[0].screenX, e.touches[0].screenY); });

function Href(e) {
    if (e.getAttribute("epub:type") == "noteref") {
        let noteid = e.href.split('#')[1];
        let note = document.getElementById(noteid);
        PD.PopupFootnote(note.innerHTML, e.offsetLeft, e.offsetTop, this.frameElement);
        return false;
    }
    PD.Link(e.href);

}