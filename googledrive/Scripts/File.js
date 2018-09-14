$(document).ready(function () {
    $("#dummydownloadbtn").click(function () {
        var $filename = $("#filename").val();
        alert($filename);
        var $data = ({ 'UniqueName': $filename });
        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Download/UpdateDownloadCount',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                console.log(result);
                $("#adownloadbtn")[0].click();
            },
            error: function () {
                alert('Error Occurred');
            }
        });
    });
});