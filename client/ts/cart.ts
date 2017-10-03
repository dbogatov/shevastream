// Event listeners for cart update

$(() => {

	let submitCartChanges = (jQueryObject) => {
		jQueryObject.parent().closest("form").submit();
	};

	$(".quantityPicker").change(function() {
		submitCartChanges($(this));
	});

	$(".quantityPicker").focusout(function() {
		submitCartChanges($(this));
	});

});
