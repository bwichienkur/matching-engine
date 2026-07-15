/// <reference path="../DynamicControls.js" />
/*
	EDDY AD API
	This Script is used for Displaying Various Ads across EDDY Web Sites.
*/

//Display Ad
function eddy_ad_render(v) {
    //Right now only DFP is supported.
    if (v.AdServer == 'DFP') {
        if (v.hasOwnProperty('AdUnitPath') && v.hasOwnProperty('Sizes')) {
            googletag.cmd.push(function () {
                googletag.defineSlot(v.AdUnitPath, v.Sizes, v.RenderingDiv).addService(googletag.pubads());
                //to handle customized additional targeting parameters
                if (v.hasOwnProperty('AddiTargetingParam') && v.AddiTargetingParam != '') {

                    var addiParam = v.AddiTargetingParam;
                    for (var property in addiParam) {
                        if (addiParam.hasOwnProperty(property)) {
                            googletag.pubads().setTargeting(property, addiParam[property]);
                        }
                    }
                }

                if (eddy_ad_make_request(v)) {
                    googletag.pubads().enableSingleRequest();
                    googletag.pubads().collapseEmptyDivs();
                    googletag.enableServices();
                }
            });
        }

        if (eddy_ad_make_request(v)) {
            googletag.cmd.push(function () { googletag.display(v.RenderingDiv); });
        }
   }
}

//Determines if a single request should be made to DFP. 
//In most cases we will not want to make multiple requests.
function eddy_ad_make_request(v) {
    var o = true;
    if(v.hasOwnProperty('SingleRequest')) {
        o = false;
        if (v.hasOwnProperty('LastItem')) {
            o = true;
        }
    }
    return o;
}

function eddy_ad_getfieldvalue(data, field, singleField) {
    var result = singleField ? "" : [];
    try {
        var grepResult = jQuery.grep(data, function (e) { return e.Key == field; });
        if (singleField) {
            result = grepResult.length == 1 ? grepResult[0].Value : "";
        }
        else {
            result = grepResult.length == 1 ? grepResult[0].Value.split(",") : [];
        }
    }catch(ex) {}
    return result;
}

//Sets FE Session Values for each Vendor.
function eddy_ad_initialize(v) {

    var wizard = true;
    if (v.IsWizard === false) {
        wizard = false;
    }

    if (v.RenderingDiv.length > 0) {

        jQuery('#' + v.RenderingDiv).html('');

        if (typeof (fe_global) == "function" && window.FormsEngine != 'undefined' && typeof (window.FormsEngine.readCookie) == "function") {

            fe_getFormFieldsSessionValues(['Categories_Selections'
                , 'SubCategories_Selections'
                , 'Specialties_Selections'
                , 'SchoolsSelected'
                , 'Highest_Level_of_Education_Completed'
                , 'Military_Affiliation'
                , 'Postal_Code'
                , 'Age'
                , 'Desired_Start_Date'
                , 'DynamicCampusSoftPreference'
                , 'Have_you_ever_had_a_student_loan'
                , 'us_citizen'
                , 'RN_license'
                , 'Year_of_Highest_Education_Completed'
                , 'GPA'
                , 'Teaching_certificate'
                , 'Desired_Degree_Level'
                , 'Undergraduate_Degree_Education'
                , 'Years_of_Teaching_Experience'
                , 'LPN_License'
                , 'registered_Radiology'
                , 'completed_1600_hours_of_clinical_experience'
                , 'employed_radiology_or_graduated_past_5_years'
                , 'registered_and_licensure'
                , 'Undergraduate_Degree_Nursing'], function (data) {

                    //Category
                    var category = eddy_ad_getfieldvalue(data, 'Categories_Selections', false);

                    //SubCategory
                    var subcategory = eddy_ad_getfieldvalue(data, 'SubCategories_Selections', false);
                
                    //Specialty
                    var specialty = eddy_ad_getfieldvalue(data, 'Specialties_Selections', false);
    
                    //SchoolsSelected
                    var school_selected = eddy_ad_getfieldvalue(data, 'SchoolsSelected', false);

                    //Education Level
                    var elevelid = eddy_ad_getfieldvalue(data, 'Highest_Level_of_Education_Completed', true);

                    //Military
                    var militaryid = eddy_ad_getfieldvalue(data, 'Military_Affiliation', true);

                    //Postal Code
                    var location = eddy_ad_getfieldvalue(data, 'Postal_Code', true);

                    //Age Code
                    var age = eddy_ad_getfieldvalue(data, 'Age', true);

                    //Desired_Start_Date Code
                    var start_date = eddy_ad_getfieldvalue(data, 'Desired_Start_Date', true);

                    //DynamicCampusSoftPreference Code
                    var dynamic_campus_soft_preference = eddy_ad_getfieldvalue(data, 'DynamicCampusSoftPreference', true);

                    //Have_you_ever_had_a_student_loan Code
                    var student_loan = eddy_ad_getfieldvalue(data, 'Have_you_ever_had_a_student_loan', true);

                    //US Citizen
                    var uscitizen = eddy_ad_getfieldvalue(data, 'us_citizen', true);

                    //RN License
                    var rnlicense = eddy_ad_getfieldvalue(data, 'RN_license', true);
                    
                    //undergrad degree nursing
                    var underdegreenursing = eddy_ad_getfieldvalue(data, 'Undergraduate_Degree_Nursing', true);

                    //Year Highest Ed
                    var yearhigh = eddy_ad_getfieldvalue(data, 'Year_of_Highest_Education_Completed', true);

                    //gpa
                    var gpa = eddy_ad_getfieldvalue(data, 'GPA', true);

                    //Teaching cert
                    var teachingcert = eddy_ad_getfieldvalue(data, 'Teaching_certificate', true);

                    //Desired Degree Level
                    var desireddegreelevel = eddy_ad_getfieldvalue(data, 'Desired_Degree_Level', true);

                    //undergrad degree education
                    var underdegreeducation = eddy_ad_getfieldvalue(data, 'Undergraduate_Degree_Education', true);

                    //Year of Teaching Experience
                    var yearsteaching = eddy_ad_getfieldvalue(data, 'Years_of_Teaching_Experience', true);

                    //LPN License
                    var lpnlicense = eddy_ad_getfieldvalue(data, 'LPN_License', true);

                    //LPN License
                    var registeredradiology = eddy_ad_getfieldvalue(data, 'registered_Radiology', true);

                    //Hours experience
                    var hoursexperience = eddy_ad_getfieldvalue(data, 'completed_1600_hours_of_clinical_experience', true);

                    //Employed Radiology experience
                    var employedradiology = eddy_ad_getfieldvalue(data, 'employed_radiology_or_graduated_past_5_years', true);

                    //Registered and licensure
                    var randlicensure = eddy_ad_getfieldvalue(data, 'registered_and_licensure', true);
                    
                    
                    
                var vendor = 'VANTAGE';

                if (v.hasOwnProperty('Vendor') && v.Vendor != '') {
                    vendor = v.Vendor.toUpperCase();
                }

                switch (vendor) {

                    case 'VANTAGE':

                        //Military
                        var vMilitaryid = militaryid == '' || militaryid == '126' ? 2 : 1;

                        var vEducationLevel = 1;
                        if (elevelid == '2' || elevelid == '3') {
                            vEducationLevel = 2;
                        }
                        else if (elevelid == '4' || elevelid == '5' || elevelid == '6' || elevelid == '7' || elevelid == '8') {
                            vEducationLevel = 3;
                        }
                        else if (elevelid == '9') {
                            vEducationLevel = 4;
                        }
                        else if (elevelid == '10') {
                            vEducationLevel = 5;
                        }
                        else if (elevelid == '11') {
                            vEducationLevel = 6;
                        }

                        googletag.cmd.push(function () { googletag.pubads().setTargeting("elevel", vEducationLevel.toString()); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("mil", vMilitaryid.toString()); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("loc", location); });

                        if (specialty.length > 0 && specialty[0] != '') {
                            fe_getISMapping('VANTAGE.MAPPINGS', 'SPECIALTY', specialty[0])
                            .success(function (data) {
                                googletag.cmd.push(function () { googletag.pubads().setTargeting("sa", data); });
                                eddy_ad_render(v);
                            })
                            .error(function () { eddy_ad_render(v); });
                        }
                        else if (subcategory.length > 0 && subcategory[0] != '') {
                            fe_getISMapping('VANTAGE.MAPPINGS', 'SUBCATEGORY', subcategory[0])
                            .success(function (data) {
                                googletag.cmd.push(function () { googletag.pubads().setTargeting("sa", data); });
                                eddy_ad_render(v);
                            })
                            .error(function () { eddy_ad_render(v); });
                        }
                        else if (category.length > 0 && category[0] != '') {
                            fe_getISMapping('VANTAGE.MAPPINGS', 'CATEGORY', category[0])
                            .success(function (data) {
                                googletag.cmd.push(function () { googletag.pubads().setTargeting("aos", data); });
                                eddy_ad_render(v);
                            })
                            .error(function () { eddy_ad_render(v); });
                        }
                        else {
                            eddy_ad_render(v);
                        }

                        break;

                    case 'CLICKSNET':

                        //Do what you need to do with Clicks Net.

                        break;

                    case 'MEDIAALPHA':

                        googletag.cmd.push(function () { googletag.pubads().setTargeting("elevel", elevelid); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("mil", militaryid); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("loc", location); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("age", age); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("category", category); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("subject", subcategory); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("specialty", specialty); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("schools", school_selected); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("desired_start_date", start_date); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("dynamic_campus_soft_preference", dynamic_campus_soft_preference); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("student_loan", student_loan); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("fesessionid", FormsEngine.FESessionId); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("trackid", FormsEngine.TrackId); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("step", FormsEngine.CurrentStep); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("workflowstep", FormsEngine.CurrentPage); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("domain", window.location.hostname); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("uscitizen", uscitizen); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("rnlicense", rnlicense); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("underdegreenursing", underdegreenursing); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("yearhigh", yearhigh); });
                        //new Adam
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("gpa", gpa); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("prospectid", FormsEngine.ProspectId); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("teachingcert", teachingcert); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("desireddegreelevel", desireddegreelevel); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("underdegreeducation", underdegreeducation); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("yearsteaching", yearsteaching); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("lpnlicense", lpnlicense); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("registeredradiology", registeredradiology); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("hoursexperience", hoursexperience); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("employedradiology", employedradiology); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("randlicensure", randlicensure); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("trackingsession", FormsEngine.SessionId); });
                        
                        eddy_ad_render(v);
                        break;

                }

            }, wizard);

        }
        else if (typeof Drupal != 'undefined') {

            if (Drupal.hasOwnProperty('settings') && (Drupal.settings.hasOwnProperty('eddy_listing_requests') || Drupal.settings.hasOwnProperty('program_details_results'))) {

                var drupal_settings = [];

                if (Drupal.settings.hasOwnProperty('program_details_results')) {
                    
                    drupal_settings = Drupal.settings.program_details_results;

                }
                else if (Drupal.settings.hasOwnProperty('eddy_listing_requests')) {

                    drupal_settings = Drupal.settings.eddy_listing_requests;

                }

                //Levels
                var levels = (drupal_settings.hasOwnProperty('level') && drupal_settings.level.length > 0) ? drupal_settings.level : [];

                //Category
                var category = (drupal_settings.hasOwnProperty('category') && drupal_settings.category.length > 0) ? drupal_settings.category : [];

                //SubCategory
                var subcategory = (drupal_settings.hasOwnProperty('subject') && drupal_settings.subject.length > 0) ? drupal_settings.subject : [];

                //Specialty
                var specialty = (drupal_settings.hasOwnProperty('specialty') && drupal_settings.specialty.length > 0) ? drupal_settings.specialty : [];

                //SchoolsSelected
                var school_selected = (drupal_settings.hasOwnProperty('institution') && drupal_settings.institution.length > 0) ? drupal_settings.institution : [];

                var paid_status_type = (drupal_settings.hasOwnProperty('paid_status_type')) ? drupal_settings.paid_status_type : "unknown";

                //Postal Code
                var location = "";

                if(drupal_settings.hasOwnProperty('postal_code')){

                    location = drupal_settings.postal_code;

                }else if(typeof jQuery.cookie != 'undefined' && jQuery.cookie('user_postal_code')){

                    location = jQuery.cookie('user_postal_code');

                }

                var vendor = 'VANTAGE';

                if (v.hasOwnProperty('Vendor') && v.Vendor != '') {
                    vendor = v.Vendor.toUpperCase();
                }

                switch (vendor) {

                    case 'VANTAGE':

                        //Loads the Get Globals Script as it will most likely not be present.
                        jQuery.getScript("[SERVICEBASE]/Static/GetGlobal", function (data, textStatus, jqxhr) {

                            if (jqxhr.status == 200) {

                                var level;

                                if (levels > 0) {

                                    switch (levels[0]) {

                                        case 4, 22, 21, 18:
                                            level = 1;
                                            break
                                        case 2:
                                            level = 2;
                                            break
                                        case 3:
                                            level = 3;
                                            break
                                        case 8:
                                            level = 4;
                                        case 7:
                                            level = 5;

                                    }

                                }
                                
                                googletag.cmd.push(function () { googletag.pubads().setTargeting("v_level", level.toString()); });
                                googletag.cmd.push(function () { googletag.pubads().setTargeting("loc", location); });

                                if (specialty.length > 0) {
                                    fe_getISMapping('VANTAGE.MAPPINGS', 'SPECIALTY', specialty[0])
                                    .success(function (data) {
                                        googletag.cmd.push(function () { googletag.pubads().setTargeting("sa", data); });
                                        eddy_ad_render(v);
                                    })
                                    .error(function () { eddy_ad_render(v); });
                                }
                                else if (subcategory.length > 0) {
                                    fe_getISMapping('VANTAGE.MAPPINGS', 'SUBCATEGORY', subcategory[0])
                                    .success(function (data) {
                                        googletag.cmd.push(function () { googletag.pubads().setTargeting("sa", data); });
                                        eddy_ad_render(v);
                                    })
                                    .error(function () { eddy_ad_render(v); });
                                }
                                else if (category.length > 0) {
                                    fe_getISMapping('VANTAGE.MAPPINGS', 'CATEGORY', category[0])
                                    .success(function (data) {
                                        googletag.cmd.push(function () { googletag.pubads().setTargeting("aos", data); });
                                        eddy_ad_render(v);
                                    })
                                    .error(function () { eddy_ad_render(v); });
                                }
                                else {
                                    eddy_ad_render(v);
                                }

                            } else {
                                eddy_ad_render(v);
                            }

                        });

                        break;

                    case 'CLICKSNET':

                        //Do what you need to do with Clicks Net.

                        break;

                    case 'MEDIAALPHA':
                        
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("elevel", [3]); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("loc", location); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("levels", levels); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("category", category); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("subject", subcategory); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("specialty", specialty); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("schools", school_selected); });
                        googletag.cmd.push(function () { googletag.pubads().setTargeting("paid_status_type", paid_status_type); });

                        eddy_ad_render(v);
                        break;

                }

            } else {

                eddy_ad_render(v);

            }

        } else {

            eddy_ad_render(v);

        }

    }

}


var googletag = googletag || {};

//Loads Object from the page after the page has completely loaded and jQuery has been found.
eddy_ad_defer_load_ad_script(function () {

    (function ($) {

        $(document).ready(function () {

            if (typeof _eaq != 'undefined') {

                var load_dfp_tag = false;

                //Checks to see if Doubleclick Should be loaded.
                $.each(_eaq, function (i, v) {

                    if (!load_dfp_tag && (v.AdServer == 'DFP' || !v.hasOwnProperty('AdServer'))) {

                        load_dfp_tag = true;

                    }

                });

                //Loads the DFP Script is it is not on the page.
                if (load_dfp_tag && !googletag.hasOwnProperty('cmd')) {

                    googletag.cmd = googletag.cmd || [];

                    var useSSL = 'https:' == document.location.protocol;
                    var gts_path = (useSSL ? 'https:' : 'http:') + '//www.googletagservices.com/tag/js/gpt.js';

                    $.getScript(gts_path);

                }

                //initialize _eaq attributes
                var count = _eaq.length;

                $.each(_eaq, function (i, v) {

                    if (!v.hasOwnProperty('WorkFlowStep')) {
                        $.extend(v, { 'SingleRequest': true });
                    }

                    if ((i + 1) == count) {

                        $.extend(v, { 'LastItem': true });

                    }

                    if (!v.hasOwnProperty('AdServer')) {

                        //Sets Double Click to be the default Ad Server.
                        v.AdServer = 'DFP';

                    }
                    
                });

                if (window.FormsEngine) { //if this page is a wizard, which contain workflowstep

                    $(FormsEngine).on("WorkflowChangedCompleted", function (page) {
                        //These are the only valid values available for the "WorkFlowStep" Property.
                        var allowed = ['START', 'MANAGEDCHOICE', 'THANKYOU', 'NOMATCH', 'NOTHANKYOU'];
                        $.each(_eaq, function (i, v) {
                            if (v.hasOwnProperty('WorkFlowStep') && $.inArray(v.WorkFlowStep, allowed) >= 0) {
                                if (page == v.WorkFlowStep) {
                                    eddy_ad_initialize(v);
                                }
                            }
                            else {
                                eddy_ad_initialize(v);
                            }
                        });
                    });
                } else {
                    $.each(_eaq, function (i, v) {
                        eddy_ad_initialize(v);
                    });
                }
            }
        });

        //Legacy way of rendering an Ad on the Wizard Pages. Should be removed after everything has been moved over to the new way.
        $(document).ready(function () {
            if (typeof (fe_global) == "function" && has_legacy_div_name()) {

                $(FormsEngine).on("WorkflowChangedCompleted", function (page) {

                    if (page == 'MANAGEDCHOICE') {
                        fe_vantageDCLoad('managedChoiceSponsoredDiv', true);
                    }
                    else if (page == 'THANKYOU') {
                        fe_vantageDCLoad('thankYouSponsoredDiv', true);
                    }
                    else if (page == 'NOMATCH') {
                        fe_vantageDCLoad('noMatchSponsoredDiv', true);
                    }
                });
            }
        });

        //Checks if Legacy Div Name is on the page.
        function has_legacy_div_name() {

            var output = false;
            if ($("[name=managedChoiceSponsoredDiv]").length > 0) {
                output = true;
            } else if ($("[name=thankYouSponsoredDiv]").length > 0) {
                output = true;
            } else if ($("[name=noMatchSponsoredDiv]").length > 0) {
                output = true;
            }

            return output;
        }
    })(jQuery);

});

//Prevents Script From Loading until jQuery has Loaded.
function eddy_ad_defer_load_ad_script(method) {
    if (window.jQuery) {
        method();
    }
    else {
        setTimeout(function () { eddy_ad_defer_load_ad_script(method) }, 50);
    };
}

/*
	Legacy Stuff
	Sites are still calling these function directly and we do not want them to cause page errors.
*/
function fe_vantageDCLoad(divName, isWizard) {
    jQuery(document).ready(function () {
        var v = {};
        v.Vendor = 'VANTAGE';
        v.AdServer = 'DFP';
        var wizard = true;
        if (isWizard === false) {
            wizard = false;
        }
        v.IsWizard = wizard;
        var div = jQuery("[name=" + divName + "]");

        if (div.length > 0) {
            v.RenderingDiv = div.attr('id');
            eddy_ad_initialize(v);
        }
    });

}