document.PopupFootnote = PopupFootnote2;

var popupFootnote = null;
document.PopupFootnote = function (content, left, top, frame) {
    if (popupFootnote != null) popupFootnote.parentNode.removeChild(popupFootnote);
    popupFootnote = document.createElement("div");
    popupFootnote.className = "popupFootnote";
    console.log(frame);
    document.body.appendChild(popupFootnote);
    left = frame.offsetLeft + left
    top = frame.offsetTop + top + 20;

    popupFootnote.innerHTML = content;
    if (left + popupFootnote.offsetWidth > window.innerWidth) {
        left = window.innerWidth - popupFootnote.offsetWidth;
    }
    console.log(top);
    if (top + popupFootnote.offsetHeight > window.innerHeight) {

        top = window.innerHeight - popupFootnote.offsetHeight;

    }
    popupFootnote.style.left = left + "px";
    popupFootnote.style.top = top + "px";
}
var PopupFootnote2 = function (content, left, top, frame) {
    if (popupFootnote != null) popupFootnote.parentNode.removeChild(popupFootnote);
    popupFootnote = document.createElement("iframe");
    popupFootnote.className = "popupFootnote";
    popupFootnote.src = frame.src + "?footnote";
    document.body.appendChild(popupFootnote);
    left = frame.offsetLeft + left
    top = frame.offsetTop + top + 20;

    popupFootnote.onload = function () {
        let styles = '';
        [].forEach.call(frame.contentWindow.document.head.querySelectorAll('style'), function (e) { styles += e.outerHTML; });
        popupFootnote.contentWindow.document.documentElement.innerHTML = "<head>" + styles + "</head><body style='margin:3%;width:94%'>" + content + "</body>";
        if (left + popupFootnote.offsetWidth > window.innerWidth) {
            left = window.innerWidth - popupFootnote.offsetWidth;
        }
        if (top + popupFootnote.offsetHeight > window.innerHeight) {
            top = window.innerHeight - popupFootnote.offsetHeight;
        }
        popupFootnote.style.left = left + "px";
        popupFootnote.style.top = top + "px";
        popupFootnote.contentWindow.document.addEventListener('contextmenu', event => event.preventDefault());
    }
    menuOn = false;
    menu.style.animation = "";

}
document.TryCloseFootnote = function () {
    if (popupFootnote != null) {
        popupFootnote.parentNode.removeChild(popupFootnote);
        popupFootnote = null;
        return true;
    }
    return false;
}