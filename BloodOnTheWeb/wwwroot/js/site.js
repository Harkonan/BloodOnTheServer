// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    var theme = getCookie("theme");
    if (theme === "dark") {
        $("body").addClass("dark");
        $("body").removeClass("light");
        $("#theme-toggle").html("Light Mode");
    } else {
        $("body").removeClass("dark");
        $("body").addClass("light");
        $("#theme-toggle").html("Dark Mode");
    }

    $("#theme-toggle").on("click", function () {
        if ($("body").hasClass("dark")) {
            $("body").removeClass("dark");
            $(this).text("Dark Mode");
            setCookie("theme", "light",14);
        } else {
            $("body").addClass("dark");
            $(this).text("Light Mode");
            setCookie("theme", "dark", 14);
        }
    });
});


function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
