var sp_frame_c = document.getElementById('sp_frame_general_container');
var sp_frame = document.getElementById('sp_frame_general');
var sp_page_scroll = 100;
document.CloseSpFrame = function () {
    sp_page_scroll = sp_frame.contentDocument.scrollingElement.scrollTop;
    sp_frame_c.style.display = 'none';
    document.getElementById('sp_frame_search_container').style.display = 'none';
}

function ImageQuickView() {
    document.MenuClose();
    sp_frame_c.style.display = "block";
    if (sp_frame.src != "aeroepub://app/ImageQuickView") {
        sp_frame.src = "aeroepub://app/ImageQuickView";
        sp_page_scroll = 0;
    }
    sp_frame.contentDocument.scrollingElement.scrollTop = sp_page_scroll;
}
function BookInfo() {
    document.MenuClose();

    sp_frame_c.style.display = "block";
    sp_frame.src = "aeroepub://app/BookInfo"
}
var search_frame_c = document.getElementById('sp_frame_search_container');
var search_frame = document.getElementById('sp_frame_search');
var search_page_init = false;
var search_page_scroll = 0;
function SearchService() {
    document.MenuClose();

    search_frame_c.style.display = "block";
    if (!search_page_init) {
        search_frame.onload = function () { search_frame.contentWindow.document.getElementById("search_input").focus(); }
        search_frame.src = "aeroepub://viewer/search-service.html";
    } else {
        search_frame.contentWindow.document.getElementById("search_input").focus();
    }
    search_page_init = true;

}