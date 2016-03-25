var total = 0;

$(document).ready(function() {
	$("#phone").mask("(999) 999-9999");

	$("#shipmentMethod").change(function() {
		if ($(this).val() !== "1") {
			$("#addressGroup").show();
		} else {
			$("#addressGroup").hide();
		}
	});



	$("#oderForm").on("submit", function(e) {
        e.preventDefault();

		var valid = true;

		$('input,textarea,select').filter('[required]:visible').each(function() {
			valid = valid && $(this).val().length > 0;
		});

		if (!valid) {
			$("#orderValidationAlert").show();
		} else {
			$("#orderValidationAlert").hide();

			var price = 170;

			$("#prevItem").text($("#item option:selected").text());
			$("#prevQuantity").text($("#quantity option:selected").text());
			$("#prevPrice").text(price + " UAH");
			$("#prevShippmentMethod").text($("#shipmentMethod option:selected").text());
			$("#prevPhone").text("+38 " + $("#phone").val());

			total = price * parseInt($("#quantity").val(), 10);
			var priceText = "";

			switch ($("#shipmentMethod").val()) {
				case "2":
					total += 30;
					priceText = total + " UAH";
					break;
				case "3":
					priceText = total + " UAH + Shipment cost (determined individually)";
					break;
				default:
					priceText = total + " UAH";
					break;
			}

			$("#prevTotal").text(priceText);

			$('#orderModal').modal({
				keyboard: false
			});

			$("#orderModal").show();
		}
	});

	$("#confirmOrderBtn").click(function(e) {
		e.preventDefault();
		$(this).attr("disabled", "disabled");
		$(this).html("Завантаження...");

		var data = {
			ProductId: parseInt($("#item").val(), 10),
			Quantity: parseInt($("#quantity").val(), 10),
			CustomerName: $("#name").val(),
			CustomerEmail: $("#email").val(),
			CustomerPhone: $("#phone").val(),
			PaymentMethodId: parseInt($("#paymentMethod").val(), 10),
			ShipmentMethodId: parseInt($("#shipmentMethod").val(), 10),
			Address: $("#address").val(),
			Comment: $("#comment").val(),

			ProductName: $("#item option:selected").text(),
			PaymentMethodName: $("#paymentMethod option:selected").text(),
			ShipmentMethodName: $("#shipmentMethod option:selected").text(),
			TotalAmountDue: total
		};

		$.put("/api/Order", data).always(function() {
			location.href = "/ThankYou";
		});
	});
});