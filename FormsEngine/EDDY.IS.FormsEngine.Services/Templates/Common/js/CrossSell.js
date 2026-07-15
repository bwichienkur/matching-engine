(function ($) {
    // CrossSell.js
    //-------------------

    //internal variables
    var SubmittingForm = false;

    function CrossSellSubmitWS(crossSellProgramArrayString, LeadDataEncoded) {

        var leadRequest = "";

        if (crossSellProgramArrayString.length < 1) {
            return;
        }

        //Validate inputs
        if (validateAdditionalQuestions() == false) {
            fe_hideLoader(true);
            SubmittingForm = false;
            $(FormsEngine.CrossSellSubmitTag).find('a').removeAttr('disabled', 'disabled').removeClass('disabled');
            AQ_setHeight()
            return;
        }

        FormsEngine.LeadDataEncoded = LeadDataEncoded;
        //take care of the additional question stuff and append to FormsEngine.LeadDataEncoded before passing back to form
        AQ_SetEncodedLeadDataForCrossSell();

        fe_getSessionId(function () {
            leadRequest += "TemplateId=" + FormsEngine.TemplateID;
            leadRequest += "&ProgramArrayString=" + crossSellProgramArrayString;
            leadRequest += "&IsBeta=" + FormsEngine.IsBeta;
            leadRequest += "&TrackId=" + FormsEngine.TrackId;
            leadRequest += "&SessionId=" + FormsEngine.SessionId;
            leadRequest += "&ProspectId=" + FormsEngine.ProspectId;
            leadRequest += "&LeadData=" + FormsEngine.LeadDataEncoded;
            leadRequest += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
            leadRequest += "&InitialLeadRawPostDataId=" + FormsEngine.RawPostDataId;
            leadRequest += "&InitialMatchWasValid=" + FormsEngine.InitialMatchWasValid;
            leadRequest += "&MatchGuid=" + FormsEngine.MatchResponseGuid;
            leadRequest += "&InitialLeadId=" + FormsEngine.InitialLeadId;
            leadRequest += "&FESessionId=" + FormsEngine.FESessionId;

            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/TemplateManager/CrossSellLeadSubmission?" + leadRequest,
                success: function (data) {
                    if (data != null) {
                        // Optimizely Conversion pixel

                        if (!FormsEngine.IsTestLead) {

                            //Google Tag Manager events
                            try {
                                var unique = 1;
                                for (index = 0; index < crossSellProgramArrayString.length; index++) {
                                    var programid = crossSellProgramArrayString[index].substring(0, crossSellProgramArrayString[index].indexOf('_'));
                                    fe_googleTagEvent('gaEvent' + unique.toString(), 'client', 'cross-sell', programid);
                                    unique++;
                                }

                                if (FormsEngine.InitialMatchWasValid == false && crossSellProgramArrayString.length > 0) {
                                    var programid = crossSellProgramArrayString[index].substring(0, crossSellProgramArrayString[0].indexOf('_'));
                                    fe_googleTagEvent('gaEvent', 'client', 'lead-success', programid);
                                }
                            } catch (ex) { }
                        }

                        

                        var SessionValues = new Object();
                        SessionValues.InitialUID = FormsEngine.UID;
                        SessionValues.IsCrossSell = true;
                        SessionValues.IsAnyLeadValid = true;
                        SessionValues.IsTestLead = FormsEngine.IsTestLead;
                        SessionValues.UserFullName = FormsEngine.UserFullName;
                        SessionValues.CrossSellThanksYouMessage = (data['Item2'] == 'undefined' || data['Item2'] == '') ? FormsEngine.CrossSellThankYouMessage : data['Item2'];
                        var SessionValuesEncoded = $.toJSON(SessionValues);

                        fe_setSessionObject("FE_Response", encodeURIComponent(SessionValuesEncoded), function () {
                            window.location = fe_GetProgramTemplateThankYouPageUrl();
                        });

                    }
                    else {
                        var SessionValues = new Object();
                        SessionValues.InitialUID = FormsEngine.UID;
                        SessionValues.IsCrossSell = true;
                        SessionValues.IsAnyLeadValid = FormsEngine.InitialMatchWasValid;
                        SessionValues.IsTestLead = FormsEngine.IsTestLead;
                        SessionValues.UserFullName = FormsEngine.UserFullName;
                        SessionValues.CrossSellThanksYouMessage = FormsEngine.CrossSellThankYouMessage;
                        var SessionValuesEncoded = $.toJSON(SessionValues);

                        fe_setSessionObject("FE_Response", encodeURIComponent(SessionValuesEncoded), function () {
                            window.location = fe_GetProgramTemplateThankYouPageUrl();
                        });

                    }

                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    hideLoader(true);
                    $(FormsEngine.CrossSellSubmitTag).find('a').removeAttr('disabled').removeClass('disabled');
                    if ($(FormsEngine.CrossSellErrorDivTag).exists()) {
                        // put the error message in an error div that exists
                        $(FormsEngine.CrossSellErrorDivTag).html(error);
                        $(FormsEngine.CrossSellErrorDivTag).removeClass('hide');
                    }
                    SubmittingForm = false;
                }
            });
        });
    }

    function validateAdditionalQuestions() {
        var PassedValidation = true;
        
        // add all the selected programs from the mc user select area to the array also add show/hide logic for input click
        var firstError = true;
        jQuery(FormsEngine.CrossSellFormTag).find('input[name=crossSellProgramCheckBox]').each(function () {
            var parentli = jQuery(this).parents('li');
            var additionalQuestionsDiv = parentli.find('div[name=cs-additional-questions]');
            if (jQuery(this).is(":checked")) {
                var AdditionalQuestions = $(additionalQuestionsDiv).find(':input');
                for (var qq = 0; qq < AdditionalQuestions.length; qq++) {
                    var passed = AQ_validateRequired(AdditionalQuestions[qq]);
                    if (!passed && firstError) {
                        firstError = false;
                        var offset = $(AdditionalQuestions[qq]).offset();
                        if (offset != undefined && offset != null) {
                            $('html, body').animate({
                                scrollTop: offset.top - 100
                            }, 1000);
                        }
                    }
                    PassedValidation &= passed;
                }
            }
        });

        return PassedValidation;
    }

    //Additional Questions Submit Region
    function AQ_SetEncodedLeadDataForCrossSell() {
        var res = AQ_buildAdditionalQuestionsAnswersForSubmit(); //must rebuild to make sure we only pass questions answered and selected
        //append serialized question/answer array if not null to leaddata then pass encoded to next step
        var theEncodedAdditionalData = FormsEngine.LeadAdditionalDataEncoded;
        var theEncodedData = FormsEngine.LeadDataEncoded;
        if (res != null || res != undefined) {
            var additionalQ = "";
            var extraLeadData = "";
            //first build the string we need for our additional data
            for (var p = 0; p < res.length; p++) {
                var control = AQ_getAdditionalQuestionControlValue(res[p].code)
                var theVal = control.valueKey == null || control.valueKey == undefined ? res[p].answer : control.valueKey;
                additionalQ += res[p].code + "-key=" + theVal + "&";
                extraLeadData += res[p].code + "=" + res[p].answer + "&";
            }
            //get the string and replace , with = and ; with & for querystring
            //by appending -key the values will actually get picked up and used
            additionalQ = additionalQ.substring(0, additionalQ.length - 1);
            extraLeadData = extraLeadData.substring(0, extraLeadData.length - 1);
            theEncodedAdditionalData += encodeURIComponent("&" + additionalQ);
            theEncodedData += encodeURIComponent("&" + extraLeadData);
        }
        FormsEngine.LeadAdditionalDataEncoded = theEncodedAdditionalData;
        FormsEngine.LeadDataEncoded = theEncodedData;
    }

    function AQ_buildAdditionalQuestionsAnswersForSubmit() {
        var additionalQuestionAnswerArray = null;

        jQuery(FormsEngine.CrossSellFormTag).find('input[name=crossSellProgramCheckBox]').each(function () {
            var parentli = jQuery(this).parents('li');
            var additionalQuestionsDiv = parentli.find('div[name=cs-additional-questions]');
            if (jQuery(this).is(":checked")) {
                jQuery(additionalQuestionsDiv).find(':input').each(function () {
                    if (jQuery(this).is(':radio')) {
                        if (jQuery(this).is(':checked')) {
                            additionalQuestionAnswerArray = AQ_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
                        }
                    }
                    else {
                        additionalQuestionAnswerArray = AQ_saveAdditionalQuestions(additionalQuestionAnswerArray, jQuery(this).attr('code'), jQuery(this).val(), false);
                    }
                });
            }
        });

        return additionalQuestionAnswerArray;
    }

    //Gets the control value and value-key if is a me-integration field
    function AQ_getAdditionalQuestionControlValue(controlCode) {

        var dependantField = jQuery(FormsEngine.CrossSellFormTag).find(":input[code='" + controlCode + "']");
        var ControlValue = {};
        ControlValue.value = "";
        ControlValue.valueKey = "";
        ControlValue.requiresKey = jQuery(dependantField).attr("me-filter") != undefined;

        if (jQuery(dependantField).is('select')) {
            ControlValue.value = jQuery(dependantField).val();
            if (ControlValue.requiresKey) {
                ControlValue.valueKey = jQuery(FormsEngine.CrossSellFormTag).find(":input[code='" + controlCode + "']>option:selected").attr('key');
            }
        }
        else if (jQuery(dependantField).is(':radio')) {
            ControlValue.value = jQuery(FormsEngine.CrossSellFormTag).find(":input[code='" + controlCode + "']:checked").val();
            if (ControlValue.requiresKey) {
                ControlValue.valueKey = jQuery(FormsEngine.CrossSellFormTag).find(":input[code='" + controlCode + "']:checked").attr('key');
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

    function AQ_saveAdditionalQuestions(data, controlCode, controlValue, saveArray) {

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
            AQ_setAdditionalQASessionObject(additionalQuestionAnswerArray);

        fe_pushSingleFieldToGTMDataLayer(controlCode, controlValue);

        return additionalQuestionAnswerArray;
    }

    function AQ_setAdditionalQASessionObject(value) {
        fe_getSessionId(function () {
            jQuery.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/Session/SetAdditionalQAObject?FESessionId=" + FormsEngine.FESessionId + "&Value=" + AQ_convertAdditionalQACollectionToString(value),
                success: function (data) {
                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                }
            });
        });
    }
    //end additional question submit region

    //additional question region


    //ajax prep and call to get additional question collection
    function AQ_getAdditionalQuestionCollection() {

        var usProgramTemplateIds = jQuery(FormsEngine.CrossSellFormTag).find('#cs-additional-questions-templates-used').attr('data-program-templates-used');
        //if we still have templates to retrieve make the call otherwise skip this brick.
        if (usProgramTemplateIds.length > 0) {
            fe_getSessionId(function () {
                var arguments = "FeSessionId=" + FormsEngine.FESessionId + "&WizardTemplateId=" + FormsEngine.TemplateId + "&ProgramTemplateIds=" + usProgramTemplateIds + "&RenderingStrategy=" + FormsEngine.RenderingStrategy + "&IsBeta=" + FormsEngine.IsBeta + "&TrackId=" + FormsEngine.TrackId;
                arguments += "&FormFilterValues=" + FormsEngine.LeadDataEncoded + "%2F" + FormsEngine.LeadAdditionalDataEncoded;

                showLoader();
                jQuery.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ServiceBaseURL + "/TemplateManager/GetAdditionalTemplateQuestionCollection?" + arguments,
                    success: function (data) {
                        AQ_injectAdditionalQuestions(data);
                    },
                    error: function (request, error) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                        hideLoader(true);
                    }
                });
            });
        }
    }

    function AQ_convertAdditionalQForTemplateStringToCollection(str) {
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

    function AQ_convertAdditionalQForTemplateCollectionToString(col) {
        var str = "";
        for (var i = 0; i < col.length; i++) {
            str += col[i].TemplateId + "," + col[i].RenderedControl + ";";
        }

        //this removes that last semicolon to make split cleaner on retrieval
        return str.substring(0, str.length - 1);
    }

    //callback for above method to actually inject the questions into the selection page.
    //data is a dictionary of templateid/renderedcontrol pairs
    function AQ_injectAdditionalQuestions(ddata) {

        //add additional questions if needed for each placeholder
        jQuery.each(jQuery(FormsEngine.CrossSellFormTag).find('div[name=cs-additional-questions]'), function (index, obj) {
            //get the li since that has school info
            var parentli = jQuery(this).parents('li');
            AQ_bindAdditionQuestions(parentli, ddata, this);
        });
        
        //if we have a session object bind question answers to whats saved in session
        AQ_setAdditionalQuestionAnswersFromSession();

        AQ_hideAskedAdditionalQuestions();

        hideLoader(true);

        AQ_setHeight();
    }

    //because a collection of objects was not working with jsonp get this data is stored as a delimited string.
    //the below two functions convert it to and from the object we ideally want to work with
    function AQ_convertAdditionalQAStringToCollection(str) {
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

    function AQ_convertAdditionalQACollectionToString(col) {
        var str = "";
        for (var i = 0; i < col.length; i++) {
            str += col[i].code + "," + col[i].answer + ";";
        }

        //this removes that last semicolon to make split cleaner on retrieval
        return str.substring(0, str.length - 1);
    }

    function AQ_getAdditionalQASessionObject(callback) {
        fe_getSessionId(function () {
            jQuery.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/Session/GetAdditionalQAObject?FESessionId=" + FormsEngine.FESessionId,
                success: function (data) {
                    callback(AQ_convertAdditionalQAStringToCollection(data));
                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    callback();
                }
            });
        });
    }

    function AQ_setAdditionalQuestionAnswersFromSession() {
        AQ_getAdditionalQASessionObject(function (data) {
            if (data != undefined && data != null && data != "") {
                for (var i = 0; i < data.length; i++) {
                    jQuery(FormsEngine.CrossSellFormTag).find(':input[code=' + data[i].code + ']').each(function () {
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
    function AQ_updateAdditionalQuestionControlIds(controlsString, campusId) {
        var str = controlsString.replace(new RegExp('<input id=\"', 'g'), '<input id=\"' + campusId + '_');
        str = str.replace(new RegExp('<select id=\"', 'g'), '<select id=\"' + campusId + '_');
        str = str.replace(new RegExp('<label for=\"', 'g'), '<label for=\"' + campusId + '_');
        str = str.replace(new RegExp(' name=\"', 'g'), ' name=\"' + campusId + '_');
        return str;
    }


    //questions already asked will be in the forms engine object. if the code for a question in our form exists in that string, hide it.
    function AQ_hideAskedAdditionalQuestions() {
        jQuery(FormsEngine.CrossSellFormTag).find(':input').each(function () {
            var theCode = jQuery(this).attr('code');
            if (FormsEngine.LeadDataEncoded.indexOf(theCode) > -1) {
                jQuery(FormsEngine.CrossSellFormTag).find("[data-controlcode='" + theCode + "']").hide();
            }
        });
    }

    //Manual required validation for additional fields
    function AQ_validateRequired(control) {
        var hasValue = false;
        var value = "";
        var controlCode = jQuery(control).attr("code");
        var containerVisible = jQuery(FormsEngine.CrossSellFormTag).find("[data-controlcode='" + controlCode + "']").is(":visible");

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

    function AQ_bindAdditionQuestions(parentli, fdata, additionalQuestionsDIV) {

        var selectedTemplateId = parentli.attr('data-templateid');
        //build up this string to be appended with the additional questions using the selected value of the ddl
        var theTemplate = jQuery.grep(fdata, function (n, i) {
            return n.TemplateId == selectedTemplateId;
        });

        jQuery(additionalQuestionsDIV).html('');
        if (theTemplate != null && theTemplate != undefined && theTemplate != "") {
            var theControls = AQ_updateAdditionalQuestionControlIds(decodeURIComponent((theTemplate[0].RenderedControl + '').replace(/\+/g, '%20')), parentli.attr('data-campusid'));
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
                AQ_getAdditionalQASessionObject(function (res) {
                    AQ_saveAdditionalQuestions(res, theCode, theVal, true);
                });

                if (jQuery(this).is(':radio')) {
                    jQuery(this).attr('checked', true);
                    jQuery(FormsEngine.CrossSellFormTag).find(':input[code=' + theCode + '][value=' + theVal + ']').each(function () {
                        jQuery(this).attr('checked', true);
                    });
                }
                else {
                    //on change set the value of call controls with this code to this value.
                    jQuery(FormsEngine.CrossSellFormTag).find(':input[code=' + theCode + ']').each(function () {
                        jQuery(this).val(theVal);
                    });
                }
                //re-validate control when data changes
                AQ_validateRequired(jQuery(this));
            });
        });

        //hide if checkbox is not selected
        var programCheckbox = parentli.find(":input[name='crossSellProgramCheckBox']");
        if (programCheckbox.is(":checked")) {
            jQuery(additionalQuestionsDIV).show();
        }
        else {
            jQuery(additionalQuestionsDIV).hide();
        }
    }

    function AQ_unbindLiClickFromPreCheckedCrossSell() {
        $(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]').each(function () {
            if ($(this).is(":checked")) {
                //unbind li and label clicks
                var parentli = $(this).parents('li');
                $(parentli).unbind("click");
                $(parentli).find('label').unbind("click");
            }
        });
    }



    //end additional question region

    function showLoader() {
        $(FormsEngine.LoaderTag).addClass('loaderOn');
        $(FormsEngine.ModalOverlayTag).show();
        $(FormsEngine.LoaderTag).show();
    }

    function hideLoader(keepOverlayShown) {
        if (!keepOverlayShown) {
            $(FormsEngine.ModalOverlayTag).hide();
        }
        $(FormsEngine.LoaderTag).hide();
    }


    function formatPhoneNumber(input, format) {
        // Strip non-numeric characters
        var digits = input.replace(/\D/g, '');

        // Replace each "X" with the next digit
        var count = 0;
        return format.replace(/X/g, function () {
            return digits.charAt(count++);
        });
    }
        
    //FINISH LOADING THE CROSS SELL...

    // load the cross sell as a modal window popup                 
    $(FormsEngine.CrossSellDivTag).addClass('success');

    $(FormsEngine.ModalOverlayTag).css("display", "").removeClass("hide");
    $("body").addClass("cross-sell-page"); //Added so the Consumer can Style this step independently.

    $('html, body').animate({ scrollTop: 0 }, 0);
    $(FormsEngine.CrossSellDivTag).fadeIn(300);

    // only X programs can fit in the scrollable program list form so this is a catch to correct program list display when no scroll bar
    var numberOfProgramsDisplayed = parseInt(FormsEngine.CrossSellProgramCount) < parseInt(FormsEngine.MaxProgramsToDisplayTotal) ? parseInt(FormsEngine.CrossSellProgramCount) : parseInt(FormsEngine.RSMaxSchoolsToDisplayTotal);
    if (numberOfProgramsDisplayed <= parseInt(FormsEngine.RSMaxProgramsToDisplayWithoutScrolling)) {
        if (numberOfProgramsDisplayed == 1) {
            $(FormsEngine.CrossSellDivTag).find('#crossSellForm').addClass('OneProgram');
        } else {
            $(FormsEngine.CrossSellDivTag).find('#crossSellForm').addClass('MultiplePrograms');
        }
    }

    hideLoader(true);


    function processNorthcentralUniversityTCPA(smartMatchList) {

        var ncIndex = -1;


        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i] == "Northcentral University") {
                ncIndex = i;
            }

        }

        //Northcentral University hardcoded rules

        if (ncIndex > -1) {
            smartMatchList[ncIndex] = "Northcentral University and its National University Affiliates";
        }

    }

    function processLynnTCPA(smartMatchList) {

        var ncIndex = -1;


        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i] == "Lynn University") {
                ncIndex = i;
            }

        }

        //Lynn University hardcoded rules

        if (ncIndex > -1) {
            smartMatchList[ncIndex] = "Lynn University/Kaplan North America, LLC.";
        }

    }


    function processPurdueKaplanTCPA(smartMatchList) {

        var kIndex = -1;
        var pIndex = -1;

        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i] == "Kaplan University") {
                kIndex = i;
            }
            else if (smartMatchList[i] == "Purdue Global") {
                pIndex = i;
            }
        }

        //Kaplan Purdue hardcoded rules
        //Both exist
        if (kIndex > -1 && pIndex > -1) {
            smartMatchList[kIndex] = "Purdue Global/Kaplan North America, LLC.";
            smartMatchList[pIndex] = "";
        }
        else if (kIndex > -1) {
            smartMatchList[kIndex] = "Purdue Global/Kaplan North America, LLC.";
        }
        else if (pIndex > -1) {
            smartMatchList[pIndex] = "Purdue Global/Kaplan North America, LLC.";
        }
    }

        
    //#57225 - RD - Make Cross Sell Blocks clickable
    function setExpressContent() {

        // if there is at least one checkbox checked/selected, enable the Request Info button
        if ($(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:checked').first().exists()) {

            $(FormsEngine.CrossSellErrorDivTag).html('').addClass('hide');
            $(FormsEngine.CrossSellSubmitTag).find('a').removeAttr('disabled', 'disabled').removeClass('disabled');

            // update the school names in the Express Consent
            var schoolNamesString = '';
            var schoolNamesArray = [];

            $(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:checked').each(function () {
                if ($(this).attr('data-hideschoolfromtcpa') != 'true') {
                    schoolNamesArray.push($(this).attr('data-institutionname'));
                }
            });
            processNorthcentralUniversityTCPA(schoolNamesArray);
            processPurdueKaplanTCPA(schoolNamesArray);
            processLynnTCPA(schoolNamesArray);
            jQuery(schoolNamesArray).each(function () {
                if (this != "") {
                    schoolNamesString += this + ', ';
                }
            });

            $('#spnExpressConsentSchools').html(schoolNamesString + 'and ');
        } else { $('#spnExpressConsentSchools').html('the schools I select above, and '); }
    }


    $(FormsEngine.CrossSellFormTag).find('li').click(function (event) {

        $(this).find(":checkbox")[0].click();
        setExpressContent();

    });
    $(FormsEngine.CrossSellFormTag).find('label').click(function (event) {
        $(this).parent().find(":checkbox")[0].click();
    });
    //#57225 - END

    // Limit the number of programs a user can submit to
    $(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]').click( function (event) {
        event.stopPropagation();
        // get the count of checked schools, and if it is greater than or equal to the crossSellAllowedSchoolCount, disable the other select checkboxes
        if ($(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:checked').length >= parseInt(FormsEngine.MaxCrossSellUserSelections)) {
            $(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:not(:checked)').attr('disabled', true).parent().addClass('disable');
            // else, enable all the checkboxes
        } else {
            $(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]').attr('disabled', false).parent().removeClass('disable');
        }

        var parentli = $(this).parents('li');
        var additionalQuestionsDiv = parentli.find('div[name=cs-additional-questions]');
        var AdditionalQuestions = $(additionalQuestionsDiv).find(':input');
        if ($(this).is(":checked")) {
            //unbind click event for li and things that are not the check box.
            if (AdditionalQuestions.length > 0) {
                $(parentli).unbind("click");
                $(parentli).find('label').unbind("click");
            }
            additionalQuestionsDiv.show();
            AQ_setHeight();
            if ($(this).attr('data-showtwoucheckbox') == "True") {
                fe_showUserAgreementForTwoULeadShareSchool($(this).attr('data-institutionname'), $(this).attr('data-programtype'), '#dvCrossSellExpressConsent');
            }
        }
        else {
            //rebind click event for li and things that are not the check box
            if (AdditionalQuestions.length > 0) {
                $(parentli).click( function (event) {

                    $(this).find(":checkbox")[0].click();
                    setExpressContent();

                });
                $(parentli).find('label').click( function (event) { $(this).parent().find(":checkbox")[0].click(); });
            }
            additionalQuestionsDiv.hide();
            AQ_setHeight();
            if ($(this).attr('data-showtwoucheckbox') == "True") {
                fe_removeUserAgreementForTwoULeadShareSchool();
            }
        }

        setExpressContent();

        //LeadId changes for express consent
        try {
            if (typeof LeadiD != 'undefined') {
                LeadiD.formcapture.init();
            }
        }
        catch (ex) { }
    });

    //fix additional question cross sell popup
    function AQ_setHeight() {
        tallest = 0;

        jQuery("#dvCrossSellSchools li").each(function () {

            //reset each list height(for resize window)
            jQuery(this).css("height", "");

            if (jQuery(this).height() > tallest) {
                tallest = jQuery(this).height();
            }

            jQuery(this).css("height", tallest + 'px');

            //only overwrite prev(".odd") 
            jQuery(this).prev("li.odd").css("height", tallest + 'px');

            if (jQuery(this).hasClass("even")) {
                tallest = 0;
            }

        });
    }

    $(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]').on('click', function (event) {
        if ($(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]:checked').first().exists()) {
            $(FormsEngine.CrossSellDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-error');
            $(FormsEngine.CrossSellDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-black');
            $(FormsEngine.CrossSellErrorDivTag).addClass('hide');
            $(FormsEngine.CrossSellErrorDivTag).html('');
            $(FormsEngine.CrossSellSubmitTag).find('a').removeAttr('disabled', 'disabled').removeClass('disabled');
        }
    });


    //Close Cross Sell Popup  (can be the x button in top right or the No Thanks link on bottom right) 
    $(FormsEngine.CrossSellDivTag).find('[name="close-cross-sell-button"]').on('click', function (event) {
        event.preventDefault();
        if (FormsEngine.IsForOptimizelyCrossSell.toLowerCase() == 'true') {
            return;
        }

        // data should have already been stored in session from the first lead submission
        window.location = fe_GetProgramTemplateThankYouPageUrl;
    });
    $(FormsEngine.CrossSellDivTag).find('[name="close-cross-sell-NoThanks"]').on('click', function (event) {
        event.preventDefault();
        if (FormsEngine.IsForOptimizelyCrossSell.toLowerCase() == 'true') {
            return;
        }

        // data should have already been stored in session from the first lead submission
        window.location = fe_GetProgramTemplateThankYouPageUrl();
    });


    //Alternative express consent message (optimizely set)
    if ($(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]').length > 0) {
        if (FormsEngine.AlternativeExpressConsent != undefined && FormsEngine.AlternativeExpressConsent.length > 0) {
            fe_getSessionObject("ProgramTemplateMobileNumbers", function (numberArray) {
                var Phones = "";

                if (numberArray.length > 1) {
                    Phones = formatPhoneNumber(numberArray[0], "XXX.XXX.XXXX") + ' and ' + formatPhoneNumber(numberArray[1], "XXX.XXX.XXXX");
                }
                else if (numberArray.length == 1) {
                    Phones = formatPhoneNumber(numberArray[0], "XXX.XXX.XXXX");
                }

                FormsEngine.AlternativeExpressConsent = FormsEngine.AlternativeExpressConsent.replace('{mobile-number}', Phones);
                FormsEngine.AlternativeExpressConsent = FormsEngine.AlternativeExpressConsent.replace('{school-name}', '<span id="spnExpressConsentSchools">');
                FormsEngine.AlternativeExpressConsent = FormsEngine.AlternativeExpressConsent.replace('{/school-name}', '</span>');

                //change text and label
                var express = $(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]');
                $(express).val(FormsEngine.AlternativeExpressConsent);
                $("label[for='" + $(express).attr('id') + "']").html(FormsEngine.AlternativeExpressConsent);
            });
        }

        var expressC = $(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]');
        //change checkbox to label (hide checkbox and preselect) in cross sell consent UA
        if (FormsEngine.ExpressConsentAsLabel === true) {
            $(expressC).css({ "display": "none" });
            $(expressC).prop('checked', true);
        }

        //LeadId changes for express consent
        try {
            if (typeof LeadiD != 'undefined') {
                LeadiD.formcapture.init();
            }
        }
        catch (ex) { }
    }

    //Track event
    if (FormsEngine.TrackEvent) {
        FormsEngine.TrackEvent('CrossSell');
    }

    function ReplaceTags(text) {

        FormsEngine.CrossSellInstitutionName = FormsEngine.CrossSellInstitutionName == undefined ? "" : FormsEngine.CrossSellInstitutionName;
        FormsEngine.CrossSellProgramName = FormsEngine.CrossSellProgramName == undefined ? "" : FormsEngine.CrossSellProgramName;
        FormsEngine.CrossSellMaxSchoolsText = FormsEngine.CrossSellMaxSchoolsText == undefined ? "" : FormsEngine.CrossSellMaxSchoolsText;

        text = text.replace("{school-name}", FormsEngine.CrossSellInstitutionName);
        text = text.replace("{program-name}", FormsEngine.CrossSellProgramName);
        text = text.replace("{maxschools-text}", FormsEngine.CrossSellMaxSchoolsText);

        return text;
    }

    function centerCrossSell() {

        //horizontal center cross sell popup window
        //fix #63050
        if ($(window).width() <= 980) {
            console.log($(window).width());
            console.log($(".cross-sell-popup").width());
            $(".success").css("left", ($(window).width() - $(".cross-sell-popup").width()) / 2);
        } else if ($(window).width() <= 1180) {
            $(".success").css("left", ($(window).width() - $(".cross-sell-popup").width()) / 3);
        } else {
            $(".success").css("left", ($(window).width() - $(".cross-sell-popup").width()) / 5);
        }
    }

    //Custom message and submessage for x-sell
    if (FormsEngine.CrossSellTopMessage != undefined) {
        $("#CrossSellTopMessage").text(ReplaceTags(FormsEngine.CrossSellTopMessage));
    }
    if (FormsEngine.CrossSellTopSubMessage != undefined) {
        jQuery("#CrossSellTopSubMessage").text(ReplaceTags(FormsEngine.CrossSellTopSubMessage));
    }

    function checkForOptimizelyCrossSellTheme() {

        //Optimizely CrossSell Styling
        if (FormsEngine.Theme == 'CrossSellAlternateLIST' || FormsEngine.Theme == 'CrossSellAlternateBLOCK') {
            $(FormsEngine.CrossSellDivTag).find('.onlinecampusflag img').each(function () { $(this).attr('src', $(this).attr('srcSizeLarge')); });
        }
    }

    function changeImageSizeForCrossSellNoOverlay() {

        if (FormsEngine.Theme == 'CrossSellNoOverlay') {
            $(FormsEngine.CrossSellDivTag).find('.groundcampusflag img, .onlinecampusflag img').each(function () { $(this).attr('src', $(this).attr('srcSizeLarge')); });
        }
    }

    // Execute Cross Sell Submission event
    $(FormsEngine.CrossSellSubmitTag).find('a').on("click", function (event) {
        event.preventDefault();

        //Prevent multiple submissions
        if (SubmittingForm) {
            return;
        }
        SubmittingForm = true;
        $(FormsEngine.CrossSellSubmitTag).addClass('disabled');

        if (FormsEngine.IsForOptimizelyCrossSell.toLowerCase() == 'true') {
            return;
        }

        showLoader();
        //$('html, body').animate({ scrollTop: FormsEngine.WindowHeightHalf - 100 }, 0);

        $(this).attr('disabled', 'disabled').addClass('disabled');

        if ($(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:checked').length < 1) {
            // show an error message if they hit submit without choosing at least one program
            hideLoader(true);
            var errorMessage = '<p>Please select at least one school in order to request information.</p>';
            if ($(FormsEngine.CrossSellErrorDivTag).exists()) {
                // put the error message in an error div that exists
                $(FormsEngine.CrossSellErrorDivTag).html(errorMessage);
                $(FormsEngine.CrossSellErrorDivTag).removeClass('hide');
                SubmittingForm = false;
            }
            return;
        }

        if ($(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]').length > 0) {
            if ($(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]:checked').length < 1) {
                // show an error message if they hit submit without checking the Express Consent box
                hideLoader(true);
                var errorMessage = '<p>You must agree to the acknowledgment below in order to request information.</p>';
                if ($(FormsEngine.CrossSellErrorDivTag).exists()) {
                    // put the error message in an error div that exists
                    $(FormsEngine.CrossSellErrorDivTag).html(errorMessage);
                    $(FormsEngine.CrossSellErrorDivTag).removeClass('hide');
                    SubmittingForm = false;
                }
                $(FormsEngine.CrossSellDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-black');
                $(FormsEngine.CrossSellDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-error');
                return;
            } else {
                // prepare to save the Express Consent text with the lead data
                var expressConsentText = $(FormsEngine.CrossSellDivTag).find('label[for=ckbExpressConsent]').text();
                $(FormsEngine.CrossSellDivTag).find('input[id=ckbExpressConsent]:checked').val(expressConsentText);
                var expressConsentTextEncoded = encodeURIComponent(expressConsentText.replace(/&/g, "amp;"));
                FormsEngine.LeadDataEncoded = FormsEngine.LeadDataEncoded + '%26' + 'CrossSellExpressConsent' + '%3D' + expressConsentTextEncoded;
            }
        }

        var crossSellProgramArray = new Array();

        // add all the selected programs from the cross sell to the array
        $.each($(FormsEngine.CrossSellDivTag).find('input[name=crossSellProgramCheckBox]:checked'), function (index, obj) {
            var programProductId = $(obj).attr('data-programproductid');
            var templateId = $(obj).attr('data-templateid');
            crossSellProgramArray.push(programProductId + '_' + templateId);
        });

        // if there are selected programs and there is a error div on the page, hide it
        $(FormsEngine.CrossSellErrorDivTag).addClass('hide');
        $(FormsEngine.CrossSellErrorDivTag).html('');

        CrossSellSubmitWS(crossSellProgramArray, FormsEngine.LeadDataEncoded);
    });

    $(document).ready(function () {

        checkForOptimizelyCrossSellTheme();
        setExpressContent();
        AQ_getAdditionalQuestionCollection();
        changeImageSizeForCrossSellNoOverlay();

        //Function for Optimizely customization function call if exist.
        fe_OptimizelyCustomization();
        
        setTimeout(centerCrossSell, 100);
        //resize cross-sell when screen resize, fix #63050
        var width = $(window).width();
        var resize_continue = false;

        AQ_unbindLiClickFromPreCheckedCrossSell();

        $(window).resize(function () {
            //only width change makes resize happen
            if ($(this).width() != width) {

                
                width = $(this).width();

                //resize clicksnet only when resize is stopped/completed
                if (resize_continue)
                    clearTimeout(resize_continue);

                resize_continue = setTimeout(function () {
                    centerCrossSell();
                }, 100);

            }

        });

        if (FormsEngine.CrossSellLoaded)
        {
            FormsEngine.CrossSellLoaded();
        }
        $(FormsEngine).trigger("CrossSellLoaded");
    });
})(jQuery);