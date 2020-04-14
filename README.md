# AeroEpubViewer
An Epub Viewer on Windows. Using C#, CEFSharp.

## Features
+ ```page-progress-direction``` value ```rtl``` (vertical text).
+ EPUB2 ncx table of content / EPUB3 nav toc.
+ Scroll Mode(arrow key, touch drag, mouse wheel).
+ Popup-footnote.
+ Font size control.
+ Detect locale from book.
+ Theme(Default, Warm, Dark). Image will be processed to fit background when select Warm.
+ Draggable scrollbar for whole book.
+ BookInfo
+ Have a quick view of all images.
+ Search
## 功能
+ 支持```page-progress-direction```值```rtl``` (直排)
+ 支持EPUB2/EPUB3目录(点左上角打开目录)
+ 浏览方式为无缝滚动浏览（方向键，触屏拖拽，鼠标滚轮）
+ 弹出式脚注
+ 字号调整
+ 按照书调整语言设置（主要是字体）
+ 主题（默认，暖色，黑色）。选择暖色主题时，图片会被处理以符合背景。
+ 可拖拽的全书进度条
+ 书籍信息
+ 速览插图
+ 搜索

## 细节
### 搜索
快捷键：`Ctrl+F` 打开搜索页面，可以直接输入关键词；`Enter`搜索；出现搜索结果后，`↓↑`选择结果，也可以重新聚焦到搜索框，`Enter`跳转结果。
已经访问过的搜索结果会被标记。
### 复制文本
剪贴板中将存在两种格式：HTML和文本。
对于带注音（即ruby、rt标签）的内容，需要纯文本的时候（例如粘贴至记事本）将不带注音，需要富文本时（例如粘贴至Word文档）含有注音。
只复制注音时将在纯文本中保留。
## 开发计划

见[to-do.md](https://github.com/Aeroblast/AeroEpubViewer/blob/master/to-do.md)
