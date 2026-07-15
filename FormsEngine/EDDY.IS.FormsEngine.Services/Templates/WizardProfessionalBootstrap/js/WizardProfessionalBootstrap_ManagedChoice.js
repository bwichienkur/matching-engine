(function ($) {
    // WizardProfessional_ManagedChoice.js

    // Constants

    // Internals variables

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

        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#wizard-form-submit-button"));
        }

        if (FormsEngine.ClickableUSSchool) {
            initializeUSClickableBoxes();
        }

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