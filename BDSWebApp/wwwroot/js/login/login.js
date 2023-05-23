$(document).ready(function () {
    try {
        $('#btnLogin').click(function () {
            $('#btnLogin').attr("disabled", true);

            var model = {
                tellerNumber: $('#txtTellerNumber').val()
            }
            $.ajax({
                url: $('#Login').val(),
                data: { dataModel: JSON.stringify(model) },
                type: "POST",
                success: function (result) {
                    if (result.responseCode != 0) {
                        alert(result.responseMessage);
                        return;
                    }

                    alert("Place your finger");
                    window.location.href = result.url;


                }, error: function (result) {
                    alert("ERROR");
                }
            });
            $('#btnLogin').attr("disabled", false);

        });

        $('input').keydown(function (e) {
            if (e.keyCode == 13) {
                $("#btnLogin").focus().click();
                return false;
            }
        });


    } catch (e) {
        alert(e.message);
    }

});


