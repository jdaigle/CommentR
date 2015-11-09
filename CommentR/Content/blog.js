// requires jquery

window.commentr = {
    config: {
        stylesheetURL: '/css',
        loadCommentsURL: '/comments',
        submitCommentURL: '/comment'
    }
},

$(function () {
    var placeholderElement = $($("[data-commentr-placeholder]")[0]);
    var permalink = $(placeholderElement).data('commentr-permalink');

    placeholderElement.addClass("commentr");

    var newSS = document.createElement('link');
    newSS.rel = 'stylesheet';
    newSS.href = window.commentr.config.stylesheetURL;
    document.getElementsByTagName("head")[0].appendChild(newSS);

    var handleLoadedComments = function (html) {
        placeholderElement.empty();
        placeholderElement.append(html);
        $("[data-commentr-datetime]").each(function (i, el) {
            var dt = new Date($(el).data("commentr-datetime"));
            $(el).html(dt.toLocaleString());
        });
    };

    // load the comments
    $.ajax({
        dataType: "html",
        url: window.commentr.config.loadCommentsURL,
        data: { permalink: permalink },
        success: handleLoadedComments
    }).fail(function () {
        placeholderElement.empty();
        placeholderElement.find('.error').remove();
        placeholderElement.html('<span class="error">Error Loading Comments</span>');
    });

    // handle new comment
    $(placeholderElement).on('submit', '#commentr-submit-form', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            type: "POST",
            dataType: "html",
            url: window.commentr.config.submitCommentURL,
            data: formData,
            success: handleLoadedComments
        }).fail(function () {
            placeholderElement.find('.error').remove();
            placeholderElement.append('<span class="error">Error Submitting Comment ' + new Date().getTime() + '</span>');
        });
    });
});