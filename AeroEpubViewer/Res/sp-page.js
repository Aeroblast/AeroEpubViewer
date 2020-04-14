var PD = this.parent.document;
document.addEventListener('contextmenu', event => event.preventDefault());
var themeStyle = document.createElement("style");
themeStyle.innerHTML = PD.theme.frameStyle;
document.head.appendChild(themeStyle);
document.body.onmouseup = function (e) {
    if (e.button == 0)//left click
    {
    } else
        if (e.button == 2)//right
        {
            console.log(e.target.tagName)
            if (e.target.tagName == "IMG") {
                ContextMenu(e.target, e.pageX, e.pageY, window.frameElement);
            }
        }

};

var contextMenu = document.getElementById("context_menu");
var contextMenuOn = false;
var ContextMenu = function (e, left, top, frame) {
    contextMenuOn = true;
    left = frame.offsetLeft + left;
    top = frame.offsetTop + top;
    contextMenu.style.left = left + "px";
    contextMenu.style.top = top + "px";
    contextMenu.style.display = "block";
    if (e.tagName == "IMG") {
        contextMenu.innerHTML = "<div onclick=\"CopyImage('" + e.src + "')\">复制图片</div>"
    }
}
var TryCloseContextMenu = function () {
    if (contextMenuOn) {
        contextMenu.style.display = "none";
        contextMenuOn = false;
        return true;
    }
    return false;
}
function CopyImage(url) {
    AppCall("aeroepub://app/CopyImage/" + url.substring("aeroepub://book/".length));
    TryCloseContextMenu();
}
function AppCall(url) {
    let xhttp = null
    xhttp = new XMLHttpRequest();
    xhttp.open("GET", url, true);
    xhttp.send();
}