$(function () {
    UpdateDropDown();


    $("#start-vote").on("click", function () {
        if ($("#ReadyCheckToggle").is(":checked")) {
            connection.invoke("AdminTriggerReadyCheck", SessionId);
        } else {
            StartVote();
        }
    });

    $("#change-players").on("click", function () {
        var NewPlayerNumber = $("#number-of-players").val();

        connection.invoke("AdminSendNewPlayerCount", NewPlayerNumber, SessionId).catch(function (err) {
            return console.error(err.toString());
        });
    });

    $("#swap-button").on("click", function () {
        connection.invoke("AdminSwapPlayers", $("#swap-voter-one").val(), $("#swap-voter-two").val(), SessionId).catch(function (err) {
            return console.error(err.toString());
        });
    });

    $("#clear-log").on("click", function () {
        connection.invoke("AdminUpdateRecord", SessionId, "<i>No Votes Recorded</i>");
    });

    $("#RecordResultDialog").dialog({
        autoOpen: false,
        modal: true,
        position: { my: "center", at: "center", of: "#container" },
        buttons: {
            "Record": recordVote,
            "Close without recording": function () {
                $("#RecordResultDialog").dialog("close");
            }
        }
    });

    checkAFK();
});


connection.on("PlayerReady", function (voter_id) {
    if ($(".voter.player.not-ready").length === 0) {
        StartVote(); 
    }
});

function StartVote() {
    var Time = $("#TimePerVoter").val();
    var Type = $("input[name='vote-type']:checked").val();
    var Start = $("#nominated-voter option:selected").val();
    console.log(Start);
    connection.invoke("AdminSendStartTimer", Time, Type, Start, SessionId).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("PlayerRequestPlayerNumber", function () {
    var NewPlayerNumber = $("#number-of-players").val();

    connection.invoke("AdminSendNewPlayerCount", NewPlayerNumber, SessionId).catch(function (err) {
        return console.error(err.toString());
    });
});

function triggerRecordVote(LastVoter) {
    if ($("#LogVoteCheck").is(":checked")) {
        var Votes = $(".vote.execute-vote").length;
        $("#VoteResult").val(Votes + " votes for " + LastVoter);
        $("#RecordResultDialog").dialog("open");
    }
}

function recordVote() {

    $("#vote-log").find("i").remove();
    $("#vote-log").append('<div><span>' + moment().toISOString() + ': </span> ' + $("#VoteResult").val() + '</div>');
    $("#RecordResultDialog").dialog("close");
    connection.invoke("AdminUpdateRecord", SessionId, $("#vote-log").html());

}

function UpdateDropDown() {
    $("#nominated-voter").children("option").remove();
    $("#swap-voter-one").children("option").remove();
    $("#swap-voter-two").children("option").remove();

    $(".username").each(function () {

        var id = $(this).siblings(".voter").attr("id");
        var Username = id;
        var number = $(this).siblings(".voter").attr("data-id");

        if (!isEmpty($(this))) {
            Username = "(" + number + ") " + $(this).text();
        }

        $("#nominated-voter").append(
            $('<option></option>').val(id).text(Username)
        );

        $("#swap-voter-one").append(
            $('<option></option>').val(number).text(Username)
        );

        $("#swap-voter-two").append(
            $('<option></option>').val(number).text(Username)
        );
    });
}

function checkAFK() {
    if ($(".voter.player[data-afk=true]").length > 0) {
        $("#start-vote").prop('disabled', true);
        $("#afk-error").show();
    } else {
        $("#start-vote").prop('disabled', false);
        $("#afk-error").hide();
    }
}

function isEmpty(el) {
    return !$.trim(el.html());
}