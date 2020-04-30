"use strict";



var MyStatus = {
    health: null,
    vote_status: null,
    current_vote: null
};

var MyName = "";



$(function () {

    if (window.location.href.split('/').length !== 8) {
        $("#admin-link")[0].click();
    }

    $(".clock").hide();

    $("#my_name_display").hide();
    $("#my_name_display").text($("#my_name").val());

    $(".voter.me").on('click', function () {
        var my_vote = $(".voter.me");

        var current_vote = my_vote.attr("data-current").toLowerCase() === "true";
        my_vote.attr("data-current", !current_vote);

        SendMyStatus();
    });

    $("#my_name").on("change", function () {
        SendMyStatus();
        $("#my_name_display").text($("#my_name").val());
    });



    $("#my_name").on("focusout", function () {
        toggleMyName();
    });

    $("#my_name_display").on("click", function () {
        toggleMyName();
    });


    $("#kill-switch").on('click', function () {
        var my_vote = $(".voter.me");


        if (my_vote.attr("data-health") === "alive") {
            $(this).removeClass("alive");
            $(this).removeClass("traveler");
            $(this).addClass("dead");
            my_vote.removeClass("alive");
            my_vote.removeClass("traveler");
            my_vote.addClass("dead");
            my_vote.attr("data-health", "dead");
        } else if (my_vote.attr("data-health") === "dead") {
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

        SendMyStatus();
    });

    $("#used-switch").on('click', function () {
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

    $("#ReadyCheckDialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            "Ready": SendReady
            }
    });



});

function toggleMyName() {

    $("#my_name_display").text($("#my_name").val());

    if ($("#my_name").val() !== "") {
        if ($("#my_name").is(":hidden")) {
            $("#my_name").show();
            $("#my_name_display").hide();
            $("#my_name").get(0).focus();

        } else {
            $("#my_name").hide();
            $("#my_name_display").show();
        }
    }
}





var connection = new signalR.HubConnectionBuilder().withUrl("/Clocktower").build();

var Timer;
var AnalogTimer;



connection.on("StartTimer", function (timePerUser, type) {
    $(".voting").hide();
    $(".voter").removeClass("not-ready");
    var Voters = $(".vote:not(.used-vote)").parent(".voter.player");
    var TimePerUser = 0;
    var i = 0;

    clearInterval(Timer);

    TimePerUser = timePerUser;
    Voters.siblings(".timer").text(timePerUser);
    $("#master-timer").text(timePerUser);
    $(Voters[i]).siblings(".voting").show();
    $("#current-voter").text($(Voters[i]).siblings(".username").text());

    Timer = setInterval(UserTimer, 1000);

    function UserTimer() {        

        var TimerDiv = $(Voters[i]).siblings(".timer");
        var MasterTimer = $("#master-timer");

        if (i < Voters.length) {
            var MasterNext = MasterTimer.text() - 1;
            MasterTimer.text(MasterNext);
            TimerDiv.text(MasterNext); //individual timer for non circle votes

            if (MasterNext === -1) {
                TimerDiv.text("");
                i++;

                $(".voting").hide();
                if (i < Voters.length) {
                    MasterTimer.text(TimePerUser);
                    $(Voters[i]).siblings(".voting").show();
                    $("#current-voter").text($(Voters[i]).siblings(".username").text());
                } else {
                    MasterTimer.text("");
                    $("#current-voter").text("");
                }
            }

        } else {
            clearInterval(Timer);
        }

    }
});

connection.on("SwapPlayers", function (voterOne, voterTwo) {

    if ($(".voter.me").attr("data-id") !== "0") {
        var MyId = $(".voter.me").attr("data-id");

        if (MyId === "" + voterOne) {
            window.location.href = "/vote/index/" + numberOfVoters + "/" + voterTwo + "/" + SessionId;
        }

        if (MyId === "" + voterTwo) {
            window.location.href = "/vote/index/" + numberOfVoters + "/" + voterOne + "/" + SessionId;
        }
    }
});


connection.on("ChangePlayerNumber", function (newPlayerNumber) {
    var MyId = $(".voter.me").attr("data-id");

    if (MyId === undefined) {
        MyId = 0;
    }

    window.location.href = "/vote/index/" + newPlayerNumber + "/" + MyId + "/" + SessionId;
});

connection.on("ReOrderFromServer", function (nominatedVoterId) {


    var FirstVoter = $("#" + nominatedVoterId).parent();
    var Preceeding = FirstVoter.prevAll();
    $("#container").append(Preceeding.get().reverse());

    var i = 0;
    $("#container .info-holder").each(function () {
        i++;
        $(this).attr("data-sitting-order", i);
    });

});

connection.on("GetReadyResponse", function () {
    var my_vote = $(".voter.me");
    $(".voter.player").addClass("not-ready")
    if (my_vote.attr("data-id") !== "0") {
        $("#ReadyCheckDialog").dialog("open");
    }
});

function SendReady() {
    var my_vote = $(".voter.me");
    if (my_vote.attr("data-id") !== "0") {
        var voter_id = my_vote.attr("id");
        connection.invoke("ClientReadyResponse", SessionId, voter_id);
    }
    $("#ReadyCheckDialog").dialog("close");
}

connection.on("PlayerReady", function (voter_id) {
    var voter = $("#" + voter_id);
    voter.removeClass("not-ready");
});


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
        voter.removeClass("traveler");
        voter.addClass("alive");

    } else if (health === "traveler") {
        voter.removeClass("alive");
        voter.removeClass("dead");
        voter.addClass("traveler");
    } else {
        voter.removeClass("alive");
        voter.removeClass("traveler");
        voter.addClass("dead");
    }

    if (new_vote === "True") {
        voter.find(".vote").removeClass("execute-vote");
        voter.find(".vote").addClass("abstain-vote");
    } else {
        voter.find(".vote").removeClass("abstain-vote");
        voter.find(".vote").addClass("execute-vote");
    }

    if (typeof UpdateDropDown === "function") {
        UpdateDropDown();
    }


});






connection.on("GetCurrentClientVote", function () {
    SendMyStatus();
});


connection.start().then(StartupProcess).catch(function (err) {
    return console.error(err.toString());
});

function Reconnect() {
    connection.start().then(StartupProcess).catch(function (err) {
        return console.error(err.toString());
    });
}




function StartupProcess() {
    if ($(".voter.me").attr("data-id") !== "0") {

        if (getCookie(SessionId) !== null) {
            MyStatus = $.parseJSON(getCookie(SessionId));
            UpdateStatus(MyStatus.current_vote, MyStatus.vote_status, MyStatus.health);
        }

        if (getCookie("name") !== null) {
            MyName = getCookie("name");
            $("#my_name").val(MyName);
            toggleMyName();
        }
    }


    connection.invoke("JoinSession", SessionId, $(".voter.me").attr("data-id"));
    connection.invoke("ClientRequestsLatest", SessionId);
}


function SendMyStatus() {
    var my_vote = $(".voter.me");
    if (my_vote.attr("data-id") !== "0") {
        var voter_id = my_vote.attr("id");
        var current_vote = my_vote.attr("data-current").toLowerCase() === "true";
        var my_name = $("#my_name").val();
        var health = my_vote.attr("data-health");
        var vote_status = my_vote.find(".vote.me").attr("data-vote");



        connection.invoke("ClientToServerVote", voter_id, my_name, current_vote, health, vote_status, SessionId).catch(function (err) {
            return console.error(err.toString());
        });

        if ($(".voter.me").attr("data-id") !== "0") {
            MyStatus.health = health;
            MyStatus.current_vote = current_vote;
            MyStatus.vote_status = vote_status;

            setCookie(SessionId, JSON.stringify(MyStatus), 14);
            setCookie("name", my_name, 14);
        }
    }
}


function UpdateStatus(new_vote, vote_status, health) {

    $(".voter.me").attr("data-current", new_vote);
    $(".voter.me").attr("data-health", health);
    $(".voter.me .vote").attr("data-vote", vote_status);
    $("#my_name_display").text($("#my_name").val());
}

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
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}