var stringTable = [
    ["Inspector", "检查", "検査"],//0
    ["View illustration", "速览插图", "イラスト チラ見"],//1
    ["Search", "搜索", "検索"],//2
    ["Book Info", "书籍信息", "書籍情報"],//3
    ["Copy Image", "复制图片", "画像をコピー"],//4
    ["Inspect Element", "检查元素", "検査"],//5
];

var stringLoadList = [
    ["string_inspector", 0],
    ["string_viewillu", 1],
    ["string_search", 2],
    ["string_bookinfo", 3]
];

var stringNameList = {
    CopyImage: 4,
    InspectElement: 5
};

var currentLanguage = 0;
switch (navigator.language) {
    case "ja": currentLanguage = 2; break;
    case "zh-CN": currentLanguage = 1; break;
}
LoadString();

function LoadString() {
    stringLoadList.forEach(function (x) {
        document.getElementById(x[0]).innerHTML = stringTable[x[1]][currentLanguage];
    });
}

function GetStringByName(name) {
    return stringTable[stringNameList[name]][currentLanguage];
}