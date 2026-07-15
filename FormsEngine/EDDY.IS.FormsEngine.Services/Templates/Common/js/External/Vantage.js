jQuery(document).ready(function () {

    function loadiFrame(iframeID, args){
        var iframe = jQuery("#" + iframeID);
        if (iframe.length > 0) {
            var url = iframe.attr('data-src');
            if(url.indexOf("?")==-1) {
                url= url+ "?";
            }
            iframe.attr('src', url+ '&' + args.join('&'));
        }
    }

    function vantageiFrameLoad(iframeID) {
        var iframe = jQuery("#" + iframeID);
        if (iframe.length > 0) {
            fe_getFormFieldsSessionValues(['Categories_Selections', 'SubCategories_Selections', 'Specialties_Selections', 'Highest_Level_of_Education_Completed', 'Military_Affiliation', 'Postal_Code'], function (data) {

                //Category
                var result = jQuery.grep(data, function (e) { return e.Key == 'Categories_Selections'; });
                var category = result.length == 1 ? result[0].Value.split(",")[0] : "";

                //SubCategory
                var result = jQuery.grep(data, function (e) { return e.Key == 'SubCategories_Selections'; });
                var subcategory = result.length == 1 ? result[0].Value.split(",")[0] : "";

                //Specialty
                var result = jQuery.grep(data, function (e) { return e.Key == 'Specialties_Selections'; });
                var specialty = result.length == 1 ? result[0].Value.split(",")[0] : "";

                //Education Level
                var result = jQuery.grep(data, function (e) { return e.Key == 'Highest_Level_of_Education_Completed'; });
                var elevelid = result.length == 1 ? result[0].Value : "";


                //Military
                var result = jQuery.grep(data, function (e) { return e.Key == 'Military_Affiliation'; });
                var militaryid = result.length == 1 ? result[0].Value : "";

                //Postal Code
                var result = jQuery.grep(data, function (e) { return e.Key == 'Postal_Code'; });
                var location = result.length == 1 ? result[0].Value : "";

                //Military
                var vMilitaryid = militaryid == '' || militaryid == '126' ? 2 : 1;

                var vEducationLevel = 1;
                if (elevelid == '2' || elevelid == '3') {
                    vEducationLevel = 2;
                }
                else if (elevelid == '4' || elevelid == '5' || elevelid == '6' || elevelid == '7' || elevelid == '8') {
                    vEducationLevel = 3;
                }
                if (elevelid == '9') {
                    vEducationLevel = 4;
                }
                if (elevelid == '10') {
                    vEducationLevel = 5;
                }
                if (elevelid == '11') {
                    vEducationLevel = 6;
                }

                var qsArguments = [];
                qsArguments.push('elevel=' + vEducationLevel);
                qsArguments.push('mil=' + vMilitaryid);
                qsArguments.push('loc=' + location);

                if (specialty != '') {
                    fe_getISMapping('VANTAGE.MAPPINGS', 'SPECIALTY', specialty)
                    .success(function (data) {
                        qsArguments.push('sa=' + data);
                        loadiFrame(iframeID, qsArguments);
                    })
                    .error(function () { loadiFrame(iframeID, qsArguments); });
                }
                else if (subcategory != '') {
                    fe_getISMapping('VANTAGE.MAPPINGS', 'SUBCATEGORY', subcategory)
                    .success(function (data) {
                        qsArguments.push('sa=' + data);
                        loadiFrame(iframeID, qsArguments);
                    })
                    .error(function () { loadiFrame(iframeID, qsArguments); });
                }
                else if (category != '') {
                    fe_getISMapping('VANTAGE.MAPPINGS', 'CATEGORY', category)
                    .success(function (data) {
                        qsArguments.push('aos=' + data);
                        loadiFrame(iframeID, qsArguments);
                    })
                    .error(function () { loadiFrame(iframeID, qsArguments); });
                }
                else {
                    loadiFrame(iframeID, qsArguments);
                }
            });
        }
    }

    if (typeof (fe_global) == "function") {
        jQuery(FormsEngine).on("WorkflowChangedCompleted", function (page) {

            if (page == 'MANAGEDCHOICE') {
                vantageiFrameLoad('managedChoiceSponsorediframe');
            }
            else if (page == 'THANKYOU') {
                vantageiFrameLoad('thankYouSponsorediframe');
            }
            else if (page == 'NOMATCH') {
                vantageiFrameLoad('noMatchSponsorediframe');
            }
        });
    }

});

