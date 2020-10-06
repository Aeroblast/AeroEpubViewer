document.FrameOnCopy = function (e) {
    e.preventDefault();
    let sel = e.target.ownerDocument.defaultView.getSelection();

    if (sel.type == "Range") {
        let str_html = "";
        let str_txt = "";
        let flag_ruby = false;
        if (sel.anchorNode == sel.focusNode) {
            str_html = str_txt = sel.anchorNode.nodeValue.substr(sel.anchorOffset, sel.focusOffset - sel.anchorOffset);
        }
        else {
            let range = sel.getRangeAt(0);
            if (range.startContainer.nodeType == Node.ELEMENT_NODE) {

            }
            else if (range.startContainer.nodeType == Node.TEXT_NODE) {
                str_html += "<" + range.startContainer.parentNode.nodeName + ">";
                let t = range.startContainer.nodeValue.substr(range.startOffset);
                str_html += t;
                str_txt += t;
            }
            let last = range.startContainer;
            if (last != range.endContainer)
                do {
                    let next;
                    if (last.nodeType == Node.ELEMENT_NODE && last.childNodes.length != 0) {
                        next = last.firstChild;
                    } else {
                        next = last.nextSibling;
                        while (next == null) {
                            last = last.parentNode;
                            next = last.nextSibling;
                            str_html += "</" + last.nodeName + ">";
                            switch (last.nodeName) {
                                case 'P': case 'DIV':
                                    str_txt += '\n'; break;
                                case 'RT':
                                    flag_ruby = false;
                                    break;
                            }
                        }
                    }
                    last = next;
                    if (last == range.endContainer) { break; }
                    if (last.nodeType == Node.ELEMENT_NODE) {
                        switch (last.nodeName) {
                            case 'RT':
                                flag_ruby = true;
                                break;
                        }
                        if (last.childNodes.length == 0) {
                            str_html += "<" + last.nodeName + "/>";

                        }
                        else { str_html += "<" + last.nodeName + ">"; }

                    }
                    else if (last.nodeType == Node.TEXT_NODE) {
                        let t = last.nodeValue;
                        str_html += t;
                        if (!flag_ruby) str_txt += Trim(t);
                    }
                } while (true);
            if (range.endContainer.nodeType == Node.ELEMENT_NODE) {

                str_html += range.endContainer.outerHTML;
            }
            else if (range.endContainer.nodeType == Node.TEXT_NODE) {
                let t = range.endContainer.nodeValue.substr(0, range.endOffset);
                str_html += t;
                if (!flag_ruby) str_txt += Trim(t);
                str_html += "</" + range.endContainer.parentNode.nodeName + ">";
            }
        }

        e.clipboardData.setData("text/html", str_html);
        e.clipboardData.setData("text/plain", str_txt);

        let message = str_txt;
        if (message.length > 20) message = message.substr(0, 20) + "бн";
        message = message.replace('\n', '');
        console.log("Copy:" + message);
    }
}