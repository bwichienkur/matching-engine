//taken from wizard
(function ($) {


    // Constants
    FormsEngine.DefaultFormTag = "#eddynexusform-wizard";
    FormsEngine.DefaultCountryCode = "US";
    FormsEngine.DefaultValidationLevel = 3;
    FormsEngine.DefaultSelectText = "- Select -";
    FormsEngine.GlobalErrorBoxExists = $(FormsEngine.DefaultFormTag).find("#ErrorBox").exists();
    FormsEngine.SectionErrorBoxExists = $('[id^="ErrorBoxSection"]').exists();
    FormsEngine.SubmitButton = "#wizard-form-submit-button";
    FormsEngine.SubmitButtonLabel = "#form-submit-button-label";
    if (FormsEngine.SubmitButtonLabelTextNormal != 'Next') {
        FormsEngine.SubmitButtonLabelTextNormal = "Continue";
    }
    FormsEngine.BackButton = "#form-navback-button";
    FormsEngine.CurrentStep = 1;
    FormsEngine.Source = 'IS_FormsEngine_QDFTemplate';
    FormsEngine.replace_Phone = "replace_Phone";
    FormsEngine.replace_Alternate_Phone = "replace_Alternate_Phone";
    FormsEngine.OnFormLoad = function () { onFormLoad(); }; //Will be triggered after the form is reloaded and binded
    FormsEngine.PendingAsyncValidations = new Array();
    FormsEngine.HasAdditionalQuestions = false;


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
            if (FormsEngine.RecoverSplitPhoneFields) {
                FormsEngine.RecoverSplitPhoneFields();
            }
        }
        else if (controlCode == 'Alternate_Phone' && controlValue != '') {
            if (FormsEngine.RecoverSplitPhoneFields) {
                FormsEngine.RecoverSplitPhoneFields();
            }
        }
        else if (controlCode == 'Highest_Level_of_Education_Completed') {
            if (controlValue == '1') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Year_of_Highest_Education_Completed']"))
                ) {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }

                if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()) {
                    if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']"))) {
                        $(FormsEngine.DefaultFormTag).find("div[data-controlcode='K12']").show();
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

            if (controlValue === '10' || controlValue === '11') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))
                ) {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Desired_Degree_Level']").show();
                }
            }
            else {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))
                ) {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Desired_Degree_Level']").hide();
                    jQuery(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").each(function () { jQuery(this).attr("checked", false); });
                }
            }

        }
        else if (controlCode === 'us_citizen') {

            if (controlValue === 'No') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']"))) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").val('126');
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Military_Affiliation']").hide();
                }
                if ($(FormsEngine.DefaultFormTag).find(":input[code='GCH']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='GCH']"))) {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='GCH']").show();
                }
            }
            else if (controlValue === 'Yes') {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='GCH']").exists()
                    && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='GCH']"))) {
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").val('No');
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='GCH']").hide();
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
            var stateCode = $(FormsEngine.DefaultFormTag).find(':input[code="State"]').val();
            var session = ""
            try {
                session = jQuery.cookie('_Session');
            }
            catch (e) { }

            $(FormsEngine.DefaultFormTag).find("select[code='Country']").val('US');
            fe_logClientException(null, "", "Georgia Bandaid3 applied State=" + stateCode + " PostalCode=" + postalCode + " TrackingSession=" + session);
        }
    }

    //Will be triggered every time the form is reloaded (session, querystring, etc)
    function onFormLoad() {
        if (FormsEngine.ReplaceDefaultOptionFields != undefined) {
            FormsEngine.ReplaceDefaultOptionFields();
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

        if ($(FormsEngine.DefaultFormTag).find('input[code="' + ErrorCode + '_{' + controltypename + '}"]').exists()) {
            errorMessage = $(FormsEngine.DefaultFormTag).find('input[code="' + ErrorCode + '_{' + controltypename + '}"]').val().replace('{Control}', label).replace('{NumberMin}', minLength).replace('{NumberMax}', maxLength);
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

    //jQuery Validation extension method to support phone check rule
    var CustomPhoneErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "value", "ERR_Valid");
    }

    jQuery.validator.addMethod(
        "phoneservercheck",
        function (value, element, param) {
            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val();
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

    //jQuery Validation extension method to support zipCode country/state validation
    var CustomZipErrorMessage = function (value, element, param) {

        //return getErrorMessage(element, "value", "ERR_ZipCountryState");
        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        return getErrorMessage(element, "value", "ERR_ZipCountryState").replace('{ZipValue}', postalCode);
    }

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

        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        return getErrorMessage(element, "value", "ERR_ZipCodeInvalid").replace('{ZipValue}', postalCode);

    }

    // Validator for just Zippies..
    jQuery.validator.addMethod(
        "ziponlycheck",
        function (value, element, param) {
            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val();
            var stateCode = $(FormsEngine.DefaultFormTag).find("select[code='State']").val();
            var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();

            // if country is on the same step
            // check when appearing before and after the zip code..
            if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('Country')) {

                if (countryCode !== "" && ["US", "CA"].indexOf(countryCode) < 0) {
                    return true;
                }
            }

            // if state is on the same step
            // check when appearing before and after the zip code..
            if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('State')) {

                if (stateCode == "" && $(FormsEngine.DefaultFormTag).find('select[code="State"]:visible').exists()) {
                    return true;
                }
            }

            if (postalCode == "") {
                return true;
            }

            value = postalCode;
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ZipCodeStateCountryCheck/?CountryCode=" + countryCode + "&StateCode=" + stateCode + "&ZipCode=" + postalCode + "&ZipOnly=true" + "&n=", CustomZipInvalidErrorMessage);

        }
        ,
        CustomZipInvalidErrorMessage
    );

    //jQuery Validation extension method to support email validation rule
    var CustomEmailErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "", "ERR_Email");
    }
    jQuery.validator.addMethod(
        "emailservercheck",
        function (value, element, param) {
            //EmailCheckEx(string EmailAddress, bool XVerify, bool Background, string Experiment, string TrackId)
            var arguments = "TrackId=" + (FormsEngine.readCookie('_Session') ? FormsEngine.readCookie('_Session') : "");
            arguments += "&XVerify=" + (FormsEngine.UseXVerify === true).toString();
            arguments += "&Immediate=" + (FormsEngine.XVerifyImmediate === true).toString();
            arguments += "&Experiment=" + (FormsEngine.ExperimentName ? FormsEngine.ExperimentName : "");
            fe_consolelog(arguments);
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/EmailCheckEx/?" + arguments + "&EmailAddress=", CustomEmailErrorMessage);
        },
        CustomEmailErrorMessage
    );


    //Required field, minlength, maxlength custom messages
    jQuery.extend(jQuery.validator.messages, {
        required: function (value, element, param) {
            var message = "";
            if ($(element).attr('code') === 'UserAgreement') {
                /*message = "You must indicate that you have read and acknowledge our terms and conditions before submitting this form.";*/
                message = "You must consent to be contacted to submit this form.";
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

        if (State != undefined && $(State).val() != "" && $(State).hasClass('error')) {
            if ($(State).valid()) {
                fe_hideValidationError(State);
            }
        }
        if (Country != undefined && $(Country).val() != "" && $(Country).hasClass('error')) {
            if ($(Country).valid()) {
                fe_hideValidationError(Country);
            }
        }
    }

    function getMetaData(data) {
        //Get the resource meta data texts.
        FormsEngine.ResourceData = data;
        setupMetaData(data);
    }

    function setupMetaData(data) {
        
        fe_getCampaignDetailByTrackId(FormsEngine.TrackId, function (data) {
            FormsEngine.CampaignDetail = data;

        });
    }


    //Configure defaults on every template reload
    function configureDefaults() {

        //jquery validator defaults
        $.validator.setDefaults({
            onkeyup: false
            //onfocusout: true,
            //onsubmit: true
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
                        error.appendTo($(FormsEngine.DefaultFormTag).find('#ErrorBox' + element.attr('section')));
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
                // DEV-2618:
                // Trigger City/State/Country lookup when Postal Code changes
                // so hidden fields and corresponding key attributes are populated
                // before Matching Engine submission.
                $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").blur(function (event) {
                    copyZipCodeValues($(this).attr('code'), $.trim($(this).val()));                  
                    var postalCode = $.trim($(this).val());
                    if (postalCode) {
                        fe_getCityStateCountry(postalCode);
                    }
                });
            }

            if ($(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").exists()) {

                $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code_Duplicate']").rules("add", { zipcitycountrycheck: true });
            }
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
        $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.StepLast).find(':input').each(function () {
            $(this).change(function () {
                var code = $(this).attr('code');
                if (code == undefined || code == null || code == 'UserAgreement') {
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

        $(FormsEngine.DefaultFormTag).find("select[code=Country]").on("change", function () {
            fe_ApplyPhoneMask();
        });

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

            $(FormsEngine.DefaultFormTag).find('.radio-inline label.error').each(function () {
                var parent = $(this).parent().parent();
                parent.append($(this));
            });

            //Validate form
            if ($(FormsEngine.DefaultFormTag).valid() && checkIfStateExistsAndInvalid()) {

                //form is valid. before moving forward check for zip code controls to ensure they are copied
                checkCurrentStepForZipToCopy();

                checkNavigation(1);

                if ($(FormsEngine.DefaultFormTag).find('.error[for="CampusSoftPreference"]').length != 0) {
                    $(FormsEngine.DefaultFormTag).find('.error[for="CampusSoftPreference"]').insertAfter($(FormsEngine.DefaultFormTag).find('.error[for="CampusSoftPreference"]').parents('ul'));
                }

                $(FormsEngine.DefaultFormTag).find('.field-holder.radio .error').each(function () {
                    var parent = $(this).parent();
                    parent.append($(this));
                });

                if ($(FormsEngine.DefaultFormTag).find('.UserAgreement .error').length != 0) {
                    $(FormsEngine.DefaultFormTag).find('.UserAgreement .error').insertBefore($(FormsEngine.DefaultFormTag).find('.UserAgreement .error').parent());l
                }

                RemoveScrollbar();
            }
            else {
                fe_checkShowScreenButton();
                
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
                    else if (jQuery(FormsEngine.DefaultFormTag).find("[code=Desired_Degree_Level]:visible").exists()) {
                        return;
                    }

                    fe_wiz_AutoForwardStep(this);

                }
            }
        });

        $(FormsEngine.DefaultFormTag).find(":input[code='Age']").keypress(function (e) {
            fe_wiz_KeyAutoAdvance($(this), e, 2);
        });

        $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").keypress(function (e) {
            var countryCode = jQuery(FormsEngine.DefaultFormTag).find("select[code='Country']").val() || "";
            var localCountry = (["US", "CA", ""].indexOf(countryCode) >= 0);
            if (!FormsEngine.UseInternationalTemplate && FormsEngine.IsLocalIP === true && localCountry) {
                // Check if Canadian Zip code (xxx xxx)..
                var value = jQuery(this).val();
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
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Year_of_Highest_Education_Completed']").show();
                }

            });
        }

        if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").change(function () {
                if ($(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "1") {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='K12']").show();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='K12']").hide();
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
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Year_of_Highest_Education_Completed']").hide();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Year_of_Highest_Education_Completed']").show();
                }
            });
        }

        //Smart desired grad degree level
        if ($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").exists()
            && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']"))) {

            //both controls exist in the form and are in the order we need so lets add the onchange function
            $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").change(function () {
                var edLevel = $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val();
                if (edLevel == "10" || edLevel == "11") {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Desired_Degree_Level']").show();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Desired_Degree_Level']").hide();
                    jQuery(FormsEngine.DefaultFormTag).find(":input[code='Desired_Degree_Level']").each(function () { jQuery(this).attr("checked", false); });
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
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Military_Affiliation']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else if (!isRadio && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").val() == "No") {
                    //the user selected HS not completed so set the value of the corresponding input and disable it.
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").val('126');
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Military_Affiliation']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='Military_Affiliation']").show();
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
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='GCH']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").trigger("change");
                }
                else if (!isRadio && $(FormsEngine.DefaultFormTag).find(":input[code='us_citizen']").val() == "Yes") {
                    $(FormsEngine.DefaultFormTag).find(":input[code='GCH']").val('No');
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='GCH']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']").trigger("change");
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='GCH']").show();
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
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='FinancialAidProof']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").trigger("change");
                }
                else if (!isRadio &&
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").val() == "No") {
                    //the user the user does not have funds so no need to ask for proof
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").val('No');
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='FinancialAidProof']").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").trigger("change");
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("div[data-controlcode='FinancialAidProof']").show();
                }

            });
            $(FormsEngine.DefaultFormTag).find("div[data-controlcode='FinancialAidProof']").hide(); //hide by default
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

        //Smart Address Edit text
        if (FormsEngine.SmartAddressEditText && FormsEngine.SmartAddressEditText.length > 0) {
            $(FormsEngine.DefaultFormTag).find('#EditSmartAddressText').text(FormsEngine.SmartAddressEditText);
        }
        
        georgiaBandaid();

        setSubmitButtonLabels();
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
            FormsEngine.SubmitButtonLabelTextLast = "Filter Results";
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
        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code']").exists()) {
            copyZipCodeValues('Postal_Code', $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code']").val());
        }
        else if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code_Duplicate']").exists()) {
            copyZipCodeValues('Postal_Code_Duplicate', $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(":input[code='Postal_Code_Duplicate']").val());
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
    
    function delayedValidation(callback) {
        if (FormsEngine.DelayedValidation == undefined) {
            FormsEngine.DelayedValidation = window.setInterval(function () {
                if (FormsEngine.PendingAsyncValidations.length == 0) {
                    if (FormsEngine.DelayedValidation != undefined) {
                        window.clearInterval(FormsEngine.DelayedValidation);
                        FormsEngine.DelayedValidation = undefined;
                    }
                    validStep(callback);
                }
            }, 300);
        }
        else if (FormsEngine.PendingAsyncValidations.length == 0) {
            if (FormsEngine.DelayedValidation != undefined) {
                window.clearInterval(FormsEngine.DelayedValidation);
            }
            validStep(callback);
        }
    }

    function validStep(callback) {
        var valid = true;
        var hasScrolled = false;
        var hasPendingValidations = FormsEngine.PendingAsyncValidations && FormsEngine.PendingAsyncValidations.length > 0;
        fe_showLoader();

        //find all input tags except hidden, with the exception of hiddens used for categories and subcategories
        var fieldsToValidate = $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(":input").not("[controltypename*='Multi Check Box List']").not("[type=hidden]:not([code*='Categories']):not([code='Specialties'])").not("[novalidate='true']").not("[type=button]");
        for (var i = 0; i < fieldsToValidate.length; i++) {
            var controlName = $(fieldsToValidate[i]).attr('name');
            var controlCode = $(fieldsToValidate[i]).attr('code');
            //fe_consolelog("TRYING.VALIDSTEP.control." + controlCode);

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
                valid = valid && controlResult;
                //fe_consolelog("VALIDATESTEP.control." + $(fieldsToValidate[i]).attr('code') + ".Result=" + controlResult);
            }

        }

        if (hasPendingValidations) {
            delayedValidation(callback);
        }
        else {
            fe_hideLoader();
            //fe_consolelog("VALIDATESTEP.ALL.valid=" + valid);
            callback(valid);
        }
    }


    function validateLastStepRequiredFields() {
        var valid = true;
        var step = fe_wiz_getLastStep();
        $(FormsEngine.DefaultFormTag).find("#Step" + step).find(":input[required]").not("[code='UserAgreement']").not("[code='EDDYUserAgreement']").each(function () {
            if ($(this).is(':radio') || $(this).is(':checkbox')) {
                var name = $(this).attr("name");
                valid &= ($(FormsEngine.DefaultFormTag).find("#Step" + step).find(":input[name='" + name + "']:checked").length > 0);
            }
            else {

                valid &= ($(this).val() != '');

            }
        });
        return valid;
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

        FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];

        //Additional Fields support
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
            $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).hide();
        }
        FormsEngine.CurrentStep += direction;
        //Special label on last step
        if (fe_isLastStep()) {
            //Helen added to show "Submit" button instead of "Continue" at last step
            if ($(FormsEngine.DefaultFormTag).find('.steps:visible').next(".steps").length == 0) {
                $(FormsEngine.SubmitButton).addClass("buttonLastStep");
            }

            if (!FormsEngine.SmartMatchZero) {
                $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextLast);
                if (FormsEngine.ShowContinueMobileButton) {
                    $(FormsEngine.DefaultFormTag).find("#screen-button span").html(FormsEngine.SubmitButtonLabelTextLast.toUpperCase() + " <i class='fa fa-arrow-circle-right form-submit-button-icon'></i>");
                }
            }
        }
        else {
            $(FormsEngine.SubmitButton).removeClass("buttonLastStep");
            $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextNormal);
            if (FormsEngine.ShowContinueMobileButton) {
                $(FormsEngine.DefaultFormTag).find("#screen-button span").html(FormsEngine.SubmitButtonLabelTextNormal.toUpperCase() + " <i class='fa fa-arrow-circle-right form-submit-button-icon'></i>");
            }
        }

        //Google analytics and internal tracking
        trackEvent();



        //navBar
        refreshNavbar();

        $(FormsEngine.DefaultFormTag).find("#ErrorBoxSection" + FormsEngine.CurrentStep).show();

        if (hideStep == true) {
            $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).show();
        }
    }

    function onAnimateStepIn(direction) {
        if (FormsEngine.CurrentStep > 1) {
            $(FormsEngine.BackButton).show();

            //show "Start Now" in header only for Step1
            $(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-header .start-now').css('display', 'none');

        }
        else {
            $(FormsEngine.BackButton).hide();
            $(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-header .start-now').css('display', 'block');

        }
        fe_hideLoader(false);
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
            if ($(FormsEngine.DefaultFormTag).find("#screen-button").length > 0) {
                checkValidMobileButton();
            }
        }

        if ($(FormsEngine.DefaultFormTag).find("#screen-button").length > 0) {
            fe_checkShowScreenButton();
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
            $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).fadeOut(100, function () {
                onAnimateStepOut(direction, false);
                $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).fadeIn(50, function () {
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
        var isEmpty = $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find(':input:visible').length == 0;
        //check that we are not on category, subcategory, or specialties step because those may still be loading
        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find("input[name='Categories_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find("input[name='SubCategories_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find("input[name='Specialties_Selections']").length > 0) {
            isEmpty = false;
        }

        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find("#school-picker-carousel").length > 0) {
            isEmpty = false;
        }

        if ($(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).find("#school-picker-failures").length > 0) {
            isEmpty = false;
        }

        //dont skip the dynamic step
        if (FormsEngine.CurrentStep == FormsEngine.StepDynamicQuestions) {
            isEmpty = false;
        }
        return isEmpty;
    }
    
    //Validate if last required question of last step to call ME.
    function checkLastStepRequired() {
        if (validateLastStepRequiredFields()) {
            if (FormsEngine.SMLeadsCreatedCount != undefined && FormsEngine.SMLeadsCreatedCount != null && FormsEngine.SMLeadsCreatedCount > 0) {
                fe_consolelog("SM Leads already created for this user, NOT getting SMs");
                return;
            }
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

    function checkNavigation(direction) {
        //prevent multiple firing while switching
        if (MovingStep) {
            fe_consolelog("Ignoring click, transitioning already");
            return;
        }

        //lock event
        MovingStep = true;

        var isLastStep = fe_isLastStep();

        //last (final) step
        if (direction > 0 && isLastStep) {
            validStep(function (result) {
                if (result) {
                    fe_consolelog('Submit form');
                    fe_showLoader();

                    //External Wizard submit
                    if ((!FormsEngine.IsMobile || FormsEngine.AllowLeaveBehindeOnMobile === true) && FormsEngine.StartLeaveBehindURL != undefined && FormsEngine.StartLeaveBehindURL != null && FormsEngine.StartLeaveBehindURL.length > 0) {
                        FormsEngine.PopupStartLeaveBehind = window.open('', '_blank');
                    }
                    FormsEngine.LoadWorkflowStep('QDF', getFormData());
                    MovingStep = false;
                    return;
                }
                else {
                    MovingStep = false;
                }

                fe_checkShowScreenButton();
            });
        }
        //If on second to last step then call get additional questions
        // previous of last step = DynamicQuestions step -- 2        
        else if ($(FormsEngine.DefaultFormTag).find("#Step" + (FormsEngine.CurrentStep + direction)).length > 0) {
            //backward or forward valid
            if (direction < 0) {
                moveStep(direction);
            }
            else {
                validStep(function (result) {
                    if (result) {
                        if (FormsEngine.CheckDependencies) {
                            //Fire bind on last control of this step
                            var control = $("[id='Step" + FormsEngine.CurrentStep + "']").find(":input:last");
                            FormsEngine.CheckDependencies(control, true);
                        }
                        moveStep(direction);
                        var LastStep = fe_wiz_getLastStep();
                        if (FormsEngine.CurrentStep + direction == LastStep ||
                            FormsEngine.CurrentStep == LastStep) {
                        }
                    }
                    else {
                        MovingStep = false;
                    }
                });
            }
        }
        else {
            MovingStep = false;
        }
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
       
    function RemoveScrollbar(category) {
        //business's subcategory does NOT show scrollbar            
        if ($(FormsEngine.DefaultFormTag).find('.interest-field.Categories li input:checkbox[checked="checked"]').length >= 2 || $(FormsEngine.DefaultFormTag).find('.interest-field.Categories li input:checkbox[checked="true"]').length >= 2) {
            $(FormsEngine.DefaultFormTag).find('.interest-field.SubCategories').removeClass('no-scroll');
        }
        else if ($(FormsEngine.DefaultFormTag).find('.interest-field.Categories li input:checkbox[checked="checked"]').length == 1 || $(FormsEngine.DefaultFormTag).find('.interest-field.Categories li input:checkbox[checked="true"]').length == 1) {
            category_chk = $(FormsEngine.DefaultFormTag).find('.interest-field.Categories li input:checkbox[checked="checked"]');
            //category not set, then as long as only one category checked, no scrollbar showes

            if (category == '' || typeof (category) === 'undefined') {
                fe_consolelog("add no-scroll class");
                $(FormsEngine.DefaultFormTag).find('.interest-field.SubCategories').addClass('no-scroll');
            }
            else {
                if ($(category_chk).next().text() == category) {
                    $(FormsEngine.DefaultFormTag).find('.interest-field.SubCategories').addClass('no-scroll');
                }
                else {
                    $(FormsEngine.DefaultFormTag).find('.interest-field.SubCategories').removeClass('no-scroll');
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
        $(FormsEngine.DefaultFormTag).find("#Step" + FormsEngine.CurrentStep).show();

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
        FormsEngine.StepDynamicQuestions = $(FormsEngine.DefaultFormTag).find("div[data-title='DynamicQuestions']").attr("data-step");

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
            fe_loadFormFromPassThru();

            formAdditionalLoad();

            //set the status of controls with values
            fe_setControlStatus();
            //bind status update events to all required controls
            fe_bindControlStatusEvent();

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
        
        //Google Tag Manager event
        fe_googleTagEvent('gaEvent', 'client', 'qdf-start', FormsEngine.TemplateId);

        fe_ApplyPhoneMask();

        checkValidMobileButton();

        $(FormsEngine.DefaultFormTag).find('.eddy-form-container').find('input, select').change(function () { checkValidMobileButton(); });

        //E-mail and opt in on the same step no auto advance from e-mail.
        var email = $(FormsEngine.DefaultFormTag).find(':input[code="Email"]');
        var optin = $(FormsEngine.DefaultFormTag).find(':input[code="NewsLetterOptIn"]');
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
                    $(FormsEngine.DefaultFormTag).find('#Step' + emailstep).click(function () { fe_wiz_AutoForwardStep(); });
                }

            }
        }


        if (typeof $().select2 == "function") {
            var $selectControls = $(FormsEngine.DefaultFormTag).find("select.typeahead.select2"); //this can return more than one so it should loop
            jQuery.each($selectControls, function () {
                var $selectControl = $(this);

                if ($selectControl.length > 0) {
                    // if typeahead and inline
                    if ($selectControl.hasClass("typeahead") && $selectControl.hasClass("inlineDropDown")) {
                        if ($selectControl.hasClass("searchablePredfinedValueList")) {
                            if ($selectControl.find("option[value='']").length > 0) {
                                $selectControl.select2({
                                    placeholder: $selectControl.attr("placeholder"),
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
                                    fe_consolelog(data);
                                    $(this).children('[value="' + data['id'] + '"]').attr(
                                        {
                                            'key': data["key"], //dynamic value from data array

                                        }
                                    );
                                }).val(0).trigger('change');
                            }
                            else {
                                $selectControl.select2({
                                    placeholder: $selectControl.attr("placeholder"),
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
                                    fe_consolelog(data);
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
                                    placeholder: $selectControl.attr("placeholder")
                                });
                            }
                            else if ($selectControl.siblings("label.inline-hidden-label").length > 0) {
                                $selectControl.select2({
                                    placeholder: $selectControl.attr("placeholder")
                                });
                            }
                            else {
                                $selectControl.select2({
                                    placeholder: $selectControl.attr("placeholder"),
                                    allowClear: true
                                });
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
                                    fe_consolelog(data);
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
                                    fe_consolelog(data);
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
                                $selectControl.select2({
                                    placeholder: $selectControl.attr("placeholder"),
                                    allowClear: true
                                });
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
                                    fe_consolelog(data);
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
                                    fe_consolelog(data);
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
                        $selectControl.select2({
                            placeholder: $selectControl.attr("placeholder"),
                            allowClear: true
                        });
                    }

                    if ($selectControl.hasClass('multiselectdropdown')) {
                        $selectControl.on('select2:unselect', function (e) {
                            //if we deselected from a multiselect dropdown we need to make sure the hidden inputs are updated properly
                            var hiddenInput = $("input[name='" + $selectControl.attr('code') + "_Selections']");
                            if (hiddenInput) {
                                hiddenInput.val($selectControl.val());
                                hiddenInput.change();
                            }
                        });
                    }
                }
            });
        }



        //hide the back button
        $(FormsEngine.BackButton).hide();
        $(FormsEngine.DefaultFormTag).find('.eddy-form-wizard-header .start-now').css('display', 'block');

        if (fe_isLastStep()) {
            $(FormsEngine.SubmitButton).addClass("buttonLastStep");

        }
        else {
            $(FormsEngine.SubmitButton).removeClass("buttonLastStep");
            $(FormsEngine.SubmitButtonLabel).text(FormsEngine.SubmitButtonLabelTextNormal);
        }

    }
    
    $(document).ready(function () {
        window.FormsEngine = window.FormsEngine || {};
        window.FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];
        $(FormsEngine).on("loadFormWizard", function () {
            fe_consolelog('load form qdf executed')
            start_loadFormWizard();
        });
    });
    

})(jQuery);