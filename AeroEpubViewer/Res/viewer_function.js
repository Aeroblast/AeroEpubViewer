function CopyImage(url) {
    AppCall("aeroepub://app/CopyImage/" + url.substring("aeroepub://book/".length));
    document.TryCloseContextMenu();
}
function InspectElement() {
    console.log(tempTargetElement);
    Inspector();
    document.TryCloseContextMenu();
}

function SetTheme() {
    let name = document.getElementById("themeSelect").value;
    gen_theme_warm(document.userSettings.warmColor);
    switch (name) { case "default": document.theme = theme_default; break; case "warm": document.theme = theme_warm; break; case "dark": document.theme = theme_dark; break; }
    for (let i = 0; i < frameList.length; i++) { frameList[i].contentWindow.location.reload(); }
    themeStyle.innerHTML = document.theme.viewerStyle;
    AppCall("aeroepub://app/theme/" + name);
}

function ApplyFontSize(a) {
    document.userSettings.bookFontSize = parseInt(a);
    AppCall("aeroepub://app/pos" + GetBookPos());
    AppCall("aeroepub://app/bookfontsize/" + document.userSettings.bookFontSize);
}
function Inspector() {
    AppCall("aeroepub://app/inspector");
}
function ScreenTest() {
    let e = document.getElementById("screenTest");
    AppCall("aeroepub://app/screentest/" + e.offsetWidth + "/" + e.offsetHeight);
}