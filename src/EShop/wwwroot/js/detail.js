$(document).ready(function () {

	$(".gallery img").each(function () {
		$(this).attr("data-mfp-src", $(this).attr("src"));
	});

	$(".gallery img").magnificPopup({
		gallery: {
			enabled: true
		},
		type: "image"
	});

	$(".addToCartBtn").click(function () {
		Cart.setCallback(function () {
			alert("Added");
		}).addItem($(this).data("product"), 1);
	});

	$(".addToCartAndCheckoutBtn").click(function () {
		Cart.setCallback(function () {
			location.href = "/Store/Cart";
		}).addItem($(this).data("product"), 1);
	});
});