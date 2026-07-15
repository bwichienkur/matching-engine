(function ($) {
    // WizardProfessional_ManagedChoice.js

    // Constants

    // Internals variables
    FormsEngine.CurrentSchoolStep = 1;


    function getMetaData() {
        //Get the resource meta data texts.
        if (FormsEngine.ResourceData == undefined) {
            fe_getResourceMetaDataTextForKey('JS.WIZARD.USERAGREEMENT_UNIFIED,JS.WIZARD.JOBSUSERAGREEMENT', function (data) {

                FormsEngine.ResourceData = data;
                if (FormsEngine.WizardAlternativeExpressConsent != undefined && FormsEngine.WizardAlternativeExpressConsent.length > 10) {
                    FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'] = FormsEngine.WizardAlternativeExpressConsent;
                }
                mobileUpdateExpressConsent();
            });
        }
        else {
            if (FormsEngine.WizardAlternativeExpressConsent != undefined && FormsEngine.WizardAlternativeExpressConsent.length > 10) {
                FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'] = FormsEngine.WizardAlternativeExpressConsent;
            }
            mobileUpdateExpressConsent();
        }
    }

    function mobileUpdateExpressConsent() {
        var expressConsent = FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'];

        var phoneFormatted = '';

        if (FormsEngine.MobilePhones != undefined) {
            var PhoneList = [];
            for (var i = 0; i < FormsEngine.MobilePhones.length; i++) {
                PhoneList.push(fe_formatPhone(FormsEngine.MobilePhones[i]));
            }
            phoneFormatted = PhoneList.join(' and ');
            phoneFormatted = (phoneFormatted.length > 0 ? ' ' + phoneFormatted : phoneFormatted);

        }


        jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find(".schoolExpressConsentMessage").each(function () {
            var schoolNamesString = jQuery(this).attr('data-institution-name');
            var controlID = jQuery(this).attr('for');
            var hasCustomTCPA = jQuery(this).html();

            if (hasCustomTCPA) {
                expressConsent = jQuery(this).html().replace(/&nbsp;/g, ' ')
                    .replace(/&lt;/g, '<')
                    .replace(/&gt;/g, '>')
                    .replace(/&amp;/g, '&');

            } else {
                expressConsent = FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'];
            }
            //Tag replacements
            //Mobile
            var xC = fe_replaceTag(expressConsent, '{mobile-number}', phoneFormatted);

            //US Schools
            if (schoolNamesString != undefined && schoolNamesString.length > 0) {
                xC = fe_replaceTag(xC, '{school}', schoolNamesString);
            }
            else {
                xC = fe_replaceTag(xC, '{school}', '');
            }

            xC = fe_replaceTag(xC, '{thirdpartyschool}', '');
            xC = fe_replaceTag(xC, '{thirdpartytype}', ' ');

            //label
            jQuery('label[for="' + controlID + '"]').html(xC);

            //value
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('#' + controlID).val(xC);
        });

        //LeadId changes for express consent, legacy recapture requested by leadid.
        try {
            if (typeof LeadiD != 'undefined') {
                LeadiD.formcapture.init();
            }
        }
        catch (ex) {
            //cmnt
        }
    }

    function getExpressConsentTextEncoded(campusid) {
        var expressConsentText = $(".schoolExpressConsentMessage[data-campusid=" + campusid + "]").text();
        var expressConsentTextEncoded = encodeURIComponent(expressConsentText.replace(/&/g, "amp;"));
        return '%26' + 'SchoolSelectionExpressConsent' + '%3D' + expressConsentTextEncoded;
    }

    function rebuildAdditionalDataEncoded(campusid) {
        var res = fe_wiz_buildAdditionalQuestionsAnswersForSubmitByCampus(campusid); //must rebuild to make sure we only pass questions answered and selected
        //append serialized question/answer array if not null to leaddata then pass encoded to next step
        var theEncodedAdditionalData = FormsEngine.LeadAdditionalDataEncoded;
        var theEncodedData = FormsEngine.LeadDataEncoded;

        if (res !== null && res !== undefined) {
            var additionalQ = "";
            var extraLeadData = "";
            var anyExtraData = false;
            //first build the string we need for our additional data
            for (var p = 0; p < res.length; p++) {
                var control = fe_wiz_mc_getAdditionalQuestionControlValue(res[p].code)
                var theVal = control.valueKey === null || control.valueKey === undefined ? res[p].answer : control.valueKey;
                if (theEncodedData && theEncodedData.indexOf("%26" + res[p].code + "%3") === -1) {
                    additionalQ += res[p].code + "-key=" + theVal + "&";
                    extraLeadData += res[p].code + "=" + res[p].answer + "&";
                    anyExtraData = true;
                }
            }
            if (anyExtraData) {
                //get the string and replace , with = and ; with & for querystring
                //by appending -key the values will actually get picked up and used
                additionalQ = additionalQ.substring(0, additionalQ.length - 1);
                extraLeadData = extraLeadData.substring(0, extraLeadData.length - 1);
                theEncodedAdditionalData += encodeURIComponent("&" + additionalQ);
                theEncodedData += encodeURIComponent("&" + extraLeadData);
            }
        }

        if (theEncodedData && theEncodedData.indexOf("xxTrustedFormToken_MC") === -1) {
            var tokenField = $('#eddynexusform-wizard-managedchoice').find(":input[code='xxTrustedFormToken_MC']");
            if (tokenField.length > 0) {
                theEncodedData += encodeURIComponent("&xxTrustedFormToken_MC=" + tokenField.val());
            }
        }
        if (theEncodedData && theEncodedData.indexOf("xxTrustedFormCertUrl_MC") === -1) {
            var certUrlField = $('#eddynexusform-wizard-managedchoice').find(":input[code='xxTrustedFormCertUrl_MC']");
            if (certUrlField.length > 0) {
                theEncodedData += encodeURIComponent("&xxTrustedFormCertUrl_MC=" + certUrlField.val());
            }
        }

        FormsEngine.LeadAdditionalDataEncoded = theEncodedAdditionalData;
        FormsEngine.LeadDataEncoded = theEncodedData;
    }

    function submitSingleSchoolSelection(button) {
        var campusid = $(button).attr("data-campusid");
        var nextStep = parseInt($(button).attr("data-stepnumber")) + 1;

        if (validateSchool(campusid)) {
            var mcUserSelectProgramArray = [];
            var selectedProgram = $('option[data-campusid= ' + campusid + ']:selected');
            var programProductId = $(selectedProgram).attr('data-programproductid');
            var templateId = $(selectedProgram).attr('data-templateid');
            templateId = (typeof templateId === 'undefined' || templateId === null || templateId === "") ? 0 : templateId;
            mcUserSelectProgramArray.push(programProductId + '_' + templateId);
            rebuildAdditionalDataEncoded(campusid);
            fe_mc_UserSelectSubmitWS(mcUserSelectProgramArray, FormsEngine.LeadDataEncoded + getExpressConsentTextEncoded(campusid), false,
                function () {
                    FormsEngine.SchoolSelectionSubmittedCount++;
                    FormsEngine.AnySchoolSubmitted = true;
                    FormsEngine.LastSchoolWasSubmitted = true;
                    showSchoolStep(nextStep);
                }
            );
        }
    }

    function validateSchool(campusid) {
        if (!$(".schoolExpressConsentCheckbox[data-campusid=" + campusid + "]").is(':checked')) {
            $(".schoolExpressConsentError[data-campusid=" + campusid + "]").show();
            return false;
        }
        else {
            $(".schoolExpressConsentError[data-campusid=" + campusid + "]").hide();
            return validateAdditionalQuestions(campusid);
        }
    }

    function validateAdditionalQuestions(campusid) {
        var additionalQuestionBlock = $("div[name=mc-SM-additional-questions][data-campusid=" + campusid + "]");
        var passedValidation = true;

        //Validate inputs
        if ($(additionalQuestionBlock).exists()) {
            var AdditionalQuestions = $(additionalQuestionBlock).find(':input');
            for (var qq = 0; qq < AdditionalQuestions.length; qq++) {
                var passed = fe_wiz_ss_validateRequired(AdditionalQuestions[qq]);
                passedValidation &= passed;
            }
        }

        return passedValidation;
    }


    function showSchoolStep(schoolStep) {

        var GoThankYou = (FormsEngine.AnySchoolSubmitted && schoolStep > FormsEngine.SchoolSelectionTotalSchools)
            || FormsEngine.SchoolSelectionSubmittedCount >= parseInt(FormsEngine.MaxManagedChoiceUserSelections);

        var GoNoMatch = schoolStep > FormsEngine.SchoolSelectionTotalSchools;

        //End of chain go to ThankYou
        if (GoThankYou) {
            FormsEngine.LoadWorkflowStep('THANKYOU', '');
            return;
        }

        if (GoNoMatch) {
            FormsEngine.LoadWorkflowStep('NOMATCH', '');
            return;
        }

        if (schoolStep > 1) {
            //Hide SmartMatch after the first submit or skip
            if ($('#mc-SM-Area').is(':visible')) {
                $('#mc-SM-Area').hide();
                $('#ProgramWizardMessage').hide();
                $('.eddy-form-wizard-footer').hide();
            }
            if ($('#ProgramWizardMessage').is(':visible')) {
                $('#ProgramWizardMessage').hide();
            }

            if ($('#DaisyHeader').is(':visible')) {
                $('#DaisyHeader').hide();
            }

            //Generic message
            if (FormsEngine.LastSchoolWasSubmitted) {
                $('#mc-US-GenericMessage').html("Your request has been submitted.");
                FormsEngine.LastSchoolWasSubmitted = false;
            } else {
                $('#mc-US-GenericMessage').html("Didn't like the last school?");
            }
        }

        //Hide All
        $("[id^=SchoolContainer]").hide();
        $("[id^=SchoolDisclaimer]").hide();
        //Show Next
        $("#SchoolContainer" + schoolStep).show();
        $("#SchoolDisclaimer" + schoolStep).show();
        //Trigger differential additional questions logic
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('select[name="mc-us-program-ddl"]').trigger('change');

        var HideNext = FormsEngine.AnySchoolSubmitted === false && schoolStep === FormsEngine.SchoolSelectionTotalSchools;

        fe_consolelog("HideNext = " + HideNext + "\nFormsEngine.SchoolSelectionSubmittedCount = " + FormsEngine.SchoolSelectionSubmittedCount + "\nFormsEngine.MaxManagedChoiceUserSelections = " + FormsEngine.MaxManagedChoiceUserSelections);
        //Second to last or Max allowed and school not submitted

        $(".form-school-next-button").show();

        //if (HideNext) {
        //    $(".form-school-next-button").hide();
        //}
        //else {
        //    $(".form-school-next-button").show();
        //}

        try {
            $("#DaisyHeader").html(FormsEngine.DaisyHeader.replace("@@SCHOOL@@", schoolStep));
        }
        catch (ex) {
            //cmnt
        }
        //Event for external widgets
        var institutionid = $("#SchoolContainer" + schoolStep).find(".btn-choose-school").attr("data-institutionid");
        $(FormsEngine).trigger("OnDaisyChainInstitutionLoaded", institutionid);
    }

    $(document).ready(function () {
        FormsEngine.SchoolSelectionSubmittedCount = 0;
        showSchoolStep(1);
        FormsEngine.WorkflowManagedChoiceDivTag = FormsEngine.WorkflowManagedChoiceDivTag || $('#eddy-form-wizard-managedchoice-container');

        FormsEngine.LimitNumberOfUserSelections = true;

        //AN: Progressbar on managed choice 99% TBD
        fe_setProgressBar(99);

        // user select checkboxes click event
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').on('change', function (event) {
            // Limit the number of programs a user can submit to
            fe_mc_limitUserSelectionCount();

            // if there is at least one checkbox checked/selected, enable the Request Info button
            if ($(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').first().exists()) {
                fe_mc_removeErrorMsgForSubmit();
                fe_mc_removeExpressConsentErrorMsg();
                fe_mc_updateExpressConsent();
            }
            else {
                fe_mc_updateExpressConsent();
            }
        });

        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#wizard-form-submit-button"));
        }

        if (FormsEngine.ClickableUSSchool) {
            initializeUSClickableBoxes();
        }

        //Express consent
        getMetaData();


        $("[name='managechoice-form-school-submit-button']").click(function () {
            submitSingleSchoolSelection(this);
        });

        $("[name='managechoice-form-school-next-button']").click(function () {
            showSchoolStep(parseInt($(this).attr("data-stepnumber")) + 1);
        });
    }); //end of document.ready

})(jQuery);

function initializeUSClickableBoxes() {
    jQuery(".eddy-form-wizard-container .mc-US-Area .mc-us-school").addClass("clickable");

    jQuery(".mc-us-school div.field-holder input:checkbox:checked").closest(".mc-us-school").addClass("mc-us-schoolcontainer-selected");
    jQuery(".mc-us-school div.field-holder input:checkbox").not(":checked").closest(".mc-us-school").removeClass("mc-us-schoolcontainer-selected");

    jQuery(".mc-us-school div.field-holder").find("div.row:first div.school-name").append('<img src="' +
        FormsEngine.ServiceBaseURL + '/Templates/Common/images/info.png" class="mc-us-more-info">');

    jQuery(document).on("click", ".school-info-container i, .mc-us-school select, .us-program-popup-link, .mc-us-more-info, div[name='mc-SM-additional-questions']", function (e) {
        e.stopImmediatePropagation();
    })

    jQuery(document).on("click", ".mc-us-school:not(.disable)", function (e) {
        if (jQuery(this).find(".fa.checkbox-icon.fa-square-o").length > 0) {
            jQuery(this).addClass("mc-us-schoolcontainer-selected");
            jQuery(this).find(".fa.checkbox-icon").removeClass("fa-square-o").addClass("fa-check-square-o");
            jQuery(this).find("input:checkbox").prop("checked", true).trigger("change");
        }
        else {
            jQuery(this).removeClass("mc-us-schoolcontainer-selected");
            jQuery(this).find(".fa.checkbox-icon").removeClass("fa-check-square-o").addClass("fa-square-o");
            jQuery(this).find("input:checkbox").prop("checked", false).trigger("change");

        }
    })

    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).on('click', '.mc-us-more-info', fe_mc_showUSPopup);
}