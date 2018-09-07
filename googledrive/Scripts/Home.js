﻿$(document).ready(function () {
    LoadFolders();
    CreateBreadCrumbs("Home", 0);

    $(function () {
        $.contextMenu({
            selector: '#folder',
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
                    if (!confirm("Are you sure ! Its Subfolders and files will be deleted")) {
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
                            alert($name + ' Deleted Successfully');
                            $tr.remove();
                        },
                        error: function (err) {
                            alert(err.statusText);
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
            callback: function (key, options) {
                if (key == "rename") {
                    $("#createfileinput").slideDown();
                    var $td1 = $(this).find(':nth-child(1)').text();
                    var $td2 = $(this).find('td:nth-child(2)').text();
                    $("#mfileid").val($td1);
                    $("#mfilename").val($td2);
                }
                else
                if (key == "delete") {
                    var $tr = $(this);
                    var $id = $tr.find(':nth-child(1)').text();
                    var $name = $tr.find('td:nth-child(2)').text();
                    var $ext = $tr.find(':nth-child(3)').text();
                    if (!confirm("Are you sure you want to delete \"" + $name + $ext + "\"")) {
                        return;
                    }

                    $.ajax({
                        url: '/Home/DeleteFile/' + $id,
                        type: "POST",
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            alert($name + ' Deleted Successfully');
                            $tr.remove();
                        },
                        error: function (err) {
                            alert(err.statusText);
                        }
                    });
                }
                else
                if (key == "download") {
                    var $id = $(this).find(':nth-child(1)').text();
                    $.ajax({
                        type: "POST",
                        url: '/Home/DownloadFile/' + $id,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            //alert(result.UniqueName + result.FileExt);
                            window.location = "Uploads/" + result.UniqueName + result.FileExt;
                            //$.fileDownload("Uploads/" + result.UniqueName + result.FileExt);
                        },
                        error: function (err) {
                            alert(err.statusText);
                        }
                    });
                    
                }
            },
            items: {
                "rename": { name: "Rename", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" },
                "download": { name: "Download", icon: "paste" }
            }
        });

        $("#savefile").click(function () {
            var $id = $("#mfileid").val();
            var $filename = $("#mfilename").val();
            var $data = ({ 'Id': $id, 'Name': $filename });

            $.ajax({
                type: 'POST',
                dataType: 'JSON',
                url: '/Home/EditFile',
                data: JSON.stringify($data),
                contentType: "application/json",
                processdata: false,
                success: function (result) {
                    $("#createfileinput").slideUp();
                    LoadFolders();
                },
                error: function () {
                    alert('Failed');
                }
            });
        });

        $("#savefolderbtnclose").click(function () {
            $("#savefolderinput").fadeOut();
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
                    td.text(result[i].ParentFolderId);
                    tr.append(td);
                    td = $('<td>');
                    td.text("--");
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].CreatedBy);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].CreatedOn);
                    tr.append(td);
                    var body = $('#TableBody');
                    body.append(tr);
                    BindEvents();
                }
                LoadFiles();
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
                    sp.text(result[i].Name)
                    td.append(sp);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].FileExt);
                    td.attr({ style: 'display : none;' });
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].ParentFolderId);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].FileSizeInKB);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].CreatedBy);
                    tr.append(td);
                    td = $('<td>');
                    td.text(result[i].UploadedOn);
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
                    alert(result);
                    $("#upload").trigger("reset");
                    ClearTable();
                    LoadFolders();
                },
                error: function (err) {
                    alert(err.statusText);
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
        var $id = $("#mfolderid").val();
        var $parentid = $("#parentfolderid").val();
        var $foldername = $("#mfoldername").val();

        if (!$foldername){
            alert("Error ! Folder Name Can Not Be Empty");
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
                    alert("\"" + $foldername + "\" already exist in the current directory");
                }
                $('#mfoldername').val("");
                LoadFolders();
                $("#createfolderinput").slideUp();
            },
            error: function () {
                alert('Failed');
            }
        });
    });

});