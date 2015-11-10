var _commentsFrame = window.document.getElementById("comments-frame");

var permalink = _commentsFrame.getAttribute("data-commentr-permalink");
var permalinkEncoded = encodeURIComponent(permalink);

window.addEventListener('message', function (e) {
    var eventName = e.data[0];
    var data = e.data[1];
    switch (eventName) {
        case 'setHeight':
            _commentsFrame.height = data + "px";
            break;
    }
}, false);
_commentsFrame.style.width = "100%";
_commentsFrame.style.border = "none";
_commentsFrame.style.overflow = "hidden";
_commentsFrame.src = "/frame?permalink=" + permalinkEncoded;