(function ($) {
    // WizardCallCenterWhiteLabel_Start.js
    
   
    $(document).ready(function () {

        // Constants
        FormsEngine.SubmitButton = "#wizard-form-submit-button";
        FormsEngine.BackButton = "#form-navback-button";
        FormsEngine.ProgramCounterTag = FormsEngine.ProgramCounterTag || '#WizardStepContainer #ProgramMatches';
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        FormsEngine.UseProgramCounter = true;

        // needed for the data model retrieved for SchoolSelection page
        FormsEngine.SplitCampusTypeInResults = true;
        fe_saveWorkflowData(function () { }); // update the SessionDTO
 
        // set the program count from the start
        var ProgramCount = jQuery(FormsEngine.ProgramCounterTag).html();
        if (ProgramCount == undefined || ProgramCount == null || ProgramCount == '' || ProgramCount == 0) {
            fe_wiz_updateMatchCount();
        }
         
        fe_wiz_showSelectAllButtons();

        fe_wiz_focusOnNextFocusable('null');

        $(FormsEngine.DefaultFormTag).keydown(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) {
                fe_wiz_keyTabAndEnterEvents(e);
            }
        });
        
        //NextStep on Enter and tab management
        $(document).keypress(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        $(document).keydown(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        $(FormsEngine.DefaultFormTag).keypress(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });
                

        if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']")
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "")
              ) {
            $("div[data-controlcode='K12']").hide();
        }
    });

})(jQuery);