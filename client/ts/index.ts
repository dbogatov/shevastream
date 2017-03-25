import "bootstrap"

$(() => {

	let fixHeight = () => {

		var maxHeight = 0;

		["testimoni-item", "item", "post-title", "post-preview"].forEach(
			function (element, index, array) {
				maxHeight = 0;

				$("." + element).each(function () {
					var thisH = $(this).height();
					if (thisH > maxHeight) { maxHeight = thisH; }
				});

				$("." + element).height(maxHeight);
			}
		);

	}

	$(document).ready(function () { setTimeout(fixHeight, 300); });
	$(window).resize(fixHeight);

	//Slider
	$("#slider").carousel({
		interval: 5000
	});

	$("#testi").carousel({
		interval: 4000
	});

	$("#itemsingle").carousel({
		interval: 4000
	});
});


