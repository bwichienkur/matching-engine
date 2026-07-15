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
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').on('click', function (event) {
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

    }); //end of document.ready

})(jQuery);