//Shortcut extension method for jquery selector exist
jQuery.fn.exists = function () { return this.length > 0; }

    function fep_stripHtmlFromText(html) {
        var tmp = document.createElement("DIV");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText || "";
    }

    function fep_getQuerystring() {
        return window.location.search.replace("?", "");
    }


    function fep_getParameterByNameAndAliasFromString(formObj, pluginSettings, code, string) {
        var ControlValue = "";

        //Based on control code
        ControlValue = fep_getParameterByNameFromString(code, string);

        //Based on aliases if supported
        if (ControlValue == undefined || ControlValue == "") {
            var alias = jQuery(formObj).find(":input[code='" + code + "']").attr("alias");
            if (alias != undefined && alias != "") {
                var aliasList = alias.split(',');
                for (i = 0; i < aliasList.length; i++) {
                    ControlValue = fep_getParameterByNameFromString(aliasList[i], string);
                    if (ControlValue != undefined && ControlValue != "") {
                        return ControlValue;
                    }
                }
            }
        }
        return ControlValue;
    }

    function fep_getParameterByNameFromString(name, string) {
        match = new RegExp('(?:^|&)' + name + '=([^&]*)', 'i').exec(string);
        return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
    }

    function fep_getParameterByName(name) {
        match = new RegExp('[?&]' + name + '=([^&]*)', 'i').exec(window.location.search);
        return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
    }

    function fep_getDistinctList(list) {
        var result = [];
        jQuery.each(list, function (i, e) {
            if (jQuery.inArray(e, result) == -1) result.push(e);
        });
        return result;
    }


    function fep_setControlValue(formObj, pluginSettings,controlCode, controlValue) {
        var Field = jQuery(formObj).find(":input[code='" + controlCode + "']");
        if (Field != undefined && controlCode != "UserAgreement" && controlCode != "EDDYUserAgreement") {

            if (jQuery(Field).is(':radio')) {
                jQuery(formObj).find("[type=radio][code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
            }
            else if (jQuery(Field).is(':checkbox')) {
                jQuery(formObj).find("[type=checkbox][code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
            }
            else if (jQuery(Field).is('select')) {
                if (jQuery(Field).find("option[value='" + controlValue + "']").length > 0) { //this is a select so only set the value if the value is in the list
                    jQuery(Field).val(controlValue);
                    //jquery responsive
                    jQuery(formObj).find(":input[parent-code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
                }
            }
            else {
                jQuery(Field).val(controlValue);

                //jquery responsive
                jQuery(formObj).find(":input[parent-code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true)
            }

            fep_consolelog(formObj, pluginSettings,"SetControlValue Code: " + controlCode + ", Value: " + controlValue);
        }
    }

    function fep_getControlValue(formObj, pluginSettings,controlCode) {

        var dependantField = jQuery(formObj).find(":input[code='" + controlCode + "']");
        var ControlValue = {};
        ControlValue.value = "";
        ControlValue.valueKey = "";
        ControlValue.requiresKey = jQuery(dependantField).attr("me-filter") != undefined;

        if (jQuery(dependantField).is('select')) {
            ControlValue.value = jQuery(dependantField).val();
            ControlValue.valueKey = jQuery(formObj).find(":input[code='" + controlCode + "']>option:selected").text();
        }
        else if (jQuery(dependantField).is(':radio')) {
            ControlValue.value = jQuery(formObj).find(":input[code='" + controlCode + "']:checked").val();
            ControlValue.valueKey = jQuery(formObj).find(":input[code='" + controlCode + "']:checked").text();
        }
        else {
            ControlValue.value = jQuery(dependantField).val();
        }

        ControlValue.value = ControlValue.value == null || ControlValue.value == undefined ? "" : ControlValue.value;
        ControlValue.valueKey = ControlValue.valueKey == null || ControlValue.valueKey == undefined ? "" : ControlValue.valueKey;

        return ControlValue;
}


function fep_getControlText(formObj, pluginSettings, controlCode) {

    var dependantField = jQuery(formObj).find(":input[code='" + controlCode + "']");
    var ControlValue = {};
    ControlValue.value = "";
    ControlValue.valueKey = "";
    ControlValue.requiresKey = jQuery(dependantField).attr("me-filter") != undefined;

    if (jQuery(dependantField).is('select')) {
        ControlValue.value = jQuery(dependantField).val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(formObj).find(":input[code='" + controlCode + "']>option:selected").attr('key');
        }
    }
    else if (jQuery(dependantField).is(':radio')) {
        ControlValue.value = jQuery(formObj).find(":input[code='" + controlCode + "']:checked").val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(formObj).find(":input[code='" + controlCode + "']:checked").attr('key');
        }
    }

    ControlValue.value = ControlValue.value == null || ControlValue.value == undefined ? "" : ControlValue.value;
    ControlValue.valueKey = ControlValue.valueKey == null || ControlValue.valueKey == undefined ? "" : ControlValue.valueKey;

    return ControlValue;
}

    function fep_consolelog(formObj, pluginSettings, value, printObject) {
        if (pluginSettings.debugMode && pluginSettings.debugMode == true && typeof console == "object") {
            if (typeof console.dir == "function" && printObject === true) {
                console.dir(value);
            }
            else if (typeof console.log == "function") {
                console.log("eddyQDFP:" + value);
            }
        }
    }
    function fep_logClientException(formObj, pluginSettings, jqXHR, url, message) {
        try {
            //debug
            var debugInfo = "TrackId=" + pluginSettings.trackId;
            debugInfo += " RenderingStrategy=" + pluginSettings.renderingStrategy;

            //ex detail
            var ex = jqXHR ? jqXHR.responseText : "";

            var args = "Title=FE.Template." + pluginSettings.templateId;
            args += "&URL=" + url;
            args += "&DebugInfo=" + fep_stripHtmlFromText(debugInfo);
            args += "&Exception=" + fep_stripHtmlFromText(message);
            args += "&ExceptionDetail=" + fep_stripHtmlFromText(ex);

            jQuery.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: pluginSettings.serviceBaseURL + "/TemplateManager/LogClientException?" + args,
                error: function (request, error) {
                    fep_consolelog(formObj, pluginSettings,arguments + "\n" + " Error: " + request.responseText);
                }
            });
        }
        catch (ex) { }
    }