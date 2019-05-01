"use strict";

    
$(function () {
    $(".voter.me").on('click', function() {
        var my_vote = $(".voter.me");

        var current_vote = my_vote.attr("data-current").toLowerCase() === "true";
         my_vote.attr("data-current", !current_vote);
        
        SendMyStatus();
    });

    $("#my_name").on("change", function() {
        SendMyStatus();
    });

    $("#kill-switch").on('click', function() {
        var my_vote = $(".voter.me");

        if (my_vote.attr("data-health") === "dead") {

            my_vote.removeClass("dead");
            my_vote.addClass("alive");
            my_vote.attr("data-health", "alive");

        } else {

            my_vote.removeClass("alive");
            my_vote.addClass("dead");
            my_vote.attr("data-health", "dead");

        }

        SendMyStatus();
    });

    $("#used-switch").on('click', function() {
        var my_vote = $(".vote.me");

        if (my_vote.attr("data-vote") === "free") {

            my_vote.addClass("used-vote");
            my_vote.attr("data-vote", "used");

        } else {

            my_vote.removeClass("used-vote");
            my_vote.attr("data-vote", "free");

        }

        SendMyStatus();

    });
});

function SendMyStatus() {
    var my_vote = $(".voter.me");

    var voter_id = my_vote.attr("id");
    var current_vote = my_vote.attr("data-current").toLowerCase() === "true";
    var my_name = $("#my_name").val();
    var health = my_vote.attr("data-health");
    var vote_status = my_vote.find(".vote.me").attr("data-vote");

    connection.invoke("ClientToServerVote", voter_id, my_name, current_vote, health, vote_status, SessionId).catch(function(err) {
        return console.error(err.toString());
    });
}



var connection = new signalR.HubConnectionBuilder().withUrl("/Clocktower").build();

connection.on("ServerToClientVote", function (voter_id, voter_name, new_vote, vote_status, health) {
    var voter = $("#" + voter_id);
    voter.attr("data-current", new_vote);
    voter.siblings(".username.them").text(voter_name);

    if (vote_status === "free") {
        voter.find(".vote").removeClass("used-vote");
    } else {
        voter.find(".vote").addClass("used-vote");
    }

    if (health === "alive") {
        voter.removeClass("dead");
        voter.addClass("alive");
        
    } else {
        voter.removeClass("alive");
        voter.addClass("dead");
    }

    if (new_vote === "True") {
        voter.find(".vote").removeClass("execute-vote");
        voter.find(".vote").addClass("abstain-vote");
    } else {
        voter.find(".vote").removeClass("abstain-vote");
        voter.find(".vote").addClass("execute-vote");
    }
});

connection.on("GetCurrentClientVote", function() {
    SendMyStatus();
});


connection.start().then(function() {
    connection.invoke("JoinSession", SessionId);
    connection.invoke("ClientRequestsLatest", SessionId);
}).catch(function (err) {
    return console.error(err.toString());
});






