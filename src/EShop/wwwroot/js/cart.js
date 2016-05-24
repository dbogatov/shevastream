var Cart = function () {
	var currentCallback = null;
	
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
				if (currentCallback != null) {
					currentCallback();
					currentCallback = null;
				}
			});
		},

		getItems: function () {
			$.get("/api/Cart", {}, function (response) {
				if (currentCallback != null) {
					currentCallback();
					currentCallback = null;
				}
			});
		},

		changeItem: function (productId, quantity) {
			var data = {
				ProductId: productId,
				Quantity: quantity
			};

			$.post("/api/Cart", data, function (response) {
				if (currentCallback != null) {
					currentCallback();
					currentCallback = null;
				}
			});
		},

		removeItem: function (productId) {
			var data = {
				ProductId: productId,
			};

			$.delete("/api/Cart", data, function (response) {
				if (currentCallback != null) {
					currentCallback();
					currentCallback = null;
				}
			});
		},
	};
} ();