$(document).ready(function () {

    $("#createaccount").click(function () {
        $(".login-form").hide();
        $(".register-form").slideDown("slow");
        return false;
    });

    $("#login").click(function () {
        $(".register-form").hide();
        $(".login-form").slideDown("slow");
        return false;
    });
    
    $("#loginbtn").click(function () {
        var $userlogin = $("#userlogin").val();
        var $userpassword = $("#userpassword").val();

        if ((!$userlogin) || (!$userpassword)) {
            var $alert = $("#lg-msg");
            $alert.addClass("alert-danger");
            $alert.find("strong").text("Error ! ");
            $alert.find("span").text("One or More Fields Are Empty");
            $alert.slideDown("slow").delay(3000).slideUp("slow");
            return false;
        }

        var $data = { 'Login': $userlogin, 'Password': $userpassword };

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/User/ValidateUser',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                if (result == "") {
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-danger");
                    $alert.find("strong").text("Error ! ");
                    $alert.find("span").text("Invalid Username Or Password");
                    $alert.slideDown("slow").delay(3000).slideUp("slow");
                    $('#userpassword').val("");
                }
                else {
                    window.location.href = '/Home';
                }
            },
            error: function () {
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Error ! ");
                $alert.find("span").text(err.statusText);
                $alert.slideDown("slow").delay(3000).slideUp("slow");
            }
        });

    });

    $("#registerbtn").click(function () {
        var $regusername = $("#regusername").val();
        var $reguserlogin = $("#reguserlogin").val();
        var $reguseremail = $("#reguseremail").val();
        var $reguserpassword = $("#reguserpassword").val();

        if ((!$regusername) || (!$reguserlogin) || (!$reguseremail) || (!$reguserpassword)) {
            var $alert = $("#lg-msg");
            $alert.addClass("alert-danger");
            $alert.find("strong").text("Error ! ");
            $alert.find("span").text("One or More Fields Are Empty");
            $alert.slideDown("slow").delay(3000).slideUp("slow");
            return false;
        }

        var $data = { 'Name': $regusername, 'Login': $reguserlogin, 'Email': $reguseremail, 'Password': $reguserpassword };

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/User/RegisterUser',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                if (result == 0) {
                    $("#sm-msg").fadeIn("slow");
                    $('#sm-msg').delay(5000).fadeOut("slow");
                }
                else {
                    $("#login").trigger('click');
                    $("#userlogin").val($reguserlogin);
                    var $alert = $("#lg-msg");
                    $alert.removeClass("alert-danger");
                    $alert.addClass("alert-success");
                    $alert.find("strong").text("Well Done ! ");
                    $alert.find("span").text("Registration Successfull");
                    $alert.slideDown("slow").delay(3000).slideUp("slow");
                    $("#registerbtn").closest("div").find("input").val("");
                }
            },
            error: function (err) {
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Error ! ");
                $alert.find("span").text(err.statusText);
                $alert.slideDown("slow").delay(3000).slideUp("slow");
            }
        });

    });
});