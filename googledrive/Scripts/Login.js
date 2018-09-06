$(document).ready(function () {
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
});