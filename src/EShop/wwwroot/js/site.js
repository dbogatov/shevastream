// Write your Javascript code.

$(document).ready(function() {
	$("#" + globalActive).addClass("active");
	
	$("#callMeBackBtn").click(function() {
		alert("Poka eshche ne sdelal");
	});
});

$(document).ready(fixFooter);
$(window).resize(fixFooter);

function fixFooter() {
	if ($("body").height() < $(window.top).height()) {
		$("#footerFix").height($(window.top).height() - $("body").height());
	}	
}