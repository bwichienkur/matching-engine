(function ($) {
    // WizardProfessional_ManagedChoice.js

    // Constants

    // Internals variables

    //Support for precheck

    function getMetaData() {
        //Get the resource meta data texts.
        if (FormsEngine.ResourceData == undefined) {
            fe_getResourceMetaDataTextForKey('JS.WIZARD.USERAGREEMENT_UNIFIED,JS.WIZARD.JOBSUSERAGREEMENT', function (data) {

                FormsEngine.ResourceData = data;
                if (FormsEngine.WizardAlternativeExpressConsent != undefined && FormsEngine.WizardAlternativeExpressConsent.length > 10) {
                    FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'] = FormsEngine.WizardAlternativeExpressConsent;
                }
                fe_mc_updateExpressConsent();
                mobileUpdateExpressConsent();
            });
        }
        else {
            if (FormsEngine.WizardAlternativeExpressConsent != undefined && FormsEngine.WizardAlternativeExpressConsent.length > 10) {
                FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'] = FormsEngine.WizardAlternativeExpressConsent;
            }
            fe_mc_updateExpressConsent();
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
            console.log("custom tcpa - " + hasCustomTCPA + " |");

            if (hasCustomTCPA.trim() !== "") {
                expressConsent = jQuery(this).html();
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
            jQuery('label[for="' + controlID + '"]').text(xC);

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

    function initManagedChoice() {
        var anyChecked = false;
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').each(function () {
            anyChecked = true;
            //check/uncheck
            var icon = $(this).next('.checkbox-icon');
            if ($(this).is(":checked")) {
                $(icon).removeClass("fa-square-o").addClass("fa-check-square-o");
            } else {
                $(icon).removeClass("fa-check-square-o").addClass("fa-square-o");
            }
        });
        
        if (anyChecked) {
            fe_mc_limitUserSelectionCount();
            fe_mc_removeErrorMsgForSubmit();
            fe_mc_removeExpressConsentErrorMsg();
            getMetaData();
        }
    }

    $(document).ready(function () {

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

        //click the checkbox when checkbox-icon click
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('.checkbox-icon').on("click", function () {
            $(this).prev().click();
            if ($(this).prev().is(":checked")) {
                $(this).removeClass("fa-square-o").addClass("fa-check-square-o");
            } else {
                $(this).removeClass("fa-check-square-o").addClass("fa-square-o");

            }

        });
        //pre-select support
        initManagedChoice();

        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#wizard-form-submit-button"));
        }

    }); //end of document.ready

})(jQuery);