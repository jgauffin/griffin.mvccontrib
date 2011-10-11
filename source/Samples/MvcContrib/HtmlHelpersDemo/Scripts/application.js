$(function () {
    watermark();
    tooltip();
});

function watermark() {
    $('[title*="Watermark:"]').each(function () {
        $(this).watermark($(this).attr('title').substr(10));
        $(this).removeAttr('title');
    });
}

function tooltip() {
    var xOffset = 10;
    var yOffset = 10;
    var $tooltipDiv = $("body").append("<div id='tooltip'>" + this.t + "</div>");

    $("[title]").hover(function (e) {
        $tooltipDiv.html($(this).attr('title'));
        $tooltipDiv
			.css("top", (e.pageY - xOffset) + "px")
			.css("left", (e.pageX + yOffset) + "px")
			.fadeIn("fast");
    },
	function () {
	    $tooltipDiv.hide();
	});
    $("[title]").mousemove(function (e) {
        $tooltipDiv
			.css("top", (e.pageY - xOffset) + "px")
			.css("left", (e.pageX + yOffset) + "px");
    });
};
