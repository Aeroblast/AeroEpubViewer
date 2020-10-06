var nav = document.getElementById("nav");
document.navOn = false;
function NavSwitch() {
    if (document.navOn) {
        document.navOn = false;
        nav.style.animation = "navHide 0.5s ease 0s 1 ";

    } else {
        document.navOn = true;
        nav.style.animation = "navDisplay 0.5s ease 0s 1";

    }
    nav.style.animationFillMode = "forwards";

}
document.menuOn = false;
var menu = document.getElementById("menu");

document.MenuSwitch = function () {
    if (document.menuOn || document.navOn) {
        document.MenuClose();
    } else {
        document.MenuOpen();
    }

};
document.MenuClose = function () {
    if (document.menuOn) {
        document.menuOn = false;
        menu.style.animation = "menuHide 0.5s ease 0s 1 ";
        menu.style.animationFillMode = "forwards";
    }
    if (document.navOn) { NavSwitch(); }
};
document.MenuOpen = function () {
    document.menuOn = true;
    menu.style.animation = "menuDisplay 0.5s ease 0s 1";
    menu.style.animationFillMode = "forwards";
};


//Right-click
var contextMenu = document.getElementById("context_menu");
var contextMenuOn = false;
var tempTargetElement = null;
document.ContextMenu = function (e, left, top, frame) {
    tempTargetElement = e;
    contextMenuOn = true;
    left = frame.offsetLeft + left;
    top = frame.offsetTop + top;
    contextMenu.style.left = left + "px";
    contextMenu.style.top = top + "px";
    contextMenu.style.display = "block";
    let inner = "<div onclick=\"InspectElement()\">" + GetStringByName("InspectElement")+"</div>";
    if (e.tagName == "IMG") {
        inner += "<div onclick=\"CopyImage('" + e.src + "')\">" + GetStringByName("CopyImage")+"</div>";
    }
    if (e.tagName.toUpperCase() == "IMAGE") {
        inner += "<div onclick=\"CopyImage('" + ReferPath(frame.src, e.getAttribute("xlink:href")) + "')\">" + GetStringByName("CopyImage")+"</div>";
    }
    contextMenu.innerHTML = inner;
}
document.TryCloseContextMenu = function () {
    if (contextMenuOn) {
        contextMenu.style.display = "none";
        contextMenuOn = false;
        return true;
    }
    return false;
}