(function ($) {
    // WizardECMobile_Start.js

    $(document).ready(function () {
        if (typeof Drupal != 'undefined') {

            if (Drupal.settings.pageContent != null) {

                mobile_logo = document.createElement('img');
                mobile_logo.src = Drupal.settings.pageContent.logo;
                $("#mobile_header_image").html(mobile_logo);


                if (Drupal.settings.pageContent.content.field_mobile_text != null) {
                    $("#mobile_header_text").html(Drupal.settings.pageContent.content.field_mobile_text[0].markup);
                }

                if (Drupal.settings.pageContent.content.field_mobile_copy != null) {
                    $("#mobile_copy").html(Drupal.settings.pageContent.content.field_mobile_copy[0].markup);
                    $("#mobile_copy").prepend("<br />");
                    $("#mobile_copy").append("<br /><br />");
                }

                if (Drupal.settings.pageContent.content.field_mobile_success_kit != null) {
                    $("#mobile_success_kit").html(Drupal.settings.pageContent.content.field_mobile_success_kit[0].markup);
                }

                if (Drupal.settings.pageContent.content.field_mobile_disclaimer != null) {

                    $("#mobile_disclaimer").html(Drupal.settings.pageContent.content.field_mobile_disclaimer[0].markup);
                    $("#mobile_disclaimer").prepend("<br />");
                }

                if (Drupal.settings.pageContent.content.copyright != null) {
                    $("#mobile_copyr").html('Drupal.settings.pageContent.content.field_copyright[0].markup');
                }
            }
        }
        FormsEngine.IsMobileForm = true;
        $('input[name="UseDayTimePhone"]').attr('id', 'UseDayTimePhone');

        $(function () {
            $('#mobile_header_image img').data('size', 'big');
        });

        $(window).scroll(function () {
            if ($(document).scrollTop() > 0) {
                if ($('#mobile_header_image img').data('size') == 'big') {

                    $('#mobile_header_image img').data('size', 'small');
                    $('#mobile_header_image img').stop().animate({
                        height: '36px'
                    }, 300);
                    $("#mobile_header_text").css({ "font-size": "12px" });
                }
            }
            else {
                if ($('#mobile_header_image img').data('size') == 'small') {
                    $('#mobile_header_image img').data('size', 'big');
                    $('#mobile_header_image img').stop().animate({
                        height: '53px'
                    }, 300);
                    $("#mobile_header_text").css({ "font-size": "16px" });
                }
            }
        });


        FormsEngine.IsMobileForm = true;
        $('input[name="UseDayTimePhone"]').attr('id', 'UseDayTimePhone');


        //Radio selected
        $(FormsEngine.DefaultFormTag).find(":radio").on("click", function () {
            var controls = $(":radio[code='" + $(this).attr("code") + "']");
            $(controls).each(function () {
                $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").removeClass("check-select");
            });
            $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").addClass("check-select");
        });

        //precheck the ones selected
        $(FormsEngine.DefaultFormTag).find(":radio:checked").each(function () {
            var controls = $(":radio[code='" + $(this).attr("code") + "']");
            $(controls).each(function () {
                $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").removeClass("check-select");
            });
            $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").addClass("check-select");
        });

        //Dynamic additional controls
        $(FormsEngine).on("OnAdditionalQuestionsAdded", function () {
            $("#Step" + FormsEngine.StepDynamicQuestions).find(":radio").on("click", function () {
                var controls = $(":radio[code='" + $(this).attr("code") + "']");
                $(controls).each(function () {
                    $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").removeClass("check-select");
                });
                $(FormsEngine.DefaultFormTag).find("label[for='" + $(this).attr('id') + "']").addClass("check-select");
            });
        });

        // Constants
        FormsEngine.SubmitButton = "#wizard-form-submit-button";
        FormsEngine.BackButton = "#form-navback-button";

        // Internals variables
        var isStepBack = [];

        // needed for the data model retrieved for SchoolSelection page
        FormsEngine.SplitCampusTypeInResults = true;
        fe_saveWorkflowData(function () { }); // update the SessionDTO
        //Copy Phone
        $(FormsEngine.DefaultFormTag).find(":input[id=UseDayTimePhone]").click(function (event) {
            event.preventDefault ? event.preventDefault() : event.returnValue = false;
            if ($(FormsEngine.DefaultFormTag).find(":input[id=UseDayTimePhone]")) {
                $(':input[code=Alternate_Phone]').val($(':input[code=Phone]').val());
            }
        });

        var useClickToCall = GetQueryStringParam('useclicktocall');
        var phoneToUse = GetQueryStringParam('phonenumber');

        if (useClickToCall) {
            $("#clickToCallDiv").show();
            if (phoneToUse.match("^[0-9]{10}")) {
                $("#clickToCallLink").attr("href", "tel:" + phoneToUse);
            }
        }

    });
    function GetQueryStringParam(name) {
        var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (!results) {
            return 0;
        }
        return results[1] || 0;
    };


    if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']")
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "")
          ) {
        $("div[data-controlcode='K12']").hide();
    }
})(jQuery);
