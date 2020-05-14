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

        $.ajax({
            url: "/api/voteapi/SetSeats",
            data: { "SessionId": SessionId, "NewSeats": NewPlayerNumber },
            success: function (){
                connection.invoke("AdminTriggerCheckPlayerCount", SessionId).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        });
    });

    SwapChange();

    $("input[type=radio][name=swapType]").on('change', function () {
        SwapChange();
    });

    $("#swap-button").on("click", function () {
        if ($("input[name='swapType']:checked").val() === "s") {


            connection.invoke("AdminSwapPlayers", $("#swap-voter-one").val(), $("#swap-voter-two").val(), SessionId).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else if ($("input[name='swapType']:checked").val() === "r") {

            var VotersIds = [];

            $(".voter.player").each(function () {
                VotersIds.push($(this).attr("data-id"));
            });

            var NewVotersIds = VotersIds.slice();
            shuffle(NewVotersIds);


            for (var i = 0; i < VotersIds.length; i++) {
                connection.invoke("AdminSwapPlayers", VotersIds[i], NewVotersIds[i], SessionId).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }

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

var PingRequestRunning = false;

function SendPing() {
    var PingID = Math.random().toString().slice(2);
    PingRequestRunning = PingID;
    connection.invoke("AdminPing", SessionId, PingID);
}

var Pongs = [];

connection.on("AdminTriggerPing", function () {
    SendPing();
});

connection.on("AdminRecievdPong", function (PingID, Seat, UID) {
    var details = { "PingId": PingID, "Seat": Seat, "UID": UID };
    Pongs.push(details);

    var PingIdPongs = Pongs.filter(function (item) {
        return item.PingId === PingID;
    });

    var result = Object.values(PingIdPongs.reduce((c, v) => {
        let k = v.Seat;
        c[k] = c[k] || [];
        c[k].push(v);
        return c;
    }, {})).reduce((c, v) => v.length > 1 ? c.concat(v) : c, []);    
    
    if (result.length > 0) {
        if (PingRequestRunning === result[0].PingId) {
            PingRequestRunning = false;
            result.pop();
            console.log(result);
            for (var i = 0; i < result.length; i++) {
                connection.invoke("ResolveDuplicates", SessionId, result[i].UID);
            }
        }
    }
});

function SwapChange() {
    
    if ($("input[name='swapType']:checked").val() === 's') {
        $("#swap-voter-one").prop("disabled", false);
        $("#swap-voter-two").prop("disabled", false);
        $("#swap-button").val("Swap");
    } else {
        $("#swap-voter-one").prop("disabled", true);
        $("#swap-voter-two").prop("disabled", true);
        $("#swap-button").val("Shuffle");
    }
}

connection.on("PlayerReady", function (voter_id) {
    if ($(".voter.player.not-ready").length === 0) {
        StartVote();
    }
});

function StartVote() {
    var Time = $("#TimePerVoter").val();
    var Type = $("input[name='vote-type']:checked").val();
    var Start = $("#nominated-voter option:selected").val();
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

function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
        let j = Math.floor(Math.random() * (i + 1)); // random index from 0 to i

        // swap elements array[i] and array[j]
        // we use "destructuring assignment" syntax to achieve that
        // you'll find more details about that syntax in later chapters
        // same can be written as:
        // let t = array[i]; array[i] = array[j]; array[j] = t
        [array[i], array[j]] = [array[j], array[i]];
    }
}