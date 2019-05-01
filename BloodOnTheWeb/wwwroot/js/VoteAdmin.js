$(function() {
    UpdateDropDown();

    $("#nominated-voter").on("change", function() {
        connection.invoke("AdminSendNominatedVoter", $(this).val(), SessionId).catch(function(err) {
            return console.error(err.toString());
        });
    });


    $("#start-vote").on("click", function() {
        var Time = $("#TimePerVoter").val();

        connection.invoke("AdminSendStartTimer", Time, SessionId).catch(function(err) {
            return console.error(err.toString());
        });
    });

});


function UpdateDropDown() {
    $("#nominated-voter").children("option").remove();
    $(".username").each(function() {
        
        var id = $(this).siblings(".voter").attr("id");
        var Username = id;

        if (!isEmpty($(this))) {
            Username = $(this).text();
        }

        $("#nominated-voter").append(
            $('<option></option>').val(id).text(Username)
        );
});
}







function isEmpty(el) {
    return !$.trim(el.html());
}