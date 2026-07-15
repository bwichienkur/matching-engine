// ManagedChoice_GlobalFunctions.js
// ** PREPEND ALL GLOBAL FUNCTIONS WITH fe_mc_
// ** ALL jquery usage must be with the full "jQuery()" snytax in this file, no "$()" allowed!
//---------------------------------------------------------------------------------------------

// Global Constants

// Internals variables


function fe_mc_limitUserSelectionCount() {
    if (FormsEngine.LimitNumberOfUserSelections != undefined && FormsEngine.LimitNumberOfUserSelections) {
        // get the count of checked schools, and if it is greater than or equal to the managedChoiceAllowedSchoolCount, disable the other select checkboxes
        if (jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').length >= parseInt(FormsEngine.MaxManagedChoiceUserSelections)) {
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:not(:checked)').parents("[name='mc-us-schoolcontainer']").attr('disabled', true).addClass('disable');
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:not(:checked)').parents("[name='mc-us-schoolcontainer']").find('input,button,textarea,a,select').attr('disabled', 'disabled');
            // else, enable all the checkboxes
        } else {
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:not(:checked)').parents("[name='mc-us-schoolcontainer']").attr('disabled', false).removeClass('disable');
            jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:not(:checked)').parents("[name='mc-us-schoolcontainer']").find('input,button,textarea,a,select').attr('disabled', false);
        }
    }
}

function fe_mc_isOverUserSelectionCount() {
    return (FormsEngine.LimitNumberOfUserSelections != undefined && FormsEngine.LimitNumberOfUserSelections &&
        (jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').length > parseInt(FormsEngine.MaxManagedChoiceUserSelections)));
}

function toggleHiddenSchools() {

    if (jQuery(".hidden-schools").css("display") != "none") {
        jQuery(".hidden-schools").hide();
        jQuery(".hide-more-text").hide();
        jQuery(".hide-more-alternate").hide();
        jQuery(".show-more-alternate").show();
        jQuery(".show-more-text").show();
    }
    else {
        jQuery(".hidden-schools").show();
        jQuery(".hide-more-text").show();
        jQuery(".hide-more-alternate").show();
        jQuery(".show-more-alternate").hide();
        jQuery(".show-more-text").hide();
    }
}

function processLynnTCPA(smartMatchList) {

    var ncIndex = -1;

    for (var i = 0; i < smartMatchList.length; i++) {
        if (smartMatchList[i] == "Lynn University") {
            ncIndex = i;
            break;
        }

    }

    //Kaplan Purdue hardcoded rules

    if (ncIndex > -1) {
        smartMatchList[ncIndex] = "Lynn University/Kaplan North America, LLC.";
    }

}

function processPurdueKaplanTCPA(smartMatchList) {

    var kIndex = -1;
    var pIndex = -1;

    for (var i = 0; i < smartMatchList.length; i++) {
        if (smartMatchList[i] == "Kaplan University") {
            kIndex = i;
        }
        else if (smartMatchList[i] == "Purdue Global") {
            pIndex = i;
        }
    }

    //Kaplan Purdue hardcoded rules
    //Both exist
    if (kIndex > -1 && pIndex > -1) {
        smartMatchList[kIndex] = "Purdue Global/Kaplan North America, LLC.";
        smartMatchList[pIndex] = "";
    }
    else if (kIndex > -1) {
        smartMatchList[kIndex] = "Purdue Global/Kaplan North America, LLC.";
    }
    else if (pIndex > -1) {
        smartMatchList[pIndex] = "Purdue Global/Kaplan North America, LLC.";
    }
}

function processNorthcentralUniversityTCPA(smartMatchList) {

    var ncIndex = -1;

    for (var i = 0; i < smartMatchList.length; i++) {
        if (smartMatchList[i] == "Northcentral University") {
            ncIndex = i;
            break;
        }
      
    }

    //Kaplan Purdue hardcoded rules
    
    if (ncIndex > -1) {
        smartMatchList[ncIndex] = "Northcentral University and its National University Affiliates";
    }
    
}

function processNationalUniversityTCPA(smartMatchList) {

    var ncIndex = -1;

    for (var i = 0; i < smartMatchList.length; i++) {
        if (smartMatchList[i] == "National University") {
            ncIndex = i;
            break;
        }

    }

    //National hardcoded rules

    if (ncIndex > -1) {
        smartMatchList[ncIndex] = "National University and National University System affiliates (City University of Seattle, Northcentral University and National University Virtual High School)";
    }

}

function fe_mc_updateExpressConsent() {
    var schoolNamesString = '';
    var schoolNamesArray = [];
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').each(function () {
        if (jQuery(this).parents("[name='mc-us-schoolcontainer']").attr('data-hideschoolfromtcpa') != 'true') {

            schoolNamesArray.push(jQuery(this).parents("[name='mc-us-schoolcontainer']").attr('data-institution-name'));
        }
    });
    processNorthcentralUniversityTCPA(schoolNamesArray);
    processPurdueKaplanTCPA(schoolNamesArray);
    processNationalUniversityTCPA(schoolNamesArray);
    processLynnTCPA(schoolNamesArray);
    jQuery(schoolNamesArray).each(function () {
        if (this != "") {
            schoolNamesString += this + ', ';
        }
    });

    // Retrieve the Call Center TCPA if this is a Call Center, otherwise use the default School Selection Agreement..
    //var expressConsent = (FormsEngine.CampaignDetail && FormsEngine.CampaignDetail.IsCallCenter && FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED.CALLCENTER.SS']) ? FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED.CALLCENTER.SS'] : FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'];
    var expressConsent = FormsEngine.ResourceData['JS.WIZARD.USERAGREEMENT_UNIFIED'];
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

    //US Schools
    if (schoolNamesString != undefined && schoolNamesString.length > 0) {
        expressConsent = fe_replaceTag(expressConsent, '{school}', schoolNamesString);
    }
    else {
        expressConsent = fe_replaceTag(expressConsent, '{school}', '');
    }

    expressConsent = fe_replaceTag(expressConsent, '{thirdpartyschool}', '');
    expressConsent = fe_replaceTag(expressConsent, '{thirdpartytype}', ' ');

    //label
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').html(expressConsent);
    //value
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('#ckbExpressConsent').val(expressConsent);

    if (FormsEngine.LastUASSText != expressConsent) {

        //Business requirement to avoid false pre-check detection, uncheck if TCPA message changes
        var tcpaControl = jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('#ckbExpressConsent');
        if (jQuery(tcpaControl).is(':checked')) {
            jQuery(tcpaControl).checked = false;
            jQuery(tcpaControl).removeAttr('checked');
            jQuery(tcpaControl).trigger('change');
        }

        //LeadId changes for express consent, legacy recapture requested by leadid.
        try {
            if (typeof LeadiD != 'undefined') {
                LeadiD.formcapture.init();
            }
        }
        catch (ex) { }
    }
    FormsEngine.LastUASSText = expressConsent;
}

function fe_mc_removeErrorMsgForSubmit() {
    jQuery(FormsEngine.ManagedChoiceErrorDivTag).addClass('hide');
    jQuery(FormsEngine.ManagedChoiceErrorDivTag).html('');
    jQuery(FormsEngine.ManagedChoiceSubmitTag).find('a').removeAttr('disabled', 'disabled').removeClass('disabled');
}

function fe_mc_removeExpressConsentErrorMsg() {
    // remove express consent error
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').removeClass('express-consent-error');
    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).find('label[for=ckbExpressConsent]').addClass('express-consent-black');
}

function serializeDictionary(dictionary, paramName) {
    return Object.keys(dictionary)
        .map(key => encodeURIComponent(paramName + '[' + key + ']') + '=' + encodeURIComponent(dictionary[key]))
        .join('&');
}


function fe_mc_UserSelectSubmitWS(mcUserSelectProgramArray, LeadDataEncoded, shouldProcessWorkflow, callBack) {
    var serviceArgs = "";
    fe_getSessionId(function () {
        serviceArgs += "TemplateId=" + FormsEngine.TemplateId;
        serviceArgs += "&ProgramArrayString=" + mcUserSelectProgramArray;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&TrackId=" + FormsEngine.TrackId;
        serviceArgs += "&SessionId=" + FormsEngine.SessionId;
        serviceArgs += "&MatchId=" + FormsEngine.MatchResponseGuid;
        serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
        serviceArgs += "&LeadData=" + LeadDataEncoded;
        serviceArgs += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
        serviceArgs += "&PreviousSMLeadsCreatedCount=" + FormsEngine.SMLeadsCreatedCount;
        serviceArgs += "&PreviousUSLeadsCreatedCount=" + FormsEngine.USLeadsCreatedCount;
        serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
        serviceArgs += "&LimboAlternativeCampaignTrackid=" + FormsEngine.LimboAlternativeCampaignTrackid;
        serviceArgs += "&LimboAlternativeCampaignTrackidUtilized=" + (FormsEngine.LimboAlternativeCampaignTrackidUtilized == true ? "true" : "false");
        serviceArgs += "&FormTemplateType=" + FormsEngine.FormTemplateType;


        jQuery.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/TemplateManager/ManagedChoiceLeadSubmission?" + serviceArgs,
            success: function (data) {
                if (data != null) {
                    FormsEngine.SMLeadsCreatedCount = data.SMLeadsCreatedCount;
                    FormsEngine.USLeadsCreatedCount = data.USLeadsCreatedCount;
                    //if (shouldProcessWorkflow) {
                    //    fe_saveWorkflowData(function () { });
                    //}
                    if (data.MoveToThankYou && callBack && !shouldProcessWorkflow) {
                        callBack();
                    }

                    if (data.MoveToThankYou) {
                        if (shouldProcessWorkflow) {
                            fe_consolelog('User Select Leads Saved, Moving to ThankYou.');
                            FormsEngine.LoadWorkflowStep('THANKYOU', '');
                        }
                        return;
                    } else {
                        if (shouldProcessWorkflow) {
                            FormsEngine.LoadWorkflowStep('NOMATCH', '');
                        }
                        return;
                    }
                }
                else {
                    if (shouldProcessWorkflow) {
                        FormsEngine.LoadWorkflowStep('NOMATCH', '');
                    }
                    return;
                }
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                SubmittingForm = false;
                fe_hideLoader(false);
            }
        });
    });
}

function fe_mc_UserSelectSubmitWSCustomTCPA(mcUserSelectProgramArray, mcCustomTCPAArray, LeadDataEncoded, shouldProcessWorkflow, callBack) {
    var serviceArgs = "";
    fe_getSessionId(function () {
        serviceArgs += "TemplateId=" + FormsEngine.TemplateId;
        serviceArgs += "&ProgramArrayString=" + mcUserSelectProgramArray;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&TrackId=" + FormsEngine.TrackId;
        serviceArgs += "&SessionId=" + FormsEngine.SessionId;
        serviceArgs += "&MatchId=" + FormsEngine.MatchResponseGuid;
        serviceArgs += "&ProspectId=" + FormsEngine.ProspectId;
        serviceArgs += "&LeadData=" + LeadDataEncoded;
        serviceArgs += "&AdditionalData=" + FormsEngine.LeadAdditionalDataEncoded;
        serviceArgs += "&PreviousSMLeadsCreatedCount=" + FormsEngine.SMLeadsCreatedCount;
        serviceArgs += "&PreviousUSLeadsCreatedCount=" + FormsEngine.USLeadsCreatedCount;
        serviceArgs += "&FESessionId=" + FormsEngine.FESessionId;
        serviceArgs += "&LimboAlternativeCampaignTrackid=" + FormsEngine.LimboAlternativeCampaignTrackid;
        serviceArgs += "&LimboAlternativeCampaignTrackidUtilized=" + (FormsEngine.LimboAlternativeCampaignTrackidUtilized == true ? "true" : "false");
        serviceArgs += "&FormTemplateType=" + FormsEngine.FormTemplateType;
        serviceArgs += "&" + serializeDictionary(mcCustomTCPAArray, 'tcpaArrayString');;


        jQuery.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ServiceBaseURL + "/TemplateManager/ManagedChoiceLeadSubmission?" + serviceArgs,
            success: function (data) {
                if (data != null) {
                    FormsEngine.SMLeadsCreatedCount = data.SMLeadsCreatedCount;
                    FormsEngine.USLeadsCreatedCount = data.USLeadsCreatedCount;
                    //if (shouldProcessWorkflow) {
                    //    fe_saveWorkflowData(function () { });
                    //}
                    if (data.MoveToThankYou && callBack && !shouldProcessWorkflow) {
                        callBack();
                    }

                    if (data.MoveToThankYou) {
                        if (shouldProcessWorkflow) {
                            fe_consolelog('User Select Leads Saved, Moving to ThankYou.');
                            FormsEngine.LoadWorkflowStep('THANKYOU', '');
                        }
                        return;
                    } else {
                        if (shouldProcessWorkflow) {
                            FormsEngine.LoadWorkflowStep('NOMATCH', '');
                        }
                        return;
                    }
                }
                else {
                    if (shouldProcessWorkflow) {
                        FormsEngine.LoadWorkflowStep('NOMATCH', '');
                    }
                    return;
                }
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
                SubmittingForm = false;
                fe_hideLoader(false);
            }
        });
    });
}


function fe_mc_showUSPopup(event) {
    event.preventDefault();
    var dataholder = jQuery(this).parents("[name='mc-us-schoolcontainer']");
    var popup = {};
    // popup is different, and location to show popup is diffent
    if (jQuery(this).hasClass('sm-institution-popup-link')) {
        popup = jQuery(FormsEngine.SMInstitutionPopup);
        var y = event.screenY;
        jQuery(popup).css('top', event.pageY - jQuery('#mc-SM-Display').offset().top - 50 + 'px');
    }
    if (jQuery(this).hasClass('us-institution-popup-link') || jQuery(this).hasClass('mc-us-more-info')) {
        popup = jQuery(FormsEngine.USInstitutionPopup);
        var y = event.screenY;
        jQuery(popup).css('top', event.pageY - jQuery('#mc-US-Area').offset().top - 50 + 'px');
    }

    // get data
    var InstitutionName = jQuery(dataholder).attr('data-institution-name');
    //Helen add hidden institution logo for popup info
    if (jQuery(this).hasClass("sm-institution-popup-link")) {
        var InstitutionLogo = jQuery(this).html();
    } else {
        var InstitutionLogo = jQuery(dataholder).find('.us-institution-logo a.us-institution-popup-link').html();
    }
    var InstitutionDescription = decodeURIComponent((jQuery(dataholder).attr('data-institution-description') + '').replace(/\+/g, '%20'));
    var InstitutionDisclaimer = decodeURIComponent((jQuery(dataholder).attr('data-institution-disclaimer') + '').replace(/\+/g, '%20'));
    var InstitutionDisclaimerType = jQuery(dataholder).attr('data-institution-disclaimer-type');
    var IsGroundCampus = false;
    if (jQuery(dataholder).attr('data-isgroundcampus') != undefined && jQuery(dataholder).attr('data-isgroundcampus') != 'undefined'
        && jQuery(dataholder).attr('data-isgroundcampus') != null && jQuery(dataholder).attr('data-isgroundcampus') != 'null'
        && jQuery(dataholder).attr('data-isgroundcampus').toLowerCase() == 'true') {
        IsGroundCampus = true;
        var CampusName = jQuery(dataholder).attr('data-campus-name');
        var CampusCity = jQuery(dataholder).attr('data-campus-city');
        var CampusState = jQuery(dataholder).attr('data-campus-state');
    }

    // construct html
    var institutionInfo = '';
    institutionInfo += '<div id="progDescTop" class="hide"></div>';
    institutionInfo += '<div class="popup-info-wrapper">';
    institutionInfo += '<a class="sm-institution-popup-close popup-close" href="#"><span class="fa fa-times hide"></span></a>';
    institutionInfo += '<div class="popup-content">';
    //Helen add hidden school logo into popup
    institutionInfo += '<div class="us-institution-logo hide">' + InstitutionLogo + '</div>';
    institutionInfo += '<h4 class="sm-institution-popup-InstitutionName">' + InstitutionName + '</h4>';
    if (IsGroundCampus) {
        institutionInfo += '<p class="sm-institution-popup-CampusName">' + CampusName + '</p>';
        institutionInfo += '<p class="sm-institution-popup-CampusCity">' + CampusCity + '</p>';
        institutionInfo += '<p class="sm-institution-popup-CampusState">' + CampusState + '</p>';
    }
    if (InstitutionDescription != undefined && InstitutionDescription != 'undefined' && InstitutionDescription != null) {
        institutionInfo += '<div class="sm-institution-popup-InstitutionDescription">' + InstitutionDescription + '</div>';
    }
    if (InstitutionDisclaimerType != null && InstitutionDisclaimerType != "" && InstitutionDisclaimerType.toLowerCase() == "link") {
        institutionInfo += 'For ' + InstitutionName + ' disclosure information <a class="sm-institution-popup-InstitutionDisclaimer" href="' + InstitutionDisclaimer + '" target="_blank">click here.<a>';
    } else if (InstitutionDisclaimerType != null && InstitutionDisclaimerType != "" && InstitutionDisclaimerType.toLowerCase() == "text") {
        institutionInfo += '<span class="sm-institution-popup-InstitutionDisclaimer">' + InstitutionDisclaimer + '</span>';
    }
    institutionInfo += '</div></div>';
    institutionInfo += '<div id="progDescBtm" class="hide"></div>';

    jQuery(popup).html(institutionInfo);
    jQuery(popup).fadeIn('slow');
}

jQuery(document).ready(function () {
}); //end of document.ready


