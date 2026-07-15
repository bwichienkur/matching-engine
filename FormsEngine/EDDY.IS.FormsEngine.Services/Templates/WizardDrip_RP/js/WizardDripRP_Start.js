(function ($) {


    // WizardDrip_Start.js
    function copyPhoneSplitsToHidden(code) {
        var concatenatedPhone = $(FormsEngine.DefaultFormTag).find(':input[name="' + code + '_areacode"]').val() + $(FormsEngine.DefaultFormTag).find(':input[name="' + code + '_prefix"]').val() + $(FormsEngine.DefaultFormTag).find(':input[name="' + code + '_suffix"]').val();
        if (concatenatedPhone != undefined && concatenatedPhone != null && concatenatedPhone.toString() != "NaN") {
            concatenatedPhone = concatenatedPhone.replace(' ', '');
            $(FormsEngine.DefaultFormTag).find('input[code="' + code + '"]').val(concatenatedPhone);
            //trigger original Phone and Alter_Phone blur event
            if (concatenatedPhone.length >= 10) {
                $(FormsEngine.DefaultFormTag).find('input[code="' + code + '"]').trigger('blur');
            }
        }
    }

    function recoverSplitPhoneFields() {
        //saved phone/alter_phone number needs to reload into 3 fields
        var phone = $(FormsEngine.DefaultFormTag).find(":input[code='Phone']").val();
        var alternate_phone = $(FormsEngine.DefaultFormTag).find(":input[code='Alternate_Phone']").val();
        if (phone != "" && phone != undefined) {
            $(FormsEngine.DefaultFormTag).find(':input[name="Phone_areacode"]').val(phone.substring(0, 3));
            $(FormsEngine.DefaultFormTag).find(':input[name="Phone_prefix"]').val(phone.substring(3, 6));
            $(FormsEngine.DefaultFormTag).find(':input[name="Phone_suffix"]').val(phone.substring(6, 10));
        }
        if (alternate_phone != "" && alternate_phone != undefined) {
            $(FormsEngine.DefaultFormTag).find(':input[name="Alternate_Phone_areacode"]').val(alternate_phone.substring(0, 3));
            $(FormsEngine.DefaultFormTag).find(':input[name="Alternate_Phone_prefix"]').val(alternate_phone.substring(3, 6));
            $(FormsEngine.DefaultFormTag).find(':input[name="Alternate_Phone_suffix"]').val(alternate_phone.substring(6, 10));
        }
    }

    function phoneSplitFieldKeyUpEvent(c, code) {
        copyPhoneSplitsToHidden(code);
        if ($(c).val().length == $(c).attr('maxlength')) {
            var currentTab = $(c).attr('data-phone-tab-number');
            if (currentTab < 3) {
                $(FormsEngine.DefaultFormTag).find('input[name^="' + code + '"][data-phone-tab-number=' + (parseInt(currentTab) + parseInt(1)) + ']').focus();
            }
        }
    }

    function replaceDefaultOptionFields() {
        $(FormsEngine.DefaultFormTag).find('select[name="Prefix"] option:first').text("--PREFIX--");
    }


    function copyPhoneClickEvent(control) {
        var checkbox = $(FormsEngine.DefaultFormTag).find('input[name="UseDayTimePhone"]');
        var isChecked = $(checkbox).is(':checked');

        if (isChecked) {
            var sourceCode = 'Phone';
            var targetCode = 'Alternate_Phone';
            $(FormsEngine.DefaultFormTag).find(':input[name="' + targetCode + '_areacode"]').val($(FormsEngine.DefaultFormTag).find(':input[name="' + sourceCode + '_areacode"]').val());
            $(FormsEngine.DefaultFormTag).find(':input[name="' + targetCode + '_prefix"]').val($(FormsEngine.DefaultFormTag).find(':input[name="' + sourceCode + '_prefix"]').val());
            $(FormsEngine.DefaultFormTag).find(':input[name="' + targetCode + '_suffix"]').val($(FormsEngine.DefaultFormTag).find(':input[name="' + sourceCode + '_suffix"]').val());
            copyPhoneSplitsToHidden(targetCode);

            if ($("input[name=Phone]").exists() && $("input[name=Alternate_Phone]").exists()) {
                var start = $("input[name=Phone]").val();
                $("input[name=Alternate_Phone]").val(start);
            }
        }
    }


    $(document).ready(function () {

        // Constants
        FormsEngine.SubmitButton = "#wizard-form-submit-button";
        FormsEngine.BackButton = "#form-navback-button";
        FormsEngine.ProgramCounterTag = FormsEngine.ProgramCounterTag || '#WizardStepContainer #ProgramMatches';
        FormsEngine.RecoverSplitPhoneFields = FormsEngine.RecoverSplitPhoneFields || function () { recoverSplitPhoneFields(); };
        FormsEngine.ReplaceDefaultOptionFields = FormsEngine.ReplaceDefaultOptionFields || function () { replaceDefaultOptionFields(); };
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        FormsEngine.UseProgramCounter = true;

        FormsEngine.SubmitButtonTop = "#next-top";
        FormsEngine.BackButtonTop = "#prev-top";

        FormsEngine.SubmitButtonTop2 = "#next-top2";
        FormsEngine.BackButtonTop2 = "#prev-top2";

        $(FormsEngine.SubmitButtonTop).click(function (event) {
            $(FormsEngine.SubmitButton).click();
        });
        $(FormsEngine.BackButtonTop).click(function (event) {
            $(FormsEngine.BackButton).click();
        });

        $(FormsEngine.SubmitButtonTop2).click(function (event) {
            $(FormsEngine.SubmitButton).click();
        });
        $(FormsEngine.BackButtonTop2).click(function (event) {
            $(FormsEngine.BackButton).click();
        });

        // Internals variables
        var isStepBack = [];

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

        //auto step forward not working for backward steps
        $(FormsEngine.BackButton).click(function () {
            FormsEngine.BackButtonClicked = true;
            var step = $('.steps:visible').attr("data-step");
            isStepBack[step - 1] = true;
        });

        // auto tab phone fields
        $(FormsEngine.DefaultFormTag).find('input[name="Phone_areacode"], input[name="Phone_prefix"], input[name="Phone_suffix"]').keyup(function () {
            phoneSplitFieldKeyUpEvent(this, 'Phone');
        });
        $(FormsEngine.DefaultFormTag).find('input[name="Alternate_Phone_areacode"], input[name="Alternate_Phone_prefix"], input[name="Alternate_Phone_suffix"]').keyup(function () {
            phoneSplitFieldKeyUpEvent(this, 'Alternate_Phone');
        });

        // copy phone checkbox
        $(FormsEngine.DefaultFormTag).find('label[for="UseDayTimePhone"]').click(function (e) {
            e.preventDefault();
            var checkbox = $(FormsEngine.DefaultFormTag).find('input[name="UseDayTimePhone"]');
            var isChecked = $(checkbox).is(':checked');
            if (isChecked) {
                $(checkbox).checked = false;
                $(checkbox).removeAttr('checked');
                $(checkbox).trigger('change');
            } else {
                $(checkbox).checked = true;
                $(checkbox).attr({ 'checked': 'checked' });
                $(checkbox).trigger('change');
            }
        });

        $(FormsEngine.DefaultFormTag).find('input[name="UseDayTimePhone"]').change(function (e) {
            e.preventDefault();
            copyPhoneClickEvent(this);
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

        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#wizard-form-submit-button"));
        }

    });

})(jQuery);