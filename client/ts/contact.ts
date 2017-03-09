import "gmap3"

$(() => {
	$("#feedback").on("submit",  (e) => {
        e.preventDefault();

		let valid: boolean = true;

		$('input,textarea,select')
			.filter('[required]:visible')
			.each(function() {
				valid = valid && $(this).val().length > 0;
			});

		if (!valid) {
			$("#feadbackValidationAlert").show();
		} else {
			$("#feadbackValidationAlert").hide();
			
			let data = {
				Email: $("[name='email']").val(),
				Subject: $("[name='subject']").val(),
				Body: $("[name='message']").val(),
				Name: $("[name='name']").val(),
			};

			$.post("/api/Feedback", data, () => {
				$("#feadbackValidationSuccess").show();
				$("#feedback").find("input[type=text], input[type=email], textarea").val("");
			});
		}
	});
});
