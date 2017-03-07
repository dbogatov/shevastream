var Blog = function () {
	
	var currentCallback = null;

	var executeCallback = function (param) {
		$(document).trigger("eshop.blogupdated");
		if (currentCallback != null) {
			currentCallback(param);
			currentCallback = null;
		}
	};

	return {
		setCallback: function (callback) {
			currentCallback = callback;
			return this;
		},

		createPost: function (title, content, active) {
			var data = {
				Active: active,
				Title: title,
				Content: content
			};

			$.put("/api/Blog", data, function (response) {
				if (response) {
					executeCallback(response);
				} else {
					alert("Choose another name");
				}
			});
		},

		getHtml: function (content) {
			var data = {
				Content: content
			};

			$.get("/api/Blog/GetHtml", data, function (response) {
				executeCallback(response);
			});
		},

		changePost: function (postId, title, content) {
			var data = {
				Id: postId,
				Title: title,
				Content: content
			};

			$.post("/api/Blog", data, function (response) {
				executeCallback();
			});
		},

		publishPost: function (postId) {
			var data = {
				Id: postId
			};

			$.post("/api/Blog/Activate", data, function (response) {
				executeCallback();
			});
		},

		unpublishPost: function (postId) {
			var data = {
				Id: postId
			};

			$.post("/api/Blog/Deactivate", data, function (response) {
				executeCallback();
			});
		},

		removePost: function (postId) {
			var data = {
				Id: postId
			};

			$.delete("/api/Blog", data, function (response) {
				executeCallback();
			});
		},
	};
} ();
