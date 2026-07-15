(function ($) {
    // ProgramTemplate.js
    //-------------------

    //Extension method for ControlBeforeMe based on sorting criteria
    jQuery.fn.isControlBefore = function (control) { if (!$(FormsEngine.DefaultFormTag).find(":input[code='" + control + "']").exists()) { return true; } else { return parseInt($(FormsEngine.DefaultFormTag).find(":input[code='" + control + "']").attr("id-sort")) > parseInt($(this).attr("id-sort")); } }

    //Sorts alphabetically a DDL a-z case insensitive, -- select on top
    jQuery.fn.sortDDL = function () {
        var listItems = $(this).children().get();
        listItems.sort(function (a, b) { if ($(a).text() == FormsEngine.DefaultSelectText) { return -1; } else if ($(b).text() == FormsEngine.DefaultSelectText) { return 1; } else { return $(a).text().toUpperCase().localeCompare($(b).text().toUpperCase()); } });
        $(this).append(listItems);
    }

    //Distinct elements in DDL by text
    jQuery.fn.distinctDDL = function () {
        var exists = {};
        $(this).children().each(function () {
            if (exists[$(this).text()]) {
                $(this).remove();
            }
            else {
                exists[$(this).text()] = $(this).val();
            }
        });
    }

    //Sorts and gets distinct elements in a DDL by text
    jQuery.fn.sortDistinctDDL = function () {
        $(this).distinctDDL();
        $(this).sortDDL();
    }

    //Constants
    FormsEngine.SessionCookieName = "FE_SessionId";
    FormsEngine.DefaultFormTag = "#eddynexusform";
    FormsEngine.DefaultCountryCode = "US";
    FormsEngine.DefaultValidationLevel = 3; //3: MX Record check --> Make sure DNS servers are reliable
    FormsEngine.DefaultSelectText = "- Select -";
    FormsEngine.GlobalErrorBoxExists = $("#ErrorBox").exists();
    FormsEngine.SectionErrorBoxExists = $('[id^="ErrorBoxSection"]').exists();
    FormsEngine.CrossSellDivTag = '#eddycrosssell';
    FormsEngine.CrossSellPixelsDivTag = '#dvPixelCode';
    FormsEngine.CrossSellErrorDivTag = '#dvCrossSellError';
    FormsEngine.CrossSellFormTag = '#crossSellForm';
    FormsEngine.CrossSellSubmitTag = 'div[name="cross-sell-submit"]';
    FormsEngine.ModalOverlayTag = '#FEmodalOverlayBG';
    FormsEngine.LoaderTag = '#FEloader';
    FormsEngine.Source = 'IS_FormsEngine_ProgramTemplate';

    //Internal variables
    var LastStateSelection = "";
    var LastCountrySelection = "";
    var LastProgramDependencies = "-";
    FormsEngine.LastCategory = "";
    FormsEngine.LastSubCategory = "";
    FormsEngine.LastSpecialty = "";
    var SubmittingForm = false;

    //Tries to read AffiliateId and SessionId from site cookies
    function setSettingsFromCookies() {
        FormsEngine.AffiliateId = FormsEngine.readCookie('_AffiliateID') != null ? FormsEngine.readCookie('_AffiliateID') : FormsEngine.AffiliateId;
        FormsEngine.SessionId = FormsEngine.readCookie('_Session') != null ? FormsEngine.readCookie('_Session') : FormsEngine.SessionId;
        FormsEngine.CookieZipCode = FormsEngine.readCookie('user_postal_code') != null ? FormsEngine.readCookie('user_postal_code') : '';
        FormsEngine.DeviceId = FormsEngine.readCookie('_Device') != null ? FormsEngine.readCookie('_Device') : FormsEngine.DeviceId;
    }


    //Gets querystring arguments by name case insensitive
    function getParameterByName(name) {
        match = new RegExp('[?&]' + name + '=([^&]*)', 'i').exec(window.location.search);
        return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
    }

    //Gets arguments from string by name case insensitive
    function getParameterByNameFromString(name, string) {
        match = new RegExp(name + '=([^&]*)', 'i').exec(string);
        return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
    }

    function getParameterByNameAndAlias(control, code) {
        var ControlValue = "";

        //Based on control code
        ControlValue = getParameterByName(code);

        //Based on aliases if supported
        if (ControlValue == undefined || ControlValue == "") {
            var alias = $(control).attr("alias");
            if (alias != undefined && alias != "") {
                var aliasList = alias.split(',');
                for (i = 0; i < aliasList.length; i++) {
                    ControlValue = getParameterByName(aliasList[i]);
                    if (ControlValue != undefined && ControlValue != "") {
                        return ControlValue;
                    }
                }
            }
        }
        return ControlValue;
    }


    //Sets values from querystring arguments based on code submission
    function setValuesFromQuerystring() {
        $(FormsEngine.DefaultFormTag).find(':input').each(function () {
            var ControlName = $(this).attr('code');
            if (ControlName != undefined && ControlName != "") {
                var ControlValue = getParameterByNameAndAlias(this, ControlName);
                if (ControlValue != undefined && ControlValue != "") {
                    if ($(this).is(':radio')) {
                        $(FormsEngine.DefaultFormTag).find("[type=radio][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                    }
                    else if ($(this).is(':checkbox')) {
                        $(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                    }
                    else {
                        ControlValue = fe_cleanStringOfRestrictedCharacters(ControlValue);
                        $(this).val(ControlValue);
                    }
                }

                FireDependancyEventsToRecoverForm(ControlName, this)
            }
        });
    }

    //Sets values from string arguments based on input name
    function SetValuesFromString(values) {
        $(FormsEngine.DefaultFormTag).find(':input').each(function () {
            var ControlName = $(this).attr('code');
            if (ControlName != undefined && ControlName != "") {
                var ControlValue = getParameterByNameFromString(ControlName, values);
                if (ControlValue != undefined && ControlValue != "") {
                    if ($(this).is(':radio')) {
                        $(FormsEngine.DefaultFormTag).find("[type=radio][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                    }
                    else if ($(this).is(':checkbox')) {
                        $(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                    }
                    else {
                        $(this).val(fe_cleanStringOfRestrictedCharacters(ControlValue));
                    }
                }

                FireDependancyEventsToRecoverForm(ControlName, this)
            }
        });
    }

    function FireDependancyEventsToRecoverForm(controlCode, control) {
        switch (controlCode) {
            case 'Postal_Code':
                getCityStateCountry($(control).val());
                break;
        }
    }

    //Serializes form and stores info in server session
    function saveForm() {
        var FormData = $(FormsEngine.DefaultFormTag).serialize();

        // Vantage mappings
        FormData += FormData + "&Categories_Selections=" + FormsEngine.LastCategory;
        FormData += FormData + "&SubCategories_Selections=" + FormsEngine.LastSubCategory;
        FormData += FormData + "&Specialties_Selections=" + FormsEngine.LastSpecialty;

        FormData = encodeURIComponent(FormData);
        fe_setSessionObject("TFORM", FormData, function () { });
        return false;
    }


    //Deserializes form from session to support multiple forms pre-load of data during user navigation experience
    function loadForm() {
        fe_setPlacementGuid();
        fe_getSessionObject("TFORM", function (data) {
            //Load Form Pass Thru
            if (!LoadFormFromPassThru()) {
                //Set values from querystring
                SetValuesFromString(data);
            }

            //Get programs
            getPrograms(function () { checkSwitchTemplate(); });
        });
    }

    function LoadFormFromPassThru() {
        var Data = "";
        var FirstQuestion = true;

        if (FormsEngine.PassThruItems != null && FormsEngine.PassThruItems.length > 0) {


            for (var i = 0; i < FormsEngine.PassThruItems.length; i++) {
                if (FirstQuestion == true) {
                    Data = Data + FormsEngine.PassThruItems[i].QuestionName + "=";
                    FirstQuestion = false;
                }
                else {
                    Data = Data + "&" + FormsEngine.PassThruItems[i].QuestionName + "=";
                }

                var Values = FormsEngine.PassThruItems[i].Answers.split(',');
                var FirstData = true;
                for (Value in Values) {
                    if (FirstData == true) {
                        Data = Data + encodeURIComponent(Values[Value]);
                        FirstData = false;
                    }
                    else {
                        Data = Data + "," + encodeURIComponent(Values[Value]);
                    }
                }

            }
            SetValuesFromString(Data);

            return true;
        }

        return false;
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

    jQuery.validator.addMethod(
        "profanity",
        function (value, element, param) {
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ProfanityCheck?Value=", CustomProfanityErrorMessage);
        },
        CustomProfanityErrorMessage
    );

    //jQuery Validation extension method to support Country check rule
    var CustomCountryErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "", "ERR_ValidCountry");
    }

    jQuery.validator.addMethod(
        "countryservercheck",
        function (value, element, param) {

            var countryField = $(FormsEngine.DefaultFormTag).find("select[code='Country']>option:selected")
            var countryId = $(countryField).attr('key');
            var countryCode = $(countryField).val();

            if (FormsEngine.ProgramProductId != "" && countryId != "" && countryId != undefined && countryCode != 'US') {
                var arguments = "?ProgramProductId=" + FormsEngine.ProgramProductId + "&IsBeta=" + FormsEngine.IsBeta + "&CountryId=" + countryId + "&CountryCode=";

                //Custom event when country validation fails
                if (FormsEngine.CountryValidationFailureEvent) {
                    element.CustomFailureFunction = FormsEngine.CountryValidationFailureEvent;
                }

                return jqueryValidatorRemoteJsonp(this, value, element, "/Matching/CountryCheck" + arguments, CustomCountryErrorMessage);
            }
            else {
                return true;
            }
        },
        CustomCountryErrorMessage
    );


    //jQuery Validation extension method to support phone check rule
    var CustomPhoneErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "value", "ERR_Valid");
    }

    jQuery.validator.addMethod(
        "phoneservercheck",
        function (value, element, param) {
            var countryCode = $("select[code='Country']").val();
            if (countryCode == "") {
                countryCode = FormsEngine.DefaultCountryCode;
            }
            var cleanPhone = value.replace(/\(/g, "");
            cleanPhone = cleanPhone.replace(/\)/g, "");
            cleanPhone = cleanPhone.replace(/_/g, "");
            cleanPhone = cleanPhone.replace(/-/g, "");
            cleanPhone = cleanPhone.replace(/ /g, "");
            if (cleanPhone == "") {
                return true;
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

        var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
        return getErrorMessage(element, "value", "ERR_ZipCountryState").replace('{ZipValue}', postalCode);

    }

    jQuery.validator.addMethod(
        "zipcitycountrycheck",
        function (value, element, param) {
            var countryCode = $(FormsEngine.DefaultFormTag).find("select[code='Country']").val();
            var stateCode = $(FormsEngine.DefaultFormTag).find("select[code='State']").val();
            var postalCode = $(FormsEngine.DefaultFormTag).find(":input[code='Postal_Code']").val();
            if (countryCode == "" || stateCode == "" || postalCode == "") {
                return true;
            }
            else {
                value = countryCode + "_" + stateCode + "_" + postalCode;
                return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/ZipCodeStateCountryCheck/?CountryCode=" + countryCode + "&StateCode=" + stateCode + "&ZipCode=" + postalCode + "&n=", CustomZipErrorMessage);
            }
        }
        ,
        CustomZipErrorMessage
    );

    //jQuery Validation extension method to support email validation rule
    var CustomEmailErrorMessage = function (value, element, param) {
        return getErrorMessage(element, "", "ERR_Email");
    }
    jQuery.validator.addMethod(
        "emailservercheck",
        function (value, element, param) {
            return jqueryValidatorRemoteJsonp(this, value, element, "/FormValidation/EmailCheck/?EmailVerificationLevel=" + FormsEngine.DefaultValidationLevel + "&EmailAddress=", CustomEmailErrorMessage);
        },
        CustomEmailErrorMessage
    );


    //Required field, minlength, maxlength custom messages
    jQuery.extend(jQuery.validator.messages, {
        required: function (value, element, param) {
            var message = "";
            if ($(element).attr('code') === 'UserAgreement') {
                /*message = "You must indicate that you have read and acknowledge our terms and conditions before submitting this form";*/
                message = "You must consent to be contacted to submit this form.";
            }
            else if (element.nodeName.toLowerCase() === 'select') {
                message = getErrorMessage(element, "field", "ERR_RequiredField");
            }
            else {
                message = getErrorMessage(element, "This", "ERR_RequiredField");
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

    //GetTemplate
    function switchTemplate(programProductId, templateId, productId, paidStatusTypeId) {
        FormsEngine.ProgramProductId = parseInt(programProductId);
        FormsEngine.ProductId = parseInt(productId);
        var alternativeTemplateId = fe_getAlternativeTemplateId(templateId);

        if (alternativeTemplateId == FormsEngine.TemplateId) {
            fe_consolelog('Aborting switch, alternative same as current');
            return;
        }
        else if (templateId == alternativeTemplateId) {
            fe_consolelog('switching to TemplateId =' + templateId);
        }
        else {
            fe_consolelog('switching to alternative=' + alternativeTemplateId + ' requested TemplateId is =' + templateId);
        }

        showLoader(false);


        var serviceArgs = "?RenderingStrategy=" + FormsEngine.RenderingStrategy;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&ProgramProductId=" + FormsEngine.ProgramProductId;
        serviceArgs += "&ProgramId=" + FormsEngine.ProgramId;
        serviceArgs += "&TemplateId=" + templateId;
        serviceArgs += "&InstitutionId=" + FormsEngine.InstitutionId;
        serviceArgs += "&AlternativeTemplates=" + fe_serializeAlternativeTemplates();
        serviceArgs += "&IgnoreTemplateCache=" + FormsEngine.IgnoreTemplateCache;
        serviceArgs += "&Theme=" + FormsEngine.Theme;
        serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
        serviceArgs += "&TrackId=" + FormsEngine.TrackId;
        serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;
        serviceArgs += "&ProductId=" + productId;
        serviceArgs += "&PaidStatusTypeId=" + paidStatusTypeId;

        $.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ClientServiceURL + serviceArgs,
            success: function (data) {
                //Saves fields
                saveForm();
                //Saves programs to avoid re-binding
                var CurrentPrograms = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").html();
                //Saves states to avoid re-binding
                var CurrentStates = $(FormsEngine.DefaultFormTag).find(":input[code='State']").html();
                //switch html
                $('#' + FormsEngine.RenderingDiv).html(decodeURIComponent((data.Template + '').replace(/\+/g, '%20')));

                //Restore programs
                $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").html(CurrentPrograms);

                //Restore Campus Preference
                if (FormsEngine.CampusPreferenceVisible == true) {
                    fe_showCampusPreference();
                }

                //Restore States
                $(FormsEngine.DefaultFormTag).find(":input[code='State']").html(CurrentStates);

                //Restore fields
                loadForm();

                //Sets current template Id
                FormsEngine.TemplateId = data.TemplateId;

                //Sets validations and events
                configureDefaults();

                hideLoader();
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                hideLoader();
            }
        });
    }


    //forces the template to be refreshed, just for admin purposes.
    function refreshTemplate() {


        var serviceArgs = "?RenderingStrategy=" + FormsEngine.RenderingStrategy;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&ProgramProductId=" + FormsEngine.ProgramProductId;
        serviceArgs += "&TemplateId=" + FormsEngine.TemplateId;
        serviceArgs += "&InstitutionId=" + FormsEngine.InstitutionId;
        serviceArgs += "&AlternativeTemplates=" + fe_serializeAlternativeTemplates();
        serviceArgs += "&IgnoreTemplateCache=" + FormsEngine.IgnoreTemplateCache;
        serviceArgs += "&Theme=" + FormsEngine.Theme;
        serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
        serviceArgs += "&TrackId=" + FormsEngine.TrackId;
        serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;

        $.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ClientServiceURL + serviceArgs,
            success: function (data) {
                //switch html
                $('link[id=BaseCSS]').remove();
                if (FormsEngine.Theme == undefined || FormsEngine.Theme == "") {
                    FormsEngine.Theme = "default";
                }
                $('head').append('<link rel="stylesheet" id="BaseCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetBundledWizardCSS?basePath=' + CSSBasePath + '&theme=' + FormsEngine.Theme + '" type="text/css" />');
                $('#' + FormsEngine.RenderingDiv).html(decodeURIComponent((data.Template + '').replace(/\+/g, '%20')));

                //Sets current template Id
                FormsEngine.TemplateId = data.TemplateId;

                //Sets validations and events
                configureDefaults();

                //Get programs
                getPrograms(function () { checkSwitchTemplate(); });
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    }

    //Get list of states by country
    function getCountryStates(doneFunction) {
        var CountryCode = $(FormsEngine.DefaultFormTag).find('select[code="Country"] option:selected').val();

        if (CountryCode == "") {
            CountryCode = FormsEngine.DefaultCountryCode;
        }

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/FormValidation/GetStatesByCountry?Countrycode=" + CountryCode,
            success: function (data) {
                var states = $(FormsEngine.DefaultFormTag).find('select[code="State"]');
                var statevalue = $(states).val();
                states.empty();
                states.append(jQuery('<option/>', {
                    value: "",
                    text: FormsEngine.DefaultSelectText
                }));
                if (data.length > 0) {
                    jQuery.each(data, function (index, item) {
                        states.append(
                            jQuery('<option/>', {
                                'value': item.Item2,
                                'text': item.Item3,
                                'key': item.Item1
                            })
                        );
                    });
                    $(states).val(statevalue);
                }
                else {
                    states.append(jQuery('<option/>', {
                        value: "N/A",
                        text: "Not Applicable"
                    }));
                }
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        }).done(doneFunction);
    }


    function getCityStateCountry(ZipCode) {
        if (ZipCode != undefined && ZipCode != "") {
            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/FormValidation/GetCityStateCountry?ZipCode=" + ZipCode,
                success: function (data) {

                    var CountryDDL = $(FormsEngine.DefaultFormTag).find("select[code='Country']");
                    var StateDDL = $(FormsEngine.DefaultFormTag).find("select[code='State']");
                    var CityInput = $(FormsEngine.DefaultFormTag).find("input[code='City']");

                    if (data.length > 0) {
                        //Country data[3] CountryCode
                        LastStateSelection = data[1].Value;
                        LastCountrySelection = data[3].Value;


                        if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('Country')) {
                            if ($(CountryDDL).val() != data[3].Value) {
                                $(CountryDDL).val(data[3].Value);
                                if ($(CountryDDL).val() == data[3].Value) {
                                    $(CountryDDL).valid();
                                }
                            }
                        }

                        if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('State')) {
                            getCountryStates(function () {
                                //Set State
                                var StateDDL = $(FormsEngine.DefaultFormTag).find("select[code='State']");
                                var CountryDDL = $(FormsEngine.DefaultFormTag).find("select[code='Country']");
                                $(StateDDL).val(LastStateSelection);

                                //Revalidate State control
                                if ($(StateDDL).val() == LastStateSelection) {
                                    if ($(StateDDL).valid()) {
                                        $(StateDDL).removeClass('error');
                                    }
                                }

                                //Revalidate Country control
                                if ($(CountryDDL).val() == LastCountrySelection) {
                                    if ($(CountryDDL).valid()) {
                                        $(CountryDDL).removeClass('error');
                                    }
                                }
                            });
                        }

                        //set City
                        if ($(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('City')) {
                            $(CityInput).val(data[0].Value);
                            if ($(CityInput).val() == data[0].Value) {
                                $(CityInput).valid();
                            }
                        }

                    } else {
                        StateDDL.empty();
                        StateDDL.append(jQuery('<option/>', { value: "", text: FormsEngine.DefaultSelectText }));
                        StateDDL.append(jQuery('<option/>', { value: "N/A", text: "Not Applicable" }));
                        CountryDDL.val("");
                    }

                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                }
            });
        }
    }

    //Gets the previous dependant values (if sort index < element sort index)
    function getDependantFieldValues(field, fieldList) {
        var result = [];
        var fieldSort = parseInt($(FormsEngine.DefaultFormTag).find(":input[code='" + field + "']").attr("id-sort"));


        for (var index = 0; index < fieldList.length; index++) {
            var element = fieldList[index];
            var needsKey = element.indexOf("-key") >= 0;
            var elementId = needsKey ? element.replace("-key", "") : element;

            if ($(FormsEngine.DefaultFormTag).find(":input[code='" + elementId + "']").exists()) {
                var dependantField = $(FormsEngine.DefaultFormTag).find(":input[code='" + elementId + "']");
                var value = "";
                if (parseInt($(dependantField).attr("id-sort")) < fieldSort) {

                    if ($(dependantField).is('select')) {
                        if (!needsKey) {
                            value = $(dependantField).val();
                        }
                        else {
                            value = $(FormsEngine.DefaultFormTag).find(":input[code='" + elementId + "']>option:selected").attr('key')
                        }
                    }
                    else if ($(dependantField).is(':radio')) {
                        if (!needsKey) {
                            value = $(FormsEngine.DefaultFormTag).find(":input[code='" + elementId + "']:checked").val();
                        }
                        else {
                            value = $(FormsEngine.DefaultFormTag).find(":input[code='" + elementId + "']:checked").attr('key');
                        }
                    }
                    else {
                        if (!needsKey) {
                            value = $(dependantField).val();
                        }
                        else {
                            value = $(dependantField).attr('key');
                        }

                    }
                    value = value == null || value == undefined ? "" : value;
                }
                if (value != "") {
                    result.push(element.toString() + "=" + value);
                }
            }
        }

        return result.join('&');
    }


    //Get form programs (Form and Campus location)
    function getFormPrograms(dependencies, callback) {
        setSettingsFromCookies();
        var arguments = "InstitutionId=" + FormsEngine.InstitutionId + "&IsBeta=" + FormsEngine.IsBeta + "&TrackId=" + FormsEngine.TrackId;
        arguments += "&FormFilterValues=" + encodeURIComponent(dependencies);
        arguments += "&DeviceId=" + FormsEngine.DeviceId;
        arguments += "&ApplicationId=" + FormsEngine.ApplicationId;
        arguments += "&ProgramId=" + ((FormsEngine.ProgramId > 0) ? FormsEngine.ProgramId : "");
        arguments += "&AlternativeTemplates=" + fe_serializeAlternativeTemplates();

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/Matching/GetFormPrograms?" + arguments,
            success: function (data) {
                callback(data);
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    }

    //Gets the list of programs for the current institution dependant on filters
    function getPrograms(callback) {
        var dependantFields = [];
        $(FormsEngine.DefaultFormTag).find(':input[me-filter="true"]').each(function () {
            if ($.inArray($(this).attr('code') + '-key', dependantFields) == -1) {
                dependantFields.push($(this).attr('code') + '-key');
            }
        });

        dependantFields.push('CampusPreference', 'Age', 'Country-key', 'Highest_Level_of_Education_Completed', 'GPA-key', 'Year_of_Highest_Education_Completed', 'us_citizen', 'Military_Affiliation', 'Postal_Code', 'State-key', 'Years_of_Teaching_Experience-key', 'Years_of_Work_Experience-key');
        var programDependencies = getDependantFieldValues('Program_Of_Interest', dependantFields);


        //Any dependency changed, refresh
        if (programDependencies != LastProgramDependencies) {

            LastProgramDependencies = programDependencies;
            fe_consolelog("getPrograms.getFormPrograms(" + programDependencies + ")");
            getFormPrograms(programDependencies, function (data) {
                var ProgramsDDL = $(FormsEngine.DefaultFormTag).find('select[code="Program_Of_Interest"]');
                var lastProgram = $(ProgramsDDL).find(':selected').text();
                ProgramsDDL.empty();
                $(ProgramsDDL).append(decodeURIComponent(data).replace(/\+/g, ' '));

                // Insert Label into first option for inline-ddl..
                if ($(ProgramsDDL).hasClass("inlineDropDown")) {
                    var $label = $(ProgramsDDL).siblings("label[for=" + $(ProgramsDDL).prop("id") + "]");
                    $(ProgramsDDL).find("option").eq(0).text($label.text());
                }

                addDummyProgramForPreview();
                if (lastProgram && lastProgram.length > 0 && lastProgram.toLowerCase().indexOf('select') == -1) {
                    $(ProgramsDDL).find("option").each(function () {
                        if ($(this).text() == lastProgram) {
                            $(this).attr('selected', 'selected');
                            if ($(this).attr('data-showtwoucheckbox') == "True") {
                                fe_showUserAgreementForTwoULeadShareSchool(FormsEngine.InstitutionName, $(this).attr('data-programtype'), '.UserAgreement');
                            }
                            else {
                                fe_removeUserAgreementForTwoULeadShareSchool();
                            }
                        }
                    });
                }
                else if (FormsEngine.ProgramId && FormsEngine.ProgramId > 0) {
                    $(ProgramsDDL).val(FormsEngine.ProgramId);
                }

                if (FormsEngine.CampusPreferenceProcessed != true) {
                    fe_processSupportedMatchPreference();
                }
                getProgramDetail();

                if (callback) { callback() };
            });
        }
        else {
            if (callback) { callback() };
        }
    }


    function checkGSTemplate() {
        //GS / SAB / UAB
        if (FormsEngine.ApplicationId == 7 || FormsEngine.ApplicationId == 20 || FormsEngine.ApplicationId == 1) {
            var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
            var ProductId = $(ProgramDDL).attr('data-productid');
            var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');

            if (FormsEngine.ApplicationId == 7) {
                if (ProductId == '17' || ProductId == '19' || ProductId == undefined || ProductId == "") {
                    $(FormsEngine.DefaultFormTag).addClass('GradSchoolTemplate');
                }
                else {
                    $(FormsEngine.DefaultFormTag).removeClass('GradSchoolTemplate');
                }
            }

            var userAgreementControl = $(FormsEngine.DefaultFormTag).find("input[code='UserAgreement']");

            //Free
            if (PaidStatusTypeId == 1 || PaidStatusTypeId == 2) {
                if ($("#GSUserAgreementFree").length == 0 && FormsEngine.ResourceData && FormsEngine.ResourceData['GRADSCHOOL.FREE.ADDITIONALDISCLAIMER']) {
                    var AgreementHtml = $('label[for=' + $(userAgreementControl).attr('id') + ']').html();
                    $('label[for=' + $(userAgreementControl).attr('id') + ']').html(AgreementHtml + '<span id="GSUserAgreementFree"><br/>' + FormsEngine.ResourceData['GRADSCHOOL.FREE.ADDITIONALDISCLAIMER'] + '</span>');
                }
            }//Paid
            else if ($("#GSUserAgreementFree").length > 0) {
                $("#GSUserAgreementFree").remove();
            }
        }

    }

    function checkSwitchTemplate() {
        if (FormsEngine.IsForAdminPreview) {
            return false;
        }

        var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
        var ProgramId = $(ProgramDDL).val();
        ProgramId = ProgramId == undefined ? "" : ProgramId;

        checkGSTemplate();

        if (ProgramId != "") {
            var ProgramProductId = $(ProgramDDL).attr('data-programproductid');
            var ProgramTemplateId = $(ProgramDDL).attr('data-templateid');
            var ProductId = $(ProgramDDL).attr('data-productid');
            var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');
            FormsEngine.HideSchoolFromTCPA = $(ProgramDDL).attr('data-hideschoolfromtcpa') == 'true';

            FormsEngine.ProgramId = ProgramId;
            FormsEngine.ProgramProductId = ProgramProductId;
            FormsEngine.ProductId = ProductId;
            FormsEngine.ProgramName = $(ProgramDDL).text();

            //best campus found for the program and template is different --> Switch template
            if (ProgramTemplateId != "" && ProgramTemplateId != FormsEngine.TemplateId) {
                fe_consolelog("switching template");
                switchTemplate(ProgramProductId, ProgramTemplateId, ProductId, PaidStatusTypeId);
                //add the mask and remove the datatype attribute from the control to remove regex validation
                fe_ApplyPhoneMask();
                checkTwoULeadShareForProgram();
            }
            else {
                //calls IS Tracking to insert program product of all changes to program dropdown
                trackProgramSelection();
                if (FormsEngine.HideSchoolFromTCPA) {
                    updateUserAgreementText();
                }
                checkTwoULeadShareForProgram();
            }

        }
        else {
            FormsEngine.ProgramProductId = "";
        }

    }


    //********----#57653---*********//
    function updateSelectionCategorySubCategorySpecialty(data) {

        if (data.ProgramDetails.CategoryList != undefined && data.ProgramDetails.CategoryList != null && data.ProgramDetails.CategoryList.length > 0) {
            var itemCategory = { "id": data.ProgramDetails.CategoryList[0].ItemId, "text": data.ProgramDetails.CategoryList[0].ItemValue, "operation": true, "type": "Categories" };
            FormsEngine.LastCategory = data.ProgramDetails.CategoryList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemCategory, true);
        }

        if (data.ProgramDetails.SubjectList != undefined && data.ProgramDetails.SubjectList != null && data.ProgramDetails.SubjectList.length > 0) {
            var itemSubCategory = { "id": data.ProgramDetails.SubjectList[0].ItemId, "text": data.ProgramDetails.SubjectList[0].ItemValue, "operation": true, "type": "SubCategories" };
            FormsEngine.LastSubCategory = data.ProgramDetails.SubjectList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemSubCategory, true);
        }

        if (data.ProgramDetails.SpecialtyList != undefined && data.ProgramDetails.SpecialtyList != null && data.ProgramDetails.SpecialtyList.length > 0) {
            var itemSpecialty = { "id": data.ProgramDetails.SpecialtyList[0].ItemId, "text": data.ProgramDetails.SpecialtyList[0].ItemValue, "operation": true, "type": "Specialties" };
            FormsEngine.LastSpecialty = data.ProgramDetails.SpecialtyList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemSpecialty, true);
        }
    }
    //********END -- #57653*********//

    //Gets program detail information if required
    function getProgramDetail() {
        if (FormsEngine.ProgramChangedEvent) {
            var selectedProgram = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");

            if ($(selectedProgram).val() != "") {
                //get program info
                var programId = $(selectedProgram).val();

                var arguments = "?ProgramId=" + programId + "&IsBeta=" + FormsEngine.IsBeta + "&TrackId=" + FormsEngine.TrackId;
                $.ajax({
                    async: true,
                    type: 'GET',
                    dataType: 'jsonp',
                    cache: false,
                    url: FormsEngine.ServiceBaseURL + "/Matching/GetProgramDetail" + arguments,
                    success: function (data) {
                        FormsEngine.ProgramChangedEvent(data);


                        //********----#57653---*********//
                        if (data != undefined && data != null && data.ProgramDetails != undefined && data.ProgramDetails) {
                            updateSelectionCategorySubCategorySpecialty(data);
                        }
                        //********END -- #57653*********//
                    },
                    error: function (request, error) {
                        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    }
                });
            }
        }
    }

    function trackProgramSelection() {
        var programProductId = FormsEngine.ProgramProductId;

        if (programProductId) {
            if (typeof _etq != 'undefined') {
                _etq.push(['_etEvent', 'programProductId', programProductId, 'form-programTemplate']);
            }
        }
    }

    function submitProspect() {
        var LeadDataArray = $(FormsEngine.DefaultFormTag).serializeArray();
        var LeadData = "";
        var LeadAdditionalData = "";
        var preAmp = "";
        var preAmpAddtl = "";

        jQuery.each(LeadDataArray, function (index, item) {
            preAmp = LeadData == "" ? "" : "&";
            preAmpAddtl = LeadAdditionalData == "" ? "" : "&";
            var field = $(FormsEngine.DefaultFormTag).find(":input[name='" + LeadDataArray[index].name + "']");
            var code = $(field).attr("code");
            LeadData = LeadData + preAmp + code + "=" + LeadDataArray[index].value.replace(/&/g, "amp;");
            var value = "";
            if ($(field).is('select')) {
                value = $(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']>option:selected").attr('key')
            }
            else if ($(field).is(':radio')) {
                value = $(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']:checked").attr('key');
            }
            else {
                value = $(field).attr('key');
            }
            if (value != undefined && value != "") {
                LeadAdditionalData = LeadAdditionalData + preAmpAddtl + code + "-key=" + value;
            }

        });

        //SessionId AffiliateId from cookie
        setSettingsFromCookies();
        fe_getSessionId(function () {
            var Request = "IsBeta=" + FormsEngine.IsBeta;
            Request += "&SessionId=" + FormsEngine.SessionId;
            Request += "&LeadData=" + encodeURIComponent(LeadData);
            Request += "&AdditionalData=" + encodeURIComponent(LeadAdditionalData);
            Request += "&TrackId=" + FormsEngine.TrackId;
            Request += "&FESessionId=" + FormsEngine.FESessionId;
            Request += "&ApplicationId=" + FormsEngine.ApplicationId;

            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/TemplateManager/SaveProspect?" + Request,
                success: function (data) {
                    FormsEngine.ProspectId = data;
                    FormsEngine.createCookie("FE_ProspectId", data);
                    fe_consolelog('Prospect save complete. ProspectId=' + data);
                    fe_submitProspectAdditionalInfo(true);
                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                }
            });
        });
    }

    function submitForm() {

        if (SubmittingForm) {
            fe_consolelog("Ignoring click, submitting form already");
            return;
        }

        SubmittingForm = true;

        fe_submitProspectAdditionalInfo(false);

        var LeadDataArray = $(FormsEngine.DefaultFormTag).serializeArray();
        var LeadData = "";
        var LeadAdditionalData = "";
        var preAmp = "";
        var preAmpAddtl = "";

        FormsEngine.UserFullName = $(FormsEngine.DefaultFormTag).find(':input[name="First_Name"]').val() + " " + $(FormsEngine.DefaultFormTag).find(':input[name="Last_Name"]').val();

        jQuery.each(LeadDataArray, function (index, item) {
            preAmp = LeadData == "" ? "" : "&";
            preAmpAddtl = LeadAdditionalData == "" ? "" : "&";
            var field = $(FormsEngine.DefaultFormTag).find(":input[name='" + LeadDataArray[index].name + "']");
            var code = $(field).attr("code");
            LeadData = LeadData + preAmp + code + "=" + LeadDataArray[index].value.replace(/&/g, "amp;");
            var value = "";
            if ($(field).is('select')) {
                value = $(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']>option:selected").attr('key')
            }
            else if ($(field).is(':radio')) {
                value = $(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']:checked").attr('key');
            }
            else {
                value = $(field).attr('key');
            }
            if (value != undefined && value != "") {
                LeadAdditionalData = LeadAdditionalData + preAmpAddtl + code + "-key=" + value;
            }
        });
        LeadData += "&AffiliateId=" + FormsEngine.AffiliateId;

        if (!FormsEngine.AdditionalFields) {
            FormsEngine.AdditionalFields = [];
        }

        if (FormsEngine.AdditionalFields) {
            FormsEngine.AdditionalFields.push(["ExpressConsentOn", (FormsEngine.ExpressConsentOn != undefined && FormsEngine.ExpressConsentOn != null) ? FormsEngine.ExpressConsentOn : true]);
            jQuery.each(FormsEngine.AdditionalFields, function (index, item) {
                LeadData = LeadData + "&" + FormsEngine.AdditionalFields[index][0] + "=" + FormsEngine.AdditionalFields[index][1];
            });
        }
        LeadAdditionalData += "&Theme=" + FormsEngine.Theme;

        // add a global variable to use here and later on Cross Sell Submissions
        FormsEngine.LeadDataEncoded = encodeURIComponent(LeadData);
        FormsEngine.LeadAdditionalDataEncoded = encodeURIComponent(LeadAdditionalData);

        var programName = '';
        var templateId = FormsEngine.TemplateID;

        //If I'm in an alternative must submit using the original TemplateId
        if (FormsEngine.AlternativeTemplates) {
            for (var i = 0; i < FormsEngine.AlternativeTemplates.length; i++) {
                if (FormsEngine.AlternativeTemplates[i].AlternativeId == FormsEngine.TemplateID) {
                    templateId = FormsEngine.AlternativeTemplates[i].TemplateId;
                }
                break;
            }
        }

        //SessionId AffiliateId from cookie
        setSettingsFromCookies();

        var leadRequest = "";

        fe_getSessionId(function () {
            leadRequest += "TemplateId=" + templateId;
            leadRequest += "&ProgramProductId=" + FormsEngine.ProgramProductId;
            leadRequest += "&ProductId=" + FormsEngine.ProductId;
            leadRequest += "&IsBeta=" + FormsEngine.IsBeta;
            leadRequest += "&TrackId=" + FormsEngine.TrackId;
            leadRequest += "&SessionId=" + FormsEngine.SessionId;
            leadRequest += "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
            leadRequest += "&InstitutionId=" + FormsEngine.InstitutionId;
            leadRequest += "&InstitutionName=" + FormsEngine.InstitutionName;
            leadRequest += "&ProgramName=" + FormsEngine.ProgramName;
            leadRequest += "&MatchGuid=" + FormsEngine.MatchResponseGuid;
            leadRequest += "&LeadData=" + FormsEngine.LeadDataEncoded;
            leadRequest += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
            leadRequest += "&ProspectId=" + FormsEngine.ProspectId;
            leadRequest += "&FESessionId=" + FormsEngine.FESessionId;
            leadRequest += "&DeviceId=" + FormsEngine.DeviceId;
            leadRequest += "&ApplicationId=" + FormsEngine.ApplicationId;
            leadRequest += "&AlternativeTemplates=" + fe_serializeAlternativeTemplates();

            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/TemplateManager/ProcessSubmit?" + leadRequest,
                success: function (data) {
                    ProcessFormSubmitResponse(data);
                    SubmittingForm = false;
                },
                error: function (request, error) {
                    hideLoader(false);
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    SubmittingForm = false;
                }
            });
        });
    }

    function ProcessFormSubmitResponse(data) {

        if (data != undefined && data != null) {
            FormsEngine.InitialMatchWasValid = data.InitialMatchWasValid;
            FormsEngine.WasLeadCreated = data.WasLeadCreated;
            FormsEngine.InitialLeadId = data.InitialLeadId;
            FormsEngine.UID = data.UID;
            FormsEngine.RawPostDataId = data.RawPostDataId;
            FormsEngine.IsTestLead = data.IsTestLead;
            FormsEngine.CrossSellThankYouMessage = data.CrossSellResult.CrossSellThankYouMessage;

            // in case ProspectId was not saved and returned correctly from client side async ajax calls, set it based on server side sync save
            if (FormsEngine.ProspectId == null || FormsEngine.ProspectId < 1) {
                FormsEngine.ProspectId = data.ProspectId;
                FormsEngine.createCookie("FE_ProspectId", data.ProspectId);
                fe_consolelog('ProspectId retreived and set from Server side sync save. ProspectId=' + data.ProspectId);
            }
        }

        //GTM Lead Submit track event
        trackGTMLeadSubmit(FormsEngine.InitialMatchWasValid);

        if (data != undefined && data != null && data.CrossSellResult.Success && data.CrossSellResult.RenderedCrossSell != null) {

            FormsEngine.MaxCrossSellUserSelections = data.CrossSellResult.MaxCrossSellUserSelections;
            FormsEngine.CrossSellProgramCount = data.CrossSellResult.CrossSellProgramCount;


            // save data in session  
            var SessionValues = new Object();
            SessionValues.InitialUID = FormsEngine.UID;
            SessionValues.IsCrossSell = false;
            SessionValues.IsAnyLeadValid = FormsEngine.InitialMatchWasValid;
            SessionValues.IsTestLead = FormsEngine.IsTestLead;
            SessionValues.UserFullName = FormsEngine.UserFullName;
            SessionValues.CrossSellThanksYouMessage = FormsEngine.CrossSellThankYouMessage;
            var SessionValuesEncoded = $.toJSON(SessionValues);

            fe_setSessionObject("FE_Response", SessionValuesEncoded, function () {
                if ($(FormsEngine.CrossSellDivTag).exists()) {
                    //hide form on mobile
                    //hide form on mobile
                    if (FormsEngine.RenderingStrategy.toLowerCase() == 'original') {
                        if (FormsEngine.Theme.toLowerCase() == 'default' || FormsEngine.Theme.toLowerCase() == 'ecbluegray') {

                            if ($(window).width() <= 768) {
                                $(FormsEngine.DefaultFormTag).addClass("eddynexusformHide");
                            }
                        }
                    }

                    // populate the cross sell div

                    $(FormsEngine.CrossSellDivTag).html(decodeURIComponent((data.CrossSellResult.RenderedCrossSell + '').replace(/\+/g, '%20')));

                    // pixel injection
                    fe_loadInitialPixels();

                    // Google Analytics pixel fire
                    try {
                        if (_gaq != undefined) {
                            _gaq.push(['_trackPageview', 'cross-sell']);
                        }
                    } catch (e) { }
                }
            });

        } else {  //If no data/programs comes back for the cross sell, redirect to the Thank You page and pass along some data...

            var SessionValues = new Object();
            SessionValues.InitialUID = FormsEngine.UID;
            SessionValues.IsCrossSell = false;
            SessionValues.IsAnyLeadValid = (FormsEngine.InitialMatchWasValid != undefined && FormsEngine.InitialMatchWasValid != null) ? FormsEngine.InitialMatchWasValid : false;
            SessionValues.IsTestLead = FormsEngine.IsTestLead;
            SessionValues.UserFullName = FormsEngine.UserFullName;
            SessionValues.CrossSellThanksYouMessage = FormsEngine.CrossSellThankYouMessage;
            var SessionValuesEncoded = $.toJSON(SessionValues);

            fe_setSessionObject("FE_Response", SessionValuesEncoded, function () {
                window.location = fe_GetProgramTemplateThankYouPageUrl();
            });

        }

        if (FormsEngine.OnFormSubmissionResponse) {
            FormsEngine.OnFormSubmissionResponse();
        }
    }

    function CheckForOptimizelyCrossSellFlag() {
        var showCrossSellPopupOnly = getParameterByName('scpo');
        if ((showCrossSellPopupOnly != null && showCrossSellPopupOnly != '' && parseInt(showCrossSellPopupOnly) == '1')
            || (FormsEngine.OptimizelyCrossSellOn != undefined && FormsEngine.OptimizelyCrossSellOn == true)) {

            showLoader();
            var leadRequest = "TemplateId=" + FormsEngine.TemplateID + "&IsBeta=" + FormsEngine.IsBeta + "&RenderingStrategy=" + FormsEngine.RenderingStrategy;
            $.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: FormsEngine.ServiceBaseURL + "/TemplateManager/GetOptimizelyCrossSell?" + leadRequest,
                success: function (data) {
                    if (data != undefined && data != null && data.CrossSellResult.Success) {

                        FormsEngine.MaxCrossSellUserSelections = data.CrossSellResult.MaxCrossSellUserSelections;
                        FormsEngine.InitialMatchWasValid = data.InitialMatchWasValid;
                        FormsEngine.CrossSellProgramCount = data.CrossSellResult.CrossSellProgramCount;

                        FormsEngine.WasLeadCreated = data.WasLeadCreated;
                        FormsEngine.InitialLeadId = data.InitialLeadId;
                        FormsEngine.UID = data.UID;
                        FormsEngine.RawPostDataId = data.RawPostDataId;
                        FormsEngine.IsTestLead = data.IsTestLead;
                        FormsEngine.IsForOptimizelyCrossSell = true;

                        if ($(FormsEngine.CrossSellDivTag).exists()) {
                            //hide form on mobile
                            if (FormsEngine.RenderingStrategy.toLowerCase() == 'original') {
                                if (FormsEngine.Theme.toLowerCase() == 'default' || FormsEngine.Theme.toLowerCase() == 'ecbluegray') {
                                    if ($(window).width() <= 768) {
                                        $(FormsEngine.DefaultFormTag).addClass("eddynexusformHide");
                                    }
                                }
                            }
                            // populate the cross sell div
                            $(FormsEngine.CrossSellDivTag).html(decodeURIComponent((data.CrossSellResult.RenderedCrossSell + '').replace(/\+/g, '%20')));
                        }
                    }
                    hideLoader(true);
                },
                error: function (request, error) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    hideLoader(false);
                }
            });
        }
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
        fe_consolelog('VALIDATOR: element' + element.name);
        if (previous.old === value && element.name != 'Postal_Code') {
            return previous.valid;
        }

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
                previous.valid = valid;
                validator.stopRequest(element, valid);
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
        return "pending";
    }




    //binds events to filters only if they are before the program or campus controls.
    function bindEventsToFilters() {
        var dependantFields = [];
        var fieldSort = 0;
        var idsort = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").attr("id-sort");
        fieldSort = parseInt(idsort);

        dependantFields.push('CampusPreference', 'Age', 'Country', 'Highest_Level_of_Education_Completed', 'GPA', 'Year_of_Highest_Education_Completed', 'us_citizen', 'Military_Affiliation', 'Postal_Code', 'State', 'Years_of_Teaching_Experience', 'Years_of_Work_Experience');

        $(FormsEngine.DefaultFormTag).find(':input[me-filter="true"]').each(function () {
            if ($.inArray($(this).attr('code'), dependantFields) == -1) {
                dependantFields.push($(this).attr('code'));
            }
        });

        //Bind dependencies check to programs   
        $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").change(function () {
            checkSwitchTemplate();
        });


        for (var index = 0; index < dependantFields.length; index++) {
            var element = dependantFields[index];
            var value = "";
            if ($(FormsEngine.DefaultFormTag).find(":input[code='" + element + "']").exists()) {
                var dependantField = $(FormsEngine.DefaultFormTag).find(":input[code='" + element + "']");
                if (parseInt($(dependantField).attr("id-sort")) < fieldSort) {

                    if ($(dependantField).is('select')) {
                        $(dependantField).change(function () {
                            //Get programs
                            getPrograms(function () { checkSwitchTemplate(); });
                        });
                    }
                    else if ($(dependantField).is(':radio')) {
                        $(dependantField).click(function () {
                            //Get programs
                            getPrograms(function () { checkSwitchTemplate(); });
                        });
                    }
                    else {
                        $(dependantField).blur(function () {
                            //Get programs
                            getPrograms(function () { checkSwitchTemplate(); });
                        });
                    }
                }
            }
        };

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
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof'][value='No']").click();
                    $("li.FinancialAidProof").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof'][value='No']").trigger("change");
                }
                else if (!isRadio &&
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAid']").val() == "No") {
                    //the user the user does not have funds so no need to ask for proof
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof'][value='No']").click();
                    $("li.FinancialAidProof").hide();
                    $(FormsEngine.DefaultFormTag).find(":input[code='FinancialAidProof']").trigger("change");
                }
                else {
                    $("li.FinancialAidProof").show();
                }

            });
            $("li.FinancialAidProof").hide(); //hide by default
        }
    }

    //Configure defaults on every template reload
    function configureDefaults() {
        FormsEngine.TemplateID = $(FormsEngine.DefaultFormTag).attr("templateid");
        //LastProgramDependencies = "-";

        //TooltipIcon
        toolTipEdLevel();

        //User Agreement/Express Consent
        updateUserAgreementText();

        //jquery validator defaults
        $.validator.setDefaults({
            onkeyup: false
            //onfocusout: true,
            //onsubmit: true
        })

        //Global Error box
        if (FormsEngine.GlobalErrorBoxExists) {
            $(FormsEngine.DefaultFormTag).validate({
                errorLabelContainer: "#ErrorBox",
                wrapper: "li",
                invalidHandler: function (form, validator) {
                    var errors = validator.numberOfInvalids();
                    if (errors) {
                        validator.errorList[0].element.focus();
                        var offset = $('#ErrorBox' + $(validator.errorList[0].element).attr("section")).offset().top;
                        if (offset > 0) {
                            $('html, body').scrollTop(offset - 30);
                        }
                    }
                }
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
                },
                invalidHandler: function (form, validator) {
                    var errors = validator.numberOfInvalids();
                    if (errors) {
                        validator.errorList[0].element.focus();
                        var offset = $('#ErrorBox' + $(validator.errorList[0].element).attr("section")).offset().top;
                        if (offset >= 0) {
                            $('html, body').scrollTop(offset - 30);
                        }
                    }
                }
            });
        }
        else //Inline error box
        {
            $(FormsEngine.DefaultFormTag).validate();
        }


        //Email validation rule
        if ($(FormsEngine.DefaultFormTag).find("input[code='Email']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[code='Email']").rules("add", {
                email: true,
                emailservercheck: true
            });
        }

        //Profanity for texboxes and textareas
        $(FormsEngine.DefaultFormTag).find("input[type='text']")
            .not('[code=Phone]')
            .not('[code=Alternate_Phone]')
            .not('[code=Email]')
            .not('[code=Age]')
            .not('[code=Postal_Code]')
            .each(function () {
                $(this).rules("add", {
                    profanity: true
                });
            });

        //Country restrictions
        if ($(FormsEngine.DefaultFormTag).find(":input[code='Country']").exists()) {
            $(FormsEngine.DefaultFormTag).find(":input[code='Country']").rules("add", {
                countryservercheck: true
            });
            $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest']").rules("add", {
                countryservercheck: true
            });
        }


        //Phone number server validations
        if ($(FormsEngine.DefaultFormTag).find("input[code='Phone']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[code='Phone']").rules("add", { phoneservercheck: true });
            //if (typeof $.fn.inputmask !== 'undefined') $(FormsEngine.DefaultFormTag).find("input[code='Phone']").inputmask("phonenumber");//INPUT MASKING *********
        }
        //Alternate Phone number server validations
        if ($(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").rules("add", { phoneservercheck: true });
            //if (typeof $.fn.inputmask !== 'undefined') $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").inputmask("phonenumber");//INPUT MASKING *********
        }

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
        }


        //NUMERIC DataType Validations
        $(FormsEngine.DefaultFormTag).find("input[datatype='NUMERIC']").each(function () {
            $(this).rules("add", {
                regex: /^[0-9]+$/
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

        //Country City state info from zipCode
        $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").blur(function (event) {
            if ($(this).val().length > 0) {
                FormsEngine.createCookie("user_postal_code", $(this).val());
            }
            getCityStateCountry($.trim($(this).val()));
        });

        //Form Submit Event
        $(FormsEngine.DefaultFormTag).find("input[id='eddynexusformSubmit']").click(function (event) {
            event.preventDefault();

            // disable submission event for the Admin Preview
            if (FormsEngine.IsForAdminPreview) {
                return false;
            }

            showLoader();
            setSettingsFromCookies();

            //clean restricted characters out of form inputs
            fe_cleanFormData();

            //force full form validate before checking if the form is valid.
            var validator = $(FormsEngine.DefaultFormTag).validate();
            validator.form();

            //Validate form
            if ($(FormsEngine.DefaultFormTag).valid()) {
                submitForm();
            } else {
                hideLoader(false);

            }

            return false;
        });

        //Country DDL change event
        if ($(FormsEngine.DefaultFormTag).find('select[code="Country"]').isControlBefore('State')) {
            $(FormsEngine.DefaultFormTag).find('select[code="Country"]').change(function () {
                getCountryStates();
            });
        }

        //Program control 
        $(FormsEngine.DefaultFormTag).find('select[code="Program_Of_Interest"]').change(function () { getProgramDetail(); });

        //Bind field filters events
        bindEventsToFilters();

        //Form save event
        $(FormsEngine.DefaultFormTag).find(':input').each(function () {
            $(this).blur(function (event) { event.preventDefault(); saveForm(); });
        });


        //Prospect special fields blur event
        $(FormsEngine.DefaultFormTag).find("input[code='Email']").blur(function (event) {
            if ($(FormsEngine.DefaultFormTag).find("input[code='Email']").val() != "") {
                submitProspect();
            }
        });
        $(FormsEngine.DefaultFormTag).find("input[code='Phone']").blur(function (event) {
            if ($(FormsEngine.DefaultFormTag).find("input[code='Phone']").val() != "") {
                submitProspect();
            }
        });
        $(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").blur(function (event) {
            if ($(FormsEngine.DefaultFormTag).find("input[code='Alternate_Phone']").val() != "") {
                submitProspect();
            }
        });

        $(FormsEngine.DefaultFormTag).find("input[code='NewsLetterOptIn']").click(function (event) {
            fe_submitProspectAdditionalInfo(true);
        });

        //Adds class to the label of a field when the check box is checked.
        $(".radio input:checked").prev("label").addClass("checked");

        //Provides Class to the Label fields when they are clicked.
        $(".responsive #eddy-form-container .radio label").click(function () {

            if ($(this).attr("for") == $(this).next("input").attr("id")) {
                $(this).parents(".radio").find("label").removeClass("checked");
                $(this).addClass("checked");
            }

        });

        //School Custom Rules
        schoolsCustomRules();

        //LeadId
        fe_initialize_Leadid();
        fe_initialize_ActiveProspect();
        fe_setLeadSourceUrlAndFormLeadUrl();
    }

    function showLoader() {
        $(FormsEngine.LoaderTag).addClass('loaderOn');
        $(FormsEngine.ModalOverlayTag).show();
        $(FormsEngine.LoaderTag).show();

        $(FormsEngine.LoaderTag).css("left", $(window).width() / 2 - ($(FormsEngine.LoaderTag).width() / 2));

    }

    function hideLoader(keepOverlayShown) {
        if (!keepOverlayShown) {
            $(FormsEngine.ModalOverlayTag).hide();
        }
        $(FormsEngine.LoaderTag).hide();
    }

    function toolTipEdLevel() {
        var id = $(FormsEngine.DefaultFormTag).find('select[code="Highest_Level_of_Education_Completed"]').attr("id");
        $(FormsEngine.DefaultFormTag).find("label[for=" + id + "]").html($(FormsEngine.DefaultFormTag).find("label[for=" + id + "]").html() + '<a href="#" class="tooltip" title="Please choose the highest level of education you have completed or will complete in the near future. For instance, if you are about to graduate with a bachelor&#39;s degree, select &#34;Bachelor&#34; here.">&nbsp;</a>');
        $(FormsEngine.DefaultFormTag).find(".tooltip").each(function () {
            if ($(this).attr("title").length > 0) {
                $(this).after('<div class="ttip"></div>');
                $(this).next('.ttip').html($(this).attr("title"));
                $(this).removeAttr('title');
            }
        });
        $(FormsEngine.DefaultFormTag).find(".tooltip").hover(function () {
            $(this).next('.ttip').fadeIn(200);
        },
            function () {
                $(this).next('.ttip').fadeOut(200);
            });
    }

    function updateUserAgreementText() {

        if ($(FormsEngine.DefaultFormTag).find("input[code='UserAgreement']").exists()) {
            fe_getResourceMetaDataTextForKey('JS.PTEMPLATE.USERAGREEMENTDEFAULT,GRADSCHOOL.FREE.ADDITIONALDISCLAIMER', function (data) {
                FormsEngine.ResourceData = data;
                var userAgreementText = '* ' + fe_trimAsterisk(data['JS.PTEMPLATE.USERAGREEMENTDEFAULT']);
                var userAgreementControl = $(FormsEngine.DefaultFormTag).find("input[code='UserAgreement']");

                //Alternative user agreement text via Optimizely
                if (FormsEngine.AlternativeUserAgreement != undefined && FormsEngine.AlternativeUserAgreement.length > 0) {
                    userAgreementText = FormsEngine.AlternativeUserAgreement;
                }

                if (userAgreementText.indexOf('{0}') > 0 || userAgreementText.indexOf('{school-name}') > 0) {
                    FormsEngine.InstitutionName = (FormsEngine.InstitutionName != undefined && FormsEngine.InstitutionName != null) ? FormsEngine.InstitutionName : ''
                    var school = (FormsEngine.InstitutionName != '') ? FormsEngine.InstitutionName : 'this school';
                    if (FormsEngine.HideSchoolFromTCPA) {
                        school = 'this school';
                    }

                    if (school.search("Kaplan University") > -1) {
                        school = school.replace("Kaplan University", "Purdue Global/Kaplan North America, LLC.");
                    }
                    //Only Purdue
                    else if (school.search("Purdue Global") > -1) {
                        school = school.replace("Purdue Global", "Purdue Global/Kaplan North America, LLC.");
                    }

                    if (school.search("Northcentral University") > -1) {
                        school = school.replace("Northcentral University", "Northcentral University and its National University Affiliates");
                    }

                    if (school.search("Lynn University") > -1) {
                        school = school.replace("Lynn University", "Lynn University/Kaplan North America, LLC.");
                    }

                    userAgreementText = userAgreementText.replace('{0}', school);
                    userAgreementText = userAgreementText.replace('{school-name}', school);
                }

                $(userAgreementControl).val(userAgreementText);
                $(userAgreementControl).attr('label-name', userAgreementText);
                $('label[for=' + $(userAgreementControl).attr('id') + ']').html(userAgreementText);

                //change checkbox to label (hide checkbox and preselect) in UA
                if (FormsEngine.UserAgreementAsLabel === true) {
                    $(userAgreementControl).css({ "display": "none" });
                    $(userAgreementControl).prop('checked', true);
                }
            });
        }


    }

    // for admin preview add some dummy programs to ddl
    function addDummyProgramForPreview() {
        if (FormsEngine.IsForAdminPreview) {
            if ($('select[name="Program_Of_Interest"] option').length < 2) {
                var option = '<option value="0000001" data-programlevelname="Bachelor">Bachelor\'s in Basket Weaving (sample program)</option>';
                $('select[name="Program_Of_Interest"]').append(option);
            }
        }
    }

    //hold specific school exceptions
    function schoolsCustomRules() {

        //Devry: Remove current year from grad list, remove Haven’t completed high school WI.53699-53700
        if (jQuery.inArray(FormsEngine.InstitutionId, [146, 230, 127, 12, 461]) >= 0) {
            $(":input[code='Highest_Level_of_Education_Completed'] option[value='1']").remove();
            $(":input[code='Year_of_Highest_Education_Completed'] option[value='" + (new Date).getFullYear().toString() + "']").remove();
        }
    }

    function trackGTMLeadSubmit(isSuccess) {
        if (FormsEngine.ApplicationId == 7) {
            var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
            var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');

            if (PaidStatusTypeId == 1 && isSuccess) { //free
                fe_googleTagEvent('gaEvent', 'client', 'free-lead-success', FormsEngine.ProgramId);
            }
            else if (PaidStatusTypeId == 1 && !isSuccess) { //free
                fe_googleTagEvent('gaEvent', 'client', 'free-lead-failed', FormsEngine.ProgramId);
            }
            else if (PaidStatusTypeId == 2 && isSuccess) { //fraid
                fe_googleTagEvent('gaEvent', 'client', 'fraid-lead-success', FormsEngine.ProgramId);
            }
            else if (PaidStatusTypeId == 2 && !isSuccess) { //fraid
                fe_googleTagEvent('gaEvent', 'client', 'fraid-lead-failed', FormsEngine.ProgramId);
            }
            else if (isSuccess) { //paid-- normal
                fe_googleTagEvent('gaEvent', 'client', 'lead-success', FormsEngine.ProgramId);
            }
            else {
                fe_googleTagEvent('gaEvent', 'client', 'lead-failed', FormsEngine.ProgramId);
            }
        }
        else if (isSuccess) {
            fe_googleTagEvent('gaEvent', 'client', 'lead-success', FormsEngine.ProgramId);
        }
        else {
            fe_googleTagEvent('gaEvent', 'client', 'lead-failed', FormsEngine.ProgramId);
        }
    }


    function trackOPTUniqueLeadInitial() {
        if (FormsEngine.ApplicationId == 7) {
            var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
            var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');

            if (PaidStatusTypeId == 1) { //free
                //add 1 to the unique user, this basically says we converted the lead
                window['optimizely'].push(["trackEvent", "free_unique_user"]);

                //add 1 to the total leads for the initial match
                window['optimizely'].push(["trackEvent", "total_free_leads", { "anonymous": true }]);
            }
            else if (PaidStatusTypeId == 2) { //fraid
                //add 1 to the unique user, this basically says we converted the lead
                window['optimizely'].push(["trackEvent", "fraid_unique_user"]);

                //add 1 to the total leads for the initial match
                window['optimizely'].push(["trackEvent", "total_free_leads", { "anonymous": true }]);
            }
            else { //paid-- normal
                //add 1 to the unique user, this basically says we converted the lead
                window['optimizely'].push(["trackEvent", "unique_user"]);

                //add 1 to the total leads for the initial match
                window['optimizely'].push(["trackEvent", "total_leads", { "anonymous": true }]);
            }

        }
        else {

            //add 1 to the unique user, this basically says we converted the lead
            window['optimizely'].push(["trackEvent", "unique_user"]);

            //add 1 to the total leads for the initial match
            window['optimizely'].push(["trackEvent", "total_leads", { "anonymous": true }]);
        }
    }

    function checkTwoULeadShareForProgram() {
        //get selected option
        var selected = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
        //check attribute for show 2U box
        if (selected.attr('data-showtwoucheckbox') == "True") {
            fe_showUserAgreementForTwoULeadShareSchool(FormsEngine.InstitutionName, selected.attr('data-programtype'), '.UserAgreement');
        }
        else {
            fe_removeUserAgreementForTwoULeadShareSchool();
        }
    }

    $(window).resize(function () {

        //Changes loader's left position if it is the browser resizes. 
        if ($(FormsEngine.LoaderTag).length > 0) {
            $(FormsEngine.LoaderTag).css("left", $(window).width() / 2 - ($(FormsEngine.LoaderTag).width() / 2));
        }

    });


    $(document).ready(function () {

        //Enter management
        $(document).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        $(document).keydown(function (e) {
            if (e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });

        $(FormsEngine.DefaultFormTag).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
        });


        //Old IE compatibility issue
        if ($.browser.msie) {
            $('head').append('<meta http-equiv="x-ua-compatible" content="IE=edge" />');
        }

        //Set defaults
        configureDefaults();

        //Get programs
        getPrograms(function () {
            checkSwitchTemplate();
            //GoogleTag Event
            fe_trackGTMLeadStart();
        });

        //Load ZipCode from cookie if available
        setSettingsFromCookies();
        if (FormsEngine.CookieZipCode != undefined && FormsEngine.CookieZipCode != null) {
            $(FormsEngine.DefaultFormTag).find("input[code='Postal_Code']").val(FormsEngine.CookieZipCode);
            getCityStateCountry(FormsEngine.CookieZipCode);
        }

        //Load previously saved form data
        loadForm();

        //Load Form Pass Thru
        if (!LoadFormFromPassThru()) {
            //Set values from querystring
            setValuesFromQuerystring();
        }

        // show cross sell example if querystring contains optimizely cross sell flag
        CheckForOptimizelyCrossSellFlag();

        //Refresh template to be used by admin
        FormsEngine.Refresh = function () {
            refreshTemplate();
        };

        addDummyProgramForPreview();

        //GradSchoolClass for GS templates
        checkGSTemplate();

        //hide labels and swap watermarks if need be
        fe_checkResponsiveItems();

        //add the mask and remove the datatype attribute from the control to remove regex validation
        fe_ApplyPhoneMask();
    });

})(jQuery);
