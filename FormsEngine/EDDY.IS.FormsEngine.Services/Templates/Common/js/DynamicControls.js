//# sourceURL=DynamicControls.js
(function ($) {

    //Programs DataBind
    function bindPrograms(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&DeviceId=" + FormsEngine.DeviceId;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += "&InstitutionId=" + FormsEngine.InstitutionId;
        filters += '&FormTemplateType=' + FormsEngine.FormTemplateType;
        filters += "&IncludeProgramLevelGroups=" + FormsEngine.IncludeProgramLevelGroups;
        filters += "&TemplateId=" + FormsEngine.TemplateId;


        if (FormsEngine.CategoryIds != null && FormsEngine.CategoryIds.length > 0)
            filters += '&Categories=' + FormsEngine.CategoryIds;

        if (FormsEngine.SubjectIds != null && FormsEngine.SubjectIds.length > 0)
            filters += '&SubCategories=' + FormsEngine.SubjectIds;

        if (FormsEngine.SpecialtyIds != null && FormsEngine.SpecialtyIds.length > 0)
            filters += '&Specialties=' + FormsEngine.SpecialtyIds;

        if (FormsEngine.FeatureId != null && FormsEngine.FeatureId > 0)
            filters += '&FeatureId=' + FormsEngine.FeatureId;

        if (!fe_formHasEMSApplicationId())
            filters += "&ProgramId=" + ((FormsEngine.ProgramId > 0) ? FormsEngine.ProgramId : "");

        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetPrograms";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var ProgramsDDLContainer = $(FormsEngine.DefaultFormTag).find('.Program_Of_Interest');
                var ProgramsDDL = $(FormsEngine.DefaultFormTag).find('select[code="Program_Of_Interest"]');
                var lastProgram = $(ProgramsDDL).find(':selected').text();
                ProgramsDDL.empty();
                $(ProgramsDDL).append(decodeURIComponent(data).replace(/\+/g, ' '));
                var programElements = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option").filter(function (val) { return val != ""; });
                var featureListInUseWithOneProgram = programElements.length == 1 && fe_featureListInUse();

                // Inject the Label into the first option if inline-ddl..
                if ($(ProgramsDDL).hasClass("inlineDropDown")) {
                    var $label = $(ProgramsDDL).siblings("label[for=" + $(ProgramsDDL).prop("id") + "]");
                    if ($label.length == 0) {
                        $label = $(ProgramsDDL).parent().siblings("label[for=" + $(ProgramsDDL).prop("id") + "]");
                    }
                    $(ProgramsDDL).find("option").eq(0).text($label.text());
                }

                if ($(ProgramsDDL).find("option[value!='']").index(":selected") > 0) {
                    $(ProgramsDDL).find("option:contains(" + lastProgram + ")").attr('selected', 'selected');
                } else {
                    if (featureListInUseWithOneProgram) {
                        var programId = $(programElements[0]).val();
                        $(ProgramsDDL).val(programId);
                    } else if (FormsEngine.ProgramId && FormsEngine.ProgramId > 0) {
                        var programExistsInDropdown = $(ProgramsDDL).find("option[value=" + FormsEngine.ProgramId + "]").length > 0;

                        if (programExistsInDropdown) {
                            $(ProgramsDDL).val(FormsEngine.ProgramId);
                        } else {
                            $(ProgramsDDL).find("option[data-default='default']").attr('selected', 'selected');
                        }

                    } else {
                        var recoveredProgramId = fe_getParameterByNameAndAliasFromString("Program_Of_Interest", FormsEngine.StringToRecover);
                        var userSetProgram = (recoveredProgramId != null && recoveredProgramId != "" && !isNaN(recoveredProgramId));
                        var categorySubjectSpecialtyFiltersExists = ((FormsEngine.CategoryIds && FormsEngine.CategoryIds.length > 0) || (FormsEngine.SubjectIds && FormsEngine.SubjectIds.length > 0) || (FormsEngine.SpecialtyIds && FormsEngine.SpecialtyIds.length > 0));

                        if (categorySubjectSpecialtyFiltersExists && programElements.length > 0 && FormsEngine.FormTemplateType == 3 && !userSetProgram) {
                            var programsArr = [];
                            for (var i = 0; i < programElements.length; i++) {
                                programsArr.push({
                                    ProgramId: $(programElements[i]).val(),
                                    SRA: Number($(programElements[i]).data("programrankscore")) || 0
                                });
                            }

                            if (programsArr.length > 0) {
                                var topSRAProgram = programsArr.sort(function (a, b) { return b.SRA - a.SRA; })[0];
                                $(ProgramsDDL).val(topSRAProgram.ProgramId);
                            }
                        }
                    }

                    var selProgram = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
                    var selProgramId = $(selProgram).val();
                    if (selProgramId != null && selProgramId != "") {
                        FormsEngine.ProgramId = selProgramId;
                        FormsEngine.ProgramName = $(selProgram).text();
                        FormsEngine.ProgramProductId = $(selProgram).attr('data-programproductid');
                        FormsEngine.ProductId = $(selProgram).attr('data-productid');
                        FormsEngine.ProgramTemplateId = $(selProgram).attr('data-templateid');
                       
                    }

                    
                }

                FormsEngine.CustomTCPA = ProgramsDDL.find('option').first().attr('data-customtcpa');

                if (FormsEngine.CampusPreferenceProcessed != true) {
                    fe_processSupportedMatchPreference();
                }

                fe_getProgramDetail();
                fe_getProgramWizardDetail();
                fe_getInstitutionDetail();
                if (FormsEngine.StepLast === 1) {
                    FormsEngine.LoadDynamicQuestions();
                }

                $(FormsEngine).trigger("OnProgramsLoaded");
                $(FormsEngine).trigger("OnProgramSet");
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }


    //Dynamic Campuses DataBind
    function bindCampuses(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&DeviceId=" + FormsEngine.DeviceId;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += "&InstitutionId=" + FormsEngine.InstitutionId;
        filters += '&FormTemplateType=' + FormsEngine.FormTemplateType;
        filters += "&IncludeProgramLevelGroups=" + FormsEngine.IncludeProgramLevelGroups;
        filters += "&TemplateId=" + FormsEngine.TemplateId;


        if (FormsEngine.CategoryIds != null && FormsEngine.CategoryIds.length > 0)
            filters += '&Categories=' + FormsEngine.CategoryIds;

        if (FormsEngine.SubjectIds != null && FormsEngine.SubjectIds.length > 0)
            filters += '&SubCategories=' + FormsEngine.SubjectIds;

        if (FormsEngine.SpecialtyIds != null && FormsEngine.SpecialtyIds.length > 0)
            filters += '&Specialties=' + FormsEngine.SpecialtyIds;


        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCampuses";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="Campus"]');
                var dropdownItems = [];



                if (data.length > 0) {
                    jQuery.each(data, function (index, campus) {
                        dropdownItems.push({ Value: campus.CampusId, Text: campus.CampusName, Key: campus.CampusId, Selected: FormsEngine.CampusId === campus.CampusId });

                    });
                    loadOptionsIntoDropDownControl(control, dropdownItems);
                }

                $(FormsEngine).trigger("OnCampusLoaded");
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //States DataBind
    function bindStates(filters, callback) {
        if (filters == "") {
            filters = "Country=" + FormsEngine.DefaultCountryCode;
        }
        var DataBindFilter = {};
        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetStates";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var controlCountry = $(FormsEngine.DefaultFormTag).find('select[code="Country"]');
                var mobileControls = [];
                FormsEngine.LastCountryBinded = $(controlCountry).val();
                var control = $(FormsEngine.DefaultFormTag).find('select[code="State"]');
                var transformControl = $(control).attr("data-transform") == 'True';
                control.empty();

                if ($(control).hasClass("inlineDropDown")) {
                    makeInlineDropDown(control);
                }
                else {
                    control.append(jQuery('<option/>', {
                        value: "",
                        text: FormsEngine.DefaultSelectText
                    }));
                }

                jQuery(FormsEngine.DefaultFormTag).find('[name="DynamicStateDiv"]').remove();
                if (data.length > 0) {
                    jQuery.each(data, function (index, item) {
                        if (item.Selected == true) {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key, 'selected': ' ' }));
                            if (transformControl) {
                                var mobileControl = jQuery('<div/>', { 'class': 'radio-inline dropdown-button', 'name': 'DynamicStateDiv' });
                                mobileControl.append(jQuery('<input/>', { 'type': 'radio', 'value': item.Value, 'text': item.Text, 'key': item.Key, 'id': 'DDLRD_' + item.Value, 'name': 'DDLRD_DynamicStateDiv', 'class': 'radio-field', 'required': 'required', 'checked': '', 'parent-code': 'State' }));
                                mobileControl.append(jQuery('<label>', { 'for': 'DDLRD_' + item.Value }).append(item.Text));
                                mobileControls.push(mobileControl);
                            }
                        }
                        else {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key }));
                            if (transformControl) {
                                var mobileControl = jQuery('<div/>', { 'class': 'radio-inline dropdown-button', 'name': 'DynamicStateDiv' });
                                mobileControl.append(jQuery('<input/>', { 'type': 'radio', 'value': item.Value, 'text': item.Text, 'key': item.Key, 'id': 'DDLRD_' + item.Value, 'name': 'DDLRD_DynamicStateDiv', 'class': 'radio-field', 'required': 'required', 'parent-code': 'State' }));
                                mobileControl.append(jQuery('<label>', { 'for': 'DDLRD_' + item.Value }).append(item.Text));
                                mobileControls.push(mobileControl);
                            }
                        }
                    });
                }
                else {
                    control.append(jQuery('<option/>', { 'value': "N/A", 'text': "Not Applicable", 'selected': ' ' }));
                    var mobileControl = jQuery('<div/>', { 'class': 'radio-inline dropdown-button', 'name': 'DynamicStateDiv' });
                    mobileControl.append(jQuery('<input/>', { 'type': 'radio', 'value': "N/A", 'text': "Not Applicable", 'key': "N/A", 'id': 'DDLRD_NA', 'name': 'DDLRD_DynamicStateDiv', 'class': 'radio-field', 'required': 'required', 'checked': '', 'parent-code': 'State' }));
                    mobileControl.append(jQuery('<label>', { 'for': 'DDLRD_NA' }).append("Not Applicable"));
                    mobileControls.push(mobileControl);
                }

                if (transformControl) {
                    jQuery.each(mobileControls.reverse(), function (index, item) {
                        jQuery(item).insertAfter(control);
                    });
                    jQuery(FormsEngine.DefaultFormTag).find('[name=DDLRD_DynamicStateDiv]').unbind('change');
                    jQuery(FormsEngine.DefaultFormTag).find('[name=DDLRD_DynamicStateDiv]').change(function () {
                        var code = $(this).attr('parent-code');
                        $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').val($(this).val());
                        $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').change();
                    });

                    jQuery(control).unbind('change');
                    jQuery(control).change(function () {
                        var code = $(this).attr('code');
                        $(FormsEngine.DefaultFormTag).find(":input[parent-code='" + code + "'][value='" + $(this).val() + "']").prop('checked', true);
                    });
                }

                if ($(control).val()) {
                    if ($(control).valid()) {
                        $(control).removeClass('error');
                    }
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //Country DataBind
    function bindCountries(filters, callback) {
        var DataBindFilter = {};
        DataBindFilter.FilterString = filters;

        if (filters == "") {
            callback();
            return;
        }

        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCountries";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="Country"]');
                var controlvalue = $(control).val();
                control.empty();

                if ($(control).hasClass("inlineDropDown")) {
                    makeInlineDropDown(control);
                }
                else {
                    control.append(jQuery('<option/>', {
                        value: "",
                        text: FormsEngine.DefaultSelectText
                    }));
                }

                if (data.length > 0) {
                    // If no country has been selected and postal code is specified..
                    var postalCode = $(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').val() || "";
                    var selectedCountries = data.filter(function (d) { return d.Selected === true; });
                    var selectedCountryValue = "";

                    if (controlvalue && ["US", "CA"].indexOf(controlvalue) < 0 && selectedCountries.length === 0 && postalCode !== "") {
                        (data.filter(function (d) { return d.Value == controlvalue; }) || [{}])[0].Selected = true;
                    }

                    jQuery.each(data, function (index, item) {
                        if (item.Selected == true) {
                            selectedCountryValue = item.Value;
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key, 'selected': ' ' }));
                            jQuery(":input[parent-code='Country'][value='" + item.Value + "']").prop('checked', true);
                        }
                        else {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key }));
                        }
                    });
                    if (selectedCountryValue.length > 0) {
                        jQuery(":input[code='Country']").val(selectedCountryValue);
                        if (selectedCountryValue == "GE") {
                            var session = ""

                            try {
                                session = jQuery.cookie('_Session');
                            }
                            catch (e) { }

                            fe_logClientException(null, sUrl, "Georgia Country Selected (DynamicControls) DataBindFilter=" + filters + " State=" + jQuery(':input[code="State"]').val() + " TrackingSession=" + session);
                        }
                    }
                }
                else {
                    control.append(jQuery('<option/>', { 'value': "N/A", 'text': "Not Applicable", 'selected': ' ' }));
                }


                if ($(control).val()) {
                    if ($(control).valid()) {
                        $(control).removeClass('error');
                    }
                }

            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //City DataBind
    function bindCity(filters, callback) {
        var DataBindFilter = {};
        DataBindFilter.FilterString = filters;

        if (filters == "") {
            fe_AddressSmartControlShow(true);
            callback();
            return;
        }
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCity";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find("input[code='City']");
                if (data.Text != "" && data.Text != undefined) {
                    $(control).val(data.Text);
                    $(control).valid();
                    fe_AddressSmartControlShow(false);
                }
                else {
                    fe_AddressSmartControlShow(true);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //Desired Countries DataBind
    function bindDesiredCountries(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&DeviceId=" + FormsEngine.DeviceId;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += '&FormTemplateType=' + FormsEngine.FormTemplateType;
        DataBindFilter.FilterString = filters;


        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetDesiredCountries";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="Desired_Country"]');
                loadOptionsIntoDropDownControl(control, data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    function bindDesiredCities(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&DeviceId=" + FormsEngine.DeviceId;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += '&FormTemplateType=' + FormsEngine.FormTemplateType;
        DataBindFilter.FilterString = filters;

        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetDesiredCities";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="Desired_City"]');
                loadOptionsIntoDropDownControl(control, data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }


    //ProgramLevels DataBind
    function bindDesiredDegreeLevel(filters, callback) {
        getProgramLevels(filters, callback, function (data) {
            var control = $(FormsEngine.DefaultFormTag).find('select[code="Desired_Degree_Level"]');
            fe_consolelog(control);
            if (control.length > 0) {
                fe_consolelog('building degree dropdown');
                loadOptionsIntoDropDownControl(control, data);
            }
            else {
                fe_consolelog('building multicheckbox');
                //save selected options
                var selectedValues = [];
                $.each(jQuery(FormsEngine.DefaultFormTag).find('[name=Desired_Degree_Levels]:checked'), function (index, item) {
                    selectedValues.push($(item).val());
                });
                buildMultiCheckBox("Desired_Degree_Level", data, callback);
                //set previously selected options
                $.each(selectedValues, function (index, item) {
                    if ($('[name=Desired_Degree_Levels][value=' + item + ']'))
                        $('[name=Desired_Degree_Levels][value=' + item + ']').prop('checked', true);
                });
            }
        });
    }

    //EMSProgramLevels DataBind
    function bindEMSDesiredDegreeLevel(filters, callback) {
        getProgramLevels(filters, callback, function (data) {
            var control = $(FormsEngine.DefaultFormTag).find('select[code="EMSDesiredDegreeLevel"]');
            loadOptionsIntoDropDownControl(control, data);
        });
    }

    function getProgramLevels(filters, callback, loadResults) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetProgramLevels";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                loadResults(data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    function setDynamicCampusSoftPreferenceControl(data) {

        var control = $(FormsEngine.DefaultFormTag).find('select[code="DynamicCampusSoftPreference"]');
        var label = $(FormsEngine.DefaultFormTag).find("label[for='" + $(control).attr('id') + "']");
        var name = $(control).attr('name');
        var transformControl = $(control).attr("data-transform") == 'True';
        var mobileControls = [];
        var isVisible = false;

        if (name.indexOf('Alternate') >= 0) {

            $(control).parent().hide();
            FormsEngine.DynamicCampusSoftPreferenceShown = false;

            if (data.length > 0) {

                $(control).find('option').hide();
                if (data.length > 1) {

                    var selectedItem = $.grep($(data), function (item) { return item.Text == $(control).find('option:selected').text(); });
                    if (selectedItem.length == 0) $(control).val('');

                    $.each(data, function (index, item) {

                        var itemOption = $.grep($(control).find('option'), function (itemInner) {
                            return itemInner.text == item.Value;
                        });
                        if (itemOption.length > 0) {

                            $(itemOption).attr('key', item.Key);
                            $(itemOption).val(item.Value);
                            $(itemOption).show();
                        }
                    });
                    FormsEngine.DynamicCampusSoftPreferenceShown = true;
                    $(control).parent().show();
                }
                else {

                    $(control).val('');
                    var itemOption = $.grep($(control).find('option'), function (itemInner) {
                        return itemInner.text == data[0].Value;
                    });
                    if (itemOption.length > 0) {

                        $(itemOption).attr('key', data[0].Key);
                        $(itemOption).val(data[0].Value);
                        $(control).val($(itemOption).val());
                        $(itemOption).show();
                    }
                }
            }
            else $(control).val('');
        }
        else { //Regular Dynamic Control
            control.empty();

            if ($(control).hasClass("inlineDropDown")) {
                makeInlineDropDown(control);
            }
            else {
                control.append(jQuery('<option/>', {
                    value: "",
                    text: FormsEngine.DefaultSelectText
                }));
            }

            if (data.length > 0) {
                //visible
                if (data.length > 1) {
                    isVisible = true;
                    FormsEngine.DynamicCampusSoftPreferenceShown = true;
                    $(control).parent().show();
                }
                else { //hidden
                    isVisible = false;
                    FormsEngine.DynamicCampusSoftPreferenceShown = false;
                    $(control).parent().hide();
                }

                jQuery.each(data, function (index, item) {
                    if (item.Selected == true) {
                        if (!transformControl && $(control).hasClass("inlineDropDown")) {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key }));
                        }
                        else {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key, 'selected': ' ' }));
                        }

                        if (isVisible && transformControl) {
                            var mobileControl = jQuery('<div/>', { 'class': 'radio-inline dropdown-button', 'name': 'DynamicCampusSoftPreferenceDiv' });
                            mobileControl.append(jQuery('<input/>', { 'type': 'radio', 'value': item.Value, 'text': item.Text, 'key': item.Key, 'id': 'DDLRD_' + item.Value, 'name': 'DDLRD_DynamicCampusSoftPreference', 'class': 'radio-field', 'required': 'required', 'checked': '', 'parent-code': 'DynamicCampusSoftPreference' }));
                            var labelInner = jQuery('<i/>', { 'class': 'fa fa-' + item.Text });
                            mobileControl.append(jQuery('<label>', { 'for': 'DDLRD_' + item.Value }).html(labelInner).append(item.Text));
                            mobileControls.push(mobileControl);
                        }

                    }
                    else {
                        control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key }));
                        if (isVisible && transformControl) {
                            var mobileControl = jQuery('<div/>', { 'class': 'radio-inline dropdown-button', 'name': 'DynamicCampusSoftPreferenceDiv' });
                            mobileControl.append(jQuery('<input/>', { 'type': 'radio', 'value': item.Value, 'text': item.Text, 'key': item.Key, 'id': 'DDLRD_' + item.Value, 'name': 'DDLRD_DynamicCampusSoftPreference', 'class': 'radio-field', 'required': 'required', 'parent-code': 'DynamicCampusSoftPreference' }));
                            var labelInner = jQuery('<i/>', { 'class': 'fa fa-' + item.Text });
                            mobileControl.append(jQuery('<label>', { 'for': 'DDLRD_' + item.Value }).html(labelInner).append(item.Text));
                            mobileControls.push(mobileControl);
                        }
                    }
                });

                //jquery responsive
                jQuery(FormsEngine.DefaultFormTag).find('[name=DynamicCampusSoftPreferenceDiv]').remove();

                if (isVisible && transformControl) {
                    jQuery.each(mobileControls, function (index, item) {
                        jQuery(item).insertAfter(control);
                    });
                }
                jQuery(FormsEngine.DefaultFormTag).find('[name=DDLRD_DynamicCampusSoftPreference]').unbind('change');
                jQuery(FormsEngine.DefaultFormTag).find('[name=DDLRD_DynamicCampusSoftPreference]').change(function () {
                    var code = $(this).attr('parent-code');
                    $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').val($(this).val());
                    $(FormsEngine.DefaultFormTag).find(':input[code="' + code + '"]').change();
                });

                jQuery(control).unbind('change');
                jQuery(control).change(function () {
                    var code = $(this).attr('code');
                    $(FormsEngine.DefaultFormTag).find(":input[parent-code='" + code + "'][value='" + $(this).val() + "']").prop('checked', true);
                    if (FormsEngine.DynamicRequiredChangeEvent) {
                        FormsEngine.DynamicRequiredChangeEvent();
                    }
                });
            }
            //end of regular dynamic control
        }
    }

    //Dynamic Campus Soft Preference DataBind
    function bindDynamicCampusSoftPreference(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += "&DefaultCampusPreference=" + FormsEngine.DefaultCampusPreference;
        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCampusTypes";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                setDynamicCampusSoftPreferenceControl(data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //Categories DataBind
    function bindCategories(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        DataBindFilter.FilterString = filters;

        addCategorySubCategoryLoader("Categories");
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCategories";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if ($(FormsEngine.DefaultFormTag).find("select[code='Categories']").length > 0) {
                    buildMultiSelectDDLCategorySubCategoryControl("Categories", data, callback);
                }
                else {
                    buildCategorySubCategoryControl("Categories", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    }

    //SubCategories DataBind
    function bindSubCategories(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        DataBindFilter.FilterString = filters;

        addCategorySubCategoryLoader("SubCategories");
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetSubCategories";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if ($(FormsEngine.DefaultFormTag).find("select[code='SubCategories']").length > 0) {
                    buildMultiSelectDDLCategorySubCategoryControl("SubCategories", data, callback);
                }
                else {
                    buildCategorySubCategoryControl("SubCategories", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    }

    //SubCategories DataBind
    function bindSpecialties(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        DataBindFilter.FilterString = filters;

        addCategorySubCategoryLoader("Specialties");
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetSpecialties";
        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if ($(FormsEngine.DefaultFormTag).find("select[code='Specialties']").length > 0) {
                    buildMultiSelectDDLCategorySubCategoryControl("Specialties", data, callback);
                }
                else {
                    buildCategorySubCategoryControl("Specialties", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    }

    //EntryTerms DataBind
    function bindEntryTerms(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        var code = "EntryTerm";

        //Required arguments
        filters += prefix + "Name=" + code + "&";
        filters += prefix + "InstitutionId=" + ((FormsEngine.InstitutionId > 0) ? FormsEngine.InstitutionId : "");
        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetKVCodeData";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="' + code + '"]');
                loadOptionsIntoDropDownControl(control, data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //EntryTerms DataBind
    function bindReferral(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        var code = "Referral";

        //Required arguments
        filters += prefix + "Name=" + code + "&";
        filters += prefix + "InstitutionId=" + ((FormsEngine.InstitutionId > 0) ? FormsEngine.InstitutionId : "");
        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetKVCodeData";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="' + code + '"]');
                loadOptionsIntoDropDownControl(control, data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //EMSLearningPreferenceAndLocations DataBind
    function bindEMSLearningPreferenceAndLocations(filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        filters += "&InstitutionId=" + ((FormsEngine.InstitutionId > 0) ? FormsEngine.InstitutionId : "");

        if (FormsEngine.FeatureId && FormsEngine.FeatureId > 0)
            filters += '&FeatureId=' + FormsEngine.FeatureId;

        DataBindFilter.FilterString = filters;
        var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetCampusLocations";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                var control = $(FormsEngine.DefaultFormTag).find('select[code="EMSLearningPreferenceAndLocations"]');
                var dropdownItems = [];

                if (data.length > 0) {
                    jQuery.each(data, function (index, campus) {
                        dropdownItems.push({ Value: JSON.stringify(campus), Text: "Campus - " + campus.CampusName, Group: null, Selected: false });
                    });
                }

                if (dropdownItems.length > 0) {
                    //we have items, before appending lets see if we need to clear the existing list
                    if (FormsEngine.ClearCampusLocationsBeforeRebind == "true") {
                        control.find('option:enabled').remove();
                    }
                    jQuery.each(dropdownItems, function (index, item) {
                        if (item.Selected == true) {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'selected': ' ' }));
                        }
                        else {
                            control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text }));
                        }
                    });
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }


    function bindSchoolPicker(filters, callback) {

        if (typeof fe_sp_addLoadingSpinnerToComponent !== "undefined") {
            fe_sp_addLoadingSpinnerToComponent(jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel"));
        }

        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        if (filters) {
            filters = "LeadData=" + encodeURIComponent(filters);
        }

        //Required arguments
        filters += prefix + "TrackId=" + FormsEngine.TrackId;
        filters += "&IsBeta=" + FormsEngine.IsBeta;
        filters += "&ApplicationId=" + FormsEngine.ApplicationId;

        sUrl = FormsEngine.ServiceBaseURL + "/SchoolPickerWizard/GetSchoolPickerCarouselComponents";

        $.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: filters,
            cache: false,
            url: sUrl,
            success: function (response) {
                if (typeof fe_sp_createSchoolPicker !== "undefined") {
                    fe_sp_createSchoolPicker(response);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    }

    function bindSchoolPickerFailures(filters, callback) {

        fe_consolelog("Binding school picker failures");

        //var DataBindFilter = {};
        //var prefix = filters == "" || filters == undefined ? "" : "&";

        //var formData = fe_getFormData();

        ////Required arguments
        //filters += prefix + "TrackId=" + FormsEngine.TrackId + "&IsBeta=" + FormsEngine.IsBeta;
        //filters += "&ApplicationId=" + FormsEngine.ApplicationId;
        //filters += "&MaxSchoolPickerMatches=" + FormsEngine.CampaignDetail.MaxSchoolPickerMatches;
        //filters += "&LeadData=" + encodeURIComponent(formData.LeadData);
        //filters += "&AdditionalData=" + encodeURIComponent(formData.LeadAdditionalData);
        //DataBindFilter.FilterString = filters;

        //var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetFailedMatchReplacements";
        //$.ajax({
        //    async: true,
        //    type: 'GET',
        //    dataType: 'jsonp',
        //    data: DataBindFilter,
        //    cache: false,
        //    url: sUrl,
        //    success: function (failedMatchReplacements) {
        //        fe_consolelog(failedMatchReplacements);
        //    },
        //    error: function (request, textStatus, errorThrown) {
        //        fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
        //        fe_logClientException(request, sUrl, errorThrown);
        //        callback();
        //    }
        //});
    }

    function addCategorySubCategoryLoader(controlCode) {
        $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").empty();
        if ($(FormsEngine.DefaultFormTag).find("ul[code='Featured-" + controlCode + "']").exists()) {
            $(FormsEngine.DefaultFormTag).find("ul[code='Featured-" + controlCode + "']").empty(); //clear featured placeholder also
        }
        $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").append('<li>Loading...</li>');

    }

    function buildCategorySubCategoryControl(controlCode, data, callback) {
        $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").empty();
        if ($(FormsEngine.DefaultFormTag).find("ul[code='Featured-" + controlCode + "']").exists()) {
            $(FormsEngine.DefaultFormTag).find("ul[code='Featured-" + controlCode + "']").empty(); //clear featured placeholder also
        }

        if (controlCode == "Categories") {

            var hiddenValControl = $(FormsEngine.DefaultFormTag).find("input[name='Categories_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultCategories = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultCategories').attr('data-defaults') != undefined) {
                defaultCategories = $(FormsEngine.DefaultFormTag).find('#defaultCategories').attr('data-defaults').split(",");
            }
            var popularCategories = []
            if ($(FormsEngine.DefaultFormTag).find('#popularCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#popularCategories').attr('data-defaults') != undefined) {
                popularCategories = $(FormsEngine.DefaultFormTag).find('#popularCategories').attr('data-defaults').split(",");
            }
            var queryStringParam = fe_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fe_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
            }

            $.each(data.CategoryList, function (index, value) {
                var category = (value.CategoryName).replace(/[_\W]+/g, "-").toLowerCase();
                var liElement = $(document.createElement('li'));
                liElement.attr({ 'class': category });
                var label = $("<label>").attr({ 'for': controlCode + '_' + value.CategoryId, 'class': category });
                label.html(value.CategoryName);

                var checkbox = $(document.createElement('input')).attr({ id: controlCode + '_' + value.CategoryId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);
                if (idArrayDistinct != null &&
                    idArrayDistinct.length > 0 &&
                    $.inArray(value.CategoryId.toString(), queryStringParam) > -1 &&
                    $(FormsEngine.DefaultFormTag).find('ul.featured-categories').exists()) {
                    //we have values and one of them is this controls category so add it to our featured categories
                    $(FormsEngine.DefaultFormTag).find('ul.featured-categories').append(liElement);
                    if ($.inArray(value.CategoryId.toString(), popularCategories) > -1) {
                        //we have at least one popular category so hide the main categories and show the button
                        $(FormsEngine.DefaultFormTag).find('#showHideCategories').show();
                        $(FormsEngine.DefaultFormTag).find('#ulCategories').hide();
                    }
                }
                else if ($.inArray(value.CategoryId.toString(), popularCategories) > -1 && $(FormsEngine.DefaultFormTag).find('ul.featured-categories').exists()) {
                    //we have at least one popular category so hide the main categories and show the button
                    $(FormsEngine.DefaultFormTag).find('#showHideCategories').show();
                    $(FormsEngine.DefaultFormTag).find('#ulCategories').hide();
                    $(FormsEngine.DefaultFormTag).find('ul.featured-categories').append(liElement);
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").append(liElement);
                }
                checkbox.change(function (e) { changeSelectionCategorySubCategory(controlCode, e.target); });
                if ($.inArray(value.CategoryId.toString(), defaultCategories) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                    if ($.inArray(value.CategoryId.toString(), popularCategories) < 0) {
                        $(FormsEngine.DefaultFormTag).find('#ulCategories').show();
                        $(FormsEngine.DefaultFormTag).find('#showHideCategories').html('See Less Options');
                    }
                }
            });

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                assignCategorySubCategoryHiddenValues(controlCode, hiddenValControl);
            }

            //hide featured if there are none
            if ($(FormsEngine.DefaultFormTag).find('ul.featured-categories').exists()) {
                var liCount = $(FormsEngine.DefaultFormTag).find('ul.featured-categories').children().size();
                if (liCount == null || liCount == undefined || liCount == 0) {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-categories').hide();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-categories').show();
                }
            }
            if ($(FormsEngine.DefaultFormTag).find('#showHideCategories').exists()) {
                $(FormsEngine.DefaultFormTag).find('#showHideCategories').unbind('click');
                $(FormsEngine.DefaultFormTag).find('#showHideCategories').click(function () {
                    if ($(FormsEngine.DefaultFormTag).find('#ulCategories').is(":visible")) {
                        $(FormsEngine.DefaultFormTag).find('#ulCategories').hide();
                        $(this).html('See More Options');
                        $(this).html('See Less Options');
                    }
                    else {
                        $(FormsEngine.DefaultFormTag).find('#ulCategories').show();
                    }
                });
            }
        }
        else if (controlCode == "SubCategories") {
            var hiddenValControl = $(FormsEngine.DefaultFormTag).find("input[name='SubCategories_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultSubCategories = [];
            var popularSubCategories = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultSubCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultSubCategories').attr('data-defaults') != undefined) {
                defaultSubCategories = $(FormsEngine.DefaultFormTag).find('#defaultSubCategories').attr('data-defaults').split(",");
            }
            var popularCategories = []
            if ($(FormsEngine.DefaultFormTag).find('#popularSubCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#popularSubCategories').attr('data-defaults') != undefined) {
                popularSubCategories = $(FormsEngine.DefaultFormTag).find('#popularSubCategories').attr('data-defaults').split(",");
            }
            var queryStringParam = fe_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fe_getDistinctList(idArray);
            }

            $.each(data.SubjectList, function (index, value) {
                var liElement = $(document.createElement('li'));
                var label = $("<label>").attr("for", controlCode + '_' + value.SubjectId);
                label.html(value.SubjectName);

                var checkbox = $(document.createElement('input')).attr({ id: controlCode + '_' + value.SubjectId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);
                if (idArrayDistinct != null &&
                    idArrayDistinct.length > 0 &&
                    $.inArray(value.SubjectId.toString(), queryStringParam) > -1 &&
                    $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').exists()) {
                    //we have values and one of them is this controls category so add it to our featured categories
                    $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').append(liElement);
                    if ($.inArray(value.SubjectId.toString(), popularSubCategories) > -1) {
                        //we have at least one popular category so hide the main categories and show the button
                        $(FormsEngine.DefaultFormTag).find('#showHideSubCategories').show();
                        $(FormsEngine.DefaultFormTag).find('#ulSubCategories').hide();
                    }
                }
                else if ($.inArray(value.SubjectId.toString(), popularSubCategories) > -1 && $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').exists()) {
                    //we have at least one popular category so hide the main categories and show the button
                    $(FormsEngine.DefaultFormTag).find('#showHideSubCategories').show();
                    $(FormsEngine.DefaultFormTag).find('#showHideSubCategories').unbind('click');
                    $(FormsEngine.DefaultFormTag).find('#showHideSubCategories').click(function () {
                        if ($(FormsEngine.DefaultFormTag).find('#ulSubCategories').is(":visible")) {
                            $(FormsEngine.DefaultFormTag).find('#ulSubCategories').hide();
                            $(this).html('See More Options');
                        }
                        else {
                            $(FormsEngine.DefaultFormTag).find('#ulSubCategories').show();
                            $(this).html('See Less Options');
                        }
                    });
                    $(FormsEngine.DefaultFormTag).find('#ulSubCategories').hide();
                    $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').append(liElement);
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").append(liElement);
                }
                checkbox.change(function (e) { changeSelectionCategorySubCategory(controlCode, e.target); });
                if ($.inArray(value.SubjectId.toString(), defaultSubCategories) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                    if ($.inArray(value.SubjectId.toString(), popularSubCategories) < 0) {
                        $(FormsEngine.DefaultFormTag).find('#ulSubCategories').show();
                        $(FormsEngine.DefaultFormTag).find('#showHideSubCategories').html('See Less Options');
                    }
                }
            });

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                assignCategorySubCategoryHiddenValues(controlCode, hiddenValControl);
            }

            //hide featured if there are none
            if ($(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').exists()) {
                var liCount = $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').children().size();
                if (liCount == null || liCount == undefined || liCount == 0) {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').hide();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-subcategories').show();
                }
            }
        }
        else {

            var hiddenValControl = $(FormsEngine.DefaultFormTag).find("input[name='Specialties_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultSpecialties = [];
            var popularSpecialties = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultSpecialties') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultSpecialties').attr('data-defaults') != undefined) {
                defaultSpecialties = $(FormsEngine.DefaultFormTag).find('#defaultSpecialties').attr('data-defaults').split(",");
            }
            var popularCategories = []
            if ($(FormsEngine.DefaultFormTag).find('#popularSpecialties') != undefined && $(FormsEngine.DefaultFormTag).find('#popularSpecialties').attr('data-defaults') != undefined) {
                popularSpecialties = $(FormsEngine.DefaultFormTag).find('#popularSpecialties').attr('data-defaults').split(",");
            }
            var queryStringParam = fe_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fe_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
            }

            $.each(data.SpecialtyList, function (index, value) {
                var liElement = $(document.createElement('li'));
                var label = $("<label>").attr("for", controlCode + '_' + value.SpecialtyId);
                label.html(value.SpecialtyName);

                var checkbox = $(document.createElement('input')).attr({ id: controlCode + '_' + value.SpecialtyId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);
                if (idArrayDistinct != null &&
                    idArrayDistinct.length > 0 &&
                    $.inArray(value.SpecialtyId.toString(), queryStringParam) > -1 &&
                    $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').exists()) {
                    //we have values and one of them is this controls category so add it to our featured categories
                    $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').append(liElement);
                    if ($.inArray(value.SpecialtyId.toString(), popularSubCategories) > -1) {
                        //we have at least one popular category so hide the main categories and show the button
                        $(FormsEngine.DefaultFormTag).find('#showHideSpecialties').show();
                        $(FormsEngine.DefaultFormTag).find('#ulSpecialties').hide();
                    }
                }
                else if ($.inArray(value.SpecialtyId.toString(), popularSpecialties) > -1 && $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').exists()) {
                    //we have at least one popular category so hide the main categories and show the button
                    $(FormsEngine.DefaultFormTag).find('#showHideSpecialties').show();
                    $(FormsEngine.DefaultFormTag).find('#showHideSpecialties').unbind('click');
                    $(FormsEngine.DefaultFormTag).find('#showHideSpecialties').click(function () {
                        if ($(FormsEngine.DefaultFormTag).find('#ulSpecialties').is(":visible")) {
                            $(FormsEngine.DefaultFormTag).find('#ulSpecialties').hide();
                            $(this).html('See More Options');
                        }
                        else {
                            $(FormsEngine.DefaultFormTag).find('#ulSpecialties').show();
                            $(this).html('See Less Options');
                        }
                    });
                    $(FormsEngine.DefaultFormTag).find('#ulSpecialties').hide();
                    $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').append(liElement);
                }
                else {
                    $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").append(liElement);
                }
                checkbox.change(function (e) { changeSelectionCategorySubCategory(controlCode, e.target); });
                if ($.inArray(value.SpecialtyId.toString(), defaultSpecialties) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                    if ($.inArray(value.SpecialtyId.toString(), popularSpecialties) < 0) {
                        $(FormsEngine.DefaultFormTag).find('#ulSpecialties').show();
                        $(FormsEngine.DefaultFormTag).find('#showHideSpecialties').html('See Less Options');
                    }
                }
            });

            if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                assignCategorySubCategoryHiddenValues(controlCode, hiddenValControl);
            }

            //hide featured if there are none
            if ($(FormsEngine.DefaultFormTag).find('ul.featured-specialties').exists()) {
                var liCount = $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').children().size();
                if (liCount == null || liCount == undefined || liCount == 0) {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').hide();
                }
                else {
                    $(FormsEngine.DefaultFormTag).find('ul.featured-specialties').show();
                }
            }
        }
        callback();
    }


    function buildMultiSelectDDLCategorySubCategoryControl(controlCode, data, callback) {
        //clear the select of options entirely.
        var hiddenValControl = $(FormsEngine.DefaultFormTag).find("input[name='" + controlCode + "_Selections']");
        var control = $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "']");

        $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "']").find('option').not('[value=""]').remove().end();

        if (controlCode == "Categories") {

            var defaultCategories = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultCategories').attr('data-defaults') != undefined) {
                defaultCategories = $(FormsEngine.DefaultFormTag).find('#defaultCategories').attr('data-defaults').split(",");
            }

            //for each returned category add the option to the select list
            $.each(data.CategoryList, function (index, value) {
                var defaultSelected = false;

                if ($.inArray(value.CategoryId.toString(), defaultCategories) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.CategoryName, value.CategoryId, defaultSelected, defaultSelected);
                $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });
        }
        else if (controlCode == "SubCategories") {

            var defaultSubCategories = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultSubCategories') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultSubCategories').attr('data-defaults') != undefined) {
                defaultSubCategories = $(FormsEngine.DefaultFormTag).find('#defaultSubCategories').attr('data-defaults').split(",");
            }

            //for each returned subject add the option to the select list
            $.each(data.SubjectList, function (index, value) {
                var defaultSelected = false;

                if ($.inArray(value.SubjectId.toString(), defaultSubCategories) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.SubjectName, value.SubjectId, defaultSelected, defaultSelected);
                $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });
        }
        else {

            var defaultSpecialties = [];
            if ($(FormsEngine.DefaultFormTag).find('#defaultSpecialties') != undefined && $(FormsEngine.DefaultFormTag).find('#defaultSpecialties').attr('data-defaults') != undefined) {
                defaultSpecialties = $(FormsEngine.DefaultFormTag).find('#defaultSpecialties').attr('data-defaults').split(",");
            }

            //for each returned specialty add the option to the select list
            $.each(data.SpecialtyList, function (index, value) {
                var defaultSelected = false;

                if ($.inArray(value.SpecialtyId.toString(), defaultSpecialties) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.SpecialtyName, value.SpecialtyId, defaultSelected, defaultSelected);
                $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });

        }

        if ($(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
            assignCategorySubCategoryMultiSelectHiddenValues(controlCode, hiddenValControl);
        }
        control.change(function (e) { changeMultiSelectSelectionCategorySubCategory(controlCode, e.target); });
        control.trigger('change');

        //if were here and the multiselect has no values clear the hidden input
        if (!$(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").val()) {
            $(hiddenValControl).val('');
            $(hiddenValControl).change();
        }

        $(FormsEngine).trigger("subcategoriesLoaded");
        callback();
    }


    function assignCategorySubCategoryMultiSelectHiddenValues(controlCode, hiddenValControl) {
        var idArray = "";
        if (hiddenValControl.val())
            idArray = hiddenValControl.val().split(',');
        var idArrayDistinct = fe_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
        $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").val(idArrayDistinct);
        $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").select2();
        // fe_consolelog(controlCode + ' ' + hiddenValControl.val());
    }

    function assignCategorySubCategoryHiddenValues(controlCode, hiddenValControl) {
        var idArray = hiddenValControl.val().split(',');
        var idArrayDistinct = fe_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes

        var populars = [];
        if ($(FormsEngine.DefaultFormTag).find('#popular' + controlCode) != undefined && $(FormsEngine.DefaultFormTag).find('#popular' + controlCode).attr('data-defaults') != undefined) {
            populars = $(FormsEngine.DefaultFormTag).find('#popular' + controlCode).attr('data-defaults').split(",");
        }
        $.each(idArrayDistinct, function (id) {
            var $checkbox = $(FormsEngine.DefaultFormTag).find('#' + controlCode + '_' + idArrayDistinct[id]);

            if ($checkbox.exists()) {
                $checkbox.prop('checked', true);
                $checkbox.trigger('change');
                if (populars.length > 0) {
                    if ($.inArray(idArrayDistinct[id], populars) < 0 &&
                        $(FormsEngine.DefaultFormTag).find('#ul' + controlCode).find($checkbox).length > 0) {
                        //we have a selected value thats not popular so we need to show the regular div IF IT IS STILL IN THE ORIGINAL DIV AND NOT FEATURED
                        $(FormsEngine.DefaultFormTag).find('#ul' + controlCode).show();
                    }
                }
                else {
                    $(FormsEngine.DefaultFormTag).find('#ul' + controlCode).show();
                }
            } else {
                removeValueFromCommaList(hiddenValControl, idArrayDistinct[id]);

                //********----#57653---*********//
                updateSelectionCategorySubCategorySpecialty(controlCode + '_' + idArrayDistinct[id], false);
                //********END -- #57653*********//
            }
        });

        // fe_consolelog(controlCode + ' ' + hiddenValControl.val());
    }


    //********----#57653---*********//
    function updateSelectionCategorySubCategorySpecialty(checkboxID, isAdd) {

        var checkboxDetails = checkboxID.split("_");
        var itemText = $("label[for=" + checkboxID + "]").text();
        var item = { "id": checkboxDetails[1], "text": itemText, "operation": isAdd, "type": checkboxDetails[0] };

        fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(item, false);
    }
    //********END -- #57653*********//


    function changeSelectionCategorySubCategory(controlcode, checkbox) {
        var hiddenInput = $(FormsEngine.DefaultFormTag).find("input[name='" + controlcode + "_Selections']");
        var hiddenInputValue = hiddenInput.val();
        var checkBoxValue = checkbox.id.split("_")[1];
        var changed = false;

        if ($(checkbox).is(":checked")) {
            var exists = $.inArray(checkBoxValue, $(hiddenInput).val().split(',')) != -1;
            if (!exists) {
                if (hiddenInputValue) {
                    hiddenInput.val(hiddenInputValue + "," + checkBoxValue);
                } else {
                    hiddenInput.val(checkBoxValue);
                }
                changed = true;
            }
        } else {
            removeValueFromCommaList(hiddenInput, checkBoxValue);
            changed = true;
        }
        if (changed && (FormsEngine.SelectAllTriggered == undefined || !FormsEngine.SelectAllTriggered)) {
            hiddenInput.change();
        }

        //fe_consolelog(hiddenInput.val());
        //********----#57653---*********//
        updateSelectionCategorySubCategorySpecialty(checkbox.id, $(checkbox).is(":checked"));
        //********END -- #57653*********//
    }

    function changeMultiSelectSelectionCategorySubCategory(controlCode, ddl) {
        var hiddenInput = $(FormsEngine.DefaultFormTag).find("input[name='" + controlCode + "_Selections']");
        var hiddenInputValue = "";
        if (hiddenInput.val())
            hiddenInputValue = hiddenInput.val();
        var changed = false;

        //for each selected value in the dropdown check if the option is in the dropdown and if its not remove from hidden control
        if ($(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").val() && $(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").val().length > 0) {
            $.each($(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").val(), function (index, value) {
                changed = false;
                if ($(FormsEngine.DefaultFormTag).find("select[code='" + controlCode + "'].multiselectdropdown").find('option[value="' + value + '"]')) {
                    var exists = $.inArray(value, hiddenInputValue.split(',')) != -1;
                    if (!exists) {
                        if (hiddenInputValue) {
                            hiddenInput.val(hiddenInputValue + "," + value);
                        } else {
                            hiddenInput.val(value);
                        }
                        changed = true;
                    }
                } else {
                    removeValueFromCommaList(hiddenInput, value);
                    changed = true;
                }

                if (changed && (FormsEngine.SelectAllTriggered == undefined || !FormsEngine.SelectAllTriggered)) {
                    hiddenInput.change();
                }
            });
        }

        //********END -- #57653*********//
    }

    function removeValueFromCommaList(hiddenValControl, intValue) {
        var hiddenInputValue = hiddenValControl.val();

        if (hiddenInputValue) {
            if (hiddenInputValue.indexOf(",") == -1) {
                hiddenValControl.val(hiddenInputValue.replace(intValue, ""));
            } else {
                if (hiddenInputValue.indexOf(intValue) == 0) {
                    if (hiddenInputValue.indexOf(",") == -1) {
                        hiddenValControl.val(hiddenInputValue.replace(intValue, ""));
                    } else {
                        hiddenValControl.val(hiddenInputValue.replace(intValue + ",", ""));
                    }
                } else {
                    hiddenValControl.val(hiddenInputValue.replace("," + intValue, ""));
                }
            }
        }
    }

    function cleanCSVIntArray(ControlValue) {
        var CleanArray = [];

        if (ControlValue != undefined && ControlValue != "") {
            var IntArray = ControlValue.split(",");

            for (var i = 0; i < IntArray.length; i++) {
                var Item = parseInt(IntArray[i], 10);
                if (Item >= 0) {
                    CleanArray.push(Item);
                }
            }
        }
        return CleanArray.join(',');
    }


    //Recovers form from data string
    function recoverForm(data, callback) {

        FormsEngine.StringToRecover = data;
        FormsEngine.RecoveryMode = true;

        //Categories recovery
        var ControlValue = fe_getParameterByNameFromString('Categories_Selections', FormsEngine.StringToRecover);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fe_getParameterByNameFromString('Categories', FormsEngine.StringToRecover)
        }
        $(FormsEngine.DefaultFormTag).find("input[name='Categories_Selections']").val(cleanCSVIntArray(ControlValue));

        //SubCategories recovery
        ControlValue = fe_getParameterByNameFromString('SubCategories_Selections', FormsEngine.StringToRecover);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fe_getParameterByNameFromString('SubCategories', FormsEngine.StringToRecover)
        }
        $(FormsEngine.DefaultFormTag).find("input[name='SubCategories_Selections']").val(cleanCSVIntArray(ControlValue));

        //Specialties recovery
        ControlValue = fe_getParameterByNameFromString('Specialties_Selections', FormsEngine.StringToRecover);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fe_getParameterByNameFromString('Specialties', FormsEngine.StringToRecover)
        }
        $(FormsEngine.DefaultFormTag).find("input[name='Specialties_Selections']").val(cleanCSVIntArray(ControlValue));

        checkControlDataBind(0, callback);
    }

    //loads form merging fields from querystring/session to avoid double binding
    function loadFormQuick(callback) {

        //1. Get Session
        fe_getSessionObject("WFORM", function (data) {

            var FormSession = {};
            var FormQueryString = {};
            var ConsolidatedArgs = {};
            var ConsolidatedArgsArray = [];

            if (data != undefined && data != null && data.length > 0) {
                FormSession = fe_getUriToObject(data.toString());
                //will disable auto advance forward if form was recovered from session
                FormsEngine.FormsHasBeenRecovered = true;
            }

            // Prepopulate values
            // International Landers Country..
            var $countryDDL = $(FormsEngine.DefaultFormTag).find("[data-controlcode=Country] select");
            if (FormsEngine.InternationalCountryCode && $countryDDL.length > 0) {
                $countryDDL.val(FormsEngine.InternationalCountryCode);
                fe_ApplyPhoneMask();
            }

            //2. Get Querystring
            var dataQS = fe_getQuerystring();
            if (dataQS != undefined && dataQS.length > 0) {
                FormQueryString = fe_getUriToObject(dataQS);
            }


            //3. Merge fields precedence to session
            //3.1 Session
            for (var property in FormSession) {
                if (!ConsolidatedArgs[property] && property != undefined) {
                    if (FormSession[property] == "" && FormQueryString[property]) {
                        ConsolidatedArgs[property] = FormQueryString[property];
                    }
                    else {
                        ConsolidatedArgs[property] = FormSession[property];
                    }
                }
            }
            //3.2 Querystring values only
            for (var property in FormQueryString) {
                if (!ConsolidatedArgs[property] && property != undefined) {
                    ConsolidatedArgs[property] = FormQueryString[property];
                }
            }

            //3.3 pass thru
            if (FormsEngine.PassThruItems) {
                jQuery.each(FormsEngine.PassThruItems, function (index, pi) {
                    if (!ConsolidatedArgs[pi.QuestionName] && pi != undefined) {
                        ConsolidatedArgs[pi.QuestionName] = pi.Answers;
                    }
                });
            }

            //4.Consolidate values
            for (var property in ConsolidatedArgs) {
                if (property != undefined) {
                    ConsolidatedArgsArray.push(property + '=' + ConsolidatedArgs[property]);
                }
            }

            fe_consolelog("LOADING FORM FROM MERGE SESSION-QUERYSTRING");

            //5.Recover form
            recoverForm(ConsolidatedArgsArray.join('&'), callback);
        });

    }

    //loads form from session
    function loadFormFromSession(callback) {
        fe_getSessionObject("WFORM", function (data) {
            if (data != undefined && data != null && data.length > 1) {
                FormsEngine.FormsHasBeenRecovered = true;
            }
            fe_consolelog("LOADING FORM FROM SESSION");
            recoverForm(data, callback);
        });
    }

    //loads from values from querystring
    function loadFormFromQuerystring(callback) {
        var data = fe_getQuerystring();
        if (data != undefined && data.length > 0) {
            fe_consolelog("LOADING FORM FROM QUERYSTRING");
            recoverForm(data, callback);
        }
        else {
            if (callback) {
                callback();
            }
        }
    }

    //Loads values from data array
    function loadFormFromPassThrough(callback) {
        var Data = '';
        var FirstQuestion = true;

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
            for (var Value = 0; Value < Values.length; Value++) {
                if (FirstData == true) {
                    Data = Data + encodeURIComponent(Values[Value]);
                    FirstData = false;
                }
                else {
                    Data = Data + "," + encodeURIComponent(Values[Value]);
                }
            }
        }

        if (Data != undefined && Data.length > 0) {
            fe_consolelog("LOADING FORM FROM PASSTHROUGH");
            recoverForm(Data, function () {
                //Navigate To Step
                if (FormsEngine.NavigateToStep != undefined) {
                    FormsEngine.MoveStep(FormsEngine.NavigateToStep);
                }

                //Callback
                if (callback) {
                    callback();
                }
            });
        }
        else {
            if (callback) {
                callback();
            }
        }
    }


    //Finds the control index in the question array
    function findControlIndex(ControlCode, ControlId) {
        var ControlIndex = -1;
        for (var index = 0; index < FormsEngine.Questions.length; index++) {
            if (typeof ControlId != 'undefined' && FormsEngine.Questions[index].TemplateControlId == ControlId) {
                ControlIndex = index;
                break;
            }
            else if (typeof ControlId == 'undefined' && FormsEngine.Questions[index].Code == ControlCode) {
                ControlIndex = index;
                break;
            }
        }
        return ControlIndex;
    }


    //Gets the filters values for the question based on the previous controls values that are filters
    function findQuestionFilterValues(index, Question) {
        var FilterValues = [];
        var QuestionFilters = Question.Filters.slice(0);

        //Campus special FE.Property is always a filter for the control if the control is not present
        if (QuestionFilters.indexOf('Campus') > -1 && FormsEngine.CampusId > 0) {

            if (jQuery(FormsEngine.DefaultFormTag).find(":input[code='Campus']").length === 0) {
                var Filter = {};
                Filter.Key = 'Campus';
                Filter.Value = FormsEngine.CampusId;
                FilterValues.push(Filter.Key + "=" + Filter.Value);
            }

        }

        //Search in the previous controls to the question
        for (var i = 0; i < index; i++) {
            //For each filter
            for (var filterIndex = 0; filterIndex < QuestionFilters.length; filterIndex++) {
                //filter equals to the control
                if (FormsEngine.Questions[i].Code == QuestionFilters[filterIndex]) {

                    //Gets control value
                    var ControlValue = fe_getControlValue(FormsEngine.Questions[i].Code);

                    if (ControlValue.value != "") {
                        var Filter = {};
                        Filter.Key = FormsEngine.Questions[i].Code;
                        Filter.Value = ControlValue.value;
                        FilterValues.push(Filter.Key + "=" + Filter.Value);
                    }
                    if (ControlValue.requiresKey && ControlValue.valueKey != "") {
                        var Filter = {};
                        Filter.Key = FormsEngine.Questions[i].Code;
                        Filter.Value = ControlValue.valueKey;
                        FilterValues.push(Filter.Key + "-key=" + Filter.Value);
                    }

                    //Remove filter found
                    QuestionFilters.splice(filterIndex, 1);
                    break;
                }
            }
        }
        //Append Country as a filter
        if (Question.Code == "State" && FormsEngine.SpecialCountryBackRebindState) {
            var ControlValue = fe_getControlValue(FormsEngine.Questions[FormsEngine.CountryControlIndex].Code);
            if (ControlValue.value != "") {
                var Filter = {};
                Filter.Key = FormsEngine.Questions[FormsEngine.CountryControlIndex].Code;
                Filter.Value = ControlValue.value;
                FilterValues.push(Filter.Key + "=" + Filter.Value);
            }
            if (ControlValue.requiresKey && ControlValue.valueKey != "") {
                var Filter = {};
                Filter.Key = FormsEngine.Questions[FormsEngine.CountryControlIndex].Code;
                Filter.Value = ControlValue.valueKey;
                FilterValues.push(Filter.Key + "-key=" + Filter.Value);
            }
        }
        return FilterValues.join('&');
    }


    //Prints function name dynamically invoked
    function functionName(fn) {
        try {
            var name = /\W*function\s+([\w\$]+)\(/.exec(fn);
            if (!name) return 'NoName';
            return name[1];
        }
        catch (e) {
            return 'NoName';
        }
    }

    function initializeControl(index) {
        var ControlValue = fe_getParameterByNameAndAliasFromString(FormsEngine.Questions[index].Code, FormsEngine.StringToRecover);

        if (FormsEngine.Questions[index].Code == 'Campus') {
            ControlValue = FormsEngine.CampusId;
        }

        if (ControlValue != "") {
            var localCountry = true;

            if (FormsEngine.Questions[index].Code == 'Program_Of_Interest' && FormsEngine.ProgramId != undefined && FormsEngine.ProgramId != "") {
                ControlValue = FormsEngine.ProgramId;
            }
            else if (FormsEngine.Questions[index].Code == 'Country') {
                localCountry = ["US", "CA", ""].indexOf(ControlValue.toUpperCase()) >= 0;
            }

            if (FormsEngine.Questions[index].Code == 'Google_address') {
                var additionalFields = ['Address', 'City', 'State', 'Postal_Code_Duplicate', 'Country'];
                for (var i = 0; i < additionalFields.length; i++) {
                    var additionalControlValue = fe_getParameterByNameAndAliasFromString(additionalFields[i], FormsEngine.StringToRecover);
                    if (additionalControlValue != "")
                        fe_setControlValue(additionalFields[i], additionalControlValue);
                }
            }


            fe_setControlValue(FormsEngine.Questions[index].Code, ControlValue);
            if (FormsEngine.Questions[index].Code == 'Program_Of_Interest') {
                var ProgramDDL = $(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
                var ProgramId = $(ProgramDDL).val();
                ProgramId = ProgramId == undefined ? "" : ProgramId;
                if (ProgramId != undefined && ProgramId != "") {
                    var ProgramProductId = $(ProgramDDL).attr('data-programproductid');
                    var ProgramTemplateId = $(ProgramDDL).attr('data-templateid');
                    var ProductId = $(ProgramDDL).attr('data-productid');
                    var PaidStatusTypeId = $(ProgramDDL).attr('data-paidstatustypeid');

                    FormsEngine.ProgramId = ProgramId;
                    FormsEngine.ProgramProductId = ProgramProductId;
                    FormsEngine.ProgramName = $(ProgramDDL).text();
                    FormsEngine.ProductId = ProductId;

                    $(FormsEngine).trigger("OnProgramSet");
                }

            }
            else if (FormsEngine.Questions[index].Code == 'Country') {
                if (!localCountry) {
                    fe_ApplyPhoneMask();
                }
            }
        }
        //select2 start

        //select2 end
        //select2 start
        if (typeof $().select2 == "function") {
            var $selectControl = $(FormsEngine.DefaultFormTag).find("select.select2[code='" + FormsEngine.Questions[index].Code + "']");
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
                                placeholder: $selectControl.find("option[value='']").eq(0).text()
                            });
                        }
                        else if ($selectControl.siblings("label.inline-hidden-label").length > 0) {
                            $selectControl.select2({
                                placeholder: $selectControl.siblings("label.inline-hidden-label").eq(0).text()
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
                                createTag: function (params) {
                                    var term = $.trim(params.term);

                                    if (term === '') {
                                        return null;
                                    }

                                    return {
                                        id: term,
                                        text: term,
                                        key: term // add additional parameters
                                    }
                                },
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
                                        fe_consolelog(data);
                                        return {
                                            results: data
                                        }
                                    },

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

                                        return { results: data }
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
                        if ($selectControl.siblings("label.inline-hidden-label").length > 0) {
                            $selectControl.select2({
                                minimumResultsForSearch: Infinity,
                                placeholder: $selectControl.siblings("label.inline-hidden-label").eq(0).text()
                            });
                        }
                    }
                }
                else {
                    $selectControl.select2({
                        minimumResultsForSearch: Infinity
                    });
                }


                if ($selectControl.hasClass('multiselectdropdown')) {
                    $selectControl.on('select2:unselect', function (e) {
                        //if we deselected from a multiselect dropdown we need to make sure the hidden inputs are updated properly
                        var hiddenInput = $(FormsEngine.DefaultFormTag).find("input[name='" + $selectControl.attr('code') + "_Selections']");
                        if (hiddenInput) {
                            hiddenInput.val($selectControl.val());
                            hiddenInput.change();
                        }
                    });
                }
            }
            // if just select2
            else {
                $selectControl.select2({
                    placeholder: $selectControl.attr("placeholder"),
                    allowClear: true
                });
            }
        }
        //select2 end


        FormsEngine.Questions[index].Initialized = true;

    }

    //Recursive data bind helper v2.0 (within step only)
    function checkControlDataBind(index, callback) {

        //fe_consolelog("checkControlDataBind("+index.toString()+") FormsEngine.RecoveryMode=" + FormsEngine.RecoveryMode);
        if (FormsEngine.RecoveryMode == true && FormsEngine.Questions[index].LastQuestionFromStep) {
            FormsEngine.RecoveryMode = false;
            if (FormsEngine.OnFormLoad) {
                FormsEngine.OnFormLoad();
            }
            if (callback) {
                callback();
            }
        }

        if (index >= 0 && index < FormsEngine.Questions.length) {
            //On recovery mode set field value and proceed when question has not been initialized
            if (FormsEngine.Questions[index].Initialized == false && FormsEngine.Questions[index].DataBind == undefined) {
                initializeControl(index);
            }

            //If control requires a data bind find filter values on previous controls
            if (FormsEngine.Questions[index].DataBind != undefined) {
                var FilterValues = findQuestionFilterValues(index, FormsEngine.Questions[index]);

                //Bind data
                if (FormsEngine.Questions[index].LastDataBindFilters != FilterValues) {
                    //fe_consolelog('checkControlDataBind=' + functionName(FormsEngine.Questions[index].DataBind) + "(" + FilterValues + ")");
                    FormsEngine.Questions[index].LastDataBindFilters = FilterValues;
                    FormsEngine.Questions[index].DataBind(FilterValues, function () {
                        if (FormsEngine.Questions[index].Initialized == false) {
                            initializeControl(index);
                        }
                        if (!FormsEngine.Questions[index].LastQuestionFromStep) {
                            checkControlDataBind(index + 1, callback);
                        }
                        else {
                            validateStateCountry();
                        }
                    });
                }
                else {
                    if (!FormsEngine.Questions[index].LastQuestionFromStep) {
                        checkControlDataBind(index + 1, callback);
                    }
                    else {
                        validateStateCountry();
                    }
                }


            }
            else {
                if (!FormsEngine.Questions[index].LastQuestionFromStep) {
                    checkControlDataBind(index + 1, callback);
                }
                else {
                    validateStateCountry();
                }
            }
        }
    }

    //Starts cascading check of controls, starting by the control down to the last
    function checkDependencies(control, processNextStep) {
        //Dynamic filters sample
        //FormsEngine.Questions = [];

        //Question = {};
        //Question.Code = 'City';
        //Question.TemplateControlId = 2325;
        //Question.Step = 5;
        //Question.Filters = ['Country', 'Postal_Code', 'State'];
        //Question.DataBind = 'bindCity';
        //Question.LastDataBindFilters = '';
        //Question.LastQuestionFromStep = true;
        //Question.Initialized = false;
        //FormsEngine.Questions.push(Question)

        processNextStep = processNextStep == undefined ? false : processNextStep;

        if (FormsEngine.SelectAllTriggered != true) {
            var ControlCode = $(control).attr("code");
            if (ControlCode === undefined) {
                switch ($(control).attr("type")) {
                    case 'radio':
                        ControlCode = $(control).attr('parent-code');

                        break;
                }

            }
            var ControlValue = fe_getControlValue(ControlCode).value;
            var ControlIndex = 0;

           
            if (ControlCode == "Country" && FormsEngine.SpecialCountryBackRebindState
                && FormsEngine.LastCountryBinded != ControlValue) {
                FormsEngine.LastCountryBinded = ControlValue;
                ControlIndex = findControlIndex("State");
                //State has to be rebinded and subsequent controls
                ControlIndex--;
            }
            else { //normal bind
                if (ControlCode == 'LBL') {
                    ControlIndex = findControlIndex(ControlCode, $(control).attr("id"));
                }
                else {
                    ControlIndex = findControlIndex(ControlCode);
                }
            }

            //Not next step and whithin the step databind next control
            if (!processNextStep && ControlIndex + 1 < FormsEngine.Questions.length && FormsEngine.Questions[ControlIndex].Step == FormsEngine.Questions[ControlIndex + 1].Step) {
                //Next control is the first one to be checked for data bind{
                ControlIndex++;
                checkControlDataBind(ControlIndex);
            }
            // next step
            else if (processNextStep && ControlIndex + 1 < FormsEngine.Questions.length) {
                //Next control is the first one to be checked for data bind{
                ControlIndex++;
                checkControlDataBind(ControlIndex);
            }

        }

    }


    //Bind events to controls
    function bindEventsToControls() {
        for (var item = 0; item < FormsEngine.Questions.length; item++) {
            var Control = $("[code='" + FormsEngine.Questions[item].Code + "']");

            if ($(Control).is(':radio')) {
                $(Control).click(function () { checkDependencies($(this)); });
            }
            else if ($(Control).is(':checkbox')) {
                $(Control).change(function () { checkDependencies($(this)); });
            }
            else if ($(Control).is('input:text')) {
                $(Control).blur(function () { checkDependencies($(this)); });
            }
            else if ($(Control).is('select')) {
                $(Control).change(function () { checkDependencies($(this)); });
            }
            else {
                $(Control).blur(function () { checkDependencies($(this)); });
            }
        }

        //Bind Categories hidden field
        if ($(FormsEngine.DefaultFormTag).find("input[name='Categories_Selections']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[name='Categories_Selections']").change(function () { checkDependencies($(this)); });
        }

        //Bind SubCategories hidden field
        if ($(FormsEngine.DefaultFormTag).find("input[name='SubCategories_Selections']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[name='SubCategories_Selections']").change(function () { checkDependencies($(this)); });
        }

        //Bind SubCategories hidden field
        if ($(FormsEngine.DefaultFormTag).find("input[name='Specialties_Selections']").exists()) {
            $(FormsEngine.DefaultFormTag).find("input[name='Specialties_Selections']").change(function () { checkDependencies($(this)); });
        }
    }

    //Forces state and country to get revalidated after a control binds within the step
    function validateStateCountry() {
        //fe_consolelog("VALIDATING STATE COUNTRY WITHIN STEP");
        var control = $("#Step" + FormsEngine.CurrentStep).find('select[code="Country"]');
        if ($(control).length > 0 && $(control).val()) {
            if ($(control).valid()) {
                $(control).removeClass('error');
            }
        }
        control = $("#Step" + FormsEngine.CurrentStep).find('select[code="State"]');
        if ($(control).length > 0 && $(control).val()) {
            if ($(control).valid()) {
                $(control).removeClass('error');
            }
        }
    }

    function buildMultiCheckBox(controlCode, data, callback) {
        $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']").empty();
        var theUl = $(FormsEngine.DefaultFormTag).find("ul[code='" + controlCode + "']");

        $.each(data, function (index, value) {
            var item = (value.Key).replace(/[_\W]+/g, "-").toLowerCase();
            var liElement = $(document.createElement('li'));
            var label = $("<label>").attr({ 'for': controlCode + '_' + value.Value });
            label.html(value.Text);

            var checkbox = $(document.createElement('input')).attr({ id: controlCode + '_' + value.Value, type: 'checkbox', name: $(theUl).attr('name'), 'id-sort': $(theUl).attr('id-sort'), novalidate: 'true', controltypename: $(theUl).attr('controltypename'), class: 'checkbox-field', code: controlCode, key: value.Key, value: value.Value });
            liElement.append(checkbox);
            liElement.append(label);
            $(theUl).append(liElement);
            checkbox.change(function () { checkDependencies($(this)); });
        });

        callback();
    }

    function loadOptionsIntoDropDownControl(control, data) {
        var previousValue = control.val();
        control.empty();

        if ($(control).hasClass("inlineDropDown")) {
            makeInlineDropDown(control);
        }
        else {
            control.append(jQuery('<option/>', {
                value: "",
                text: FormsEngine.DefaultSelectText
            }));
        }

        if (data.length > 0) {
            jQuery.each(data, function (index, item) {
                if (item.Selected == true) {
                    control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key, 'selected': ' ' }));
                }
                else {
                    control.append(jQuery('<option/>', { 'value': item.Value, 'text': item.Text, 'key': item.Key }));
                }
            });
        }

        if (previousValue != "") {
            control.val(previousValue);
        }
    }


    $(document).ready(function () {

        //fe_debugShowAllSteps();

        //DO NOT REMOVE
        //@@DYNAMIC_FILTERS@@
        //DO NOT REMOVE

        //Bind Events
        bindEventsToControls();

        //Load Form Quick
        FormsEngine.LoadFormQuick = loadFormQuick;

        //Load Form from session
        FormsEngine.LoadFormFromSession = loadFormFromSession;

        //Load Form from querystring
        FormsEngine.LoadFormFromQuerystring = loadFormFromQuerystring;

        //Load Form from passthrough
        FormsEngine.LoadFormFromPassThrough = loadFormFromPassThrough;

        FormsEngine.CheckControlDataBind = FormsEngine.CheckControlDataBind || function (index) { checkControlDataBind(index) };

        //Global shared to trigger check dependencies for a particular control and after.
        FormsEngine.CheckDependencies = checkDependencies;


        //If Country is on the same step as State and Country is after rebind up on country change
        FormsEngine.CountryControlIndex = findControlIndex('Country');
        FormsEngine.StateControlIndex = findControlIndex('State');
        FormsEngine.SpecialCountryBackRebindState = false;
        if (FormsEngine.CountryControlIndex > -1
            && FormsEngine.StateControlIndex > -1
            && FormsEngine.CountryControlIndex > FormsEngine.StateControlIndex
            && FormsEngine.Questions[FormsEngine.StateControlIndex].Step == FormsEngine.Questions[FormsEngine.CountryControlIndex].Step) {

            FormsEngine.SpecialCountryBackRebindState = true;
        }

    });

    function makeInlineDropDown(control) {
        var $label = $(FormsEngine.DefaultFormTag).find("label[for=" + $(control).prop("id") + "]");

        control.append(jQuery('<option/>', {
            disabled: true,
            selected: true,
            value: "",
            text: ($label.length > 0) ? $label.text() : FormsEngine.DefaultSelectText
        }));
    }
})(jQuery);