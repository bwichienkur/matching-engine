
// Global.js (Forms Engine Globals)
// ** PREPEND ALL GLOBAL FUNCTIONS WITH fe_
// ** ALL jquery usage must be with the full "jQuery()" syntax in this file, no "$()" allowed!
//---------------------------------------------------------------------------------------------

//Shortcut extension method for jquery selector exist
jQuery.fn.exists = function () { return this.length > 0; }

//Extension method for ControlBeforeMe based on sorting criteria
jQuery.fn.isControlBefore = function (control) { if (!jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + control + "']").exists()) { return true; } else { return parseInt(jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + control + "']").attr("id-sort")) > parseInt(jQuery(this).attr("id-sort")); } }

//bootstrap fix for jQuery show() hide() until bootstrap decides to be JS compatible
//https://github.com/zikula/core/issues/1140
var oldShowHide = { 'show': jQuery.fn.show, 'hide': jQuery.fn.hide };
jQuery.fn.extend({
    show: function () {
        this.each(function (index) {
            jQuery(this).removeClass('hide');
        });
        return oldShowHide.show.call(this);
    },
    hide: function () {
        this.each(function (index) {
            jQuery(this).removeClass('show');
        });
        return oldShowHide.hide.call(this);
    }
});


var w = jQuery(window);
jQuery.fn.visible = function (partial, hidden, direction) {
    if (this.length < 1)
        return;
    var t = this.length > 1 ? this.eq(0) : this,
        t = jQuery(t).get(0),
        vpWidth = jQuery(w).width(),
        vpHeight = jQuery(w).height(),
        direction = (direction) ? direction : 'both',
        clientSize = hidden === true ? t.offsetWidth * t.offsetHeight : true;
    if (typeof t.getBoundingClientRect === 'function') {
        // Use this native browser method, if available.
        var rec = t.getBoundingClientRect(),
            tViz = rec.top >= 0 && rec.top < vpHeight,
            bViz = rec.bottom > 0 && rec.bottom <= vpHeight,
            lViz = rec.left >= 0 && rec.left < vpWidth,
            rViz = rec.right > 0 && rec.right <= vpWidth,
            vVisible = partial ? tViz || bViz : tViz && bViz,
            hVisible = partial ? lViz || rViz : lViz && rViz;
        if (direction === 'both')
            return clientSize && vVisible && hVisible;
        else if (direction === 'vertical')
            return clientSize && vVisible;
        else if (direction === 'horizontal')
            return clientSize && hVisible;
    } else {
        var viewTop = jQuery(w).scrollTop(),
            viewBottom = viewTop + vpHeight,
            viewLeft = jQuery(w).scrollLeft(),
            viewRight = viewLeft + vpWidth,
            offset = jQuery(t).offset(),
            _top = offset.top,
            _bottom = _top + jQuery(t).height(),
            _left = offset.left,
            _right = _left + jQuery(t).width(),
            compareTop = partial === true ? _bottom : _top,
            compareBottom = partial === true ? _top : _bottom,
            compareLeft = partial === true ? _right : _left,
            compareRight = partial === true ? _left : _right;
        if (direction === 'both')
            return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop)) && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
        else if (direction === 'vertical')
            return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop));
        else if (direction === 'horizontal')
            return !!clientSize && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
    }
};

jQuery.fn.rotateElement = function (initial, delta, times, delay) {
    var elementToRotate = this, timer, angle = initial; rotate(); function rotate() { var deg = angle, deg2radians = Math.PI * 2 / 360, rad = deg * deg2radians, costheta = Math.cos(rad), sintheta = Math.sin(rad), m11 = costheta, m12 = -sintheta, m21 = sintheta, m22 = costheta, matrixValues = 'M11=' + m11 + ',M12=' + m12 + ',M21=' + m21 + ',M22=' + m22; elementToRotate.css('-webkit-transform', 'rotate(' + deg + 'deg)').css('-moz-transform', 'rotate(' + deg + 'deg)').css('-ms-transform', 'rotate(' + deg + 'deg)').css('transform', 'rotate(' + deg + 'deg)').css('filter', 'progid:DXImageTransform.Microsoft.Matrix(sizingMethod=\'auto expand\',' + matrixValues + ')').css('-ms-filter', 'progid:DXImageTransform.Microsoft.Matrix(SizingMethod=\'auto expand\',' + matrixValues + ')'); timer = setTimeout(function () { angle += delta; times--; if (times >= 0) rotate(); }, delay); }
}

jQuery.fn.extend({
    checkImages: function () {
        return jQuery(this).each(function () {
            var obj = jQuery(this);
            var img = new Image();
            img.onload = function () {
                obj.find("img").attr("src", obj.attr("data-src"));
            };
            img.onerror = function () {
                if (obj.attr("data-src-name")) {
                    obj.find("img").replaceWith("<span class='school-logo-name'>" + obj.attr("data-src-name") + "</span>");
                }
                else {
                    obj.remove();
                }
                fe_consolelog('School Image not found:' + obj.attr("data-src"));
            }
            img.src = obj.attr("data-src");
        });
    }
});

function fe_getIEVersion() {
    var agent = navigator.userAgent;
    var reg = /MSIE\s?(\d+)(?:\.(\d+))?/i;
    var matches = agent.match(reg);
    if (matches != null) {
        return { major: matches[1], minor: matches[2] };
    }
    else if (navigator.appVersion.indexOf('Trident/') > 0) {
        return { major: "11", minor: "" };
    }
    return { major: "-1", minor: "-1" };

}


//Global - Do not remove
function fe_global() { }

//Console Log 
function fe_consolelog(value, printObject) {
    if (FormsEngine.DebugMode && FormsEngine.DebugMode == true && typeof console == "object") {
        if (typeof console.dir == "function" && printObject === true) {
            console.dir(value);
        }
        else if (typeof console.log == "function") {
            console.log("eddyFE:" + value);
        }
    }
}

function fe_checkImage(src, good, bad) {
    var img = new Image();
    img.onload = good;
    img.onerror = bad;
    img.src = src;
}



function fe_logClientException(jqXHR, url, message) {
    try {
        //debug
        var debugInfo = "TrackId=" + FormsEngine.TrackId;
        debugInfo += " FESessionId=" + FormsEngine.FESessionId;
        debugInfo += " MatchResponseGuid=" + FormsEngine.MatchResponseGuid;
        debugInfo += " RenderingStrategy=" + FormsEngine.RenderingStrategy;
        debugInfo += " SelfContained=" + FormsEngine.SelfContained;
        debugInfo += " SessionId=" + FormsEngine.SessionId;
        debugInfo += " ProspectId=" + FormsEngine.ProspectId;

        //ex detail
        var ex = jqXHR ? jqXHR.responseText : "";

        var args = "Title=FE.Template." + FormsEngine.TemplateId;
        args += "&URL=" + url;
        args += "&DebugInfo=" + fe_stripHtmlFromText(debugInfo);
        args += "&Exception=" + fe_stripHtmlFromText(message);
        args += "&ExceptionDetail=" + fe_stripHtmlFromText(ex);

        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/TemplateManager/LogClientException?" + args,
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    }
    catch (ex) { }
}

//Show loader
function fe_showLoader() {

    var d = new Date();
    //console.log("showing loader: " + d.getMilliseconds());

    if (jQuery("[id='FEloader']").length < 1) {
        var loaderHtml = '<div id="FEmodalOverlayBG" class="FEoverlay hide"><div id="FEloader" class="hide text-center"><i class="fa fa-spinner fa-pulse hide"></i><p class="loading">Loading &hellip;</p><p class="loadingParagraph"><em>Please wait one moment</em></p></div></div>';
        jQuery('.eddy-form-wizard-body').prepend(loaderHtml);
    }
    jQuery('[id="FEloader"]').addClass('loaderOn');
    jQuery('[id="FEmodalOverlayBG"]').show();
    jQuery('[id="FEloader"]').show();
}

//Simple checks
function fe_isNullOrEmpty(value) {
    return (value == null || value == 'null' || value == '');
}

//Hide Loader
function fe_hideLoader(keepOverlayShown) {
    if (!keepOverlayShown) {
        jQuery('[id="FEmodalOverlayBG"]').hide();
    }
    jQuery('[id="FEloader"]').hide();
}

function fe_setProgressBar(percent) {
    if (jQuery('#BSProgressBar').length > 0) { //bootstrap version
        jQuery('#BSProgressBar').css('width', percent + '%')
        jQuery('#BSProgressBar').html(percent + '%');
        jQuery('#BSProgressBar').attr('aria-valuenow', percent);
    }
    else { //regular version
        jQuery('.progress-bar-inside').css('width', percent + '%');
        jQuery('.progress-bar-number').text(percent + '%');
    }
}

//bind status update events to all required controls
function fe_bindControlStatusEvent() {
    jQuery(FormsEngine.DefaultFormTag).find('input,textarea,select').filter('[required]:visible').on("change", function () {
        fe_setControlStatus();
    });
}
//set the status of required controls
function fe_setControlStatus() {

    var totalRequiredControls = 0;
    var totalRequiredControlsWithValues = 0;
    if (FormsEngine.RequiredControlTotal == null) {
        FormsEngine.RequiredControlTotal = 0;
    }
    if (FormsEngine.RequiredControlIds == null) {
        FormsEngine.RequiredControlIds = [];
    }
    jQuery(FormsEngine.DefaultFormTag).find('input,textarea,select').filter('[required]:visible').filter(function () {
        if (FormsEngine.RequiredControlIds.indexOf(this.id) == -1) {
            FormsEngine.RequiredControlIds.push(this.id);
            totalRequiredControls++;
        }

        return true;
    }).length;

    for (var i = 0; i <= FormsEngine.RequiredControlIds.length; i++) {
        var control = jQuery(FormsEngine.DefaultFormTag).find('#' + FormsEngine.RequiredControlIds[i])[0];
        if (control != null) {
            var hasValue = false;
            switch (control.type) {
                case "radio":

                    if (jQuery(FormsEngine.DefaultFormTag).find('input[name=' + control.name + ']:checked').val() != null) {
                        hasValue = true;
                    }
                    break;
                case "checkbox":
                    if (jQuery(FormsEngine.DefaultFormTag).find('input[name=' + control.name + ']:checked').val() != null) {
                        hasValue = true;
                    }
                    break;
                default:
                    hasValue = control.value.length > 0;
                    break;
            }
            if (hasValue) {
                totalRequiredControlsWithValues++;
            }
        }

    }

    FormsEngine.RequiredControlTotal = (FormsEngine.RequiredControlTotal + totalRequiredControls);
    FormsEngine.RequiredControlwithValues = totalRequiredControlsWithValues;
    if (FormsEngine.StepTotal === 1 && FormsEngine.CurrentStep === 1) {
        wizardPercent = Math.floor((FormsEngine.RequiredControlwithValues * 95) / FormsEngine.RequiredControlTotal);

        fe_consolelog('wizardPercent by completed control global');
        fe_setProgressBar(wizardPercent);
    }
}



function fe_resetSession(reload) {
    try {
        fe_leadIdReinit();
    }
    catch (ex) { }

    fe_getSessionId(function () {

        FormsEngine.createCookie(FormsEngine.SessionCookieName, "");

        if (reload) {
            location.reload();

        }
    });
}
function fe_leadIdReinit() {
    if (typeof LeadiD != 'undefined') {
        LeadiD.reInit();
    }
}


function fe_isguid(str) {
    var rgex = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/;
    return rgex.test(str);
}

function fe_setPlacementGuid() {
    let placementGuid = fe_getParameterByName('placementviewguid');

    if (placementGuid != null) {
        fe_add_AdditionalField('placementviewguid', placementGuid);
    }
    else
        console.log("placementGuid not found")
}

//Internal get session id
function fe_getSessionId(callback) {


    if (FormsEngine.MultiSession == true) {
        FormsEngine.FESessionId = FormsEngine.readCookie(FormsEngine.SessionCookieName);
    } else {
        FormsEngine.FESessionId = FormsEngine.readCookie("FE_SessionId");
    }
    if (FormsEngine.FESessionId == undefined || FormsEngine.FESessionId == null || FormsEngine.FESessionId == "") {
        //refferals to form may have need the Session cookie created locally to continue a session
        //FE_SessionId is the name of the url parameter that store the sessionid from the reffered session
        var qsGuid = fe_getParameterByName("FE_SessionId");
        if (qsGuid && qsGuid.length == 36 && fe_isguid(qsGuid)) {
            var sUrl = FormsEngine.ServiceBaseURL + "/Session/GetClonedSessionId?CFESessionId=" + qsGuid;
            jQuery.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: sUrl,
                success: function (data) {
                    FormsEngine.FESessionId = data;
                    fe_setCookieName(FormsEngine.FESessionId);
                    FormsEngine.createCookie(FormsEngine.SessionCookieName, data);

                },
                error: function (request, textStatus, errorThrown) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    fe_logClientException(request, sUrl, errorThrown);
                }
            }).done(callback);

        }
        else {
            var sUrl = FormsEngine.ServiceBaseURL + "/Session/GetSessionId";
            jQuery.ajax({
                async: true,
                type: 'GET',
                dataType: 'jsonp',
                cache: false,
                url: sUrl,
                success: function (data) {
                    FormsEngine.FESessionId = data;
                    fe_setCookieName(FormsEngine.FESessionId);
                    FormsEngine.createCookie(FormsEngine.SessionCookieName, data);

                },
                error: function (request, textStatus, errorThrown) {
                    fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                    fe_logClientException(request, sUrl, errorThrown);
                }
            }).done(callback);
        }
    }
    else {
        fe_setCookieName(FormsEngine.FESessionId);
        callback();
    }
}

function fe_setCookieName(sessionId) {
    var sessionCookieName = "FE_SessionId";
    if (FormsEngine.SessionCookieName == "" || FormsEngine.SessionCookieName == undefined || FormsEngine.SessionCookieName == null) {
        FormsEngine.SessionCookieName = sessionCookieName;
    }
    if (FormsEngine.MultiSession == true) {
        if (FormsEngine.SessionName == "") {
            FormsEngine.SessionCookieName = "FE_SessionId_" + sessionId;
            FormsEngine.SessionName = FormsEngine.SessionCookieName;
            fe_leadIdReinit();
        }
    }

}


//Reads object from the session
function fe_getSessionObject(key, callback) {
    fe_getSessionId(function () {
        var sUrl = FormsEngine.ServiceBaseURL + "/Session/GetObject?FESessionId=" + FormsEngine.FESessionId + "&Key=" + key;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: sUrl,
            success: function (data) {
                callback(data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    });
}

function fe_pingFESession() {
    fe_getSessionId(function () {
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/Session/PingSession?FESessionId=" + FormsEngine.FESessionId,
            success: function (data) {
                fe_consolelog('Session Ping Successful: ' + data);
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    });
}



function fe_saveWorkflowData(callback) {
    // SessionDTO - Set into Session
    var wfObj = {};
    wfObj.CurrentPage = FormsEngine.CurrentPage;
    wfObj.TemplateId = FormsEngine.TemplateId;
    wfObj.CurrentSmartMatches = FormsEngine.CurrentSmartMatches;
    wfObj.CurrentUserSelections = FormsEngine.CurrentUserSelections;
    wfObj.LeadDataEncoded = FormsEngine.LeadDataEncoded;
    wfObj.LeadAdditionalDataEncoded = FormsEngine.LeadAdditionalDataEncoded;
    wfObj.SMLeadsCreatedCount = FormsEngine.SMLeadsCreatedCount;
    wfObj.USLeadsCreatedCount = FormsEngine.USLeadsCreatedCount;
    wfObj.UserFullName = FormsEngine.UserFullName;
    wfObj.ProspectId = FormsEngine.ProspectId;
    wfObj.MatchResponseGuid = FormsEngine.MatchResponseGuid;
    wfObj.SplitCampusTypeInResults = FormsEngine.SplitCampusTypeInResults;
    wfObj.UserSmartMatched = FormsEngine.UserSmartMatched != undefined && FormsEngine.UserSmartMatched != null ? FormsEngine.UserSmartMatched : false;
    wfObj.UserShownManagedChoice = FormsEngine.UserShownManagedChoice != undefined && FormsEngine.UserShownManagedChoice != null ? FormsEngine.UserShownManagedChoice : false;
    wfObj.UserSubmittedManagedChoiceSelection = FormsEngine.UserSubmittedManagedChoiceSelection != undefined && FormsEngine.UserSubmittedManagedChoiceSelection != null ? FormsEngine.UserSubmittedManagedChoiceSelection : false;
    wfObj.UserSkippedToConfirmation = FormsEngine.UserSkippedToConfirmation != undefined && FormsEngine.UserSkippedToConfirmation != null ? FormsEngine.UserSkippedToConfirmation : false;
    wfObj.FormTemplateType = FormsEngine.FormTemplateType;
    wfObj.ProgramTemplateId = FormsEngine.ProgramTemplateId;
    wfObj.ProgramProductId = FormsEngine.ProgramProductId;
    wfObj.ProductId = FormsEngine.ProductId;
    wfObj.InstitutionId = FormsEngine.InstitutionId;
    wfObj.ApplicationId = FormsEngine.ApplicationId;
    wfObj.InstitutionName = FormsEngine.InstitutionName;
    wfObj.ProgramName = FormsEngine.ProgramName;

    if (FormsEngine.UseInternationalTemplate) {
        wfObj.UseInternationalTemplate = FormsEngine.UseInternationalTemplate;
    }

    if (FormsEngine.IsLocalIP) {
        wfObj.IsLocalIP = FormsEngine.IsLocalIP;
    }

    fe_consolelog('DEBUG --> fe_saveWorkflowData(Currentpage) = ' + FormsEngine.CurrentPage);

    fe_setWorkflowStatus(wfObj, function () { callback() });
}


//loads workflow data
function fe_loadWorkflowData(callback) {
    fe_getWorkflowStatus(function (data) {
        // SessionDTO - Get from Session
        /* var result = "CurrentPage=" + data.CurrentPage;
         result += "\nTemplateId=" + data.TemplateId;
         result += "\nCurrentSmartMatches=" + data.CurrentSmartMatches;
         result += "\nCurrentUserSelections=" + data.CurrentUserSelections;
         result += "\nSMLeadsCreatedCount=" + data.SMLeadsCreatedCount;
         result += "\nUSLeadsCreatedCount=" + data.USLeadsCreatedCount;
         result += "\nUserFullName=" + data.UserFullName;
         result += "\nProspectId=" + data.ProspectId;
         result += "\nMatchResponseGuid=" + data.MatchResponseGuid;
         result += "\nSplitCampusTypeInResults=" + data.SplitCampusTypeInResults;
         result += "\nUserSmartMatched=" + data.UserSmartMatched;
         result += "\nFormTemplateType=" + data.FormTemplateType;
         result += "\nProgramTemplateId=" + data.ProgramTemplateId;
         result += "\nProgramProductId=" + data.ProgramProductId;
         result += "\nInstitutionId=" + data.InstitutionId;
         result += "\ApplicationId=" + data.ApplicationId;
         result += "\nInstitutionName=" + data.InstitutionName;
         result += "\nProgramName=" + data.ProgramName;
         result += "\nUseInternationalTemplate=" + data.UseInternationalTemplate;
         //result += "\nLeadDataEncoded=" + data.LeadDataEncoded;
         //result += "\nLeadAdditionalDataEncoded=" + data.LeadAdditionalDataEncoded;
         fe_consolelog(result);*/

        fe_consolelog("DEBUG --> fe_loadWorkflowData(data.CurrentPage)=" + data.CurrentPage);
        fe_consolelog("DEBUG --> fe_loadWorkflowData(data.TemplateId)=" + data.TemplateId);

        // SessionDTO - Set into Client
        FormsEngine.TemplateId = FormsEngine.TemplateId === undefined || FormsEngine.TemplateId === null || FormsEngine.TemplateId === 0 ? data.TemplateId : FormsEngine.TemplateId;

        if (data.UseInternationalTemplate || (FormsEngine.ConsumerSideWorkflowStep !== "START" && FormsEngine.SelfContained === false)) {
            FormsEngine.TemplateId = data.TemplateId;
        }

        FormsEngine.CurrentPage = data.CurrentPage;
        FormsEngine.CurrentPage = (FormsEngine.CurrentPage != undefined && FormsEngine.CurrentPage != null && FormsEngine.CurrentPage != '') ? FormsEngine.CurrentPage : 'START'; //Wizard-start
        FormsEngine.LeadDataEncoded = data.LeadDataEncoded;
        FormsEngine.LeadAdditionalDataEncoded = data.LeadAdditionalDataEncoded;
        FormsEngine.SMLeadsCreatedCount = data.SMLeadsCreatedCount;
        FormsEngine.USLeadsCreatedCount = data.USLeadsCreatedCount;
        FormsEngine.UserFullName = data.UserFullName;
        FormsEngine.ProspectId = data.ProspectId;
        FormsEngine.MatchResponseGuid = data.MatchResponseGuid != undefined && data.MatchResponseGuid != null ? data.MatchResponseGuid : FormsEngine.MatchResponseGuid;
        FormsEngine.SplitCampusTypeInResults = data.SplitCampusTypeInResults;
        FormsEngine.CurrentSmartMatches = data.CurrentSmartMatches;
        FormsEngine.CurrentUserSelections = data.CurrentUserSelections;
        FormsEngine.UserSmartMatched = data.UserSmartMatched;
        FormsEngine.UserShownManagedChoice = data.UserShownManagedChoice;
        FormsEngine.UserSubmittedManagedChoiceSelection = data.UserSubmittedManagedChoiceSelection;
        FormsEngine.UserSkippedToConfirmation = data.UserSkippedToConfirmation;

        FormsEngine.FormTemplateType = FormsEngine.FormTemplateType == undefined || FormsEngine.FormTemplateType == null ? data.FormTemplateType : FormsEngine.FormTemplateType;
        FormsEngine.ProgramTemplateId = FormsEngine.ProgramTemplateId == undefined || FormsEngine.ProgramTemplateId == null ? data.ProgramTemplateId : FormsEngine.ProgramTemplateId;
        FormsEngine.ProgramProductId = FormsEngine.ProgramProductId == undefined || FormsEngine.ProgramProductId == null ? data.ProgramProductId : FormsEngine.ProgramProductId;
        FormsEngine.ProductId = FormsEngine.ProductId == undefined || FormsEngine.ProductId == null ? data.ProductId : FormsEngine.ProductId;
        FormsEngine.InstitutionId = FormsEngine.InstitutionId == undefined || FormsEngine.InstitutionId == null ? data.InstitutionId : FormsEngine.InstitutionId;
        FormsEngine.ApplicationId = FormsEngine.ApplicationId == undefined || FormsEngine.ApplicationId == null ? data.ApplicationId : FormsEngine.ApplicationId;
        FormsEngine.InstitutionName = FormsEngine.InstitutionName == undefined || FormsEngine.InstitutionName == null ? data.InstitutionName : FormsEngine.InstitutionName;
        FormsEngine.ProgramName = FormsEngine.ProgramName == undefined || FormsEngine.ProgramName == null ? data.ProgramName : FormsEngine.ProgramName;

        // Additional questions should only be shown on initial step if Template is a ProgramWizard
        FormsEngine.ShowAllQuestionsOnFirstStep = (FormsEngine.FormTemplateType === 3 && data.ShowAllQuestionsOnFirstStep)

        callback();
    });
}

//Sets object to session
function fe_setSessionObject(key, value, callback) {
    fe_getSessionId(function () {
        var sUrl = FormsEngine.ServiceBaseURL + "/Session/SetObject?FESessionId=" + FormsEngine.FESessionId + "&key=" + key + "&value=" + value;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: sUrl,
            success: function (data) {
                fe_pushFormValuesToGTMDataLayer();
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    });
}


function fe_getWorkflowStatus(callback) {
    fe_getSessionId(function () {
        var sUrl = FormsEngine.ServiceBaseURL + "/Session/GetWorkflowStatus?FESessionId=" + FormsEngine.FESessionId;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            contentType: 'application/json; charset=utf-8',
            cache: false,
            url: sUrl,
            success: function (data) {
                callback(data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback();
            }
        });
    });
}


function fe_setWorkflowStatus(value, callback) {
    fe_getSessionId(function () {
        var sUrl = FormsEngine.ServiceBaseURL + "/Session/SetWorkflowStatus?FESessionId=" + FormsEngine.FESessionId;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            data: value,
            cache: false,
            url: sUrl,
            success: function (data) {
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        }).done(callback);
    });
}


function fe_getProgramTemplateResult(callback) {
    fe_getSessionObject("FE_Response", function (data) {
        callback(data);
    });
}

//Pixel area
function fe_setIframePixel(action) {
    fe_getSessionId(function () {
        fe_consolelog("PX:Getting pixels for page " + action);
        var URL = FormsEngine.ServiceBaseURL + "/Session/GetIframePixels?FESessionId=" + FormsEngine.FESessionId + "&PixelAction=" + action;
        fe_consolelog("PX:URL " + URL);
        if (jQuery("[id='FormsEngineIframePixels']").length < 1) {
            fe_consolelog("PX:Iframe not found injecting one ");

            var iframe = '<iframe id="FormsEngineIframePixels" style="display:none"></iframe>';
            jQuery('body').prepend(iframe);
        }
        fe_consolelog("PX: setting source of iFrame to URL ");
        jQuery('[id="FormsEngineIframePixels"]').attr('src', URL);
    });
}

//helper for ProgramTemplate ThankYou page pixel
function fe_loadPixelsForExtProgramTemplateTYPages() {
    fe_setIframePixel("loadPixelsForExtProgramTemplateTYPages");
}

//helper for AdvisingFlow NoMatch page pixel
function fe_loadPixelsForWizardAdvisingFlow() {
    fe_setIframePixel("loadpixelsforwizardadvisingflow");
}

//helper for cross sell initial pixels
function fe_loadInitialPixels() {
    fe_setIframePixel("loadInitialPixels");
}

//helper for Wizard ThankYou page pixel
function fe_loadPixelsForWizardTYPages() {
    fe_setIframePixel("loadPixelsForWizardTYPages");
}

function fe_loadPixelsForWizardMChoicePages() {
    fe_setIframePixel("loadPixelsForWizardMChoicePages");
}

function fe_loadJs(filename) {
    var script = document.createElement('script');
    script.type = "text/javascript";
    script.language = "javascript";
    script.src = FormsEngine.ServiceBaseURL + filename;
    document.body.appendChild(script);
}

function fe_loadJsWithCallback(filename, callback) {
    jQuery.getScript(FormsEngine.ServiceBaseURL + '/' + filename, function (data, textStatus, jqxhr) {
        fe_consolelog('JavaScript loaded:' + filename);
        if (callback) {
            callback();
        }
    });
}

function fe_loadCss(filename) {
    var id = 'CSS_FE_' + filename;
    if (!document.getElementById(id)) {
        var link = document.createElement("link");
        link.href = FormsEngine.ServiceBaseURL + '/Templates/Common/css/' + filename;
        link.id = id;
        link.type = "text/css";
        link.rel = "stylesheet";
        document.getElementsByTagName("head")[0].appendChild(link);
    }
}

function fe_submitProspectAdditionalInfo(Abandoned) {  // Abandoned=true <-- leave as true until final form submission
    // will only work if we have the ProspectId or Email
    var Email = (jQuery(FormsEngine.DefaultFormTag).find("input[code='Email']").exists()) ? jQuery(FormsEngine.DefaultFormTag).find("input[code='Email']").val() : '';

    // gather data
    if (!FormsEngine.ProspectAdditionalInfo) {
        FormsEngine.ProspectAdditionalInfo = [];
    }
    fe_pushProspectAdditionalInfoItem('Source', FormsEngine.Source);
    fe_pushProspectAdditionalInfoItem('FormURL', location.protocol + "//" + location.host + location.pathname);
    fe_pushProspectAdditionalInfoItem('TrackId', FormsEngine.TrackId);
    fe_pushProspectAdditionalInfoItem('Abandoned', Abandoned);

    //Additional prospect fields for limbo/abandon /responsys
    var categories = FormsEngine.LastCategory ? FormsEngine.LastCategory : jQuery(FormsEngine.DefaultFormTag).find(":input[name='Categories_Selections']").val();
    var catName = [];
    if (categories != undefined && categories != "") {
        var splitCat = categories.toString().split(",");
        jQuery.each(splitCat, function (index, item) {
            catName.push(encodeURIComponent(jQuery(FormsEngine.DefaultFormTag).find("label[for=Categories_" + item + "]").text()));
        });
    }
    fe_pushProspectAdditionalInfoItem('Categories', catName.join());

    var subcategories = FormsEngine.LastSubCategory ? FormsEngine.LastSubCategory : jQuery(FormsEngine.DefaultFormTag).find(":input[name='SubCategories_Selections']").val();
    var subName = [];
    if (subcategories != undefined && subcategories != "") {
        var splitSub = subcategories.toString().split(",");
        jQuery.each(splitSub, function (index, item) {
            subName.push(encodeURIComponent(jQuery(FormsEngine.DefaultFormTag).find("label[for=SubCategories_" + item + "]").text()));
        });
    }
    fe_pushProspectAdditionalInfoItem('SubCategories', subName.join());

    var specialties = FormsEngine.LastSpecialty ? FormsEngine.LastSpecialty : jQuery(FormsEngine.DefaultFormTag).find(":input[name='Specialties_Selections']").val();
    var spName = [];
    if (specialties != undefined && specialties != "") {
        var splitSp = specialties.toString().split(",");
        jQuery.each(splitSp, function (index, item) {
            spName.push(encodeURIComponent(jQuery(FormsEngine.DefaultFormTag).find("label[for=Specialties_" + item + "]").text()));
        });
    }
    fe_pushProspectAdditionalInfoItem('Specialties', spName.join());

    //job search fields
    if (FormsEngine.JobsKeywords) {
        fe_pushProspectAdditionalInfoItem('JobsKeywords', FormsEngine.JobsKeywords);
    }

    // NewsLetterOptIn specfic fields
    var newsLetterControl = jQuery(FormsEngine.DefaultFormTag).find("input[code='NewsLetterOptIn']");
    if (jQuery(newsLetterControl).exists()) {
        fe_pushProspectAdditionalInfoItem('NewsLetterOptIn_Text', jQuery(newsLetterControl).val().trim());
        fe_pushProspectAdditionalInfoItem('NewsLetterOptIn_Checked', jQuery(newsLetterControl).is(':checked') ? true : false);
    }

    // format for send
    var ProspectAdditionalInfoStr = '';
    jQuery.each(FormsEngine.ProspectAdditionalInfo, function (index, item) {
        ProspectAdditionalInfoStr = ProspectAdditionalInfoStr + "&" + FormsEngine.ProspectAdditionalInfo[index][0] + "=" + FormsEngine.ProspectAdditionalInfo[index][1];
    });

    var Request = "ProspectId=" + FormsEngine.ProspectId;
    Request += "&Email=" + Email;
    Request += "&ProspectAdditionalData=" + encodeURIComponent(ProspectAdditionalInfoStr);

    if (FormsEngine.LastProspectAdditionalInfo != Request) {
        var sUrl = FormsEngine.ServiceBaseURL + "/TemplateManager/SaveProspectAdditionalInfo?" + Request;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: sUrl,
            success: function (data) {
                if (data > 0) {
                    FormsEngine.ProspectId = data;
                    fe_consolelog('Prospect Additional Info save complete. ProspectId=' + data);
                    FormsEngine.LastProspectAdditionalInfo = Request;
                }
                else {
                    fe_consolelog('Prospect Additional Info save did not complete. ProspectId was returned as : ' + data);
                }
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        });
    }
}

function fe_pushProspectAdditionalInfoItem(key, value) {
    FormsEngine.ProspectAdditionalInfo = FormsEngine.ProspectAdditionalInfo || [];
    var keyExists = false;
    jQuery(FormsEngine.ProspectAdditionalInfo).each(function () {
        if (this[0] == key) {
            keyExists = true;
            this[1] = value;
        }
    });
    if (!keyExists) {
        FormsEngine.ProspectAdditionalInfo.push([key, value]);
    }
}

//Serializes form and stores info in server session
function fe_saveForm() {

    if (FormsEngine.RecoveryMode == false && (FormsEngine.SelectAllTriggered == undefined || !FormsEngine.SelectAllTriggered)) {
        var inputs = fe_serializeInputsInContainer(FormsEngine.DefaultFormTag);
        var FormData = encodeURIComponent(inputs);
        if (FormData != FormsEngine.LastSerializedFormData) {
            fe_setSessionObject("WFORM", FormData, function () { });
            FormsEngine.LastSerializedFormData = FormData;
        }
    }
    return false;
}

function fe_saveFormDynamicQuestions() {
    if (FormsEngine.RecoveryMode == false && (FormsEngine.SelectAllTriggered == undefined || !FormsEngine.SelectAllTriggered)) {
        var AdditionalQuestionStep = jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']');
        var AdditionalQuestionsFormData = encodeURIComponent(fe_serializeInputsInContainer(AdditionalQuestionStep));
        if (AdditionalQuestionsFormData != FormsEngine.LastSerializedAdditionalFormData) {
            fe_setSessionObject("WFORM_DynamicQuestions", AdditionalQuestionsFormData, function () { });
            FormsEngine.LastSerializedAdditionalFormData = AdditionalQuestionsFormData;
        }
    }
    return false;
}

function fe_saveManagedChoiceDynamicQuestions() {
    if (FormsEngine.SelectAllTriggered == undefined || !FormsEngine.SelectAllTriggered) {
        var AdditionalQuestionStep = jQuery(FormsEngine.DefaultFormTag).find('div[name="step"][data-step=' + FormsEngine.StepDynamicQuestions + ']');
        var AdditionalQuestionsFormData = encodeURIComponent(fe_serializeInputsInContainer(AdditionalQuestionStep));
        if (AdditionalQuestionsFormData != FormsEngine.LastSerializedAdditionalFormData) {
            fe_setSessionObject("WFORM_DynamicQuestions", AdditionalQuestionsFormData, function () { });
            FormsEngine.LastSerializedAdditionalFormData = AdditionalQuestionsFormData;
        }
    }
    return false;
}

function fe_serializeInputsInContainer(container) {
    //get all inputs in container
    var nameValuePairStringArray = [];
    var namesInArray = [];
    var inputCollection = jQuery(container).find(':input');

    //loop through all inputs
    for (i = 0; i < inputCollection.length; i++) {
        var currName = jQuery(inputCollection[i]).attr('name');
        var currCode = jQuery(inputCollection[i]).attr('code');
        var IsRadio = jQuery(inputCollection[i]).attr('type') == "radio";
        var currValue = encodeURIComponent(jQuery(inputCollection[i]).val());
        if (currCode == 'UserAgreement' || currCode == "EDDYUserAgreement") {
            continue;
        }
        if (IsRadio == true) {
            //its a radio button so get the value that is actually selected (name and code will be same for all radios in group)
            currValue = encodeURIComponent(jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + currCode + "']:checked").val());
        }

        //if were here we need to be adding this control
        if (currName != undefined && currName != 'undefined' && jQuery.inArray(currName, namesInArray) == -1) {
            //only add if not in the collection
            nameValuePairStringArray.push(currName + '=' + currValue);
            namesInArray.push(currName);
        }
        if (currCode != undefined && currCode != 'undefined' && currCode != '' && currName != currCode) {
            //if code <> name and both exist add by code to collection
            if (jQuery.inArray(currCode, namesInArray) == -1) {
                //only add if not in the collection
                nameValuePairStringArray.push(currCode + '=' + currValue);
                namesInArray.push(currCode);
            }
        }
    }
    //return collection joined by '&'
    return nameValuePairStringArray.join('&');
}

function fe_pushFormValuesToGTMDataLayer() {
   try {
        var dataLayerVariables = {};

        var leadDataArray = jQuery(FormsEngine.DefaultFormTag).serializeArray();

        jQuery.each(leadDataArray, function (index, item) {

            var field = jQuery(FormsEngine.DefaultFormTag).find(":input[name='" + item.name + "']").first();
            var code = field.attr("code");
            var step = Number(field.parents(".steps").attr("data-step"));

            if (code && !fe_isCodePII(code) && (!FormsEngine.FormTemplateType || step <= FormsEngine.CurrentStep)) {
                code = code.toLowerCase();

                switch (code) {
                    case "age":
                        if (field.val()) {
                            dataLayerVariables[code] = fe_getAgeGroup(field.val());
                        }
                        break;
                    case "year_of_highest_education_completed":
                        if (field.val() && field.attr("controltypename") === "Drop-Down") {
                            var gradYear = fe_getDropdownSelectedOptionText(field);
                            dataLayerVariables[code] = fe_getGradYearGroup(gradYear);
                        }
                        break;
                    case "categories":
                    case "subcategories":
                        dataLayerVariables[code] = fe_getInterestSelectionNames(code);
                        break;
                    case "military_affiliation":
                        dataLayerVariables[code] = item.value != "126" ? "yes" : "no";
                        break;
                    default:
                        dataLayerVariables[code] = fe_getDefaultFieldValueForGTMDataLayer(field);
                        break;
                }
            }
        });

        fe_pushToGTMDataLayer(dataLayerVariables);
    } catch (e) {
        fe_consolelog(e);
    }
}

function fe_pushSingleFieldToGTMDataLayer(code, value) {

    try {
        if (code && value && !fe_isCodePII(code)) {
            code = code.toLowerCase();
            value = value.toLowerCase();

            var dataLayerVariables = {};
            dataLayerVariables[code] = value;
            fe_pushToGTMDataLayer(dataLayerVariables);
        }
    } catch (e) {
        fe_consolelog(e);
    }
}


function fe_getDropdownSelectedOptionText(dropdown) {
    var optionText = ""

    var selectedOption = dropdown.find("option:selected").first();

    if (selectedOption && selectedOption.text()) {
        optionText = selectedOption.text().toLowerCase();
    }

    return optionText;
}

function fe_getAgeGroup(age) {
    var ageGroup = "";

    if (age > 0 && age < 17) {
        ageGroup = "1-16";
    } else if (age > 16 && age < 22) {
        ageGroup = age;
    } else if (age > 21 && age < 35) {
        ageGroup = "22-34";
    } else if (age > 34 && age < 45) {
        ageGroup = "35-44";
    } else if (age > 44 && age < 55) {
        ageGroup = "45-54";
    } else if (age > 54 && age < 65) {
        ageGroup = "55-64";
    } else if (age > 64) {
        ageGroup = "65+";
    }

    return ageGroup;
}

function fe_getGradYearGroup(year) {
    var gradYearGroup = ""

    var currentYear = new Date().getFullYear();

    if (year == currentYear) {
        gradYearGroup = "Current year";
    } else if (year == currentYear - 1) {
        gradYearGroup = "Previous year";
    } else if (year == currentYear - 2) {
        gradYearGroup = "2 years prior";
    } else if (year == currentYear - 3) {
        gradYearGroup = "3 years prior";
    } else if (year < 1976) {
        gradYearGroup = "Less than 1976";
    } else {
        var upperBoundYear = currentYear - 4;
        var lowerBoundYear = upperBoundYear - 19;

        while (lowerBoundYear >= 1976) {
            if (year >= lowerBoundYear && year <= upperBoundYear) {
                gradYearGroup = upperBoundYear + "-" + lowerBoundYear;
                break;
            }

            upperBoundYear = upperBoundYear - 20;
            lowerBoundYear = upperBoundYear - 19;
        }
    }

    return gradYearGroup;
}

function fe_getInterestSelectionNames(code) {
    var names = [];
    var hiddenInputName = code + "_selections";
    var inputs = jQuery(FormsEngine.DefaultFormTag).find("input");
    var hiddenSelectionsField = fe_filterElements(inputs, "name", hiddenInputName).first();

    if (hiddenSelectionsField) {

        var commaDelimitedIds = hiddenSelectionsField.val()
        if (commaDelimitedIds) {
            var ids = commaDelimitedIds.split(",");
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var inputFieldId = code + "_" + id;
                var labels = jQuery(FormsEngine.DefaultFormTag).find("label");
                var label = fe_filterElements(labels, "for", inputFieldId).first();

                if (label && label.text()) {
                    names.push(label.text().toLowerCase());
                }
            }
        }
    }

    return names;
}

function fe_filterElements(elements, htmlAttributeName, value) {
    return elements.filter(function () {
        var result = false;

        try {
            var attr = jQuery(this).attr(htmlAttributeName);
            if (attr) {
                result = attr.toLowerCase() === value;
            }
        } catch (e) {
            fe_consolelog(e);
        }

        return result;
    });
}

function fe_pushToGTMDataLayer(data) {
    if (data && typeof dataLayer !== 'undefined') {
        dataLayer.push(data);
    }
}

function fe_getDefaultFieldValueForGTMDataLayer(field) {
    var value = "";

    if (field && field.val()) {

        var controlTypeName = field.attr("controltypename");

        if (controlTypeName === "Radio Buttons") {
            value = fe_getRadioButtonFieldValueForGTMDataLayer(field);
        } else if (controlTypeName === "Text Box") {
            value = field.val().toLowerCase();
        } else if (controlTypeName === "Drop-Down") {
            value = fe_getDropdownSelectedOptionText(field);
        } else if (controlTypeName === "Multi Check Box List") {
            value = fe_getCheckboxFieldValuesForGTMDataLayer(field);
        }
    }

    return value;
}

function fe_getRadioButtonFieldValueForGTMDataLayer(field) {
    var value = "";

    var fieldName = field.attr("name");
    if (fieldName) {
        var selectedRadioBtn = jQuery(FormsEngine.DefaultFormTag).find(":input[name='" + fieldName + "']:checked").first();

        if (selectedRadioBtn && selectedRadioBtn.val()) {
            value = selectedRadioBtn.val().toLowerCase();
        }
    }

    return value;
}

function fe_getCheckboxFieldValuesForGTMDataLayer(field) {
    var checkboxFieldLabels = [];

    if (field) {
        jQuery(FormsEngine.DefaultFormTag).find("input[code='" + field.attr("code") + "']:checked").each(function (checkboxIndex, checkboxField) {
            var id = jQuery(checkboxField).attr("id");
            if (id) {
                var label = jQuery(FormsEngine.DefaultFormTag).find("label[for='" + id + "']").first();
                if (label && label.text()) {
                    checkboxFieldLabels.push(label.text().toLowerCase().trim());
                }
            }
        });
    }

    return checkboxFieldLabels;
}

function fe_isCodePII(code) {
    var result = false;
    var piiCodes = ["first_name", "last_name", "phone", "alternate_phone", "email", "address", "address_2"];

    if (code) {
        result = piiCodes.indexOf(code.toLowerCase()) > -1;
    }

    return result;
}

//Gets arguments from string by name case insensitive
function fe_getParameterByNameFromString(name, string) {
    match = new RegExp('(?:^|&)' + name + '=([^&]*)', 'i').exec(string);
    return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
}

//Gets querystring arguments by name case insensitive
function fe_getParameterByName(name) {
    match = new RegExp('[?&]' + name + '=([^&]*)', 'i').exec(window.location.search);
    return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')) : '';
}

//Gets querystring passed arguments
function fe_getQuerystring() {
    return window.location.search.replace("?", "");
}

//Gets string in the format name1=value&name2=value... into an object result[name] = value
function fe_getUriToObject(uri) {
    var result = {};
    uri.replace(
        new RegExp("([^?=&]+)(=([^&]*))?", "g"),
        function ($0, $1, $2, $3) { result[$1] = $3; }
    );
    return result;
}

//debug function
function fe_debugShowAllSteps() {
    jQuery(FormsEngine.DefaultFormTag).find("div[id^='Step']").show();
}

function fe_getParameterByNameAndAliasFromString(code, string) {
    var ControlValue = "";

    //Based on control code
    ControlValue = fe_getParameterByNameFromString(code, string);

    //Based on aliases if supported
    if (ControlValue == undefined || ControlValue == "") {
        var alias = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']").attr("alias");
        if (alias != undefined && alias != "") {
            var aliasList = alias.split(',');
            for (i = 0; i < aliasList.length; i++) {
                ControlValue = fe_getParameterByNameFromString(aliasList[i], string);
                if (ControlValue != undefined && ControlValue != "") {
                    return ControlValue;
                }
            }
        }
    }
    return ControlValue;
}


function fe_getParameterByNameAndAlias(control, code) {
    var ControlValue = "";

    //Based on control code
    ControlValue = fe_getParameterByName(code);

    //Based on aliases if supported
    if (ControlValue == undefined || ControlValue == "") {
        var alias = jQuery(control).attr("alias");
        if (alias != undefined && alias != "") {
            var aliasList = alias.split(',');
            for (i = 0; i < aliasList.length; i++) {
                ControlValue = fe_getParameterByName(aliasList[i]);
                if (ControlValue != undefined && ControlValue != "") {
                    return ControlValue;
                }
            }
        }
    }
    return ControlValue;
}

//this function will get all textbox inputs from the form and remove any '&' or '=' characters from the data.
//as the form can pass a query string of lead data to our service these characters must be reserved for that process
//and can not be allowed in the input fields.
function fe_cleanFormData() {
    jQuery(FormsEngine.DefaultFormTag).find(":input[type='text']")
        .each(function () {
            if (!jQuery(this).is(':radio') && !jQuery(this).is(':checkbox')) {
                //set the value of each control to the cleaned version of its current value
                jQuery(this).val(fe_cleanStringOfRestrictedCharacters(jQuery(this).val()));
            }
        });
}

function fe_cleanStringOfRestrictedCharacters(theValue) {
    theValue = theValue.replace(new RegExp('&', 'g'), '')
    theValue = theValue.replace(new RegExp('=', 'g'), '');
    return jQuery.trim(theValue);
}

function fe_getFormData() {
    var FormData = {};
    FormData.LeadData = "";
    FormData.LeadAdditionalData = "";
    var LeadDataArray = jQuery(FormsEngine.DefaultFormTag).serializeArray();

    var preAmp = "";
    var preAmpAddtl = "";

    jQuery.each(LeadDataArray, function (index, item) {
        preAmp = FormData.LeadData == "" ? "" : "&";
        preAmpAddtl = FormData.LeadAdditionalData == "" ? "" : "&";
        var field = jQuery(FormsEngine.DefaultFormTag).find(":input[name='" + LeadDataArray[index].name + "']");

        var code = jQuery(field).attr("code");
        if (code == 'undefined' || code == undefined) {
            return true;
        }

        if (code == "UserAgreement" || code == "EDDYUserAgreement") {
            FormData.LeadData = FormData.LeadData + preAmp + '"' + code + "=" + fe_stripHtmlFromText(LeadDataArray[index].value.replace(/&/g, "amp;")) + '"';
        }
        else if (code == "DynamicCampusSoftPreference") {
            FormData.LeadData = FormData.LeadData + preAmp + code + "=" + LeadDataArray[index].value;
            FormData.LeadData = FormData.LeadData + preAmp + 'CampusSoftPreference' + "=" + LeadDataArray[index].value;
        }
        else if (code == "Desired_Degree_Level") {
            var ddl = "";

            if (jQuery(FormsEngine.DefaultFormTag).find('input[code="Desired_Degree_Level"]').is(':checkbox')) {
                ddl = jQuery(FormsEngine.DefaultFormTag).find(':input[code="Desired_Degree_Level"]:checked').map(function () { return this.value; }).get().join(',');
            } else {
                ddl = jQuery(FormsEngine.DefaultFormTag).find(':input[code="Desired_Degree_Level"]').val();
            }
            FormData.LeadData = FormData.LeadData + preAmp + code + "=" + ddl;

        }
        else if (code == "School_Picker") {
            FormData.LeadData = FormData.LeadData + preAmp + code + "=" + JSON.stringify(FormsEngine.SchoolPickerSelections);
        }
        else {
            FormData.LeadData = FormData.LeadData + preAmp + code + "=" + fe_encodeSpecialCharacters(LeadDataArray[index].value);
        }

        //TEMP solution to get email for UpdateWidget request
        if (code == "Email") {
            FormsEngine.ProspectEmail = LeadDataArray[index].value;
        }

        var value = "";
        if (jQuery(field).is('select')) {
            value = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']>option:selected").attr('key')
        }
        else if (jQuery(field).is(':radio')) {
            value = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']:checked").attr('key');
        }
        else {
            value = jQuery(field).attr('key');
        }
        if (value != undefined && value != "") {
            FormData.LeadAdditionalData = FormData.LeadAdditionalData + preAmpAddtl + code + "-key=" + fe_encodeSpecialCharacters(value);
        }

    });

    return FormData;
}

function fe_checkIsMobile() {
    var check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
}

function fe_ScrollToTop() {
    // Check if Device is Mobile..
    var isMobile = fe_checkIsMobile();
    var offset = jQuery('[name="' + FormsEngine.RenderingDiv + '"]').offset();

    //No auto scrolling if you are on the first step of the form to avoid page jumping if set at the bottom
    if (FormsEngine.CurrentStep > 1) {

        if (offset && FormsEngine.FormScrollOffset) {
            offset.top -= FormsEngine.FormScrollOffset;
        }
        //No auto scroll on desktop if set
        if (!isMobile && FormsEngine.DisableScrolltoTopDesktop === true) {
            return;
        }

        if (!jQuery(FormsEngine.DefaultFormTag).find('[name="' + FormsEngine.RenderingDiv + '"]').visible()
            && (isMobile || !(FormsEngine.DisableScrolltoTop === true))) {
            if (offset != undefined && offset != null) {
                jQuery('html, body').scrollTop(offset.top);
            }
        }
    }
}

function fe_getResourceMetaDataTextForKey(Keys, callback) {
    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetResourceMetaDataTextForKey?Keys=" + Keys;
    jQuery.ajax({
        async: false,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
            callback('');
        }
    });
}

function fe_getEMSInstitutionTCPAMessage(institutionId, callback) {
    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetEMSInstitutionTCPAText?InstitutionId=" + institutionId;
    jQuery.ajax({
        async: false,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
            callback('');
        }
    });
}

function fe_setEMSInstitutionTCPAMessage(institutionId) {
    fe_getEMSInstitutionTCPAMessage(institutionId, function (tcpaMessage) {
        if (tcpaMessage != null && tcpaMessage.trim() != "") {
            FormsEngine.ResourceData["EMS_INSTITUTION_TCPA_MESSAGE"] = tcpaMessage;
        }
    });
}

function fe_getResourceMetaDataTextForTCPA(callback) {
    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetResourceMetaDataTextForTCPA";
    jQuery.ajax({
        async: false,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
            callback('');
        }
    });
}

function fe_getFormFieldsSessionValues(fields, callback, isWizard) {
    var fieldList = fields.join(',');
    var wizard = true;

    if (isWizard === false) {
        wizard = false;
    }

    fe_getSessionId(function () {
        var sUrl = FormsEngine.ServiceBaseURL + "/Session/GetFormSessionValues?FESessionId=" + FormsEngine.FESessionId + "&fieldKeys=" + fieldList + "&isWizard=" + wizard;
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: sUrl,
            success: function (data) {
                callback(data)
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
                callback('');
            }
        });
    });
}

function fe_getISMapping(set, category, source) {
    return jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: FormsEngine.ServiceBaseURL + "/FormValidation/GetISMapping?set=" + set + "&category=" + category + "&source=" + source,
    });
}


function fe_getCampaignDetailByTrackId(TrackId, callback) {
    var sUrl = FormsEngine.ServiceBaseURL + "/Matching/GetCampaignDetailByTrackId?TrackId=" + TrackId;

    jQuery.ajax({
        async: false,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            var result = new Object();
            result.MaxSmartMatchCount = 3;
            result.AdditionalQuestionsOnlyInSchoolSelection = false;
            result.AdditionalQuestionsFromSmartMatch = false;
            callback(result);
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
        }
    });
}

function fe_getLandingPageSettings(FormLeadUrl, callback) {
    var sUrl = FormsEngine.ServiceBaseURL + "/DataBind/GetLandingPageSettings?FormLeadUrl=" + FormLeadUrl;

    jQuery.ajax({
        async: false,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            fe_consolelog(data);
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            var result = new Object();
            callback(result);
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
        }
    });
}

//Gets the control value and value-key if is a me-integration field
function fe_getControlValue(controlCode) {

    var dependantField = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + controlCode + "']");
    var ControlValue = {};
    ControlValue.value = "";
    ControlValue.valueKey = "";
    ControlValue.requiresKey = jQuery(dependantField).attr("me-filter") != undefined;

    if (jQuery(dependantField).is('select')) {
        ControlValue.value = jQuery(dependantField).val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + controlCode + "']>option:selected").attr('key');
        }
    }
    else if (jQuery(dependantField).is(':radio')) {
        ControlValue.value = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + controlCode + "']:checked").val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + controlCode + "']:checked").attr('key');
        }
    }
    else {
        ControlValue.value = jQuery(dependantField).val();
        if (ControlValue.requiresKey) {
            ControlValue.valueKey = jQuery(dependantField).attr('key');
        }
    }

    ControlValue.value = ControlValue.value == null || ControlValue.value == undefined ? "" : ControlValue.value;
    ControlValue.valueKey = ControlValue.valueKey == null || ControlValue.valueKey == undefined ? "" : ControlValue.valueKey;

    return ControlValue;
}

function fe_setControlValue(controlCode, controlValue) {
   
    var Field = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + controlCode + "']");
    if (Field != undefined && controlCode != "UserAgreement" && controlCode != "EDDYUserAgreement") {

        if (jQuery(Field).is(':radio')) {
            jQuery(FormsEngine.DefaultFormTag).find("[type=radio][code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
        }
        else if (jQuery(Field).is(':checkbox')) {
            var splitVal = controlValue.split(",");
            if (splitVal.length > 1) { //added for mutiselect checkbox support
                jQuery.each(splitVal, function (i, e) {
                    jQuery(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + controlCode + "'][value='" + e + "']").prop('checked', true);
                });
            }
            else {
                jQuery(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
            }

        }
        else if (jQuery(Field).is('select')) {
            if (jQuery(Field).find("option[value='" + controlValue + "']").length > 0) { //this is a select so only set the value if the value is in the list
                jQuery(Field).val(controlValue);

                //jquery responsive
                jQuery(FormsEngine.DefaultFormTag).find(":input[parent-code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true);
            }
        }
        else {
            jQuery(Field).val(controlValue);

            //jquery responsive
            jQuery(FormsEngine.DefaultFormTag).find(":input[parent-code='" + controlCode + "'][value='" + controlValue + "']").prop('checked', true)
        }

        if (controlCode == 'Military_Affiliation') {
            fe_processSmartMilitary(controlValue);
        }
        if (controlCode == 'Highest_Level_of_Education_Completed') {
            fe_processSmartHighestEducationLevel(controlValue);
        }

        fe_consolelog("SetControlValue Code: " + controlCode + ", Value: " + controlValue);
        if (FormsEngine.OnControlValueSet) {
            FormsEngine.OnControlValueSet(controlCode, controlValue);
        }

        jQuery(FormsEngine.DefaultFormTag).trigger("controlLoadedFromSession");
    }
}

function fe_getDistinctList(list) {
    var result = [];
    jQuery.each(list, function (i, e) {
        if (jQuery.inArray(e, result) == -1) result.push(e);
    });
    return result;
}



//Tries to read AffiliateId and SessionId from site cookies
function fe_setSettingsFromCookies() {
    var affiliateId = FormsEngine.readCookie('_AffiliateID');
    FormsEngine.AffiliateId = affiliateId != null && affiliateId != "" && affiliateId != undefined ? affiliateId : fe_getParameterByName("aid");
    FormsEngine.SessionId = FormsEngine.readCookie('_Session') != null ? FormsEngine.readCookie('_Session') : FormsEngine.SessionId;
    FormsEngine.DeviceId = FormsEngine.readCookie('_Device') != null ? FormsEngine.readCookie('_Device') : FormsEngine.DeviceId;
}



//handle alternate templates
function fe_getAlternativeTemplateId(requestedTemplateId) {
    var result = requestedTemplateId;

    if (FormsEngine.AlternativeTemplates) {
        for (var i = 0; i < FormsEngine.AlternativeTemplates.length; i++) {
            if (FormsEngine.AlternativeTemplates[i].TemplateId == requestedTemplateId) {
                result = FormsEngine.AlternativeTemplates[i].AlternativeId;
                break;
            }
        }
    }
    return result;
}

function fe_serializeAlternativeTemplates() {
    var r = [];
    if (FormsEngine.AlternativeTemplates) {
        for (var i = 0; i < FormsEngine.AlternativeTemplates.length; i++) {
            r.push(FormsEngine.AlternativeTemplates[i].TemplateId.toString() + "=" + FormsEngine.AlternativeTemplates[i].AlternativeId.toString());
        }
    }
    return r.join(',');
}

function fe_trimAsterisk(s) {
    //s = s.trim();
    if (s.substring(0, 1) == '*') {
        return s.substring(1);
    }
    else {
        return s;
    }
}


//57653 - all exit pops/no match pages - map to categories - Form Changes
function fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(item, emptyAll) {
    try {
        var cookieCategorySubCategorySpecialtyName = 'CategorySubCategorySpecialty';
        var CategorySubCategorySpecialty = jQuery.parseJSON(decodeURIComponent(FormsEngine.readCookie(cookieCategorySubCategorySpecialtyName))) || { CATEGORIES: [], SUBCATEGORIES: [], SPECIALTIES: [] };
        if (CategorySubCategorySpecialty[item.type.toUpperCase()] != undefined && CategorySubCategorySpecialty[item.type.toUpperCase()] != null) {

            if (emptyAll) CategorySubCategorySpecialty[item.type.toUpperCase()] = [];

            CategorySubCategorySpecialty[item.type.toUpperCase()] = jQuery.grep(CategorySubCategorySpecialty[item.type.toUpperCase()], function (itemInner) { return itemInner.id !== item.id; });
            if (item.operation) { CategorySubCategorySpecialty[item.type.toUpperCase()].push({ id: item.id, text: item.text }); }

            FormsEngine.createCookie(cookieCategorySubCategorySpecialtyName, encodeURIComponent(JSON.stringify(CategorySubCategorySpecialty)), 0);
        }
    }
    catch (e) { fe_consolelog(e); }
}
//57653 - END

//Function for Optimizely customization function call if exist.
function fe_OptimizelyCustomization(param) { if (typeof (window.FormsEngine.OptimizelyCustomization) === 'function') { window.FormsEngine.OptimizelyCustomization(param); } }


//Google Tag Manager events
function fe_googleTagEvent(event, category, action, label) {
    try {
        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ "event": event, "eventCategory": category, "eventAction": action, "eventLabel": label });
        }
    } catch (e) { fe_consolelog(e); }
}

function fe_formatPhone(phone) {
    var result = '';
    if (phone.length >= 10) {
        result = phone.toString().substring(0, 3) + '.' + phone.toString().substring(3, 6) + '.' + phone.toString().substring(6);
    }

    return result;
}

function fe_replaceTag(uatext, tag, value) {
    //i.e.  uatext:  my number(s) {mobile-number} included above {/mobile-number}
    // tag: {mobile-number}
    // value: 561.204.3290 
    // Result --> my number(s) 562.204.3290
    // else if value is empty default the text inside tags
    // value: ''
    // Result --> my number(s) included above
    var endTag = "{/" + tag.substring(1);
    var start = uatext.indexOf(tag);
    var end = uatext.indexOf(endTag);
    while (start > -1) {
        if (start > -1 && end > -1) {
            if (value.length > 0) {
                uatext = uatext.replace(uatext.substring(start, end + endTag.length), value);
            }
            else {
                uatext = uatext.replace(tag, '');
                uatext = uatext.replace(endTag, '');
            }
        }
        else if (start > -1) {
            uatext = uatext.replace(tag, value);
        }
        start = uatext.indexOf(tag);
        end = uatext.indexOf(endTag);
    }

    return uatext;
}


//Gets program detail information if required
function fe_getProgramDetail() {
    if (FormsEngine.ProgramChangedEvent) {
        fe_getProgramDetailFromMatchingEngine(FormsEngine.ProgramChangedEvent);
    }
}

//Gets program detail information if required for program wizards
function fe_getProgramWizardDetail() {
    if (FormsEngine.ProgramWizardDetailChangedEvent) {
        fe_getProgramDetailFromMatchingEngine(FormsEngine.ProgramWizardDetailChangedEvent);
    }
}
function fe_getProgramDetailFromMatchingEngine(callBack) {
    var selectedProgram = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");

    if (jQuery(selectedProgram).val()) {
        //get program info
        var programId = jQuery(selectedProgram).val();
        var ags = "?ProgramId=" + programId + "&IsBeta=" + FormsEngine.IsBeta + "&TrackId=" + FormsEngine.TrackId;
        var sUrl = FormsEngine.ServiceBaseURL + "/Matching/GetProgramDetail" + ags;

        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: sUrl,
            success: function (data) {
                callBack(data);
            },
            error: function (request, textStatus, errorThrown) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                fe_logClientException(request, sUrl, errorThrown);
            }
        });
    }
}

function fe_getInstitutionDetail() {
    if (FormsEngine.InstitutionDetailLoaded) {
        fe_makeGetInstitutionRequest(FormsEngine.InstitutionDetailLoaded);
    }
}

function fe_makeGetInstitutionRequest(callback) {

    var ags = "?InstitutionId=" + FormsEngine.InstitutionId;
    ags += "&IsBeta=" + FormsEngine.IsBeta;
    ags += "&TrackId=" + FormsEngine.TrackId;
    ags += "&ApplicationId=" + FormsEngine.ApplicationId;
    var sUrl = FormsEngine.ServiceBaseURL + "/Institution/GetInstitution" + ags;

    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: sUrl,
        success: function (data) {
            callback(data);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            fe_logClientException(request, sUrl, errorThrown);
        }
    });
}

function fe_initialize_Leadid() {

    var emsApplicationId = 27;

    if (FormsEngine.ApplicationId === emsApplicationId) {
        FormsEngine.JornayaInitialized = false;
        return;
    }

    //LeadId
    // Audited by LeadId 6/2/2016
    jQuery(FormsEngine.DefaultFormTag).find('input[code="UserAgreement"]').attr("name", "leadid_tcpa_disclosure_1");
    FormsEngine.JornayaInitialized = true;

    //LeadiD external javascript events
    if (jQuery(FormsEngine.DefaultFormTag).find(':input[code="leadid_token"]').length <= 0) {
        jQuery("<input>").attr({
            id: 'leadid_token',
            type: 'hidden',
            code: 'leadid_token',
            name: 'universal_leadid'
        }).appendTo(FormsEngine.DefaultFormTag);
    }

    if (typeof LeadiD == 'undefined') {
        var script = document.createElement("script");
        script.id = "LeadiDscript";
        script.type = "text/javascript";
        script.language = "javascript";

        if (typeof FormsEngine.CampaignDetail != 'undefined' && FormsEngine.CampaignDetail.IsCallCenter == true) {
            script.text += "(function () { var s = document.createElement('script'); s.id = 'LeadiDscript_campaign'; s.type = 'text/javascript'; s.async = true; s.src = (document.location.protocol + '//create.lidstatic.com/campaign/47f1c035-06a9-8854-dc11-cc164cf5f7b7.js?snippet_version=2&f=reset'); var LeadiDscript = document.getElementById('LeadiDscript'); LeadiDscript.parentNode.insertBefore(s, LeadiDscript); })();";
        }
        else {
            script.text += "(function () { var s = document.createElement('script'); s.id = 'LeadiDscript_campaign'; s.type = 'text/javascript'; s.async = true; s.src = (document.location.protocol + '//create.lidstatic.com/campaign/50181952-3a30-427b-a8a9-4b010a76311c.js'); var LeadiDscript = document.getElementById('LeadiDscript'); LeadiDscript.parentNode.insertBefore(s, LeadiDscript); })();";
        }
        document.body.appendChild(script);
        FormsEngine.JornayaJavascriptAppended = true;
    }

    //LeadId changes for express consent
    try {
        if (typeof LeadiD != 'undefined') {
            LeadiD.formcapture.init();
        }
    }
    catch (ex) { }
}
function fe_ModifyInputs_ActiveProspect() {

    if (FormsEngine.ApplicationId !== 27) {
        var tokenField = jQuery(FormsEngine.DefaultFormTag).find(":input[name='xxTrustedFormToken']");
        if (tokenField.length > 0) {
            if (!tokenField.attr('code')) {
                tokenField.attr('code', 'xxTrustedFormToken');
            }
        }
        var certUrlField = jQuery(FormsEngine.DefaultFormTag).find(":input[name='xxTrustedFormCertUrl']");
        if (certUrlField.length > 0) {
            if (!certUrlField.attr('code')) {
                certUrlField.attr('code', 'xxTrustedFormCertUrl');
            }
        }
    }
}

function fe_initialize_ActiveProspect() {

    if (FormsEngine.ApplicationId !== 27) {

        var script = document.createElement("script");
        script.id = "activeProspectscript";
        script.type = "text/javascript";
        script.language = "javascript";
        script.text = "(function () {var field = 'xxTrustedFormCertUrl';var provideReferrer = false;var invertFieldSensitivity = false; var useTaggedConsent = true;var tf = document.createElement('script');tf.type = 'text/javascript'; tf.async = true;tf.src = 'http' + ('https:' == document.location.protocol ? 's' : '') +'://api.trustedform.com/trustedform.js?provide_referrer=' + escape(provideReferrer) + '&field=' + escape(field) + '&l=' + new Date().getTime() + Math.random() + '&invert_field_sensitivity=' + invertFieldSensitivity + '&use_tagged_consent=' + useTaggedConsent ;var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(tf, s);})();"
        document.body.appendChild(script);


        var targetNode = jQuery(FormsEngine.DefaultFormTag)[0]; // Get the DOM element
        if (targetNode) {
            var observer = new MutationObserver(function (mutationsList) {
                for (var mutation of mutationsList) {
                    if (mutation.type === 'childList') {
                        fe_ModifyInputs_ActiveProspect(); // Call the function when nodes are inserted
                    }
                }
            });

            var config = { childList: true, subtree: true }; // Configuration of the observer

            observer.observe(targetNode, config); // Start observing the target element
        }
        

        //jQuery(FormsEngine.DefaultFormTag).bind("DOMNodeInserted", function () {
        //    fe_ModifyInputs_ActiveProspect();
        //});
    }
}



//method to check for responsive label and watermark
function fe_checkResponsiveItems() {
    //if were in a responsive form, and the window is less than 640px we need to apply some changes. do that here.
    if (formLessThanSpecifiedWidth(640)) {
        //hide labels 
        changeLabelForResponsive(true);
        //use mobile watermarks
        swapWatermark(true);

        if (FormsEngine.ShowSelectAllMobile == true) {
            //show the select all buttons
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-categories').css('display', 'inline-block');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-categories').css('display', 'inline-block');
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-subcategories').css('display', 'inline-block');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-subcategories').css('display', 'inline-block');
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-specialties').css('display', 'inline-block');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-specialties').css('display', 'inline-block');
        }
        else {
            //hide the select all buttons
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-categories').css('display', 'none');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-categories').css('display', 'none');
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-subcategories').css('display', 'none');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-subcategories').css('display', 'none');
            jQuery(FormsEngine.DefaultFormTag).find('.select-all-specialties').css('display', 'none');
            jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-specialties').css('display', 'none');
        }
    }
    else {
        //make sure that any labels with hide label attribute are showing
        changeLabelForResponsive(false);
        //make sure watermark is not mobile watermark.
        swapWatermark(false);

        //show select all buttons because were on desktop
        jQuery(FormsEngine.DefaultFormTag).find('.select-all-categories').css('display', 'inline-block');
        jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-categories').css('display', 'inline-block');
        jQuery(FormsEngine.DefaultFormTag).find('.select-all-subcategories').css('display', 'inline-block');
        jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-subcategories').css('display', 'inline-block');
        jQuery(FormsEngine.DefaultFormTag).find('.select-all-subcategories').css('display', 'inline-block');
        jQuery(FormsEngine.DefaultFormTag).find('.de-select-all-subcategories').css('display', 'inline-block');
    }
}

function formLessThanSpecifiedWidth(specifiedWidth) {
    return jQuery(window).width() < specifiedWidth;
}

function changeLabelForResponsive(hidden) {
    //foreach label
    jQuery(FormsEngine.DefaultFormTag).find("label")
        .each(function () {
            if (jQuery(this).attr("data-hidelabelonmobile") && jQuery(this).attr("data-hidelabelonmobile").length > 0) {
                var hideLbl = jQuery(this).attr("data-hidelabelonmobile");
                if (hidden && hideLbl == 'True') {
                    jQuery(this).hide();
                }
                else {
                    jQuery(this).show();
                }
            }
        });
}

function swapWatermark(useMobile) {

    jQuery(FormsEngine.DefaultFormTag).find(":input[type='text'],:input[type='tel'],:input[type='email']")
        .each(function () {
            if (jQuery(this).attr("placeholder") && jQuery(this).attr("placeholder").length > 0) {
                var phVal = useMobile && jQuery(this).attr("data-placeholder-mobile") ? jQuery(this).attr("data-placeholder-mobile") : jQuery(this).attr("data-placeholder-notmobile");
                jQuery(this).attr("placeholder", phVal);
            }
        });
}

function fe_ApplyPhoneMask() {
    if (typeof (jQuery(this).inputmask) !== "function") {
        return;
    }

    var countryCode = jQuery(FormsEngine.DefaultFormTag).find("select[code='Country']").val() || "";
    var localCountry = (["US", "CA", ""].indexOf(countryCode) >= 0);
    if (FormsEngine.ApplicationId != 20 && (FormsEngine.UseInternationalTemplate !== true || FormsEngine.IsLocalIP === true) && localCountry) { //SAB rule not input mask
        if (jQuery(FormsEngine.DefaultFormTag).find(':input.replace_Phone').length == 0) {
            //get all the text inputs and apply their mask input if they have
            jQuery(FormsEngine.DefaultFormTag).find(":input[type='text'],:input[type='tel'],:input[type='email']")
                .each(function () {
                    if (jQuery(this).attr("mask") && jQuery(this).attr("mask").length > 0) {
                        jQuery(this).inputmask({ "mask": jQuery(this).attr("mask") });
                    }
                });
        }
    }
    else if (!localCountry) {
        //get all the text inputs and remove their mask input if they have
        jQuery(FormsEngine.DefaultFormTag).find(":input[type='tel']")
            .each(function () {
                if (jQuery(this).attr("mask") && jQuery(this).attr("mask").length > 0) {
                    jQuery(this).inputmask("remove");
                    jQuery(this).removeAttr("placeholder");
                }
            });
    }
}

/// Hides campus preference control, label and defaults a value
function fe_hideCampusPreference(IsOnline) {
    if (jQuery(FormsEngine.DefaultFormTag).find(':input[code="CampusPreference"]').exists()) {
        jQuery(FormsEngine.DefaultFormTag).find('.CampusPreference').remove();
        var ControlValue = IsOnline ? 'Online' : 'Campus';
        jQuery('<input>').attr({
            type: 'hidden',
            id: 'foo',
            code: 'CampusPreference',
            name: 'CampusPreference',
            value: ControlValue
        }).appendTo(FormsEngine.DefaultFormTag);
    }
}

/// Shows campus preference control and label
function fe_showCampusPreference() {
    if (jQuery(FormsEngine.DefaultFormTag).find(':input[code="CampusPreference"]').exists()) {
        if (FormsEngine.CampusPreferenceVisiblePreselectedOnline != undefined && FormsEngine.CampusPreferenceVisiblePreselectedOnline != null) {
            var ControlValue = FormsEngine.CampusPreferenceVisiblePreselectedOnline ? 'Online' : 'Campus';
            jQuery(FormsEngine.DefaultFormTag).find(":input[type=radio][code='CampusPreference'][value='" + ControlValue + "']").prop('checked', true);
        }
        jQuery(FormsEngine.DefaultFormTag).find('.CampusPreference').show();
    }
}

//Shows or hides match preference based on program results and campus type availability.
function fe_processSupportedMatchPreference() {
    var SupportCampus = false;
    var SupportOnline = false;

    var DefaultOption = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option[data-default='default']");
    var AnyCampus = jQuery(DefaultOption).attr('data-anycampus');
    var SupportCampus = AnyCampus == undefined || AnyCampus == null ? false : AnyCampus.toString().toLowerCase() == 'true';
    var AnyOnline = jQuery(DefaultOption).attr('data-anyonline');
    SupportOnline = AnyOnline == undefined || AnyOnline == null ? true : AnyOnline.toString().toLowerCase() == 'true';

    FormsEngine.CampusPreferenceVisiblePreselectedOnline = 1; //default campusType Online WI 53588

    fe_consolelog("SupportCampus=" + SupportCampus);
    fe_consolelog("SupportOnline=" + SupportOnline);
    if (SupportCampus && SupportOnline) {
        fe_showCampusPreference();
        FormsEngine.CampusPreferenceVisible = true;
    }
    else if (SupportCampus) {
        fe_hideCampusPreference(false);
        FormsEngine.CampusPreferenceVisible = false;
    }
    else {
        fe_hideCampusPreference(true);
        FormsEngine.CampusPreferenceVisible = false;
    }

    FormsEngine.CampusPreferenceProcessed = true;
}

function fe_hideValidationError(control) {
    jQuery(control).removeClass('error');
    jQuery(control).attr("id");
    if (jQuery(FormsEngine.DefaultFormTag).find('label[for="' + jQuery(control).attr('id') + '"]').exists()) {
        jQuery(FormsEngine.DefaultFormTag).find('label[for="' + jQuery(control).attr('id') + '"].error').hide();
    }
}

function fe_stripHtmlFromText(html) {
    var tmp = document.createElement("DIV");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || "";
}

function fe_encodeSpecialCharacters(value) {
    value = value ? value : "";
    value = value.replace(/\&/g, '@@amp@@');
    value = value.replace(/\=/g, '@@equal@@');
    return value;
}

//Smart Military control
function fe_processSmartMilitary(ddl_value) {
    if (jQuery(FormsEngine.DefaultFormTag).find(":input[name='military_yesno']").exists()) {
        var ddlMilitary = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Military_Affiliation']");

        if (ddl_value && ddl_value.length > 0 && ddl_value != '126') {
            jQuery(FormsEngine.DefaultFormTag).find(":input[name='military_yesno'][value='Yes']").prop('checked', true);
        }
        else if (ddl_value == '126') {
            jQuery(FormsEngine.DefaultFormTag).find(":input[name='military_yesno'][value='No']").prop('checked', true);
        }

        var checked = jQuery(FormsEngine.DefaultFormTag).find(":input[name='military_yesno']:checked").val();

        if (checked == undefined || checked == '' || checked == 'No') {
            jQuery(ddlMilitary).val('126');
            fe_hideValidationError(ddlMilitary);
            jQuery(ddlMilitary).hide();
            jQuery(ddlMilitary).trigger("change");
        }
        else if (checked == 'Yes') {
            jQuery(ddlMilitary).show();
            jQuery(ddlMilitary).trigger("change");
        }
    }
}

function fe_processSmartHighestEducationLevel(controlValue) {
    if (jQuery(FormsEngine.DefaultFormTag).find(":input[name='ddlHighestLevelNoCredits']").exists()) {
        var ddlHighestLevel = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']");
        var ddlHighestLevelNoCredits = jQuery(FormsEngine.DefaultFormTag).find(":input[name='ddlHighestLevelNoCredits']");
        var ddlHighestLevelCredits = jQuery(FormsEngine.DefaultFormTag).find(":input[name='ddlHighestLevelCredits']");
        if (controlValue) {
            //were recovering the form
            jQuery(ddlHighestLevel).val(controlValue);
            switch (controlValue) {
                case '4':
                case '5':
                case '6':
                case '7':
                    //we have some credits
                    jQuery(ddlHighestLevelCredits).show();
                    jQuery(ddlHighestLevelCredits).attr('required', 'required');
                    jQuery(ddlHighestLevelCredits).val(controlValue);
                    jQuery(ddlHighestLevelNoCredits).val('-1')
                    break;
                default:
                    //not some credits so hide second drop down
                    jQuery(ddlHighestLevelNoCredits).val(controlValue)
                    jQuery(ddlHighestLevelCredits).attr('required', '');
                    jQuery(ddlHighestLevelCredits).hide();
                    break;
            }
        }
        else {
            //ddl change handler
            if (jQuery(ddlHighestLevelNoCredits).val() == "-1") {
                //take value from second ddl and show second ddl and make it required
                jQuery(ddlHighestLevel).val(jQuery(ddlHighestLevelCredits).val());
                jQuery(ddlHighestLevelCredits).attr('required', 'required');
                jQuery(ddlHighestLevelCredits).show();
            }
            else {
                //take value from first ddl, remove required from second ddl, hide second ddl
                //fe_hideValidationError(ddlHighestLevelCredits);
                jQuery(ddlHighestLevel).val(jQuery(ddlHighestLevelNoCredits).val());
                jQuery(ddlHighestLevelCredits).attr('required', '');
                jQuery(ddlHighestLevelCredits).hide();
            }
        }
    }
}

//appends the appropriate checkbox control after the TCPA for the school removed from TCPA for 2U ticket 68074
function fe_showUserAgreementForTwoULeadShareSchool(school, programType, controlToAppendTo) {
    //if the control is already there remove it and add a new one
    fe_removeUserAgreementForTwoULeadShareSchool(); //fail safe to remove the control in case next call does not work.
    fe_getResourceMetaDataTextForKey('JS.2ULEADSHARE.SCHOOLTEXT,JS.2ULEADSHARE.USERAGREEMENT', function (data) {
        if (data != null) {
            switch (programType) {
                case 1:
                    programType = "Degrees";
                    break;
                case 2:
                    programType = "Courses";
                    break;
                case 3:
                    programType = "Certificate";
                    break;
                default:
                    programType = "";
                    break;
            }
            var schoolText = data['JS.2ULEADSHARE.SCHOOLTEXT'].replace("{school}", school).replace("{program type}", programType);
            var agreementText = data['JS.2ULEADSHARE.USERAGREEMENT'].replace("{program type}", programType);
            fe_removeUserAgreementForTwoULeadShareSchool(); //chrome is weird and this should normalize it.
            jQuery('<div name="field-holder" class="field-holder checkbox" data-controlcode="TwoULeadShare" data-controltypename="Check Box">' +
                '<fieldset><b class="twou-leadshare-text">' +
                schoolText + '</b><br /> <br />' +
                '<input type="checkbox" controltypename="Check Box" step="' + (FormsEngine.CurrentStep - 1) + '" value="' + agreementText + '" code="LeadShareAgreement" alias="LeadShareAgreement" id="cbx_TwoUAgreement" name="LeadShareUserAgreement" class="checkbox-field" label-name="' + agreementText + '" />' +
                '<label for="cbx_TwoUAgreement">' +
                agreementText +
                '</label>' +
                '</fieldset>' +
                '</div>').prependTo(controlToAppendTo);
            fe_trackLocalEvent('form', 'Show2UUserAgreement', 'true');
        }
    });
}

function fe_removeUserAgreementForTwoULeadShareSchool() {
    if (jQuery(FormsEngine.DefaultFormTag).find("div[data-controlcode='TwoULeadShare']")) {
        jQuery(FormsEngine.DefaultFormTag).find("div[data-controlcode='TwoULeadShare']").remove();
    }
}
function fe_GetThankYouPageUrl() {
    var thankYouPageUrl = FormsEngine.WorkflowThankYouPage;
    if (FormsEngine.hasOwnProperty('ThankYouApplyNowURL') && FormsEngine.hasOwnProperty('ThankYouRequestInfoURL')) {
        if (jQuery(FormsEngine.DefaultFormTag).find("div[code='ready_to_start']")) {
            var readyToStartValue = jQuery(FormsEngine.DefaultFormTag).find("input[name='ready_to_start']:checked").val();
            switch (readyToStartValue) {
                case "Apply Now":
                    thankYouPageUrl = FormsEngine.ThankYouApplyNowURL
                    break;
                case "Request Information":
                    thankYouPageUrl = FormsEngine.ThankYouRequestInfoURL
                    break;
            }
        }
    }
    return thankYouPageUrl;
}
function fe_GetProgramTemplateThankYouPageUrl() {
    var thankYouPageUrl = FormsEngine.ThankYouPage;
    if (FormsEngine.hasOwnProperty('ThankYouApplyNowURL') && FormsEngine.hasOwnProperty('ThankYouRequestInfoURL')) {
        if (jQuery(FormsEngine.DefaultFormTag).find("div[code='ready_to_start']")) {
            var readyToStartValue = jQuery(FormsEngine.DefaultFormTag).find("input[name='ready_to_start']:checked").val();
            switch (readyToStartValue) {
                case "Apply Now":
                    thankYouPageUrl = FormsEngine.ThankYouApplyNowURL
                    break;
                case "Request Information":
                    thankYouPageUrl = FormsEngine.ThankYouRequestInfoURL
                    break;
            }
        }
    }
    return thankYouPageUrl;
}

function fe_AddressSmartControlShow(visible) {
    if (jQuery(FormsEngine.DefaultFormTag).find('#SmartAddressGroup').exists()) {
        if (visible) {
            jQuery(FormsEngine.DefaultFormTag).find('#SmartAddressGroup').show();
        }
        else {
            jQuery(FormsEngine.DefaultFormTag).find('#SmartAddressGroup').hide();
        }
    }
}

function fe_AddressSmartControlClick() {
    if (jQuery(FormsEngine.DefaultFormTag).find('#SmartAddressGroup').exists()) {

        var divControl = jQuery(FormsEngine.DefaultFormTag).find('#SmartAddressGroup');
        if (jQuery(divControl).is(":visible")) {
            jQuery(divControl).hide();
        }
        else {
            jQuery(divControl).show();
        }
    }
}

function fe_add_AdditionalField(key, value) {
    FormsEngine.AdditionalFields = FormsEngine.AdditionalFields || [];
    var existIndex = -1;

    for (var i = 0; i < FormsEngine.AdditionalFields.length; i++) {
        if (FormsEngine.AdditionalFields[i][0] == key) {
            existIndex = i;
            break;
        }
    }
    if (existIndex > -1) {
        FormsEngine.AdditionalFields[existIndex][1] = value;
    }
    else {
        FormsEngine.AdditionalFields.push([key, value]);
    }
}

function fe_trackLocalEvent(source, event, message) {
    if (typeof _etq != 'undefined') {
        _etq.push(['_etEvent', event, message, source]);
    }
}

function fe_logJornaya() {
    try {
        var leadIdObjectExist = typeof LeadiD === "object";
        var leadIdRequestCatalogExist = localStorage.getItem("LeadiD-request-catalog") !== null;
        var jornayaLeadid = '';

        if (leadIdObjectExist) {
            jornayaLeadid = LeadiD.token;
        }

        if (jornayaLeadid === undefined || jornayaLeadid === null || jornayaLeadid === '' || jornayaLeadid.length !== 36) {
            fe_trackLocalEvent('form-leadid', 'leadIdObjectExist', leadIdObjectExist);
            fe_trackLocalEvent('form-leadid', 'leadIdRequestCatalogExist', leadIdRequestCatalogExist);
            fe_trackLocalEvent('form-leadid', 'leadIdInitialized', FormsEngine.JornayaInitialized === true);
            fe_trackLocalEvent('form-leadid', 'leadIdAppended', FormsEngine.JornayaJavascriptAppended === true);
            jQuery.getScript('//content.educationdynamics.com/Scripts/Ads/ads.js?' + Math.random().toString(36).slice(2), function (data, textStatus, jqxhr) {
                fe_trackLocalEvent('form-leadid', 'AdBlockerDetected', FormsEngine.AdBlockerDetected);
            }).fail(function () {
                fe_trackLocalEvent('form-leadid', 'AdBlockerDetected', 'true');
            });
        }
    }
    catch (ex) { }
}

function fe_checkShowScreenButton() {
    if (!FormsEngine.ShowContinueMobileButton) {
        return;
    }

    var formElement = jQuery("." + FormsEngine.RenderingDiv);
    if (!formElement || formElement.length == 0) {
        return;
    }

    var screenButton = jQuery("#screen-button");
    if (!screenButton || screenButton.length == 0) {
        return;
    }

    var buffer = jQuery("#header:first, .header:first");
    var top = formElement.offset().top;
    var bottom = formElement.offset().top + formElement.height();
    var topOffset = buffer != null && buffer.length > 0 ? buffer.height() : 0;
    var bottomOffset = buffer != null && buffer.length > 0 ? -(buffer.height() + screenButton.height()) : 0;
    var posY = window.pageYOffset;

    if (!posY || (posY == 0 && window.screenY)) {
        posY = window.screenY;
    }

    if (!posY || (posY == 0 && window.scrollY)) {
        posY = window.scrollY;
    }

    if (posY == 0) {
        jQuery("#screen-button").css("top", FormsEngine.ContinueMobileButton_cmbTopOffset);
    }
    else {
        jQuery("#screen-button").css("top", "auto");
    }

    var formFieldShowing =
        // Bottom in view..
        (jQuery("." + FormsEngine.RenderingDiv).offset().top + jQuery("." + FormsEngine.RenderingDiv).height() > posY +
            jQuery("#header, .header").height() && jQuery("." + FormsEngine.RenderingDiv).offset().top + jQuery("." +
                FormsEngine.RenderingDiv).height() < posY + window.innerHeight - screenButton.height())
        ||
        // Top in view..
        (jQuery("." + FormsEngine.RenderingDiv).offset().top > posY +
            jQuery("#header, .header").height() && jQuery("." + FormsEngine.RenderingDiv).offset().top < posY + window.innerHeight - screenButton.height())
        ||
        // The view is in the form.. (Top is above view and bottom is below view)..
        (jQuery("." + FormsEngine.RenderingDiv).offset().top < posY + jQuery("#header, .header").height() && jQuery("."
            + FormsEngine.RenderingDiv).offset().top + jQuery("." + FormsEngine.RenderingDiv).height() > posY +
            window.innerHeight - screenButton.height());

    if (formFieldShowing) {
        screenButton.removeClass("hide-button");
        jQuery("body").addClass("screen-button-container");
    }
    else {
        screenButton.addClass("hide-button");
        jQuery("body").removeClass("screen-button-container");
    }
}

function fe_trackGTMLeadStart() {
    if (FormsEngine.ApplicationId == 7) {
        var ProgramDDL = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Program_Of_Interest'] option:selected");
        var PaidStatusTypeId = jQuery(ProgramDDL).attr('data-paidstatustypeid');

        if (PaidStatusTypeId == 1) { //free
            fe_googleTagEvent('gaEvent', 'client', 'free-lead-start', FormsEngine.ProgramId);
        }
        else if (PaidStatusTypeId == 2) { //fraid
            fe_googleTagEvent('gaEvent', 'client', 'fraid-lead-start', FormsEngine.ProgramId);
        }
        else { //paid-- normal
            fe_googleTagEvent('gaEvent', 'client', 'lead-start', FormsEngine.ProgramId);
        }
    }
    else {
        fe_googleTagEvent('gaEvent', 'client', 'lead-start', FormsEngine.ProgramId);
    }
}


//Get list of states by country
function fe_getCountryStates(doneFunction) {
    var CountryCode = jQuery(FormsEngine.DefaultFormTag).find('select[code="Country"] option:selected').val();

    if (CountryCode == "") {
        CountryCode = FormsEngine.DefaultCountryCode;
    }

    jQuery.ajax({
        async: true,
        type: 'GET',
        dataType: 'jsonp',
        cache: false,
        url: FormsEngine.ServiceBaseURL + "/FormValidation/GetStatesByCountry?Countrycode=" + CountryCode,
        success: function (data) {
            var states = jQuery(FormsEngine.DefaultFormTag).find('select[code="State"]');
            var statevalue = jQuery(states).val();
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
                jQuery(states).val(statevalue);
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

function fe_getCityStateCountry(ZipCode) {
    if (ZipCode != undefined && ZipCode != "") {
        jQuery.ajax({
            async: true,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/FormValidation/GetCityStateCountry?ZipCode=" + ZipCode,
            success: function (data) {
                                
                // Fix DEV-2618: support both input and select controls
               // DEV-2618: Set both display values and corresponding key IDs for hidden State/Country fields.
                // Matching Engine reads StateId/CountryId from AdditionalData (State-key/Country-key),
                // so we must populate the "key" attribute to ensure downstream LeadPing/matching works correctly.
                var CountryDDL = jQuery(FormsEngine.DefaultFormTag).find(":input[code='Country']");
                jQuery(CountryDDL)
                    .val(data[3].Value)
                    .attr("key", data[4].Value)
                    .trigger("change");

                var StateDDL = jQuery(FormsEngine.DefaultFormTag).find(":input[code='State']");
                jQuery(StateDDL)
                    .val(data[1].Value)
                    .attr("key", data[2].Value)
                    .trigger("change");
                var CityInput = jQuery(FormsEngine.DefaultFormTag).find(":input[code='City']");             

                if (data.length > 0) {
                    LastStateSelection = data[1].Value;
                    LastCountrySelection = data[3].Value;

                    if (jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('Country')) {
                        if (jQuery(CountryDDL).val() != data[3].Value) {
                            jQuery(CountryDDL).val(data[3].Value);
                            if (jQuery(CountryDDL).val() == data[3].Value) {
                                jQuery(CountryDDL).valid();
                            }
                        }
                    }

                    if (jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('State')) {
                        if (jQuery(StateDDL).is("select")) {
                            fe_getCountryStates(function () {
                                //Set State
                                var StateDDL = jQuery(FormsEngine.DefaultFormTag).find("select[code='State']");
                                var CountryDDL = jQuery(FormsEngine.DefaultFormTag).find("select[code='Country']");

                                jQuery(StateDDL).val(LastStateSelection);

                                //Revalidate State control
                                if (jQuery(StateDDL).val() == LastStateSelection) {
                                    if (jQuery(StateDDL).valid()) {
                                        jQuery(StateDDL).removeClass('error');
                                    }
                                }
                                //Revalidate Country control
                                if (jQuery(CountryDDL).val() == LastCountrySelection) {
                                    if (jQuery(CountryDDL).valid()) {
                                        jQuery(CountryDDL).removeClass('error');
                                    }
                                }
                            });
                        } else {
                            jQuery(StateDDL).val(LastStateSelection);
                        }
                    }

                    if (jQuery(FormsEngine.DefaultFormTag).find('input[code="Postal_Code"]').isControlBefore('City')) {
                        jQuery(CityInput).val(data[0].Value);
                        if (jQuery(CityInput).val() == data[0].Value) {
                            jQuery(CityInput).valid();
                        }
                    }

                } else {
                    if (jQuery(StateDDL).is("select")) {
                        StateDDL.empty();
                        StateDDL.append(jQuery('<option/>', { value: "", text: FormsEngine.DefaultSelectText }));
                        StateDDL.append(jQuery('<option/>', { value: "N/A", text: "Not Applicable" }));
                    } else {
                        StateDDL.val("");
                    }

                    CountryDDL.val("");
                    CityInput.val("");
                }
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });
    }
}


//Sets values from string arguments based on input name
function fe_setValuesFromString(values) {
    jQuery(FormsEngine.DefaultFormTag).find(':input').each(function () {
        var ControlName = jQuery(this).attr('code');
        if (ControlName != undefined && ControlName != "") {
            var ControlValue = fe_getParameterByNameFromString(ControlName, values);
            if (ControlValue != undefined && ControlValue != "") {
                if (jQuery(this).is(':radio')) {
                    jQuery(FormsEngine.DefaultFormTag).find("[type=radio][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                }
                else if (jQuery(this).is(':checkbox')) {
                    jQuery(FormsEngine.DefaultFormTag).find("[type=checkbox][code='" + ControlName + "'][value='" + ControlValue + "']", this).prop('checked', true);
                }
                else {
                    jQuery(this).val(fe_cleanStringOfRestrictedCharacters(ControlValue));
                }
            }

            fe_fireDependancyEventsToRecoverForm(ControlName, this)
        }
    });
}

function fe_fireDependancyEventsToRecoverForm(controlCode, control) {
    switch (controlCode) {
        case 'Postal_Code':
            fe_getCityStateCountry(jQuery(control).val());
            break;
    }
}


function fe_loadFormFromPassThru() {
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

        fe_setValuesFromString(Data);

        return true;
    }

    return false;
}

function fe_formHasEMSApplicationId() {
    var emsApplicationId = 27;
    return FormsEngine.ApplicationId == emsApplicationId;
}

function fe_featureListInUse() {
    return FormsEngine.FeatureId != undefined && !isNaN(FormsEngine.FeatureId) && fe_formHasEMSApplicationId();
}

function fe_getUTMParameters() {
    var utmChannel = fe_getParameterByName("utm_channel") || fe_getParameterByName("utm_channel__c");
    var utmVendor = fe_getParameterByName("utm_vendor") || fe_getParameterByName("utm_vendor__c");
    var utmCampaign = fe_getParameterByName("utm_campaign") || fe_getParameterByName("utm_campaign__c");

    var utmObject = {
        "utmChannel": utmChannel,
        "utmVendor": utmVendor,
        "utmCampaign": utmCampaign
    };

    return utmObject;
}

function fe_serializeForm() {

    var fields = [];

    // this function is not complete as it returns duplicate fields
    return fields;

    var formElements = jQuery(FormsEngine.DefaultFormTag).find(":input");

    for (var i = 0; i < formElements.length; i++) {
        var element = formElements[i];
        var code = jQuery(element).attr("code");
        var elementName = element.name;
        var elementValue = "";

        if (element.type == "radio") {
            elementValue = jQuery(FormsEngine.DefaultFormTag).find(":input[code='" + code + "']:checked").val();
        } else {
            elementValue = element.value;
        }

        var fieldAlreadyAdded = fields.filter(function (field) { return field.name == elementName }).length > 0;

        if (!fieldAlreadyAdded) {
            fields.push({ "name": element.name, "value": elementValue });
        }
    }

    return fields;
}

function fe_setLeadSourceUrlAndFormLeadUrl() {
    fe_setLeadSourceUrl();
    fe_setFormLeadUrl();
    fe_setCecoId();
    fe_setSubSource2();
    fe_setLeadInitiatingUrl();
}

function fe_setSubSource2() {
    FormsEngine.SubSource2 = fe_getParameterByName("SubSource2");
    if (FormsEngine.SubSource2) {
        fe_add_AdditionalField("SubSource2", encodeURIComponent(FormsEngine.SubSource2));
        fe_setSessionObject("SubSource2", encodeURIComponent(FormsEngine.SubSource2), function () { });
    }
}

function fe_setLeadSourceUrl() {
    FormsEngine.LeadSourceUrl = fe_getParameterByName("LeadSourceUrl");
    if (FormsEngine.LeadSourceUrl) {
        fe_add_AdditionalField("LeadSourceUrl", encodeURIComponent(FormsEngine.LeadSourceUrl));
        fe_setSessionObject("LeadSourceUrl", encodeURIComponent(FormsEngine.LeadSourceUrl), function () { });
    }
}

function fe_setFormLeadUrl() {
    FormsEngine.FormLeadUrl = window.location.origin + window.location.pathname + window.location.search;
    if (FormsEngine.FormLeadUrl) {
        fe_add_AdditionalField("FormLeadUrl", encodeURIComponent(FormsEngine.FormLeadUrl));
        fe_setSessionObject("FormLeadUrl", encodeURIComponent(FormsEngine.FormLeadUrl), function () { });
    }
}

function fe_setLeadInitiatingUrl() {
    FormsEngine.LeadInitiatingUrl = fe_getParameterByName("LeadInitiatingUrl");
    if (FormsEngine.LeadInitiatingUrl) {
        fe_add_AdditionalField("LeadInitiatingUrl", encodeURIComponent(FormsEngine.LeadInitiatingUrl));
        fe_setSessionObject("LeadInitiatingUrl", encodeURIComponent(FormsEngine.LeadInitiatingUrl), function () { });
        fe_add_AdditionalField("UrlsFromQueryString", "UrlsFromQueryString");
        fe_setSessionObject("UrlsFromQueryString", "UrlsFromQueryString", function () { });
    }
}

function fe_setCecoId() {
    FormsEngine.CecId = fe_getParameterByName("CecUniqueId");
    if (FormsEngine.CecId) {
        fe_add_AdditionalField("CecId", FormsEngine.CecId);
    }
}

function fe_hasSubmitButtonLabelTextLastAlreadyBeenSet() {
    return Boolean(FormsEngine.SubmitButtonLabelTextLast);
}

function fe_isLastStep(_FormsEngine) {

    // Dependency injection for unit tests
    _FormsEngine = _FormsEngine || FormsEngine;

    var currentStepEqualsLastStep = _FormsEngine.CurrentStep == _FormsEngine.StepLast;
    var renderingStrategyIsSchoolPickerWizard = _FormsEngine.RenderingStrategy == "SCHOOLPICKERWIZARD";

    return (
        (currentStepEqualsLastStep && _FormsEngine.HasAdditionalQuestions === false)
        ||
        (_FormsEngine.CurrentStep == _FormsEngine.StepDynamicQuestions && !renderingStrategyIsSchoolPickerWizard)
        ||
        (_FormsEngine.ShowAllQuestionsOnFirstStep && currentStepEqualsLastStep)
        ||
        (renderingStrategyIsSchoolPickerWizard && currentStepEqualsLastStep)
    );
}

function fe_isGoingToStepBeforeAdditionalQuestions(_FormsEngine) {

    // Dependency injection for unit tests
    _FormsEngine = _FormsEngine || FormsEngine;

    var renderingStrategyIsSchoolPickerWizard = _FormsEngine.RenderingStrategy == "SCHOOLPICKERWIZARD";

    return (_FormsEngine.CurrentStep == (_FormsEngine.StepLast - 1) && !renderingStrategyIsSchoolPickerWizard)
        || (_FormsEngine.CurrentStep == (_FormsEngine.StepDynamicQuestions - 1) && renderingStrategyIsSchoolPickerWizard);
}

function fe_moveControlAlongsideTCPA(controlCode) {
    try {
        if (!FormsEngine.ControlsToShowAlongsideTCPA || FormsEngine.ControlsToShowAlongsideTCPA.indexOf(controlCode) < 0) {
            return;
        }

        var control = jQuery(FormsEngine.DefaultFormTag).find('div[data-controlcode="' + controlCode + '"]');
        var tcpaControl = jQuery(FormsEngine.DefaultFormTag).find('div[data-controlcode="UserAgreement"]');

        var controlStep = Number(control.parents(".steps").attr("data-step"));
        var tcpaControlStep = Number(tcpaControl.parents(".steps").attr("data-step"));

        if (FormsEngine.CurrentStep > controlStep || tcpaControlStep < controlStep) {
            control.insertBefore(tcpaControl);
        }   
    } catch (ex) {
        fe_consolelog(ex);
    }
}

function fe_determineIfHeaderDirectionButtonsShouldBeHidden() {
    if (!FormsEngine.ShowArrowsInMobileHeader) {
        jQuery(FormsEngine.DefaultFormTag).find("#next-top-img").addClass("hidden");
        jQuery(FormsEngine.DefaultFormTag).find("#prev-top-img").addClass("hidden");
    }
}

function fe_addLastStepClassToBackButtonIfOnLastStep() {
    var lastStepClass = "last-step";

    if (fe_isLastStep()) {
        jQuery(FormsEngine.BackButton).addClass(lastStepClass);
    } else {
        jQuery(FormsEngine.BackButton).removeClass(lastStepClass);
    }
}

function fe_setupPrivacyPolicyLink() {
    var privacyPolicyLink = jQuery(FormsEngine.DefaultFormTag).find("#ccpaPrivacyPolicyUrl");

    if (privacyPolicyLink) {
        if (FormsEngine.PrivacyPolicyUrl) {
            privacyPolicyLink.attr("rel", "modal:open");
            privacyPolicyLink.attr("href", FormsEngine.PrivacyPolicyUrl)
        }
    }
}

function fe_hideCCPAIfBeyondFirstStepOrEMS() {
    var emsApplicationId = 27;
    if (FormsEngine.CurrentStep > 1 || FormsEngine.ApplicationId == emsApplicationId) {
        jQuery(".ccpa-message").hide();
    } else {
        jQuery(".ccpa-message").show();
    }
}

function fe_getPrivacyPolicy() {
    jQuery.ajax({
        type: 'GET',
        dataType: 'jsonp',
        url: FormsEngine.ServiceBaseURL + "/PrivacyPolicy",
        success: function (privacyPolicyContent) {
            if (privacyPolicyContent)
                fe_showPrivacyPolicyModal(privacyPolicyContent);
        },
        error: function (request, textStatus, errorThrown) {
            fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
        }
    });
}

function fe_showPrivacyPolicyModal(privacyPolicyContent) {
    var modal = jQuery("<div></div>").attr("id", "ccpaPrivacyPolicyModal");
    var modalContent = jQuery("<div></div>").attr("id", "ccpaPrivacyPolicyModalContent");
    var modalCloseBtn = jQuery("<div>&times;</div>").attr("id", "ccpaPrivacyPolicyModalCloseBtn");

    modalContent.append(privacyPolicyContent);
    modal.append(modalCloseBtn);
    modal.append(modalContent);

    jQuery('body').append(modal);

    modalCloseBtn.on("click", function () {
        modal.remove();
    });
}

jQuery(window).resize(function () {
    fe_checkResponsiveItems();
});


if (typeof window.FormsEngineGlobal === "undefined") {
    window.FormsEngineGlobal = {};
    //Constants
    window.FormsEngine = window.FormsEngine || {};

    //Cookie helper
    var cookieHelper = " window.FormsEngine.createCookie = function createCookie(name, value, days) {if (days) {var date = new Date();date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));var expires = '; expires=' + date.toGMTString();} else var expires = '';document.cookie = name + '=' + value + expires + '; path=/';}; ";
    cookieHelper += " window.FormsEngine.readCookie = function readCookie(name) {var nameEQ = name + '=';var ca = document.cookie.split(';');for (var i = 0; i < ca.length; i++) {var c = ca[i]; while (c.charAt(0) == ' ') c = c.substring(1, c.length);if (c.indexOf(nameEQ) == 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));} return null;}; ";

    var scriptCookie = document.createElement('script');
    scriptCookie.type = "text/javascript";
    scriptCookie.language = "javascript";
    scriptCookie.text = cookieHelper;

    //Global url
    var globalURL = "[SERVICEBASE]";
    FormsEngine.ServiceBaseURL = FormsEngine.ServiceBaseURL == undefined || FormsEngine.ServiceBaseURL == "" ? globalURL : FormsEngine.ServiceBaseURL;

    if (!window.JSON) {
        fe_loadJs('/Templates/Common/js/json2.js');
    }
    FormsEngine.Picasso = function () { FormsEngine.PicassoIA = FormsEngine.PicassoIA == 0 ? 180 : 0; jQuery(FormsEngine.DefaultFormTag).find("[name='eddy-form-container']").rotateElement(FormsEngine.PicassoIA, 10, 18, 20); }
    //session keep alive 
    if (!FormsEngine.SessionPing) {
        FormsEngine.SessionPing = setInterval(fe_pingFESession, 300000); //ping every 5 min
    }

    //Override Trackid by Optimizely
    if (FormsEngine.OptimizelyTrackId) {
        fe_consolelog("TrackId set by Optimizely: " + FormsEngine.OptimizelyTrackId);
        FormsEngine.TrackId = FormsEngine.OptimizelyTrackId;
    }

    FormsEngine.ContinueMobileButton_cmbScrolled = false;
    FormsEngine.ContinueMobileButton_cmbScroll = 0;
    FormsEngine.ContinueMobileButton_cmbOffSetted = false;

    FormsEngine.SessionName = "";

    jQuery(document).ready(function () {
        document.body.appendChild(scriptCookie);       
    });
}

