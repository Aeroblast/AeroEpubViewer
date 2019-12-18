window.onmousewheel = function(e)
{
    this.parent.document.Scroll(Math.sign(e.wheelDelta)*40);
}
document.addEventListener("keydown", this.parent.document.keydown);
document.addEventListener("mouseup", this.parent.document.MenuSwitch);
var fontSizeStyle = document.createElement("style");
fontSizeStyle.innerHTML = "body{font-size:" + this.parent.document.userSettings.bookFontSize + "px}";
document.head.appendChild(fontSizeStyle);

function Href(e)
{

    this.parent.document.Link(e.href);

}