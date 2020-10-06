function AppCall(url) {
    let xhttp = null
    xhttp = new XMLHttpRequest();
    xhttp.open("GET", url, true);
    xhttp.send();
}
function ReferPath(file, href) {
    file = file.substr(0, file.lastIndexOf('/'));
    while (href.startsWith("..")) {
        href = href.substr(3);
        file = file.substr(0, file.lastIndexOf('/'));
    }
    let s = "";
    if (href[0] != '/' && file[file.length] != '/') s = "/";
    return file + s + href;
}
function Trim(t) {
    let start = 0, end = t.length - 1;
    while ((t[start] == ' ' || t[start] == '\n' || t[start] == '\t' || t[start] == '\r') && start < t.length) start++;
    if (end >= start)
        while (t[end] == ' ' || t[end] == '\n' || t[end] == '\t' || t[end] == '\r') end--;
    else return "";
    return t.substr(start, end - start + 1);
}