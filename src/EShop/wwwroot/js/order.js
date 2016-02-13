$(document).ready(function () {
	$("#phone").mask("(999) 999-9999");

	$("#oderForm").on("submit", function (e) {
        e.preventDefault();

		var valid = true;

		$('input,textarea,select').filter('[required]:visible').each(function() {
			valid = valid && $(this).val().length > 0;
		});
		
		if (!valid) {
			$("#orderValidationAlert").show();
		} else {
			$("#orderValidationAlert").hide();
			
			$('#orderModal').modal({
				keyboard: false
			});
			
			$("#orderModal").show();
		}
	});
});