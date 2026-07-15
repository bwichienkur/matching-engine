(function ($) {
    // WizardProfessional_Start.js



    $(document).ready(function () {

        // Constants
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        
        // Internals variables


        fe_wiz_focusOnNextFocusable('null');

        $(FormsEngine.DefaultFormTag).keydown(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) {
                fe_wiz_keyTabAndEnterEvents(e);
            }
        });

        //NextStep on Enter and tab management
        jQuery(document).keypress(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        jQuery(document).keydown(function (e) {
            if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        jQuery(FormsEngine.DefaultFormTag).keypress(function (e) {
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