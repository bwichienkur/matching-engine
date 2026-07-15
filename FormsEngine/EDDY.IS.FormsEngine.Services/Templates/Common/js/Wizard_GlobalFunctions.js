// Wizard_GlobalFunctions.js
// ** PREPEND ALL GLOBAL FUNCTIONS WITH fe_wiz_
// ** ALL jquery usage must be with the full "jQuery()" snytax in this file, no "$()" allowed!
//---------------------------------------------------------------------------------------------


function fe_wiz_updateMatchCount() {
    if (FormsEngine.UseProgramCounter) {
        fe_consolelog('Checking if we should update Program Count');

        var FormData = fe_getFormData();
        FormsEngine.LeadDataEncoded = encodeURIComponent(FormData.LeadData);

        // compare new Lead Data with previous Lead Data to avoid calling again
        if (FormsEngine.LeadDataEncoded != FormsEngine.LastProgramCountCallLeadDataEncoded) {
            var serviceArgs;
            // need session for this call
            fe_getSessionId(function () {
                serviceArgs = "?IsBeta=" + FormsEngine.IsBeta;
                serviceArgs += "&TrackId=" + FormsEngine.TrackId;
                serviceArgs += "&LeadData=" + FormsEngine.LeadDataEncoded;
                serviceArgs += "&InitialCall=" + (FormsEngine.InitialCall === undefined ? "true" : "false");
                serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;


                jQuery.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ServiceBaseURL + '/Matching/GetProgramsForCounter' + serviceArgs,
                    success: function (data) {
                        if (data != null) {
                            fe_wiz_onFetchMatchCountComplete(data);
                        }
                    },
                    error: function (request, error) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    }
                });

                // store the last me call
                FormsEngine.LastProgramCountCallLeadDataEncoded = FormsEngine.LeadDataEncoded;
            });
        }
        FormsEngine.InitialCall = false;
    }
}

function fe_wiz_focusOnNextFocusable(e) {
    FormsEngine.CurrentStep = FormsEngine.CurrentStep || 1;

    if (!jQuery(FormsEngine.DefaultFormTag).is(":visible")) {
        return;
    }

    var top_of_element = jQuery(FormsEngine.DefaultFormTag).offset().top;
    var bottom_of_element = jQuery(FormsEngine.DefaultFormTag).offset().top + jQuery(FormsEngine.DefaultFormTag).outerHeight();
    var bottom_of_screen = jQuery(window).scrollTop() + jQuery(window).height();


    if ((bottom_of_screen > top_of_element) && (bottom_of_screen < bottom_of_element) && FormsEngine.CurrentStep != 1) {
        // The element is visible, do something
        FormsEngine.AutoFocus = true;
    }
    else if (FormsEngine.CurrentStep === 1 && e == 'null') {
        // The element is not visible, do something else
        FormsEngine.AutoFocus = false;
        return;
    }


    var focusables = jQuery(FormsEngine.DefaultFormTag).find('[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find(":input:visible");


    if (FormsEngine.ShowAllQuestionsOnFirstStep && FormsEngine.HasAdditionalQuestions) {
        jQuery.merge(focusables, jQuery(FormsEngine.DefaultFormTag).find('[name="step"][data-step="' + FormsEngine.StepDynamicQuestions + '"]').find(":input:visible"));
    }

    if (e == 'null') {

        if (focusables.length > 0) {
            var next = focusables.eq(0);
            //we dont want to do this on dropdowns because on mobile it autoexpands them and it doesnt make sense for drop downs anyway
            //also dont focus on controls with a mask
            if (!next.is('select') && fe_getIEVersion().major == "-1") {
                next.focus();
            }
        }
    }
    else {

        if (focusables.length > 0) {

            var currentIndex = focusables.index(e.target);
            var current = focusables.eq(currentIndex);

            if (focusables.eq(currentIndex + 1).length) {

                var next = focusables.eq(currentIndex + 1);
                //we dont want to do this on dropdowns because on mobile it autoexpands them and it doesnt make sense for drop downs anyway
                //also dont focus on controls with the mask
                if (!next.is('select') || (e.keyCode == 9 || e.which == 9)) {
                    next.focus();
                }
            }
            else if (!fe_isLastStep()) {
                jQuery(FormsEngine.SubmitButton).trigger('click');
            }
        }
    }
}

function fe_wiz_onFetchMatchCountComplete(num) {
    var current = 0;
    var finish = 0;
    try {
        current = parseInt(jQuery(FormsEngine.ProgramCounterTag).html());
        current = current > 0 ? current : 0;
    } catch (e) { }

    try {
        finish = parseInt(num);
        finish = finish > 0 ? finish : 0;
    }
    catch (e) { }

    fe_consolelog('Old Program Count: ' + current + '    New Program Count: ' + finish);

    var miliseconds = 3000;
    var iterations = 20;
    var increase = current < finish;
    var delta = Math.floor(Math.abs(finish - current) / iterations);
    if (delta < iterations) {
        jQuery(FormsEngine.ProgramCounterTag).html(finish)
    }
    else {
        var rate = increase ? delta : -delta;
        var counter = setInterval(function () {
            if ((increase && current >= finish) || (!increase && current <= finish)) {
                jQuery(FormsEngine.ProgramCounterTag).html(finish);
                clearInterval(counter);
            }
            else {
                jQuery(FormsEngine.ProgramCounterTag).html(current);
                current = current + rate;
            }
        }, miliseconds / iterations);
    }
}

function fe_wiz_showSelectAllButtons() {

    // add controls
    var CategoriesControl = jQuery('ul[code="Categories"]');
    if (jQuery(CategoriesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-categories multi-select-button" name="select-all-categories">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-categories multi-select-button" name="de-select-all-categories">Clear All</div>';
        jQuery(CategoriesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery('div[name="select-all-categories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Categories', true);
    })
    jQuery('div[name="de-select-all-categories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Categories', false);
    })

    // add controls
    var SubCategoriesControl = jQuery('ul[code="SubCategories"]');
    if (jQuery(SubCategoriesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-subcategories multi-select-button" name="select-all-subcategories">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-subcategories multi-select-button" name="de-select-all-subcategories">Clear All</div>';
        jQuery(SubCategoriesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery('div[name="select-all-subcategories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('SubCategories', true);
    })
    jQuery('div[name="de-select-all-subcategories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('SubCategories', false);
    })

    // add controls
    var SpecialtiesControl = jQuery('ul[code="Specialties"]');
    if (jQuery(SpecialtiesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-specialties multi-select-button" name="select-all-specialties">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-specialties multi-select-button" name="de-select-all-specialties">Clear All</div>';
        jQuery(SpecialtiesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery('div[name="select-all-specialties"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Specialties', true);
    })
    jQuery('div[name="de-select-all-specialties"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Specialties', false);
    })
}


function fe_wiz_selectAllMultiCheckBoxField(code, isChecked) {
    var CheckBoxList = jQuery(FormsEngine.DefaultFormTag).find('input[id^="' + code + '"]');

    //Google Analytics and internal tracking event
    var trackingValue = isChecked ? "Yes" : "Clear";
    fe_wiz_selectAllEventTracking("SelectAll", trackingValue);

    FormsEngine.SelectAllTriggered = true;
    jQuery(CheckBoxList).each(function () {
        if (isChecked && !jQuery(this).is(':checked')) {
            jQuery(this).checked = true;
            jQuery(this).attr({ 'checked': 'checked' });
            jQuery(this).prop("checked", "checked");
            jQuery(this).trigger('change');
        }
        else if (!isChecked && jQuery(this).is(':checked')) {
            jQuery(this).checked = false;
            jQuery(this).removeAttr('checked');
            jQuery(this).removeProp("checked", "checked");
            jQuery(this).trigger('change');
        }
    });
    if (isChecked == false) {
        jQuery('input[name="' + code + '_Selections"]').val('');
    }
    FormsEngine.SelectAllTriggered = false;
    jQuery('input[name="' + code + '_Selections"]').trigger('change');
    fe_saveForm();
    if (isChecked && fe_wiz_GetCountOfQuestionsOnStep() == 1 && FormsEngine.BackButtonClicked != true) {
        fe_consolelog('Auto Forward Step!');
        jQuery(FormsEngine.SubmitButton).trigger('click');
    }
}

//because specific internal tracking was requested to not use workflow as event name added this function
function fe_wiz_selectAllEventTracking(eventName, eventValue) {
    var Gevent = eventName + "." + eventValue;
    if (FormsEngine.TemplateId != undefined && FormsEngine.TemplateId > 0) {
        Gevent += ".TemplateId." + FormsEngine.TemplateId;
    }

    try {
        if (typeof _gaq != 'undefined') {
            _gaq.push(['_trackPageview', Gevent]);
        }
    } catch (e) { }

    try {
        if (typeof _etq != 'undefined') {
            _etq.push(['_etEvent', eventName, eventValue, 'form-wizard']);
        }
    } catch (e) { }
}

function fe_wiz_keyTabAndEnterEvents(e) {
    if (e.keyCode == 9 || e.which == 9) {
        fe_wiz_focusOnNextFocusable(e);
    }
    else if (e.keyCode == 13 || e.which == 13) {
        if (!FormsEngine.Flags) {
            FormsEngine.Flags = new Object();
        }

        var id = jQuery(e.srcElement).attr("id");
        if (FormsEngine.Flags.hasOwnProperty(id) && FormsEngine.Flags[id] != "enterpressed" && FormsEngine.Flags[id] != "") {
            FormsEngine.Flags[id] = "enterpressed";
        }
        else {
            FormsEngine.Flags[id] = "enterpressed";
            jQuery(FormsEngine.SubmitButton).trigger('click');
        }

    }
}

function fe_wiz_getLastStep() {
    if (FormsEngine.HasAdditionalQuestions && FormsEngine.RenderingStrategy != "SCHOOLPICKERWIZARD")
        return FormsEngine.StepDynamicQuestions;
    else
        return FormsEngine.StepLast;
}

function fe_wiz_AutoForwardStep(currentElement) {
    var email = jQuery(':input[code="Email"]');
    var optin = jQuery(':input[code="NewsLetterOptIn"]');
    var code = jQuery(currentElement).attr("code");
    var optinStep = email.exists() && optin.exists() && jQuery(email).attr('step') === jQuery(optin).attr('step') && FormsEngine.CurrentStep == jQuery(email).attr('step');

    var emsApplicationId = 27;

    if (FormsEngine.BackButtonClicked
        || FormsEngine.FormsHasBeenRecovered
        || FormsEngine.CurrentStep == fe_wiz_getLastStep()
        || fe_wiz_CurrentStepContainsCategoryLikeQuestion()
        || fe_wiz_CurrentStepContainsQuestionsWithDefaults()
        || code === "Desired_Degree_Level"
        || fe_getIEVersion().major === 10
        || (optinStep && code != "NewsLetterOptIn")
        || FormsEngine.ApplicationId == emsApplicationId
        || FormsEngine.DontAllowAutoForwardStep) {
        fe_consolelog('Not Auto moving forward because either the user has hit back, form has been recovered, user is on the last step, the categories control is on this step, or there are controls with defaults, or IE10, or this is an EMS form, or auto forward step is disabled.');
        return true;
    }

    var eventDriven = false;

    var elementRules = {};
    if (currentElement) {
        elementRules = jQuery(currentElement).rules();
    }

    // Check if there are pending async validations..
    if (FormsEngine.PendingAsyncValidations && FormsEngine.PendingAsyncValidations.Length > 0) {

        FormsEngine.PendingAutoAdvance = true;

        var currentEvents = {};
        try {
            currentEvents = jQuery._data(jQuery(FormsEngine.SubmitButton)[0], "events");
        }
        catch (ex) {
            currentEvents = jQuery(FormsEngine.SubmitButton).data("events");
        }

        if (!currentEvents.hasOwnProperty("execute")) {
            // Set event listener..
            jQuery(FormsEngine.SubmitButton).on("execute", function () {
                if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
                    fe_consolelog('Auto Forward Step!');
                    if (FormsEngine.PendingAutoAdvance) {
                        jQuery(FormsEngine.SubmitButton).trigger('click');
                    }
                    FormsEngine.PendingAutoAdvance = false;
                }
            });

            eventDriven = true;
        }
    }

    if (!eventDriven) {
        if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
            FormsEngine.PendingAutoAdvance = false;
            fe_consolelog('Auto Forward Step!');
            jQuery(FormsEngine.SubmitButton).trigger('click');
        }
    }
}


function fe_wiz_KeyAutoAdvance(self, e, maxsize) {
    //wait until character is inserted
    setTimeout(function () {
        checkValidMobileButton();

        if (self && self.val && self.val().length >= maxsize) {
            if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
                if (!navigator.userAgent.match(/iPhone/i) && !navigator.userAgent.match(/iPad/i)) {
                    if (FormsEngine.PendingAutoAdvance !== true) {
                        fe_wiz_AutoForwardStep(self);
                    }
                }
            }
            else {
                fe_wiz_focusOnNextFocusable(e);
            }
        }
    }, 100);
}

function fe_wiz_CurrentStepContainsCategoryLikeQuestion() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('div[data-code="Categories"],div[data-code="SubCategories"],div[data-code="Specialties"]').exists();
}

function fe_wiz_GetCountOfQuestionsOnStep() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('div[data-controlcode]').length;
}

function fe_wiz_CurrentStepContainsQuestionsWithDefaults() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('[data-hasinstancedefault="True"]').exists();
}

function fe_wiz_AllCurrentStepQuestionsAreComplete() {
    var Result = true;
    var LeadDataArray = jQuery(FormsEngine.DefaultFormTag).serializeArray();
    jQuery.each(LeadDataArray, function (index, item) {
        if (!Result) {
            return false; // break the loop
        }
        var field = jQuery(FormsEngine.DefaultFormTag).find(":input[name='" + LeadDataArray[index].name + "']");


        if (jQuery(field).attr('step') == FormsEngine.CurrentStep || (jQuery.contains(jQuery(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.StepDynamicQuestions)[0], field[0]) && FormsEngine.ShowAllQuestionsOnFirstStep)) {
            var code = jQuery(field).attr("code");
            if (code == 'undefined' || code == undefined) {
                return true; // continue to next item in loop
            }

            var value = LeadDataArray[index].value;
            if (value.length <= 0) {
                Result = false;
            }
        }
    });

    return Result;
}

function fe_wiz_updateControlsSortIndexes() {
    jQuery(FormsEngine.DefaultFormTag).find('div[data-rowsequence]').each(function (index) {
        var i = (index + 1);
        jQuery(this).attr('data-rowsequence', i);
        jQuery(this).find('[id-sort]').each(function () {
            jQuery(this).attr('id-sort', i);
        });
    });
}

function fe_wiz_getAdditionalQuestionCollection() {

    var usProgramTemplateIds = jQuery("#mc-US-Area").find('#mc-SM-additional-questions-templates-used').attr('data-program-templates-used');

    //if we still have templates to retrieve make the call otherwise skip this brick.
    if (usProgramTemplateIds.length > 0) {
        fe_wiz_requestAdditionalQuestionCollection(usProgramTemplateIds, fe_wiz_injectAdditionalQuestions);
    }
}

function fe_wiz_requestAdditionalQuestionCollection(programTemplateIds, callback) {

    fe_getSessionId(function () {
        var arguments = "FeSessionId=" + FormsEngine.FESessionId;
        arguments += "&WizardTemplateId=" + FormsEngine.TemplateId;
        arguments += "&ProgramTemplateIds=" + programTemplateIds;
        arguments += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
        arguments += "&IsBeta=" + FormsEngine.IsBeta;
        arguments += "&TrackId=" + FormsEngine.TrackId;
        arguments += "&FormFilterValues=" + FormsEngine.LeadDataEncoded + "%2F" + FormsEngine.LeadAdditionalDataEncoded;

        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/TemplateManager/GetAdditionalTemplateQuestionCollection?" + arguments,
            success: function (data) {
                callback(data);
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    });
}

function fe_wiz_getNextQuestionInStep(stepId, controlSortId) {
    //get the div for this step
    var thediv = jQuery('#Step' + stepId);
    //find whatever has id-sort param+1 and return it
    //force convert to int so we can add one
    var nextId = parseInt(controlSortId, 0) + 1;
    return jQuery(thediv).find('[id-sort="' + nextId + '"]');
}


function displayUSProgramInfoLink(ddl) {
    var selectedItem = jQuery(ddl).find('option:selected');
    var usProgramInfoLink = jQuery(ddl).parents("[name='mc-us-schoolcontainer']").find('[name="us-program-popup-link"]');
    jQuery(usProgramInfoLink).hide();
    if (jQuery(selectedItem).exists() && jQuery(selectedItem).val() != '' && jQuery(selectedItem).attr('data-hasprogramdescription').toLowerCase() == 'true') {
        jQuery(usProgramInfoLink).text("About " + jQuery(ddl).find('option:selected').attr('data-program-name'));
        jQuery(usProgramInfoLink).show();
    }
}


function fe_wiz_mc_convertAdditionalQForTemplateStringToCollection(str) {
    var arr = new Array();
    if (str != "") {
        var split = str.split(';');
        for (var i = 0; i < split.length; i++) {
            var secSplit = split[i].split(',');
            var theQA = {
                TemplateId: secSplit[0],
                RenderedControl: secSplit[1]
            }
            arr.push(theQA);
        }
    }
    return arr;
}

function fe_wiz_mc_convertAdditionalQForTemplateCollectionToString(col) {
    var str = "";
    for (var i = 0; i < col.length; i++) {
        str += col[i].TemplateId + "," + col[i].RenderedControl + ";";
    }

    //this removes that last semicolon to make split cleaner on retrieval
    return str.substring(0, str.length - 1);
}

//callback for above method to actually inject the questions into the selection page.
//data is a dictionary of templateid/renderedcontrol pairs
function fe_wiz_injectAdditionalQuestions(data) {

    //add additional questions if needed for each placeholder
    jQuery.each(jQuery("#mc-US-Area").find('div[name=mc-SM-additional-questions]'), function (index, obj) {

        //get the li since that has school info
        var parentli = jQuery(this).parents('[name=mc-us-schoolcontainer]');
        fe_wiz_mc_bindAdditionQuestions(parentli, data, this);
    });

    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('select[name="mc-us-program-ddl"]').each(function () {
        displayUSProgramInfoLink(this);
    });

    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('select[name="mc-us-program-ddl"]').on("change", function () {
        displayUSProgramInfoLink(this);
        fe_wiz_mc_rebindAdditionQuestions(this);
        fe_wiz_mc_checkTwoULeadShareForProgram(this);
    });

    //if we have a session object bind question answers to whats saved in session
    fe_wiz_mc_setAdditionalQuestionAnswersFromSession();

    fe_wiz_mc_hideAskedAdditionalQuestions();

    fe_wiz_mc_hideAdditionalQuestionDivIfAllQuestionsHidden();

    fe_wiz_mc_showAdditionalQuestionsForCheckedSchools();
}

function fe_wiz_mc_showAdditionalQuestionsForCheckedSchools() {
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').each(function () {
        var parentli = jQuery(this).parents('[name=mc-us-schoolcontainer]');
        var additionalQuestionsDiv = parentli.find("[name=mc-SM-additional-questions]");
        if (jQuery(this).is(":checked")) {
            jQuery(additionalQuestionsDiv).show();
        }
    });
}


//because a collection of objects was not working with jsonp get this data is stored as a delimited string.
//the below two functions convert it to and from the object we ideally want to work with
function fe_wiz_mc_convertAdditionalQAStringToCollection(str) {
    var arr = new Array();
    if (str != undefined && str != null && str != "") {
        var split = str.split(';');
        for (var i = 0; i < split.length; i++) {
            var secSplit = split[i].split(',');
            var theQA = {
                code: secSplit[0],
                answer: secSplit[1]
            }
            arr.push(theQA);
        }
    }
    return arr;
}

function fe_wiz_mc_convertAdditionalQACollectionToString(col) {
    var str = "";
    for (var i = 0; i < col.length; i++) {
        str += col[i].code + "," + col[i].answer + ";";
    }

    //this removes that last semicolon to make split cleaner on retrieval
    return str.substring(0, str.length - 1);
}

function fe_wiz_mc_getAdditionalQASessionObject(callback) {
    fe_getSessionId(function () {
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/Session/GetAdditionalQAObject?FESessionId=" + FormsEngine.FESessionId,
            success: function (data) {
                callback(fe_wiz_mc_convertAdditionalQAStringToCollection(data));
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                callback();
            }
        });
    });
}

function fe_wiz_mc_setAdditionalQASessionObject(value) {
    fe_getSessionId(function () {
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/Session/SetAdditionalQAObject?FESessionId=" + FormsEngine.FESessionId + "&Value=" + fe_wiz_mc_convertAdditionalQACollectionToString(value),
            success: function (data) {
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    });
}

function fe_wiz_mc_setAdditionalQuestionAnswersFromSession() {
    fe_wiz_mc_getAdditionalQASessionObject(function (data) {
        if (data != undefined && data != null && data != "") {
            for (var i = 0; i < data.length; i++) {
                jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(':input[code=' + data[i].code + ']').each(function () {
                    if (jQuery(this).is(':radio')) {
                        if (jQuery(this).attr('value') == data[i].answer) {
                            jQuery(this).attr('checked', true);
                        }
                    }
                    else {
                        jQuery(this).val(data[i].answer);
                    }
                });
            }
        }
    });
}

//this function will prepend the campusId to all controlIds and names in the provided string. 
//this will avoid the additional questions on the school selection page having the same Ids/names
function fe_wiz_mc_updateAdditionalQuestionControlIds(controlsString, campusId) {
    var str = controlsString.replace(new RegExp('<input id=\"', 'g'), '<input id=\"' + campusId + '_');
    str = str.replace(new RegExp('<select id=\"', 'g'), '<select id=\"' + campusId + '_');
    str = str.replace(new RegExp('<label for=\"', 'g'), '<label for=\"' + campusId + '_');
    str = str.replace(new RegExp(' name=\"', 'g'), ' name=\"' + campusId + '_');
    return str;
}

function fe_wiz_mc_saveAdditionalQuestions(data, controlCode, controlValue, saveArray) {

    var userEntry = {
        code: controlCode,
        answer: controlValue
    }

    var additionalQuestionAnswerArray = null;

    if (data == null || data == undefined || data == "") {
        //there is no session object so initialize an array and push our first object which was passed in
        additionalQuestionAnswerArray = new Array(userEntry);
    }
    else {
        //there is data in session so we need to check if our code is already in there and replace the value
        //or if its not there just add it to the array
        additionalQuestionAnswerArray = data;
        var i = 0;
        var foundEntry = false;
        //check if question/answer are already in the array
        for (i = 0; i < additionalQuestionAnswerArray.length; i++) {
            if (additionalQuestionAnswerArray[i].code == userEntry.code) {
                //its in there. set our flag and set the new value to that code.
                foundEntry = true;
                additionalQuestionAnswerArray[i].answer = userEntry.answer;
            }
        }
        //we didnt find it so add it
        if (!foundEntry) {
            additionalQuestionAnswerArray.push(userEntry);
        }
    }

    if (saveArray == true)
        fe_wiz_mc_setAdditionalQASessionObject(additionalQuestionAnswerArray);

    fe_pushSingleFieldToGTMDataLayer(controlCode, controlValue);

    return additionalQuestionAnswerArray;
}

function fe_wiz_buildAdditionalQuestionsAnswersForSubmit() {
    var additionalQuestionAnswerArray = null;

    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').each(function () {
        var parentli = jQuery(this).parents('[name=mc-us-schoolcontainer]');
        var additionalQuestionsDiv = parentli.find("[name=mc-SM-additional-questions]");
        if (jQuery(this).is(":checked")) {
            jQuery(additionalQuestionsDiv).find(':input').each(function () {
                if (jQuery(this).is(':radio')) {
                    if (jQuery(this).attr('checked') == true || jQuery(this).attr('checked') == 'checked') {
                        additionalQuestionAnswerArray = fe_wiz_mc_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
                    }
                }
                else {
                    additionalQuestionAnswerArray = fe_wiz_mc_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
                }
            });
        }
    });

    return additionalQuestionAnswerArray;
}

function fe_wiz_buildAdditionalQuestionsAnswersForSubmitByCampus(campusid) {
    var additionalQuestionAnswerArray = null;
    var additionalQuestionBlock = jQuery("div[name=mc-SM-additional-questions][data-campusid=" + campusid + "]");

    jQuery(additionalQuestionBlock).find(':input').each(function () {
        if (jQuery(this).is(':radio')) {
            if (jQuery(this).attr('checked') === true || jQuery(this).attr('checked') === 'checked') {
                additionalQuestionAnswerArray = fe_wiz_mc_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
            }
        }
        else {
            additionalQuestionAnswerArray = fe_wiz_mc_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
        }
    });

    return additionalQuestionAnswerArray;
}

//questions already asked will be in the forms engine object. if the code for a question in our form exists in that string, hide it.
function fe_wiz_mc_hideAskedAdditionalQuestions() {
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(':input').each(function () {
        var theCode = jQuery(this).attr('code');
        if (FormsEngine.LeadDataEncoded.indexOf(theCode) > -1) {
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find("[data-controlcode='" + theCode + "']").hide();
        }
    });
}

function fe_wiz_mc_hideAdditionalQuestionDivIfAllQuestionsHidden() {
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('[name=mc-SM-additional-questions]').each(function () {
        var hasVisibleControl = false;
        jQuery(this).find(':input').each(function () {
            //for each input check if any are visible. if they are we are done here.
            if (jQuery(this).visible()) {
                hasVisibleControl = true;
            }
        });
        if (!hasVisibleControl) {
            jQuery(this).find('#additionalQuestionLabel').html('');
            jQuery(this).hide();
        }
    });

}

//Manual required validation for additional fields
function fe_wiz_ss_validateRequired(control) {
    var hasValue = false;
    var value = "";
    var controlCode = jQuery(control).attr("code");
    var containerVisible = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find("[data-controlcode='" + controlCode + "']").is(":visible");

    if (!containerVisible) {
        return true;
    }

    if (jQuery(control).is(':radio')) {
        value = jQuery(":input[code='" + controlCode + "']:checked").val();
    }
    else {
        value = jQuery(control).val();
    }

    value = (value == undefined || value == null) ? "" : value;
    hasValue = value.length > 0;

    if (hasValue) {
        jQuery("label[for='" + jQuery(control).attr("code") + "']").hide();
    }
    else {
        jQuery("label[for='" + jQuery(control).attr("code") + "']").show();
    }
    return hasValue;
}

function fe_wiz_mc_bindAdditionQuestions(parentli, data, additionalQuestionsDIV) {

    var selectedTemplateId = parentli.find("select[name='mc-us-program-ddl'] option:selected").attr('data-templateid');
    //build up this string to be appended with the additional questions using the selected value of the ddl
    var theTemplate = jQuery.grep(data, function (n, i) {
        return n.TemplateId == selectedTemplateId;
    });

    jQuery(additionalQuestionsDIV).html('');
    if (theTemplate != null && theTemplate != undefined && theTemplate != "") {
        var theControls = fe_wiz_mc_updateAdditionalQuestionControlIds(decodeURIComponent((theTemplate[0].RenderedControl + '').replace(/\+/g, '%20')), parentli.attr('data-campusid'));
        jQuery(theControls).appendTo(jQuery(additionalQuestionsDIV));
    }


    //bind change methods 
    //for each input in the additional questions add change to all other inputs with the same code
    jQuery(additionalQuestionsDIV).find(':input').each(function () {

        //change all other additional questions with the same code to have the same value.
        jQuery(this).change(function () {
            var theCode = jQuery(this).attr('code');
            var theVal = jQuery(this).val();
            //save value to session on change
            fe_wiz_mc_getAdditionalQASessionObject(function (res) {
                fe_wiz_mc_saveAdditionalQuestions(res, theCode, theVal, true);
            });

            if (jQuery(this).is(':radio')) {
                jQuery(this).attr('checked', true);
                jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(':input[code=' + theCode + '][value=' + theVal + ']').each(function () {
                    jQuery(this).attr('checked', true);
                });
            }
            else {
                //on change set the value of call controls with this code to this value.
                jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(':input[code=' + theCode + ']').each(function () {
                    jQuery(this).val(theVal);
                });
            }
            //re-validate control when data changes
            fe_wiz_ss_validateRequired(jQuery(this));

            checkValidMobileButton();
        });
    });

    //hide if checkbox is not selected
    var programCheckbox = parentli.find(":input[name='ms-us-school-checkbox']");
    if (programCheckbox.is(":checked")) {
        jQuery(additionalQuestionsDIV).show();
    }
    else {
        jQuery(additionalQuestionsDIV).hide();
    }
}

//when a ddl is changed to select a different program on the school selection the additional questions need to be rebound
function fe_wiz_mc_rebindAdditionQuestions(programChoiceDDL) {
    var campus = jQuery(programChoiceDDL).attr('data-campusid');
    var parentli = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find("[name='mc-us-schoolcontainer'][data-campusid=" + campus + "]");
    var addQuest = parentli.find('div[name=mc-SM-additional-questions]');
    fe_getSessionObject("ADDITIONALQUESTIONSPERTEMPLATEARRAY", function (data) {
        fe_wiz_mc_bindAdditionQuestions(parentli, fe_wiz_mc_convertAdditionalQForTemplateStringToCollection(data), addQuest);
        fe_wiz_mc_hideAskedAdditionalQuestions();
        fe_wiz_mc_setAdditionalQuestionAnswersFromSession();
        fe_wiz_mc_hideAdditionalQuestionDivIfAllQuestionsHidden();
    });
}

function fe_wiz_mc_checkTwoULeadShareForProgram(programChoiceDDL) {
    var showTwoU = jQuery(programChoiceDDL).attr('data-showtwoucheckbox');
    if (showTwoU == "True") {
        var campus = jQuery(programChoiceDDL).attr('data-campusid');
        var parentli = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find("[name='mc-us-schoolcontainer'][data-campusid=" + campus + "]");
        var schoolName = parentli.attr('data-institution-name');
        var programType = jQuery(programChoiceDDL).attr('data-programtype');
        fe_showUserAgreementForTwoULeadShareSchool(schoolName, programType, '#dvMCExpressConsent');
    }
}

//Gets the control value and value-key if is a me-integration field
function fe_wiz_mc_getAdditionalQuestionControlValue(controlCode) {

    var dependantField = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(":input[code='" + controlCode + "']");
    var ControlValue = {};
    ControlValue.value = "";
    ControlValue.valueKey = "";
    ControlValue.requiresKey = jQuery(dependantField).attr("me-filter") != undefined;

    if (jQuery(dependantField).is('select')) {
        ControlValue.value = jQuery(dependantField).val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(":input[code='" + controlCode + "']>option:selected").attr('key');
        }
    }
    else if (jQuery(dependantField).is(':radio')) {
        ControlValue.value = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(":input[code='" + controlCode + "']:checked").val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(":input[code='" + controlCode + "']:checked").attr('key');
        }
    }
    else {
        ControlValue.value = jQuery(dependantField).val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(dependantField).attr('key');
        }
    }

    ControlValue.value = ControlValue.value == null || ControlValue.value == undefined ? "" : ControlValue.value;
    ControlValue.valueKey = ControlValue.valueKey == null || ControlValue.valueKey == undefined ? ControlValue.value : ControlValue.valueKey;

    return ControlValue;
}


function fe_wiz_setErrorMessage(message) {
    jQuery(FormsEngine.DefaultFormTag).find("#ErrorMessage").text(message);
    if (message && message != "") {
        var topDiff = jQuery("#ErrorMessage")[0].getClientRects()[0].top;
        jQuery('html, body').animate({
            scrollTop: topDiff
        }, 100);
    }
}

function checkValidMobileButton() {

    if (jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("*[required=required]")
        .filter(function () { return this.value != '' }).length <
        jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("*[required=required]").length
        || jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("[type=checkbox][required=required]:visible:checked").length < jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("[type=checkbox][required=required]:visible").length

        || (jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').exists() && !jQuery('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + '] *').validate().checkForm())) {

        jQuery("#screen-button").removeClass("enabled-button");
    }
    else if (!jQuery("#screen-button").hasClass("enabled-button")) {
        jQuery("#screen-button").addClass("enabled-button");
    }
}

function checkMobileButtonCoveringInput() {
    var $cmb = jQuery("#screen-button:visible");
    var $currentInput = jQuery(":focus");

    if ($currentInput.length == 0 || $cmb.length == 0) {
        return;
    }

    var currentInputBottomMargin = parseInt($currentInput.parents("div.field-holder").css("margin-bottom"));
    var currentInputRect = $currentInput[0].getClientRects()[0];
    var cmbRect = $cmb[0].getClientRects()[0];

    // If there is a current control with focus and CMB is showing..
    if (currentInputRect && cmbRect) {
        // Check if CMB is covering current input..
        if (currentInputRect.bottom >= cmbRect.top) {
            var topDiff = currentInputRect.bottom - cmbRect.top + currentInputBottomMargin;
            jQuery('html, body').animate({
                scrollTop: topDiff
            }, 10);
        }
    }
}

function ContinueMobileButtonInit($currentButton) {
    var $screenButton = jQuery("#screen-button");
    if ($screenButton.length == 0) {
        return;
    }

    jQuery("body").addClass("screen-button-container");
    $currentButton.addClass("non-mobile-btn");
    $screenButton.removeClass("hide-button");

    // Assign the click event to the CMB..
    $screenButton.click(function () {
        $currentButton.trigger("click");
    });

    // Calculate the position of the CMB..
    FormsEngine.ContinueMobileButton_cmbHeight = $screenButton.height();
    FormsEngine.ContinueMobileButton_cmbTopOffset = window.innerHeight - FormsEngine.ContinueMobileButton_cmbHeight;
    FormsEngine.jQuery = jQuery.noConflict();
    FormsEngine.jQuery(document).ready(function () {
        $screenButton.css("top", FormsEngine.ContinueMobileButton_cmbTopOffset);
    });

    // Determine whether/where to place the CMB..
    fe_checkShowScreenButton();

    // Assign the window scroll event to determine whether/where to place the CMB..
    jQuery(window).scroll(function (e) {
        if (!FormsEngine.ContinueMobileButton_cmbScrolled) {
            fe_checkShowScreenButton();
        }
        FormsEngine.ContinueMobileButton_cmbScrolled = !FormsEngine.ContinueMobileButton_cmbScrolled;
    });

    // When the screen is resize (rotating mobile device) recalculate the height..
    jQuery(window).resize(function (event) {
        FormsEngine.ContinueMobileButton_cmbTopOffset = window.innerHeight - FormsEngine.ContinueMobileButton_cmbHeight;
        fe_checkShowScreenButton();
        if (jQuery("input:focus").length > 0) {
            FormsEngine.CheckingMobileButtonCoverInput = true;
        }
    });

    jQuery(document).on('focus', "input", function () {
        // If CMB is showing make sure it doesn't cover the current input..
        checkMobileButtonCoveringInput();
    });

    if (typeof (MutationObserver) != 'undefined') {
        // MutationObserver..

        // create an observer instance
        var observer = new MutationObserver(function (mutations) {
            // It's for fun..
            if (FormsEngine.CheckingMobileButtonCoverInput == true) {
                var $focused = jQuery("input:focus");
                if ($focused.length > 0) {
                    checkMobileButtonCoveringInput();
                    FormsEngine.CheckingMobileButtonCoverInput = false;
                }
            }
        });

        // configuration of the observer:
        var config = { attributes: true, childList: true, characterData: true };

        // pass in the target node, as well as the observer options
        observer.observe($screenButton[0], config);

        // later, you can stop observing
        if (FormsEngine.CMBMutationObserverDisconnect) {
            observer.disconnect();
        }

        // End MutationObserver..
    }
    else {
        // Use DOM Event if MutationObserver is not supported..
        jQuery(document).on('DOMNodeInserted', function (e) {
            // If currently editing an input and CMB is showing make sure it doesn't cover the current input..
            if (FormsEngine.CheckingMobileButtonCoverInput == true) {
                var $focused = jQuery("input:focus");
                if ($focused.length > 0) {
                    checkMobileButtonCoveringInput();
                    FormsEngine.CheckingMobileButtonCoverInput = false;
                }
            }
        });
    }

    // Assign blur event on text input to determine whether/where to place the CMB..
    jQuery(document).on('blur', "input[type=text]", function () { fe_checkShowScreenButton(); });
}

function fe_validateOpenMail(profileId, values, callback) {
    fe_getSessionId(function () {
        var sUrl =
            FormsEngine.ServiceBaseURL +
            '/OpenMail/ValidateRules?ProfileId=' +
            profileId +
            '&' +
            decodeURIComponent(values)
        jQuery
            .ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: sUrl,
                success: function (data) { },
                error: function (request, textStatus, errorThrown) {
                    fe_consolelog(arguments + '\n' + ' Error: ' + request.responseText)
                    fe_logClientException(request, sUrl, errorThrown)
                },
            })
            .done(callback)
    })
}

function fe_OpenMail(emsApplicationId, callback) {
    if (
        FormsEngine.CampaignDetail.OpenMailProfileId &&
        FormsEngine.ApplicationId !== emsApplicationId
    ) {
        var inputs = fe_serializeInputsInContainer(FormsEngine.DefaultFormTag);
        var FormData = encodeURIComponent(inputs);
        fe_validateOpenMail(
            FormsEngine.CampaignDetail.OpenMailProfileId,
            FormData,
            function (result) {
                if (result.SentToNoMatch) {
                    fe_trackLocalEvent('form-wizard', 'OpenMailRuleHit', result.MatchedRuleGroup);
                    FormsEngine.LoadWorkflowStep('NOMATCH', '');
                } else {
                    callback();
                }
            }
        )
    } else {
        callback();
    }
}

function fe_setDynamicTags(direction) {
    if (!FormsEngine) return
    var dt_currentStep = direction ? FormsEngine.CurrentStep + direction : FormsEngine.CurrentStep
    if (dt_currentStep > FormsEngine.StepTotal) dt_currentStep = FormsEngine.StepTotal
    var dt_valueTags = {
        '{TotalSteps}': FormsEngine.StepTotal,
        '{CurrentStep}': dt_currentStep,
        '{RemainingSteps}': FormsEngine.StepTotal - dt_currentStep,
    }
    var dt_reg = /({[^}]+})/g
    var dt_selector = '.formsEngineDynamicTag'
    var dt_templateAttr = 'dt-template'
    var dt_defaultAttr = 'dt-default'
    var dt_placements = jQuery(dt_selector).toArray()

    if (!dt_placements && dt_placements.length === 0) return

    dt_placements.forEach(function (dt_placement) {
     
        var dt_template = jQuery(dt_placement).attr(dt_templateAttr)

        var dt_default = jQuery(dt_placement).attr(dt_defaultAttr)
        if (!dt_template || !dt_default) return

        var dt_tags = dt_template.match(dt_reg)
        if (!dt_tags) return

        dt_tags.forEach(function (dt_tag) {
            if (!dt_template) return
            var dt_value = dt_valueTags[dt_tag]
            if (!dt_value) {
                dt_value = fe_getValue(dt_tag)
                dt_valueTags[dt_tag] = dt_value
            }
            if (!dt_value || dt_value === '') {
                fe_dt_getValueFromService(dt_placement, dt_template, dt_tag);
                dt_template = undefined
                return
            }
            dt_template = dt_template.replace(dt_tag, dt_value)
        })
        if (dt_template) {
            jQuery(dt_placement).html(dt_template)
        } else {
            jQuery(dt_placement).html(dt_default)
        }
    })
}

function fe_dt_formatTemplate(dt_placement, dt_template, dt_tag, dt_tag_value) {
    if (dt_placement && dt_template && dt_tag && dt_tag_value) {
        dt_template = dt_template.replace(dt_tag, dt_tag_value);
        if (dt_template) {
            jQuery(dt_placement).html(dt_template)
        }
    }
}

function fe_dt_getValueFromService(dt_placement, dt_template, dt_tag) {
    var queryStringId = undefined;
    switch (dt_tag) {
        case '{Category}':
            queryStringId = 'categories'
            break;
        case '{Subcategory}':
            queryStringId = 'subcategories'
            break;
        case '{Specialty}':
            queryStringId = 'specialties'
            break;
    }
    if (queryStringId) {
        var paramValue = fe_getParameterByName(queryStringId);
        var dt_label_value = '';
        if (!isNaN(paramValue)) {
            switch (queryStringId) {
                case 'categories':
                    fe_dt_getCategory(dt_placement, dt_template, dt_tag,paramValue);

                    break;
                case 'subcategories':
                    fe_dt_getSubCategory(dt_placement, dt_template, dt_tag,paramValue);

                    break;
                case 'specialties':
                    fe_dt_getSpecialty(dt_placement, dt_template, dt_tag, paramValue);
                    break;
            }
        }
    }
}

function fe_dt_getCategory(dt_placement, dt_template, dt_tag, categoryId) {


    var DataBindFilter = {};
    var prefix = filters == "" || filters == undefined ? "" : "&";

    //Required arguments
    var filters = prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
    filters += "&ApplicationId=" + FormsEngine.ApplicationId;

    DataBindFilter.FilterString = filters;


    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCategories";
    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        data: DataBindFilter,
        cache: false,
        url: sUrl,
        success: function (data) {
            if (data) {
                if (data.CategoryList) {
                    {
                        var categoryfromService = fe_dt_filterCategory(data.CategoryList, categoryId);
                        if (categoryfromService) {
                            var category = categoryfromService;
                            if (category) {
                                fe_dt_formatTemplate(dt_placement, dt_template, dt_tag, category.CategoryName);
                            }
                        }
                    }
                }
            }
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);

        }
    });

}

function fe_dt_filterCategory(categoryList, categoryId) {
    var category = null;
    if (!isNaN(categoryId)) {
        if (categoryList) {
            var filteredCategoryList = categoryList.filter(function (categoryListItem) {
                return categoryListItem.CategoryId == categoryId
            });
            if (filteredCategoryList) {
                if (filteredCategoryList.length > 0) {
                    if (filteredCategoryList[0]) {
                        category = filteredCategoryList[0];
                    }
                }
            }
        }
    }
    return category;
}

function fe_dt_getSubCategory(dt_placement, dt_template, dt_tag, subjectId) {


    var DataBindFilter = {};
    var prefix = filters == "" || filters == undefined ? "" : "&";

    //Required arguments
    var filters = prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
    filters += "&ApplicationId=" + FormsEngine.ApplicationId;

    DataBindFilter.FilterString = filters;


    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetSubCategories";
    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        data: DataBindFilter,
        cache: false,
        url: sUrl,
        success: function (data) {
            if (data) {
                if (data.SubjectList) {
                    {
                        var categoryfromService = fe_dt_filterSubCategory(data.SubjectList, subjectId);
                        if (categoryfromService) {
                            var category = categoryfromService;
                            if (category) {
                                fe_dt_formatTemplate(dt_placement, dt_template, dt_tag, category.SubjectName);
                            }
                        }
                    }
                }
            }
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);

        }
    });

}

function fe_dt_filterSubCategory(subjectList, subjectId) {
    var category = null;
    if (!isNaN(subjectId)) {
        if (subjectList) {
            var filteredCategoryList = subjectList.filter(function (categoryListItem) {
                return categoryListItem.SubjectId == subjectId;
            });
            if (filteredCategoryList) {
                if (filteredCategoryList.length > 0) {
                    if (filteredCategoryList[0]) {
                        category = filteredCategoryList[0];

                    }
                }
            }
        }
    }
    return category;
}

function fe_dt_getSpecialty(dt_placement, dt_template, dt_tag, specialtyId) {

   
    var DataBindFilter = {};
    var prefix = filters == "" || filters == undefined ? "" : "&";

    //Required arguments
    var filters = prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
    filters += "&ApplicationId=" + FormsEngine.ApplicationId;

    DataBindFilter.FilterString = filters;


    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetSpecialties";
    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        data: DataBindFilter,
        cache: false,
        url: sUrl,
        success: function (data) {

            if (data) {
                if (data.SpecialtyList) {
                    {
                        var specialtyfromService = fe_dt_filterSpecialty(data.SpecialtyList, specialtyId);
                        if (specialtyfromService) {
                            specialty = specialtyfromService;
                            if (specialty) {
                                fe_dt_formatTemplate(dt_placement, dt_template, dt_tag, specialty.SpecialtyName);
                            }
                        }
                    }
                }
            }
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);

        }
    });

}

function fe_dt_filterSpecialty(specialtyList, specialtyId) {
    var specialty = null;
    if (!isNaN(specialtyId)) {
        if (specialtyList) {
            var filteredSpecialtyList = specialtyList.filter(function (specialtyListItem) {
                return specialtyListItem.SpecialtyId == specialtyId
            });
            if (filteredSpecialtyList) {
                if (filteredSpecialtyList.length > 0) {
                    if (filteredSpecialtyList[0]) {
                        specialty = filteredSpecialtyList[0];
                    }
                }
            }
        }
    }
    return specialty;
}

function fe_getValue(dt_tag) {

    var dt_tagNames = {
        City: '{City}',
        Category: '{Category}',
        Subcategory: '{Subcategory}',
        InstitutionName: '{InstitutionName}',
        CampusPreference: '{CampusPreference}',
        Specialty: '{Specialty}',
        DesiredStartDate: '{DesiredStartDate}',
    }

    var dt_getValueFromControl = function (selector, queryStringId) {
        var dt_label = ''
        var dt_id = jQuery(FormsEngine.DefaultFormTag)
            .find("input[code='" + selector + "']")
            .val()
        if (dt_id && typeof dt_id === 'string') {
            dt_id = dt_id.split(',')[0]
            dt_label = jQuery('[for="' + selector + '_' + dt_id + '"]').html()
        }
        if ((!dt_label || dt_label === '') && queryStringId) {
            dt_label = fe_getParameterByName(queryStringId);
        }
        return dt_label
    }




    switch (dt_tag) {
        case dt_tagNames.City:
            return jQuery('[code="City"]').val()
        case dt_tagNames.InstitutionName:
            return FormsEngine.InstitutionName
        case dt_tagNames.Category:
            var dt_label_value = dt_getValueFromControl('Categories', 'Categories_Name');

            return dt_label_value;
        case dt_tagNames.Subcategory:

            var dt_label_value = dt_getValueFromControl('SubCategories', 'SubCategories_Name');

            return dt_label_value;
        case dt_tagNames.Specialty:
            var dt_label_value = dt_getValueFromControl('Specialties', 'Specialties_Name');

            return dt_label_value;
        case dt_tagNames.CampusPreference:
            return jQuery(FormsEngine.DefaultFormTag)
                .find("[code='CampusPreference']")
                .val()
        case dt_tagNames.DesiredStartDate:
            return jQuery(FormsEngine.DefaultFormTag)
                .find("[code='Desired_Start_Date']")
                .val()
        default:
            break
    }
}

function fe_loadGoogleAddress() {
    if (jQuery(FormsEngine.DefaultFormTag).find('input[code="Google_address"]').length > 0) {
        var apikey = jQuery(FormsEngine.DefaultFormTag).find('input[code="Address"]').attr('api-key');
        var script = document.createElement('script');
        script.type = "text/javascript";
        script.language = "javascript";
        script.src = "https://maps.googleapis.com/maps/api/js?key="+apikey+"&libraries=places&callback=fe_googleAddressSetUp";
        document.body.appendChild(script);
    }
}
function fe_googleAddressSetUp() {
    var ga_input = jQuery('input[code="Google_address"]')[0]
    var ga_options = {
        componentRestrictions: { country: ['us','ca'] },
        fields: ['formatted_address', 'name', 'address_component'],
        strictBounds: false,
        types: ['address'],
    }
    var ga_autocomplete = new google.maps.places.Autocomplete(ga_input, ga_options)

    ga_input.addEventListener('change', function (event) {
        fe_clearGoogleAddress();
    });
    ga_autocomplete.addListener('place_changed', function(event) {
        const place = ga_autocomplete.getPlace()
        if (place != undefined) {
            jQuery(FormsEngine.DefaultFormTag).find('input[code="Address"]').val(place.name)
            for (let index = 0; index < place.address_components.length; index++) {
                const element = place.address_components[index]
                switch (element.types[0]) {
                    case 'locality':
                        jQuery(FormsEngine.DefaultFormTag).find('input[code="City"]').val(element.long_name)
                        break
                    case 'administrative_area_level_1':
                        jQuery(FormsEngine.DefaultFormTag).find('input[code="State"]').val(element.short_name)
                        break
                    case 'postal_code':
                        jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code_Duplicate"]').val(element.short_name)
                        jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').val(element.short_name)
                        break
                    case 'country':
                        jQuery(FormsEngine.DefaultFormTag).find('input[code="Country"]').val(element.short_name)
                        break
                    default:
                        break
                }
                jQuery(FormsEngine.DefaultFormTag).find("input[code='Google_address']").valid()
            }
        } else {
            fe_clearGoogleAddress();
        }
    })
}

function fe_clearGoogleAddress() {
    jQuery(FormsEngine.DefaultFormTag).find('input[code="Address"]').val('')
    jQuery(FormsEngine.DefaultFormTag).find('input[code="City"]').val('')
    jQuery(FormsEngine.DefaultFormTag).find('input[code="State"]').val('')
    jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code_Duplicate"]').val('')
    jQuery(FormsEngine.DefaultFormTag).find('input[code="Country"]').val('')
}


jQuery(document).ready(function () {

    FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
    FormsEngine.ProgramCounterTag = FormsEngine.ProgramCounterTag || '#WizardStepContainer #ProgramMatches';

    // Internals variables
    var LastCount = {};
})




