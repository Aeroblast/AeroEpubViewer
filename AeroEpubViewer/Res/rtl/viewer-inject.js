window.onmousewheel = function(e)
{
    this.parent.document.Scroll(Math.sign(e.wheelDelta)*30);
}