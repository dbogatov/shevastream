import * as GMaps from "gmaps"

$(() => {

	new GMaps({
		div: '#maps',
		lat: 50.441892,
		lng: 30.511544
	}).addMarker({
		lat: 50.441892,
		lng: 30.511544
	});

	$("#feedback").on("submit", (e) => {
		e.preventDefault();

		let valid: boolean = true;

		$('input,textarea,select')
			.filter('[required]:visible')
			.each(function () {
				valid = valid && $(this).val().length > 0;
			});

		if (!valid) {
			$("#feedbackValidationAlert").show();
		} else {
			$("#feedbackValidationAlert").hide();

			let data = {
				Email: $("[name='email']").val(),
				Subject: $("[name='subject']").val(),
				Body: $("[name='message']").val(),
				Name: $("[name='name']").val(),
			};

			$.post("/api/Feedback", data, () => {
				$("#feedbackValidationSuccess").show();
				$("#feedback").find("input[type=text], input[type=email], textarea").val("");
			}, "text");
		}
	});
});
