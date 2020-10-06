var direction_rtl = new Object();
direction_rtl.GetParallelLength = function (e) { return e.offsetWidth; }
direction_rtl.SetParallelLength = function (e, px) { e.style.width = px + "px"; }
direction_rtl.GetElementPos = function (e, f) { return f.contentWindow.document.documentElement.scrollWidth - e.getBoundingClientRect().right; }
direction_rtl.GetWindowLength = function () { return window.innerWidth; }
direction_rtl.AdjustFrameSize = function (f) { f.width = f.contentWindow.document.documentElement.scrollWidth + "px"; }
direction_rtl.SetPosStyle = function (e) { e.style.right = e.pos + "px"; }
direction_rtl.GetEventParallel = function (e) { return e.target.offsetWidth - e.x; }
direction_rtl.GetEventParallelPercent = function (e) { return (e.target.offsetWidth - e.x) / e.target.offsetWidth; }
direction_rtl.SetParallelPositivePos = function (e, px) { e.style.right = px + "px"; }
direction_rtl.GetEffective = function (x, y) { return x; };
direction_rtl.GetDelta = function (d1, d2) { return d1 - d2; };
direction_rtl.KeyToDirection = function (k) {
    switch (k) {
        case "ArrowLeft": return -1;
        case "ArrowUp": return 1;
        case "ArrowRight": return 1;
        case "ArrowDown": return -1;
    }
    return 0;
}
direction_rtl.style = "\
iframe{height:90vh;top:2vh;}\
#scrollBar_container{width: 100vw;height:7vh; left: 0; bottom: 0; margin: 0;}\
#scrollBar{\
width: 100vw; left: 0; bottom: 0; margin: 0;margin-top:2vh;\
transform-origin: 50vw; transform: rotate(180deg); \
}\
#scrollBar::-webkit-slider-runnable-track{height:3vh;}\
#scrollBar::-webkit-slider-thumb{width: 15px;height: 3vh;}\
.scrollBarChapterDisplay{height:3vh;margin-top:2vh;}\
.scrollBarChapterName{margin-top:-1.5em;}\
#measure_container{position:fixed;width:100%;height:1vh;left:0;bottom:6.5vh;}\
#measure_container>div{position:fixed;height:1vh;width:1vh;bottom:6vh;border-radius:0.3vh;}\
.scrollBarChapterName{box-shadow: -0.1em -0.1em 0.3em -0.1em #808080, 0.05em -0.1em 0.3em -0.1em #808080;width: auto;display: inline-block;padding-left:0 0.2em 0 0.2em;}\
";
direction_rtl.injectStyle = "body{height:100vh;min-width:66vh;}";
var direction_default = new Object();
direction_default.GetParallelLength = function (e) { return e.offsetHeight; }
direction_default.SetParallelLength = function (e, px) { e.style.height = px + "px"; }
direction_default.GetElementPos = function (e, f) { return e.getBoundingClientRect().top; }
direction_default.GetWindowLength = function () { return window.innerHeight; }
direction_default.AdjustFrameSize = function (f) { f.height = f.contentWindow.document.documentElement.scrollHeight + "px"; }
direction_default.SetPosStyle = function (e) { e.style.top = e.pos + "px"; }
direction_default.GetEventParallel = function (e) { return e.y; }
direction_default.GetEventParallelPercent = function (e) { return (e.y) / window.innerHeight; }//limited usage
direction_default.SetParallelPositivePos = function (e, px) { e.style.top = px + "px"; }
direction_default.GetEffective = function (x, y) { return y; };
direction_default.GetDelta = function (d1, d2) { return d2 - d1; };
direction_default.KeyToDirection = function (k) {
    switch (k) {
        case "ArrowUp": return 1;
        case "ArrowDown": return -1;
    }
    return 0;
}
direction_default.style = "\
iframe{width:91vw;left:4.5vw;min-height:90vh;} \
#scrollBar_container{width: 3vw;height:100vh; right: 0; bottom: 0; margin: 0;}\
#scrollBar{\
margin:0 0 0 3vw;\
width:100vh;height:3vw;\
transform-origin:top left;\
transform: rotate(90deg);\
}\
#scrollBar::-webkit-slider-runnable-track{width:3vh;}\
#scrollBar::-webkit-slider-thumb{width: 3vw;height: 3vh;}\
.scrollBarChapterDisplay{width:3vh;}\
.scrollBarChapterName{margin-left:-10em;width:10em;white-space:normal}\
#measure_container{position:fixed;width:1vw;height:100%;right:3vw;}\
#measure_container>div{position:fixed;height:1vh;width:1vw;right:2.5vw;border-radius:1vw;}\
.scrollBarChapterName{box-shadow: -0.1em 0.1em 0.3em -0.1em #808080;}\
";
direction_default.injectStyle = "body{width:100vw;padding-bottom:3vh;}";


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

