$(document).ready(function () {
    LoadFolders();
    CreateBreadCrumbs("Home", 0);

    function CreateBreadCrumbs($text, $value) {
        var $ol = $(".breadcrumb");
        var $li = $("<li>");
        var $a = $("<a>");
        $a.text($text);
        $li.append($a);
        $li.attr({ value: $value, class: 'breadcrumb-item' });
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
        $('tr').unbind("click").bind("click", function () {
            var $td1 = $(this).find(':nth-child(1)').text();
            var $td2 = $(this).find('td:nth-child(2)').text();
            $("#parentfolderid").val($td1);
            LoadFolders();
            //LoadFiles();
            var $ol = $(".breadcrumb");
            var $li = $("<li>");
            var $a = $("<a>");
            $a.text($td2);
            $li.append($a);
            $li.attr({ value: $td1, class: 'breadcrumb-item' });
            $ol.append($li);
            $(".breadcrumb-item").unbind("click").bind("click", function () {
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
    });

    $("#savefolder").click(function () {
        var $parentid = $("#parentfolderid").val();
        var $foldername = $("#mfoldername").val();
        var $data = { 'ParentFolderId': $parentid, 'Name': $foldername };

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Home/CreateFolder',
            data: JSON.stringify($data),
            contentType: "application/json",
            processdata: false,
            success: function (result) {
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