// This instructions should be loaded for every page
// Most of it is dealing with the layout piece

import "jquery-mask-plugin"
import "bootstrap"

$(() => {

	$("[href='#']").click((e) => {
		// Cancel the jump
		e.preventDefault();
	});

	// Footer needs to be "stick" to the bottom
	let fixFooter = () => {
		if ($("body").height() < $(window.top).height()) {
			$("#footerFix").height($(window.top).height() - $("body").height());
		}
	};

	let registerCallMeBack = () => {

		$("#callMeBackModal").on("shown.bs.modal", () => {
			$("#callBackPhone").focus();
		});

		$("#callMeBackModal").on("hidden.bs.modal", () => {
			$("#callBackPhone").val("");
			$("#requestCallBackBody").show();
			$("#confirmCallBackBtn").show();
			$("#responseCallBackBody").hide();
		})

		$("#callBackPhone").mask("(999) 999-9999");

		$("#confirmCallBackBtn").click(() => {

			if ($("#callBackPhone").val().length === 0) {
				return;
			}

			var data = {
				Phone: $("#callBackPhone").val()
			};

			$.post("/api/CallbackRequest", data, () => {
				$("#confirmCallBackBtn").hide();
				$("#requestCallBackBody").hide();
				$("#responseCallBackBody").show();
			}, "text")
		});
	};

	$(document).ready(fixFooter);
	$(window).resize(fixFooter);
	registerCallMeBack();
});

// Extend JQuery with AJAX PUT and DELETE methods
jQuery.each(["put", "delete"], function (i, method) {
	jQuery[method] = function (url, data, callback, type) {
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
