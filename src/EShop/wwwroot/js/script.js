$(document).ready(function () {
	
	// Contact Maps
	$("#maps").gmap3({
		map: {
			options: {
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
});

