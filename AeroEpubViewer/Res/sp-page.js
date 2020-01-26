var PD = this.parent.document;
document.addEventListener('contextmenu', event => event.preventDefault());
var themeStyle = document.createElement("style");
themeStyle.innerHTML = PD.theme.frameStyle;
document.head.appendChild(themeStyle);