$(function () {

    $(".voter.me").on('click', function () {
        $(".vote.me").toggleClass("execute-vote");
        $(".vote.me").toggleClass("abstain-vote");

    });

    $("#kill-switch").on("click", function () {
        var my_vote = $("#death");

        if ($(this).hasClass("alive")) {
            $(this).removeClass("alive");
            $(this).removeClass("traveler");
            $(this).addClass("dead");
            my_vote.removeClass("alive");
            my_vote.removeClass("traveler");
            my_vote.addClass("dead");
            my_vote.attr("data-health", "dead");
        } else if ($(this).hasClass("dead")) {
            $(this).removeClass("dead");
            $(this).removeClass("traveler");
            $(this).addClass("traveler");
            my_vote.removeClass("dead");
            my_vote.removeClass("traveler");
            my_vote.addClass("traveler");
            my_vote.attr("data-health", "traveler");
        } else {
            $(this).removeClass("dead");
            $(this).removeClass("traveler");
            $(this).addClass("alive");
            my_vote.removeClass("dead");
            my_vote.removeClass("traveler");
            my_vote.addClass("alive");
            my_vote.attr("data-health", "alive");
        }
    });

    $("#used-switch").on("click", function () {
        
        $("#dead-demo").find(".voter").find(".vote").toggleClass("used-vote");

    });

});