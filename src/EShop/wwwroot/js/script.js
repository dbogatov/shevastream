$(document).ready(function() {
	
	// Contact Maps
	$("#maps").gmap3({
		map: {
			options: {
			  center: [-7.866315,110.389574],
			  zoom: 8,
			  scrollwheel: false
			}  
		 },
		marker:{
			latLng: [-7.866315,110.389574],
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

	$("#feedback").on("submit", function(e){
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
			
			$.post("/api/Feedback", data, function() {
				$("#feadbackValidationSuccess").show();
			});
		}
		
		console.log("Submitted.");
	});
});

