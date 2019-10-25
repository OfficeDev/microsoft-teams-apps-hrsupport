//$(function () {
$(document).ready(function () {

    // Check if the button text is Edit or Ok then proceed to either form submit or disable textbox of respective section
    // such as teams or knowledgebase
    $(".toggleToEditButton").on('click',function () {
        var currentElement = $(this);
        var currentForm = currentElement.closest(".formToSubmit");
        if (currentElement.val() === "Edit") {
            currentForm.find(".toggleTextBox").removeAttr("disabled");
            currentElement.val("OK");
            currentElement.attr('aria-label', 'Save ' + currentElement.parents('form').find('label.font-bold').text() + '');
        }
        else {
            currentForm.submit();
            currentElement.attr('aria-label', 'Edit ' + currentElement.parents('form').find('label.font-bold').text() + '');
        }
    });

    //Display team Id if its saved if yes then perform edit UI changes
    $.ajax({
        type: "GET",
        url: "/Home/GetSavedTeamIdAsync",
        success: function (result) {
            if (result !== "") {                //Display the default UI when no team Id results are available for display
                $(".teamIdTextBox").val(result);
                toggleToEdit("team");
            }
        },
        error: function (error) {
            saveFailureIconAndText("teamStorageStatusIcon", "teamStorageStatusMessage", "Unable to display team Id due to HTTP: " + error.status + ", " + error.statusText);
        }
    });

    //Display knowledgeBase Id if its saved if yes then perform edit UI changes
    $.ajax({
        type: "GET",
        url: "/Home/GetSavedKnowledgeBaseIdAsync",
        success: function (result) {
            if (result !== "") {                //Display the default UI when no knowledge Id results are available for display
                $(".knowledgeBaseIdTextBox").val(result);
                toggleToEdit("knowledgeBase");
            }
        },
        error: function (error) {
            saveFailureIconAndText("knowledgeBaseStorageStatusIcon", "knowledgeBaseStorageStatusMessage", "Unable to display knowledge base Id due to HTTP: " + error.status + ", " + error.statusText);
        }
    });

    //Display welcome message text if its saved if yes then perform edit UI changes
    $.ajax({
        type: "GET",
        url: "/Home/GetSavedWelcomeMessageAsync",
        success: function (result) {
            if (result !== "") {                //Display the default UI when no welcome message results are available for display
                $(".welcomeMessageIdTextBox").val(result);
                toggleToEdit("welcomeMessage");
            }
        },
        error: function (error) {
            saveFailureIconAndText("welcomeMessageStorageStatusIcon", "welcomeMessageStorageStatusMessage", "Unable to display welcome message due to HTTP: " + error.status + ", " + error.statusText);
        }
    });

});

// Display a green tick when save is success
function onSaveSuccess() {
    var currentFormName = $(this).closest(".formToSubmit").find(".form-group").attr("data-form-name");
    clearStatusIconAndMessage(currentFormName);
    saveSuccessIcon(currentFormName + "StorageStatusIcon");
    toggleToEdit(currentFormName);
}

// Display a red cross with error text message when save is not success due to error
function onSaveFailure(errorText) {
    var currentFormName = $(this).closest(".formToSubmit").find(".form-group").attr("data-form-name");
    clearStatusIconAndMessage(currentFormName);
    saveFailureIconAndText(currentFormName + "StorageStatusIcon", currentFormName + "StorageStatusMessage", errorText.statusText);
}

// Perform changes when Edit related UI has to be displayed
function toggleToEdit(section) {
    $("#" + section + "SubmitButton").val("Edit");
    $("#" + section + "SubmitButton").attr("Edit " + section + "");
    $("." + section + "IdTextBox").attr("disabled", "disabled");
}

// Provide the div whose icon and color needs to be changed on successful save
function saveSuccessIcon(statusIconId) {
    $("#" + statusIconId + "").addClass("glyphicon glyphicon-ok").addClass("storageStatusIcon-success");
    setTimeout(function () {
        $("#" + statusIconId + "").removeClass("glyphicon glyphicon-ok").removeClass("storageStatusIcon-success");
    },3000);
}

// Provide the div whose icon, color and text needs to be changed due to failure while save
function saveFailureIconAndText(statusIconId, statusMessageId, errorText) {
    $("#" + statusIconId + "").addClass("glyphicon glyphicon-remove").addClass("storageStatusIcon-failure");
    $("#" + statusMessageId + "").addClass("error-message alert alert-danger").text(errorText);
}

// Clear icon and message
function clearStatusIconAndMessage(section) {
    $("#" + section + "StorageStatusIcon").removeAttr("class");
    $("#" + section + "StorageStatusMessage").removeAttr("class").text("");
}



// Show spinner indicator on all AJAX calls
$(document).ajaxSend(function () {
    $("#spinner").show();
});

// Hide spinner indicator when all AJAX calls stops
$(document).ajaxStop(function () {
    $("#spinner").hide();
});