// Write your Javascript code.

$(document).ready(function() {
	$("#" + globalActive).addClass("active");
	
	$("#callMeBackBtn").click(function() {
		alert("Poka eshche ne sdelal");
	});
	
	$("#feedback").on("submit", function (e) {
        e.preventDefault();

		var valid = true;

		if ($("[name='name']").val().length == 0) {
			valid = false;
		}

		if ($("[name='email']").val().length == 0) {
			valid = false;
		}

		if ($("[name='subject']").val().length == 0) {
			valid = false;
		}

		if ($("[name='message']").val().length == 0) {
			valid = false;
		}

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