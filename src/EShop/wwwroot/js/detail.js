$(document).ready(function () {

	$(".gallery img").each(function() {
		$(this).attr("data-mfp-src", $(this).attr("src"));
	});

	$(".gallery img").magnificPopup({
		gallery: {
			enabled: true
		},
		type: "image"
	});

	//$("#fbcomment").data("href", window.location.href);
	//var p = $("#fbcomment").data("href");
});