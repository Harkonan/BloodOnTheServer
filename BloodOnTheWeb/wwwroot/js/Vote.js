﻿"use strict";



var MyStatus = {
    health: null,
    vote_status: null,
    current_vote: null
};

var MyName = "";


$(function () {
    GetStats();

    CheckPlayerNumber();

    $(".clock").hide();

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

    $("#AddNoteButton").on("click", function () {
        addNote();
        
    });

    $("#kill-switch").on('click', function () {
        var my_vote = $(".voter.me");
        if (my_vote.attr("data-health") === "alive") {
            //player is currently alive so clicking sets them as dead
            $(this).removeClass("alive");
            $(this).addClass("dead");
            my_vote.removeClass("alive");
            my_vote.addClass("dead");
            my_vote.attr("data-health", "dead");
            $("#traveller-switch").hide();
            $("#used-switch").show();
        } else if (my_vote.attr("data-health") === "dead") {
            //player is currently dead so clicking sets them as alive
            $(this).removeClass("dead");
            $(this).addClass("alive");
            my_vote.removeClass("dead");
            my_vote.addClass("alive");
            my_vote.attr("data-health", "alive");
            $("#traveller-switch").show();
            $("#used-switch").hide();
            //return vote if its currently set as used
            $(".vote.me").removeClass("used-vote");
            $(".vote.me").attr("data-vote", "free");


        }
        SendMyStatus();
    });

    $("#traveller-switch").on('click', function () {
        var my_vote = $(".voter.me");
        if (my_vote.attr("data-traveller") === "true") {
            //player is currently a traveller so clicking sets them as not
            $(this).removeClass("traveller");
            my_vote.removeClass("traveller");
            my_vote.attr("data-traveller", "false");
        } else if (my_vote.attr("data-traveller") === "false") {
            //player is currently a not traveller so clicking sets them as one
            $(this).addClass("traveller");
            my_vote.addClass("traveller");
            my_vote.attr("data-traveller", "true");
        }
        SendMyStatus();
    });



    $("#used-switch").on('click', function () {
        var my_vote = $(".vote.me");
        if (my_vote.attr("data-vote") === "free") {
            //User currently has a vote so remove it
            my_vote.addClass("used-vote");
            my_vote.attr("data-vote", "used");
        } else {
            //User currently has no vote so return it
            my_vote.removeClass("used-vote");
            my_vote.attr("data-vote", "free");
        }

        SendMyStatus();
    });

    $("#afk-switch").on("click", function () {
        var my_vote = $(".voter.me");
        var current_vote = my_vote.attr("data-afk").toLowerCase() === "true";
        my_vote.attr("data-afk", !current_vote);
        $(this).toggleClass("home");
        $(this).toggleClass("away");

        SendMyStatus();
    });

    $("#ReadyCheckDialog").dialog({
        autoOpen: false,
        modal: true,
        position: { my: "center", at: "center", of: "#container" },
        buttons: {
            "Ready": SendReady
        }
    });

    if ($("#VoteLog div").lenght > 0) {
        $("#vote-log-container").show();
    } else {
        $("#vote-log-container").hide();
    }

    loadNotes();
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

connection.on("StartTimer", function (timePerUser, type, start) {
    var startSeat = $("#" + start);

    console.log(start);
    $(".voting").hide();
    $(".voter").removeClass("not-ready");
    var OriginalVoters = $(".vote:not(.used-vote)").parent(".voter.player");
    var StartIndex = OriginalVoters.index(startSeat);

    var Start = OriginalVoters.splice(StartIndex);
    var Voters = Start.concat(OriginalVoters.toArray());

    var Nominated = Voters[Voters.length - 1];
    $(".nominated").remove();
    $(Nominated).append("<div class='nominated'></div>");

    var TimePerUser = 0;
    var i = 0;


    clearInterval(Timer);

    TimePerUser = timePerUser;
    for (var n = 0; n < Voters.length; n++) {
        $(Voters[n]).siblings(".timer").text(timePerUser);
    }


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
                    $(".nominated").remove();
                }
            }

        } else {
            clearInterval(Timer);
            if ($(".voter.me").attr("data-id") === "0") {
                triggerRecordVote($(Voters[i - 1]).siblings(".username").text().trim());
            }
        }

    }
});

connection.on("SwapPlayers", function (voterOne, voterTwo) {

    if (MySeatId !== 0 && MySeatId !== 100) {

        if (MySeatId === voterOne) {
            window.location.href = "/vote/" + voterTwo + "/" + SessionId;
        }

        if (MySeatId === voterTwo) {
            window.location.href = "/vote/" + voterOne + "/" + SessionId;
        }
    }
});

connection.on("ChangeSeat", function (oldSeat, newSeat) {
    if (MySeatId !== 0 && MySeatId !== 100) {
        if (MySeatId === oldSeat) {
            window.location.href = "/vote/" + newSeat + "/" + SessionId;
        }
    }
});

connection.on("CheckPlayerNumber", function () {
    CheckPlayerNumber();
});

function CheckPlayerNumber() {
    $.ajax({
        url: "/api/voteapi/GetSeats",
        data: { "SessionId": SessionId },
        success: function (newPlayerNumber) {
            var MyId = $(".voter.me").attr("data-id");

            if (MyId === undefined) {
                MyId = 0;
            }

            if (PlayerNumber !== newPlayerNumber) {
                window.location.href = "/vote/" + MyId + "/" + SessionId;
            }
        }
    });
}

connection.on("GetReadyResponse", function () {
    var my_vote = $(".voter.me");
    $(".voter.player").addClass("not-ready");
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


connection.on("ServerToClientVote", function (voter_id, voter_name, new_vote, vote_status, health, traveller, afk_status) {
    var voter = $("#" + voter_id);
    voter.attr("data-current", new_vote);
    voter.attr("data-occupied", true);

    var namechange = voter.siblings(".username.them").text().trim() !== voter_name.trim();
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

    if (traveller === "true") {
        voter.addClass("traveller");
    } else {
        voter.removeClass("traveller");
    }

    if (new_vote === "True") {
        voter.find(".vote").removeClass("execute-vote");
        voter.find(".vote").addClass("abstain-vote");
    } else {
        voter.find(".vote").removeClass("abstain-vote");
        voter.find(".vote").addClass("execute-vote");
    }

    voter.attr("data-afk", afk_status);

    if (namechange && typeof UpdateDropDown === "function") {
        UpdateDropDown();
    }

    if ($(".voter.me").attr("data-id") === "0") {

        checkAFK();
    }
    GetStats();
});

connection.on("AdminRequestPong", function (PingId) {
    if (MySeatId !== 0 && MySeatId !== 100) {
        connection.invoke("ClientPong", SessionId, PingId, MySeatId, MyUID);
    }
});

connection.on("ClientResolveDuplicates", function (UID) {
    if (UID === MyUID) {
        eraseCookie(SessionId + "_Seat");
        window.location.href = "/vote/join/" + SessionId;

    }
});


connection.on("UpdateLog", function (log) {
    $("#vote-log").html(log);

    if ($("#vote-log div").length > 0) {
        $("#vote-log-container").show();
    } else {
        $("#vote-log-container").hide();
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
    if (MySeatId !== 0 && MySeatId !== 100) {

        if (getCookie(SessionId) !== null) {
            MyStatus = $.parseJSON(getCookie(SessionId));
            UpdateStatus(MyStatus.current_vote, MyStatus.vote_status, MyStatus.health, MyStatus.traveller);
        }
        if (getCookie("name") !== null) {
            MyName = getCookie("name");
            $("#my_name").val(MyName);
            $("#my_name_display").text(MyName);

            if (MyName.length > 0) {
                $("#my_name").hide();
                $("#my_name_display").show();
            } else {
                $("#my_name").show();
                $("#my_name_display").hide();
            }
        }
    }

    connection.invoke("JoinSession", SessionId, MySeatId, MyUID);
    connection.invoke("ClientRequestsLatest", SessionId);

}

function SendMyStatus() {
    var my_vote = $(".voter.me");

    if (MySeatId !== 0 && MySeatId !== 100) {
        var voter_id = my_vote.attr("id");
        var current_vote = my_vote.attr("data-current").toLowerCase() === "true";
        var my_name = $("#my_name").val();
        var health = my_vote.attr("data-health");
        var traveller = my_vote.attr("data-traveller");
        var vote_status = my_vote.find(".vote.me").attr("data-vote");
        var afk_status = my_vote.attr("data-afk");


        connection.invoke("ClientToServerVote", voter_id, my_name, current_vote, health, traveller, vote_status, afk_status, SessionId).catch(function (err) {
            return console.error(err.toString());
        });

        if ($(".voter.me").attr("data-id") !== "0") {
            MyStatus.health = health;
            MyStatus.current_vote = current_vote;
            MyStatus.vote_status = vote_status;
            MyStatus.traveller = traveller;

            setCookie(SessionId, JSON.stringify(MyStatus), 14);
            setCookie("name", my_name, 14);
        }
    }
}

function GetStats() {
    $("#stat-players").text($(".voter.player:not(.traveller)").length);
    $("#stat-players-alive").text($(".voter.player.alive").length);
    $("#stats-travellers").text($(".voter.player.alive.traveller").length);
    $("#stats-votes-all").text($(".vote[data-vote=free]").length);
    $("#stats-votes-alive").text($(".voter.player.alive .vote[data-vote=free]").length);
    $("#stats-votes-dead").text($(".voter.player.dead .vote[data-vote=free]").length);
    $("#stats-min-vote").text(Math.ceil($(".voter.player.alive").length / 2));
    $("#stats-current-votes").text($(".vote[data-vote=free].execute-vote").length);

}

function UpdateStatus(new_vote, vote_status, health, traveller) {
    $(".voter.me").attr("data-current", new_vote);
    $(".voter.me").attr("data-health", health);
    if (health === "dead") {
        $("#traveller-switch").hide();
        $("#used-switch").show();
    }
    if (health === "alive") {
        $("#traveller-switch").show();
        $("#used-switch").hide();
    }
    $(".voter.me").attr("data-traveller", traveller);
    $(".voter.me .vote").attr("data-vote", vote_status);
    $("#my_name_display").text($("#my_name").val());

    GetStats();
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


function addNote(position, width=300, height=200, text="", NoteID) {
    if (NoteID === undefined) {
        NoteID = Math.random().toString().slice(2);
    }

    var $DialogHolder = $("#noteDialogs");
    
    var template = '<div id="dialog_' + NoteID + '" class="note-dialog"><textarea data-id=' + NoteID + '>'+text+'</textarea></div>';
    $DialogHolder.append(template);

    $("#dialog_" + NoteID).dialog({
        width: width+35,
        height: height + 38,
        dragStop: function () {
            saveDialog($("#dialog_" + NoteID + " textarea"));
        },
        resize: function () {
            saveDialog($("#dialog_" + NoteID + " textarea"));
        },
        close: function () {
            var data;
            if (getCookie(SessionId + "_dialogs") !== null) {
                data = $.parseJSON(getCookie(SessionId + "_dialogs"));
            } else {
                data = { [NoteID]: null };
            }

            delete data[NoteID];
            setCookie(SessionId + "_dialogs", JSON.stringify(data));
        }
    });


    if (position !== undefined) {
        $("#dialog_" + NoteID).dialog({
            position: {
                my: 'left top', at: 'left+' + position.left + ' top+'+ position.top, of:window } });
    }
   

    $("#dialog_" + NoteID + " textarea").on("change", function () {
        saveDialog($(this));
    });
}

function saveDialog(dialog) {
    var data;


    if (getCookie(SessionId + "_dialogs") !== null) {
        data = $.parseJSON(getCookie(SessionId + "_dialogs"));
    } else {
        data = { [$(dialog).attr("data-id")]: null };
    }

    data[$(dialog).attr("data-id")] = {
        text: $(dialog).val(),
        position: $("#dialog_" + $(dialog).attr("data-id")).offset(),
        width: $("#dialog_" + $(dialog).attr("data-id")).width(),
        height: $("#dialog_" + $(dialog).attr("data-id")).height()
    };
    setCookie(SessionId + "_dialogs", JSON.stringify(data));
}

function loadNotes() {
    if (getCookie(SessionId + "_dialogs") !== null) {
        var data = $.parseJSON(getCookie(SessionId + "_dialogs"));
        for (var i in data) {

            addNote(data[i].position, data[i].width, data[i].height, data[i].text, i);
        }
    }
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
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/';
}

