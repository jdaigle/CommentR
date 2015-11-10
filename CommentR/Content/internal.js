window.parent.postMessage(["setHeight", 50], "*");

function resize() {
    var height = document.body.scrollHeight;
    console.log(height);
    window.parent.postMessage(["setHeight", height], "*");
}

// requires jquery
$(function () {
    var placeholderElement = $("#comments-placeholder");
    placeholderElement.addClass("commentr");

    var handleLoadedComments = function (html) {
        placeholderElement.empty();
        placeholderElement.append(html);
        $("[data-commentr-datetime]").each(function (i, el) {
            var dt = new Date($(el).data("commentr-datetime"));
            $(el).html(dt.toLocaleString());
        });
        resize();
    };

    var loadComments = function () {
        $.ajax({
            dataType: "html",
            url: "/comments",
            data: { permalink: window.commentr.config.permalink },
            success: handleLoadedComments
        }).fail(function () {
            placeholderElement.empty();
            placeholderElement.find('.error').remove();
            placeholderElement.html('<span class="error">Error Loading Comments</span>');
            resize();
        });
    };

    // handle new comment
    $(placeholderElement).on('submit', '#commentr-submit-form', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/comment",
            data: formData,
            success: handleLoadedComments
        }).fail(function () {
            placeholderElement.find('.error').remove();
            placeholderElement.append('<span class="error">Error Submitting Comment ' + new Date().getTime() + '</span>');
            resize();
        });
    });

    loadComments();
});