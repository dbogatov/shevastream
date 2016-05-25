var total = 0;

var products = [];

$(document).ready(function () {

	$("#phone").mask("(999) 999-9999");

	$("#shipmentMethod").change(function () {
		if ($(this).val() !== "1") {
			$("#addressGroup").show();
		} else {
			$("#addressGroup").hide();
		}
	});



	$("#oderForm").on("submit", function (e) {
        e.preventDefault();

		var valid = true;

		$('input,textarea,select').filter('[required]:visible').each(function () {
			valid = valid && $(this).val().length > 0;
		});

		if (!valid) {
			$("#orderValidationAlert").show();
		} else {
			$("#orderValidationAlert").hide();

			$("#putOrderBtn").attr("disabled", "disabled");
			$("#putOrderBtn").text("Завантаження...");

			var data = {
				CustomerName: $("#name").val(),
				CustomerEmail: $("#email").val(),
				CustomerPhone: $("#phone").val(),
				PaymentMethodId: parseInt($("#paymentMethod").val(), 10),
				ShipmentMethodId: parseInt($("#shipmentMethod").val(), 10),
				Address: $("#address").val(),
				Comment: $("#comment").val(),

				PaymentMethodName: $("#paymentMethod option:selected").text(),
				ShipmentMethodName: $("#shipmentMethod option:selected").text(),
			};

			$.put("/api/Order", data).always(function () {
				location.href = "/ThankYou";
			});
		}
	});
});