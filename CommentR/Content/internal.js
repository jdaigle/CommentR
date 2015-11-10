
function resize() {
    var height = document.getElementsByTagName("html")[0].scrollHeight;
    window.parent.postMessage(["setHeight", height], "*");
}

// requires jquery
$(function () {
    var placeholderElement = $("#comments-placeholder");
    placeholderElement.addClass("commentr");

    var handleLoadedComments = function (html) {
        console.log("handleLoadedComments")
        placeholderElement.empty();
        placeholderElement.append(html);
        $("[data-commentr-datetime]").each(function (i, el) {
            var dt = new Date($(el).data("commentr-datetime"));
            $(el).html(dt.toLocaleString());
        });
        resize();
    };

    // load the comments
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
});