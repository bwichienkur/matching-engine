(function ($) {
    // ClientWizardBase.js (WizardTemplates loader)
    //----------------------

    function getSchoolPickerSubmissionServiceArgs() {
        //var sMLeadsCreatedCount = (FormsEngine.SMLeadsCreatedCount != undefined && FormsEngine.SMLeadsCreatedCount != null) ? FormsEngine.SMLeadsCreatedCount : 0;
        //if (sMLeadsCreatedCount > 0) {
        //    fe_consolelog('Smart Match leads already created for this user, not Smart Matching again.');
        //}
        var uSLeadsCreatedCount = FormsEngine.USLeadsCreatedCount !== undefined && FormsEngine.USLeadsCreatedCount !== null ? FormsEngine.USLeadsCreatedCount : 0;

        var serviceArgs = "";

        fe_setSettingsFromCookies();

        // need session for this call
        fe_getSessionId(function () {
            serviceArgs = "?TemplateId=" + FormsEngine.TemplateId;
            serviceArgs += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
            serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
            serviceArgs += "&TrackId=" + FormsEngine.TrackId;
            serviceArgs += "&SessionId=" + FormsEngine.SessionId;
            serviceArgs += "&MatchGuid=" + FormsEngine.MatchResponseGuid;
            serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
            serviceArgs += "&LeadData=" + FormsEngine.LeadDataEncoded;
            serviceArgs += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
            serviceArgs += "&SplitCampusTypeInResults=" + FormsEngine.SplitCampusTypeInResults;
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;
            serviceArgs += "&RenderingExperience=" + FormsEngine.RenderingExperience;
            serviceArgs += "&LimboAlternativeCampaignTrackid=" + FormsEngine.LimboAlternativeCampaignTrackid;
            serviceArgs += "&LimboAlternativeCampaignTrackidUtilized=" + (FormsEngine.LimboAlternativeCampaignTrackidUtilized === true ? "true" : "false");
            serviceArgs += '&FormTemplateType=' + FormsEngine.FormTemplateType;
            serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
        });

        return serviceArgs;
    }

    function getManagedChoiceServiceArgs() {
        var sMLeadsCreatedCount = FormsEngine.SMLeadsCreatedCount !== undefined && FormsEngine.SMLeadsCreatedCount !== null ? FormsEngine.SMLeadsCreatedCount : 0;
        if (sMLeadsCreatedCount > 0) {
            fe_consolelog('Smart Match leads already created for this user, not Smart Matching again.');
        }
        var uSLeadsCreatedCount = FormsEngine.USLeadsCreatedCount !== undefined && FormsEngine.USLeadsCreatedCount !== null ? FormsEngine.USLeadsCreatedCount : 0;

        var serviceArgs = "";

        fe_setSettingsFromCookies();

        var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
        var ProgramTemplateId = $(ProgramDDL).attr('data-templateid');
        var ProgramProductId = $(ProgramDDL).attr('data-programproductid');
        var ProductId = $(ProgramDDL).attr('data-productid');
        ProgramTemplateId = ProgramTemplateId === undefined || ProgramTemplateId === null ? FormsEngine.ProgramTemplateId : ProgramTemplateId;
        ProgramTemplateId = ProgramTemplateId === undefined || ProgramTemplateId === null ? "" : ProgramTemplateId;
        ProgramProductId = ProgramProductId === undefined || ProgramProductId === null ? FormsEngine.ProgramProductId : ProgramProductId;
        ProgramProductId = ProgramProductId === undefined || ProgramProductId === null ? "" : ProgramProductId;
        ProductId = ProductId === undefined || ProductId === null ? FormsEngine.ProductId : ProductId;
        ProductId = ProductId === undefined || ProductId === null ? "" : ProductId;

        // need session for this call
        fe_getSessionId(function () {
            serviceArgs = "?TemplateId=" + FormsEngine.TemplateId;
            serviceArgs += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
            serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
            serviceArgs += "&TrackId=" + FormsEngine.TrackId;
            serviceArgs += "&SessionId=" + FormsEngine.SessionId;
            serviceArgs += "&MatchGuid=" + FormsEngine.MatchResponseGuid;
            serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
            serviceArgs += "&LeadData=" + FormsEngine.LeadDataEncoded;
            serviceArgs += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
            serviceArgs += "&SMLeadsCreatedCount=" + sMLeadsCreatedCount;
            serviceArgs += "&USLeadsCreatedCount=" + uSLeadsCreatedCount;
            serviceArgs += "&SplitCampusTypeInResults=" + FormsEngine.SplitCampusTypeInResults;
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;
            serviceArgs += "&RenderingExperience=" + FormsEngine.RenderingExperience;
            serviceArgs += "&LimboAlternativeCampaignTrackid=" + FormsEngine.LimboAlternativeCampaignTrackid;
            serviceArgs += "&LimboAlternativeCampaignTrackidUtilized=" + (FormsEngine.LimboAlternativeCampaignTrackidUtilized === true ? "true" : "false");
            serviceArgs += '&FormTemplateType=' + FormsEngine.FormTemplateType;
            serviceArgs += '&ProgramTemplateId=' + ProgramTemplateId;
            serviceArgs += '&ProgramProductId=' + ProgramProductId;
            serviceArgs += '&ProductId=' + ProductId;
            serviceArgs += '&InstitutionId=' + FormsEngine.InstitutionId;
            serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
            serviceArgs += "&InstitutionName=" + FormsEngine.InstitutionName;
            serviceArgs += "&ProgramName=" + FormsEngine.ProgramName;
        });

        return serviceArgs;
    }

    function getStartServiceArgs() {
        var serviceArgs;

        // need session for this call
        fe_getSessionId(function () {
            serviceArgs = "?RenderingStrategy=" + FormsEngine.RenderingStrategy;
            serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
            serviceArgs += "&TemplateId=" + FormsEngine.TemplateId;
            serviceArgs += "&IgnoreTemplateCache=" + FormsEngine.IgnoreTemplateCache;
            serviceArgs += "&Theme=" + FormsEngine.Theme;
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            serviceArgs += "&TrackId=" + FormsEngine.TrackId;

            if (FormsEngine.readCookie) {
                var ipOverrideCookie = FormsEngine.readCookie("FE_IPOverride");
                if (ipOverrideCookie && ipOverrideCookie !== '') {
                    serviceArgs += "&IPOverride=" + ipOverrideCookie;
                }
            }
        });

        return serviceArgs;
    }


    function getThankYouServiceArgs() {
        var serviceArgs = "";

        // need session for this call
        fe_getSessionId(function () {
            serviceArgs = "?TemplateId=" + FormsEngine.TemplateId;
            serviceArgs += "&Theme=" + FormsEngine.Theme;
            serviceArgs += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
            serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            serviceArgs += "&TrackId=" + FormsEngine.TrackId;
            serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
            serviceArgs += "&UseNewEndpoint=";
            serviceArgs += FormsEngine.RenderingStrategy === "SCHOOLPICKERWIZARD" ? "true" : "false";
        });
        return serviceArgs;
    }

    function getNoMatchServiceArgs(aggs) {
        var serviceArgs = "?Theme=" + FormsEngine.Theme;
        serviceArgs += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
        serviceArgs += "&GenericNoMatch=" + (aggs === true ? true : false);
        return serviceArgs;
    }

    //Dynamic javascript
    function loadJs(filename) {
        var script = document.createElement('script');
        script.type = "text/javascript";
        script.language = "javascript";
        if (FormsEngine.CompressJs) {
            script.src = FormsEngine.ServiceBaseURL + '/Static/GetJs?BasePath=Common&FileName=' + filename + '&CompressJs=true';
        }
        else {
            script.src = FormsEngine.ServiceBaseURL + '/Templates/Common/js/' + filename + '.js'
        }
        document.body.appendChild(script);
    }

    //check FE arguments
    function checkArguments() {
        var result = true;

        if (window['FormsEngine'] === undefined
            || FormsEngine === null
            || !FormsEngine.hasOwnProperty('RenderingStrategy')
            || !FormsEngine.hasOwnProperty('RenderingDiv')
            || !FormsEngine.hasOwnProperty('SelfContained')
            || !FormsEngine.hasOwnProperty('IsBeta')
            || !FormsEngine.hasOwnProperty('TrackId')
            || !FormsEngine.hasOwnProperty('SelfContained')
            || (FormsEngine.SelfContained === false &&
                (!FormsEngine.hasOwnProperty('WorkflowStartPage')
                    || !FormsEngine.hasOwnProperty('WorkflowManagedChoicePage')
            || !(FormsEngine.hasOwnProperty('WorkflowThankYouPage') || (FormsEngine.hasOwnProperty('ThankYouApplyNowURL') && FormsEngine.hasOwnProperty('ThankYouRequestInfoURL')))
            || !FormsEngine.hasOwnProperty('WorkflowNoMatchPage')
          
            
                )
            )
        ) {
            $(document.body).append("<div>Error: Wizard FormsEngine object has to be defined before Client Script is included with the appropriate settings.<br/><br/>"
                + " i.e.<br/> var FormsEngine = FormsEngine || {}; <br/>"
                + " FormsEngine.RenderingStrategy ='[RenderingStrategy]'; <br/>"
                + " FormsEngine.RenderingDiv = '[RenderingDiv]'; <br/>"
                + " FormsEngine.SelfContained = [true/false]; <br/>"
                + " FormsEngine.NoMatch = '[NoMatchPage/NoMatchDiv]'; <br/>"
                + " FormsEngine.IsBeta = [true/false]; <br/>"
                + " FormsEngine.TrackId = '[TrackId]'; <br/>");
            result = false;
        }
        else if (typeof window.FormsEngineGlobal === "undefined") {
            $(document.body).append("<div>FormsEngine Error: FormsEngine Global javascript must be included first. <br/><be/>"
                + " i.e. <br/> &lt;script type='text/javascript' src='//server/Static/GetGlobal'>&lt;/script> </div>");
            result = false;
        }
        else if (FormsEngine.RenderingStrategy === 'WIZARDCALLCENTERWHITELABEL' && (FormsEngine.TrackId === '' || FormsEngine.TrackId === undefined)) {
            $(document.body).append("<div>Error: You need a campaign TrackId in order to render the Form.<br/><br/></div>");
            result = false;
        }

        if (FormsEngine.InstitutionName && FormsEngine.InstitutionName.length > 0) {
            FormsEngine.InstitutionName = $('<div />').html(FormsEngine.InstitutionName === undefined ? '' : FormsEngine.InstitutionName).text();
        }

        return result;
    }

    //Google analytics and internal tracking events
    function trackEvent(event) {
        var Gevent = event;
        if (FormsEngine.TemplateId !== undefined && FormsEngine.TemplateId > 0) {
            Gevent = event + ".TemplateId." + FormsEngine.TemplateId;
        }

        //If Google Tag manager is detected fire same events there else fire regular gaq
        try {
            if (typeof dataLayer !== 'undefined') {
                dataLayer.push({
                    'event': 'virtualPageView',
                    'url': Gevent
                });
            }
            else if (typeof _gaq !== 'undefined') {
                _gaq.push(['_trackPageview', Gevent]);
            }
        } catch (e) {/**/}

        try {
            if (typeof _etq !== 'undefined') {
                _etq.push(['_etEvent', 'workflow', event, 'form-wizard']);
            }
        } catch (e) {/**/}

    }

    //sets default values
    function setDefaults() {
        FormsEngine.ServiceBaseURL = "[SERVICEBASE]";
        FormsEngine.SelfContained = FormsEngine.SelfContained === undefined || FormsEngine.SelfContained === "" ? true : FormsEngine.SelfContained;
        FormsEngine.IsBeta = FormsEngine.IsBeta === undefined || FormsEngine.IsBeta === "" ? false : FormsEngine.IsBeta;
        FormsEngine.DebugMode = FormsEngine.DebugMode === undefined || FormsEngine.DebugMode === "" ? false : FormsEngine.DebugMode;
        FormsEngine.CompressJs = false; //FormsEngine.CompressJs == undefined ? false : FormsEngine.CompressJs;
        FormsEngine.Theme = FormsEngine.Theme === undefined || FormsEngine.Theme === "" ? "default" : FormsEngine.Theme;
        FormsEngine.RenderingStrategy = FormsEngine.AlternativeRenderingStrategy === undefined ? FormsEngine.RenderingStrategy : FormsEngine.AlternativeRenderingStrategy;
        FormsEngine.LoadWorkflowStep = loadWorkflowStep;
        FormsEngine.TrackEvent = function (event) { trackEvent(event); };
        FormsEngine.DisableScrolltoTop = FormsEngine.DisableScrolltoTop === undefined || FormsEngine.DisableScrolltoTop === "" ? false : FormsEngine.DisableScrolltoTop;

        if (FormsEngine.OptimizelyTheme) {
            FormsEngine.Theme = FormsEngine.OptimizelyTheme;
        }

        //check for alternate templates and use that if needed
        FormsEngine.TemplateId = fe_getAlternativeTemplateId(FormsEngine.TemplateId);
    }

    function refreshWorkflowContainers(showList, hideList) {
        jQuery.each(showList, function (index, item) {
            $('[name="' + item + '"]').show();
        });
        jQuery.each(hideList, function (index, item) {
            $('[name="' + item + '"]').hide();
        });
    }

    function loadDiv(data) {
        // Inject Html per Workflow Page
        var renderedHtml = "";
        if (FormsEngine.TrackEvent) {
            //Currently not counting if redirected on the flow on MChoice only
            // Thank you page redirect to no match TBD since is a flow for jobs
            if (FormsEngine.CurrentPage === 'MANAGEDCHOICE') {
                if (!data.MoveToStart
                    && !data.MoveToThankYou
                    && !data.MoveToNoMatch) {
                    // Track that the user actually sees the Managed Choice..
                    FormsEngine.TrackEvent("MANAGEDCHOICE");
                }
            }
            else {
                FormsEngine.TrackEvent(FormsEngine.CurrentPage);
            }
        }
        if (FormsEngine.CurrentPage === 'MANAGEDCHOICE') {

            // set some data back to the page from the MC result
            FormsEngine.LeadId = data.ProgramWizardInitialValidLeadId;
            FormsEngine.SMLeadsCreatedCount = FormsEngine.SMLeadsCreatedCount !== undefined && FormsEngine.SMLeadsCreatedCount !== null ? FormsEngine.SMLeadsCreatedCount : 0;
            if (data.SMLeadsCreatedCount > 0) {
                FormsEngine.IsTestLead = data.IsTestLead;
                if (!FormsEngine.IsTestLead) {
                    //Google Tag Manager events
                    try {
                        //Individual programId event coming from Razor View

                        if (data.SMLeadsCreatedCount > 0) {
                            fe_googleTagEvent('gaEvent', 'client', 'wizard-success', FormsEngine.TemplateId);
                        }

                    } catch (ex) {/**/}
                }
            }

            FormsEngine.MatchResponseGuid = data.MatchGuid;
            FormsEngine.SMLeadsCreatedCount = data.SMLeadsCreatedCount;
            FormsEngine.USLeadsCreatedCount = data.USLeadsCreatedCount;
            FormsEngine.UserFullName = data.UserFullName
            if ((FormsEngine.UserSmartMatched === undefined || FormsEngine.UserSmartMatched === false) && FormsEngine.SMLeadsCreatedCount > 0) {
                FormsEngine.UserSmartMatched = true;
            }



            fe_saveWorkflowData(function () { });

            // in case ProspectId was not saved and returned correctly from client side async ajax calls, set it based on server side sync save
            if (FormsEngine.ProspectId === null || FormsEngine.ProspectId < 1) {
                FormsEngine.ProspectId = data.ProspectId;
                FormsEngine.createCookie("FE_ProspectId", data.ProspectId);
                fe_consolelog('ProspectId retreived and set from Server side sync save. ProspectId=' + data.ProspectId);
            }

            if (data.MoveToStart) {
                fe_consolelog('No LeadData, Moving to Start.');
                loadWorkflowStep('START', "Set Error");
                return;
            }

            if (data.MoveToThankYou) {
                fe_consolelog('Smart Match Leads Saved, No Available or Allowed User Selections, Moving to ThankYou.');
                //DEV-940 DL event push
                if (data.SMLeadsCreatedCount > 0) {
                    fireInitialLeadDataLayerEvent();
                }

                loadWorkflowStep('THANKYOU', '');
                return;
            }

            if (data.MoveToNoMatch) {
                fe_googleTagEvent('gaEvent', 'client', 'wizard-limbo', FormsEngine.TemplateId);
                fe_consolelog('No Available or Allowed Smart Matches or User Selections, Moving to NoMatch.');
                var IsGenericNoMatch = FormsEngine.RenderingExperience === "Prospect"; //if we are in prospect flow then use generic no match
                loadWorkflowStep('NOMATCH', IsGenericNoMatch);
                return;
            }

            FormsEngine.UserShownManagedChoice = true;
            FormsEngine.MaxManagedChoiceUserSelections = data.MaxManagedChoiceUserSelections;
            renderedHtml = decodeURIComponent((data.RenderedManagedChoice + '').replace(/\+/g, '%20'));
            $('[name="' + FormsEngine.RenderingDiv + '"]').html(renderedHtml);

            //Pixels
            fe_loadPixelsForWizardMChoicePages();
            fe_ScrollToTop();

            //fire our optimizely 
        }
        else if (FormsEngine.CurrentPage === 'THANKYOU') {
            if (data.MoveToNoMatch) {
                fe_consolelog('No ThankYou Data, Moving to NoMatch.');
                loadWorkflowStep('NOMATCH', '');
                return;
            }
            if (data.MoveToStart) {
                fe_consolelog('No ThankYou Data, Moving to Start.');
                loadWorkflowStep('START', '');
                return;
            }

            if (FormsEngine.SMLeadsCreatedCount === 0 && FormsEngine.USLeadsCreatedCount > 0) {
                fe_googleTagEvent('gaEvent', 'client', 'wizard-success', FormsEngine.TemplateId);
            }

            renderedHtml = decodeURIComponent((data.RenderedThankYou + '').replace(/\+/g, '%20'));
            $('[name="' + FormsEngine.RenderingDiv + '"]').html(renderedHtml);

            //pixels
            fe_loadPixelsForWizardTYPages();

            fe_setProgressBar(100);
            fe_ScrollToTop();
        }
        else if (FormsEngine.CurrentPage === 'NOMATCH') {
            renderedHtml = decodeURIComponent((data.RenderedNoMatch + '').replace(/\+/g, '%20'));
            $('[name="' + FormsEngine.RenderingDiv + '"]').html(renderedHtml);
            fe_loadPixelsForWizardAdvisingFlow();
            fe_setProgressBar(100);
            fe_ScrollToTop();
        }
        else if (FormsEngine.CurrentPage === 'NOTHANKYOU') {
            //Not yet requested
            fe_ScrollToTop();
        }
        else { /// Start/Wizard
            // If data.UseInternationalTemplate then replace the templateID in the FormsEngine..
            if (data.UseInternationalTemplate) {
                FormsEngine.TemplateId = data.TemplateId;
                FormsEngine.UseInternationalTemplate = true;

                if (data.InternationalCountryCode) {
                    FormsEngine.InternationalCountryCode = data.InternationalCountryCode;
                }

                FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];

                FormsEngine.AdditionalFields.push(["OriginalTemplateId", data.OriginalTemplateId]);
                FormsEngine.AdditionalFields.push(["InternationalOverride", "true"]);
            }
            else if (data.IsLocalIP) {
                FormsEngine.IsLocalIP = data.IsLocalIP;
            }

            renderedHtml = decodeURIComponent((data.Template + '').replace(/\+/g, '%20'));

            var emsApplicationId = 27;
            if (FormsEngine.ApplicationId === emsApplicationId) {
                var styleTag = "<style id='useragreement-style'>" +
                    ".eddy-form-container .eddy-form-wizard-container .field-holder[data-controlcode='UserAgreement'] {" +
                    "display: none;" +
                    "}" +
                    "</style>";
                renderedHtml = styleTag + renderedHtml;
            }

            $('[name="' + FormsEngine.RenderingDiv + '"]').html(renderedHtml);
            if (data.FormTemplateType) {
                FormsEngine.FormTemplateType = data.FormTemplateType;
            }
            $(FormsEngine).trigger("loadFormWizard");
            FormsEngine.SiteOverrideKillExpressConsent = data.SiteOverrideKillExpressConsent;

            // Additional questions should only be shown on initial step if Template is a ProgramWizard
            FormsEngine.ShowAllQuestionsOnFirstStep = (FormsEngine.FormTemplateType === 3 && data.ShowAllQuestionsOnFirstStep)
        }

        FormsEngine.TemplateId = data.TemplateId !== undefined && data.TemplateId > 0 ? data.TemplateId : FormsEngine.TemplateId;
        FormsEngine.DefaultTemplateId = data.DefaultTemplateId !== undefined && data.DefaultTemplateId > 0 ? data.DefaultTemplateId : FormsEngine.DefaultTemplateId;

        fe_hideLoader(false);

        //Post load event
        if (FormsEngine.WorkflowChangedCompleted) {
            FormsEngine.WorkflowChangedCompleted(FormsEngine.CurrentPage);
        }

        //3rd party events
        if (FormsEngine.WorkflowChangedExternal) {
            FormsEngine.WorkflowChangedExternal(FormsEngine.CurrentPage);
        }
        //New event for Drupal
        $(FormsEngine).trigger("WorkflowChangedCompleted", FormsEngine.CurrentPage);
    }


    function clicksNetFix() {
        try {
            window.onbeforeunload = null;
        }
        catch (ex) {/**/}
    }

    //validates  if user is on the correct page if not redirects
    function validateCorrectPage(page) {
        var result = true;
        var CurrentURL = cleanURL(window.location.href);
        var newPage = "";

        //new workflow method
        if (FormsEngine.ConsumerSideWorkflowStep !== undefined && FormsEngine.ConsumerSideWorkflowStep !== null) {
            if (FormsEngine.CurrentPage !== FormsEngine.ConsumerSideWorkflowStep) {
                result = false;

                switch (page) {
                    case 'MANAGEDCHOICE': newPage = FormsEngine.WorkflowManagedChoicePage; break;
                    case 'THANKYOU':
                        newPage = fe_GetThankYouPageUrl();
                        break;
                    case 'NOMATCH': newPage = FormsEngine.WorkflowNoMatchPage; break;
                    case 'START': newPage = FormsEngine.WorkflowStartPage; break;
                }
            }
        }

        if (!result) {
            clicksNetFix();

            if (FormsEngine.PopupStartLeaveBehind) {
                clicksNetFix();
                FormsEngine.PopupStartLeaveBehind.location.href = newPage;
                document.location = FormsEngine.StartLeaveBehindURL;
            }
            else {
                document.location = newPage;
            }

        }

        return result;
    }

    function loadWorkflowStep(page, args) {
        if (page === "START" || page === "MANAGEDCHOICE" || page === "THANKYOU" || page === "NOMATCH") {
            fe_consolelog("loadWorkflowStep(page) = " + page + " FormsEngine.CurrentPage=" + FormsEngine.CurrentPage);
            var errorMsg = "";
            if (args === "Set Error") {
                errorMsg = "We’re sorry!  We had a small technical issue with your submission, please take a quick second to resubmit your request.";
                fe_googleTagEvent('gaEvent', 'client', 'wizard-re-start-error', FormsEngine.TemplateId);
            }

            //Set current page
            FormsEngine.CurrentPage = page;
            fe_getSessionObject("DuplicateForInstitutionList", function (data) {
                if (data !== "") {
                    if (typeof data === 'object') {
                        FormsEngine.DuplicateForInstitutionList = JSON.parse(data);
                    }
                }
            });
            //Save workflow step
            fe_saveWorkflowData(function () {

                configureFormToSkipManagedChoiceRedirectForEMS();

                //TL-EMS Custom Workflow to ThankYou/NoMatch event
                if ((page === 'THANKYOU' || page === 'NOMATCH') && FormsEngine.WorkflowThankYouRedirectEvent === true) {
                    $(FormsEngine).trigger("OnWorkflowThankYouRedirect", {
                        "UserFullName": FormsEngine.UserFullName,
                        "LeadId": FormsEngine.LeadId,
                        "RedirectTarget": page,
                        "FormSubmittedFields": FormsEngine.LeadDataEncoded ? decodeURIComponent(FormsEngine.LeadDataEncoded).split('&') : [],
                        "FormSubmittedAdditionalFields": FormData.LeadAdditionalData ? decodeURIComponent(FormData.LeadAdditionalData).split('&') : []
                    });
                    return;
                }

                if (FormsEngine.SelfContained === false && !validateCorrectPage(page)) {
                    return;
                }
                if (FormsEngine.PopupStartLeaveBehind) {
                    clicksNetFix();
                    FormsEngine.PopupStartLeaveBehind.location.href = window.location.href;
                    document.location = FormsEngine.StartLeaveBehindURL;
                    return;
                }

                //Fire event if supported to load content.
                if (FormsEngine.WorkflowChanged) {
                    FormsEngine.WorkflowChanged(FormsEngine.CurrentPage);
                }

                $(FormsEngine).trigger("WorkflowChanged", FormsEngine.CurrentPage);

                // Service Arguments per Workflow Page
                var serviceArgs = "";
                if (FormsEngine.CurrentPage === 'MANAGEDCHOICE') {

                    if (FormsEngine.RenderingStrategy === "SCHOOLPICKERWIZARD") {
                        serviceArgs = getSchoolPickerSubmissionServiceArgs();
                        FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/Submission/SubmitSchoolPickerWizard';
                    } else {
                        serviceArgs = getManagedChoiceServiceArgs();
                        FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetManagedChoice';
                    }

                }
                else if (FormsEngine.CurrentPage === 'THANKYOU') {
                    try {
                        var currentUserSelections = FormsEngine.CurrentUserSelections !== undefined ? FormsEngine.CurrentUserSelections : 0;
                    } catch (e) {/**/}

                    serviceArgs = getThankYouServiceArgs();
                    FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetThankYou';

                }
                else if (FormsEngine.CurrentPage === 'NOMATCH') {
                    serviceArgs = getNoMatchServiceArgs(args);
                    FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetNoMatch';
                }
                else { // Start/Wizard
                    fe_setLeadSourceUrlAndFormLeadUrl();
                    serviceArgs = getStartServiceArgs();
                    FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetWizardTemplate';
                }

                $.ajax({
                    async: false,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ClientServiceURL + serviceArgs,
                    success: function (data) {
                        loadDiv(data);
                        fe_wiz_setErrorMessage(errorMsg);
                    },
                    error: function (request, error) {
                        console.log(error);
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    }
                });
            });// save

            //set location hash
            //Start is broken down into steps and set in wizard.js.
            if (page === "MANAGEDCHOICE")
                window.location.hash = "#_SchoolSelection";
            else if (page === "NOTHANKYOU")
                window.location.hash = "#_NoThankYou";
            else if (page === "NOMATCH")
                window.location.hash = "#_NoMatch";
            else if (page === "THANKYOU")
                window.location.hash = "#_ThankYou";

        }
    }

    function configureFormToSkipManagedChoiceRedirectForEMS() {
        var emsApplicationId = 27;
        if (FormsEngine.ApplicationId === emsApplicationId) {

            var startPageExists = FormsEngine.WorkflowStartPage ? true : false;
            var managedChoicePageExists = FormsEngine.WorkflowManagedChoicePage ? true : false;
            var thankYouPageExists = FormsEngine.WorkflowThankYouPage ? true : false;
            var thankYouApplyNowURLExists = FormsEngine.ThankYouApplyNowURL ? true : false;
            var thankYouRequestInfoURLExists = FormsEngine.ThankYouRequestInfoURL ? true : false;
            var noMatchPageExists = FormsEngine.WorkflowNoMatchPage ? true : false;
            var workflowPagesExist = startPageExists && managedChoicePageExists && thankYouPageExists && noMatchPageExists;
            var workflowReadyToStartPagesExist = startPageExists && managedChoicePageExists && thankYouRequestInfoURLExists && thankYouApplyNowURLExists && noMatchPageExists;
        
            if (workflowPagesExist || workflowReadyToStartPagesExist) {
                if (FormsEngine.CurrentPage === "MANAGEDCHOICE") {
                    FormsEngine.SelfContained = true;
                }               
                else if (FormsEngine.CurrentPage === "THANKYOU") {
                    FormsEngine.SelfContained = false;
                }
            }
        }
    }

    function cleanURL(url) {
        url = url.split('?')[0];
        url = url.toLowerCase();

        var lastSlash = url.lastIndexOf('/');
        if (lastSlash === url.length - 1) {
            url = url.substring(0, lastSlash);
        }
        return url;
    }

    //DEV-940 to pass hashed phone email to setup GTM on DL event
    function fireInitialLeadDataLayerEvent() {
        try {
            if (window.__initialLeadEventFired) {
                return;
            }

            window.__initialLeadEventFired = true;

            function sha256(message) {
                return crypto.subtle.digest("SHA-256", new TextEncoder().encode(message.trim().toLowerCase()))
                    .then(function (hash) {
                        return Array.from(new Uint8Array(hash))
                            .map(function (b) { return b.toString(16).padStart(2, '0'); })
                            .join('');
                    });
            }

            function getLeadDataValue(code) {
                var data = decodeURIComponent(FormsEngine.LeadDataEncoded || "");
                var match = new RegExp("(^|&)" + code + "=([^&]*)", "i").exec(data);
                return match ? decodeURIComponent(match[2].replace(/\+/g, " ")) : "";
            }

            var userEmail = getLeadDataValue("Email");
            var userPhone = getLeadDataValue("Phone") || getLeadDataValue("Alternate_Phone");

            Promise.all([
                userEmail ? sha256(userEmail) : null,
                userPhone ? sha256(userPhone.replace(/\D/g, '')) : null
            ]).then(function (results) {
                window.dataLayer = window.dataLayer || [];
                window.dataLayer.push({
                    event: "initial_lead",
                    user_data: {
                        email_sha256: results[0],
                        phone_sha256: results[1]
                    }
                });
            }).catch(function (e) {
                if (typeof fe_consolelog === "function") {
                    fe_consolelog(e);
                }
            });

        } catch (e) {
            if (typeof fe_consolelog === "function") {
                fe_consolelog(e);
            }
        }
    }

    function start_load() {
        $(document).ready(function () {

            var Loaded = false;
            fe_consolelog("start_load event triggered, loading form...");
            //validate Forms Engine arguments
            if (!checkArguments()) {
                return;
            }

            // FE Default Values
            setDefaults();

            //Reset Session
            var ResetSession = fe_getParameterByName('ResetSession');
            if (ResetSession !== undefined && ResetSession.toLowerCase() === 'true') {
                fe_resetSession(false);
            }


            //Reset workflow to START if argument passed or property set
            var ResetWorkflow = fe_getParameterByName('ResetWorkflow').toLowerCase() === 'true';
            if (FormsEngine.ConsumerSideWorkflowStep === "START" && FormsEngine.SelfContained === false) {
                ResetWorkflow = true;
            }
            else if (FormsEngine.SelfContained === true && FormsEngine.StartLeaveBehindURL === undefined) {
                ResetWorkflow = true;
            }

            if (FormsEngine.ResetWorkflow) {
                ResetWorkflow = FormsEngine.ResetWorkflow === true;
                FormsEngine.ResetWorkflow = false;
            }
            if (ResetWorkflow) {
                fe_loadWorkflowData(function () {
                    loadWorkflowStep('START', '');
                });
            }
            else {

                //Current start page url an a non self contained to reset to start page
                if (FormsEngine.SelfContained === false && FormsEngine.ConsumerSideWorkflowStep === undefined) {
                    var CurrentURL = cleanURL(window.location.href);
                    if (CurrentURL === cleanURL(FormsEngine.WorkflowStartPage)) {
                        Loaded = true;
                        fe_loadWorkflowData(function () {
                            loadWorkflowStep('START', '');
                        });
                    }
                }

                if (!Loaded) {
                    //Load workflow data
                    fe_loadWorkflowData(function () {
                        var args = FormsEngine.CurrentPage.toUpperCase() === 'NOMATCH' ? FormsEngine.RenderingExperience === 'Prospect' : "";
                        loadWorkflowStep(FormsEngine.CurrentPage.toUpperCase(), args);
                    });
                }
            }

            if (FormsEngine.Theme && FormsEngine.Theme.toLowerCase() === "plain") {
                $('head').append('<link rel="stylesheet" id="PlainCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetCommonCSS?fileName=Plain" type="text/css" />');
                $('head').append('<link rel="stylesheet" id="PlainManagedChoiceCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetCommonCSS?fileName=PlainManagedChoice" type="text/css" />');
                $('head').append('<link rel="stylesheet" id="ComplianceCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetCommonCSS?fileName=Compliance" type="text/css" />');
            } else {
                if (!FormsEngine.NoBundleWizardCSS) {
                    $('head').append('<link rel="stylesheet" id="BaseCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetBundledWizardCSS?basePath=' + FormsEngine.RenderingStrategy + '&theme=' + FormsEngine.Theme + '" type="text/css" />');
                }
            }
        });
    }

    window.FormsEngine = window.FormsEngine || {};
    FormsEngine.LoadForm = function () {
        start_load();
    }
})(jQuery);