// Write your Javascript code.

$(document).ready(function() {
	$("#" + globalActive).addClass("active");

	registerCallMeBack();
});

$(document).ready(fixFooter);
$(window).resize(fixFooter);

function fixFooter() {
	if ($("body").height() < $(window.top).height()) {
		$("#footerFix").height($(window.top).height() - $("body").height());
	}
}

function registerCallMeBack() {

	$("#callMeBackModal").on("shown.bs.modal", function() {
		$("#callBackPhone").focus();
	});

	$("#callMeBackModal").on("hidden.bs.modal", function() {
		$("#callBackPhone").val("");
		$("#requestCallBackBody").show();
		$("#confirmCallBackBtn").show();
		$("#responseCallBackBody").hide();
	})

	$("#callBackPhone").mask("(999) 999-9999");

	$("#confirmCallBackBtn").click(function() {

		if ($("#callBackPhone").val().length === 0) {
			return;
		}

		var data = {
			Phone: $("#callBackPhone").val()
		};

		$.post("/api/Feedback/CallMeBack", data, function() {
			$("#confirmCallBackBtn").hide();
			$("#requestCallBackBody").hide();
			$("#responseCallBackBody").show();
		});
	});
}

jQuery.each(["put", "delete"], function(i, method) {
	jQuery[method] = function(url, data, callback, type) {
		if (jQuery.isFunction(data)) {
			type = type || callback;
			callback = data;
			data = undefined;
		}

		return jQuery.ajax({
			url: url,
			type: method,
			dataType: type,
			data: data,
			success: callback
		});
	};
});