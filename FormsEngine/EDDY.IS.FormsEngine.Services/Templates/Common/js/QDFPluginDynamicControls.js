//# sourceURL=QDFPluginDynamicControls.js

    //ProgramLevels DataBind
function qpd_bindDesiredDegreeLevel(formObj, pluginSettings, filters, callback) {
        qpd_getProgramLevels(formObj, pluginSettings, filters, callback, function (data) {
            var control = jQuery(formObj).find('select[code="Desired_Degree_Level"]');
            qpd_loadOptionsIntoDropDownControl(formObj, pluginSettings, control, data);
        });
        
    }


    function qpd_getProgramLevels(formObj, pluginSettings, filters, callback, qpd_loadResults) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";
        //Required arguments
        filters += prefix + "TrackId=" + pluginSettings.trackId + "&IsBeta=" + pluginSettings.isBeta;
        filters += "&ApplicationId=" + pluginSettings.applicationId;
        DataBindFilter.FilterString = filters;
        var sUrl = pluginSettings.serviceUrl + "/DataBind/GetProgramLevels";

        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                qpd_loadResults(data);
            },
            error: function (request, textStatus, errorThrown) {
                fep_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fep_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    }

function qpd_setDynamicCampusSoftPreferenceControl(formObj, pluginSettings, data) {

    var control = jQuery(formObj).find('select[code="DynamicCampusSoftPreference"]');
    var label = jQuery(formObj).find("label[for='" + jQuery(control).attr('id') + "']");
        var name = jQuery(control).attr('name');
        var transformControl = jQuery(control).attr("data-transform") == 'True';
        var mobileControls = [];
        var isVisible = false;

        if (name.indexOf('Alternate') >= 0) {

            jQuery(control).parent().hide();

            if (data.length > 0) {

                jQuery(control).find('option').hide();
                if (data.length > 1) {

                    var selectedItem = jQuery.grep(jQuery(data), function (item) { return item.Text == jQuery(control).find('option:selected').text(); });
                    if (selectedItem.length == 0) jQuery(control).val('');

                    jQuery.each(data, function (index, item) {

                        var itemOption = jQuery.grep(jQuery(control).find('option'), function (itemInner) {
                            return itemInner.text == item.Value;
                        });
                        if (itemOption.length > 0) {

                            jQuery(itemOption).attr('key', item.Key);
                            jQuery(itemOption).val(item.Value);
                            jQuery(itemOption).show();
                        }
                    });
                    jQuery(control).parent().show();
                }
                else {

                    jQuery(control).val('');
                    var itemOption = jQuery.grep(jQuery(control).find('option'), function (itemInner) {
                        return itemInner.text == data[0].Value;
                    });
                    if (itemOption.length > 0) {

                        jQuery(itemOption).attr('key', data[0].Key);
                        jQuery(itemOption).val(data[0].Value);
                        jQuery(control).val(jQuery(itemOption).val());
                        jQuery(itemOption).show();
                    }
                }
            }
            else jQuery(control).val('');
        }
        else { //Regular Dynamic Control

            if (jQuery(control).hasClass("inlineDropDown")) {
                control.empty();
                qpd_makeInlineDropDown(control);
            }
            else {
                control.find('option').not('[value=""]').remove().end();
                
            }

            if (data.length > 0) {
                //visible
                if (data.length > 1) {
                    isVisible = true;
                    jQuery(control).parent().show();
                }
                else { //hidden
                    isVisible = false;
                    jQuery(control).parent().hide();
                }

                jQuery.each(data, function (index, item) {
                    if (item.Selected == true) {
                        if (!transformControl && jQuery(control).hasClass("inlineDropDown")) {
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

            }
            //end of regular dynamic control
        }
    }

    //Dynamic Campus Soft Preference DataBind
function qpd_bindDynamicCampusSoftPreference(formObj, pluginSettings,filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
    filters += prefix + "TrackId=" + pluginSettings.trackId + "&IsBeta=" + pluginSettings.isBeta;
    filters += "&ApplicationId=" + pluginSettings.applicationId;
        //filters += "&DefaultCampusPreference=" + FormsEngine.DefaultCampusPreference;
        DataBindFilter.FilterString = filters;
    var sUrl = pluginSettings.serviceUrl + "/DataBind/GetCampusTypes";

        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                qpd_setDynamicCampusSoftPreferenceControl(data);
            },
            error: function (request, textStatus, errorThrown) {
                fep_consolelog(formObj, pluginSettings, arguments + "\n" + " Error: " + request.responseText);
                fep_logClientException(formObj, pluginSettings, request, sUrl, errorThrown);
            }
        }).done(callback);
    }

    //Categories DataBind
function qpd_bindCategories(formObj, pluginSettings, filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
    filters += prefix + "TrackId=" + pluginSettings.trackId + "&IsBeta=" + pluginSettings.isBeta;
    filters += "&ApplicationId=" + pluginSettings.applicationId;
        DataBindFilter.FilterString = filters;

    qpd_addCategorySubCategoryLoader(formObj, pluginSettings,"Categories");
    var sUrl = pluginSettings.serviceUrl + "/DataBind/GetCategories";
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if (jQuery(formObj).find("select[code='Categories']").length > 0) {
                    qpd_buildMultiSelectDDLCategorySubCategoryControl(formObj, pluginSettings,"Categories", data, callback);
                }
                else {
                    qpd_buildCategorySubCategoryControl(formObj, pluginSettings,"Categories", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fep_consolelog(formObj, pluginSettings, arguments + "\n" + " Error: " + request.responseText);
                fep_logClientException(formObj, pluginSettings, request, sUrl, errorThrown);
                callback();
            }
        });
    }

    //SubCategories DataBind
function qpd_bindSubCategories(formObj, pluginSettings, filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
    filters += prefix + "TrackId=" + pluginSettings.trackId + "&IsBeta=" + pluginSettings.isBeta;
    filters += "&ApplicationId=" + pluginSettings.applicationId;
        DataBindFilter.FilterString = filters;

    qpd_addCategorySubCategoryLoader(formObj, pluginSettings,"SubCategories");
    var sUrl = pluginSettings.serviceUrl + "/DataBind/GetSubCategories";
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if (jQuery(formObj).find("select[code='SubCategories']").length > 0) {
                    qpd_buildMultiSelectDDLCategorySubCategoryControl(formObj, pluginSettings, "SubCategories", data, callback);
                }
                else {
                    qpd_buildCategorySubCategoryControl(formObj, pluginSettings,"SubCategories", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fep_consolelog(formObj, pluginSettings,arguments + "\n" + " Error: " + request.responseText);
                fep_logClientException(formObj, pluginSettings,request, sUrl, errorThrown);
                callback();
            }
        });
    }

    //SubCategories DataBind
function qpd_bindSpecialties(formObj, pluginSettings,filters, callback) {
        var DataBindFilter = {};
        var prefix = filters == "" || filters == undefined ? "" : "&";

        //Required arguments
    filters += prefix + "TrackId=" + pluginSettings.trackId + "&IsBeta=" + pluginSettings.isBeta;
    filters += "&ApplicationId=" + pluginSettings.applicationId;
        DataBindFilter.FilterString = filters;

    qpd_addCategorySubCategoryLoader(formObj, pluginSettings,"Specialties");
    var sUrl = pluginSettings.serviceUrl + "/DataBind/GetSpecialties";
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: DataBindFilter,
            cache: false,
            url: sUrl,
            success: function (data) {
                if (jQuery(formObj).find("select[code='Specialties']").length > 0) {
                    qpd_buildMultiSelectDDLCategorySubCategoryControl(formObj, pluginSettings,"Specialties", data, callback);
                }
                else {
                    qpd_buildCategorySubCategoryControl(formObj, pluginSettings,"Specialties", data, callback);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fep_consolelog(formObj, pluginSettings,arguments + "\n" + " Error: " + request.responseText);
                fep_logClientException(formObj, pluginSettings,request, sUrl, errorThrown);
                callback();
            }
        });
    }


function qpd_addCategorySubCategoryLoader(formObj, pluginSettings,controlCode) {
    jQuery(formObj).find("ul[code='" + controlCode + "']").empty();
    if (jQuery(formObj).find("ul[code='Featured-" + controlCode + "']").exists()) {
        jQuery(formObj).find("ul[code='Featured-" + controlCode + "']").empty(); //clear featured placeholder also
        }
    jQuery(formObj).find("ul[code='" + controlCode + "']").append('<li>Loading...</li>');

    }

function qpd_buildCategorySubCategoryControl(formObj, pluginSettings,controlCode, data, callback) {
        jQuery(formObj).find("ul[code='" + controlCode + "']").empty();

        if (controlCode == "Categories") {

            var hiddenValControl = jQuery(formObj).find("input[name='Categories_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultCategories = [];
            if (jQuery(formObj).find('#defaultCategories') != undefined && jQuery(formObj).find('#defaultCategories').attr('data-defaults') != undefined) {
                defaultCategories = jQuery(formObj).find('#defaultCategories').attr('data-defaults').split(",");
            }
            var queryStringParam = fep_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fep_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
            }

            jQuery.each(data.CategoryList, function (index, value) {
                var category = (value.CategoryName).replace(/[_\W]+/g, "-").toLowerCase();
                var liElement = jQuery(document.createElement('li'));
                liElement.attr({ 'class': category });
                var label = jQuery("<label>").attr({ 'for': controlCode + '_' + value.CategoryId, 'class': category });
                label.html(value.CategoryName);

                var checkbox = jQuery(document.createElement('input')).attr({ id: controlCode + '_' + value.CategoryId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);
                jQuery(formObj).find("ul[code='" + controlCode + "']").append(liElement);
                checkbox.change(function (e) { qpd_changeSelectionCategorySubCategory(formObj, pluginSettings,controlCode, e.target); });
                if (jQuery.inArray(value.CategoryId.toString(), defaultCategories) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                }
            });

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                qpd_assignCategorySubCategoryHiddenValues(formObj, pluginSettings,controlCode, hiddenValControl);
            }

            if (jQuery(formObj).find('#showHideCategories').exists()) {
                jQuery(formObj).find('#showHideCategories').unbind('click');
                jQuery(formObj).find('#showHideCategories').click(function () {
                    if (jQuery(formObj).find('#ulCategories').is(":visible")) {
                        jQuery(formObj).find('#ulCategories').hide();
                        jQuery(this).html('See More Options');
                        jQuery(this).html('See Less Options');
                    }
                    else {
                        jQuery(formObj).find('#ulCategories').show();
                    }
                });
            }
        }
        else if (controlCode == "SubCategories") {
            var hiddenValControl = jQuery(formObj).find("input[name='SubCategories_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultSubCategories = [];
            if (jQuery(formObj).find('#defaultSubCategories') != undefined && jQuery(formObj).find('#defaultSubCategories').attr('data-defaults') != undefined) {
                defaultSubCategories = jQuery(formObj).find('#defaultSubCategories').attr('data-defaults').split(",");
            }
            var queryStringParam = fep_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fep_getDistinctList(idArray);
            }

            jQuery.each(data.SubjectList, function (index, value) {
                var liElement = jQuery(document.createElement('li'));
                var label = jQuery("<label>").attr("for", controlCode + '_' + value.SubjectId);
                label.html(value.SubjectName);

                var checkbox = jQuery(document.createElement('input')).attr({ id: controlCode + '_' + value.SubjectId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);

                jQuery(formObj).find("ul[code='" + controlCode + "']").append(liElement);

                checkbox.change(function (e) { qpd_changeSelectionCategorySubCategory(formObj, pluginSettings, controlCode, e.target); });
                if (jQuery.inArray(value.SubjectId.toString(), defaultSubCategories) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                }
            });

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                qpd_assignCategorySubCategoryHiddenValues(formObj, pluginSettings,controlCode, hiddenValControl);
            }

        }
        else {

            var hiddenValControl = jQuery(formObj).find("input[name='Specialties_Selections']");
            var idArray = null;
            var idArrayDistinct = null;
            var defaultSpecialties = [];
            if (jQuery(formObj).find('#defaultSpecialties') != undefined && jQuery(formObj).find('#defaultSpecialties').attr('data-defaults') != undefined) {
                defaultSpecialties = jQuery(formObj).find('#defaultSpecialties').attr('data-defaults').split(",");
            }
            var queryStringParam = fep_getParameterByName(controlCode);
            if (queryStringParam != null && queryStringParam != undefined && queryStringParam != "") {
                queryStringParam = queryStringParam.split(',');
            }
            else {
                queryStringParam = [];
            }

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                idArray = hiddenValControl.val().split(',');
                idArrayDistinct = fep_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
            }

            jQuery.each(data.SpecialtyList, function (index, value) {
                var liElement = jQuery(document.createElement('li'));
                var label = jQuery("<label>").attr("for", controlCode + '_' + value.SpecialtyId);
                label.html(value.SpecialtyName);

                var checkbox = jQuery(document.createElement('input')).attr({ id: controlCode + '_' + value.SpecialtyId, type: 'checkbox', novalidate: 'true' });
                liElement.append(checkbox);
                liElement.append(label);

                jQuery(formObj).find("ul[code='" + controlCode + "']").append(liElement);
                checkbox.change(function (e) { qpd_changeSelectionCategorySubCategory(formObj, pluginSettings, controlCode, e.target); });
                if (jQuery.inArray(value.SpecialtyId.toString(), defaultSpecialties) > -1) {
                    checkbox.prop('checked', true);
                    checkbox.trigger('change');
                }
            });

            if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
                qpd_assignCategorySubCategoryHiddenValues(formObj, pluginSettings,controlCode, hiddenValControl);
            }

        }
        callback();
    }


    function qpd_buildMultiSelectDDLCategorySubCategoryControl(formObj, pluginSettings,controlCode, data, callback) {
        //clear the select of options entirely.
        var hiddenValControl = jQuery(formObj).find("input[name='" + controlCode + "_Selections']");
        var control = jQuery(formObj).find("select[code='" + controlCode + "']");
        var index = qpd_findControlIndex(formObj, pluginSettings, controlCode);

        jQuery(formObj).find("select[code='" + controlCode + "']").find('option').not('[value=""]').remove().end();
        pluginSettings.questions[index].Initialized = false;

        if (controlCode == "Categories") {

            var defaultCategories = [];
            if (jQuery(formObj).find('#defaultCategories') != undefined && jQuery(formObj).find('#defaultCategories').attr('data-defaults') != undefined) {
                defaultCategories = jQuery(formObj).find('#defaultCategories').attr('data-defaults').split(",");
            }

            //for each returned category add the option to the select list
            jQuery.each(data.CategoryList, function (index, value) {
                var defaultSelected = false;

                if (jQuery.inArray(value.CategoryId.toString(), defaultCategories) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.CategoryName, value.CategoryId, defaultSelected, defaultSelected);
                jQuery(formObj).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });
        }
        else if (controlCode == "SubCategories") {

            var defaultSubCategories = [];
            if (jQuery(formObj).find('#defaultSubCategories') != undefined && jQuery(formObj).find('#defaultSubCategories').attr('data-defaults') != undefined) {
                defaultSubCategories = jQuery(formObj).find('#defaultSubCategories').attr('data-defaults').split(",");
            }

            //for each returned subject add the option to the select list
            jQuery.each(data.SubjectList, function (index, value) {
                var defaultSelected = false;

                if (jQuery.inArray(value.SubjectId.toString(), defaultSubCategories) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.SubjectName, value.SubjectId, defaultSelected, defaultSelected);
                jQuery(formObj).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });
        }
        else {

            var defaultSpecialties = [];
            if (jQuery(formObj).find('#defaultSpecialties') != undefined && jQuery(formObj).find('#defaultSpecialties').attr('data-defaults') != undefined) {
                defaultSpecialties = jQuery(formObj).find('#defaultSpecialties').attr('data-defaults').split(",");
            }

            //for each returned specialty add the option to the select list
            jQuery.each(data.SpecialtyList, function (index, value) {
                var defaultSelected = false;

                if (jQuery.inArray(value.SpecialtyId.toString(), defaultSpecialties) > -1) {
                    defaultSelected = true;
                }

                var newOption = new Option(value.SpecialtyName, value.SpecialtyId, defaultSelected, defaultSelected);
                jQuery(formObj).find("select[code='" + controlCode + "']").append(newOption).trigger('change');
            });

        }

        if (jQuery(hiddenValControl).exists() && hiddenValControl.val().length > 0) {
            qpd_assignCategorySubCategoryMultiSelectHiddenValues(formObj, pluginSettings, controlCode, hiddenValControl);
        }
        control.change(function (e) { qpd_changeMultiSelectSelectionCategorySubCategory(formObj, pluginSettings, controlCode, e.target); });
        control.trigger('change');

        //if were here and the multiselect has no values clear the hidden input
        if (!jQuery(formObj).find("select[code='" + controlCode + "']").val()) {
            jQuery(hiddenValControl).val('');
            jQuery(hiddenValControl).change();
        }

        //jQuery(FormsEngine).trigger("subcategoriesLoaded");
        callback();
    }


    function qpd_assignCategorySubCategoryMultiSelectHiddenValues(formObj, pluginSettings,controlCode, hiddenValControl) {
        var idArray = "";
        if (hiddenValControl.val())
            idArray = hiddenValControl.val().split(',');
        var idArrayDistinct = fep_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes
        jQuery(formObj).find("select[code='" + controlCode + "']").val(idArrayDistinct);
        jQuery(formObj).find("select[code='" + controlCode + "']").select2();
        // fe_consolelog(controlCode + ' ' + hiddenValControl.val());
    }

    function qpd_assignCategorySubCategoryHiddenValues(formObj, pluginSettings,controlCode, hiddenValControl) {
        var idArray = hiddenValControl.val().split(',');
        var idArrayDistinct = fe_getDistinctList(idArray); // temp fix until Alexis can find the fix he previously had for SubCategory dupes

        jQuery.each(idArrayDistinct, function (id) {
            var jQuerycheckbox = jQuery(formObj).find('#' + controlCode + '_' + idArrayDistinct[id]);

            if (jQuerycheckbox.exists()) {
                jQuerycheckbox.prop('checked', true);
                jQuerycheckbox.trigger('change');
                if (populars.length > 0) {
                    if (jQuery.inArray(idArrayDistinct[id], populars) < 0 &&
                        jQuery(formObj).find('#ul' + controlCode).find(jQuerycheckbox).length > 0) {
                        //we have a selected value thats not popular so we need to show the regular div IF IT IS STILL IN THE ORIGINAL DIV AND NOT FEATURED
                        jQuery(formObj).find('#ul' + controlCode).show();
                    }
                }
                else {
                    jQuery(formObj).find('#ul' + controlCode).show();
                }
            } else {
                qpd_removeValueFromCommaList(formObj, pluginSettings,hiddenValControl, idArrayDistinct[id]);

                //********----#57653---*********//
                qpd_updateSelectionCategorySubCategorySpecialty(formObj, pluginSettings,controlCode + '_' + idArrayDistinct[id], false);
                //********END -- #57653*********//
            }
        });

        // fe_consolelog(controlCode + ' ' + hiddenValControl.val());
    }


    //********----#57653---*********//
    function qpd_updateSelectionCategorySubCategorySpecialty(formObj, pluginSettings,checkboxID, isAdd) {

        var checkboxDetails = checkboxID.split("_");
        var itemText = jQuery(formObj).find("label[for=" + checkboxID + "]").text();
        var item = { "id": checkboxDetails[1], "text": itemText, "operation": isAdd, "type": checkboxDetails[0] };

    }
    //********END -- #57653*********//


    function qpd_changeSelectionCategorySubCategory(formObj, pluginSettings,controlcode, checkbox) {
        var hiddenInput = jQuery(formObj).find("input[name='" + controlcode + "_Selections']");
        var hiddenInputValue = hiddenInput.val();
        var checkBoxValue = checkbox.id.split("_")[1];
        var changed = false;

        if (jQuery(checkbox).is(":checked")) {
            var exists = jQuery.inArray(checkBoxValue, jQuery(hiddenInput).val().split(',')) != -1;
            if (!exists) {
                if (hiddenInputValue) {
                    hiddenInput.val(hiddenInputValue + "," + checkBoxValue);
                } else {
                    hiddenInput.val(checkBoxValue);
                }
                changed = true;
            }
        } else {
            qpd_removeValueFromCommaList(formObj, pluginSettings,hiddenInput, checkBoxValue);
            changed = true;
        }
        if (changed) {
            hiddenInput.change();
        }

        //fe_consolelog(hiddenInput.val());
        //********----#57653---*********//
        qpd_updateSelectionCategorySubCategorySpecialty(formObj, pluginSettings,checkbox.id, jQuery(checkbox).is(":checked"));
        //********END -- #57653*********//
    }

//the code in this function had issues in jquery 1.10.2 so it had to be commented out. required for multiselect dropdowns. 
    function qpd_changeMultiSelectSelectionCategorySubCategory(formObj, pluginSettings,controlCode, ddl) {
        //var hiddenInput = jQuery(formObj).find("input[name='" + controlCode + "_Selections']");
        //var hiddenInputValue = "";
        //if (hiddenInput.val())
        //    hiddenInputValue = hiddenInput.val();
        //var changed = false;

        ////for each selected value in the dropdown check if the option is in the dropdown and if its not remove from hidden control
        //if (jQuery(formObj).find("select[code='" + controlCode + "']").val() && jQuery(formObj).find("select[code='" + controlCode + "']").val().length > 0) {
        //    jQuery.each(jQuery("select[code='" + controlCode + "']").val(), function (index, value) {
        //        changed = false;
        //        if (hiddenInput && jQuery(formObj).find("select[code='" + controlCode + "']").find('option[value="' + value + '"]')) {
        //            var exists = jQuery.inArray(parseInt(value), hiddenInputValue.split(',')) != -1;
        //            if (!exists) {
        //                if (hiddenInputValue) {
        //                    hiddenInput.val(hiddenInputValue + "," + value);
        //                } else {
        //                    hiddenInput.val(value);
        //                }
        //                changed = true;
        //            }
        //        } else {
        //            qpd_removeValueFromCommaList(formObj, pluginSettings,hiddenInput, value);
        //            changed = true;
        //        }

        //        if (changed) {
        //            hiddenInput.change();
        //        }
        //    });
        //}

        //********END -- #57653*********//
    }

    function qpd_removeValueFromCommaList(formObj, pluginSettings,hiddenValControl, intValue) {
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

    function qpd_cleanCSVIntArray(ControlValue) {
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
    function qpd_recoverForm(formObj, pluginSettings,data, callback) {

        pluginSettings.stringToRecover = data;

        //Categories recovery
        var ControlValue = fep_getParameterByNameFromString('Categories_Selections', data);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fep_getParameterByNameFromString('Categories', data)
        }

        if (jQuery(formObj).find("input[name='Categories_Selections']").exists())
            jQuery(formObj).find("input[name='Categories_Selections']").val(qpd_cleanCSVIntArray(ControlValue));
        else if (jQuery(formObj).find("input[name='Categories_Hidden']").exists())
            jQuery(formObj).find("input[name='Categories_Hidden']").val(qpd_cleanCSVIntArray(ControlValue));

        //SubCategories recovery
        ControlValue = fep_getParameterByNameFromString('SubCategories_Selections', data);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fep_getParameterByNameFromString('SubCategories', data)
        }

        if (jQuery(formObj).find("input[name='SubCategories_Selections']").exists())
            jQuery(formObj).find("input[name='SubCategories_Selections']").val(qpd_cleanCSVIntArray(ControlValue));
        else if (jQuery(formObj).find("input[name='SubCategories_Hidden']").exists())
            jQuery(formObj).find("input[name='SubCategories_Hidden']").val(qpd_cleanCSVIntArray(ControlValue));

        //Specialties recovery
        ControlValue = fep_getParameterByNameFromString('Specialties_Selections', data);
        if (ControlValue == "" || ControlValue == undefined) {
            ControlValue = fep_getParameterByNameFromString('Specialties', data)
        }
        jQuery(formObj).find("input[name='Specialties_Selections']").val(qpd_cleanCSVIntArray(ControlValue));

        qpd_checkControlDataBind(formObj, pluginSettings,0, callback);
    }

//Loads values from data array
function qpd_loadFormFromPassThrough(formObj, pluginSettings,callback) {
    var Data = '';
    var FirstQuestion = true;

    for (var i = 0; i < pluginSettings.passThruItems.length; i++) {
        if (FirstQuestion == true) {
            Data = Data + pluginSettings.passThruItems[i].QuestionName + "=";
            FirstQuestion = false;
        }
        else {
            Data = Data + "&" + pluginSettings.passThruItems[i].QuestionName + "=";
        }

        var Values = pluginSettings.passThruItems[i].Answers.split(',');
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

    return Data;

    //if (Data != undefined && Data.length > 0) {
    //    fep_consolelog(formObj, pluginSettings,"LOADING FORM FROM PASSTHROUGH");
    //    qpd_recoverForm(formObj, pluginSettings,Data, function () {
    //        //Callback
    //        if (callback) {
    //            callback();
    //        }
    //    });
    //}
    //else {
    //    if (callback) {
    //        callback();
    //    }
    //}
}


    //Finds the control index in the question array
function qpd_findControlIndex(formObj, pluginSettings, ControlCode, ControlId) {
        var ControlIndex = -1;
        for (var index = 0; index < pluginSettings.questions.length; index++) {
            if (typeof ControlId != 'undefined' && pluginSettings.questions[index].TemplateControlId == ControlId) {
                ControlIndex = index;
                break;
            }
            else if (typeof ControlId == 'undefined' && pluginSettings.questions[index].Code == ControlCode) {
                ControlIndex = index;
                break;
            }
        }
        return ControlIndex;
    }


    //Gets the filters values for the question based on the previous controls values that are filters
    function qpd_findQuestionFilterValues(formObj, pluginSettings,index, Question) {
        var FilterValues = [];
        var QuestionFilters = Question.Filters.slice(0);
                
        //Search in the previous controls to the question
        for (var i = 0; i < index; i++) {
            //For each filter
            for (var filterIndex = 0; filterIndex < QuestionFilters.length; filterIndex++) {
                //filter equals to the control
                if (pluginSettings.questions[i].Code == QuestionFilters[filterIndex]) {
                    //Gets control value
                    var ControlValue = fep_getControlValue(formObj, pluginSettings, pluginSettings.questions[i].Code);
                    if (ControlValue.value != "") {
                        var Filter = {};
                        Filter.Key = pluginSettings.questions[i].Code;
                        Filter.Value = ControlValue.value;
                        FilterValues.push(Filter.Key + "=" + Filter.Value);
                    }
                    if (ControlValue.requiresKey && ControlValue.valueKey != "") {
                        var Filter = {};
                        Filter.Key = pluginSettings.questions[i].Code;
                        Filter.Value = ControlValue.valueKey;
                        FilterValues.push(Filter.Key + "-key=" + Filter.Value);
                    }

                    //Remove filter found
                    QuestionFilters.splice(filterIndex, 1);
                    break;
                }
            }
        }
        return FilterValues.join('&');
    }

function qpd_initializeControl(formObj, pluginSettings, index) {
    var ControlValue = fep_getParameterByNameAndAliasFromString(formObj, pluginSettings, pluginSettings.questions[index].Code, pluginSettings.stringToRecover);

        if (ControlValue != "") {
            fep_setControlValue(formObj, pluginSettings,pluginSettings.questions[index].Code, ControlValue);            
        }
        //select2 start

        //select2 end
        //select2 start
        if (typeof jQuery().select2 == "function") {
            var jQueryselectControl = jQuery(formObj).find("select.select2[code='" + pluginSettings.questions[index].Code + "']");
            if (jQueryselectControl.length > 0) {
                // if typeahead and inline
                if (jQueryselectControl.hasClass("typeahead") && jQueryselectControl.hasClass("inlineDropDown")) {
                    if (jQueryselectControl.hasClass("searchablePredfinedValueList")) {
                        if (jQueryselectControl.find("option[value='']").length > 0) {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
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
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.attr("placeholder"),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
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
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                    } else {
                        if (jQueryselectControl.find("option[value='']").length > 0) {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.find("option[value='']").eq(0).text()
                            });
                        }
                        else if (jQueryselectControl.siblings("label.inline-hidden-label").length > 0) {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.siblings("label.inline-hidden-label").eq(0).text()
                            });
                        }
                        else {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.attr("placeholder"),
                                allowClear: true
                            });
                        }
                    }
                }
                // if just typeahead
                else if (jQueryselectControl.hasClass("typeahead")) {
                    if (jQueryselectControl.hasClass("searchablePredfinedValueList")) {
                        if (jQueryselectControl.find("option[value='']").length > 0) {
                            jQueryselectControl.select2({
                                createTag: function (params) {
                                    var term = jQuery.trim(params.term);

                                    if (term === '') {
                                        return null;
                                    }

                                    return {
                                        id: term,
                                        text: term,
                                        key: term // add additional parameters
                                    }
                                },
                                placeholder: jQueryselectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
                                        }


                                        return query;
                                    },
                                    processResults: function (data) {
                                        // data has text and value..
                                        return {
                                            results: data
                                        }
                                    },

                                },
                                minimumInputLength: 3
                            }).on('select2:select', function (e) {
                                var data = e.params.data;
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.attr("placeholder"),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
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
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                    } else {
                        if (jQueryselectControl.find("option[value='']").length > 0) {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.find("option[value='']").eq(0).text()
                            });
                        }
                        else {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.attr("placeholder"),
                                allowClear: true
                            });
                        }
                    }
                }
                // if just inline
                else if (jQueryselectControl.hasClass("inlineDropDown")) {
                    if (jQueryselectControl.hasClass("searchablePredfinedValueList")) {
                        if (jQueryselectControl.find("option[value='']").length > 0) {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.find("option[value='']").eq(0).text(),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
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
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                        else {
                            jQueryselectControl.select2({
                                placeholder: jQueryselectControl.attr("placeholder"),
                                ajax: {
                                    url: pluginSettings.serviceUrl + "/DataBind/SearchPreDefinedValueList",

                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        var query = {
                                            term: params.term,
                                            standardControlCode: jQueryselectControl.attr('code')
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
                                jQuery(this).children('[value="' + data['id'] + '"]').attr(
                                    {
                                        'key': data["key"], //dynamic value from data array

                                    }
                                );
                            }).val(0).trigger('change');
                        }
                    } else {
                        if (jQueryselectControl.siblings("label.inline-hidden-label").length > 0) {
                            jQueryselectControl.select2({
                                minimumResultsForSearch: Infinity,
                                placeholder: jQueryselectControl.siblings("label.inline-hidden-label").eq(0).text()
                            });
                        }
                    }
                }
                else {
                    jQueryselectControl.select2({
                        minimumResultsForSearch: Infinity
                    });
                }


                if (jQueryselectControl.hasClass('multiselectdropdown')) {
                    jQueryselectControl.on('select2:unselect', function (e) {
                        //if we deselected from a multiselect dropdown we need to make sure the hidden inputs are updated properly
                        var hiddenInput = jQuery("input[name='" + jQueryselectControl.attr('code') + "_Selections']");
                        if (hiddenInput) {
                            hiddenInput.val(jQueryselectControl.val());
                            hiddenInput.change();
                        }
                    });
                }
            }
            // if just select2
            else {
                jQueryselectControl.select2({
                    placeholder: jQueryselectControl.attr("placeholder"),
                    allowClear: true
                });
            }
        }
        //select2 end


        pluginSettings.questions[index].Initialized = true;

    }

    //Recursive data bind helper v2.0 (within step only)
    function qpd_checkControlDataBind(formObj, pluginSettings, index, callback) {

        if (index >= 0 && index < pluginSettings.questions.length) {
            //On recovery mode set field value and proceed when question has not been initialized
            if (pluginSettings.questions[index].Initialized == false && pluginSettings.questions[index].DataBind == undefined) {
                qpd_initializeControl(formObj, pluginSettings,index);
            }

            //If control requires a data bind find filter values on previous controls
            if (pluginSettings.questions[index].DataBind != undefined) {
                var FilterValues = qpd_findQuestionFilterValues(formObj, pluginSettings, index, pluginSettings.questions[index]);
                //Bind data
                if (pluginSettings.questions[index].LastDataBindFilters != FilterValues) {
                    //fe_consolelog('checkControlDataBind=' + functionName(pluginSettings.questions[index].DataBind) + "(" + FilterValues + ")");
                    pluginSettings.questions[index].LastDataBindFilters = FilterValues;
                    pluginSettings.questions[index].DataBind(formObj, pluginSettings,FilterValues, function () {
                        if (pluginSettings.questions[index].Initialized == false) {
                            qpd_initializeControl(formObj, pluginSettings,index);
                        }
                        qpd_checkControlDataBind(formObj, pluginSettings, index + 1, callback);
                    });
                }
                else {
                    qpd_checkControlDataBind(formObj, pluginSettings, index + 1, callback);
                }


            }
            else {
                qpd_checkControlDataBind(formObj, pluginSettings, index + 1, callback);
            }
        }
    }

    //Starts cascading check of controls, starting by the control down to the last
function qpd_checkDependencies(formObj, pluginSettings, control) {

        var ControlCode = jQuery(control).attr("code");
        var ControlValue = fep_getControlValue(formObj, pluginSettings,ControlCode).value;
        var ControlIndex = 0;

        if (ControlCode == 'LBL') {
            ControlIndex = qpd_findControlIndex(formObj, pluginSettings,ControlCode, jQuery(control).attr("id"));
        }
        else {
            ControlIndex = qpd_findControlIndex(formObj, pluginSettings,ControlCode);
        }

        //Not next step and whithin the step databind next control
        if (ControlIndex + 1 < pluginSettings.questions.length) {
            //Next control is the first one to be checked for data bind{
            ControlIndex++;
            qpd_checkControlDataBind(formObj, pluginSettings, ControlIndex);
        }

    }


    //Bind events to controls
    function qpd_bindEventsToControls(formObj, pluginSettings) {
        for (var item = 0; item < pluginSettings.questions.length; item++) {
            var Control = jQuery(formObj).find("[code='" + pluginSettings.questions[item].Code + "']");
            if (jQuery(Control).is(':radio')) {
                jQuery(Control).click(function () { qpd_checkDependencies(formObj, pluginSettings, jQuery(this)); });
            }
            else if (jQuery(Control).is(':checkbox')) {
                jQuery(Control).change(function () { qpd_checkDependencies(formObj, pluginSettings, jQuery(this)); });
            }
            else if (jQuery(Control).is('input:text')) {
                jQuery(Control).blur(function () { qpd_checkDependencies(formObj, pluginSettings, jQuery(this)); });
            }
            else if (jQuery(Control).is('select')) {
                jQuery(Control).change(function () { qpd_checkDependencies(formObj, pluginSettings, jQuery(this)); });
            }
            else {
                jQuery(Control).blur(function () { qpd_checkDependencies(formObj, pluginSettings, jQuery(this)); });
            }
        }

        //Bind Categories hidden field
        if (jQuery(formObj).find("input[name='Categories_Selections']").exists()) {
            jQuery(formObj).find("input[name='Categories_Selections']").change(function () { qpd_checkDependencies(formObj, pluginSettings,jQuery(this)); });
        }

        //Bind SubCategories hidden field
        if (jQuery(formObj).find("input[name='SubCategories_Selections']").exists()) {
            jQuery(formObj).find("input[name='SubCategories_Selections']").change(function () { qpd_checkDependencies(formObj, pluginSettings,jQuery(this)); });
        }

        //Bind SubCategories hidden field
        if (jQuery(formObj).find("input[name='Specialties_Selections']").exists()) {
            jQuery(formObj).find("input[name='Specialties_Selections']").change(function () { qpd_checkDependencies(formObj, pluginSettings,jQuery(this)); });
        }
    }


function qpd_loadOptionsIntoDropDownControl(formObj, pluginSettings,control, data) {
        var previousValue = control.val();

    if (jQuery(control).hasClass("inlineDropDown")) {
        control.empty();
        qpd_makeInlineDropDown(control);
    }
    else {
        control.find('option').not('[value=""]').remove().end();

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


    function qpd_bindDynamicPluginControls(formObj, pluginSettings) {

        //Bind Events
        qpd_bindEventsToControls(formObj, pluginSettings);

        //Load Form from querystring
        //qpd_loadFormFromQuerystring(formObj, pluginSettings);

        //Load Form from passthrough
        var data = qpd_loadFormFromPassThrough(formObj, pluginSettings);

        qpd_recoverForm(formObj, pluginSettings, data != undefined ? data: "", function () {
            //Callback
            if (callback) {
                callback();
            }
        });

        //Global shared to trigger check dependencies for a particular control and after.
        //qpd_checkDependencies(formObj, pluginSettings);
}


function qpd_makeInlineDropDown(formObj, pluginSettings, control) {
    var $label = $(formObj).find("label[for=" + $(control).prop("id") + "]");

    control.append(jQuery('<option/>', {
        disabled: true,
        selected: true,
        value: "",
        text: ($label.length > 0) ? $label.text() : "--Select--"
    }));
}
