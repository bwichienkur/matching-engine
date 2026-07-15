var fe_wizard = (function ($) {
    // Wizard.js
    //-------------------

    // Constants
    FormsEngine.DefaultFormTag = "#eddynexusform-wizard";
    FormsEngine.DefaultCountryCode = "US";
    FormsEngine.DefaultValidationLevel = 3;
    FormsEngine.DefaultSelectText = "- Select -";
    FormsEngine.GlobalErrorBoxExists = $("#ErrorBox").exists();
    FormsEngine.SectionErrorBoxExists = $('[id^="ErrorBoxSection"]').exists();
    FormsEngine.SubmitButton = "#wizard-form-submit-button";
    FormsEngine.SubmitButtonLabel = "#form-submit-button-label";
    if (FormsEngine.SubmitButtonLabelTextNormal != 'Next') {
        FormsEngine.SubmitButtonLabelTextNormal = "Continue";
    }
    FormsEngine.BackButton = "#form-navback-button";
    FormsEngine.CurrentStep = 1;
    FormsEngine.Source = 'IS_FormsEngine_WizardTemplate';
    FormsEngine.replace_Phone = "replace_Phone";
    FormsEngine.replace_Alternate_Phone = "replace_Alternate_Phone";
    FormsEngine.OnFormLoad = function () { onFormLoad(); }; //Will be triggered after the form is reloaded and binded
    FormsEngine.PendingAsyncValidations = new Array();
    FormsEngine.UserShownManagedChoice = false;
    FormsEngine.UserSubmittedManagedChoiceSelection = false;
    FormsEngine.UserSkippedToConfirmation = false;
    FormsEngine.ShowArrowsInMobileHeader = true;
    FormsEngine.IncludeProgramLevelGroups = true;

    // Internal variables
    var LastStateSelection = "";
    var LastCountrySelection = "";
    var LastProgramDependencies = "-";
    var LastCampusDependencies = "-";
    var LastFormProgramResults;
    var MovingStep = false;
    var emsApplicationId = 27;

    function onDynamicChangeEvent() {
        checkLastStepRequired();
    }

    function onControlValueSet(controlCode, controlValue) {
        if (controlCode == 'Phone' && controlValue != '') {
            checkForExpressConsent();
            if (FormsEngine.RecoverSplitPhoneFields) {
                FormsEngine.RecoverSplitPhoneFields();
            }
        }
        else if (controlCode == 'Alternate_Phone' && controlValue != '') {
            checkForExpressConsent();
            if (FormsEngine.RecoverSplitPhoneFields) {
                FormsEngine.RecoverSplitPhoneFields();
            }
        }
        else if (controlCode == 'Highest_Level_of_Education_Completed') {
            if (controlValue == '1') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']"))
                ) {
                    $("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }

                if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()) {
                    if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']"))) {
                        $("div[data-controlcode='K12']").show();
                    }
                    else {
                        $(FormsEngine.DefaultFormTag).find(":input[code='K12'][value='No']").prop('checked', true);
                    }
                }
            }
            else {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='K12'][value='No']").prop('checked', true);
                }

            }

            if (!$(FormsEngine.DefaultFormTag).find(":input[name^='Desired_Degree_Level_ST']").exists()) {
                if (controlValue === '10' || controlValue === '11') {
                    if ($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
                        && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))
                    ) {
                        $("div[data-controlcode='Desired_Degree_Level']").show();
                    }
                }
                else {
                    if ($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
                        && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))
                    ) {
                        $("div[data-controlcode='Desired_Degree_Level']").hide();
                        jQuery(":input[code='Desired_Degree_Level']").each(function () { jQuery(this).attr("checked", false); });
                    }
                }
            }

        }
        else if (controlCode === 'us_citizen') {

            if (controlValue === 'No') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']"))) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").val('126');
                    $("div[data-controlcode='Military_Affiliation']").hide();
                }
                if ($(FormsEngine.DefaultFormTag).find(":input[code='GCH']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='GCH']"))) {
                    $("div[data-controlcode='GCH']").show();
                }
            }
            else if (controlValue === 'Yes') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='GCH']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='GCH']"))) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").val('No');
                    $("div[data-controlcode='GCH']").hide();
                }
            }
        }
        checkValidMobileButton();
        georgiaBandaid();
    }

    function georgiaBandaid() {
        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        var countryCode = $(FormsEngine.DefaultFormTag).find(":input[code='Country']").val();

        if (countryCode === "GE" && postalCode.length === 5) {
            var stateCode = $(':input[code="State"]').val();
            var session = ""
            try {
                session = jQuery.cookie('_Session');
            }
            catch (e) { }

            $("select[code='Country']").val('US');
            fe_logClientException(null, "", "Georgia Bandaid3 applied State=" + stateCode + " PostalCode=" + postalCode + " TrackingSession=" + session);
        }
    }

    //Will be triggered every time the form is reloaded (session, querystring, etc)
    function onFormLoad() {
        if (FormsEngine.ReplaceDefaultOptionFields != undefined) {
            FormsEngine.ReplaceDefaultOptionFields();
        }

        var eddyUserAgreementControl = $(FormsEngine.DefaultFormTag).find('input[code="EDDYUserAgreement"]').parents('div.field-holder');

        if (eddyUserAgreementControl.length > 0) {
            $(eddyUserAgreementControl).hide();
        }

        if (FormsEngine.IsMobileForm === true) {
            $(FormsEngine.DefaultFormTag).find('select[code="Prefix"]>option[value=""]').text('- Prefix -');
        }

        //AutoSubmit
        if (fe_getParameterByName('AutoSubmit') == 'true') {
            $(FormsEngine.DefaultFormTag).find(':input[type="checkbox"]').each(function () {
                if (fe_getParameterByName($(this).attr('name')) == $(this).val()) {
                    $(this).prop('checked', true);
                }
            });
            $(FormsEngine.SubmitButton).trigger("click");
        }

        var formContainerHasName = $("[name='" + FormsEngine.RenderingDiv + "']").exists();

        //LeadId form capture
        if (FormsEngine.JornayaDelayUntilSecondStep !== true && FormsEngine.JornayaDelayUntilFirstUserInteraction !== true) {
            fe_initialize_Leadid();
            fe_initialize_ActiveProspect();
        }
        else if (FormsEngine.JornayaDelayUntilFirstUserInteraction === true) {
            if (!formContainerHasName) {
                $("#" + FormsEngine.RenderingDiv).on("click", function (event) {
                    try {
                        if (FormsEngine.JornayaInitialized === true) {
                            return;
                        }
                        if (event.target.toString().toLowerCase().indexOf("select") === -1) {
                            fe_consolelog('Initializing Jornaya');
                            fe_initialize_Leadid();
                            fe_initialize_ActiveProspect();
                        }
                    } catch (e) {
                        fe_consolelog('Initializing Jornaya by exception');
                        fe_initialize_Leadid();
                        fe_initialize_ActiveProspect();
                    }
                });
            }
            else {
                $("[name='" + FormsEngine.RenderingDiv + "']").on("click", function (event) {
                    try {
                        if (FormsEngine.JornayaInitialized === true) {
                            return;
                        }
                        if (event.target.toString().toLowerCase().indexOf("select") === -1) {
                            fe_consolelog('Initializing Jornaya');
                            fe_initialize_Leadid();
                            fe_initialize_ActiveProspect();
                        }
                    } catch (e) {
                        fe_consolelog('Initializing Jornaya by exception');
                        fe_initialize_Leadid();
                        fe_initialize_ActiveProspect();
                    }
                });
            }
        }

        ////LeadId form capture
        //if (FormsEngine.JornayaDelayUntilSecondStep !== true && FormsEngine.JornayaDelayUntilFirstUserInteraction !== true) {
        //    fe_initialize_Leadid();
        //    fe_initialize_ActiveProspect();
        //}
        //else {
        //    if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.LoadJornayaFirstStep === true) {
        //        fe_initialize_Leadid();
        //        fe_initialize_ActiveProspect();
        //    } else {
        //        if (!formContainerHasName) {
        //            $("#" + FormsEngine.RenderingDiv).on("click", function (event) {
        //                try {
        //                    if (FormsEngine.JornayaInitialized === true) {
        //                        return;
        //                    }
        //                    if (event.target.toString().toLowerCase().indexOf("select") === -1) {
        //                        fe_consolelog('Initializing Jornaya');
        //                        fe_initialize_Leadid();
        //                        fe_initialize_ActiveProspect();
        //                    }
        //                } catch (e) {
        //                    fe_consolelog('Initializing Jornaya by exception');
        //                    fe_initialize_Leadid();
        //                    fe_initialize_ActiveProspect();
        //                }
        //            });
        //        }
        //        else {
        //            $("[name='" + FormsEngine.RenderingDiv + "']").on("click", function (event) {
        //                try {
        //                    if (FormsEngine.JornayaInitialized === true) {
        //                        return;
        //                    }
        //                    if (event.target.toString().toLowerCase().indexOf("select") === -1) {
        //                        fe_consolelog('Initializing Jornaya');
        //                        fe_initialize_Leadid();
        //                        fe_initialize_ActiveProspect();
        //                    }
        //                } catch (e) {
        //                    fe_consolelog('Initializing Jornaya by exception');
        //                    fe_initialize_Leadid();
        //                    fe_initialize_ActiveProspect();
        //                }
        //            });
        //        }
        //    }
        //}

        FormsEngine.WidgetRequestGuid = FormsEngine.WidgetRequestGuid || fe_getParameterByName("WidgetRequestGuid")
        FormsEngine.WidgetName = FormsEngine.WidgetName || fe_getParameterByName("WidgetName")

        fe_setDynamicTags();
        fe_loadGoogleAddress();

    }

    function onProgramSet() {
        fe_consolelog("OnProgramSet");
        checkGSTemplate();
    }

    //Gets the error message based on control label and error code
    function getErrorMessage(element, defaultLabel, ErrorCode) {
        var label = element.attributes['label-name'];
        var minLength = element.attributes['minlength'] != undefined ? element.attributes['minlength'].nodeValue : "";
        var maxLength = element.attributes['maxlength'] != undefined ? element.attributes['maxlength'].nodeValue : "";
        var isValid = !(label == undefined || label == null || label == "");
        var controltypename = element.attributes['controltypename'].nodeValue;
        label = isValid ? label.nodeValue : defaultLabel;
        var errorMessage = "";

        if ($('input[code="' + ErrorCode + '_{' + controltypename + '}"]').exists()) {
            errorMessage = $('input[code="' + ErrorCode + '_{' + controltypename + '}"]').val().replace('{Control}', label).replace('{NumberMin}', minLength).replace('{NumberMax}', maxLength);
        }
        return errorMessage;
    }

    //jQuery Validation extension method to support regex in rules
    jQuery.validator.addMethod(
        "regex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        function (value, element, param) {
            return getErrorMessage(element, "value", "ERR_Valid");
        }
    );

    //jQuery Validation extension method to support validationLibrary regex in rules
    jQuery.validator.addMethod(
        "customregex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        function (value, element, param) {
            var errorcode = element.attributes['customerrorcode'] != undefined ? "ERR_" + element.attributes['customerrorcode'].nodeValue : "ERR_Valid";
            return getErrorMessage(element, "", errorcode);
        }
    );

    //jQuery Validation extension method to support profanity check rule
    var CustomProfanityErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "this field", "ERR_Profanity");
    }

    // jQuery.validator.addMethod(
    //     "profanity",
    //     function (value, element, param) {
    //         return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ProfanityCheck?Value=", CustomProfanityErrorMessage);
    //     },
    //     CustomProfanityErrorMessage
    //);

    //jQuery Validation extension method to support regex in rules
    jQuery.validator.addMethod(
        "atLeastOneTCPAChecked", function (value, element) {
            return $('.smtcpaitem:checked').length > 0;
        }, "You must consent to be contacted by at least one school before submitting this form.");


    //jQuery Validation extension method to support phone check rule
    var CustomPhoneErrorMessage = function (value, element, param) {
        return FormsEngine.ValidationMessages && FormsEngine.ValidationMessages["Phone"] ? FormsEngine.ValidationMessages["Phone"] : getErrorMessage(element, "value", "ERR_Valid");
    };
        
    jQuery.validator.addMethod(
        "phoneservercheck",
        function (value, element, param) {
            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val() || "";
            if (countryCode == "") {
                countryCode = FormsEngine.DefaultCountryCode;
            }
            var cleanPhone = value.replace(/\(/g, "");
            cleanPhone = cleanPhone.replace(/\)/g, "");
            cleanPhone = cleanPhone.replace(/_/g, "");
            cleanPhone = cleanPhone.replace(/-/g, "");
            cleanPhone = cleanPhone.replace(/ /g, "");


            var localCountry = (["US", "CA"].indexOf(countryCode) >= 0);
            if (cleanPhone == "" || FormsEngine.UseInternationalTemplate || FormsEngine.IsLocalIP === false || !localCountry) {
                if (FormsEngine.UseInternationalTemplate || !localCountry) {
                    // Still check there are no letters..
                    var regex = /^[0-9]*$/;
                    return regex.test(cleanPhone);
                }
                else {
                    return true;
                }
            }
            else {
                return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/PhoneNumberCheck/?CountryCode=" + countryCode + "&PhoneNumber=", CustomPhoneErrorMessage);
            }
        }
        ,
        CustomPhoneErrorMessage
    );

    var CustomDateErrorMessage = function (value, element, param) {
        return FormsEngine.ValidationMessages && FormsEngine.ValidationMessages["BirthDate"] ? FormsEngine.ValidationMessages["BirthDate"] : getErrorMessage(element, "value", "ERR_BirthDate").replace('{BirthDate}', $(element).val());
    };

    jQuery.validator.addMethod(
        "validdatecheck",
        function (value, element, param) {
            var birthDateString = $(element).val() || "";
            if (birthDateString == "") {
                return true;
            }
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/BirthDateCheck/?BirthDate=", CustomDateErrorMessage);
        }
        ,
        CustomDateErrorMessage
    );
    
    //jQuery Validation extension method to support zipCode country/state validation
    var CustomZipErrorMessage = function (value, element, param) {
        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        return getErrorMessage(element, "value", "ERR_ZipCountryState").replace('{ZipValue}', postalCode);
    };

    jQuery.validator.addMethod(
        "zipcitycountrycheck",
        function (value, element, param) {
            if ($(element).attr('code') == 'Postal_Code_Duplicate') copyZipCodeValues($(element).attr('code'), $.trim($(element).val()));

            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val();
            var stateCode = $(FormsEngine.DefaultFormTag).find("select[code='State']").val();
            var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
            var postalCodeDuplicate = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").val();

            if (FormsEngine.IsLocalIP && ["Postal_Code", "Postal_Code_Duplicate"].indexOf($(element).attr("code")) > -1 && ["US", "CA"].indexOf(countryCode) < 0) {
                countryCode = "";
            }

            if (countryCode == "" || stateCode == "" || postalCode == "" || (FormsEngine.UseInternationalTemplate && ["US", "CA"].indexOf(countryCode) < 0)) {
                return true;
            }
            else if ($(element).attr("code") == "Country") {
                if ($(FormsEngine.DefaultFormTag).find('select[code="Country"]').isControlBefore('Postal_Code') && !$(FormsEngine.DefaultFormTag).find('select[code="Postal_Code"]:visible').exists()) {
                    return true;
                }

                if ($(FormsEngine.DefaultFormTag).find('select[code="Country"]').isControlBefore('State') && !$(FormsEngine.DefaultFormTag).find('select[code="State"]:visible').exists()) {
                    return true;
                }
            }
            else if ($(element).attr("code") == "State") {
                if ($(FormsEngine.DefaultFormTag).find('select[code="State"]').isControlBefore('Postal_Code') && $(!FormsEngine.DefaultFormTag).find('select[code="Postal_Code"]:visible').exists()) {
                    return true;
                }
            }

            value = countryCode + "_" + stateCode + "_" + postalCode + "_" + postalCodeDuplicate;
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ZipCodeStateCountryCheck/?CountryCode=" + countryCode + "&StateCode=" + stateCode + "&ZipCode=" + postalCode + "&n=", CustomZipErrorMessage);
        }
        ,
        CustomZipErrorMessage
    );


    //jQuery Validation extension method to support zipCodeOnly validation
    var CustomZipInvalidErrorMessage = function (value, element, param) {
        if (FormsEngine.ValidationMessages && FormsEngine.ValidationMessages["Postal_Code"]) {
            return FormsEngine.ValidationMessages["Postal_Code"];
        } else if (FormsEngine.ApplicationId == emsApplicationId && !$(FormsEngine.DefaultFormTag).find("select[code='Country']").exists()) {
            return "Please provide a valid US Zip Code.";
        }

        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        return getErrorMessage(element, "value", "ERR_ZipCodeInvalid").replace('{ZipValue}', postalCode);
    };

    // Validator for just Zippies..
    jQuery.validator.addMethod(
        "ziponlycheck",
        function (value, element, param) {
            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val() || ""; 
            var stateCode = $(FormsEngine.DefaultFormTag).find("select[code='State']").val() || "";
            var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val() || "";

            // if country is on the same step
            // check when appearing before and after the zip code..
            if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('Country')) {

                if (countryCode !== "" && ["US", "CA"].indexOf(countryCode) < 0) {
                    return true;
                }
            }

            if (postalCode == "") {
                return true;
            }

            value = postalCode;
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ZipCodeStateCountryCheck/?CountryCode=" + countryCode + "&StateCode=" + stateCode + "&ZipCode=" + postalCode + "&ZipOnly=true" + "&ApplicationId=" + FormsEngine.ApplicationId + "&n=", CustomZipInvalidErrorMessage);

        }
        ,
        CustomZipInvalidErrorMessage
    );

    var CustomGoogleAddressErrorMessage = function (value, element, param) {
        return FormsEngine.ValidationMessages && FormsEngine.ValidationMessages["Google_address"] ? FormsEngine.ValidationMessages["Google_address"] : getErrorMessage(element, "", "ERR_GoogleAddress");        
    }

    jQuery.validator.addMethod(
        "googleaddresscheck",
        function (value, element, param) {
            return ($(element).parents('.sections').find('input[code="Postal_Code_Duplicate"]').val() != ""
                && $(element).parents('.sections').find('input[code="State"]').val() != ""
                && $(element).parents('.sections').find('input[code="City"]').val() != ""
                && $(element).parents('.sections').find('input[code="Address"]').val() != "")
        },
        CustomGoogleAddressErrorMessage
    );

    //jQuery Validation extension method to support email validation rule
    var CustomEmailErrorMessage = function (value, element, param) {
        return FormsEngine.ValidationMessages && FormsEngine.ValidationMessages["Email"] ? FormsEngine.ValidationMessages["Email"] : getErrorMessage(element, "", "ERR_Email");
    };

    jQuery.validator.addMethod(
        "emailservercheck",
        function (value, element, param) {
            //EmailCheckEx(string EmailAddress, bool XVerify, bool Background, string Experiment, string TrackId)
            var ags = "TrackId=" + (FormsEngine.readCookie('_Session') ? FormsEngine.readCookie('_Session') : "");
            ags += "&XVerify=" + (FormsEngine.UseXVerify === true).toString();
            ags += "&Immediate=" + (FormsEngine.XVerifyImmediate === true).toString();
            ags += "&Experiment=" + (FormsEngine.ExperimentName ? FormsEngine.ExperimentName : "");
            fe_consolelog(ags);
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/EmailCheckEx/?" + ags + "&EmailAddress=", CustomEmailErrorMessage);
        },
        CustomEmailErrorMessage
    );


    //Required field, minlength, maxlength custom messages
    jQuery.extend(jQuery.validator.messages, {
        required: function (value, element, param) {
            var message = "";
            if ($(element).attr('code') === 'UserAgreement' || $(element).attr('code') === 'EDDYUserAgreement') {
                /*message = "You must indicate that you have read and acknowledge our terms and conditions before submitting this form.";*/
                message = "You must consent to be contacted before submitting this form.";
            }
            else if (element.nodeName.toLowerCase() === 'select') {
                message = getErrorMessage(element, "field", "ERR_RequiredField");
            }
            else {
                message = getErrorMessage(element, "This", "ERR_RequiredField");
            }
            if (message == "") {
                if ($(element).attr('controltypename') === "Radio Buttons") {
                    message = "Please select a value";
                }
            }

            return message;
        },
        minlength: function (value, element, param) {
            return getErrorMessage(element, "this field", "ERR_MinLength");
        },
        maxlength: function (value, element, param) {
            return getErrorMessage(element, "this field", "ERR_MaxLength");
        }
    });


    function submitProspect() {
        var FormData = fe_getFormData();
        fe_setSettingsFromCookies();
        fe_getSessionId(function () {
            var Request = "IsBeta=" + FormsEngine.IsBeta;
            Request += "&SessionId=" + FormsEngine.SessionId;
            Request += "&LeadData=" + encodeURIComponent(FormData.LeadData);
            Request += "&AdditionalData=" + encodeURIComponent(FormData.LeadAdditionalData);
            Request += "&TrackId=" + FormsEngine.TrackId;
            Request += "&FESessionId=" + FormsEngine.FESessionId;
            Request += "&ApplicationId=" + FormsEngine.ApplicationId;

            if (FormsEngine.ProspectId == undefined || FormsEngine.ProspectId == null) {
                FormsEngine.ProspectId = 0;
            }

            if (FormsEngine.LastProspectSave != Request) {
                var sUrl = FormsEngine.ServiceBaseURL + "/TemplateManager/SaveProspect?" + Request;

                $.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: sUrl,
                    success: function (data) {
                        FormsEngine.ProspectId = data;
                        FormsEngine.createCookie("FE_ProspectId", data);
                        fe_consolelog('Prospect save complete. ProspectId=' + data);
                        FormsEngine.LastProspectSave = Request;
                        fe_submitProspectAdditionalInfo(true);
                    },
                    error: function (request, textStatus, errorThrown) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                        fe_logClientException(request, sUrl, errorThrown);
                    }
                });
            }
        });
    }

    //Jquery Validation Jsonp request based on remote extension from jqueryvalidation
    function jqueryValidatorRemoteJsonp(thisObj, value, element, param, message) {
        if (thisObj.optional(element)) {
            return "dependency-mismatch";
        }

        var previous = thisObj.previousValue(element);
        if (!thisObj.settings.messages[element.name]) {
            thisObj.settings.messages[element.name] = {};
        }
        previous.originalMessage = $.isFunction(message) ? message(value, element, param) : message;
        thisObj.settings.messages[element.name].remote = previous.message;

        if (thisObj.pending[element.name]) {
            return "pending";
        }

        if (previous.old === value) {
            return previous.valid;
        }

        FormsEngine.PendingAsyncValidations.push(element.name);
        previous.old = value;
        var validator = thisObj;
        thisObj.startRequest(element);

        $.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            port: "validate" + element.name,
            url: FormsEngine.ServiceBaseURL + param + value,
            mode: "abort",
            success: function (response) {
                validator.settings.messages[element.name].remote = previous.originalMessage;
                var valid = response === true || response === "true";
                if (valid) {
                    var submitted = validator.formSubmitted;
                    validator.prepareElement(element);
                    validator.formSubmitted = submitted;
                    validator.successList.push(element);
                    validator.showErrors();
                    //optional function to trigger server validation pass
                    if (element.CustomSuccessFunction) {
                        element.CustomSuccessFunction();
                    }
                } else {
                    var errors = {};
                    errors[element.name] = previous.message = $.isFunction(message) ? message(value, element, param) : message;
                    validator.invalid[element.name] = true;
                    validator.showErrors(errors);
                    //optional function to trigger server validation failure
                    if (element.CustomFailureFunction) {
                        element.CustomFailureFunction();
                    }
                }
                var position = $.inArray(element.name, FormsEngine.PendingAsyncValidations);

                if (position > -1) {
                    FormsEngine.PendingAsyncValidations.splice(position, 1);
                }


                previous.valid = valid;
                validator.stopRequest(element, valid);

                if (valid === true && FormsEngine.PendingAutoAdvance === true && (!FormsEngine.PendingAsyncValidations || FormsEngine.PendingAsyncValidations.Length == 0)) {
                    jQuery(FormsEngine.SubmitButton).trigger('execute');
                }
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                var position = $.inArray(element.name, FormsEngine.PendingAsyncValidations);

                if (position > -1) {
                    FormsEngine.PendingAsyncValidations.splice(position, 1);
                }
            }
        });
        //return false;
        return "pending";
    }



    //Used to revalidate composed controls State,Country 
    function validateStateCityCountry() {
        var State = $(FormsEngine.DefaultFormTag).find(":input[code='State']");
        var Country = $(FormsEngine.DefaultFormTag).find(":input[code='Country']");
        var Zip = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']");
        var DuplicateZip = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']");

        var locationFields = [State, Country, Zip, DuplicateZip];

        for (var i = 0; i < locationFields.length; i++) {
            var locationField = locationFields[i];

            if (locationField != undefined && $(locationField).val() != "" && $(locationField).hasClass('error')) {
                if ($(locationField).valid()) {
                    fe_hideValidationError(locationField);
                }
            }
        }
    }

    function getMetaData(data) {
        //Get the resource meta data texts.
        FormsEngine.ResourceData = data;

        if (FormsEngine.ApplicationId == emsApplicationId) {
            fe_getEMSInstitutionTCPAMessage(FormsEngine.InstitutionId, function (tcpaMessage) {
                if (tcpaMessage) {
                    FormsEngine.ResourceData["EMS_INSTITUTION_TCPA_MESSAGE"] = tcpaMessage;
                }
                setupMetaData(data);
            });
        } else {
            setupMetaData(data);
        }
    }

    function setupMetaData(data) {
        if (FormsEngine.WizardAlternativeExpressConsent != undefined && FormsEngine.WizardAlternativeExpressConsent.length > 10) {
            data['JS.WIZARD.USERAGREEMENT_UNIFIED'] = FormsEngine.WizardAlternativeExpressConsent;
        }
        checkUserAgreementText();
        //ProgramWizard support
        if (FormsEngine.FormTemplateType == 3) {
            defaultCurrentInstitutionSmartMatch();
        }

        //Get landing page settings
        fe_getLandingPageSettings(FormsEngine.FormLeadUrl, function (data) {
            FormsEngine.LandingPageDetails = data;
        })


        fe_getCampaignDetailByTrackId(FormsEngine.TrackId, function (data) {
            FormsEngine.CampaignDetail = data;
            fe_consolelog("Campaign.SmartMatchAllowed: " + FormsEngine.CampaignDetail.MaxSmartMatchCount);

            //hardcoded business rule!!!!!!!!!! requested by Venessa 2-26-2016
            if ((FormsEngine.RenderingStrategy == 'WIZARDPROFESSIONALBOOTSTRAP' || FormsEngine.RenderingStrategy == 'WIZARDPROFESSIONAL')
                && FormsEngine.Theme == 'alternative'
                && FormsEngine.CampaignDetail != null
                && FormsEngine.CampaignDetail.MaxSmartMatchCount == 0) {
                FormsEngine.SmartMatchZero = true;
            }
            if (FormsEngine.CampaignDetail != null) {
                if (FormsEngine.CampaignDetail.HasXVerify != null) {
                    if (FormsEngine.CampaignDetail.HasXVerify === true) {
                        FormsEngine.UseXVerify = true;
                        FormsEngine.XVerifyImmediate = true;
                       
                    }
                }

                if (FormsEngine.ApplicationId !== emsApplicationId) { //Non EMS Wizard
                    //Wizard with no remonetization and no SM TCPA is not a checkbox
                    if (FormsEngine.FormTemplateType === 2
                        && FormsEngine.CampaignDetail.AllowRemonetization === false
                        && FormsEngine.CampaignDetail.MaxSmartMatchCount === 0) {
                        var userAgreementControl = $(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]').parents('div.field-holder');
                        var eddyUserAgreementControl = $(FormsEngine.DefaultFormTag).find('input[code="EDDYUserAgreement"]').parents('div.field-holder');
                        FormsEngine.DoNotDisplayTCPA = true;
                        $(userAgreementControl).hide();
                        $(eddyUserAgreementControl).hide();
                    }
                }
            }
        });
    }


    // Validations for Gradschools template
    function checkGSTemplate() {
        fe_consolelog("checkGSTemplate");
        //GS / SAB / UAB
        if (FormsEngine.ApplicationId == 7 || FormsEngine.ApplicationId == 20 || FormsEngine.ApplicationId == 1) {
          
            var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
            var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');

            //Free
            if (PaidStatusTypeId == 1 || PaidStatusTypeId == 2) {

                if ($(".GSAddtDisclaimer").length == 0 && FormsEngine.ResourceData && FormsEngine.ResourceData['GRADSCHOOL.FREE.ADDITIONALDISCLAIMER']) {
                    $(".steps .form-page-step-message").after("<p id='GSAddtDisclaimer' class='small GSAddtDisclaimer'><i>" + FormsEngine.ResourceData['GRADSCHOOL.FREE.ADDITIONALDISCLAIMER'] + "</i></p>");
                }
            }//Paid
            else if ($(".GSAddtDisclaimer").length > 0) {
                $(".GSAddtDisclaimer").remove();
            }
        }
    }

    //Configure defaults on every template reload
    function configureDefaults() {
        FormsEngine.HasAdditionalQuestions = false;
        FormsEngine.UserSmartMatched = FormsEngine.UserSmartMatched == undefined || FormsEngine.UserSmartMatched == null ? false : FormsEngine.UserSmartMatched;

        //Gets meta data and related dependant resources.
        getMetaData([METADATA]);

        //jquery validator defaults
        $.validator.setDefaults({
            onkeyup: false,
            messages: FormsEngine.ValidationMessages || {}
        });

        //Global Error box
        if (FormsEngine.GlobalErrorBoxExists) {
            $(FormsEngine.DefaultFormTag).validate({
                errorLabelContainer: "#ErrorBox",
                wrapper: "li"
            });
        }//By section Error box
        else if (FormsEngine.SectionErrorBoxExists) {
            $(FormsEngine.DefaultFormTag).validate({
                wrapper: "li",
                errorPlacement: function (error, element) {
                    if (element.attr("code") == "UserAgreement") {
                        var ul = $(element.parent()).find("ul");
                        if ($(ul).exists()) {
                            error.appendTo(ul);
                        }
                        else {
                            element.parent().prepend("<ul></ul>");
                            error.appendTo(element.parent().find("ul"));
                        }
                    }
                    else {
                        error.appendTo($('#ErrorBox' + element.attr('section')));
                    }
                }
            });
        }
        else //Inline error box
        {
            $(FormsEngine.DefaultFormTag).validate({
                wrapper: "",
                errorPlacement: function (error, element) {
                    var code = $(element).attr("code");
                    //responsive icon
                    if ($(element).parent().hasClass('input-group')) {
                        $(error).appendTo($('[data-controlcode="' + code + '"]'));
                    } else {
                        $(error).appendTo($(element).parent());
                    }

                },
                success: function (error, element) {
                    $(error).remove();
                    $(element).removeClass("error");
                }
            });


        }

        //Program selections
        $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").change(function () {
            var ProgramsDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
            var ProgramId = $(ProgramsDDL).val();
            ProgramId = ProgramId == undefined ? "" : ProgramId;
            
            if (ProgramId != "") {
                FormsEngine.ProgramProductId = $(ProgramsDDL).attr('data-programproductid');
                FormsEngine.ProgramTemplateId = $(ProgramsDDL).attr('data-templateid');
                FormsEngine.ProductId = $(ProgramsDDL).attr('data-productid');
                FormsEngine.ProgramId = ProgramId;
                FormsEngine.ProgramName = $(ProgramsDDL).text();
                FormsEngine.RemovedTwoUSchools = $(ProgramsDDL).attr('data-hideschoolfromtcpa') == 'false' ? '' : FormsEngine.InstitutionId;
                FormsEngine.ShowTwoULeadShareControl = $(ProgramsDDL).attr('data-showtwoucheckbox') == 'False' ? false : true;
            }
            checkGSTemplate();
            if (FormsEngine.StepLast === 1) {
                onLoadDynamicQuestions();
            }
        });


        //Campus selections
        $(FormsEngine.DefaultFormTag).find(":input[code='Campus']").change(function () {

            var CampusDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Campus'] option:selected");
            var CampusId = $(CampusDDL).val();
            CampusId = CampusId == undefined ? "" : CampusId;

            if (CampusId != "") {
                FormsEngine.CampusId = CampusId;
            }
        });

        //Google Address validation rule
        if ($(FormsEngine.DefaultFormTag).find("input[code='Google_address']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[code='Google_address']").rules("add", {
                googleaddresscheck: true,
            });
        }

        //Email validation rule
        if ($(FormsEngine.DefaultFormTag).find("input[code='Email']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[code='Email']").rules("add", {
                email: true,
                emailservercheck: true
            });
        }

        //Phone number server validations
        if ($(FormsEngine.DefaultFormTag).find("input[code='Phone']").exists()) {

            $(FormsEngine.DefaultFormTag).find("input[code='Phone']").rules("add", { phoneservercheck: true });
        }
        //Alternate Phone number server validations
        if ($(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").exists()) {

            $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").rules("add", { phoneservercheck: true });
        }

        //birth date validations
        if ($(FormsEngine.DefaultFormTag).find("input[code='BirthDate']").exists()) {

            $(FormsEngine.DefaultFormTag).find("input[code='BirthDate']").rules("add", { validdatecheck: true });
        }

        //Smart Military control
        $(FormsEngine.DefaultFormTag).find("input[name='military_yesno']").click(function (event) {
            fe_processSmartMilitary();
        });

        //Smart Level of Education control
        $(FormsEngine.DefaultFormTag).find(":input[name='ddlHighestLevelNoCredits']").change(function (event) {
            fe_processSmartHighestEducationLevel();
        });

        $(FormsEngine.DefaultFormTag).find(":input[name='ddlHighestLevelCredits']").change(function (event) {
            fe_processSmartHighestEducationLevel();
        });

        //Zip City Country check
        if ($(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='State']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Country']").exists()
        ) {
            $(FormsEngine.DefaultFormTag).find(":input[code='State']").rules("add", {
                zipcitycountrycheck: true
            });
            $(FormsEngine.DefaultFormTag).find(":input[code='Country']").rules("add", {
                zipcitycountrycheck: true
            });


            $(FormsEngine.DefaultFormTag).find(":input[code='State']").change(function () { validateStateCityCountry(); });
            $(FormsEngine.DefaultFormTag).find(":input[code='Country']").change(function () { validateStateCityCountry(); });
            $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").blur(function () { validateStateCityCountry(); });

            if (FormsEngine.IsLocalIP === true) {
                $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").rules("add", { ziponlycheck: true });
                $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").blur(function (event) {
                    copyZipCodeValues($(this).attr('code'), $.trim($(this).val()));
                    // DEV-2618:
                    // Trigger City/State/Country lookup when Postal Code changes
                    // so hidden fields and corresponding key attributes are populated                    
                    var postalCode = $.trim($(this).val());
                    if (postalCode) {
                        fe_getCityStateCountry(postalCode);
                    }
                });
            }

            if ($(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").exists()) {

                $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").rules("add", { zipcitycountrycheck: true });
            }
        } else if ($(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").exists() && FormsEngine.ApplicationId == emsApplicationId && FormsEngine.IsLocalIP) {
            $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").rules("add", { ziponlycheck: true });
        }


        //NUMERIC DataType Validations
        $(FormsEngine.DefaultFormTag).find("input[datatype='NUMERIC']").each(function () {
            $(this).rules("add", {
                regex: /^[() -]*[0-9]+$/
            });
        });

        //ALPHA DataType Validations
        $(FormsEngine.DefaultFormTag).find("input[datatype='ALPHA']").each(function () {
            $(this).rules("add", {
                regex: /^[-\sa-zA-Z]+$/
            });
        });

        //ALPHANUMERIC DataType Validations
        $(FormsEngine.DefaultFormTag).find("input[datatype='ALPHANUMERIC']").each(function () {
            $(this).rules("add", {
                regex: /^[-\sa-zA-Z0-9]+$/
            });
        });

        //Validation Library Regex support
        $(FormsEngine.DefaultFormTag).find("input[customregex]").each(function () {
            $(this).rules("add", {
                customregex: $(this).attr('customregex')
            });
        });


        //Duplicate Postal code support
        $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code_Duplicate']").blur(function (event) {
            copyZipCodeValues($(this).attr('code'), $.trim($(this).val()));
        });
        $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").blur(function (event) {
            copyZipCodeValues($(this).attr('code'), $.trim($(this).val()));
        });

        //Form save events
        $(FormsEngine.DefaultFormTag).find(':input').each(function () {
            $(this).blur(function (event) {
                event.preventDefault();
                fe_saveForm();
            });
        });

        //Last step required field changes
        $("#Step" + FormsEngine.StepLast).find(':input').each(function () {
            $(this).change(function () {
                var code = $(this).attr('code');
                if (code == undefined || code == null || code == 'UserAgreement' || code == 'EDDYUserAgreement') {
                    return;
                }
                checkLastStepRequired();
            });
        });


        //Save event for Cat/SubCat hidden fields
        $(FormsEngine.DefaultFormTag).find('input[type="hidden"]').each(function () {
            $(this).change(function (event) {
                event.preventDefault();
                fe_saveForm();
            });
        });

        // Moves cursor to end of valid input. 
        // This prevents the cursor from being placed at the end of the field.
        $(FormsEngine.DefaultFormTag).find("input[code='Phone']").focus(function (event) {
            var inputElement = event.target;

            if (!inputElement) {
                return;
            }

            setTimeout(function () {
                var index = 1;
                while (index < inputElement.value.length) {
                    if (inputElement.value[index] === "_") {
                        break;
                    }
                    index++;
                }
                inputElement.setSelectionRange(index, index);
            }, 100);
        });
        
        //Prospect special fields blur event
        if (FormsEngine.RenderingExperience != "Prospect") {

            $(FormsEngine.DefaultFormTag).find("input[code='Email']").blur(function (event) {
                if ($(FormsEngine.DefaultFormTag).find("input[code='Email']").val() != "") {
                    submitProspect();
                }
            });
            $(FormsEngine.DefaultFormTag).find("input[code='Phone']").blur(function (event) {
                checkForExpressConsent();
                submitProspect();
            });
            $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").blur(function (event) {
                checkForExpressConsent();
                submitProspect();
            });

            $(FormsEngine.DefaultFormTag).find("input[code='NewsLetterOptIn']").click(function (event) {
                fe_submitProspectAdditionalInfo(true);
            });

            if ($("input[name='Categories_Selections']").exists() && FormsEngine.ProspectId > 0) {
                $("input[name='Categories_Selections']").change(function () {
                    submitProspect();
                });
            }

            if ($("input[name='SubCategories_Selections']").exists() && FormsEngine.ProspectId > 0) {
                $("input[name='SubCategories_Selections']").change(function () {
                    submitProspect();
                });
            }

            if ($("input[name='Specialties_Selections']").exists() && FormsEngine.ProspectId > 0) {
                $("input[name='Specialties_Selections']").change(function () {
                    submitProspect();
                });
            }

            $("select[code=Country]").on("change", function () {
                fe_ApplyPhoneMask();
            });

        } else {
            FormsEngine.UseProgramCounter = false;
            $("#ProgramMatches").hide();
        }

        //Campus soft preference filter
        $(FormsEngine.DefaultFormTag).find("input[code='CampusSoftPreference']").click(function (event) {
            checkCampusSoftPreference();
        });

        $(FormsEngine.DefaultFormTag).find(".image-checkbox-field-holder").click(function (event) {
            $(this).find("input[code='CampusSoftPreference']").prop('checked', true);
            checkCampusSoftPreference();
        });

        //Prefix has a different default selector text requested by Kim 09-06-2013
        if (FormsEngine.IsMobileForm === true) {
            $(FormsEngine.DefaultFormTag).find('select[code="Prefix"]>option[value=""]').text('- Prefix -');
        }
        else {
            $(FormsEngine.DefaultFormTag).find('select[code="Prefix"]>option[value=""]').text(FormsEngine.DefaultSelectText);
        }

        //Form Next/Submit Event
        $(FormsEngine.SubmitButton).click(function (event) {

            //add manual validation check here
            //force full form validate before checking if the form is valid.
            var validator = $(FormsEngine.DefaultFormTag).validate();
            validator.form();

            $('.radio-inline label.error').each(function () {
                var parent = $(this).parent().parent();
                parent.append($(this));
            });

            //Validate form
            if ($(FormsEngine.DefaultFormTag).valid() && checkIfStateExistsAndInvalid()) {

                //form is valid. before moving forward check for zip code controls to ensure they are copied
                checkCurrentStepForZipToCopy();

                checkNavigation(1);

                if ($('.error[for="CampusSoftPreference"]').length != 0) {
                    $('.error[for="CampusSoftPreference"]').insertAfter($('.error[for="CampusSoftPreference"]').parents('ul'));
                }

                $('.field-holder.radio .error').each(function () {
                    var parent = $(this).parent();
                    parent.append($(this));
                });

                if ($('.UserAgreement .error').length != 0) {
                    $('.UserAgreement .error').insertBefore($('.UserAgreement .error').parent());
                }

                RemoveScrollbar();
            }
            else {
                fe_checkShowScreenButton();
                //its an invalid form. scroll to error 
                var errorLabel = $(FormsEngine.DefaultFormTag).find("label.error:visible").first();
                if (!errorLabel.visible()) {
                    if (errorLabel.siblings("input[code=UserAgreement]").exists()) {
                        var userAgreementCB = $(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]');
                        userAgreementCB.focus();
                        $('html, body').animate({
                            scrollTop: (userAgreementCB.offset().top - 100)
                        }, 1000);
                    }
                    else {
                        errorLabel.focus();
                        $('html, body').animate({
                            scrollTop: (errorLabel.offset().top - 100)
                        }, 1000);
                    }
                }
            }
        });



        //Forms Back Event
        $(FormsEngine.BackButton).click(function (event) {
            FormsEngine.BackButtonClicked = true;
            checkNavigation(-1);
        });

        $(FormsEngine.DefaultFormTag).find(':input').change(function () {
            if (FormsEngine.OnBeforeAutoAdvance) {
                FormsEngine.OnBeforeAutoAdvance(FormsEngine.CurrentStep);
            }
            else {

                if (!FormsEngine.Flags) {
                    FormsEngine.Flags = new Object();
                }

                var id = $(this).attr("id");
                if (FormsEngine.Flags.hasOwnProperty(id) && FormsEngine.Flags[id] != "changed" && FormsEngine.Flags[id] != "") {
                    FormsEngine.Flags[id] = "changed";
                }
                else {
                    FormsEngine.Flags[id] = "changed";

                    //if its the email box, and the immediate next question is opt in we should not auto advance on change     
                    if ($(this).attr("code") == "Email") {
                        var sortid = $(this).attr("id-sort");
                        var nextcontrol = fe_wiz_getNextQuestionInStep(FormsEngine.CurrentStep, sortid);
                        if (nextcontrol && $(nextcontrol).attr("code") == "NewsLetterOptIn") {
                            return;
                        }
                    }
                    else if ($(this).attr("code") == "Highest_Level_of_Education_Completed" && jQuery("div[data-controlcode='Desired_Degree_Level']").exists()) {
                        var edLevel = $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val();
                        if (edLevel == "10" || edLevel == "11") {
                            return;
                        }
                    }
                    else if (($(this).attr("code") == "us_citizen" || $(this).attr("code") == "Military_Affiliation") && jQuery("div[data-controlcode='GCH']").exists()) {
                        if (jQuery(FormsEngine.DefaultFormTag).find(':input[code="us_citizen"][value="No"]').is(":checked")) {
                            return;
                        }
                    }
                    else if (jQuery("[code=Desired_Degree_Level]:visible").exists()) {
                        return;
                    }

                    fe_wiz_AutoForwardStep(this);
                }
            }
        });

        $(FormsEngine.DefaultFormTag).find(":input[code='Age']").keypress(function (e) {
            fe_wiz_KeyAutoAdvance($(this), e, 2);
        });

        $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").keyup(function (e) {
            var countryCode = jQuery(FormsEngine.DefaultFormTag).find("select[code='Country']").val() || "";
            var localCountry = (["US", "CA", ""].indexOf(countryCode) >= 0);
            if (!FormsEngine.UseInternationalTemplate && FormsEngine.IsLocalIP === true && localCountry) {
                // Check if Canadian Zip code (xxx xxx)..
                var value = jQuery(this).val();

                var minZipCodeLength = 5;
                if (value.length >= minZipCodeLength) {
                    if (value.indexOf(' ') > -1) {
                        fe_wiz_KeyAutoAdvance($(this), e, 7);
                    }
                    else if (value.match(/[a-z]/i)) {
                        fe_wiz_KeyAutoAdvance($(this), e, 6);
                    }
                    else {
                        fe_wiz_KeyAutoAdvance($(this), e, 5);
                    }   
                }
            }
        });

        //certain questions will get default values set to them based on a prior answer. set events here
        //if the wizard asks both year of graduation and highest level of education if the user selects that they have not completed hs we set default for
        //graduation year to be the year before this one
        if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']"))) {
            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").change(function () {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "1") {
                    //the user selected HS not completed so set the value of the corresponding input and disable it.
                    var d = new Date();
                    var curr_year = d.getFullYear() - 1;
                    $(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']").val(curr_year);
                    $("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }
                else {
                    $("div[data-controlcode='Year_of_Highest_Education_Completed']").show();
                }

            });
        }

        if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").change(function () {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "1") {
                    $("div[data-controlcode='K12']").show();
                }
                else {
                    $("div[data-controlcode='K12']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='K12'][value='No']").prop('checked', true);
                }

            });
        }

        //smart highest level of education
        if ($(FormsEngine.DefaultFormTag).find("#ddlHighestLevelNoCredits").exists()
            && $(FormsEngine.DefaultFormTag).find("#ddlHighestLevelNoCredits").exists()
            && $(FormsEngine.DefaultFormTag).find("#ddlHighestLevelNoCredits").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']"))) {
            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find("#ddlHighestLevelNoCredits").change(function () {
                if ($(FormsEngine.DefaultFormTag).find("#ddlHighestLevelNoCredits").val() == "1") {
                    //the user selected HS not completed so set the value of the corresponding input and disable it.
                    var d = new Date();
                    var curr_year = d.getFullYear() - 1;
                    $(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']").val(curr_year);
                    $("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }
                else {
                    $("div[data-controlcode='Year_of_Highest_Education_Completed']").show();
                }
            });
        }

        //Smart desired grad degree level
        if (!$(FormsEngine.DefaultFormTag).find(":input[name^='Desired_Degree_Level_ST']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").change(function () {
                var edLevel = $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val();
                if (edLevel == "10" || edLevel == "11") {
                    $("div[data-controlcode='Desired_Degree_Level']").show();
                }
                else {
                    $("div[data-controlcode='Desired_Degree_Level']").hide();
                    jQuery(":input[code='Desired_Degree_Level']").each(function () { jQuery(this).attr("checked", false); });
                }

            });
        }

        //if the wizard asks for both us citizen YN and military background if the user selects not a us citizen then default military background to no
        if ($(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").change(function () {
                var isRadio = $(FormsEngine.DefaultFormTag).find("[code='us_citizen'][type=radio]").exists();
                //both controls exist in the form and are in the order we need so lets add the onchange function
                if (isRadio &&
                    !$(FormsEngine.DefaultFormTag).find("[type=radio][code='us_citizen'][value='Yes']").is(":checked")) {
                    //the user selected HS not completed so set the value of the corresponding input and disable it.
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").val('126');
                    $("div[data-controlcode='Military_Affiliation']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else if (!isRadio && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").val() == "No") {
                    //the user selected HS not completed so set the value of the corresponding input and disable it.
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").val('126');
                    $("div[data-controlcode='Military_Affiliation']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else {
                    $("div[data-controlcode='Military_Affiliation']").show();
                }

            });
        }

        if ($(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='GCH']"))) {

            $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").change(function () {
                var isRadio = $(FormsEngine.DefaultFormTag).find("[code='us_citizen'][type=radio]").exists();
                if (isRadio &&
                    $(FormsEngine.DefaultFormTag).find("[type=radio][code='us_citizen'][value='Yes']").is(":checked")) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").val('No');
                    $("div[data-controlcode='GCH']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").trigger("change");
                }
                else if (!isRadio && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").val() == "Yes") {
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").val('No');
                    $("div[data-controlcode='GCH']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else {
                    $("div[data-controlcode='GCH']").show();
                }
            });
        }

        //smart financial aid/financial aid proof
        if ($(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").change(function () {
                var isRadio = $(FormsEngine.DefaultFormTag).find("[code='FinancialAid'][type=radio]").exists();
                //both controls exist in the form and are in the order we need so lets add the onchange function
                if (isRadio &&
                    $(FormsEngine.DefaultFormTag).find("[type=radio][code='FinancialAid'][value='No']").is(":checked")) {
                    //the user does not have funds so dont need to ask for proof
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").val('No');
                    $("div[data-controlcode='FinancialAidProof']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").trigger("change");
                }
                else if (!isRadio &&
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").val() == "No") {
                    //the user the user does not have funds so no need to ask for proof
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").val('No');
                    $("div[data-controlcode='FinancialAidProof']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").trigger("change");
                }
                else {
                    $("div[data-controlcode='FinancialAidProof']").show();
                }

            });
            $("div[data-controlcode='FinancialAidProof']").hide(); //hide by default
        }


        //Responsive control events
        $(FormsEngine.DefaultFormTag).find(':input[id^="DDLRD_"]').change(function () {
            var code = $(this).attr('parent-code');
            $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').val($(this).val());
            $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').change();
        });

        $(FormsEngine.DefaultFormTag).find('select').change(function () {
            var code = $(this).attr('code');
            $(":input[parent-code='" + code + "']").removeAttr('checked');
            $(":input[parent-code='" + code + "'][value='" + $(this).val() + "']").prop('checked', true);
        });

        //Program control 
        $(FormsEngine.DefaultFormTag).find('select[code="Program_Of_Interest"]').change(function () {
            fe_getProgramDetail();
            fe_getProgramWizardDetail();
        });

        //Smart Address Edit text
        if (FormsEngine.SmartAddressEditText && FormsEngine.SmartAddressEditText.length > 0) {
            $(FormsEngine.DefaultFormTag).find('#EditSmartAddressText').text(FormsEngine.SmartAddressEditText);
        }

        // International Templates
        if (FormsEngine.UseInternationalTemplate) {

            // On change of country ddl auto check US citizen if the country is US..
            jQuery(FormsEngine.DefaultFormTag).find("select[code='Country']").on("change", function () {
                var countryCodeSelected = jQuery(FormsEngine.DefaultFormTag).find("select[code='Country'] option:selected").val();
                var usCitizenValue = 'Yes';
                if (countryCodeSelected.toLowerCase() == "us"
                    && (!jQuery(FormsEngine.DefaultFormTag).find("[code=us_citizen]:checked").exists()
                        || (jQuery(FormsEngine.DefaultFormTag).find("[code='us_citizen']").val() != "Yes"))) {
                    fe_setControlValue("us_citizen", "Yes");
                }
            });
        }

        // Hide Program_Of_Interest control by default for EMS Program Wizards
        // The control is shown if there is more than one program returned from ME when using a feature list
        if (FormsEngine.ApplicationId == emsApplicationId
            && FormsEngine.FormTemplateType == 3
            && fe_featureListInUse()
            && FormsEngine.FeaturedListSingleProgram
            && FormsEngine.FeaturedListSingleProgram.indexOf(FormsEngine.FeatureId)!==-1) {
            var ProgramsDDLContainer = $(FormsEngine.DefaultFormTag).find('.Program_Of_Interest');
            $(ProgramsDDLContainer).css("display", "none");
        }
        
        georgiaBandaid();
        setSubmitButtonLabels();
        fe_determineIfHeaderDirectionButtonsShouldBeHidden();
    }

    function setSubmitButtonLabels() {
        if ($(FormsEngine.SubmitButtonLabel).exists()) {
            setDefaultSubmitButtonLabelTextLast();

            if (FormsEngine.SubmitButtonLabelTextLast && FormsEngine.StepTotal <= 1) {
                $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextLast);
            } else if (FormsEngine.SubmitButtonLabelTextNormal) {
                $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextNormal);
            }
        }
    }

    function setDefaultSubmitButtonLabelTextLast() {
        if (!fe_hasSubmitButtonLabelTextLastAlreadyBeenSet()) {
            var programWizardType = 3;
            FormsEngine.SubmitButtonLabelTextLast = FormsEngine.FormTemplateType == programWizardType ? "Request Info" : "Submit";
        }
    }

    //#68736 - Kaplan leads are rejected for invalid State/Zipcode
    function checkIfStateExistsAndInvalid() {
        var ddlCountry = jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"] select[code="Country"]');
        var ddlState = jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step="' + FormsEngine.CurrentStep + '"] select[code="State"]');

        if (ddlCountry && ddlCountry.val() == 'US' && ddlState && ddlState.val() == 'N/A') return false;

        return true;
    }

    function checkCurrentStepForZipToCopy() {
        if ($("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code']").exists()) {
            copyZipCodeValues('Postal_Code', $("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code']").val());
        }
        else if ($("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code_Duplicate']").exists()) {
            copyZipCodeValues('Postal_Code_Duplicate', $("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code_Duplicate']").val());
        }
    }

    function checkCampusSoftPreference() {
        $(FormsEngine.DefaultFormTag).find("input[code='CampusSoftPreference']").each(function () {
            if ($(this).is(':checked')) {
                $(FormsEngine.DefaultFormTag).find(".image-checkbox-field-holder." + $(this).val().toLowerCase()).addClass("selected");
            }
            else {
                $(FormsEngine.DefaultFormTag).find(".image-checkbox-field-holder." + $(this).val().toLowerCase()).removeClass("selected");
            }
        });
    }

    function isInt(n) {
        return typeof n === 'number' && n % 1 == 0;
    }

    function delayedValidation(callback) {
        if (FormsEngine.DelayedValidation == undefined) {
            FormsEngine.DelayedValidation = window.setInterval(function () {
                if (FormsEngine.PendingAsyncValidations.length == 0) {
                    if (FormsEngine.DelayedValidation != undefined) {
                        window.clearInterval(FormsEngine.DelayedValidation);
                        FormsEngine.DelayedValidation = undefined;
                    }
                    fe_consolelog("No more pending asyc validations...");
                    callback();
                }
            }, 300);
        }
        else if (FormsEngine.PendingAsyncValidations.length == 0) {
            if (FormsEngine.DelayedValidation != undefined) {
                window.clearInterval(FormsEngine.DelayedValidation);
            }
            callback();
        }
    }

    function validStep(callback) {
        var valid = true;
        var hasScrolled = false;
        var hasPendingValidations = FormsEngine.PendingAsyncValidations && FormsEngine.PendingAsyncValidations.length > 0;
        fe_consolelog("validStep.Validating Step=" + FormsEngine.CurrentStep);
        fe_consolelog("validStep.hasPendingValidations:" + hasPendingValidations);
        fe_showLoader();
        function validBlock() {
            fe_consolelog("validStep.validBlock.Start Validating Step=" + FormsEngine.CurrentStep);
            var isValid = true;
            //find all input tags except hidden, with the exception of hiddens used for categories and subcategories
            var fieldsToValidate = $("#Step" + FormsEngine.CurrentStep).find(":input").not("[type=hidden]:not([code*='Categories']):not([code='Specialties'])").not("[code='Desired_Degree_Level']");
            fe_consolelog("validStep.validBlock.Validating #Controls=" + fieldsToValidate.length);
            for (var i = 0; i < fieldsToValidate.length; i++) {
                var controlName = $(fieldsToValidate[i]).attr('name');
                var controlCode = $(fieldsToValidate[i]).attr('code');
                fe_consolelog("TRYING.VALIDSTEP.validBlock.control." + controlCode);
                if ((controlCode != undefined && $(fieldsToValidate[i]).visible())
                    || controlCode == 'Categories' || controlCode == 'SubCategories' || controlCode == 'Specialties' || controlCode == 'Phone' || controlCode == 'Alternate_Phone'
                ) {
                    var controlResult = false;
                    var pendingValidation = controlCode != undefined && FormsEngine.PendingAsyncValidations && FormsEngine.PendingAsyncValidations.indexOf(controlName) > -1;
                    if (!pendingValidation) {
                        $(fieldsToValidate[i]).validate();
                        controlResult = $(fieldsToValidate[i]).valid();
                        //if the control is invalid we should scroll to it assuming we havent already scrolled
                        if (!controlResult && !hasScrolled) {
                            hasScrolled = true;
                            var errorLabel = $(FormsEngine.DefaultFormTag).find("label.error").first();
                            if ($(errorLabel).length > 0) {
                                if (!errorLabel.visible()) {
                                    $(errorLabel).focus();
                                    $('html, body').animate({
                                        scrollTop: ($(errorLabel).offset().top - 100)
                                    }, 2000);
                                }
                            }
                        }
                    }
                    isValid = isValid && controlResult;
                    fe_consolelog("VALIDATESTEP.validBlock.control." + $(fieldsToValidate[i]).attr('code') + ".Result=" + controlResult);
                }
            }
            fe_hideLoader();
            fe_consolelog("VALIDATESTEP.validBlock.AllValidations.isValid=" + isValid);
            return callback(isValid);
        }

        if (hasPendingValidations) {
            fe_consolelog("VALIDATESTEP.Calling Delayed validation");
            delayedValidation(validBlock);
        }
        else {
            fe_consolelog("VALIDATESTEP.No delayed");
            validBlock();
        }
    }


    function validateLastStepRequiredFields() {
        var valid = true;
        var step = fe_wiz_getLastStep();
        $("#Step" + step).find(":input[required]").not("[code='UserAgreement']").not("[code='EDDYUserAgreement']").each(function () {
            if ($(this).is(':radio') || $(this).is(':checkbox')) {
                var name = $(this).attr("name");
                valid &= ($("#Step" + step).find(":input[name='" + name + "']:checked").length > 0);
            }
            else {

                valid &= ($(this).val() != '');

            }
        });
        return valid;
    }

    function smartMatchZeroPreping(callback) {
        var serviceArgs = getSMCallServiceArgs();
        fe_getSessionId(function () {
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            var sUrl = FormsEngine.ServiceBaseURL + "/TemplateManager/AnySchoolMatches" + serviceArgs;

            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: sUrl,
                success: function (data) {
                    FormsEngine.AnySchoolMatches = data;
                    callback();

                },
                error: function (request, textStatus, errorThrown) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    fe_logClientException(request, sUrl, errorThrown);
                    callback();
                }
            });
        });
    }

    function determineIfAdditionalQuestionsAreShownOnInitialStep() {
        if (FormsEngine.FormsHasBeenRecovered && FormsEngine.ProgramLoadedFromRecovery == null) {
            FormsEngine.ProgramLoadedFromRecovery = true;
            onLoadDynamicQuestions();
        }

        if (FormsEngine.HasAdditionalQuestions) {
            if (FormsEngine.ShowAllQuestionsOnFirstStep) {
                showAdditionalQuestionsOnInitialStep();
            } else {
                $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextNormal);
            }
        } else {
            setSubmitButtonLabels();
        }
    }

    function showAdditionalQuestionsOnInitialStep() {
        var programControl = fe_getControlValue("Program_Of_Interest");
        var programId = programControl.value;
        var AdditionalQuestionStep = $(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']');

        if (AdditionalQuestionStep != null && programId) {
            AdditionalQuestionStep.show();
            $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextLast);
            if (FormsEngine.ShowContinueMobileButton) {
                $("#screen-button span").html(FormsEngine.SubmitButtonLabelTextLast.toUpperCase() + " <i class='fa fa-arrow-circle-right form-submit-button-icon'></i>");
            }
            showAdditionalQuestionsInEducationInfoSectionIfItExists();
        }
    }

    function showAdditionalQuestionsInEducationInfoSectionIfItExists() {
        var additionalQuestionStep = $("#Step" + FormsEngine.StepDynamicQuestions);

        var getEducationInfoParentId = function (sectionTitle) {
            return $(FormsEngine.DefaultFormTag).find(".sections legend:contains('" + sectionTitle + "')").parent().attr("id");
        }
        
        var educationInfoElementId = getEducationInfoParentId("Education Info") || getEducationInfoParentId("Educational Info");
        
        if (educationInfoElementId != undefined) {    
            $("#" + educationInfoElementId).append(additionalQuestionStep);
            $("#additionalQuestionLabel").remove();        
        }

        additionalQuestionStep.css("min-height", "0px");
    }

    
    function onLoadDynamicQuestions() {
        fe_consolelog("loading additional questions single step...");

        var _loadDynamicQuestions = function () {
            loadDynamicQuestions(function (data) {
                ProcessLoadDynamicQuestionsResponse(data);
                determineIfAdditionalQuestionsAreShownOnInitialStep();
            });
        }

        if (FormsEngine.SmartMatchZero === true) {
            smartMatchZeroPreping(function () {
                _loadDynamicQuestions();
            });
        } else {
            _loadDynamicQuestions();
        }
    }

    function loadDynamicQuestions(callback) {
        georgiaBandaid();

        if (FormsEngine.RenderingStrategy == "SCHOOLPICKERWIZARD") {
            $(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']').find("#DynamicQuestions").html('');
        } else {
            $(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']').find("#Section1").html('');
        }

        if (FormsEngine.CampaignDetail != undefined && FormsEngine.CampaignDetail != null && FormsEngine.IsMobileForm == true) {
            FormsEngine.CampaignDetail.AdditionalQuestionsOnlyInSchoolSelection = false;
            FormsEngine.CampaignDetail.AdditionalQuestionsFromSmartMatch = false;
        }
        if (FormsEngine.FormTemplateType == 3) {
            FormsEngine.CampaignDetail.MaxSmartMatchCount = 1;
            FormsEngine.CampaignDetail.IsWizardAnyMatch = FormsEngine.CampaignDetail.ProgramWizardAdditionalQuestionsFlowType == 1;
            FormsEngine.CampaignDetail.AdditionalQuestionsOnlyInSchoolSelection = false;
            FormsEngine.CampaignDetail.AdditionalQuestionsFromSmartMatch = true;
        }

        var templateIsAWizard = FormsEngine.FormTemplateType == 2;
        var templateIsAProgramWizard = FormsEngine.FormTemplateType == 3;
        var programIsSelected = Boolean(FormsEngine.ProgramId);
        var userSmartMatched = FormsEngine.UserSmartMatched === true;

        //Additional questions disabled by campaign support or already smart matched
        if ((userSmartMatched && templateIsAWizard) ||
            (templateIsAProgramWizard && !programIsSelected) ||
            FormsEngine.CampaignDetail != undefined && FormsEngine.CampaignDetail != null
            && (
                FormsEngine.CampaignDetail.AdditionalQuestionsOnlyInSchoolSelection === true
                || (FormsEngine.CampaignDetail.AdditionalQuestionsFromSmartMatch && FormsEngine.CampaignDetail.MaxSmartMatchCount == 0)
            )) {
            var data = [];
            data.HasAdditionalControls = false;
            fe_consolelog("CampaignDetail.AdditionalQuestionsOnlyInSchoolSelection=true");
            callback(data);
        }
        else {
            if (FormsEngine.RenderingStrategy == "SCHOOLPICKERWIZARD") {
                var programTemplateIds = fe_sp_getUniqueProgramTemplateIdsFromSchoolPickerSelections();

                if (programTemplateIds && programTemplateIds.length > 0) {
                    fe_wiz_requestAdditionalQuestionCollection(programTemplateIds, function (data) {
                        callback(data);
                    });
                } else {
                    var data = { HasAdditionalControls: false };
                    callback(data);
                }
            } else {
                var ProgramProductId = 0;
                var ProductId = 0;

                if (FormsEngine.FormTemplateType == 3) {
                    var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
                    var ProgramTemplateId = $(ProgramDDL).attr('data-templateid');
                    ProgramProductId = $(ProgramDDL).attr('data-programproductid');
                    ProductId = $(ProgramDDL).attr('data-productid');
                    checkGSTemplate();
                } 
                getAdditionalTemplateQuestions(ProductId, ProgramProductId, callback);
            }
        }
    }

    function getAdditionalTemplateQuestions(productId, programProductId, callback) {

        productId = productId || 0;
        programProductId = programProductId || 0;
        
        var serviceArgs = getSMCallServiceArgs();
        serviceArgs += "&UseSmartMatch=" + FormsEngine.CampaignDetail.AdditionalQuestionsFromSmartMatch;
        serviceArgs += "&IsWizardAnyMatch=" + FormsEngine.CampaignDetail.IsWizardAnyMatch;
        serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
        serviceArgs += "&InstitutionId=" + FormsEngine.InstitutionId;
        serviceArgs += "&ProgramProductId=" + programProductId;
        serviceArgs += "&ProductId=" + productId;

        fe_getSessionId(function () {
            serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
            var sUrl = FormsEngine.ServiceBaseURL + "/TemplateManager/GetAdditionalTemplateQuestions" + serviceArgs;

            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: sUrl,
                success: function (data) {
                    callback(data);
                },
                error: function (request, textStatus, errorThrown) {
                    var data = [];
                    data.HasAdditionalControls = false;
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    fe_logClientException(request, sUrl, errorThrown);
                    callback(data);
                }
            });

        });
    }

    function getFormData() {

        // prepare to save the Express Consent text with the lead data
        var expressConsentText = $(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]').siblings('label').text();
        var userAgreement = $(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]');
        $(userAgreement).attr('value', expressConsentText);

        //Set Form Settings Tracking sessionid and AffiliateId from cookies
        fe_setSettingsFromCookies();

        //Set User name
        FormsEngine.UserFullName = $(FormsEngine.DefaultFormTag).find(':input[name="First_Name"]').val() + " " + $(FormsEngine.DefaultFormTag).find(':input[name="Last_Name"]').val();

        //Set User name
        FormsEngine.ProspectFullName = FormsEngine.UserFullName;

        var FormData = fe_getFormData();
        //AffiliateId
        FormData.LeadData += "&AffiliateId=" + FormsEngine.AffiliateId;

        //Widget guid
        if (FormsEngine.WidgetRequestGuid) {
            FormData.LeadData += "&WidgetRequestGuid=" + FormsEngine.WidgetRequestGuid;
        }
        if (FormsEngine.WidgetName) {
            FormData.LeadData += "&WidgetName=" + FormsEngine.WidgetName;
        } 

        //Additional Fields support
        FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];

        //Cleansing additional clashing with form questions
        if (FormsEngine.Questions && FormsEngine.AdditionalFields) {
            jQuery.each(FormsEngine.Questions, function (index, item) {
                FormsEngine.AdditionalFields = jQuery.grep(FormsEngine.AdditionalFields, function (value) {
                    return value[0].toUpperCase() !== item.Code.toUpperCase();
                });
            });
        }
        
        if (FormsEngine.AdditionalFields) {
            jQuery.each(FormsEngine.AdditionalFields, function (index, item) {
                FormData.LeadData = FormData.LeadData + "&" + FormsEngine.AdditionalFields[index][0] + "=" + FormsEngine.AdditionalFields[index][1];
            });
        }

        //Campus Soft Preference
        var campusSoftPreferenceShown = false;
        if ($(FormsEngine.DefaultFormTag).find(':input[name="CampusSoftPreference"]').exists()) {
            campusSoftPreferenceShown = ($(FormsEngine.DefaultFormTag).find(':input[name="CampusSoftPreference"]').is(":hidden") == false);
        }
        else {
            campusSoftPreferenceShown = FormsEngine.DynamicCampusSoftPreferenceShown === true;
        }
        FormData.LeadData = FormData.LeadData + "&CampusSoftPreferenceShown=" + campusSoftPreferenceShown;

        //Campus Preference
        var campusPreferenceShown = false;
        if ($(FormsEngine.DefaultFormTag).find(':input[name="CampusPreference"]').exists()) {
            campusPreferenceShown = ($(FormsEngine.DefaultFormTag).find(':input[name="CampusPreference"]').is(":hidden") == false);
        } else {
            campusPreferenceShown = FormsEngine.CampusPreferenceVisible === true;
        }
        FormData.LeadData = FormData.LeadData + "&CampusPreferenceShown=" + campusPreferenceShown;

        // UTM Args
        if (FormsEngine.ApplicationId == emsApplicationId) {
            var utmParameters = fe_getUTMParameters();

            if (utmParameters.utmChannel) {
                FormData.LeadData = FormData.LeadData + "&UTMChannel=" + utmParameters.utmChannel;
            }

            if (utmParameters.utmVendor) {
                FormData.LeadData = FormData.LeadData + "&UTMVendor=" + utmParameters.utmVendor;
            }

            if (utmParameters.utmCampaign) {
                FormData.LeadData = FormData.LeadData + "&UTMCampaign=" + utmParameters.utmCampaign;
            }
        }

        //Boost Specialties to ME if available
        var boostSpecialties = fe_getParameterByName("BSpecialties");
        if (boostSpecialties != undefined && boostSpecialties && boostSpecialties.length > 0) {
            FormData.LeadData = FormData.LeadData + "&BSpecialties=" + boostSpecialties;
        }


        //Theme
        FormData.LeadAdditionalData += "&" + "Theme=" + FormsEngine.Theme;

        //removed 2U schools
        if (FormsEngine.RemovedTwoUSchools) {
            FormData.LeadData = FormData.LeadData + "&RemovedTwoUSchools=" + FormsEngine.RemovedTwoUSchools;
        }

        //InternationalCallcenter
        if (FormsEngine.InternationalCallcenterForm) {
            FormData.LeadData = FormData.LeadData + "&InternationalCallcenterForm=true";
        }

        if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.IsNewTCPAExperience) {
            if (FormsEngine.UserShownManagedChoice == null) {
                //Set TCPA schools
                if (FormsEngine.TCPASelectedSchoolList && FormsEngine.TCPASelectedSchoolList.length > 0) {

                    var selectedTCPA = "";

                    jQuery.each(FormsEngine.TCPASelectedSchoolList, function (index, item) {
                        selectedTCPA += FormsEngine.TCPASelectedSchoolList[index] + ",";
                    });

                    selectedTCPA = selectedTCPA.replace(/,$/, '');

                    FormData.LeadData = FormData.LeadData + "&TCPASelectedSchoolList=" + selectedTCPA;
                }

                FormData.LeadData = FormData.LeadData + "&SmartMatchUserAgreement=" + FormsEngine.ResourceData["SM.USERAGREEMENT"];
                FormData.LeadData = FormData.LeadData + "&IsNewSmartMatchTCPA=Yes";
            } 
        }

        // add a global variable to use here and later on Managed Choice Submissions
        FormsEngine.LeadDataEncoded = encodeURIComponent(FormData.LeadData);
        FormsEngine.LeadAdditionalDataEncoded = encodeURIComponent(FormData.LeadAdditionalData);

        var UserSubmission = {};
        UserSubmission.LeadDataEncoded = FormsEngine.LeadDataEncoded;
        UserSubmission.LeadAdditionalDataEncoded = FormsEngine.LeadAdditionalDataEncoded;

        return UserSubmission;
    }


    function onAnimateStepOut(direction, hideStep) {
        if (hideStep == true) {
            $("#Step" + FormsEngine.CurrentStep).hide();
        }
        FormsEngine.CurrentStep += direction;
        //Special label on last step
        if (fe_isLastStep()) {
            //Helen added to show "Submit" button instead of "Continue" at last step
            if ($('.steps:visible').next(".steps").length == 0) {
                $(FormsEngine.SubmitButton).addClass("buttonLastStep");
                $(FormsEngine.SubmitButton).attr("data-tf-element-role", "submit");
            }

            if (!FormsEngine.SmartMatchZero) {
                $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextLast);
                if (FormsEngine.ShowContinueMobileButton) {
                    $("#screen-button span").html(FormsEngine.SubmitButtonLabelTextLast.toUpperCase() + " <i class='fa fa-arrow-circle-right form-submit-button-icon'></i>");
                }
            }
        }
        else {
            $(FormsEngine.SubmitButton).removeClass("buttonLastStep");
            $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextNormal);
            if (FormsEngine.ShowContinueMobileButton) {
                $("#screen-button span").html(FormsEngine.SubmitButtonLabelTextNormal.toUpperCase() + " <i class='fa fa-arrow-circle-right form-submit-button-icon'></i>");
            }
        }

        //Google analytics and internal tracking
        trackEvent();



        //navBar
        refreshNavbar();

        $("#ErrorBoxSection" + FormsEngine.CurrentStep).show();

        if (hideStep == true) {
            $("#Step" + FormsEngine.CurrentStep).show();
        }
    }

    function onAnimateStepIn(direction) {
        if (FormsEngine.CurrentStep > 1) {
            $(FormsEngine.BackButton).show();

            //show "Start Now" in header only for Step1
            $('.eddy-form-wizard-header .start-now').css('display', 'none');

        }
        else {
            $(FormsEngine.BackButton).hide();
            $('.eddy-form-wizard-header .start-now').css('display', 'block');

        }
        fe_hideLoader(false);
        fe_addLastStepClassToBackButtonIfOnLastStep();

        MovingStep = false;
        fe_wiz_focusOnNextFocusable('null');
        fe_ScrollToTop();

        //set location hash for current step
        window.location.hash = "#_Step" + FormsEngine.CurrentStep;

        if (FormsEngine.JornayaDelayUntilSecondStep === true && FormsEngine.LeadIdLoaded !== true && FormsEngine.CurrentStep === 2) {
            FormsEngine.LeadIdLoaded = true;
            fe_initialize_Leadid();
        }

        //Step Loaded event
        if (FormsEngine.OnStepLoaded) {
            FormsEngine.OnStepLoaded(FormsEngine.CurrentStep);
        }
        //Step Loaded event internal
        if (FormsEngine.OnStepLoadedInternal) {
            FormsEngine.OnStepLoadedInternal(FormsEngine.CurrentStep, direction);
        }
        if (FormsEngine.OnStepLoadedExternal) {
            FormsEngine.OnStepLoadedExternal(FormsEngine.CurrentStep, FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions);
        }

        $(FormsEngine).trigger("OnStepLoaded", FormsEngine.CurrentStep, FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions);

        if (currentStepIsEmpty()) {
            moveStep(direction);
        }
        else {
            if ($("#screen-button").length > 0) {
                checkValidMobileButton();
            }
        }

        if ($("#screen-button").length > 0) {
            fe_checkShowScreenButton();
        }

        if (fe_wiz_getLastStep() == FormsEngine.CurrentStep) {
            checkForExpressConsent();
        }
    }


    //Animates and advances (back/forward) the step
    function moveStep(direction) {
        fe_wiz_setErrorMessage("");
        if (FormsEngine.CustomTransition) {
            onAnimateStepOut(direction, true);
            FormsEngine.CustomTransition(function () {
                onAnimateStepIn(direction);
            });
        }
        else {
            $("#Step" + FormsEngine.CurrentStep).fadeOut(100, function () {
                onAnimateStepOut(direction, false);
                $("#Step" + FormsEngine.CurrentStep).fadeIn(50, function () {
                    //set the status of controls with values
                    fe_setControlStatus();
                    //bind status update events to all required controls
                    fe_bindControlStatusEvent();
                });
                onAnimateStepIn(direction);
            });
        }



    }

    function currentStepIsEmpty() {
        var isEmpty = $("#Step" + FormsEngine.CurrentStep).find(':input:visible').length == 0;
        //check that we are not on category, subcategory, or specialties step because those may still be loading
        if ($("#Step" + FormsEngine.CurrentStep).find("input[name='Categories_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($("#Step" + FormsEngine.CurrentStep).find("input[name='SubCategories_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($("#Step" + FormsEngine.CurrentStep).find("input[name='Specialties_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($("#Step" + FormsEngine.CurrentStep).find("#school-picker-carousel").length > 0) {
            isEmpty = false;
        }

        //dont skip the dynamic step
        if (FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions) {
            isEmpty = false;
        }

        return isEmpty;
    }

    function checkForExpressConsent() {
        var Phone1 = $(FormsEngine.DefaultFormTag).find(":input[code='Phone']").val();
        var Phone2 = $(FormsEngine.DefaultFormTag).find(":input[code='Alternate_Phone']").val();

        FormsEngine.MobilePhones = [];
        if (Phone1 && Phone1.length > 0) {
            FormsEngine.MobilePhones.push(Phone1.replace(/\D/g, ''));
        }
        if (Phone2 && Phone2.length > 0 && Phone1 != Phone2) {
            FormsEngine.MobilePhones.push(Phone2.replace(/\D/g, ''));
        }

        FormsEngine.SmartMatchSchoolNames = '';
        //Program Wizard
        if (FormsEngine.FormTemplateType == 3) {
            defaultCurrentInstitutionSmartMatch();
        }
        else {

            if (FormsEngine.RenderingStrategy === "SCHOOLPICKERWIZARD") {
                fe_sp_setSmartMatchSchoolNamesFromSchoolPickerSelections();
                checkUserAgreementText();
            } else if (((FormsEngine.CampaignDetail != null && FormsEngine.CampaignDetail.MaxSmartMatchCount > 0)) //We will be calling ME regardless express consent on the last question of the last step if campaign allows SM.
                && FormsEngine.UserSmartMatched === false
                && FormsEngine.DynamicQuestionCheckOccurred
                && validateLastStepRequiredFields()) {
                getSmartMatchesFromME();
            }
            else {
                checkUserAgreementText();

                if (validateLastStepRequiredFields()
                    && (FormsEngine.CampaignDetail != null && FormsEngine.CampaignDetail.MaxSmartMatchCount == 0)) {
                    //automatically submit the form once the last question has been answered if the campaign is set to sm 0
                    $(FormsEngine.SubmitButton).click();
                } 
            }
        }
    }

    //Validate if last required question of last step to call ME.
    function checkLastStepRequired() {
        if (validateLastStepRequiredFields()) {
            if (FormsEngine.SMLeadsCreatedCount != undefined && FormsEngine.SMLeadsCreatedCount != null && FormsEngine.SMLeadsCreatedCount > 0) {
                fe_consolelog("SM Leads already created for this user, NOT getting SMs");
                return;
            }
            checkForExpressConsent();
        }
    }

    //Google Analytics and internal tracking event
    function trackEvent() {
        if (FormsEngine.TrackEvent) {
            var Step = "Step" + FormsEngine.CurrentStep;
            if (FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions) {
                Step = "Step-AdditionalQuestions";
            }
            FormsEngine.TrackEvent(Step);
        }
    }

    //navbar percent
    function refreshNavbar() {
        if (FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions) {
            WizardPercent = 98;
        }
        else {
            //individual control progress support on one step forms
            if (FormsEngine.StepTotal === 1 && FormsEngine.CurrentStep === 1) {
                WizardPercent = Math.floor((FormsEngine.RequiredControlwithValues * 95) / FormsEngine.RequiredControlTotal);

                fe_consolelog('wizardPercent by completed control wizard');
            } else {
                WizardPercent = Math.floor((FormsEngine.CurrentStep * 95) / FormsEngine.StepLast);
            }
        }
        fe_setProgressBar(WizardPercent);
    }


    //Navigation logic
    // step1 -->... --> last step --> additional step
    // at least 2 steps required frm admin (+addtnl questions step)
    function checkNavigation(direction) {
        //prevent multiple firing while switching
        if (MovingStep) {
            fe_consolelog("Ignoring click, transitioning already");
            return;
        }

        //lock event
        MovingStep = true;
        fe_setDynamicTags(direction);
        var isLastStep = fe_isLastStep();
        if (direction > 0 && isLastStep) { //last (final) step
            fe_OpenMail(emsApplicationId,function () {
                validStep(function (result) {
                    if (result) {
                        //Log Jornaya events if not available on ET
                        fe_logJornaya();
                        if (FormsEngine.RenderingExperience != "Prospect") {
                            fe_submitProspectAdditionalInfo(false);
                        }
                        fe_consolelog('Submit form');
                        fe_showLoader();

                        //External Wizard submit
                        if ((!FormsEngine.IsMobile || FormsEngine.AllowLeaveBehindeOnMobile === true) && FormsEngine.StartLeaveBehindURL != undefined && FormsEngine.StartLeaveBehindURL != null && FormsEngine.StartLeaveBehindURL.length > 0) {
                            FormsEngine.PopupStartLeaveBehind = window.open('', '_blank');
                        }
                        FormsEngine.LoadWorkflowStep('MANAGEDCHOICE', getFormData());
                        return;
                    }
                    else {
                        MovingStep = false;
                    }

                    fe_checkShowScreenButton();
                })
            });
        }
        //If on second to last step then call get additional questions
        // previous of last step = DynamicQuestions step -- 2
        else if (direction > 0 && fe_isGoingToStepBeforeAdditionalQuestions()) {
            fe_OpenMail(emsApplicationId,function () {
                validStep(function (result) {
                    if (result) {
                        fe_showLoader();
                        if (FormsEngine.CheckDependencies) {
                            //Fire bind on last control of this step
                            var control = $("[id='Step" + FormsEngine.CurrentStep + "']").find(":input:last");
                            FormsEngine.CheckDependencies(control, true);
                        }
                        fe_wiz_updateMatchCount();
                        if (FormsEngine.StepLast !== 1) {
                            fe_consolelog("loading additional questions regular");
                            if (FormsEngine.SmartMatchZero === true) {
                                smartMatchZeroPreping(function () {
                                    loadDynamicQuestions(function (data) {
                                        ProcessLoadDynamicQuestionsResponse(data, direction);
                                    });
                                });
                            } else {

                                loadDynamicQuestions(function (data) {
                                    ProcessLoadDynamicQuestionsResponse(data, direction);
                                });
                            }
                        }
                        else {
                            if (direction) {
                                moveStep(direction);
                            }
                            MovingStep = false;
                            checkLastStepRequired();
                        }
                    }
                    else {
                        MovingStep = false;
                    }
                })
            });
        }
        else if ($("#Step" + (FormsEngine.CurrentStep + direction)).length > 0) {
            //backward or forward valid
            if (direction < 0) {
                fe_wiz_updateMatchCount();
                moveStep(direction);
            }
            else {
                fe_OpenMail(emsApplicationId,function () {
                    validStep(function (result) {
                        if (result) {
                            if (FormsEngine.CheckDependencies) {
                                //Fire bind on last control of this step
                                var control = $("[id='Step" + FormsEngine.CurrentStep + "']").find(":input:last");
                                FormsEngine.CheckDependencies(control, true);
                            }
                            fe_wiz_updateMatchCount();
                            moveStep(direction);
                            var LastStep = fe_wiz_getLastStep();
                            if (FormsEngine.CurrentStep + direction == LastStep ||
                                FormsEngine.CurrentStep == LastStep) {
                                checkForExpressConsent();
                            }
                        }
                        else {
                            MovingStep = false;
                        }
                    })
                });
            }
        }
        else {
            MovingStep = false;
        }

        
    }

    function ProcessLoadDynamicQuestionsResponse(data, direction) {
        if (data.HasAdditionalControls) {
            FormsEngine.HasAdditionalQuestions = true;
            var AdditionalQuestionStep = $(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']');
            var additionalQuestionValues = getAdditionalQuestionAnswers();
            var additionalControls = decodeURIComponent((data.RenderedControls + '').replace(/\+/g, '%20'));

            if (FormsEngine.RenderingStrategy == "SCHOOLPICKERWIZARD") {
                $(AdditionalQuestionStep).find("#DynamicQuestions").html(additionalControls);
            } else {
                $(AdditionalQuestionStep).find("#Section1").html(additionalControls);
            }
            
            fe_add_AdditionalField('AdditionalQuestionsShown.Start', 'true');
            fe_trackLocalEvent('form-wizard', 'AdditionalQuestionsShown.Start', 'true');
            
            // update controls sort indexes
            fe_wiz_updateControlsSortIndexes();

            //Ticket 82224: State showing a list of US/Canada states even if not US/Canada country is selected
            //TODO: If additional questions have dependencies they are not reacting to previous questions.
            // Dynamic controls need to account for additional questions.
            // Temporary fix to support this case.
            var statesDDL = $("#Step" + FormsEngine.StepDynamicQuestions).find(':input[code="State"]');
            if (statesDDL.length > 0) {
                var contryDDL = $('select[code="Country"]');
                if (contryDDL.length > 0 && contryDDL.val() != 'US' && contryDDL.val() != 'CA') {
                    $(statesDDL).find('option:not(:first)').remove();
                    $(statesDDL).append(new Option('Not Applicable', 'N/A', true, true));
                }
            }

            recoverAdditionalQuestions(additionalQuestionValues);

            // recover the additional questions/answers using the new dynamic databind method
            fe_getSessionObject("WFORM_DynamicQuestions", function (data) {
                fe_consolelog("LOADING FORM FROM SESSION for additional questions");
                if (!fe_isNullOrEmpty(data)) {
                    FormsEngine.FormsHasBeenRecovered = true;
                    FormsEngine.StringToRecoverAdditionalQuestions = data;

                    var AdditionalQuestionStep = $(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']');
                    $(AdditionalQuestionStep).find('div[data-controlcode]').each(function () {
                        var ControlCode = $(this).attr('data-controlcode');
                        var input = $(this).find(':input').first();
                        var ControlValue = fe_getParameterByNameAndAliasFromString(ControlCode, FormsEngine.StringToRecoverAdditionalQuestions);
                        if (ControlValue != "" && ControlCode != "UserAgreement" && ControlCode != "EDDYUserAgreement") {
                            fe_setControlValue(ControlCode, ControlValue);
                            if ($(input).exists()) {
                                if ($(input).is(':radio') || $(input).is(':checkbox')) {
                                    $(AdditionalQuestionStep).find(':input[code="' + ControlCode + '"]').trigger('change');
                                } else if ($(input).is('select')) {
                                    $(AdditionalQuestionStep).find(':input[code="' + ControlCode + '"]').trigger('click');
                                }
                            }
                        }
                    });
                    fe_consolelog("Additional Questions Recovery Complete");
                }
            });

            checkLastStepRequired();

            //Bind Events to Dynamically Added Controls
            $("#Step" + FormsEngine.StepDynamicQuestions).find(':input').each(function () {
                if ($(this).is('select')) {
                    $(this).change(function (event) {
                        fe_saveFormDynamicQuestions();
                    });
                } else if ($(this).is(':checkbox')) {
                    $(this).click(function (event) {
                        fe_saveFormDynamicQuestions();
                    });
                } else if ($(this).is(':radio')) {
                    $(this).click(function (event) {
                        fe_saveFormDynamicQuestions();
                    });
                }

                $(this).change(function () {
                    var code = $(this).attr('code');
                    if (code == undefined || code == null || code == 'UserAgreement' || code == 'EDDYUserAgreement') {
                        return;
                    }
                    checkLastStepRequired();
                    checkValidMobileButton();
                });
            });

            $(FormsEngine).trigger("OnAdditionalQuestionsAdded");
        }
        else {
            fe_consolelog("No additional questions");
            FormsEngine.HasAdditionalQuestions = false;
            fe_add_AdditionalField('AdditionalQuestionsShown.Start', 'false');
            fe_trackLocalEvent('form-wizard', 'AdditionalQuestionsShown.Start', 'false');
            checkForExpressConsent();
        }
        checkUserAgreementText();
        if (direction) {
            moveStep(direction);
        }
        MovingStep = false;
        FormsEngine.DynamicQuestionCheckOccurred = true;
        checkLastStepRequired();
    }

    function recoverAdditionalQuestions(additionalQuestionValues) {
        if (additionalQuestionValues != null && additionalQuestionValues.length > 0) {
            $("#Step" + FormsEngine.StepDynamicQuestions).find(':input').each(function () {
                var code = $(this).attr('code');

                for (i = 0; i < additionalQuestionValues.length; i++) {
                    var s = additionalQuestionValues[i].split("|");

                    if (code == s[0]) {
                        if ($(this).is(':radio')) {
                            $(FormsEngine.DefaultFormTag).find("[type=radio][code='" + code + "'][value='" + s[1] + "']").prop('checked', true);
                        }
                        else if ($(this).is(':checkbox')) {
                            $(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + code + "'][value='" + s[1] + "']").prop('checked', true);
                        }
                        else {
                            $(this).val(s[1]);
                        }
                        break;
                    }
                }
            });
        }
    }

    function getAdditionalQuestionAnswers() {
        var additionalQuestions = new Array();

        $("#Step" + FormsEngine.StepDynamicQuestions).find(':input').not("[code='UserAgreement']").not("[code='EDDYUserAgreement']").each(function () {
            var code = $(this).attr('code');
            var value = '';

            if ($(this).is(':radio')) {
                value = $(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']:checked").val();
            }
            else {
                value = $(this).val();
            }

            additionalQuestions.push(code + '|' + value);
        });

        return additionalQuestions;
    }

    function copyZipCodeValues(controlCode, controlValue) {
        if (controlCode == 'Postal_Code') {
            $(FormsEngine.DefaultFormTag).find('input[code="Postal_Code_Duplicate"]').val(controlValue);
        }
        else if (controlCode == 'Postal_Code_Duplicate') {
            $(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').val(controlValue);
        }
    }

    //HTML5 placeholder (watermark) support if not use plugin
    function hasPlaceholderSupport() {
        var input = document.createElement('input');
        return ('placeholder' in input);
    }

    function setUserAgreementText(userAgreement, eddyUserAgreement) {
        if (FormsEngine.DoNotDisplayTCPA === true) {
            fe_consolelog('Not displaying TCPA');
            return;
        }

        var userAgreementControl = $(FormsEngine.DefaultFormTag).find('[data-fieldholdercode="UserAgreement"]');
        var eddyUserAgreementControl = $(FormsEngine.DefaultFormTag).find('[data-fieldholdercode="EDDYUserAgreement"]');

        if (FormsEngine.ShowTCPAOnLastRequiredQuestionOnly) {
            $(userAgreementControl).find('label').not('.error').html(userAgreement);
            $(eddyUserAgreementControl).find('label').not('.error').html(eddyUserAgreement);
            if (fe_wiz_getLastStep() == FormsEngine.CurrentStep && validateLastStepRequiredFields()) {
                $(userAgreementControl).find('label').attr("data-tf-element-role", "consent-language")
                $(userAgreementControl).show();
            }
            else {
                $(userAgreementControl).hide();
                $(eddyUserAgreementControl).hide();
            }
        }
        else {
            (userAgreementControl).find('label').attr("data-tf-element-role", "consent-language")
            $(userAgreementControl).find('label').not('.error').html(userAgreement);
            $(eddyUserAgreementControl).find('label').not('.error').html(eddyUserAgreement);
        }
        checkGSTemplate();
    }


    function checkUserAgreementText() {

        FormsEngine.SMLeadsCreatedCount = (FormsEngine.SMLeadsCreatedCount != undefined && FormsEngine.SMLeadsCreatedCount != null) ? FormsEngine.SMLeadsCreatedCount : 0;

        // Move User agreement to other step if we have additional questions.
        var userAgreementControl = $(FormsEngine.DefaultFormTag).find('[data-fieldholdercode="UserAgreement"]');
        var eddyUserAgreementControl = $(FormsEngine.DefaultFormTag).find('[data-fieldholdercode="EDDYUserAgreement"]');

        //SmartMatch Zero "enhancement"
        if (FormsEngine.SmartMatchZero === true && FormsEngine.AnySchoolMatches === true) {
            $(userAgreementControl).hide();
            $(eddyUserAgreementControl).hide();
        }

        //Custom TCPA SM 0
        if (FormsEngine.CampaignDetail != null
            && FormsEngine.CampaignDetail.MaxSmartMatchCount == 0) {
            $(userAgreementControl).hide();
            $(eddyUserAgreementControl).hide();
        }


        if (FormsEngine.HasAdditionalQuestions===true) {
            var dynamicStep = $(FormsEngine.DefaultFormTag).find('#DynamicQuestionsUserAgreementSection');

            //only add if not already there
            if ($(dynamicStep).find('input[code="UserAgreement"]').length === 0 && userAgreementControl.length > 0) {
                
                if (FormsEngine.ShowAllQuestionsOnFirstStep) {
                    $(FormsEngine.DefaultFormTag).append(userAgreementControl);
                    $(FormsEngine.DefaultFormTag).append(eddyUserAgreementControl);
                } else {
                    $(userAgreementControl).appendTo(dynamicStep);
                    $(eddyUserAgreementControl).appendTo(dynamicStep);
                    $(eddyUserAgreementControl).hide();
                }
                
            }
        }
        else {
            var lastStep = $("#Step" + fe_wiz_getLastStep());

            //only add if not already there
            if ($(lastStep).find('input[code="UserAgreement"]').length === 0 && userAgreementControl.length > 0) {
                $(userAgreementControl).appendTo(lastStep);

                if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.IsNewTCPAExperience) {
                    $(userAgreementControl).css({ "display": "none" });
                    $(userAgreementControl).prop('checked', true);
                    $(userAgreementControl).prop('type', 'hidden');

                    if (FormsEngine.IsTitaniumOrNoSmartMatch) {

                        if ($(lastStep).find('input[code="EDDYUserAgreement"]').length === 0 && eddyUserAgreementControl.length > 0) {
                            $(eddyUserAgreementControl).appendTo(lastStep);
                        }
                    }
                }
            }
        }

        fe_moveControlAlongsideTCPA("Phone");

        //Already created Smart matches print default message
        if (FormsEngine.SMLeadsCreatedCount > 0) {
            FormsEngine.SmartMatchSchoolNames = '';
        }

        fe_consolelog('Replacing TCPA Text.');

        var expressConsent;
        var emsInstitutionTcpaMessage = FormsEngine.ResourceData["EMS_INSTITUTION_TCPA_MESSAGE"] || FormsEngine.ResourceData["JS.WIZARD.USERAGREEMENT_EMS"];

        if (FormsEngine.ApplicationId == emsApplicationId && emsInstitutionTcpaMessage != "" && emsInstitutionTcpaMessage != null && FormsEngine.WizardAlternativeExpressConsent == null) {
            expressConsent = emsInstitutionTcpaMessage;
        } else if (FormsEngine.CampaignDetail && FormsEngine.CampaignDetail.CampaignTCPAMessageName && FormsEngine.CampaignDetail.CampaignTCPAMessageName != '') {
            expressConsent = FormsEngine.ResourceData[FormsEngine.CampaignDetail.CampaignTCPAMessageName]
        } else {
            if (FormsEngine.CustomTCPA != null && FormsEngine.CustomTCPA != "") {
                expressConsent = FormsEngine.CustomTCPA;
            } else {
                expressConsent = FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'];
            }
        }

        var eddyExpressConsent = FormsEngine.ResourceData['EDDY.USERAGREEMENT'];

        var phoneFormatted = '';

        if (FormsEngine.MobilePhones != undefined) {
            var PhoneList = [];
            for (var i = 0; i < FormsEngine.MobilePhones.length; i++) {
                PhoneList.push(fe_formatPhone(FormsEngine.MobilePhones[i]));
            }
            phoneFormatted = PhoneList.join(' and ');
            phoneFormatted = (phoneFormatted.length > 0 ? ' ' + phoneFormatted : phoneFormatted);

        }

        //Tag replacements
        //Mobile
        expressConsent = fe_replaceTag(expressConsent, '{mobile-number}', phoneFormatted);

        //Smart Match Count
        expressConsent = fe_replaceTag(expressConsent, '{SMCount}', (FormsEngine.CampaignDetail ? FormsEngine.CampaignDetail.MaxSmartMatchCount : ''));

        //Eddy Agreement Mobile
        eddyExpressConsent = fe_replaceTag(eddyExpressConsent, '{mobile-number}', phoneFormatted);

        //SM Schools
        if (FormsEngine.SmartMatchSchoolNames != undefined && FormsEngine.SmartMatchSchoolNames.length > 0) {
            console.log(expressConsent);

            // Trim the last comma..
            expressConsent = fe_replaceTag(expressConsent, '{school}', FormsEngine.SmartMatchSchoolNames);     

            console.log(expressConsent);
        }
        else if (FormsEngine.StaticLimboTCPASchoolName) {
            expressConsent = fe_replaceTag(expressConsent, '{school}', FormsEngine.StaticLimboTCPASchoolName + ' and ');
        }
        else {
            if ((fe_wiz_getLastStep() == FormsEngine.CurrentStep && validateLastStepRequiredFields())) {
                /*expressConsent = fe_replaceTag(expressConsent, '{school}', 'EducationDynamics, LLC, including its brand Education Network');*/
                expressConsent = 'By checking the box, I am providing my e-signature and consent for EducationDynamics, LLC d/b/a Education Network to contact me via email, text, or phone number {mobile-number}{/mobile-number}, to match me with educational opportunities, including using automated technology regardless of whether the number is on any federal or state do not call list. I acknowledge that I am the subscriber and regular user of this number and am authorized to provide this consent. I acknowledge that my consent is not a requirement to purchase any goods or services, and I may revoke my consent anytime. Message/data rates may apply.';
                expressConsent = fe_replaceTag(expressConsent, '{mobile-number}', phoneFormatted);

                if ((FormsEngine.SmartMatchSchoolNames == undefined || FormsEngine.SmartMatchSchoolNames.length == 0) && FormsEngine.CampaignDetail.AllowRemonetization === false) {
                    $(userAgreementControl).hide();

                    $(FormsEngine.SubmitButton).trigger("click");
                }

            } else {
                expressConsent = fe_replaceTag(expressConsent, '{school}', '');
            }
        }

        if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.IsNewTCPAExperience) {
            $(userAgreementControl).css({ "display": "none" });
            $(userAgreementControl).prop('checked', true);
            $(userAgreementControl).prop('type', 'hidden');

            if (FormsEngine.HasSmartMatches === true) {
                var tcpaDiv = $(document.createElement('div'));
                tcpaDiv.attr({ 'class': 'field-holder form-group mctcpasection' })
                var preLabelElement = $(document.createElement('label'));
                preLabelElement.css({ 'font-size': '11px', 'font-weight': 'normal' })
                preLabelElement.html("*I acknowledge that, by clicking the checkbox as my official signature, I consent to representatives of")
                var ulElement = $(document.createElement('ul'));
                ulElement.attr({ 'class': 'smtcpalist field-holder-group checkbox' });

                var saliElement = $(document.createElement('li'));
                saliElement.css({ 'list-style-type': 'none' })
                var salabel = $("<label>").attr({ 'for': 'smtcpaselectall' });
                salabel.css({ 'font-size': '14px', 'font-weight': 'bold' })
                salabel.html("Select All");

                var sacheckbox = $(document.createElement('input')).attr({ id: 'smtcpaselectall', type: 'checkbox', novalidate: 'true', class: 'smtcpaselectall' });
                saliElement.append(sacheckbox);
                saliElement.append(salabel);
                ulElement.append(saliElement);

                if (FormsEngine.HasSmartMatches) {
                    $.each(FormsEngine.TCPASchoolList, function (index, value) {
                        var liElement = $(document.createElement('li'));
                        liElement.css({ 'list-style-type': 'none' })
                        var label = $("<label>").attr({ 'for': 'smtcpaitem' + '_' + value.InstitutionId });
                        label.html(value.InstitutionName);

                        var checkbox = $(document.createElement('input')).attr({ id: 'smtcpaitem_' + value.InstitutionId, type: 'checkbox', novalidate: 'true', class: 'smtcpaitem', value: value.InstitutionId, code: 'smtcpaitem_' + value.InstitutionId, name: 'smtcpaitem_' + value.InstitutionId });
                        liElement.append(checkbox);
                        liElement.append(label);
                        ulElement.append(liElement);
                    })
                }

                var postLabelElement = $(document.createElement('label'));
                postLabelElement.css({ 'font-size': '11px', 'font-weight': 'normal' })
                postLabelElement.html("contacting me about educational opportunities via email, text, or phone, including my mobile phone number(s) " + phoneFormatted + ", using an automatic dialer.  Message and data rates may apply. I understand that my consent is not a requirement for enrollment, and I may withdraw my consent at any time.")
                tcpaDiv.append(preLabelElement);
                tcpaDiv.append(ulElement);
                tcpaDiv.append(postLabelElement);

                if (!$('.mctcpasection').exists()) {
                    $(tcpaDiv).insertBefore(eddyUserAgreementControl);
                } else {
                    $('.mctcpasection').replaceWith(tcpaDiv);
                }

                if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.SelectAllSmartMatchTCPA) {
                    if ($('.smtcpaitem').exists()) {
                        $('.smtcpaitem').each(function () {
                            $(this).rules("add", {
                                required: true,
                                messages: {
                                    required: "You must consent to be contacted by " + $(this).next('label').text() + " prior to submitting this form "
                                }
                            });
                        });
                    } 
                } else {
                    if ($('.smtcpaitem').exists()) {
                        $('.smtcpaitem').each(function () {
                            $(this).rules("add", {
                                atLeastOneTCPAChecked: true,
                            });
                        });
                    }
                }

                //Configure tcpa multcheckbox select
                $(".smtcpaselectall").click(function (e) {
                    var isChecked = this.checked;
                    $(".smtcpaitem").each(function () {
                        $(this).prop('checked', isChecked);
                        $(this).trigger('change');
                        $(this).siblings('.error').remove();
                    });

                    FormsEngine.TCPASelectedSchoolList = [];

                    if (this.checked) {
                        $(".smtcpaitem").each(function () {
                            FormsEngine.TCPASelectedSchoolList.push($(this).val());
                        });
                    }

                });

                $(".smtcpaitem").click(function (e) {

                    var index;

                    if (!this.checked) {
                        $('.smtcpaselectall').prop('checked', false);
                        //remove from selected TCPA list
                        while ((index = FormsEngine.TCPASelectedSchoolList.indexOf($(this).val())) !== -1) {
                            FormsEngine.TCPASelectedSchoolList.splice(index, 1);
                        }
                    } else {
                        FormsEngine.TCPASelectedSchoolList.push($(this).val());
                    }

                    if (FormsEngine.LandingPageDetails.SelectAllSmartMatchTCPA === false) {
                        $(".smtcpaitem").each(function () {
                            $(this).siblings('.error').remove();
                        });
                    }
                });
            }
        }

        //Third party
        if (FormsEngine.HasThirdPartySmartMatches === true) {
            expressConsent = fe_replaceTag(expressConsent, '{thirdpartyschool}', FormsEngine.ThirdPartySchoolNames);
            if (FormsEngine.HasThirdPartyLoanSmartMatches == true) {
                expressConsent = fe_replaceTag(expressConsent, '{thirdpartytype}', ' and/or loan consolidation');
            }
            else {
                expressConsent = fe_replaceTag(expressConsent, '{thirdpartytype}', '');
            }
        }
        else {
            expressConsent = fe_replaceTag(expressConsent, '{thirdpartyschool}', '');
            expressConsent = fe_replaceTag(expressConsent, '{thirdpartytype}', ' ');
        }

        //Submit button text
        if (FormsEngine.SubmitButtonLabelTextLast) {
            expressConsent = fe_replaceTag(expressConsent, '{buttontext}', FormsEngine.SubmitButtonLabelTextLast);
        }

        //Event to manage Custom TCPA manipulation
        if (FormsEngine.TCPAMessageChanged) {
            expressConsent = FormsEngine.TCPAMessageChanged(expressConsent);
        }
        
        setUserAgreementText(expressConsent, eddyExpressConsent);

        if (FormsEngine.ApplicationId == emsApplicationId) {
            $('[name="' + FormsEngine.RenderingDiv + '"]').find("#useragreement-style").remove();

            var userAgreementControl = $(FormsEngine.DefaultFormTag).find("input[code='UserAgreement']");
            if (FormsEngine.UserAgreementAsLabel || FormsEngine.UserAgreementAsLabel == null) {
                $(userAgreementControl).css({ "display": "none" });
                $(userAgreementControl).prop('checked', true);
                $(userAgreementControl).prop('type', 'hidden');
            }
        }

        

        //Changed TCPA
        if (FormsEngine.LastUAText != expressConsent) {

            //Business requirement to avoid false pre-check detection, uncheck if TCPA message changes
            var tcpaControl = $(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]');
            if ($(tcpaControl).is(':checked')) {
                $(tcpaControl).checked = false;
                $(tcpaControl).removeAttr('checked');
                $(tcpaControl).trigger('change');
            }


            //LeadId changes for express consent, legacy recapture requested by leadid.
            try {
                if (typeof LeadiD != 'undefined') {
                    LeadiD.formcapture.init();
                }
            }
            catch (ex) { }
        }
        FormsEngine.LastUAText = expressConsent;
    }

    function defaultCurrentInstitutionSmartMatch() {
        FormsEngine.HasSmartMatches = true;
        FormsEngine.SmartMatchSchoolNames = FormsEngine.InstitutionName + ' ';
        FormsEngine.ThirdPartySchoolNames = '';
        FormsEngine.SMLeadsCreatedCount = 0;

        //Kaplan Purdue hardcoded rules
        //Only Kaplan
        if (FormsEngine.InstitutionName.search("Kaplan University") > -1) {
            FormsEngine.SmartMatchSchoolNames = "Purdue Global/Kaplan North America, LLC. ";
        }
        //Only Purdue
        else if (FormsEngine.InstitutionName.search("Purdue Global") > -1) {
            FormsEngine.SmartMatchSchoolNames = "Purdue Global/Kaplan North America, LLC. ";
        }

        if (FormsEngine.InstitutionName.search("National University") > -1) {
            FormsEngine.SmartMatchSchoolNames = "National University and National University System affiliates (City University of Seattle, Northcentral University and National University Virtual High School) ";
        }

        if (FormsEngine.InstitutionName.search("Northcentral University") > -1) {
            FormsEngine.SmartMatchSchoolNames = "Northcentral University and its National University Affiliates ";
        }

        if (FormsEngine.InstitutionName.search("Lynn University") > -1) {
            FormsEngine.SmartMatchSchoolNames = "Lynn University/Kaplan North America, LLC. ";
        }

        checkUserAgreementText();

        if (FormsEngine.ShowTwoULeadShareControl) {
            fe_showUserAgreementForTwoULeadShareSchool(FormsEngine.InstitutionName, $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected").attr('data-programtype'), '#DynamicQuestionsUserAgreementSection');
        }
        else {
            fe_removeUserAgreementForTwoULeadShareSchool();
        }
    }

    function processLynnTCPA(smartMatchList) {

        var ncIndex = -1;

        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i].InstitutionName == "Lynn University") {
                ncIndex = i;
            }
        }

        //Northcentral University hardcoded rules
        if (ncIndex > -1) {
            smartMatchList[ncIndex].InstitutionName = "Lynn University/Kaplan North America, LLC.";
        }

    }

    function processPurdueKaplanTCPA(smartMatchList) {

        var kIndex = -1;
        var pIndex = -1;

        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i].InstitutionName == "Kaplan University") {
                kIndex = i;
            }
            else if (smartMatchList[i].InstitutionName == "Purdue Global") {
                pIndex = i;
            }
        }

        //Kaplan Purdue hardcoded rules
        //Both exist
        if (kIndex > -1 && pIndex > -1) {
            smartMatchList[kIndex].InstitutionName = "Purdue Global/Kaplan North America, LLC.";
            smartMatchList[pIndex].InstitutionName = "";
        }
        else if (kIndex > -1) {
            smartMatchList[kIndex].InstitutionName = "Purdue Global/Kaplan North America, LLC.";
        }
        else if (pIndex > -1) {
            smartMatchList[pIndex].InstitutionName = "Purdue Global/Kaplan North America, LLC.";
        }

    }

    function processNorthcentralUniversityTCPA(smartMatchList) {
        
        var ncIndex = -1;

        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i].InstitutionName == "Northcentral University") {
                ncIndex = i;
            }
        }

        //Northcentral University hardcoded rules
        if (ncIndex > -1) {
            smartMatchList[ncIndex].InstitutionName = "Northcentral University and its National University Affiliates";
        }

    }

    function processNationallUniversityTCPA(smartMatchList) {

        var ncIndex = -1;

        for (var i = 0; i < smartMatchList.length; i++) {
            if (smartMatchList[i].InstitutionName == "National University") {
                ncIndex = i;
            }
        }

        //Northcentral University hardcoded rules
        if (ncIndex > -1) {
            smartMatchList[ncIndex].InstitutionName = "National University and National University System affiliates (City University of Seattle, Northcentral University and National University Virtual High School)";
        }

    }

    function getSmartMatchesFromME() {

        if (FormsEngine.ProcessingSM === true) {
            return;
        }
        georgiaBandaid();
        fe_showLoader();

        var serviceArgs = getSMCallServiceArgs();
        // compare new Lead Data with previous Lead Data to avoid calling again
        if (FormsEngine.LeadDataEncoded != FormsEngine.LastSMCallLeadDataEncoded || (fe_wiz_getLastStep() == FormsEngine.CurrentStep && validateLastStepRequiredFields())) {

            FormsEngine.ProcessingSM = true;

            // need session for this call
            fe_getSessionId(function () {
                serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
                var sUrl = FormsEngine.ServiceBaseURL + "/TemplateManager/GetMatchingEngineWizardResponse" + serviceArgs;
                fe_consolelog('Calling ME for new SMs.');
                $.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: sUrl,
                    success: function (data) {
                        // store the last me call
                        FormsEngine.LastSMCallLeadDataEncoded = FormsEngine.LeadDataEncoded;
                        FormsEngine.HasThirdPartySmartMatches = false;

                        FormsEngine.TCPASchoolList = [];
                        FormsEngine.TCPASelectedSchoolList = [];
                        FormsEngine.IsTitaniumOrNoSmartMatch = false;
                        FormsEngine.CustomTCPAList = [];

                        fe_setSessionObject("DuplicateForInstitutionList", JSON.stringify(data.DuplicateForInstitutionList), function () { });
                        FormsEngine.DuplicateForInstitutionList = data.DuplicateForInstitutionList;
                        if (data != undefined && data != null) {
                            FormsEngine.LimboAlternativeCampaignTrackid = data.LimboAlternativeTrackId;
                            FormsEngine.LimboAlternativeCampaignTrackidUtilized = (data.LimboAlternativeTrackIdUtilized == true);
                            FormsEngine.MatchResponseGuid = data.MatchResponseGuid;
                          
                            if (data.SmartMatchList.length > 0 || data.ThirdPartyMatchList.length > 0) {
                                FormsEngine.HasSmartMatches = true;
                                var removedSchools = [];
                                FormsEngine.SmartMatchSchoolNames = '';
                                FormsEngine.ThirdPartySchoolNames = '';
                                processPurdueKaplanTCPA(data.SmartMatchList);
                                processNorthcentralUniversityTCPA(data.SmartMatchList);
                                processNationallUniversityTCPA(data.SmartMatchList);
                                processLynnTCPA(data.SmartMatchList);
                                for (var i = 0; i < data.SmartMatchList.length; i++) {

                                    FormsEngine.CustomTCPA = data.SmartMatchList[i].CustomTCPA;

                                    if (data.SmartMatchList[i].ProductId == 52) {
                                        FormsEngine.IsTitaniumOrNoSmartMatch = true;
                                     }

                                    //keep track of smart match institutions to show the tcpa for
                                    FormsEngine.TCPASchoolList.push({ InstitutionId : data.SmartMatchList[i].InstitutionId, InstitutionName : data.SmartMatchList[i].InstitutionName })

                                    //keep track of institutions with custom tcpa
                                    FormsEngine.CustomTCPAList.push({ InstitutionId: data.SmartMatchList[i].InstitutionId, InstitutionName: data.SmartMatchList[i].InstitutionName, CustomTCPA: data.SmartMatchList[i].CustomTCPA });

                                    if (!data.SmartMatchList[i].Is2USchool && data.SmartMatchList[i].InstitutionName != "") {
                                        FormsEngine.SmartMatchSchoolNames += data.SmartMatchList[i].InstitutionName + ', ';
                                    }
                                    else if (data.SmartMatchList[i].ShowTwoULeadShareControl) {
                                        fe_showUserAgreementForTwoULeadShareSchool(data.SmartMatchList[i].InstitutionName, data.SmartMatchList[i].ProgramType, '#DynamicQuestionsUserAgreementSection');
                                    }
                                    else {
                                        //lets add this to a collection to keep track of removed schools
                                        if ($.inArray(data.SmartMatchList[i].InstitutionId, removedSchools) == -1) {
                                            removedSchools.push(data.SmartMatchList[i].InstitutionId);
                                        }
                                    }
                                }
                                if (data.ThirdPartyMatchList.length > 0) {
                                    FormsEngine.HasThirdPartySmartMatches = true;
                                    for (var i = 0; i < data.ThirdPartyMatchList.length; i++) {

                                        FormsEngine.CustomTCPA = data.ThirdPartyMatchList[i].CustomTCPA;

                                        FormsEngine.ThirdPartySchoolNames += data.ThirdPartyMatchList[i].InstitutionName + ', ';

                                        //keep track of smart match institutions to show the tcpa for
                                        FormsEngine.TCPASchoolList.push({ InstitutionId: data.ThirdPartyMatchList[i].InstitutionId, InstitutionName: data.ThirdPartyMatchList[i].InstitutionName })

                                        if (data.ThirdPartyMatchList[i].InstitutionType == 2) {
                                            FormsEngine.HasThirdPartyLoanSmartMatches = true;
                                        }
                                    }
                                    FormsEngine.ThirdPartySchoolNames = FormsEngine.ThirdPartySchoolNames.replace(/,\s*$/, "");
                                }

                                if (removedSchools.length > 0) {
                                    FormsEngine.RemovedTwoUSchools = removedSchools.join(',');
                                }
                            } else {
                                FormsEngine.HasSmartMatches = false;
                                FormsEngine.SmartMatchSchoolNames = '';
                                FormsEngine.IsTitaniumOrNoSmartMatch = true;
                                fe_consolelog('No SMs returned from ME');
                            }
                        } else {
                            FormsEngine.HasSmartMatches = false;
                            FormsEngine.SmartMatchSchoolNames = '';
                            FormsEngine.IsTitaniumOrNoSmartMatch = true;
                            fe_consolelog('ME data is null.');
                        }

                        //added this function to see if we need to hide/bypass or display the EDDY User Agreement
                        toggleEDDYUserAgreementByProduct();

                        checkUserAgreementText();
                        fe_hideLoader(false);
                        FormsEngine.ProcessingSM = false;
                    },
                    error: function (request, textStatus, errorThrown) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                        fe_logClientException(request, sUrl, errorThrown);
                        fe_hideLoader(false);
                        FormsEngine.ProcessingSM = false;
                    }
                });
            });
        }
        else {
            fe_consolelog('User Data has not changed, not calling ME for new SMs.');
            fe_hideLoader(false);
        }
    }

    function toggleEDDYUserAgreementByProduct() {

        var eddyUserAgreementControl = $(FormsEngine.DefaultFormTag).find('input[code="EDDYUserAgreement"]').parents('div.field-holder');

        if (eddyUserAgreementControl.exists()) {
            if (FormsEngine.LandingPageDetails && FormsEngine.LandingPageDetails.IsNewTCPAExperience) {
                //only show the eddy tcpa if there is no sm or one of the smart mmatched product is titanium
                if (FormsEngine.IsTitaniumOrNoSmartMatch) {
                    $(eddyUserAgreementControl).show();
                } else {
                    $(eddyUserAgreementControl).hide();
                    $(eddyUserAgreementControl).css({ "display": "none" });
                    $(eddyUserAgreementControl).prop('checked', true);
                    $(eddyUserAgreementControl).prop('type', 'hidden');
                }
            } else {
                $(eddyUserAgreementControl).hide();
                $(eddyUserAgreementControl).css({ "display": "none" });
                $(eddyUserAgreementControl).prop('checked', true);
                $(eddyUserAgreementControl).prop('type', 'hidden');
            }
        }
    }

    function getSMCallServiceArgs() {
        var sMLeadsCreatedCount = (FormsEngine.SMLeadsCreatedCount != undefined && FormsEngine.SMLeadsCreatedCount != null) ? FormsEngine.SMLeadsCreatedCount : 0;
        var uSLeadsCreatedCount = (FormsEngine.USLeadsCreatedCount != undefined && FormsEngine.USLeadsCreatedCount != null) ? FormsEngine.USLeadsCreatedCount : 0;

        // get latest form data
        var UserSubmission = getFormData();
        FormsEngine.LeadDataEncoded = UserSubmission.LeadDataEncoded;
        FormsEngine.LeadAdditionalDataEncoded = UserSubmission.LeadAdditionalDataEncoded;
        fe_saveWorkflowData(function () { }); // update the SessionDTO


        FormsEngine.ProgramTemplateId = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected").attr('data-templateid');
        FormsEngine.ProgramTemplateId = FormsEngine.ProgramTemplateId == undefined || FormsEngine.ProgramTemplateId == null ? "" : FormsEngine.ProgramTemplateId;


        var serviceArgs = '?IsBeta=' + FormsEngine.IsBeta;
        serviceArgs += '&WizardTemplateId=' + FormsEngine.TemplateId;
        serviceArgs += '&TrackId=' + FormsEngine.TrackId;
        serviceArgs += '&SessionId=' + FormsEngine.SessionId;
        serviceArgs += '&MatchId=' + FormsEngine.MatchResponseGuid;
        serviceArgs += '&LeadData=' + FormsEngine.LeadDataEncoded
        serviceArgs += '&AdditionalData=' + FormsEngine.LeadAdditionalDataEncoded;
        serviceArgs += '&PreviousSMLeadCount=' + sMLeadsCreatedCount;
        serviceArgs += '&PreviousUSLeadCount=' + uSLeadsCreatedCount;
        serviceArgs += '&DeviceId=' + FormsEngine.DeviceId;
        serviceArgs += '&FormTemplateType=' + FormsEngine.FormTemplateType;
        serviceArgs += '&ProgramTemplateId=' + FormsEngine.ProgramTemplateId;
        serviceArgs += '&ApplicationId=' + FormsEngine.ApplicationId;

        return serviceArgs;
    }


    function RemoveScrollbar(category) {
        //business's subcategory does NOT show scrollbar            
        if ($('.interest-field.Categories li input:checkbox[checked="checked"]').length >= 2 || $('.interest-field.Categories li input:checkbox[checked="true"]').length >= 2) {
            $('.interest-field.SubCategories').removeClass('no-scroll');
        }
        else if ($('.interest-field.Categories li input:checkbox[checked="checked"]').length == 1 || $('.interest-field.Categories li input:checkbox[checked="true"]').length == 1) {
            category_chk = $('.interest-field.Categories li input:checkbox[checked="checked"]');
            //category not set, then as long as only one category checked, no scrollbar showes

            if (category == '' || typeof (category) === 'undefined') {
                fe_consolelog("add no-scroll class");
                $('.interest-field.SubCategories').addClass('no-scroll');
            }
            else {
                if ($(category_chk).next().text() == category) {
                    $('.interest-field.SubCategories').addClass('no-scroll');
                }
                else {
                    $('.interest-field.SubCategories').removeClass('no-scroll');
                }
            }
        }
    }


    //Delayed function execution of Load Form from session
    function loadFormFromSession(callback) {
        if (FormsEngine.LoadFormFromSession == undefined && FormsEngine.DelayedLoad == undefined) {
            FormsEngine.DelayedLoad = window.setInterval(function () { loadFormFromSession(callback) }, 300)
        }
        else if (FormsEngine.LoadFormFromSession != undefined) {
            if (FormsEngine.DelayedLoad != undefined) {
                window.clearInterval(FormsEngine.DelayedLoad);
            }
            FormsEngine.LoadFormFromSession(callback);
        }
    }


    //Delayed function execution of Load Form Quick
    function loadFormQuick(callback) {
        if (FormsEngine.LoadFormQuick == undefined && FormsEngine.DelayedLoadQ == undefined) {
            FormsEngine.DelayedLoadQ = window.setInterval(function () { loadFormQuick(callback) }, 300)
        }
        else if (FormsEngine.LoadFormQuick != undefined) {
            if (FormsEngine.DelayedLoadQ != undefined) {
                window.clearInterval(FormsEngine.DelayedLoadQ);
            }
            FormsEngine.LoadFormQuick(callback);
        }
    }


    // Additional load after form gets recovered.
    function formAdditionalLoad() {
        //Display Current Step
        $("#Step" + FormsEngine.CurrentStep).show();

        //Placeholder support
        if (!hasPlaceholderSupport()) {
            fe_loadJsWithCallback('Templates/Common/js/jquery.placeholder.min.js', function () {
                $(FormsEngine.DefaultFormTag).find(':input').placeholder();
            });
        }

        //Google analytics and internal tracking initial firing
        trackEvent();



        //navBar initial firing
        refreshNavbar();


    }

    function cleanLastBind() {
        if (FormsEngine.Questions != undefined) {
            for (var index = 0; index < FormsEngine.Questions.length; index++) {
                if (FormsEngine.Questions[index].Code == 'Categories' || FormsEngine.Questions[index].Code == 'Specialties' || FormsEngine.Questions[index].Code == 'SubCategories') {
                    FormsEngine.Questions[index].LastDataBindFilters = 'none';
                }
            }
        }
    }

    function start_loadFormWizard() {
        fe_consolelog("start_loadFormWizard EVENT TRIGGERED");

        if (FormsEngine.DebugMode === true) {
            FormsEngine.createCookie('FE_DebugMode', 'true');
        }
        //Old IE compatibility issue
        if ($.browser && $.browser.msie) {
            $('head').append('<meta http-equiv="x-ua-compatible" content="IE=edge" />');
        }

        // Check Mobile device
        var isMobile = false;
        (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) isMobile = true })(navigator.userAgent || navigator.vendor || window.opera);

        FormsEngine.IsMobile = isMobile;


        MovingStep = false;
        FormsEngine.CurrentStep = 1;

        //Dynamic questions step
        FormsEngine.StepDynamicQuestions = $("div[data-title='DynamicQuestions']").attr("data-step");

        if (FormsEngine.RenderingStrategy != "SCHOOLPICKERWIZARD") {
            FormsEngine.StepLast = $(FormsEngine.DefaultFormTag).find("[id^=Step]").length - 1;
            FormsEngine.StepTotal = $(FormsEngine.DefaultFormTag).find("[id^=Step]").length - 1;
        } else {
            FormsEngine.StepLast = $(FormsEngine.DefaultFormTag).find("[id^=Step]").length;
            FormsEngine.StepTotal = $(FormsEngine.DefaultFormTag).find("[id^=Step]").length;
        }

        


        //Set defaults
        configureDefaults();



        //Move step
        FormsEngine.MoveStep = moveStep;

        //Force re-bind
        cleanLastBind();

        //Load Form
        loadFormQuick(function () {
            formAdditionalLoad();

            fe_setPlacementGuid();

            //set the status of controls with values
            fe_setControlStatus();
            //bind status update events to all required controls
            fe_bindControlStatusEvent();

            fe_loadFormFromPassThru();
        });

        //hide labels and swap watermarks if need be
        fe_checkResponsiveItems();

        //set hash location for step 1
        $(FormsEngine.DefaultFormTag).click(function () {
            if (FormsEngine.CurrentStep == 1 && window.location.hash != "#_Step1") {
                window.location.hash = "#_Step" + FormsEngine.CurrentStep;
            }
        });

        //Form Loaded event
        if (FormsEngine.OnFormLoaded) {
            FormsEngine.OnFormLoaded();
        }

        //ControlValue set event
        FormsEngine.OnControlValueSet = onControlValueSet;

        FormsEngine.DynamicRequiredChangeEvent = onDynamicChangeEvent;

        //Form navigation exposed
        FormsEngine.CheckNavigation = checkNavigation;

        //Dynamic questions exposed for PW one step
        FormsEngine.LoadDynamicQuestions = onLoadDynamicQuestions;

        //Google Tag Manager event
        fe_googleTagEvent('gaEvent', 'client', 'wizard-start', FormsEngine.TemplateId);

        fe_ApplyPhoneMask();

        checkValidMobileButton();

        $('.eddy-form-container').find('input, select').change(function () { checkValidMobileButton(); });

        //E-mail and opt in on the same step no auto advance from e-mail.
        var email = $(':input[code="Email"]');
        var optin = $(':input[code="NewsLetterOptIn"]');
        if (email.exists() && optin.exists() && $(email).attr('step') === $(optin).attr('step')) {
            var emailstep = $(email).attr('step');
            var optinstep = $(optin).attr('step');
            var emailsortid = $(email).attr('id-sort');

            //check if opt in is next and in that step
            var nextquestion = fe_wiz_getNextQuestionInStep(emailstep, emailsortid);
            if ($(nextquestion).attr('code') == 'NewsLetterOptIn') {
                //check that opt in is last in that step
                var nextnextquestioninstep = fe_wiz_getNextQuestionInStep(emailstep, parseInt(emailsortid, 0) + 1);
                if (nextnextquestioninstep.length == 0) {
                    //if yes to all above bind click to the step in question and have it call autoforwardstep
                    $('#Step' + emailstep).click(function () { fe_wiz_AutoForwardStep(); });
                }

            }
        }


        if (typeof $().select2 == "function") {
            var $selectControl = $(FormsEngine.DefaultFormTag).find("select.typeahead.select2");
            if ($selectControl.length > 0) {
                // if typeahead and inline
                if ($selectControl.hasClass("typeahead") && $selectControl.hasClass("inlineDropDown")) {
                    if ($selectControl.hasClass("searchablePredfinedValueList")) {
                        if ($selectControl.find("option[value='']").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            $selectControl.select2({
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }

                    } else {
                        if ($selectControl.find("option[value='']").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.find("option[value='']").eq(0).text()
                            });
                        }
                        else if ($selectControl.siblings("label.inline-hidden-label").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.siblings("label.inline-hidden-label").eq(0).text()
                            });
                        }
                        else {
                            $selectControl.select2();
                        }
                    }
                }
                // if just typeahead
                else if ($selectControl.hasClass("typeahead")) {
                    if ($selectControl.hasClass("searchablePredfinedValueList")) {
                        if ($selectControl.find("option[value='']").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            $selectControl.select2({
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }

                    } else {
                        if ($selectControl.find("option[value='']").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.find("option[value='']").eq(0).text()
                            });
                        }
                        else {
                            $selectControl.select2();
                        }
                    }
                }
                // if just inline
                else if ($selectControl.hasClass("inlineDropDown")) {
                    if ($selectControl.hasClass("searchablePredfinedValueList")) {
                        if ($selectControl.find("option[value='']").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            $selectControl.select2({
                                ajax: {
                                    url: FormsEngine.ServiceBaseURL + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: $selectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },
                                    cache: true

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                $(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }

                    } else {
                        if ($selectControl.siblings("label.inline-hidden-label").length > 0) {
                            $selectControl.select2({
                                minimumResultsForSearch: Infinity,
                                placeholder: $selectControl.siblings("label.inline-hidden-label").eq(0).text()
                            });
                        }
                        else {
                            $selectControl.select2({
                                minimumResultsForSearch: Infinity
                            });
                        }
                    }
                }
                // if just select2
                else {
                    $selectControl.select2();
                }
            }
        }
    }

    
    $(document).ready(function () {
        window.FormsEngine = window.FormsEngine || {};
        window.FormsEngine.JornayaDelayUntilFirstUserInteraction = true;
        $(FormsEngine).on("loadFormWizard", start_loadFormWizard);
        $(FormsEngine).on("OnProgramSet", onProgramSet);
    });

    return {
        currentStepIsEmpty: currentStepIsEmpty
    };

})(jQuery);
