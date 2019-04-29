"use strict";

    
$(function () {
    $(".voter.me").on('click', function() {
        
        var voter_id = $(this).attr("id");
        var current_vote = $(this).attr("data-current") === "True";
        var my_name = $("#my_name").val();

        connection.invoke("ClientToServerVote", voter_id, my_name, !current_vote).catch(function(err) {
            return console.error(err.toString());
        });
    });

    $("#my_name").on("change", function(){
        var my_vote = $(".voter.me");

        var voter_id = my_vote.attr("id");
        var current_vote = my_vote.attr("data-current") === "True";
        var my_name = $("#my_name").val();

        connection.invoke("ClientToServerVote", voter_id, my_name, current_vote).catch(function(err) {
            return console.error(err.toString());
        });
    });
});





var connection = new signalR.HubConnectionBuilder().withUrl("/Clocktower").build();

connection.on("ServerToClientVote", function (voter_id, voter_name, new_vote) {
    var voter = $("#" + voter_id);
    voter.attr("data-current", new_vote);
    voter.siblings(".username.them").text(voter_name);

    if (new_vote === "True") {
        voter.find(".vote").text("Y");
    } else {
        voter.find(".vote").text("N");
    }
});

connection.on("GetCurrentClientVote", function() {
    var voter_id = $(".voter.me").attr("id");
    var current_vote = $(".voter.me").attr("data-current") === "True";
    var voter_name = $("#my_name").val();
    connection.invoke("ClientToServerVote", voter_id, voter_name, current_vote).catch(function(err) {
        return console.error(err.toString());
    });

});


connection.start().then(function() {
    connection.invoke("ClientRequestsLatest");

}).catch(function (err) {
    return console.error(err.toString());
});






