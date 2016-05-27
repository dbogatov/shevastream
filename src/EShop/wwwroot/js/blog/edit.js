$(document).ready(function () {
	var postId = -1;

	if ($("#data").data("post") !== undefined) {
		postId = parseInt($("#data").data("post"));
	}

	$("#publishBtn").click(function () {
		savePost(true, postId);
	});

	$("#draftBtn").click(function () {
		savePost(false, postId);
	});

	$("#removeBtn").click(function () {
		removePost(postId);
	});
});

function savePost(publish, postId) {

	var title = $("#title").val().trim();
	var content = $("#content").val().trim();

	if (
		title.length == 0 ||
		content.length == 0
	) {
		alert("Fields may not be empty");
	}

	if (postId > -1) {
		Blog.changePost(postId, title, content);
		if (publish) {
			Blog.setCallback(function () {
				location.href = "/Blog/" + $("#data").data("url");
			}).publishPost(postId);
		} else {
			Blog.setCallback(function () {
				alert("Saved.");
			}).unpublishPost(postId);
		}
	} else {
		Blog.setCallback(function (response) {
			if (publish && response.length > 0) {
				location.href = "/Blog/" + response;
			} else if (response.length == 0) {
				alert("Choose different title");
			} else {
				location.href = "/Blog/Edit/" + response;
			}
		}).createPost(title, content, publish);
	}
}

function removePost(postId) {
	Blog.setCallback(function () {
		location.href = "/Blog";
	}).removePost(postId);
}