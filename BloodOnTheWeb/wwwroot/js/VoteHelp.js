$(function () {

    $(".voter.me").on('click', function () {
        $(".vote.me").toggleClass("execute-vote");
        $(".vote.me").toggleClass("abstain-vote");
    });

    $("#kill-switch").on("click", function () {

        $(this).siblings(".voter").toggleClass("alive");
        $(this).siblings(".voter").toggleClass("dead");
    });

    $("#used-switch").on("click", function () {
        $(this).siblings(".voter").find(".vote").toggleClass("used-vote");

    });

});