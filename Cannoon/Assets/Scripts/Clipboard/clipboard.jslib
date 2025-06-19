mergeInto(LibraryManager.library, {
    CopyToClipboard: function (textPtr) {
        const text = UTF8ToString(textPtr);
        navigator.clipboard.writeText(text).then(function() {
            console.log("Copied to clipboard: " + text);
        }).catch(function(err) {
            console.error("Could not copy text: ", err);
        });
    }
});