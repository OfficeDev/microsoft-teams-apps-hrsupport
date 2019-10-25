var tableRowsCount;
var table;
var imageUri = '';
var imageIndex = 0;
var imageSubStr = '';
var ImageExtIndex = 0;
var imageName = '';
var IsEdit = false;

$(document).ready(function () {
    // To make Text area resizable
    $("#WelcomeMessage").resizable();
    $("#WelcomeMessage").parent('.ui-wrapper').resize(function () {
        var textAreaHeight = $(this).height();
        $(this).children('#WelcomeMessage').css('height', textAreaHeight + 14);
    });

    $(".ddlButton").prop("readonly", true);
    $("#helpTileTable").on("click", "td .deleteConfig", function (e) {
        if (confirm("Are you sure you want to delete this item?")) {
            var rowKey = $(this).closest("tr").attr('id');
            if (rowKey !== null && rowKey !== undefined) {
                deleteData($(this).closest("tr").attr('id').split('$')[0]);
            }
            table.row($(this).closest("tr")).remove().draw();
            loadData();
            console.log("Configurator : Row Deleted");
            appInsights.trackEvent("Configurator : Row Deleted");
        }
        return false;
    });

    $('#helpTileTable tbody tr').each(function () {
        $(this).find('.redirectUri').find('a').attr('aria-label', 'Go to ' + $(this).find('td:first').text() + '');
        $(this).find('.redirectUri').find('a').attr('title', $(this).find('.redirectUri').find('a').text());
    });

    $("#helpTileTable").on('click.edit', ".editConfig", function (e) {
        checkRowCountValidation(true);
        IsEdit = true;
        $(this).removeClass("editConfig").addClass("saveConfig");
        $(this).text("Save");
        $(this).attr('aria-label', 'save card ' + $(this).parent('td').siblings('td:first').text());
        $(this).attr('title', 'Save');
        var $row = $(this).closest("tr").off("mousedown");
        var $tds = $row.find("td").not(':last');
        var counter = 0;
        $.each($tds, function (i, el) {
            var txt = $(this).text();
            if (txt !== null || txt !== undefined) {
                txt = txt.trim();
            }
            if (counter === 0) {
                $(this).html("").append("<input type='text' maxlength='50' placeholder='Max 50 characters' value=\"" + txt + "\">");
            }
            else if (counter === 1) {
                $(this).html("").append("<textarea type='text' maxlength='200' placeholder='Max 200 characters'>" + txt + "</textarea>");
            }
            else if (counter === 3) // Image DDL Logic.
            {
                $(this).html($(this).html());
                $(this).find(".ddlButton").removeAttr("disabled");
            }
            else if (counter === 4) {
                $(this).html("").append("<input type='text' title='Comma separated tags without space' placeholder='Comma separated tags without space' value=\"" + txt + "\">");
            }
            else {
                $(this).html("").append("<input type='text' value=\"" + txt + "\">");
            }

            counter++;
        });
    });

    $("#helpTileTable").on('click', "input", function (e) {
        e.stopPropagation();
    });

    $("#helpTileTable").on('click.save', ".saveConfig", function (e) {
        checkRowCountValidation(true);
        var $row = $(this).closest("tr");
        var $tds = $row.find("td").not(':last');

        var data = [];
        $.each($tds, function (i, el) {
            var txt = $(this).children().first().val();
            $(this).html(txt);
            data.push(txt);
        });
        var userfulLinksObject = {
            Title: data[0],
            Description: data[1],
            RedirectUrl: data[2],
            ImageUrl: data[3],
            Tags: data[4].toLowerCase().trim(),
            RowKey: $row.attr('id') !== undefined ? $row.attr('id').split('$')[0] : $row.attr('id'),
            IsEdit: IsEdit,
            TileOrder: $row.attr('id') !== undefined ? $row.attr('id').split('$')[1] : $row.attr('id')
        };
        var spaceCheck = 0;
        if (data[4].length <= 200) {
            var res = data[4].trim().split(",");
            if (res.length <= 5) {
                $.each(res, function (i, val) {
                    if (val !== '' && val.length <= 20) {
                        spaceCheck = val.trim().indexOf(' ');
                        if (spaceCheck >= 0) {
                            return false;
                        }
                    }
                    else {
                        spaceCheck = 0;
                        return false;
                    }
                });
            }
        }

        if ((data[0] !== null && data[0] !== '' && data[0] !== 'undefined') &&
            (data[1] !== null && data[1] !== '' && data[1] !== 'undefined') &&
            (data[2] !== null && data[2] !== '' && data[2] !== 'undefined') &&
            (data[3] !== null && data[3] !== '' && data[3] !== 'undefined') &&
            ((data[4] !== null && data[4] !== '' && data[4] !== 'undefined') && spaceCheck === -1)
        ) {
            saveData(userfulLinksObject, $row);
            loadData();
            $(this).removeClass("saveConfig").addClass("editConfig");
            $(this).text("Edit");
            $(this).attr('aria-label', 'Edit card ' + $(this).parent('td').siblings('td:first').text());
            $(this).attr('title', 'Edit');
            var counter = 0;
            $.each($tds, function (i, el) {
                var txt = $(this).text();
                if (txt !== null || txt !== undefined) {
                    txt = txt.trim();
                }
                if (counter === 1) {
                    $(this).html("").text(userfulLinksObject.Description);
                }
                if (counter === 2) {
                    $(this).html("").append("<a title='" + userfulLinksObject.RedirectUrl + "' href='" + userfulLinksObject.RedirectUrl + "'>" + userfulLinksObject.RedirectUrl + "</a>")
                }
                if (counter === 3) // Image DDL Logic.
                {
                    var imageurl = $(this).html();
                    imageUri = imageurl;
                    imageIndex = imageUri.lastIndexOf('/');
                    imageSubStr = imageUri.substring(imageIndex + 1);
                    ImageExtIndex = imageSubStr.lastIndexOf('.');
                    imageName = imageSubStr.substring(0, ImageExtIndex);
                    if (imageName.length > 10) {
                        imageName = imageName.substring(0, 9);
                    }
                    else {
                        imageName = imageName;
                    }
                    $(this).html($("#newRow tbody tr").find('td.imageDDL').html());
                    $(this).find(".ddlButton").attr('disabled', 'disabled');
                    $(this).find("#imageURLData").val(imageurl);
                    $(this).find("#selectedImageName").text(imageName);
                    $(this).find("#selectedImageURL").attr("currentSrc", imageurl);
                    $(this).find("#selectedImageURL").attr("src", imageurl);
                }
                counter++;
            });
            IsEdit = false;
            console.log("Configurator : Row got Saved");
            appInsights.trackEvent("Configurator : Row got Saved");
        }
        else {
            $(this).removeClass("editConfig").addClass("saveConfig");
            $(this).text("Save");
            var index = 0;
            $.each($tds, function (i, el) {
                if (index === 0) {
                    $(this).html("").append("<input type='text' maxlength='50' placeholder='Max 50 characters' value=\"\">");
                }
                else if (index === 1) {
                    $(this).html("").append("<textarea type='text' maxlength='200' placeholder='Max 200 characters'></textarea>");
                }
                else if (index === 3) // Image DDL Logic.
                {
                    var imageurl = $(this).html();
                    if (imageurl === '') {
                        imageurl = $("#selectedImageURL").attr('src');
                        imageName = "Select Image";
                    }
                    imageUri = imageurl;
                    imageIndex = imageUri.lastIndexOf('/');
                    imageSubStr = imageUri.substring(imageIndex + 1);
                    ImageExtIndex = imageSubStr.lastIndexOf('.');
                    if (imageName !== 'Select Image') {
                        imageName = imageSubStr.substring(0, ImageExtIndex);
                    }
                    if (imageName.length > 10 && imageName !== 'Select Image') {
                        imageName = imageName.substring(0, 9);
                    }
                    else {
                        imageName = imageName;
                    }
                    $(this).html($("#newRow tbody tr").find('td.imageDDL').html());
                    $("#imageURLData").val(imageurl);
                    $("#selectedImageName").text(imageName);
                    $("#selectedImageURL").attr("currentSrc", imageurl);
                    $("#selectedImageURL").attr("src", imageurl);
                    $("#selectedImageURL").attr("alt", imageName);
                }
                else if (index === 4) {
                    $(this).html("").append("<input type='text' title='Comma separated tags without space' placeholder='Comma separated tags without space' value=\"\">");
                }
                else {
                    $(this).html("").append("<input type='text' value=\"\">");
                }

                $(this).children().first().val(data[index]);
                if (data[index] === '' || (index === 4 && spaceCheck >= 0)) {
                    if (index <= 4 || (index === 4 && spaceCheck >= 0))
                        $(this).children().first().addClass('mandatory');
                }
                console.error("Configurator : Error While saving the data");
                appInsights.trackEvent("Configurator : Error While saving the data");
                index++;

            });
            e.stopPropagation();
        }
    });


    $("#helpTileTable").on('click', "#selectbasic", function (e) {
        e.stopPropagation();
    });

    loadData();
    // add row
    $('#addRow').on('click', function () {
        IsEdit = false;
        loadData();
        var rowHtml = $("#newRow").find("tr")[0].outerHTML;
        table.row.add($(rowHtml)).draw(false);
        checkRowCountValidation(true);
    });

    function saveData(userfulLinksObject, tr) {
        $.ajax({
            url: "/Home/SaveDetailsAsync",
            method: "POST",
            async: false,
            data: userfulLinksObject
        }).promise().then(function (data) {
            if (data.statusCode === 200) {
                tr.attr('id', data.rowKey + "$" + data.tileOrder);
            }
            else {
                alert('There is some issue while saving the data');
                appInsights.trackEvent("Configurator : Save Data Error, Status Code : " + data.statusCode + "");
            }
        }, function (err) {
            console.error("Configurator : " + err + "");
            appInsights.trackEvent("Configurator : " + err + "");
        });
    }

    function deleteData(rowKey) {
        $.ajax({
            url: "/Home/DeleteUsefulLinksRecord?rowKey=" + rowKey,
            method: "GET",
            async: false
        }).promise().then(function (data) {
            if (data) {
                console.log(data);
            }
            else {
                alert('There Is some error in deleting the record');
                appInsights.trackEvent("delete operation failed");
            }
        }, function (err) {
            console.error("Configurator : " + err + "");
            appInsights.trackEvent("Configurator : " + err + "");
        });
    }

    checkRowCountValidation();

    $("#helpTileTable thead tr th").each(function () {
        if ($(this).hasClass("sorting_asc")) {
            $(this).removeClass("sorting_asc");
        }
    });
});

function loadData() {
    if (table !== undefined) {
        table.destroy();
    }

    table = $('#helpTileTable').DataTable({
        searching: false,
        paging: false,
        info: false,
        order: false,
        "columnDefs": [
            {
                "targets": 0,
                "orderable": false
            },
            {
                "targets": 1,
                "orderable": false
            },
            {
                "targets": 2,
                "orderable": false
            },
            {
                "targets": 3,
                "orderable": false
            },
            {
                "targets": 4,
                "orderable": false
            },
            {
                "targets": 5,
                "orderable": false
            }
        ]
    });

    checkRowCountValidation();
}

// Grid max row validaiton code.
function checkRowCountValidation(isEditOrSave) {
    if ($('#helpTileTable tr').length > 8) {
        // $("#addRow").css("display", "none");
        $("#addRow").attr('disabled', true);
        $("#addRow").attr('title', 'Can not add more than 8 rows.');
        console.log("Configurator : Cannot add more than 8 rows.");
        appInsights.trackEvent("Configurator : Cannot add more than 8 rows.");
    }
    else if (isEditOrSave) {
        // $("#addRow").css("display", "none");
        $("#addRow").attr('disabled', true);
    }
    else {
        $("#addRow").attr('disabled', false);
        // $("#addRow").css("display", "block");
        console.log("Configurator : Row added");
        appInsights.trackEvent("Configurator : Row added");
    }
}

function ImageClickData(e, id) {
    var imageURL = $(e).find("img").attr('src');
    if (id === null) {
        $(e).parents('ul').siblings('button.ddlButton').find('img').attr('src', imageURL);
        $(e).parents('ul').siblings('button.ddlButton').find('img').attr('currentSrc', imageURL);
        $(e).parents('ul').siblings('button.ddlButton').find('span:first').text(e.innerText);
        $(e).parents('ul').siblings('button.ddlButton').parent('div').siblings('input').val(imageURL);
    }
    else {
        $(e).parents('ul').siblings('button.ddlButton').find('img').attr('src', imageURL);
        $(e).parents('ul').siblings('button.ddlButton').find('img').attr('currentSrc', imageURL);
        $(e).parents('ul').siblings('button.ddlButton').find('span:first').text(e.innerText);
        $(e).parents('ul').siblings('button.ddlButton').parent('div').siblings('input').val(imageURL);
    }
}