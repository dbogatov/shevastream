$(document).ready(function () {
	$("#feedback").on("submit", function (e) {
        e.preventDefault();

		var valid = true;

		$('input,textarea,select').filter('[required]:visible').each(function () {
			valid = valid && $(this).val().length > 0;
		});

		if (!valid) {
			$("#feadbackValidationAlert").show();
		} else {
			$("#feadbackValidationAlert").hide();
			var data = {
				Email: $("[name='email']").val(),
				Subject: $("[name='subject']").val(),
				Body: $("[name='message']").val(),
				Name: $("[name='name']").val(),
			};

			$.post("/api/Feedback", data, function () {
				$("#feadbackValidationSuccess").show();
				$("#feedback").find("input[type=text], input[type=email], textarea").val("");
			});
		}
	});
});