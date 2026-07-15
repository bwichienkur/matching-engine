

var fe_sp_schoolPickerCarouselMessageType = {
    limitReached: "LIMIT_REACHED",
    default: "DEFAULT"
};

var fe_sp_schoolPickerCarouselId = "#school-picker-carousel";
var fe_sp_schoolPickerCarouselIndicatorId = "#school-picker-carousel-indicators";

function fe_sp_createSchoolPicker(response) {
    response = response || {};

    fe_sp_destroyExistingSchoolPicker();
    
    if (response.components && response.components.length > 0) {
        FormsEngine.SchoolPickerCarouselCount = response.components.length;
        fe_sp_setSchoolPickerCarouselMessages(response.metaDataMessages);
        fe_sp_addComponentsToSchoolPickerCarousel(response.components);
        fe_sp_setSchoolPickerCarouselMessage(fe_sp_schoolPickerCarouselMessageType.default);
        fe_sp_bindSchoolPickerCarouselEventHandlers();
        fe_sp_addCarouselArrows(fe_sp_schoolPickerCarouselId, fe_sp_schoolPickerCarouselIndicatorId);
        fe_sp_selectInitialSlide(fe_sp_schoolPickerCarouselId, fe_sp_schoolPickerCarouselIndicatorId);
        fe_sp_enableSubmitButton();
        fe_sp_removeLoadingSpinner();
        FormsEngine.SchoolPickerCarouselLoaded = true;
    } else {
        fe_sp_goToNoMatchPage();
        FormsEngine.SchoolPickerCarouselLoaded = false;
        FormsEngine.SchoolPickerCarouselCount = 0;
    }

    FormsEngine.MatchResponseGuid = response.matchResponseGuid;
}

function fe_sp_goToNoMatchPage() {
    if (FormsEngine.LoadWorkflowStep) {
        FormsEngine.LoadWorkflowStep('NOMATCH', '');
    }
}

function fe_sp_setSchoolPickerCarouselMessages(messages) {
    FormsEngine.SchoolPickerCarouselMessages = fe_sp_replacePlaceholdersInMessages(messages);
}

function fe_sp_replacePlaceholdersInMessages(messages) {
    var formattedMessages = {};

    for (var key in messages) {
        var message = messages[key];
        formattedMessages[key] = fe_sp_replacePlaceholdersInMessage(message);
    }

    return formattedMessages;
}

function fe_sp_replacePlaceholdersInMessage(message) {
    message = message.replace(/{maxsubmissioncount}/g, fe_sp_convertNumberToWord(fe_se_getMaxSchoolPickerSelectionCount()));
    return message;
}

function fe_se_getMaxSchoolPickerSelectionCount() {
    var maxSubmissionCount = fe_sp_getMaxSubmissionCount();
    var schoolPickerCarouselCount = FormsEngine.SchoolPickerCarouselCount || 0;

    return schoolPickerCarouselCount >= maxSubmissionCount ? maxSubmissionCount : schoolPickerCarouselCount;
}

function fe_sp_destroyExistingSchoolPicker() {
    FormsEngine.SchoolPickerSelections = {};
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel").html("");
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel-indicators").html("");
}

function fe_sp_addComponentsToSchoolPickerCarousel(components) {
    for (var i = 0; i < components.length; i++) {
        var component = components[i];

        jQuery(FormsEngine.DefaultFormTag + " " + fe_sp_schoolPickerCarouselId).append(component);

        var slideNumber = i + 1;
        fe_sp_addCarouselDotIndicator(slideNumber, fe_sp_schoolPickerCarouselId, fe_sp_schoolPickerCarouselIndicatorId);
    }
}

function fe_sp_bindSchoolPickerCarouselEventHandlers() {
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel .institution-carousel-slide").on("change", function (event) {
        fe_sp_institutionSchoolPickerComponentChange(jQuery(event.target));
    });

    jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel .institution-carousel-slide button").on("click", function (event) {
        fe_sp_selectionButtonClicked(jQuery(event.target));
    });
}

function fe_sp_selectionButtonClicked(button) {
    var isSelected = button.attr("data-selected");

    if (isSelected != null) {
        fe_sp_unselectButton(button);
    } else {
        fe_sp_selectButton(button);
    }

    fe_sp_institutionSchoolPickerComponentChange(button);
}

function fe_sp_unselectButton(button) {
    if (button) {
        button.removeClass("btn-primary");
        button.addClass('btn-default');
        button.removeAttr("data-selected");
        button.text("Click if Interested");
    }
}

function fe_sp_selectButton(button) {
    if (button) {
        button.removeClass('btn-default');
        button.addClass("btn-primary");
        button.attr("data-selected", "");
        button.text("Yes, I'm Interested");
    }
}

function fe_sp_addCarouselDotIndicator(slideNumber, carouselId, dotContainerId) {

    var slideIndicatorDot = jQuery("<span class='dot' data-slidenumber='" + slideNumber + "'></span>");

    slideIndicatorDot.on("click", function () {
        fe_sp_moveToSlide(slideNumber, carouselId, dotContainerId);
    });

    jQuery(FormsEngine.DefaultFormTag + " " + dotContainerId).append(slideIndicatorDot);
}

function fe_sp_addCarouselArrows(carouselId, dotContainerId) {
    fe_sp_addPrevArrow(carouselId, dotContainerId);
    fe_sp_addNextArrow(carouselId, dotContainerId);
}

function fe_sp_addPrevArrow(carouselId, dotContainerId) {
    var prevArrow = jQuery("<a class='prev'>&#10094;</a>");

    prevArrow.on("click", function () {
        fe_sp_moveToPrevSlide(carouselId, dotContainerId);
    });

    jQuery(FormsEngine.DefaultFormTag + " " + carouselId).append(prevArrow);
}

function fe_sp_addNextArrow(carouselId, dotContainerId) {
    var nextArrow = jQuery("<a class='next'>&#10095;</a>");

    nextArrow.on("click", function () {
        fe_sp_moveToNextSlide(carouselId, dotContainerId);
    });

    jQuery(FormsEngine.DefaultFormTag + " " + carouselId).append(nextArrow);
}

function fe_sp_selectInitialSlide(carouselId, dotContainerId) {

    if (FormsEngine.SelectFirstSchoolInCarousel) {
        jQuery(FormsEngine.DefaultFormTag).find('.school-picker-selection-btn').first().trigger("click");
    }

    fe_sp_showSlides(1, carouselId, dotContainerId);
}

function fe_sp_institutionSchoolPickerComponentChange(child) {
    var carouselSlide = fe_sp_getParentSlide(child);

    if (!carouselSlide) {
        return;
    }

    try {
        var institutionId = child.attr("data-institutionid");
        var elementTagName = child.get(0).nodeName;

        if (institutionId && elementTagName) {
            fe_sp_updateSchoolPickerSelections(institutionId, carouselSlide, elementTagName);
        }
    } catch (e) {
        fe_consolelog(e);
    }
}

function fe_sp_getParentSlide(child) {
    return jQuery(child.parents(".institution-carousel-slide")[0]);
}

function fe_sp_updateSchoolPickerSelections(institutionId, carouselSlide, initiatingElementTagName) {

    var option = carouselSlide.find("select[data-institutionid='" + institutionId + "'] option:selected").first();
    var button = carouselSlide.find("button[data-institutionid='" + institutionId + "']").first();

    if (option && button && initiatingElementTagName) {
        FormsEngine.SchoolPickerSelections = FormsEngine.SchoolPickerSelections || {};

        var buttonIsSelected = jQuery(button).attr("data-selected") != null;
        var initiatingElementWasTheDropdown = initiatingElementTagName.toLowerCase() === "select";

        if (buttonIsSelected || initiatingElementWasTheDropdown) {
            fe_sp_upsertSchoolPickerSelection(institutionId, option, button);
        } else if (!buttonIsSelected && institutionId) {
            fe_sp_deleteSchoolPickerSelection(institutionId);
        }

        fe_sp_toggleSchoolPickerCarouselControlsOnMaxSubmissionLimit();
        fe_sp_enableSubmitButton();
        fe_sp_setSmartMatchSchoolNamesFromSchoolPickerSelections();
    }
}

function fe_sp_setSmartMatchSchoolNamesFromSchoolPickerSelections() {

    FormsEngine.SmartMatchSchoolNames = "";

    for (var key in FormsEngine.SchoolPickerSelections) {
        var selection = FormsEngine.SchoolPickerSelections[key];

        if (selection) {
            FormsEngine.SmartMatchSchoolNames += selection.institutionName + ", ";
        }
    }
}

function fe_sp_upsertSchoolPickerSelection(institutionId, option, button) {

    institutionId = Number(institutionId);
    var institutionName = jQuery(option).attr("data-institutionname");
    var programId = Number(jQuery(option).attr("data-programid"));
    var programProductId = Number(jQuery(option).attr("data-programproductid"));
    var programTemplateId = Number(jQuery(option).attr("data-programtemplateid"));

    var allValuesExist = programId && programProductId && programTemplateId && institutionId;

    if (allValuesExist) {
        FormsEngine.SchoolPickerSelections[institutionId] = {
            institutionId: institutionId,
            institutionName: institutionName,
            programId: programId,
            programProductId: programProductId,
            programTemplateId: programTemplateId
        };
    }

    fe_sp_selectButton(button);
}

function fe_sp_deleteSchoolPickerSelection(institutionId) {
    delete FormsEngine.SchoolPickerSelections[institutionId];
}

function fe_sp_toggleSchoolPickerCarouselControlsOnMaxSubmissionLimit() {
    var unselectedButtons = fe_sp_getUnselectedButtons();
    var unselectedDropdowns = fe_sp_getUnselectedDropdowns(unselectedButtons);

    if (fe_sp_maxSubmissionLimitReached()) {
        fe_sp_disableInputElements(unselectedButtons, unselectedDropdowns);
        fe_sp_setSchoolPickerCarouselMessage(fe_sp_schoolPickerCarouselMessageType.limitReached);
    } else {
        fe_sp_enableInputElements(unselectedButtons, unselectedDropdowns);
        fe_sp_setSchoolPickerCarouselMessage(fe_sp_schoolPickerCarouselMessageType.default);
    }
}

function fe_sp_getUnselectedButtons() {
    return jQuery(FormsEngine.DefaultFormTag + " .school-picker-selection-btn:not([data-selected])");
}

function fe_sp_getUnselectedDropdowns(unselectedButtons) {
    var unselectedDropdownDomElements = [];

    if (unselectedButtons) {
        for (var i = 0; i < unselectedButtons.length; i++) {
            var unselectedButton = unselectedButtons[i];
            var institutionId = jQuery(unselectedButton).attr("data-institutionid");
            var unselectedDropdown = jQuery(FormsEngine.DefaultFormTag + " .school-picker-selection-dropdown[data-institutionid='" + institutionId + "']");

            if (unselectedDropdown) {
                var unselectedDropdownDomElement = unselectedDropdown.get(0);
                unselectedDropdownDomElements.push(unselectedDropdownDomElement);
            }
        }
    }

    return jQuery(unselectedDropdownDomElements);
}


function fe_sp_disableInputElements() {
    for (var i = 0; i < arguments.length; i++) {
        var elements = arguments[i];
        if (elements) {
            elements.attr("disabled", "");
        }
    }
}

function fe_sp_enableInputElements() {
    for (var i = 0; i < arguments.length; i++) {
        var elements = arguments[i];
        if (elements) {
            elements.removeAttr("disabled");
        }
    }
}

function fe_sp_maxSubmissionLimitReached() {
    var limit = fe_sp_getMaxSubmissionCount();
    var numberOfSelections = fe_sp_getSchoolPickerSelectionCount();

    return numberOfSelections >= limit;
}

function fe_sp_toggleSubmitButtonOnStepMovements(stepNumber) {
    if (stepNumber) {
        var schoolPickerCarouselCount = jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find("#school-picker-carousel").length;

        if (schoolPickerCarouselCount < 1) {
            fe_sp_enableSubmitButton();
        } else {
            fe_sp_toggleSubmitButton();
        }
    }
}

function fe_sp_toggleSubmitButton() {
    var numberOfSelections = fe_sp_getSchoolPickerSelectionCount();

    if (numberOfSelections > 0) {
        fe_sp_enableSubmitButton();
    } else {
        fe_sp_disableSubmitButton();
    }
}

function fe_sp_getSchoolPickerSelectionCount() {
    var selections = FormsEngine.SchoolPickerSelections || {};
    return Object.keys(selections).length;
}

function fe_sp_disableSubmitButton() {
    var overlayClass = "wizard-form-disabled-submit-button-overlay";
    var buttonAlreadyDisabled = jQuery(FormsEngine.SubmitButton).find("." + overlayClass).length > 0;

    if (!buttonAlreadyDisabled) {
        var overlayElement = jQuery('<div></div>');
        overlayElement.addClass(overlayClass);
        jQuery(FormsEngine.SubmitButton).prepend(overlayElement);

        overlayElement.on("click", function (event) {
            event.stopPropagation();
        });
    }
}

function fe_sp_enableSubmitButton() {
    jQuery(FormsEngine.SubmitButton).find(".wizard-form-disabled-submit-button-overlay").remove();
}

function fe_sp_moveToSlide(index, carouselId, dotContainerId) {
    fe_sp_showSlides(index, carouselId, dotContainerId);
}

// Next/previous controls
function fe_sp_moveToPrevSlide(carouselId, dotContainerId) {
    fe_sp_moveSlides(-1, carouselId, dotContainerId);
}

function fe_sp_moveToNextSlide(carouselId, dotContainerId) {
    fe_sp_moveSlides(1, carouselId, dotContainerId);
}

function fe_sp_moveSlides(slidesToMove, carouselId, dotContainerId) {
    var slideNumber = fe_sp_getCurrentSlideNumber(dotContainerId);
    fe_sp_showSlides(slideNumber + slidesToMove, carouselId, dotContainerId);
}

function fe_sp_getCurrentSlideNumber(dotContainerId) {
    var slideNumber = jQuery(FormsEngine.DefaultFormTag + " " + dotContainerId + " .active").data("slidenumber");
    return Number(slideNumber) || 1;
}

function fe_sp_showSlides(slideIndex, carouselId, dotContainerId) {
    try {
        var i;
        var slides = jQuery(FormsEngine.DefaultFormTag + " " + carouselId).find(".mySlides");
        var dots = jQuery(FormsEngine.DefaultFormTag + " " + dotContainerId).find(".dot");

        if (slideIndex > slides.length) {
            slideIndex = 1;
        }

        if (slideIndex < 1) {
            slideIndex = slides.length;
        }

        for (i = 0; i < slides.length; i++) {
            jQuery(slides[i]).css("display","none");
        }

        for (i = 0; i < dots.length; i++) {
            jQuery(dots[i]).removeClass("active");
        }

        jQuery(slides[slideIndex - 1]).css("display", "block");
        jQuery(dots[slideIndex - 1]).addClass("active");
    } catch (e) {
        fe_consolelog(e);
    }

}

function fe_sp_getUniqueProgramTemplateIdsFromSchoolPickerSelections() {
    var programTemplateIds = [];

    var uniqueProgramTemplateIds = {};

    for (var key in FormsEngine.SchoolPickerSelections) {
        var schoolPickerSelection = FormsEngine.SchoolPickerSelections[key];

        var programTemplateId = schoolPickerSelection.programTemplateId;

        if (programTemplateId && uniqueProgramTemplateIds[programTemplateId] == null) {
            uniqueProgramTemplateIds[programTemplateId] = programTemplateId;
            programTemplateIds.push(programTemplateId);
        }
    }

    return programTemplateIds;
}

function fe_sp_initializeMatchReplacementEventListeners(stepNumber, direction) {
    if (stepNumber == FormsEngine.StepDynamicQuestions) {
        fe_sp_bindChangeEventsToAdditionalQuestions();

        if (direction > 0) {
            fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete(false);
        }
    }
}

function fe_sp_bindChangeEventsToAdditionalQuestions() {
    var inputFields = jQuery(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.StepDynamicQuestions).find(":input");

    for (var i = 0; i < inputFields.length; i++) {
        var inputField = jQuery(inputFields[i]);
        fe_sp_bindSubmitButtonShouldBeShownOrNotEventToAdditionalQuestion(inputField);
        fe_sp_bindFailedMatchReplacementChangeEventToAdditionalQuestion(inputField);
    }
}

function fe_sp_bindSubmitButtonShouldBeShownOrNotEventToAdditionalQuestion(field) {
    if (field) {
        field.unbind("change", fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown);
        field.on("change", fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown);
    }
}

function fe_sp_bindFailedMatchReplacementChangeEventToAdditionalQuestion(field) {
    if (field) {
        field.unbind("change", fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete);
        field.on("change", fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete);
    }
}

function fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete(moveForward) {
    moveForward = typeof moveForward === "boolean" ? moveForward : true;

    if (fe_sp_validateStepWithoutErrorMessages(FormsEngine.StepDynamicQuestions)) {
        fe_sp_loadFailedMatchReplacements();

        if (!FormsEngine.DontAllowAutoForwardStep && moveForward && !FormsEngine.FormsHasBeenRecovered) {
            jQuery(FormsEngine.SubmitButton).trigger("click");
        }
    }
}

function fe_sp_loadFailedMatchReplacements() {
    fe_sp_addPlaceholderInputToMatchReplacementContainer();
    fe_sp_addLoadingSpinnerToComponent(jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures"));
    fe_sp_removeReplacementMatchesFromSchoolPickerSelections();
    fe_sp_makeGetFailedMatchReplacementsRequest();
}

function fe_sp_removeReplacementMatchesFromSchoolPickerSelections() {
    for (var key in FormsEngine.SchoolPickerSelections) {
        var selection = FormsEngine.SchoolPickerSelections[key];
        if (selection.isReplacementMatch) {
            fe_sp_deleteSchoolPickerSelection(selection.institutionId);
        }
    }
}

function fe_sp_makeGetFailedMatchReplacementsRequest() {
    var formData = fe_getFormData();

    //Required arguments
    var filters = "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
    filters += "&ApplicationId=" + FormsEngine.ApplicationId;
    filters += "&LeadData=" + encodeURIComponent(formData.LeadData);
    filters += "&AdditionalData=" + encodeURIComponent(formData.LeadAdditionalData);

    var sUrl = FormsEngine.ServiceBaseURL + "/SchoolPickerWizard/GetFailedMatchReplacements";
    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        data: filters,
        cache: false,
        url: sUrl,
        success: function (failedMatchReplacements) {
            fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
        }
    });
}

function fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements) {

    console.log(failedMatchReplacements);

    fe_sp_moveStepIfThereAreNoReplacementMatches(failedMatchReplacements.ReplacementMatches);
    fe_sp_addNewMatchesToSchoolPickerSelections(failedMatchReplacements.ReplacementMatches);
    fe_sp_addFailedMatchReplacementComponentsToDom(failedMatchReplacements);
    fe_sp_removeLoadingSpinner();
}

function fe_sp_moveStepIfThereAreNoReplacementMatches(replacementMatches) {
    var currentStepContainsMatchReplacementContainer = jQuery(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep).find("#school-picker-failures").length > 0;

    if (replacementMatches.length < 1 && currentStepContainsMatchReplacementContainer) {
        jQuery(FormsEngine.SubmitButton).trigger("click");
    }
}

function fe_sp_addNewMatchesToSchoolPickerSelections(replacementMatches) {
    for (var i = 0; i < replacementMatches.length; i++) {
        var replacementMatch = replacementMatches[i];

        FormsEngine.SchoolPickerSelections[replacementMatch.InstitutionId] = {
            institutionId: replacementMatch.InstitutionId,
            institutionName: replacementMatch.InstitutionName,
            programId: replacementMatch.ProgramId,
            programProductId: replacementMatch.ProgramProductId,
            programTemplateId: replacementMatch.ProgramTemplateId,
            isReplacementMatch: true
        };
    }

    fe_sp_setSmartMatchSchoolNamesFromSchoolPickerSelections();
}

function fe_sp_addFailedMatchReplacementComponentsToDom(failedMatchReplacements) {
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").html("");

    if (failedMatchReplacements.FailedMatches.length > 0) {
        var failedMatchMessage = jQuery("<h3>" + failedMatchReplacements.Message + "</h3>");
        jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").append(failedMatchMessage);
    }

    var replacementHtmlComponents = failedMatchReplacements.ReplacementHtmlComponents;
    for (var i = 0; i < replacementHtmlComponents.length; i++) {
        var replacementHtmlComponent = replacementHtmlComponents[i];
        jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").append(replacementHtmlComponent);
    }

    fe_sp_bindFailedMatchReplacementClickHandlers();
}

function fe_sp_bindFailedMatchReplacementClickHandlers() {
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures button").on("click", function (event) {
        var button = jQuery(event.target);
        fe_sp_matchReplacementButtonClicked(button);
    });
}

function fe_sp_matchReplacementButtonClicked(button) {
    var buttonParent = button.parent();
    var institutionId = buttonParent.attr("data-institutionid");

    if (button.hasClass("yes-btn")) {
        fe_sp_swapMatchReplacementButtonClasses(button, "no-btn");
        fe_sp_upsertSchoolPickerSelection(institutionId, buttonParent);
        FormsEngine.SchoolPickerSelections[institutionId].isReplacementMatch = true;
    } else if (button.hasClass("no-btn")) {
        fe_sp_swapMatchReplacementButtonClasses(button, "yes-btn");
        fe_sp_deleteSchoolPickerSelection(institutionId);
    }
}

function fe_sp_swapMatchReplacementButtonClasses(selectedButton, unselectedButtonClass) {
    var unselectedButton = selectedButton.siblings("." + unselectedButtonClass).first();

    if (unselectedButton) {
        fe_sp_addUnselectedClass(unselectedButton);
    }

    fe_sp_addSelectedClass(selectedButton);
}

function fe_sp_addUnselectedClass(button) {
    button.removeClass("btn-primary");
    button.addClass("btn-default");
}

function fe_sp_addSelectedClass(button) {
    button.removeClass("btn-default");
    button.addClass("btn-primary");
}

function fe_sp_validateStepWithoutErrorMessages(stepNumber) {

    if (!stepNumber) {
        return false;
    }

    var stepIsValid = true;
    var stepElement = jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber);
    
    //find all input tags except hidden, with the exception of hiddens used for categories and subcategories
    var fieldsToValidate = stepElement.find(":input[required='required']").not("[type=hidden]:not([code*='Categories']):not([code='Specialties'])").not("[novalidate='true']");

    for (var i = 0; i < fieldsToValidate.length; i++) {
        var fieldToValidate = jQuery(fieldsToValidate[i]);
        var controlCode = fieldToValidate.attr('code');

        if ((controlCode && fieldToValidate.visible())
            || controlCode == 'Categories' || controlCode == 'SubCategories' || controlCode == 'Specialties' || controlCode == 'Phone' || controlCode == 'Alternate_Phone'
        ) {
            var controlIsValid;

            var type = fieldToValidate.attr("type");
            if (type === "checkbox" || type === "radio") {
                var checkedCount = fieldsToValidate.filter(":input[code='" + controlCode + "']:checked").length;
                controlIsValid = checkedCount > 0;
            } else {
                controlIsValid = Boolean(fieldToValidate.val());
            }

            stepIsValid = stepIsValid && controlIsValid;
        }
    }

    return stepIsValid;
}

function fe_sp_addPlaceholderInputToMatchReplacementContainer() {
    var placeHolderInput = jQuery('<input type="text" name="school-picker-failures-placeholder" value="" disabled />');
    placeHolderInput.css("opacity", "0");
    jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").append(placeHolderInput);
}

function fe_sp_addLoadingSpinnerToComponent(component) {

    if (!component) {
        return;
    }

    var loaderIsntAlreadyAddedToDom = jQuery(FormsEngine.DefaultFormTag).find("#component-loader").length < 1;
    if (loaderIsntAlreadyAddedToDom) {
        var loaderHtml = '<div id="component-loader-overlay" class="FEoverlay hide"><div id="component-loader" class="hide text-center"><i class="fa fa-spinner fa-pulse hide"></i><p class="loading">Loading &hellip;</p><p class="loadingParagraph"><em>Please wait one moment</em></p></div></div>';
        component.prepend(loaderHtml);
    }

    jQuery('[id="component-loader"]').addClass('loaderOn');
    jQuery('[id="component-loader-overlay"]').show();
    jQuery('[id="component-loader"]').show();
}

function fe_sp_removeLoadingSpinner() {
    jQuery("#component-loader-overlay").remove();
}

function fe_sp_getPostalCodeFromIpAddress() {
    if (FormsEngine.ShouldGetZipCodeFromIp) {
        fe_sp_makeRequestToGetPostalCodeFromIpAddress();
    }
}

function fe_sp_makeRequestToGetPostalCodeFromIpAddress() {
    var filters = "";

    if (FormsEngine.readCookie) {
        var ipOverrideCookie = FormsEngine.readCookie("FE_IPOverride");
        if (ipOverrideCookie) {
            filters = "ipOverride=" + ipOverrideCookie;
        }
    }

    var sUrl = FormsEngine.ServiceBaseURL + "/Location/GetPostalCode";
    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        data: filters,
        cache: false,
        url: sUrl,
        success: function (postalCode) {
            fe_sp_setPostalCodeFieldFromIpAddress(postalCode);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
        }
    });
}

function fe_sp_setPostalCodeFieldFromIpAddress(postalCode) {
    jQuery(FormsEngine.DefaultFormTag + " [code='Postal_Code']").val(postalCode);
    jQuery(FormsEngine.DefaultFormTag + " [code='Postal_Code_Duplicate']").val(postalCode);
}

function fe_sp_setSchoolPickerCarouselMessage(messageType) {
    var message = fe_sp_getSchoolPickerCarouselMessage(messageType);

    if (message) {
        jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel-message").text(message);
    }
}

function fe_sp_getSchoolPickerCarouselMessage(messageType) {

    var message;

    switch (messageType) {
        case fe_sp_schoolPickerCarouselMessageType.limitReached:
            message = FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE"];
            break;
        case fe_sp_schoolPickerCarouselMessageType.default:
            message = FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE"];
            break;
        default:
            message = "";
            break;
    }

    return message;
}

function fe_sp_getMaxSubmissionCount() {
    var campaignDetails = FormsEngine.CampaignDetail || {};
    return campaignDetails.MaxSubmissionCount || 0;
}

function fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown() {
    var selector = FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep;
    var currentStepContainsSchoolPickerCarousel = jQuery(selector).find("#school-picker-carousel").length > 0;

    if (FormsEngine.CurrentStep == 1) {
        jQuery(FormsEngine.BackButton).hide();
        jQuery(FormsEngine.RestartButton).hide();
    } else if (FormsEngine.SchoolPickerCarouselLoaded || currentStepContainsSchoolPickerCarousel) {
        jQuery(FormsEngine.BackButton).hide();
        jQuery(FormsEngine.RestartButton).show();
    } else {
        jQuery(FormsEngine.BackButton).show();
        jQuery(FormsEngine.RestartButton).hide();
    }
}

function fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions(stepNumber) {
    if (FormsEngine.StepDynamicQuestions == stepNumber) {
        var additionalQuestionsHtml = jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find("#DynamicQuestions").html();

        if (!additionalQuestionsHtml) {
            jQuery(FormsEngine.SubmitButton).trigger("click");
        }
    }
}

function fe_sp_loadFormWizardCallBack() {
    fe_sp_buildStepIndicatorBar();
    fe_sp_hideStepTitles();
    fe_sp_getPostalCodeFromIpAddress();
    fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown();

    FormsEngine.SchoolPickerCarouselLoaded = false;
    fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();
}

function fe_sp_buildStepIndicatorBar() {
    fe_sp_removeExistingStepIndicators();
    fe_sp_addStepIndicatorsToDom();
    fe_sp_selectVisitedStepIndicators();
    fe_sp_displayFutureSteps(3);
}

function fe_sp_removeExistingStepIndicators() {
    jQuery(".eddy-form-wizard-header #institution-wizard-step-indicator-container").html("");
}

function fe_sp_addStepIndicatorsToDom() {
    var stepIndicatorNames = fe_sp_getUniqueStepIndicatorNames();
    jQuery(fe_sp_getStepIndicatorContainerIdentifier()).addClass("total-" + stepIndicatorNames.length + "-steps");

    for (var x = 0; x < stepIndicatorNames.length; x++) {
        var stepIndicatorName = stepIndicatorNames[x];
        var stepNumber = x + 1;
        fe_sp_addStepIndicatorToDom(stepIndicatorName, stepNumber);
    }
}

function fe_sp_getUniqueStepIndicatorNames() {
    FormsEngine.StepIndicators = {};

    var stepIndicatorNames = [];
    var uniqueStepIndicatorNames = {};

    for (var i = 1; i <= FormsEngine.StepTotal; i++) {
        var stepIndicatorName = jQuery(FormsEngine.DefaultFormTag).find("[id=NameForStep" + i + "]").text().trim();

        FormsEngine.StepIndicators[i] = fe_sp_prepIndicatorNameForId(stepIndicatorName);

        if (!uniqueStepIndicatorNames[stepIndicatorName]) {
            uniqueStepIndicatorNames[stepIndicatorName] = true;
            stepIndicatorNames.push(stepIndicatorName);
        }
    }

    return stepIndicatorNames;
}

function fe_sp_addStepIndicatorToDom(stepIndicatorName, stepNumber) {
    var stepIndicatorHtml = fe_sp_buildStepIndicatorHtml(stepIndicatorName, stepNumber);
    jQuery(".eddy-form-wizard-header #institution-wizard-step-indicator-container").append(stepIndicatorHtml);
}

function fe_sp_buildStepIndicatorHtml(stepIndicatorName, stepNumber) {
    var zIndex = 1000 - stepNumber;
    var stepIndicatorId = fe_sp_getStepIndicatorIdName(stepIndicatorName);

    var stepIndicatorHtml = "<div class='step-indicator' id='" + stepIndicatorId + "' style='z-index:" + zIndex + "'>";
    stepIndicatorHtml += "<p>";
    stepIndicatorHtml += "Step " + stepNumber + ": <br/>";
    stepIndicatorHtml += stepIndicatorName;
    stepIndicatorHtml += "</p>";
    stepIndicatorHtml += "</div>";

    return stepIndicatorHtml;
}

function fe_sp_selectVisitedStepIndicators() {
    fe_sp_removeAllVisitedStepIndicatorClasses();
    fe_sp_addVisitedStepIndicatorClasses();
}

function fe_sp_removeAllVisitedStepIndicatorClasses() {
    var stepIndicatorContainerIdentifier = fe_sp_getStepIndicatorContainerIdentifier();
    jQuery(stepIndicatorContainerIdentifier).find(".visited-step-indicator").each(function () {
        jQuery(this).removeClass("visited-step-indicator").removeClass("current-step");
    });
}

function fe_sp_addVisitedStepIndicatorClasses() {
    if (FormsEngine.StepIndicators && FormsEngine.CurrentStep) {
        var currentStepNumber = FormsEngine.CurrentStep;

        for (var stepCount = 1; stepCount <= currentStepNumber; stepCount++) {
            var stepIndicatorName = FormsEngine.StepIndicators[stepCount];
            if (stepIndicatorName) {
                var identifier = fe_sp_getCurrentStepIndicatorIndentifier(stepIndicatorName);
                jQuery(fe_sp_getStepIndicatorContainerIdentifier() + " .visited-step-indicator").removeClass("current-step");
                jQuery(identifier).addClass("visited-step-indicator current-step");
            }
        }
    }
}

function fe_sp_getCurrentStepIndicatorIndentifier(stepIndicatorName) {
    var stepIndicatorContainerIdentifier = "";

    if (stepIndicatorName) {
        stepIndicatorContainerIdentifier = fe_sp_getStepIndicatorContainerIdentifier();
        stepIndicatorContainerIdentifier += " #";
        stepIndicatorContainerIdentifier += fe_sp_getStepIndicatorIdName(stepIndicatorName);
    }

    return stepIndicatorContainerIdentifier;
}

function fe_sp_getStepIndicatorContainerIdentifier() {
    return ".eddy-form-wizard-header #institution-wizard-step-indicator-container";
}

function fe_sp_getStepIndicatorIdName(stepIndicatorName) {
    return stepIndicatorName ? "step-indicator-" + fe_sp_prepIndicatorNameForId(stepIndicatorName) : "";
}

function fe_sp_prepIndicatorNameForId(indicatorName) {
    return indicatorName ? indicatorName.toLowerCase().replace(/\s/g, '') : "";
}

function fe_sp_hideStepTitles() {
    jQuery(FormsEngine.DefaultFormTag + " .form-page-step-message").hide();
}

//identify the last step(step-last-display)to display and future-steps-hide not display
function fe_sp_displayFutureSteps(allowedMaxDisplayFutureSteps) {
    var futureSteps = jQuery(fe_sp_getStepIndicatorContainerIdentifier() + " .step-indicator:not(.visited-step-indicator)");
    var visitedSteps = jQuery(fe_sp_getStepIndicatorContainerIdentifier() + " .visited-step-indicator:not(.current-step)");

    var futureStepsCount = 0;
    var visitedStepsCount = 0;

    jQuery(fe_sp_getStepIndicatorContainerIdentifier() + " .step-indicator").removeClass("step-last-display").removeClass("step-first-display").removeClass("steps-hide");

    //real future steps number >= allowedMaxDisplayFutureSteps
    if (futureSteps.length >= allowedMaxDisplayFutureSteps) {
        jQuery(futureSteps).each(function () {
            futureStepsCount++;

            if (futureStepsCount == allowedMaxDisplayFutureSteps) {
                jQuery(this).addClass("step-last-display");
            } else if (futureStepsCount > allowedMaxDisplayFutureSteps) {
                jQuery(this).addClass("steps-hide");
            }
        });

        //hide all visited steps. only .current-step allow to display
        jQuery(visitedSteps).addClass("steps-hide");

    } else {

        //hide previous visited steps
        if (visitedSteps.length > 0) {
            jQuery(visitedSteps).each(function () {
                if (visitedStepsCount <= (visitedSteps.length - (allowedMaxDisplayFutureSteps - futureSteps.length)))
                    jQuery(this).addClass("steps-hide");

                visitedStepsCount++;

            });
        }
    }

    jQuery(fe_sp_getStepIndicatorContainerIdentifier() + " .step-indicator:visible").first().addClass("step-first-display");
}

function fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown() {
    var schoolPickerCarouselCount = jQuery(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep).find("#school-picker-carousel").length;

    if (schoolPickerCarouselCount < 1) {
        var valid = fe_sp_validateStepWithoutErrorMessages(FormsEngine.CurrentStep);
        if (valid) {
            fe_sp_enableSubmitButton();
        } else {
            fe_sp_disableSubmitButton();
        }
    } else {
        fe_sp_toggleSubmitButton();
    }
}

function fe_sp_buildSubcategoryCarousel() {

    var buttons = jQuery("#ulSubCategories").find("li");
    var numberOfButtons = buttons.length;
    var buttonHeight = buttons.first().height();
    var marginHeight = jQuery(".eddy-form-wizard-footer").height() + jQuery(".eddy-form-wizard-header").height();
    var viewHeight = (window.innerHeight - marginHeight) - (buttonHeight * 2);
    var numberOfButtonsPerSlide = Math.floor(viewHeight / buttonHeight);
    var numberOfSlides = Math.ceil(numberOfButtons / numberOfButtonsPerSlide);

    jQuery("#subcategory-carousel").remove();
    var carouselContainer = jQuery("<div id='subcategory-carousel'></div>");
    var slides = [];

    var currentSlide = jQuery("<div class='mySlides'><div>");
    var numberOfButtonsInSlide = 0;

    buttons.each(function () {

        currentSlide.append(jQuery(this));
        numberOfButtonsInSlide++;

        if (numberOfButtonsInSlide === numberOfButtonsPerSlide) {
            numberOfButtonsInSlide = 0;
            slides.push(currentSlide);
            currentSlide = jQuery("<div class='mySlides'><div>");

            if (slides.length === numberOfSlides) {
                return false;
            }
        }
    });

    for (var i = 0; i < slides.length; i++) {
        var slide = slides[i];
        carouselContainer.append(slide);
        var slideNumber = i + 1;
        fe_sp_addCarouselDotIndicator(slideNumber, "#subcategory-carousel", "#subcategories-carousel-indicators");
    }

    jQuery("#ulSubCategories").html(carouselContainer);

    fe_sp_addCarouselArrows("#subcategory-carousel", "#subcategories-carousel-indicators");
    fe_sp_selectInitialSlide("#subcategory-carousel", "#subcategories-carousel-indicators");
    fe_sp_showOrHideSubcategorySlideIndicators();

    jQuery(window).unbind("resize", fe_sp_showOrHideSubcategorySlideIndicators);
    jQuery(window).on("resize", fe_sp_showOrHideSubcategorySlideIndicators);
}

function fe_sp_showOrHideSubcategorySlideIndicators() {
    var widthSum = 0;

    jQuery(FormsEngine.DefaultFormTag + " #subcategories-carousel-indicators").find(".dot").each(function () {
        widthSum += jQuery(this).outerWidth(true);
    });

    var containerWidth = jQuery(FormsEngine.DefaultFormTag + " #subcategories-carousel-indicators").innerWidth();
    if (widthSum > containerWidth) {
        jQuery(FormsEngine.DefaultFormTag + " #subcategories-carousel-indicators").hide();
    } else {
        jQuery(FormsEngine.DefaultFormTag + " #subcategories-carousel-indicators").show();
    }
}

function fe_sp_convertNumberToWord(num) {

    if (!num) return '';

    var numString = num.toString(),
        units, tens, scales, start, end, chunks, chunksLen, chunk, ints, i, word, words;

    if (parseInt(numString) === 0) return 'zero';

    units = ['', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];
    tens = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];
    scales = ['', 'thousand', 'million', 'billion', 'trillion', 'quadrillion', 'quintillion', 'sextillion', 'septillion', 'octillion', 'nonillion', 'decillion', 'undecillion', 'duodecillion', 'tredecillion', 'quatttuor-decillion', 'quindecillion', 'sexdecillion', 'septen-decillion', 'octodecillion', 'novemdecillion', 'vigintillion', 'centillion'];

    start = numString.length;
    chunks = [];
    while (start > 0) {
        end = start;
        chunks.push(numString.slice((start = Math.max(0, start - 3)), end));
    }

    chunksLen = chunks.length;
    if (chunksLen > scales.length) return '';

    words = [];
    for (i = 0; i < chunksLen; i++) {
        chunk = parseInt(chunks[i]);

        if (chunk) {
            ints = chunks[i].split('').reverse().map(parseFloat);

            if (ints[1] === 1) ints[0] += 10;

            if ((word = scales[i])) words.push(word);

            if ((word = units[ints[0]])) words.push(word);

            if ((word = tens[ints[1]])) words.push(word);

            if ((word = units[ints[2]])) words.push(word + ' hundred');
        }
    }

    return words.reverse().join(' ');
}