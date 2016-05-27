$(document).ready(function () {
	var postId = 0;

	if ($("#data").data("post") === undefined) {
		postId = -1;
	} else {
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
			if (response.length > 0) {
				location.href = "/Blog/" + response;
			} else {
				alert("Choose different title");
			}
		}).createPost(title, content, publish);
	}
}

function removePost(postId) {
	Blog.setCallback(function () {
		location.href = "/Blog";
	}).removePost(postId);
}