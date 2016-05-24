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
		Cart.addItem($(this).data("product"), 1);
	});

	$(".addToCartAndCheckoutBtn").click(function () {
		Cart.setCallback(function () {
			location.href = "/Store/Cart";
		}).addItem($(this).data("product"), 1);
	});
});

$(document).ready(function () {
	updateCartButtons();
});

$(document).on("eshop.cartupdated", function () {
	updateCartButtons();
});

function updateCartButtons() {
	var thisId = parseInt($(".addToCartBtn").data("product"));

	var isAdded = $.cookie("Cart") == null ? false : (JSON.parse($.cookie("Cart")).Elements.filter(function (element) {
		return element.ProductId == thisId;
	}).length > 0);

	if (isAdded) {
		$(".addToCartBtn").text("Already in the cart");
		$(".addToCartBtn").addClass("disabled");
		$(".addToCartBtn").off( "click", "**" );

		$(".addToCartAndCheckoutBtn").text("Already in the cart");
		$(".addToCartAndCheckoutBtn").addClass("disabled");
		$(".addToCartAndCheckoutBtn").off( "click", "**" );
	}
}