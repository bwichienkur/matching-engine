(function ($) {
    // WizardECMobile_ManagedChoice.js

    function limitUserSelectionCount() {
        if (FormsEngine.LimitNumberOfUserSelections != undefined && FormsEngine.LimitNumberOfUserSelections) {
            parseInt(FormsEngine.MaxManagedChoiceUserSelections)

            var selectedSchools = 0;

            $(".btnChooseSchool").each(function () {
                if ($(this).css("display") == "none") {
                    selectedSchools++;
                } 
            });

            if (selectedSchools >= parseInt(FormsEngine.MaxManagedChoiceUserSelections)) {
                $(".btnChooseSchool").unbind("click");
                $(".btnChooseSchool").addClass("btnChooseSchoolDisabled");
                $(".btnChooseSchool").removeClass("btnChooseSchool");
                $(".programSelectList").prop('disabled', 'disabled');
                $(".ExpressConsentOptInSchoolSelection").hide();
            }
        }
    }

    function displayInitialTabs(OnlineCampusTabs, GroundTab, OnlineTab, USGroundSchools, USOnlineSchools) {
        if ($(USOnlineSchools).length > 0) { $(OnlineTab).show(); };
        if ($(USGroundSchools).length > 0) { $(GroundTab).show(); };

        if ($(USOnlineSchools).length > 0) {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(USGroundSchools, USOnlineSchools);
            } else {
                switchToOnlineTab(USGroundSchools, USOnlineSchools);
            }
        } else {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(USGroundSchools, USOnlineSchools);
            } else {
                $(OnlineCampusTabs).hide();
            }
        }
    }

    function switchToOnlineTab(USGroundSchools, USOnlineSchools) {
        $("#OnlineButton").addClass("tabsOnlineOn");
        $("#OnlineButton").removeClass("tabsOnlineOff");

        $("#CampusButton").addClass("tabsCampusOff");
        $("#CampusButton").removeClass("tabsCampusOn");

        $(USGroundSchools).css("display", "none");
        $(USOnlineSchools).css("display", "block");
    }

    function switchToGroundTab(USGroundSchools, USOnlineSchools) {
        $("#OnlineButton").addClass("tabsOnlineOff");
        $("#OnlineButton").removeClass("tabsOnlineOn");

        $("#CampusButton").addClass("tabsCampusOn");
        $("#CampusButton").removeClass("tabsCampusOff");

        $(USOnlineSchools).css("display", "none");
        $(USGroundSchools).css("display", "block");
    }

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
        catch (ex) { }
    }

    function submitSingleSchoolSelection(button) {
        var campusid = $(button).attr("data-campusid");

        if (validateChooseSchool(campusid)) {

            var expressConsentText = $(".schoolExpressConsentMessage[data-campusid=" + campusid + "]").html();
            var expressConsentTextEncoded = encodeURIComponent(expressConsentText.replace(/&/g, "amp;"));
            FormsEngine.LeadDataEncoded = FormsEngine.LeadDataEncoded + '%26' + 'SchoolSelectionExpressConsent' + '%3D' + expressConsentTextEncoded;

            var mcUserSelectProgramArray = [];
            var selectedProgram = $('option[data-campusid= ' + campusid + ']:selected');
            var programProductId = $(selectedProgram).attr('data-programproductid');
            var templateId = $(selectedProgram).attr('data-templateid');
            templateId = (typeof templateId == 'undefined' || templateId == null || templateId == "") ? 0 : templateId;
            mcUserSelectProgramArray.push(programProductId + '_' + templateId);

            fe_mc_UserSelectSubmitWS(mcUserSelectProgramArray, FormsEngine.LeadDataEncoded, false);

            $(".btnChooseSchool[data-campusid=" + campusid + "]").hide();
            $(".programSelection[data-campusid=" + campusid + "]").hide();
            $(".ExpressConsentOptInSchoolSelection[data-campusid=" + campusid + "]").hide();

            $(".programDescLnk[data-campusid=" + campusid + "]").text(selectedProgram.text());

            $(".btnThankYou[data-campusid=" + campusid + "]").show();
            $(".programDescLnk[data-campusid=" + campusid + "]").show();

            limitUserSelectionCount();

            //Confirmed matches link (thank you)
            $("#ConfirmedMatchesBotom").show();
            $("#ConfirmedMatchesTop").show();

        }
    }

    function validateChooseSchool(campusid) {
        if (!$(".schoolExpressConsentCheckbox[data-campusid=" + campusid + "]").is(':checked')) {
            $(".schoolExpressConsentError[data-campusid=" + campusid + "]").show();
            return false;
        }
        else {
            $(".schoolExpressConsentError[data-campusid=" + campusid + "]").hide();
            return true;
        }
    }

    $(document).ready(function () {


        if (typeof Drupal != 'undefined') { 

            if (Drupal.settings.pageContent != null) {

                mobile_logo = document.createElement('img');
                mobile_logo.src = Drupal.settings.pageContent.logo;
                $("#mobile_header_image").html(mobile_logo);


                if (Drupal.settings.pageContent.content.field_mobile_text != null) {
                    $("#mobile_header_text").html(Drupal.settings.pageContent.content.field_mobile_text[0].markup);
                }

                if (Drupal.settings.pageContent.content.copyright != null) {
                    $("#mobile_copyr").html('test');
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

        FormsEngine.WorkflowManagedChoiceDivTag = FormsEngine.WorkflowManagedChoiceDivTag || $('#eddy-form-wizard-managedchoice-container');
        //FormsEngine.MoveLoader = FormsEngine.MoveLoader || function () { moveLoader(); };

        // needed for the data model retrieved for SchoolSelection page
        FormsEngine.SplitCampusTypeInResults = true;
        fe_saveWorkflowData(function () { }); // update the SessionDTO

        FormsEngine.LimitNumberOfUserSelections = true;

        //tab switch code starts here
        var USOnlineSchools = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#mc-US-Area li[data-isgroundcampus="False"]');
        var USGroundSchools = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#mc-US-Area li[data-isgroundcampus="True"]');
        var OnlineCampusTabs = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#OnlineCampusTabs');
        var OnlineTab = $(OnlineCampusTabs).find('#OnlineTab');
        var GroundTab = $(OnlineCampusTabs).find('#CampusTab');

        $(OnlineTab).click(function () {
            switchToOnlineTab(USGroundSchools, USOnlineSchools);
        });

        $(GroundTab).click(function () {
            switchToGroundTab(USGroundSchools, USOnlineSchools);
        });

        //initial display schools
        displayInitialTabs(OnlineCampusTabs, GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);

        $(".btnChooseSchool").click(function () {
            submitSingleSchoolSelection(this);
        });

        $(".btnCtaConfirmMatches").click(function () {
            FormsEngine.LoadWorkflowStep('THANKYOU', '');

        });

        //Express consent
        getMetaData();
             
    }); //end of document.ready



})(jQuery);
