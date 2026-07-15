(function ($) {
    // WizardPlain_Start.js

    //globals
    var JobsLastZipCode = "";

    function printUserAgreementJobs() {
        if (FormsEngine.ResourceData) {
            var userAgreementControl = $(FormsEngine.DefaultFormTag).find('input[code="UAJobs"]').parents('div.field-holder');
            var jobsUserAgreement = FormsEngine.ResourceData['JS.WIZARD.JOBSUSERAGREEMENT'];

            //WebSite name placeholder
            if (FormsEngine.SiteName == undefined || FormsEngine.SiteName == '') {
                FormsEngine.SiteName = "Website";
            }
            jobsUserAgreement = jobsUserAgreement.replace(/\{0\}/g, FormsEngine.SiteName);

            //PhoneNumber placeholder
            var phoneNumber = $(FormsEngine.DefaultFormTag).find(":input[code='Phone']").val();
            if (phoneNumber != undefined) {
                phoneNumber = phoneNumber.replace(/[^\d.]/g, "");
            }
            else {
                phoneNumber = '';
            }
            if (phoneNumber.length >= 10) {
                phoneNumber = phoneNumber.substring(0, 3) + '.' + phoneNumber.substring(3, 6) + '.' + phoneNumber.substring(6);
            }
            jobsUserAgreement = jobsUserAgreement.replace(/\{1\}/g, phoneNumber);

            //Print jobs user agreement
            $(userAgreementControl).find('label').not('.error').html(jobsUserAgreement);
            $(FormsEngine.DefaultFormTag).find('input[code="UAJobs"]').val(jobsUserAgreement);
        }
    }

    function printDynamicHeaders() {
        if (FormsEngine.JobsCount != undefined && FormsEngine.JobsCount != '') {
            $(FormsEngine.DefaultFormTag).find('[data-dyn="jobscounter"]').html(FormsEngine.JobsCount.toString());
        }

        if (FormsEngine.JobsKeywords != undefined && FormsEngine.JobsKeywords != '') {
            $(FormsEngine.DefaultFormTag).find('[data-dyn="keywords"]').html(FormsEngine.JobsKeywords.toString());
        }

        if ($(FormsEngine.DefaultFormTag).find(':input[code="First_Name"]').val().length > 0) {
            FormsEngine.LastFirstName = $(FormsEngine.DefaultFormTag).find(':input[code="First_Name"]').val();
            if (FormsEngine.LastFirstName != undefined && FormsEngine.LastFirstName != '') {
                $(FormsEngine.DefaultFormTag).find('[data-dyn="name"]').html(FormsEngine.LastFirstName);
            }
        }
    }




    function SaveProspectJobContactMe() {
        fe_getSessionId(function () {

            fe_setSettingsFromCookies();
            var FormData = fe_getFormData();
            var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();

            var serviceArgs = "?TrackId=" + FormsEngine.TrackId;
            serviceArgs += "&SessionId=" + FormsEngine.SessionId;
            serviceArgs += "&MatchGuid=" + FormsEngine.MatchResponseGuid;
            serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
            serviceArgs += "&LeadData=" + encodeURIComponent(FormData.LeadData);
            serviceArgs += "&AdditionalData=" + encodeURIComponent(FormData.LeadAdditionalData);
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;
            serviceArgs += "&TemplateId=" + FormsEngine.TemplateId;
            serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
            serviceArgs += "&SiteURL=" + FormsEngine.SiteName;
            serviceArgs += "&Job=" + FormsEngine.JobsKeywords;
            serviceArgs += "&PostalCode=" + postalCode;

            var url = FormsEngine.ServiceBaseURL + '/TemplateManager/SaveProspectJobContactMe';

            $.ajax({
                async: false,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: url + serviceArgs,
                success: function (data) {
                    fe_consolelog("Prospect Saved");
                    //Generic No Match "Thank you" message
                    FormsEngine.LoadWorkflowStep('NOMATCH', true);
                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                }
            });
        });

    }


    //Google Analytics and internal tracking event
    function trackEvent(Event) {
        if (FormsEngine.TrackEvent) {
            FormsEngine.TrackEvent("JobsEvent." + Event);
        }
    }

    function onFormLoaded() {
        printUserAgreementJobs();
        printDynamicHeaders();
        var PostalQuerystring = fe_getParameterByName('Postal_Code');
        if (PostalQuerystring != undefined && PostalQuerystring != '') {
            getCityStateCountry(PostalQuerystring);
            $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").val(PostalQuerystring);
        }
    }

    function onStepLoaded(StepNumber) {
        //Hide back and Continue since menu is there
        if (StepNumber == FormsEngine.JobsSpecialMenuStep || StepNumber == FormsEngine.JobsSpecialMenuStep + 1) {
            $(FormsEngine.BackButton).hide();
        }

        //Step2 "Menu"
        if (StepNumber == FormsEngine.JobsSpecialMenuStep) {
            $(FormsEngine.SubmitButton).hide();

            //Hide required field legend
            $(FormsEngine.DefaultFormTag).find('.required-legend').hide();

            //hide next will come from the menu
            $(FormsEngine.DefaultFormTag).find("[code='Alternate_Phone']").unbind();
            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@EDMATCH@@']").unbind();
            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@EDMATCH@@']").click(function () {
                trackEvent('UserChoice.FindMyEducation');
                FormsEngine.CheckNavigation(1);
            });

            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@ADVIDOR@@']").unbind();
            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@ADVIDOR@@']").click(function () {
                trackEvent('UserChoice.ContactAdvisor');
                if (FormsEngine.JobsSetCookie) {
                    FormsEngine.JobsSetCookie(90);
                }
                SaveProspectJobContactMe();
            });


            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@JOBS@@']").unbind();
            $(FormsEngine.DefaultFormTag).find("[data-watermark='@@JOBS@@']").click(function () {
                trackEvent('UserChoice.JobsFind');
                if (FormsEngine.JobsClosePopup) {
                    FormsEngine.JobsClosePopup();
                }

            });
        }
        else {
            //Hide required field legend
            $(FormsEngine.DefaultFormTag).find('.required-legend').show();

            //Show next
            $(FormsEngine.SubmitButton).show();
        }

        if (StepNumber > FormsEngine.JobsSpecialMenuStep) {
            updateMatchCount();
        }
    }

    function onBeforeAutoAdvance(StepNumber) {
        //Step1 has user agreement, Step 2 has menu
        if (StepNumber != 1 && StepNumber != FormsEngine.JobsSpecialMenuStep && StepNumber != FormsEngine.JobsSpecialUserAgreement) {
            fe_wiz_AutoForwardStep();
        }
    }

    function updateMatchCount() {
        var FormData = fe_getFormData();
        FormsEngine.LeadDataEncoded = encodeURIComponent(FormData.LeadData);
        FormsEngine.LeadAdditionalDataEncoded = encodeURIComponent(FormData.LeadAdditionalData);

        // compare new Lead Data with previous Lead Data to avoid calling again
        if (FormsEngine.LeadDataEncoded != FormsEngine.LastProgramCountCallLeadDataEncoded) {
            var serviceArgs;
            // need session for this call
            fe_getSessionId(function () {
                serviceArgs = "?IsBeta=" + FormsEngine.IsBeta;
                serviceArgs += "&TrackId=" + FormsEngine.TrackId;
                serviceArgs += "&LeadData=" + FormsEngine.LeadDataEncoded;
                serviceArgs += "&InitialCall=" + (FormsEngine.InitialCall === undefined ? "true" : "false");
                serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;

                jQuery.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ServiceBaseURL + '/Matching/GetProgramsForCounter' + serviceArgs,
                    success: function (data) {
                        if (data != null) {
                            $(FormsEngine.DefaultFormTag).find('[data-dyn="programcount"]').html(data);
                        }
                    },
                    error: function (request, error) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    }
                });

                // store the last me call
                FormsEngine.LastProgramCountCallLeadDataEncoded = FormsEngine.LeadDataEncoded;
            });
        }
        FormsEngine.InitialCall = false;
    }

    function getCityStateCountry(ZipCode) {
        if (ZipCode != undefined && ZipCode != "" && ZipCode != JobsLastZipCode) {
            var YourLocation = "your location";
            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/FormValidation/GetCityStateCountry?ZipCode=" + ZipCode,
                success: function (data) {
                    JobsLastZipCode = ZipCode;
                    if (data.length > 0) {
                        YourLocation = data[0].Value + "," + data[1].Value; //City , State
                    }
                    $(FormsEngine.DefaultFormTag).find('[data-dyn="location"]').html(YourLocation);

                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    $(FormsEngine.DefaultFormTag).find('[data-dyn="location"]').html(YourLocation);
                }
            });
        }
    }


    $(document).ready(function () {
        FormsEngine.JobsSpecialMenuStep = $(FormsEngine.DefaultFormTag).find("[data-watermark='@@ADVIDOR@@']").attr('data-step');
        FormsEngine.JobsSpecialMenuStep = (FormsEngine.JobsSpecialMenuStep == undefined || FormsEngine.JobsSpecialMenuStep == '' ? -10 : FormsEngine.JobsSpecialMenuStep);

        FormsEngine.JobsSpecialUserAgreement = $(FormsEngine.DefaultFormTag).find('input[code="UAJobs"]').attr('data-step');
        FormsEngine.JobsSpecialUserAgreement = (FormsEngine.JobsSpecialUserAgreement == undefined || FormsEngine.JobsSpecialUserAgreement == '' ? -10 : FormsEngine.JobsSpecialUserAgreement);


        // Constants
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        FormsEngine.UseProgramCounter = false; //Custom counter not global counter

        //Events
        FormsEngine.OnFormLoaded = onFormLoaded;
        FormsEngine.OnStepLoadedInternal = onStepLoaded;
        FormsEngine.OnBeforeAutoAdvance = onBeforeAutoAdvance;
        FormsEngine.RefreshJobsDynamicHeader = onFormLoaded; //Drupal uses this function

        //Select all
        fe_wiz_showSelectAllButtons();

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

        //User agreement jobs
        $(FormsEngine.DefaultFormTag).find("input[code='Phone']").blur(function (event) {
            printUserAgreementJobs();
        });

        //FirstName bindings
        $(FormsEngine.DefaultFormTag).find("input[code='First_Name']").blur(function (event) {
            printDynamicHeaders();
        });

        if ($(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").exists()) {

            //kill events from hidden
            $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").unbind();

            //PhoneNumber bindings
            $(FormsEngine.DefaultFormTag).find("input[code='Phone']").blur(function (event) {
                $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").val($(FormsEngine.DefaultFormTag).find("input[code='Phone']").val());
            });

            //Hide additional phone container
            $(FormsEngine.DefaultFormTag).find('div[data-controlcode="Alternate_Phone"]').hide();

            //Hide control
            $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").hide();
        }

        //Country City state info from zipCode
        $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").blur(function (event) {
            if ($(this).val().length > 0) {
                FormsEngine.createCookie("user_postal_code", $(this).val());
            }
            getCityStateCountry($.trim($(this).val()));
        });


        //Refresh state synamic fields
        onFormLoaded();

        fe_OptimizelyCustomization();


        if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']")
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "")
              ) {
            $("div[data-controlcode='K12']").hide();
        }
    });

})(jQuery);