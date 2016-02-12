$(document).ready(function () {
	
	// Contact Maps
	$("#maps").gmap3({
		map: {
			options: {
				// <iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1360.8515410934035!2d30.508795777760245!3d50.47017566282568!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x40d4ce131df3c781%3A0xbaf2d859b6a4678e!2sMezhyhirska+St%2C+44%2C+Kyiv%2C+Ukraine!5e0!3m2!1sen!2sus!4v1455248738865" width="600" height="450" frameborder="0" style="border:0" allowfullscreen></iframe>
				center: [50.470909, 30.508872],
				zoom: 15,
				scrollwheel: false
			}
		},
		marker: {
			latLng: [50.470909, 30.508872],
			options: {
				icon: new google.maps.MarkerImage(
					"https://dl.dropboxusercontent.com/u/29545616/Preview/location.png",
					new google.maps.Size(48, 48, "px", "px")
					)
			}
		}
	});
	
	//Slider
	$("#slider").carousel({
		interval: 5000
	});

	$("#testi").carousel({
		interval: 4000
	});

	$("#itemsingle").carousel({
		interval: false
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

