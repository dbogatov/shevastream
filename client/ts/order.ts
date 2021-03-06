import * as Cookies from "js-cookie";
import "jquery-mask-plugin"

$(() => {

	$("#addressGroup").hide();

	// Restore user information if available
	var userData = Cookies.get("UserOrderData");

	if (userData != null) {
		var data = JSON.parse(userData);
		$("[name='CustomerName']").val(data.Name);
		$("[name='CustomerEmail']").val(data.Email);
		$("[name='CustomerPhone']").val(data.Phone);
		$("[name='Address']").val(data.Address);
	}

	$("[name='CustomerPhone']").mask("(999) 999-9999");

	$("[name='ShipmentMethodId']").change(function () {
		if ($(this).val() !== "1") {
			$("#addressGroup").show();
		} else {
			$("#addressGroup").hide();
		}
	});

	$("form").submit(function () {

		$("#submitBtn").attr("disabled", "disabled");
		$("#submitBtn").text("Завантаження...");
		
		return true;
	});
});
