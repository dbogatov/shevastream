import "gmap3"

$(() => {

	// Contact Maps
	$("#maps").gmap3({
		map: {
			options: {
				center: [50.441892, 30.511544],
				zoom: 15,
				scrollwheel: false
			}
		},
		marker: {
			latLng: [50.441892, 30.511544],
			options: {
				// icon: new google.maps.MarkerImage(
				// 	"/images/location.png",
				// 	new google.maps.Size(48, 48, "px", "px")
				// )
			}
		}
	});

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
