$(document).ready(function () {
    LoadFolders();
    CreateBreadCrumbs("Home", 0);

    $("#cancelsharebtn").click(function () {
        $(this).closest("div").slideUp();
    });

    $("#suserlogin").keyup(function () {
        var $login = $("#suserlogin").val();
        if ($login == "") {
            $("#sharebtn").text("Close").removeClass("btn-outline-success").addClass("btn-outline-danger");
            $("#sharebtn").removeAttr("id").attr({ id: "cancelsharebtn" });
            return;
        }
        $("#cancelsharebtn").text("Share").removeClass("btn-outline-danger").addClass("btn-outline-success");
        $("#cancelsharebtn").removeAttr("id").attr({ id: "sharebtn" });
        return;
    });

    $("#sharedusers").on('click', "#removeuseranchor", function () {
        $("#spinner").show();
        var $tr = $(this).closest("tr");
        var $id = $("#sfileid").val();
        var $login = $tr.find(":nth-child(1)").text();
        if (!confirm("Are you sure ! You Want To Remove " + $login)) {
            return false;
        }
        var $data = ({ 'Id': $id, 'Login': $login });
        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Home/DeleteSharedUser',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                $("#spinner").hide();
                if (result > 0) {
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-success");
                    $alert.find("strong").text("Success ! ");
                    $alert.find("span").text("User Removed Successfully");
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                    $tr.remove();
                    LoadFolders();
                }
                else {
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-danger");
                    $alert.find("strong").text("Oops ! ");
                    $alert.find("span").text("Unable To Remove User");
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                }
            },
            error: function (err) {
                $("#spinner").hide();
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Error ! ");
                $alert.find("span").text(err.statusText);
                $alert.fadeIn("slow").delay(5000).slideUp("slow");
            }
        });
        return false;
    });

    $("#copyclipboardbtn").click(function () {
        var copyText = document.getElementById("copyclipboard");
        copyText.select();
        document.execCommand("copy");
        var $alert = $("#lg-msg");
        $alert.addClass("alert-success");
        $alert.find("strong").text("Success ! ");
        $alert.find("span").text("Link Copied To Clipboard");
        $alert.fadeIn("slow").delay(5000).slideUp("slow");
        $(this).hide("slow");
    });

    $("#searchinput").keyup(function () {
        $("#spinner").show();
        var $search = $("#searchinput").val();
        var $data = { 'search': $search };
        setTimeout(function () {
            $("TableBody").hide();
            if ($search == "") {
                LoadFolders();
                $("#spinner").hide();
                return false;
            }
            $.ajax({
                type: 'POST',
                dataType: 'JSON',
                url: '/Home/SearchFolders',
                data: JSON.stringify($data),
                contentType: "application/json; charset=utf-8",
                processdata: false,
                success: function (result) {
                    ClearTable();
                    console.log(result);
                    FolderResult(result);
                    $.ajax({
                        type: 'POST',
                        dataType: 'JSON',
                        url: '/Home/SearchFiles',
                        data: JSON.stringify($data),
                        contentType: "application/json; charset=utf-8",
                        processdata: false,
                        success: function (result) {
                            console.log(result);
                            FileResult(result);
                            $("TableBody").show();
                            $("#spinner").hide();
                        },
                        error: function (err) {
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("strong").text("Error ! ");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                },
                error: function (err) {
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-danger");
                    $alert.find("strong").text("Error ! ");
                    $alert.find("span").text(err.statusText);
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                }
            });    
        }, 500);
        return false;
    });

    function FolderResult(result) {
        for (var i = 0; i < result.length; i++) {
            var tr = $('<tr>');
            tr.attr({ value: result[i].Id, id: 'folder' });
            var td = $('<td>');
            td.text(result[i].Id);
            td.hide();
            tr.append(td);
            td = $('<td>');
            var div = $('<div>');
            div.attr({ class: 'icon folder' });
            td.append(div);
            var sp = $('<span>');
            sp.text(result[i].Name)
            td.append(sp);
            tr.append(td);
            td = $('<td>');
            tr.append(td);
            td = $('<td>');
            td.text("--");
            tr.append(td);
            td = $('<td>');
            td.text(result[i].CreatedOn);
            tr.append(td);
            td = $('<td>');
            td.text(result[i].ViewCount);
            tr.append(td);
            var body = $('#TableBody');
            body.append(tr);
            BindEvents();
        }
        return false;
    }

    function FileResult(result) {
        for (var i = 0; i < result.length; i++) {
            var tr = $('<tr>');
            tr.attr({ value: result[i].Id, id: 'file' });
            var td = $('<td>');
            td.text(result[i].Id);
            td.hide();
            tr.append(td);
            td = $('<td>');
            var div = $('<div>');
            div.attr({ class: 'icon file' });
            td.append(div);
            var sp = $('<span>');
            sp.text(result[i].Name + result[i].FileExt);
            td.append(sp);
            tr.append(td);
            //Share Icon
            td = $('<td>');
            if (result[i].Token != null) {
                var font = $('<i>');
                font.addClass('fa fa-chain');
                font.attr({ style: 'color: #428BCA' });
                td.append(font);
                if (result[i].Share != null) {
                    var font = $('<i>');
                    font.addClass('fa fa-user');
                    font.attr({ style: 'color: #428BCA' });
                    td.append(font);
                }
            }
            tr.append(td);
            td = $('<td>');
            td.text(result[i].FileExt);
            td.attr({ style: 'display : none;' });
            tr.append(td);
            td = $('<td>');
            td.text(result[i].FileSizeInKB + " KB");
            tr.append(td);
            td = $('<td>');
            td.text(result[i].UploadedOn);
            tr.append(td);
            td = $('<td>');
            td.text(result[i].Downloads);
            tr.append(td);
            var body = $('#TableBody');
            body.append(tr);
        }
        return false;
    }

    $(function () {
        $.contextMenu({
            selector: '#folder',
            autoHide: true,
            callback: function (key, options) {
                if (key == "rename") {
                    $("#createfolderbtn").trigger('click');
                    var $td1 = $(this).find(':nth-child(1)').text();
                    var $td2 = $(this).find('td:nth-child(2)').text();
                    $("#mfolderid").val($td1);
                    $("#mfoldername").val($td2);
                }           
                else
                if (key == "delete") {
                    $("#spinner").show();
                    if (!confirm("Are you sure ! Its Subfolders and files will be deleted")) {
                        $("#spinner").hide();
                        return;
                    }
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    var $name = $tr.find('td:nth-child(2)').text();
                    $.ajax({
                        url: '/Home/DeleteFolder/' + $id,
                        type: "POST",
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.removeClass("alert-danger");
                            $alert.addClass("alert-success");
                            $alert.find("strong").text("Yep ! ");
                            $alert.find("span").text("Folder \"" + $name + "\" Deleted Successfully");
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                            $tr.remove();
                        },
                        error: function (err) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("strong").text("Error ! ");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                }
            },
            items: {
                "rename": {name: "Rename", icon: "edit"},
                "delete": {name: "Delete", icon: "delete"}
            }
        });
        $.contextMenu({
            selector: '#file',
            autoHide: true,
            callback: function (key, options) {
                if (key == "rename") {
                    $("#createfileinput").slideDown();
                    var $td1 = $(this).find(':nth-child(1)').text();
                    var $td2 = $(this).find('td:nth-child(2)').text();
                    $td2 = $td2.split('.')[0];
                    $("#mfileid").val($td1);
                    $("#mfilename").val($td2);
                }
                else
                if (key == "delete") {
                    $("#spinner").show();
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    var $name = $tr.find('td:nth-child(2)').text();
                    if (!confirm("Are you sure you want to delete \"" + $name + "\"")) {
                        $("#spinner").hide();
                        return;
                    }

                    $.ajax({
                        url: '/Home/DeleteFile/' + $id,
                        type: "POST",
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.removeClass("alert-danger");
                            $alert.addClass("alert-success");
                            $alert.find("strong").text("Yep ! ");
                            $alert.find("span").text("File \"" + $name + "\" Deleted Successfully");
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                            $tr.remove();
                        },
                        error: function (err) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                }
                else
                if (key == "download") {
                    $("#spinner").show();
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    var $downloads = $tr.find(':nth-child(7)').text();
                    $downloads = parseInt($downloads) + 1;
                    $.ajax({
                        type: "POST",
                        url: '/Home/DownloadFile/' + $id,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            $("#spinner").hide();
                            $tr.find(':nth-child(7)').text($downloads);
                            //alert(result.UniqueName + result.FileExt + result.Name);
                            //window.location = "Uploads/" + result.UniqueName + result.FileExt;
                            //$.fileDownload("Uploads/" + result.UniqueName + result.FileExt);
                            $("#downloadanchor").attr({ href: '/Uploads/' + result.UniqueName + result.FileExt, download: result.Name });
                            $("#downloadanchor").text(result.Name);
                            $("#downloadanchor")[0].click();
                            console.log(result);
                        },
                        error: function (err) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("strong").text("Error ! ");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                }
                else
                if (key == "Public") {
                    $("#spinner").show();
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    $.ajax({
                        type: "POST",
                        url: '/Home/GenerateFileToken/' + $id,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            $("#spinner").hide();
                            console.log(result);
                            result = window.location.origin + "/Download/File/" + result;
                            $("#copyclipboard").val(result);
                            $("#copyclipboardbtn").show("slow");
                            var $td = $tr.find(':nth-child(3)');
                            $td.empty();
                            var $font = $('<i>');
                            $font.attr({ class: "fa fa-chain", style: "color: #428BCA" });
                            $td.append($font);
                        },
                        error: function (err) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("strong").text("Error ! ");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                }
                else
                if (key == "Specific") {
                    $("#spinner").show();
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    $.ajax({
                        type: 'POST',
                        dataType: 'JSON',
                        url: '/Home/GetFileUsers/' + $id,
                        contentType: "application/json",
                        processdata: false,
                        success: function (result) {
                            console.log(result);
                            $("#specificshare").slideDown("slow");
                            if (result.length == 0) {
                                $("#shareduserstable").hide();
                                $("#spinner").hide();
                                return;
                            }
                            $("#shareduserstable").show();
                            $("#sharedusers").empty();
                            for (var i = 0; i < result.length; i++) {
                                var $str = $("<tr>");
                                $str.addClass("table-light");
                                var $sth = $("<th>");
                                $sth.text(result[i].Login);
                                $str.append($sth);
                                $sth = $("<th>");
                                var $sa = $("<a>");
                                $sa.attr({ href: '#', id:'removeuseranchor' });
                                var $si = $("<i>");
                                $si.addClass("fa fa-remove");
                                $sa.append($si);
                                $sth.append($sa);
                                $str.append($sth);
                                $("#sharedusers").append($str);
                            } 
                            $("#spinner").hide();
                        },
                        error: function (result) {
                            $("#spinner").hide();
                            console.log(result);
                        }
                    });
                    $("#sfileid").val($id);
                    $("#suserlogin").val("");
                    $("#specificshare").slideDown("slow");
                    $("#sharedusers").empty();
                    $("#specificshare").on('click', '#sharebtn', function () {
                        var $btn = $(this).prop('disabled', true);
                        $btn.closest('div').hide();
                        $("#spinner").show();
                        var $id = $("#sfileid").val();
                        var $login = $("#suserlogin").val();
                        var $data = ({ 'Id': $id, 'Login': $login });

                        $.ajax({
                            type: 'POST',
                            dataType: 'JSON',
                            url: '/Home/GenerateSpecificFileToken',
                            data: JSON.stringify($data),
                            contentType: "application/json",
                            processdata: false,
                            success: function (result) {
                                console.log(result);
                                $("#spinner").hide();
                                $btn.prop('disabled', false);
                                if (result == "CurrentUser") {
                                    var $alert = $("#lg-msg");
                                    $alert.removeClass("alert-success");
                                    $alert.addClass("alert-danger");
                                    $alert.find("strong").text("Oops ! ");
                                    $alert.find("span").text("You Cannot Add Yourself");
                                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                                }
                                else
                                if (result == "UserAlreadyExist") {
                                    var $alert = $("#lg-msg");
                                    $alert.removeClass("alert-success");
                                    $alert.addClass("alert-danger");
                                    $alert.find("strong").text("Oops ! ");
                                    $alert.find("span").text("User Already Added");
                                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                                }
                                else
                                if (result == "UserNotFound") {
                                    var $alert = $("#lg-msg");
                                    $alert.removeClass("alert-success");
                                    $alert.addClass("alert-danger");
                                    $alert.find("strong").text("Oops ! ");
                                    $alert.find("span").text("User Not Found");
                                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                                }
                                else
                                if (result == "FileNotFound") {
                                    var $alert = $("#lg-msg");
                                    $alert.removeClass("alert-success");
                                    $alert.addClass("alert-danger");
                                    $alert.find("strong").text("Oops ! ");
                                    $alert.find("span").text("File Not Found");
                                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                                }
                                else {
                                    var $td = $tr.find(':nth-child(3)');
                                    $td.empty();
                                    var $font = $('<i>');
                                    $font.attr({ class: "fa fa-chain", style: "color: #428BCA" });
                                    $td.append($font);
                                    $font = $('<i>');
                                    $font.attr({ class: "fa fa-user", style: "color: #428BCA" });
                                    $td.append($font);
                                    var $alert = $("#lg-msg");
                                    $alert.removeClass("alert-danger");
                                    $alert.addClass("alert-success");
                                    $alert.find("strong").text("Success ! ");
                                    $alert.find("span").text("A Download Link Is Sent To " + $login);
                                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                                }
                            },
                            error: function (err) {
                                $("#spinner").hide();
                                var $alert = $("#lg-msg");
                                $alert.addClass("alert-danger");
                                $alert.find("strong").text("Error ! ");
                                $alert.find("span").text(err.statusText);
                                $alert.fadeIn("slow").delay(5000).slideUp("slow");
                            }
                        });
                    });
                }
                else
                if (key == "Only Me") {
                    $("#spinner").show();
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    $.ajax({
                        type: "POST",
                        url: '/Home/RemoveFileToken/' + $id,
                        contentType: false,
                        processData: false,
                        success: function (result) { 
                            console.log(result);
                            var $td = $tr.find(':nth-child(3)');
                            $td.empty();
                            $("#spinner").hide();
                        },
                        error: function (err) {
                            $("#spinner").hide();
                            var $alert = $("#lg-msg");
                            $alert.addClass("alert-danger");
                            $alert.find("strong").text("Error ! ");
                            $alert.find("span").text(err.statusText);
                            $alert.fadeIn("slow").delay(5000).slideUp("slow");
                        }
                    });
                }
            },
            items: {
                "rename": { name: "Rename", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" },
                "download": { name: "Download", icon: "fa-download" },
                "share": {
                    "name": "Share",
                    "icon": "fa-share-alt",
                    "items": {
                        "Public": { "name": "Public", "icon": "fa-users" },
                        "Specific": { "name": "Specific", "icon": "fa-user" },
                        "Only Me": { "name": "Only Me", "icon": "fa-lock" }
                    }
                }
            }
        });

        $("#savefile").click(function () {
            $("#spinner").show();
            var $id = $("#mfileid").val();
            var $filename = $("#mfilename").val();
            var $data = ({ 'Id': $id, 'Name': $filename });
            if (!$filename) {
                $("#spinner").hide();
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Oops ! ");
                $alert.find("span").text("Empty File Name");
                $alert.fadeIn("slow").delay(5000).slideUp("slow");
                $("#createfileinput").slideUp();
                return false;
            }
            if (!$id) {
                $("#spinner").hide();
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Oops ! ");
                $alert.find("span").text("File Id Not Supplied");
                $alert.fadeIn("slow").delay(5000).slideUp("slow");
                $("#createfileinput").slideUp();
                return false;
            }

            $.ajax({
                type: 'POST',
                dataType: 'JSON',
                url: '/Home/EditFile',
                data: JSON.stringify($data),
                contentType: "application/json",
                processdata: false,
                success: function (result) {
                    $("#spinner").hide();
                    $("#createfileinput").slideUp();
                    LoadFolders();
                },
                error: function () {
                    alert('Failed');
                }
            });
        });

        $("#savefolderbtnclose").click(function () {
            $("#savefolderinput").slideUp();
        });

        
    });

    function CreateBreadCrumbs($text, $value) {
        var $ol = $(".breadcrumb");
        var $li = $("<li>");
        var $a = $("<a>");
        $a.text($text);
        $li.append($a);
        $li.attr({ value: $value, class: 'breadcrumb-item active' });
        $ol.append($li);
    }

    function ClearTable() {
        var $body = $("#TableBody");
        $body.find('tr').remove();
    }

    function LoadFolders() {
        $("#spinner").show();
        var $id = $("#parentfolderid").val();
        var $data = { 'Id': $id };
        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Home/ListFolders',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                ClearTable();
                console.log(result);
                for (var i = 0; i < result.length; i++) {
                    var tr = $('<tr>');
                    tr.attr({ value: result[i].Id, id: 'folder' });
                    var td = $('<td>');
                    td.text(result[i].Id);
                    td.hide();
                    tr.append(td);
                    td = $('<td>');
                    var div = $('<div>');
                    div.attr({ class: 'icon folder' });
                    td.append(div);
                    var sp = $('<span>');
                    sp.text(result[i].Name)
                    td.append(sp);
                    tr.append(td);
                    td = $('<td>');
                    tr.append(td);
                    td = $('<td>');
                    td.text("--");
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].CreatedOn);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].ViewCount);
                    tr.append(td);
                    var body = $('#TableBody');
                    body.append(tr);
                    BindEvents();
                }
                LoadFiles();
                $("#spinner").hide();
            },
            error: function () {
                alert('Error Occured');
            }
        });
    }

    function LoadFiles() {
        var $id = $("#parentfolderid").val();
        var $data = { 'Id': $id };
        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: 'Home/ListFiles',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                console.log(result);
                for (var i = 0; i < result.length; i++) {
                    var tr = $('<tr>');
                    tr.attr({ value: result[i].Id, id: 'file'  });
                    var td = $('<td>');
                    td.text(result[i].Id);
                    td.hide();
                    tr.append(td);
                    td = $('<td>');
                    var div = $('<div>');
                    div.attr({ class: 'icon file' });
                    td.append(div);
                    var sp = $('<span>');
                    sp.text(result[i].Name + result[i].FileExt)
                    td.append(sp);
                    tr.append(td);
                    //Share Icon
                    td = $('<td>');
                    if (result[i].Token != null) {
                        var font = $('<i>');
                        font.addClass('fa fa-chain');
                        font.attr({ style: 'color: #428BCA' });
                        td.append(font);
                        if (result[i].Share != null) {
                            var font = $('<i>');
                            font.addClass('fa fa-user');
                            font.attr({ style: 'color: #428BCA' });
                            td.append(font);
                        }
                    }
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].FileExt);
                    td.attr({ style: 'display : none;' });
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].FileSizeInKB + " KB");
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].UploadedOn);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].Downloads);
                    tr.append(td);
                    var body = $('#TableBody');
                    body.append(tr);
                }
            },
            error: function () {
                alert('Error Occured');
            }
        });
    }

    function BindEvents() {
        $('#TableBody tr').unbind("click").bind("click", function () {
            var $td1 = $(this).find(':nth-child(1)').text();
            var $td2 = $(this).find('td:nth-child(2)').text();
            $("#parentfolderid").val($td1);
            LoadFolders();
            //LoadFiles();
            var $ol = $(".breadcrumb");
            $(".breadcrumb .breadcrumb-item").removeClass('active');
            var $li = $("<li>");
            var $a = $("<a>");
            $a.text($td2);
            $li.append($a);
            $li.attr({ value: $td1, class: 'breadcrumb-item active' });
            $ol.append($li);
            $(".breadcrumb-item").unbind("click").bind("click", function () {
                $(this).addClass('active');
                $(this).nextAll().remove();
                var $br = $(this).val();
                $("#parentfolderid").val($br);
                LoadFolders();
                //LoadFiles();
            });
            return false;
        });
    }

    $('#FileUploadDummy').click(function () {
        $('#FileUpload').trigger('click');
    });

    $('#FileUpload').change(function () {
        $("#spinner").show();
        if (window.FormData !== undefined) {
            var $parent = $("#parentfolderid").val();
            var fileUpload = $("#FileUpload").get(0);
            var files = fileUpload.files;

            var fileData = new FormData();
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            $.ajax({
                url: '/Home/UploadFiles/' + $parent,
                type: "POST",
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    var $alert = $("#lg-msg");
                    if (result == files.length) {
                        $alert.removeClass("alert-danger");
                        $alert.addClass("alert-success");
                        $alert.find("span").text(result + " File(s) Uploaded Successfully");
                    }
                    else {
                        $alert.removeClass("alert-success");
                        $alert.addClass("alert-danger");
                        $alert.find("span").text(files.length - result + " File(s) Already Exist");
                    }
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                    $("#upload").trigger("reset");
                    ClearTable();
                    LoadFolders();
                    $("#spinner").hide();
                },
                error: function (err) {
                    $("#spinner").hide();
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-danger");
                    $alert.find("strong").text("Error ! ");
                    $alert.find("span").text(err.statusText);
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });

    $("#createfolderbtn").click(function () {
        $("#createfolderinput").slideToggle();
        $("#mfoldername").val("New Folder");
        $("#mfolderid").val("0");
    });

    $("#savefolder").click(function () {
        $("#spinner").show();
        var $id = $("#mfolderid").val();
        var $parentid = $("#parentfolderid").val();
        var $foldername = $("#mfoldername").val();
        if (!$foldername) {
            $("#spinner").hide();
            var $alert = $("#lg-msg");
            $alert.addClass("alert-danger");
            $alert.find("strong").text("Oops ! ");
            $alert.find("span").text("Empty Folder Name");
            $alert.fadeIn("slow").delay(5000).slideUp("slow");
            $("#createfolderinput").slideUp();
            return false;
        }

        var $data = ({ 'Id': $id, 'Name': $foldername, 'ParentFolderId': $parentid });

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Home/CreateFolder',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
                if (result == 0) {
                    var $alert = $("#lg-msg");
                    $alert.addClass("alert-danger");
                    $alert.find("strong").text("Oops ! ");
                    $alert.find("span").text("\"" + $foldername + "\" Already Exist");
                    $alert.fadeIn("slow").delay(5000).slideUp("slow");
                    $("#createfolderinput").slideUp();
                }
                $('#mfoldername').val("");
                LoadFolders();
                $("#createfolderinput").slideUp();
                $("#spinner").hide();
                return false;
            },
            error: function (err) {
                $("#spinner").hide();
                var $alert = $("#lg-msg");
                $alert.addClass("alert-danger");
                $alert.find("strong").text("Error ! ");
                $alert.find("span").text(err.statusText);
                $alert.fadeIn("slow").delay(5000).slideUp("slow");
                return false;
            }
        });
    });

});