(function ($) {
    // ManagedChoice.js
    //-------------------

    // Constants

    // Internals variables
    var SubmittingForm = false;
    var emsApplicationId = 27;


    function hideErrorMessage(c) {
        // if there are selected programs and there is a error div on the page, hide it       
        $(c).addClass('hide');
        $(c).html('');
        $(FormsEngine.ManagedChoiceSubmitTag).removeClass('disabled');
    }

    function showErrorMessage(c, msg) {
        $(FormsEngine.ManagedChoiceSubmitTag).addClass('disabled');
        $(c).html(msg);
        $(c).removeClass('hide');
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
            console.log(hasCustomTCPA);

            if (hasCustomTCPA.trim() !== "") {
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

    function getMetaData() {
        //Get the resource meta data texts.
        if (FormsEngine.ResourceData == undefined) {
            //removed hardcoded list and now retrieve all messages labeled as tcpa
            fe_getResourceMetaDataTextForTCPA(function (data) {

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

    function checkValidMobileButton() {

        if (jQuery("#eddynexusform-wizard-managedchoice .managedchoice-express-consent input[type=checkbox]:checked").length < 1
            || (jQuery("#eddynexusform-wizard-managedchoice input[type=checkbox]:checked").length < 2)
            || !jQuery("#eddynexusform-wizard-managedchoice").validate().checkForm()) {

            jQuery("#screen-button").removeClass("enabled-button");
        }
        else if (!jQuery("#screen-button").hasClass("enabled-button")) {
            jQuery("#screen-button").addClass("enabled-button");
        }
    }

    $(document).ready(function () {

        //LeadiD external javascript events
        if ($(FormsEngine.DefaultFormTag).find('input[code="leadid_token"]').length <= 0 && FormsEngine.ApplicationId !== emsApplicationId) {
            $("<input>").attr({
                id: 'leadid_token',
                type: 'hidden',
                code: 'leadid_token',
                name: 'universal_leadid'
            }).appendTo(FormsEngine.DefaultFormTag);

            var script = document.createElement("script");
            script.id = "LeadiDscript";
            script.type = "text/javascript";
            script.language = "javascript";

            script.text += "(function () { var s = document.createElement('script'); s.id = 'LeadiDscript_campaign'; s.type = 'text/javascript'; s.async = true; s.src = (document.location.protocol + '//d1tprjo2w7krrh.cloudfront.net/campaign/50181952-3a30-427b-a8a9-4b010a76311c.js'); var LeadiDscript = document.getElementById('LeadiDscript'); LeadiDscript.parentNode.insertBefore(s, LeadiDscript); })();";

            document.body.appendChild(script);

            fe_initialize_ActiveProspect();
            //$('#eddynexusform-wizard-managedchoice').bind("DOMNodeInserted", function () {
            //    var tokenField = $('#eddynexusform-wizard-managedchoice').find(":input[name='xxTrustedFormToken']");
            //    if (tokenField.length > 0) {
            //        if (!tokenField.attr('code')) {
            //            tokenField.attr('code', 'xxTrustedFormToken_MC');
            //        }
            //    }
            //    var certUrlField = $('#eddynexusform-wizard-managedchoice').find(":input[name='xxTrustedFormCertUrl']");
            //    if (certUrlField.length > 0) {
            //        if (!certUrlField.attr('code')) {
            //            certUrlField.attr('code', 'xxTrustedFormCertUrl_MC');
            //        }
            //    }
            //});
            // Select the target element to observe
            var targetNode = document.getElementById('eddynexusform-wizard-managedchoice');

            if (targetNode) {
                // Create a MutationObserver instance
                var observer = new MutationObserver(function (mutationsList) {
                    mutationsList.forEach(function (mutation) {
                        if (mutation.type === 'childList') {
                            // Find the 'xxTrustedFormToken' input field
                            var tokenField = $('#eddynexusform-wizard-managedchoice').find(":input[name='xxTrustedFormToken']");
                            if (tokenField.length > 0) {
                                if (!tokenField.attr('code')) {
                                    tokenField.attr('code', 'xxTrustedFormToken_MC');
                                }
                            }

                            // Find the 'xxTrustedFormCertUrl' input field
                            var certUrlField = $('#eddynexusform-wizard-managedchoice').find(":input[name='xxTrustedFormCertUrl']");
                            if (certUrlField.length > 0) {
                                if (!certUrlField.attr('code')) {
                                    certUrlField.attr('code', 'xxTrustedFormCertUrl_MC');
                                }
                            }
                        }
                    });
                });

                // Configuration of the observer
                var config = { childList: true, subtree: true };

                // Start observing the target node for DOM changes
                observer.observe(targetNode, config);
            }

            // Configuration of the observer
            var config = { childList: true, subtree: true };

            // Start observing the target node for DOM changes
            observer.observe(targetNode, config);
        }
      
        //define areas
        FormsEngine.WorkflowManagedChoiceDivTag = $('#eddy-form-wizard-managedchoice-container');
        FormsEngine.ManagedChoiceErrorDivTag = '#dvManagedChoiceError';
        FormsEngine.ManagedChoiceFormTag = '#managedChoiceForm';
        FormsEngine.ManagedChoiceSubmitTag = 'div[name=managechoice-form-submit-button]';
        FormsEngine.ManagedChoiceNoThanksTag = 'div[name="form-nothanks-button"]';

        FormsEngine.SMInstitutionPopup = $('#sm-institution-popup');
        FormsEngine.SMProgramPopup = $('#sm-program-popup');
        FormsEngine.USInstitutionPopup = $('#us-institution-popup');
        FormsEngine.USInstitutionDisclaimerPopup = $('#us-institution-disclaimer-popup');
        FormsEngine.USProgramPopup = $('#us-program-popup');

        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').on('change', function (event) {
            hideErrorMessage(FormsEngine.ManagedChoiceErrorDivTag);
            var parentli = $(this).parents('[name=mc-us-schoolcontainer]');
            var additionalQuestionsDiv = parentli.find("[name=mc-SM-additional-questions]");
            if ($(this).is(":checked")) {
                additionalQuestionsDiv.show();
            }
            else {
                additionalQuestionsDiv.hide();
            }
            checkValidMobileButton();
        });

        $(FormsEngine.WorkflowManagedChoiceDivTag).on('change', '#ckbExpressConsent', function (event) {
            checkValidMobileButton();
        });

        checkValidMobileButton();



        // SM Insitution Info Popup && US Insitution Info Popup
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('.sm-institution-popup-link, .us-institution-popup-link').on('click', fe_mc_showUSPopup);

        // SM Program Info Popup
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('.sm-program-popup-link').on('click', function (event) {
            event.preventDefault();
            var dataholder = $(this).parents("[name='mc-us-schoolcontainer']");

            // popup is different, and location to show popup is diffent
            var popup = $(FormsEngine.SMProgramPopup);
            var y = event.screenY;
            $(popup).css('top', event.pageY - $('#mc-SM-Display').offset().top - 50 + 'px');


            //var popup = $(FormsEngine.SMProgramPopup);
            // get data
            var InstitutionName = $(dataholder).attr('data-institution-name');
            //Helen add hidden institution logo for popup info
            var InstitutionLogo = $(dataholder).find('.us-institution-logo a.us-institution-popup-link').html();
            var ProgramName = $(dataholder).attr('data-program-name');
            var ProgramDescription = decodeURIComponent(($(dataholder).attr('data-program-description') + '').replace(/\+/g, '%20'));
            var ProgramDisclaimer = decodeURIComponent(($(dataholder).attr('data-program-disclaimer') + '').replace(/\+/g, '%20'));
            var ProgramDisclaimerType = $(dataholder).attr('data-program-disclaimer-type');
            var IsGroundCampus = false;
            if ($(dataholder).attr('data-isgroundcampus') != undefined && $(dataholder).attr('data-isgroundcampus') != 'undefined' && $(dataholder).attr('data-isgroundcampus') != null && $(dataholder).attr('data-isgroundcampus') != 'null' && $(dataholder).attr('data-isgroundcampus').toLowerCase() == 'true') {
                IsGroundCampus = true;
                var CampusName = $(dataholder).attr('data-campus-name');
                var CampusCity = $(dataholder).attr('data-campus-city');
                var CampusState = $(dataholder).attr('data-campus-state');
            }

            // construct html
            var programInfo = '';
            programInfo += '<div id="progDescTop" class="hide"></div>';
            programInfo += '<div class="popup-info-wrapper">';
            programInfo += '<a class="sm-program-popup-close popup-close" href="#"><span class="fa fa-times hide"></span></a>';
            programInfo += '<div class="popup-content">';
            programInfo += '<div class="us-institution-logo hide">' + InstitutionLogo + '</div>';
            programInfo += '<h4 class="sm-program-popup-InstitutionName">' + InstitutionName + '</h4>';
            if (IsGroundCampus) {
                programInfo += '<p class="sm-institution-popup-CampusName">' + CampusName + '</p>';
                programInfo += '<p class="sm-institution-popup-CampusCity">' + CampusCity + '</p>';
                programInfo += '<p class="sm-institution-popup-CampusState">' + CampusState + '</p>';
            }
            programInfo += '<h5 class="sm-program-popup-ProgramName">' + ProgramName + '</h5>';
            if (ProgramDescription != undefined && ProgramDescription != 'undefined' && ProgramDescription != null) {
                programInfo += '<div class="sm-program-popup-ProgramDescription">' + ProgramDescription + '</div>';
            }
            if (ProgramDisclaimerType != null && ProgramDisclaimerType != "" && ProgramDisclaimerType.toLowerCase() == "link") {
                programInfo += 'For ' + ProgramName + ' disclosure information <a class="sm-program-popup-ProgramDisclaimer" href="' + ProgramDisclaimer + '" target="_blank">click here.<a>';
            } else if (ProgramDisclaimerType != null && ProgramDisclaimerType != "" && ProgramDisclaimerType.toLowerCase() == "text") {
                programInfo += '<span class="sm-program-popup-ProgramDisclaimer">' + ProgramDisclaimer + '</span>';
            }
            programInfo += '</div></div>';
            programInfo += '<div id="progDescBtm" class="hide"></div>';

            $(popup).html(programInfo);
            $(popup).fadeIn('slow');
        });


        // US School Disclaimer Popup
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('.us-disclosure-popup-link').on('click', function (event) {
            event.preventDefault();

            var popup = $(FormsEngine.USInstitutionDisclaimerPopup);
            var y = event.screenY;
            $(popup).css('top', event.pageY - $('#mc-US-Area').offset().top - 50 + 'px');


            // get data
            var disclosure = $(this).parents('div.more-info').find('div.dvUserSelectSchoolDisclaimer').text().trim();

            // construct html
            var programInfo = '';
            programInfo += '<div id="progDescTop" class="hide"></div>';
            programInfo += '<div class="popup-info-wrapper">';
            programInfo += '<a class="sm-program-popup-close popup-close" href="#"><span class="fa fa-times hide"></span></a>';
            programInfo += '<div class="popup-content">';
            programInfo += disclosure;
            programInfo += '</div></div>';
            programInfo += '<div id="progDescBtm" class="hide"></div>';


            $(popup).html(programInfo);
            $(popup).fadeIn('slow');
        });

        // US Program Info Popup
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('.us-program-popup-link').on('click', function (event) {
            // TODO - Ben - use better selectors and refactor this code to be reuseable and a function available on the global wizard script
            event.preventDefault();
            var dataholder = $(this).parents("[name='mc-us-schoolcontainer']").find('option:selected');
            var popup = $(FormsEngine.USProgramPopup);
            var y = event.screenY;
            $(popup).css('top', event.pageY - $('#mc-US-Area').offset().top - 50 + 'px');

            // get data
            var InstitutionName = $(dataholder).parents("[name='mc-us-schoolcontainer']").attr('data-institution-name');
            //Helen add hidden institution logo for popup info
            var InstitutionLogo = $(dataholder).parents("[name='mc-us-schoolcontainer']").find('.us-institution-logo a.us-institution-popup-link').html();
            var ProgramName = $(dataholder).attr('data-program-name');
            var programDetail = {};

            // call ME for US program data
            if ($(dataholder).val() != "") {
                //get program info
                var programId = $(dataholder).attr('data-programid');

                var arguments = "?ProgramId=" + programId + "&IsBeta=" + FormsEngine.IsBeta + "&TrackId=" + FormsEngine.TrackId;
                $.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ServiceBaseURL + "/Matching/GetProgram" + arguments, // "/Matching/GetProgramDetail" method was too slow since data was not cached
                    success: function (data) {
                        programDetail.ProgramDescription = decodeURIComponent((data.ProgramList[0].ProgramDescription + '').replace(/\+/g, '%20'));
                        programDetail.ProgramDisclaimer = decodeURIComponent((data.ProgramList[0].ProgramDisclaimer + '').replace(/\+/g, '%20'));
                        programDetail.ProgramDisclaimerType = data.ProgramList[0].ProgramDisclaimerType;

                        var IsGroundCampus = false;
                        if ($(dataholder).attr('data-isgroundcampus') != undefined && $(dataholder).attr('data-isgroundcampus') != 'undefined' && $(dataholder).attr('data-isgroundcampus') != null && $(dataholder).attr('data-isgroundcampus') != 'null' && $(dataholder).attr('data-isgroundcampus').toLowerCase() == 'true') {
                            IsGroundCampus = true;
                            var CampusName = $(dataholder).attr('data-campus-name');
                            var CampusCity = $(dataholder).attr('data-campus-city');
                            var CampusState = $(dataholder).attr('data-campus-state');
                        }

                        // construct html
                        var programInfo = '';
                        programInfo += '<div id="progDescTop" class="hide"></div>';
                        programInfo += '<div class="popup-info-wrapper">';
                        programInfo += '<a class="us-program-popup-close popup-close" href="#"><span class="fa fa-times hide"></span></a>';
                        programInfo += '<div class="popup-content">';
                        //Helen add hidden school logo into popup
                        programInfo += '<div class="us-institution-logo hide">' + InstitutionLogo + '</div>';
                        programInfo += '<h4 class="us-program-popup-InstitutionName">' + InstitutionName + '</h4>';
                        if (IsGroundCampus) {
                            programInfo += '<p class="sm-institution-popup-CampusName">' + CampusName + '</p>';
                            programInfo += '<p class="sm-institution-popup-CampusCity">' + CampusCity + '</p>';
                            programInfo += '<p class="sm-institution-popup-CampusState">' + CampusState + '</p>';
                        }
                        programInfo += '<h5 class="us-program-popup-ProgramName">' + ProgramName + '</h5>';
                        if (programDetail.ProgramDescription != undefined && programDetail.ProgramDescription != 'undefined' && programDetail.ProgramDescription != null && programDetail.ProgramDescription != 'null' && programDetail.ProgramDescription.length > 0) {
                            programInfo += '<div class="us-program-popup-ProgramDescription"><div name="us-program-popup-ProgramDescription-header">Program Description:</div><div>' + programDetail.ProgramDescription + '</div></div>';
                        }
                        if (programDetail.ProgramDisclaimerType != null && programDetail.ProgramDisclaimerType != "" && programDetail.ProgramDisclaimerType.toLowerCase() == "link") {
                            programInfo += 'For ' + ProgramName + ' disclosure information <a class="us-program-popup-ProgramDisclaimer" href="' + programDetail.ProgramDisclaimer + '" target="_blank">click here.<a>';
                        } else if (programDetail.ProgramDisclaimerType != null && programDetail.ProgramDisclaimerType != "" && programDetail.ProgramDisclaimerType.toLowerCase() == "text") {
                            programInfo += '<span class="us-program-popup-ProgramDisclaimer">' + programDetail.ProgramDisclaimer + '</span>';
                        }
                        programInfo += '</div></div>';
                        programInfo += '<div id="progDescBtm" class="hide"></div>';


                        $(popup).html(programInfo);
                        $(popup).fadeIn('slow');
                    },
                    error: function (request, error) {
                        if (FormsEngine.DebugMode) {
                            fe_consolelog(arguments);
                            fe_consolelog(" Error: " + request.responseText);
                        }
                    }
                });
            }
        });

        // Execute Managed Choice Submission event
        $(FormsEngine.ManagedChoiceSubmitTag).off('click'); //kill click before attaching new click. this prevents double submit on school selection if second pass
        $(FormsEngine.ManagedChoiceSubmitTag).on('click', function (event) {
            event.preventDefault();
            var PassedValidation = true;

            //Log Jornaya events if not available on ET
            fe_logJornaya();

            //Prevent multiple submissions
            if (SubmittingForm) {
                return;
            }
            SubmittingForm = true;
            $(FormsEngine.ManagedChoiceSubmitTag).addClass('disabled');

            fe_showLoader();

            if (FormsEngine.MoveLoader != undefined) {
                FormsEngine.MoveLoader();
            }

            var selectedCount = $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').length;

            // show an error message if they hit submit without choosing at least one program
            if (selectedCount < 1) {
                fe_hideLoader(false);
                showErrorMessage(FormsEngine.ManagedChoiceErrorDivTag, '<p>Please select at least one school and program in order to request information.</p>');
                SubmittingForm = false;

                // Scroll to 
                var offset = $(FormsEngine.ManagedChoiceErrorDivTag).offset();
                if (offset != undefined && offset != null) {
                    $('html, body').animate({
                        scrollTop: offset.top - 100
                    }, 1000);
                }
                return;
            }

            // Limit the number of programs a user can submit to
            // get the count of checked schools, and if it is greater than the MCAllowedSchoolCount throw an error
            if (FormsEngine.LimitNumberOfUserSelections != undefined && FormsEngine.LimitNumberOfUserSelections) {
                if (selectedCount > parseInt(FormsEngine.MaxManagedChoiceUserSelections)) {
                    fe_hideLoader(true);
                    var removeCount = selectedCount - FormsEngine.MaxManagedChoiceUserSelections;
                    showErrorMessage(FormsEngine.ManagedChoiceErrorDivTag, '<p>You can only select up to ' + FormsEngine.MaxManagedChoiceUserSelections + ' school(s). Please remove ' + removeCount + ' of your selections.</p>');
                    SubmittingForm = false;
                    fe_hideLoader(false);
                    fe_ScrollToTop();
                    return;
                }
            }

            var $expressConsentCB = $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[id=ckbExpressConsent]');
            if ($expressConsentCB.length > 0) {
                var checkedExpressConsent = $expressConsentCB.is(':checked');
                if (selectedCount > 0) {
                    if (!checkedExpressConsent) {
                        // show an error message if they hit submit without checking the Express Consent box
                        fe_hideLoader(true);
                        showErrorMessage(FormsEngine.ManagedChoiceErrorDivTag, '<p>You must agree to the acknowledgment below in order to request information.</p>');
                        $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-black');
                        $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-error');
                        PassedValidation = false;
                    } else {
                        // prepare to save the Express Consent text with the lead data
                        var expressConsentText = $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').text();
                        $expressConsentCB.val(expressConsentText);
                        var expressConsentTextEncoded = encodeURIComponent(expressConsentText.replace(/&/g, "amp;"));
                        FormsEngine.LeadDataEncoded = FormsEngine.LeadDataEncoded + '%26' + 'SchoolSelectionExpressConsent' + '%3D' + expressConsentTextEncoded;
                    }
                } else {
                    // if user checks express consent but has not selected school, show error
                    if (checkedExpressConsent) {
                        showErrorMessage('<p>Please select at least one school and program in order to request information.</p>');
                        $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-error');
                        $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-black');
                        PassedValidation = false;
                    }
                }

                if (!checkedExpressConsent) {
                    // Scroll to 
                    var offset = $expressConsentCB.offset();
                    if (offset != undefined && offset != null) {
                        $('html, body').animate({
                            scrollTop: offset.top - 100
                        }, 1000);
                    }
                }
            }

            var mcUserSelectProgramArray = new Array();
            var mcUserSelectTCPAArray = [];

            // add all the selected programs from the mc user select area to the array also add show/hide logic for input click
            var SelectedPms = $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked');
            var firstError = true;
            for (var i = 0; i < SelectedPms.length ; i++) {
                var obj = SelectedPms[i];
                var selectedSchool = $(obj).parents("[name='mc-us-schoolcontainer']");
                //var campusid = $(obj).attr('data-campusid');
                var selectedProgram = $(selectedSchool).find('select[name=mc-us-program-ddl] option:selected');
                var programProductId = $(selectedProgram).attr('data-programproductid');
                var templateId = $(selectedProgram).attr('data-templateid');
                var additionalQuestionBlock = $(selectedSchool).find('div[name=mc-SM-additional-questions]');
                var selectedTCPA = $(obj).closest('.mc-us-school').find('.schoolExpressConsentMessage').text();

                //Validate inputs
                if ($(additionalQuestionBlock).exists()) {
                    var AdditionalQuestions = $(additionalQuestionBlock).find(':input');
                    for (var qq = 0; qq < AdditionalQuestions.length; qq++) {
                        var passed = fe_wiz_ss_validateRequired(AdditionalQuestions[qq]);
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

                templateId = (typeof templateId == 'undefined' || templateId == null || templateId == "") ? 0 : templateId;
                mcUserSelectProgramArray.push(programProductId + '_' + templateId);
                mcUserSelectTCPAArray[programProductId] = encodeURIComponent(selectedTCPA);
            }

            //Failed validation
            if (!PassedValidation) {
                SubmittingForm = false;
                fe_hideLoader(false);
                return;
            } else {
                //Fire external event for ads
                if (FormsEngine.OnSchoolSelectionSubmit) {
                    FormsEngine.OnSchoolSelectionSubmit();
                }
            }


            hideErrorMessage(FormsEngine.ManagedChoiceErrorDivTag);

            // if they have selected programs, go back to server to save leads, else move to Thank You/No Match
            if (mcUserSelectProgramArray.length > 0) {
                var res = fe_wiz_buildAdditionalQuestionsAnswersForSubmit(); //must rebuild to make sure we only pass questions answered and selected
                //append serialized question/answer array if not null to leaddata then pass encoded to next step
                var theEncodedAdditionalData = FormsEngine.LeadAdditionalDataEncoded;
                var theEncodedData = FormsEngine.LeadDataEncoded;
              
               
                if (res != null || res != undefined) {
                    var additionalQ = "";
                    var extraLeadData = "";
                    //first build the string we need for our additional data
                    for (var p = 0; p < res.length; p++) {
                        var control = fe_wiz_mc_getAdditionalQuestionControlValue(res[p].code)
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

                var tokenField = $('#eddynexusform-wizard-managedchoice').find(":input[code='xxTrustedFormToken_MC']");
                if (tokenField.length > 0) {
                    theEncodedData += encodeURIComponent("&xxTrustedFormToken_MC=" + tokenField.val()); 
                }

                var certUrlField = $('#eddynexusform-wizard-managedchoice').find(":input[code='xxTrustedFormCertUrl_MC']");
                if (certUrlField.length > 0) {              
                    theEncodedData += encodeURIComponent("&xxTrustedFormCertUrl_MC=" + certUrlField.val());
                }  
                
                FormsEngine.LeadAdditionalDataEncoded = theEncodedAdditionalData;
                FormsEngine.LeadDataEncoded = theEncodedData;
                FormsEngine.CurrentUserSelections = mcUserSelectProgramArray.length;
                FormsEngine.UserSubmittedManagedChoiceSelection = true;
                fe_mc_UserSelectSubmitWSCustomTCPA(mcUserSelectProgramArray, mcUserSelectTCPAArray, FormsEngine.LeadDataEncoded, true);
            } else if (FormsEngine.SMLeadsCreatedCount > 0 || FormsEngine.USLeadsCreatedCount > 0) {
                FormsEngine.LoadWorkflowStep('THANKYOU', '');
            } else {
                FormsEngine.LoadWorkflowStep('NOMATCH', '');
            }

            SubmittingForm = false;

        });

        $('.schoolExpressConsentCheckbox').change(function () {
            // Find the nearest .ms-us-school-checkbox element and change its checked state
            $(this).closest('.mc-us-school').find('.ms-us-school-checkbox').prop('checked', $(this).is(':checked'));
        });

        $(FormsEngine.ManagedChoiceNoThanksTag).on('click', function (event) {
            // move to Thank You/No Match
            if (FormsEngine.SMLeadsCreatedCount > 0 || FormsEngine.USLeadsCreatedCount > 0) {
                FormsEngine.UserSkippedToConfirmation = true;
                FormsEngine.LoadWorkflowStep('THANKYOU', '');
            } else {
                FormsEngine.LoadWorkflowStep('NOMATCH', '');
            }
        });

        $(document).on('click', '.popup-close', function (event) {
            event.preventDefault();
            $(this).parents('.popup-info').fadeOut('slow');
        });

        $('.popup-info').scroll(function () {
            var el = $(this).find('.popup-close');
            var elpos_original = 10;
            var y = $(this).scrollTop();
            var finaldestination = y;
            if (y < elpos_original) {
                finaldestination = elpos_original;
                el.stop().css({ 'top': 10 });
            } else {
                el.stop().animate({ 'top': finaldestination - elpos_original + 15 }, 100);
            }
        });

        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[id=ckbExpressConsent]').on('click', function (event) {
            if ($(FormsEngine.WorkflowManagedChoiceDivTag).find('input[id=ckbExpressConsent]:checked').exists()) {
                $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-black');
                $(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-error');
                hideErrorMessage(FormsEngine.ManagedChoiceErrorDivTag);
            }
        });

        fe_wiz_getAdditionalQuestionCollection();
        getMetaData();        

        // Fancy Clickable US boxes on SS..
        //initializeUSClickableBoxes();

    }); //end of document.ready

})(jQuery);
