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
            alert("Error ! One or More Fields Are Empty");
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
                    alert("Invalid Username Or Password");
                    $('#userpassword').val("");
                }
                else {
                    window.location.href = '/Home';
                }
            },
            error: function () {
                alert('Failed To Login ! Internal Error');
            }
        });

    });

    $("#registerbtn").click(function () {
        var $regusername = $("#regusername").val();
        var $reguserlogin = $("#reguserlogin").val();
        var $reguseremail = $("#reguseremail").val();
        var $reguserpassword = $("#reguserpassword").val();

        if ((!$regusername) || (!$reguserlogin) || (!$reguseremail) || (!$reguserpassword)) {
            alert("Error ! One or More Fields Are Empty");
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
                    alert("Login ! Already Exist Try Another");
                }
                else {
                    $("#login").trigger('click');
                    $("#userlogin").val($reguserlogin);
                    alert('Registration Successfull ! Try Login');
                }
            },
            error: function () {
                alert('Failed To Register ! Internal Error');
            }
        });

    });
});