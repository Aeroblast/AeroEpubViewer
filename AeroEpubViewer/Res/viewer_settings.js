
var theme_default = new Object();
theme_default.name = "default";
theme_default.viewerStyle = "#nav,#menu{background:white;color:black;}.scrollBarChapterName {background:white;}";
theme_default.frameStyle = "body{background:white;color:black;}";
var warmColor = "";
var theme_warm = new Object();
function gen_theme_warm(_warmColor) {
    warmColor = _warmColor;
    theme_warm.name = "warm";
    theme_warm.viewerStyle = "body,#nav,#menu{background:{warm};color:black;}.scrollBarChapterName {background:{warm};}".replace(/{warm}/g, warmColor);
    theme_warm.frameStyle = "body{background:{warm};color:black;}".replace(/{warm}/g, warmColor);
}

var theme_dark = new Object();
theme_dark.name = "dark";
theme_dark.viewerStyle = "body,#nav,#menu{background:black;color:white;}.scrollBarChapterName{background:black;}";
theme_dark.frameStyle = "body{background:black;color:white;}";

document.theme = theme_default;
var themeStyle = document.createElement("style");
document.head.appendChild(themeStyle);
var directionStyle = document.createElement('style');
document.head.appendChild(directionStyle);
var spreadStyle = document.createElement('style');
document.head.appendChild(spreadStyle);

function LoadUserSettings(json) {
    document.userSettings = json;
    document.getElementById("fontSizeInput").value = json.bookFontSize;
    document.getElementById("themeSelect").value = json.viewerTheme;
    SetTheme();
}