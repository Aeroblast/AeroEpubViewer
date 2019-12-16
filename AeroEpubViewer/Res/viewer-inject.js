window.onmousewheel = function(e)
{
    this.parent.document.Scroll(Math.sign(e.wheelDelta)*40);
}
document.addEventListener("keydown", this.parent.document.keydown);