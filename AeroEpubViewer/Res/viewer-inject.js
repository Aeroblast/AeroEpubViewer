window.onmousewheel = function(e)
{
    this.parent.document.Scroll(Math.sign(e.wheelDelta)*40);
}
document.addEventListener("keydown", this.parent.document.keydown);
document.body.onclick=this.parent.document.FrameMouseUp;
var fontSizeStyle = document.createElement("style");
fontSizeStyle.innerHTML = "body{font-size:" + this.parent.document.userSettings.bookFontSize + "px}";
document.head.appendChild(fontSizeStyle);
var style = document.createElement("style");
style.innerHTML = this.parent.document.direction.injectStyle;
document.head.appendChild(style);
function Href(e)
{
    if (e.getAttribute("epub:type") == "noteref")
    {
        let noteid = e.href.split('#')[1];
        let note = document.getElementById(noteid);
        this.parent.document.PopupFootnote(note.innerHTML, e.offsetLeft, e.offsetTop, this.frameElement);
        return false;
    }
    this.parent.document.Link(e.href);

}