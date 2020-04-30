$(function() {
    UpdateDropDown();

    $("#nominated-voter").on("change", function() {
        connection.invoke("AdminSendNominatedVoter", $(this).val(), SessionId).catch(function(err) {
            return console.error(err.toString());
        });
    });


    $("#start-vote").on("click", function() {
        

        if ($("#ReadyCheckToggle").is(":checked")) {
            connection.invoke("AdminTriggerReadyCheck", SessionId);
        } else {
            var Time = $("#TimePerVoter").val();
            var Type = $("input[name='vote-type']:checked").val();

            connection.invoke("AdminSendStartTimer", Time, Type, SessionId).catch(function (err) {
                return console.error(err.toString());
            });
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
});


connection.on("PlayerReady", function (voter_id) {
    if ($(".voter.player.not-ready").length === 0) {
        var Time = $("#TimePerVoter").val();
        var Type = $("input[name='vote-type']:checked").val();
        connection.invoke("AdminSendStartTimer", Time, Type, SessionId).catch(function (err) {
            return console.error(err.toString());
        });
    }
});

function UpdateDropDown() {
    $("#nominated-voter").children("option").remove();
    $("#swap-voter-one").children("option").remove();
    $("#swap-voter-two").children("option").remove();

    $(".username").each(function () {
        
        var id = $(this).siblings(".voter").attr("id");
        var Username = id;
        var number = $(this).siblings(".voter").attr("data-id");

        if (!isEmpty($(this))) {
            Username = "("+number+") " + $(this).text();
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

function isEmpty(el) {
    return !$.trim(el.html());
}