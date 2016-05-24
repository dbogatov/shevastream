var Cart = function () {
	var currentCallback = null;

	var executeCallback = function () {
		$(document).trigger("eshop.cartupdated");
		if (currentCallback != null) {
			currentCallback();
			currentCallback = null;
		}
	};

	return {
		setCallback: function (callback) {
			currentCallback = callback;
			return this;
		},

		addItem: function (productId, quantity) {
			var data = {
				ProductId: productId,
				Quantity: quantity
			};

			$.put("/api/Cart", data).always(function () {
				executeCallback();
			});
		},

		getItems: function () {
			$.get("/api/Cart", {}, function (response) {
				executeCallback();
			});
		},

		changeItem: function (productId, quantity) {
			var data = {
				ProductId: productId,
				Quantity: quantity
			};

			$.post("/api/Cart", data, function (response) {
				executeCallback();
			});
		},

		removeItem: function (productId) {
			var data = {
				ProductId: productId,
			};

			$.delete("/api/Cart", data, function (response) {
				executeCallback();
			});
		},
	};
} ();

$(document).ready(function () {
	updateCartNumber();
});

$(document).on("eshop.cartupdated", function () {
	updateCartNumber();
});

function updateCartNumber() {
	var count = $.cookie("Cart") == null ? 0 : JSON.parse($.cookie("Cart")).Elements.length;

	if (count > 0) {
		$("#cartItemsNumber").text("(" + count + ")");
	}
}