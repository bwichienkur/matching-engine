(function ($) {

    //taken from wizard base

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

        });

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
            script.src = FormsEngine.ServiceBaseURL + '/Templates/Common/js/' + filename + '.js';
        }
        document.body.appendChild(script);
    }

    //check FE arguments
    function checkArguments() {
        var result = true;

        if (window['FormsEngine'] == undefined
            || FormsEngine == null
            || !FormsEngine.hasOwnProperty('RenderingStrategy')
            || !FormsEngine.hasOwnProperty('RenderingDiv')
            || !FormsEngine.hasOwnProperty('IsBeta')
            || !FormsEngine.hasOwnProperty('PlacementToken')
            )
        {
            $(document.body).append("<div>Error: QDF FormsEngine object has to be defined before Client Script is included with the appropriate settings.<br/><br/>"
                + " i.e.<br/> var FormsEngine = FormsEngine || {}; <br/>"
                + " FormsEngine.RenderingStrategy ='[RenderingStrategy]'; <br/>"
                + " FormsEngine.RenderingDiv = '[RenderingDiv]'; <br/>"
                + " FormsEngine.PlacementToken = '[PlacementToken]'; <br/>"
                + " FormsEngine.IsBeta = [true/false]; <br/>");
            result = false;
        }
        else if (typeof window.FormsEngineGlobal === "undefined") {
            $(document.body).append("<div>FormsEngine Error: FormsEngine Global javascript must be included first. <br/><be/>"
                + " i.e. <br/> &lt;script type='text/javascript' src='//server/Static/GetGlobal'>&lt;/script> </div>");
            result = false;
        }

        if (FormsEngine.InstitutionName && FormsEngine.InstitutionName.length > 0) {
            FormsEngine.InstitutionName = $('<div />').html(FormsEngine.InstitutionName == undefined ? '' : FormsEngine.InstitutionName).text();
        }

        return result;
    }

    //Google analytics and internal tracking events
    function trackEvent(event) {
        var Gevent = event;
        if (FormsEngine.TemplateId != undefined && FormsEngine.TemplateId > 0) {
            Gevent = event + ".TemplateId." + FormsEngine.TemplateId;
        }

        //If Google Tag manager is detected fire same events there else fire regular gaq
        try {
            if (typeof dataLayer != 'undefined') {
                dataLayer.push({
                    'event': 'virtualPageView',
                    'url': Gevent
                });
            }
            else if (typeof _gaq != 'undefined') {
                _gaq.push(['_trackPageview', Gevent]);
            }
        } catch (e) { }

        try {
            if (typeof _etq != 'undefined') {
                _etq.push(['_etEvent', 'workflow', event, 'qdf']);
            }
        } catch (e) { }

    }


    //sets default values
    function setDefaults() {
        FormsEngine.ServiceBaseURL = "[SERVICEBASE]";
        FormsEngine.IsBeta = FormsEngine.IsBeta == undefined || FormsEngine.IsBeta === "" ? false : FormsEngine.IsBeta;
        FormsEngine.DebugMode = FormsEngine.DebugMode == undefined || FormsEngine.DebugMode === "" ? false : FormsEngine.DebugMode;
        FormsEngine.CompressJs = false; //FormsEngine.CompressJs == undefined ? false : FormsEngine.CompressJs;
        FormsEngine.Theme = FormsEngine.Theme == undefined || FormsEngine.Theme === "" ? "default" : FormsEngine.Theme;
        FormsEngine.RenderingStrategy = FormsEngine.AlternativeRenderingStrategy == undefined ? FormsEngine.RenderingStrategy : FormsEngine.AlternativeRenderingStrategy;
        FormsEngine.LoadWorkflowStep = loadWorkflowStep;
        FormsEngine.TrackEvent = function (event) { trackEvent(event); };
        FormsEngine.DisableScrolltoTop = FormsEngine.DisableScrolltoTop == undefined || FormsEngine.DisableScrolltoTop === "" ? false : FormsEngine.DisableScrolltoTop;

        if (FormsEngine.OptimizelyTheme) {
            FormsEngine.Theme = FormsEngine.OptimizelyTheme;
        }

    }


    function loadDiv(data) {
        // Inject Html per Workflow Page
        var renderedHtml = "";

        FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];

        if (FormsEngine.TrackEvent) {
            //Currently not counting if redirected on the flow on MChoice only
            // Thank you page redirect to no match TBD since is a flow for jobs
            if (FormsEngine.CurrentPage == 'QDF') {
                if (!data.MoveToStart
                    && !data.MoveToThankYou
                    && !data.MoveToNoMatch) {
                    // Track that the user actually sees the Managed Choice..
                    FormsEngine.TrackEvent("QDF");
                }
            }
            else {
                FormsEngine.TrackEvent(FormsEngine.CurrentPage);
            }
        }
         /// Start/Wizard        
        renderedHtml = decodeURIComponent((data.Template + '').replace(/\+/g, '%20'));
        
        $('[name="' + FormsEngine.RenderingDiv + '"]').html(renderedHtml);
        if (data.FormTemplateType) {
            FormsEngine.FormTemplateType = data.FormTemplateType;
        }
        $(FormsEngine).trigger("loadFormWizard");
        FormsEngine.SiteOverrideKillExpressConsent = data.SiteOverrideKillExpressConsent;
        
        FormsEngine.TemplateId = data.TemplateId != undefined && data.TemplateId > 0 ? data.TemplateId : FormsEngine.TemplateId;
        FormsEngine.DefaultTemplateId = data.DefaultTemplateId != undefined && data.DefaultTemplateId > 0 ? data.DefaultTemplateId : FormsEngine.DefaultTemplateId;

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


    function loadWorkflowStep(page, args) {
        if (page == "START" || page == "QDF") {
            fe_consolelog("loadWorkflowStep(page) = " + page + " FormsEngine.CurrentPage=" + FormsEngine.CurrentPage);
            var errorMsg = "";
            if (args == "Set Error") {
                errorMsg = "We’re sorry!  We had a small technical issue with your submission, please take a quick second to resubmit your request.";
            }

            //Set current page
            FormsEngine.CurrentPage = page;
            fe_getSessionObject("DuplicateForInstitutionList", function (data) {
                if (data != "") {
                    if (typeof data == 'object') {
                        FormsEngine.DuplicateForInstitutionList = JSON.parse(data);
                    }
                }
            });
            //Save workflow step
            fe_saveWorkflowData(function () {

                //Fire event if supported to load content.
                if (FormsEngine.WorkflowChanged) {
                    FormsEngine.WorkflowChanged(FormsEngine.CurrentPage);
                }

                $(FormsEngine).trigger("WorkflowChanged", FormsEngine.CurrentPage);

                // Service Arguments per Workflow Page
                var serviceArgs = "";

                if (FormsEngine.CurrentPage != "QDF") {
                    fe_consolelog("start_load event triggered, loading form...");
                    fe_setLeadSourceUrlAndFormLeadUrl();
                    serviceArgs = getStartServiceArgs();
                    FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetQDFTemplate';

                    $.ajax({
                        async: false,
                        type: 'GET',
                        dataType: 'jsonp',
                        cache: false,
                        url: FormsEngine.ClientServiceURL + serviceArgs,
                        success: function (data) {
                            loadDiv(data);
                            fe_wiz_setErrorMessage(errorMsg);
                            loadAds();
                        },
                        error: function (request, error) {
                            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                        }
                    });
                }
                else {
                    loadAds();
                    fe_hideLoader(false);
                }
               
            });// save

        }
    }

    function loadAds() {
        //start/wizard
        var jsonForm = {};
        jsonForm["eddyclickurl"] = FormsEngine.EddyClickUrl;
        if (FormsEngine.AdditionalFields) {
            jQuery.each(FormsEngine.AdditionalFields, function (index, item) {
                jsonForm[getAdAgrNameForField(FormsEngine.AdditionalFields[index][0])] = FormsEngine.AdditionalFields[index][1];
            });
        }


        var LeadDataArray = jQuery(FormsEngine.DefaultFormTag).serializeArray();
        //the above does not format the multiselect items the way we want so we need an additional method to serialize select 2 multiselects

        jQuery.each(LeadDataArray, function (index, item) {
            if (item.value != "") {
                var theAdAgrName = getAdAgrNameForField(item.name);
                if (jsonForm[theAdAgrName]) {
                    jsonForm[theAdAgrName] = jsonForm[theAdAgrName] + "," + item.value;
                }
                else {
                    jsonForm[theAdAgrName] = item.value;
                }
            }
        });
        jQuery.each(jQuery(FormsEngine.DefaultFormTag).find('.multiselectdropdown'), function (index, item) {
            if (jQuery(this).val() != undefined) {
                jsonForm[jQuery(this).attr('code')] = jQuery(this).val().join();
            }
        });

        if (FormsEngine.CurrentPage == "START") {
            if (FormsEngine.PassThruItems) {
                jQuery.each(FormsEngine.PassThruItems, function (index, item) {
                    jsonForm[getAdAgrNameForField(item.QuestionName)] = item.Answers;
                });
            }
        }

        //if (typeof jQuery(FormsEngine.DefaultFormTag).find('#eddyListings').eddyAd != 'undefined') {
        jQuery('#eddyListings').eddyAd({
            placementtoken: FormsEngine.PlacementToken, //2. Partner - DMS - Classes & Careers - SEO - NM (No Match)
            trackId: FormsEngine.TrackId,
            isWidget: true,
            testmode: false,
            useIframe: false, //true will render ads inside iframe
            skipIframeCheck: true, //necessary if placing code inside iframe
            ExtendedFields: jsonForm,
            widgetName: FormsEngine.WidgetName,
            widgetRequestGuid: FormsEngine.WidgetRequestGuid,
            Originator: 'QDF'
        });
    }

    function getAdAgrNameForField(fieldname) {
        switch (fieldname) {
            case "Category":
            case "Categories":
            case "MultiCategory":
            case "MultiDynamicCategory":
            case "Categories_Hidden":
                return "categories_selections";
            case "Subject":
            case "SubCategories":
            case "MultiSubject":
            case "MultiDynamicSubject":
            case "SubCategories_Hidden":
                return "subcategories_selections";
            case "Specialties":
            case "MultiDynamicSpecialty":
                return "specialties_selections";
            case "US_Citizen_YN":
                return "us_citizen";
            case "Desired_Degree_Levels":
                return "desired_degree_level";
            case "Desired_Start_Date_Radio":
                return "desired_start_date";
            case "CampusSoftPreference":
                return "dynamiccampussoftpreference";
            case "SubSource2":
                return "sub_source";
            case "LeadSourceUrl":
                return "lead_initiating_url";
            default:
                return fieldname.toLowerCase();
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
            if (ResetSession != undefined && ResetSession.toLowerCase() === 'true') {
                fe_resetSession(false);
            }


            //Reset workflow to START if argument passed or property set
            var ResetWorkflow = fe_getParameterByName('ResetWorkflow').toLowerCase() === 'true';
            if (FormsEngine.ConsumerSideWorkflowStep == "START" && FormsEngine.SelfContained == false) {
                ResetWorkflow = true;
            }
            else if (FormsEngine.SelfContained == true && FormsEngine.StartLeaveBehindURL == undefined) {
                ResetWorkflow = true;
            }

            if (FormsEngine.ResetWorkflow) {
                ResetWorkflow = FormsEngine.ResetWorkflow == true;
                FormsEngine.ResetWorkflow = false;
            }
            if (ResetWorkflow) {
                fe_loadWorkflowData(function () {
                    loadWorkflowStep('START', '');
                });
            }
            else {

                //Current start page url an a non self contained to reset to start page
                if (FormsEngine.SelfContained == false && FormsEngine.ConsumerSideWorkflowStep == undefined) {
                    var CurrentURL = cleanURL(window.location.href);
                    if (CurrentURL == cleanURL(FormsEngine.WorkflowStartPage)) {
                        Loaded = true;
                        fe_loadWorkflowData(function () {
                            loadWorkflowStep('START', '');
                        });
                    }
                }

                if (!Loaded) {
                    //Load workflow data
                    fe_loadWorkflowData(function () {
                        var args = "";
                        loadWorkflowStep(FormsEngine.CurrentPage.toUpperCase(), args);
                    });
                }
            }

            $('head').append('<link rel="stylesheet" id="PlainCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetCommonCSS?fileName=Plain" type="text/css" />');
            $('head').append('<link rel="stylesheet" id="BaseCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetBundledQDFCSS?basePath=' + FormsEngine.RenderingStrategy + '&theme=' + FormsEngine.Theme + '" type="text/css" />');
        });
    }

    window.FormsEngine = window.FormsEngine || {};
    FormsEngine.LoadForm = function () {
        start_load();
    };


})(jQuery);

//taken from wizard global functions
function fe_wiz_focusOnNextFocusable(e) {
    FormsEngine.CurrentStep = FormsEngine.CurrentStep || 1;

    if (!jQuery(FormsEngine.DefaultFormTag).is(":visible")) {
        return;
    }

    var top_of_element = jQuery(FormsEngine.DefaultFormTag).offset().top;
    var bottom_of_element = jQuery(FormsEngine.DefaultFormTag).offset().top + jQuery(FormsEngine.DefaultFormTag).outerHeight();
    var bottom_of_screen = jQuery(window).scrollTop() + jQuery(window).height();


    if ((bottom_of_screen > top_of_element) && (bottom_of_screen < bottom_of_element) && FormsEngine.CurrentStep != 1) {
        // The element is visible, do something
        FormsEngine.AutoFocus = true;
    }
    else if (FormsEngine.CurrentStep === 1 && e == 'null') {
        // The element is not visible, do something else
        FormsEngine.AutoFocus = false;
        return;
    }


    var focusables = jQuery(FormsEngine.DefaultFormTag).find('[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find(":input:visible");


    if (FormsEngine.ShowAllQuestionsOnFirstStep && FormsEngine.HasAdditionalQuestions) {
        jQuery.merge(focusables, jQuery(FormsEngine.DefaultFormTag).find('[name="step"][data-step="' + FormsEngine.StepDynamicQuestions + '"]').find(":input:visible"));
    }

    if (e == 'null') {

        if (focusables.length > 0) {
            var next = focusables.eq(0);
            //we dont want to do this on dropdowns because on mobile it autoexpands them and it doesnt make sense for drop downs anyway
            //also dont focus on controls with a mask
            if (!next.is('select') && fe_getIEVersion().major == "-1") {
                next.focus();
            }
        }
    }
    else {

        if (focusables.length > 0) {

            var currentIndex = focusables.index(e.target);
            var current = focusables.eq(currentIndex);

            if (focusables.eq(currentIndex + 1).length) {

                var next = focusables.eq(currentIndex + 1);
                //we dont want to do this on dropdowns because on mobile it autoexpands them and it doesnt make sense for drop downs anyway
                //also dont focus on controls with the mask
                if (!next.is('select') || (e.keyCode == 9 || e.which == 9)) {
                    next.focus();
                }
            }
            else {
                jQuery(FormsEngine.SubmitButton).trigger('click');
            }
        }
    }
}


function fe_wiz_showSelectAllButtons() {
    // add controls
    var CategoriesControl = jQuery(FormsEngine.DefaultFormTag).find('ul[code="Categories"]');
    if (jQuery(CategoriesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-categories multi-select-button" name="select-all-categories">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-categories multi-select-button" name="de-select-all-categories">Clear All</div>';
        jQuery(CategoriesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery(FormsEngine.DefaultFormTag).find('div[name="select-all-categories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Categories', true);
    })
    jQuery(FormsEngine.DefaultFormTag).find('div[name="de-select-all-categories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Categories', false);
    })

    // add controls
    var SubCategoriesControl = jQuery(FormsEngine.DefaultFormTag).find('ul[code="SubCategories"]');
    if (jQuery(SubCategoriesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-subcategories multi-select-button" name="select-all-subcategories">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-subcategories multi-select-button" name="de-select-all-subcategories">Clear All</div>';
        jQuery(SubCategoriesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery(FormsEngine.DefaultFormTag).find('div[name="select-all-subcategories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('SubCategories', true);
    })
    jQuery(FormsEngine.DefaultFormTag).find('div[name="de-select-all-subcategories"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('SubCategories', false);
    })

    // add controls
    var SpecialtiesControl = jQuery(FormsEngine.DefaultFormTag).find('ul[code="Specialties"]');
    if (jQuery(SpecialtiesControl).exists()) {
        var SelectAllDiv = '<div class="select-all-specialties multi-select-button" name="select-all-specialties">Select All</div>';
        var DeSelectAllDiv = '<div class="de-select-all-specialties multi-select-button" name="de-select-all-specialties">Clear All</div>';
        jQuery(SpecialtiesControl).parent('fieldset').after(SelectAllDiv + DeSelectAllDiv);
    }

    // bind events
    jQuery(FormsEngine.DefaultFormTag).find('div[name="select-all-specialties"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Specialties', true);
    })
    jQuery(FormsEngine.DefaultFormTag).find('div[name="de-select-all-specialties"]').click(function () {
        fe_wiz_selectAllMultiCheckBoxField('Specialties', false);
    })
}


function fe_wiz_selectAllMultiCheckBoxField(code, isChecked) {
    var CheckBoxList = jQuery(FormsEngine.DefaultFormTag).find('input[id^="' + code + '"]');

    //Google Analytics and internal tracking event
    var trackingValue = isChecked ? "Yes" : "Clear";
    fe_wiz_selectAllEventTracking("SelectAll", trackingValue);

    FormsEngine.SelectAllTriggered = true;
    jQuery(CheckBoxList).each(function () {
        if (isChecked && !jQuery(this).is(':checked')) {
            jQuery(this).checked = true;
            jQuery(this).attr({ 'checked': 'checked' });
            jQuery(this).prop("checked", "checked");
            jQuery(this).trigger('change');
        }
        else if (!isChecked && jQuery(this).is(':checked')) {
            jQuery(this).checked = false;
            jQuery(this).removeAttr('checked');
            jQuery(this).removeProp("checked", "checked");
            jQuery(this).trigger('change');
        }
    });
    if (isChecked == false) {
        jQuery(FormsEngine.DefaultFormTag).find('input[name="' + code + '_Selections"]').val('');
    }
    FormsEngine.SelectAllTriggered = false;
    jQuery(FormsEngine.DefaultFormTag).find('input[name="' + code + '_Selections"]').trigger('change');
    fe_saveForm();
    if (isChecked && fe_wiz_GetCountOfQuestionsOnStep() == 1 && FormsEngine.BackButtonClicked != true) {
        fe_consolelog('Auto Forward Step!');
        jQuery(FormsEngine.SubmitButton).trigger('click');
    }
}

//because specific internal tracking was requested to not use workflow as event name added this function
function fe_wiz_selectAllEventTracking(eventName, eventValue) {
    var Gevent = eventName + "." + eventValue;
    if (FormsEngine.TemplateId != undefined && FormsEngine.TemplateId > 0) {
        Gevent += ".TemplateId." + FormsEngine.TemplateId;
    }

    try {
        if (typeof _gaq != 'undefined') {
            _gaq.push(['_trackPageview', Gevent]);
        }
    } catch (e) { }

    try {
        if (typeof _etq != 'undefined') {
            _etq.push(['_etEvent', eventName, eventValue, 'form-wizard']);
        }
    } catch (e) { }
}

function fe_wiz_keyTabAndEnterEvents(e) {
    if (e.keyCode == 9 || e.which == 9) {
        fe_wiz_focusOnNextFocusable(e);
    }
    else if (e.keyCode == 13 || e.which == 13) {
        if (!FormsEngine.Flags) {
            FormsEngine.Flags = new Object();
        }

        var id = jQuery(e.srcElement).attr("id");
        if (FormsEngine.Flags.hasOwnProperty(id) && FormsEngine.Flags[id] != "enterpressed" && FormsEngine.Flags[id] != "") {
            FormsEngine.Flags[id] = "enterpressed";
        }
        else {
            FormsEngine.Flags[id] = "enterpressed";
            jQuery(FormsEngine.SubmitButton).trigger('click');
        }

    }
}

function fe_wiz_getLastStep() {
    if (FormsEngine.HasAdditionalQuestions)
        return FormsEngine.StepDynamicQuestions;
    else
        return FormsEngine.StepLast;
}

function fe_wiz_AutoForwardStep(currentElement) {
    var email = jQuery(FormsEngine.DefaultFormTag).find(':input[code="Email"]');
    var optin = jQuery(FormsEngine.DefaultFormTag).find(':input[code="NewsLetterOptIn"]');
    var code = jQuery(currentElement).attr("code");
    var optinStep = email.exists() && optin.exists() && jQuery(email).attr('step') === jQuery(optin).attr('step') && FormsEngine.CurrentStep == jQuery(email).attr('step');

    var emsApplicationId = 27;

    if (FormsEngine.BackButtonClicked
        || FormsEngine.FormsHasBeenRecovered
        || FormsEngine.CurrentStep == fe_wiz_getLastStep()
        || fe_wiz_CurrentStepContainsCategoryLikeQuestion()
        || fe_wiz_CurrentStepContainsQuestionsWithDefaults()
        || code === "Desired_Degree_Level"
        || fe_getIEVersion().major === 10
        || (optinStep && code != "NewsLetterOptIn")
        || FormsEngine.ApplicationId == emsApplicationId) {
        fe_consolelog('Not Auto moving forward because either the user has hit back, form has been recovered, user is on the last step, the categories control is on this step, or there are controls with defaults, or IE10, or this is an EMS form.');
        return true;
    }

    var eventDriven = false;

    var elementRules = {};
    if (currentElement) {
        elementRules = jQuery(currentElement).rules();
    }

    // Check if there are pending async validations..
    if (FormsEngine.PendingAsyncValidations && FormsEngine.PendingAsyncValidations.Length > 0) {

        FormsEngine.PendingAutoAdvance = true;

        var currentEvents = {};
        try {
            currentEvents = jQuery._data(jQuery(FormsEngine.SubmitButton)[0], "events");
        }
        catch (ex) {
            currentEvents = jQuery(FormsEngine.SubmitButton).data("events");
        }

        if (!currentEvents.hasOwnProperty("execute")) {
            // Set event listener..
            jQuery(FormsEngine.SubmitButton).on("execute", function () {
                if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
                    fe_consolelog('Auto Forward Step!');
                    if (FormsEngine.PendingAutoAdvance) {
                        jQuery(FormsEngine.SubmitButton).trigger('click');
                    }
                    FormsEngine.PendingAutoAdvance = false;
                }
            });

            eventDriven = true;
        }
    }

    if (!eventDriven) {
        if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
            FormsEngine.PendingAutoAdvance = false;
            fe_consolelog('Auto Forward Step!');
            jQuery(FormsEngine.SubmitButton).trigger('click');
        }
    }
}


function fe_wiz_KeyAutoAdvance(self, e, maxsize) {
    //wait until character is inserted
    setTimeout(function () {
        checkValidMobileButton();

        if (self && self.val && self.val().length >= maxsize) {
            if (fe_wiz_AllCurrentStepQuestionsAreComplete()) {
                if (!navigator.userAgent.match(/iPhone/i) && !navigator.userAgent.match(/iPad/i)) {
                    if (FormsEngine.PendingAutoAdvance !== true) {
                        fe_wiz_AutoForwardStep(self);
                    }
                }
            }
            else {
                fe_wiz_focusOnNextFocusable(e);
            }
        }
    }, 100);
}

function fe_wiz_CurrentStepContainsCategoryLikeQuestion() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('div[data-code="Categories"],div[data-code="SubCategories"],div[data-code="Specialties"]').exists();
}

function fe_wiz_GetCountOfQuestionsOnStep() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('div[data-controlcode]').length;
}

function fe_wiz_CurrentStepContainsQuestionsWithDefaults() {
    return jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"]').find('[data-hasinstancedefault="True"]').exists();
}

function fe_wiz_AllCurrentStepQuestionsAreComplete() {
    var Result = true;
    var LeadDataArray = jQuery(FormsEngine.DefaultFormTag).serializeArray();
    jQuery.each(LeadDataArray, function (index, item) {
        if (!Result) {
            return false; // break the loop
        }
        var field = jQuery(FormsEngine.DefaultFormTag).find(":input[name='" + LeadDataArray[index].name + "']");


        if (jQuery(field).attr('step') == FormsEngine.CurrentStep || (jQuery.contains(jQuery(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.StepDynamicQuestions)[0], field[0]) && FormsEngine.ShowAllQuestionsOnFirstStep)) {
            var code = jQuery(field).attr("code");
            if (code == 'undefined' || code == undefined) {
                return true; // continue to next item in loop
            }

            var value = LeadDataArray[index].value;
            if (value.length <= 0) {
                Result = false;
            }
        }
    });

    return Result;
}

function fe_wiz_getNextQuestionInStep(stepId, controlSortId) {
    //get the div for this step
    var thediv = jQuery(FormsEngine.DefaultFormTag).find('#Step' + stepId);
    //find whatever has id-sort param+1 and return it
    //force convert to int so we can add one
    var nextId = parseInt(controlSortId, 0) + 1;
    return jQuery(thediv).find('[id-sort="' + nextId + '"]');
}

function fe_wiz_setErrorMessage(message) {
    jQuery(FormsEngine.DefaultFormTag).find("#ErrorMessage").text(message);
    if (message && message != "") {
        var topDiff = jQuery(FormsEngine.DefaultFormTag).find("#ErrorMessage")[0].getClientRects()[0].top;
        jQuery('html, body').animate({
            scrollTop: topDiff
        }, 100);
    }
}

function checkValidMobileButton() {

    if (jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("*[required=required]")
        .filter(function () { return this.value != '' }).length <
        jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("*[required=required]").length
        || jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("[type=checkbox][required=required]:visible:checked").length < jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').find("[type=checkbox][required=required]:visible").length

        || (jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + ']').exists() && !jQuery(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-container *[data-step=' + FormsEngine.CurrentStep + '] *').validate().checkForm())) {

        jQuery(FormsEngine.DefaultFormTag).find("#screen-button").removeClass("enabled-button");
    }
    else if (!jQuery(FormsEngine.DefaultFormTag).find("#screen-button").hasClass("enabled-button")) {
        jQuery(FormsEngine.DefaultFormTag).find("#screen-button").addClass("enabled-button");
    }
}

function checkMobileButtonCoveringInput() {
    var $cmb = jQuery(FormsEngine.DefaultFormTag).find("#screen-button:visible");
    var $currentInput = jQuery(":focus");

    if ($currentInput.length == 0 || $cmb.length == 0) {
        return;
    }

    var currentInputBottomMargin = parseInt($currentInput.parents("div.field-holder").css("margin-bottom"));
    var currentInputRect = $currentInput[0].getClientRects()[0];
    var cmbRect = $cmb[0].getClientRects()[0];

    // If there is a current control with focus and CMB is showing..
    if (currentInputRect && cmbRect) {
        // Check if CMB is covering current input..
        if (currentInputRect.bottom >= cmbRect.top) {
            var topDiff = currentInputRect.bottom - cmbRect.top + currentInputBottomMargin;
            jQuery('html, body').animate({
                scrollTop: topDiff
            }, 10);
        }
    }
}

function ContinueMobileButtonInit($currentButton) {
    var $screenButton = jQuery(FormsEngine.DefaultFormTag).find("#screen-button");
    if ($screenButton.length == 0) {
        return;
    }

    jQuery("body").addClass("screen-button-container");
    $currentButton.addClass("non-mobile-btn");
    $screenButton.removeClass("hide-button");

    // Assign the click event to the CMB..
    $screenButton.click(function () {
        $currentButton.trigger("click");
    });

    // Calculate the position of the CMB..
    FormsEngine.ContinueMobileButton_cmbHeight = $screenButton.height();
    FormsEngine.ContinueMobileButton_cmbTopOffset = window.innerHeight - FormsEngine.ContinueMobileButton_cmbHeight;
    FormsEngine.jQuery = jQuery.noConflict();
    FormsEngine.jQuery(document).ready(function () {
        $screenButton.css("top", FormsEngine.ContinueMobileButton_cmbTopOffset);
    });

    // Determine whether/where to place the CMB..
    fe_checkShowScreenButton();

    // Assign the window scroll event to determine whether/where to place the CMB..
    jQuery(window).scroll(function (e) {
        if (!FormsEngine.ContinueMobileButton_cmbScrolled) {
            fe_checkShowScreenButton();
        }
        FormsEngine.ContinueMobileButton_cmbScrolled = !FormsEngine.ContinueMobileButton_cmbScrolled;
    });

    // When the screen is resize (rotating mobile device) recalculate the height..
    jQuery(window).resize(function (event) {
        FormsEngine.ContinueMobileButton_cmbTopOffset = window.innerHeight - FormsEngine.ContinueMobileButton_cmbHeight;
        fe_checkShowScreenButton();
        if (jQuery(FormsEngine.DefaultFormTag).find("input:focus").length > 0) {
            FormsEngine.CheckingMobileButtonCoverInput = true;
        }
    });

    jQuery(document).on('focus', "input", function () {
        // If CMB is showing make sure it doesn't cover the current input..
        checkMobileButtonCoveringInput();
    });

    if (typeof (MutationObserver) != 'undefined') {
        // MutationObserver..

        // create an observer instance
        var observer = new MutationObserver(function (mutations) {
            // It's for fun..
            if (FormsEngine.CheckingMobileButtonCoverInput == true) {
                var $focused = jQuery(FormsEngine.DefaultFormTag).find("input:focus");
                if ($focused.length > 0) {
                    checkMobileButtonCoveringInput();
                    FormsEngine.CheckingMobileButtonCoverInput = false;
                }
            }
        });

        // configuration of the observer:
        var config = { attributes: true, childList: true, characterData: true };

        // pass in the target node, as well as the observer options
        observer.observe($screenButton[0], config);

        // later, you can stop observing
        if (FormsEngine.CMBMutationObserverDisconnect) {
            observer.disconnect();
        }

        // End MutationObserver..
    }
    else {
        // Use DOM Event if MutationObserver is not supported..
        jQuery(document).on('DOMNodeInserted', function (e) {
            // If currently editing an input and CMB is showing make sure it doesn't cover the current input..
            if (FormsEngine.CheckingMobileButtonCoverInput == true) {
                var $focused = jQuery(FormsEngine.DefaultFormTag).find("input:focus");
                if ($focused.length > 0) {
                    checkMobileButtonCoveringInput();
                    FormsEngine.CheckingMobileButtonCoverInput = false;
                }
            }
        });
    }

    // Assign blur event on text input to determine whether/where to place the CMB..
    jQuery(document).on('blur', "input[type=text]", function () { fe_checkShowScreenButton(); });
}

jQuery(document).ready(function () {

    FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
    FormsEngine.ProgramCounterTag = FormsEngine.ProgramCounterTag || '#WizardStepContainer #ProgramMatches';

    // Internals variables
    var LastCount = {};
});
