(function ($) {
    // WizardProfessional_Start.js

    function onStepLoaded(StepNumber) {
        if (StepNumber == 1) {
            $('#prev-top-img').hide();
        }
        else {
            $('#prev-top-img').show();
        }

        //Read the Legend for the Step and place it on the header 
        if ($('fieldset:visible legend').length > 1) {
            step_title = $('#Step' + StepNumber + ' #Section1').closest('fieldset').children('legend').text();
            $('#Step' + StepNumber + ' #Section1').closest('fieldset').children('legend').hide();
        }
        else {
            step_title = $('fieldset:visible legend').text();
            $('#Step' + StepNumber + ' #Section1 legend').hide();
        }

        if ($("#wizard-step-title").length == 0) {
            $(".wizard-header-content").prepend('<div id="wizard-step-title"><h3>' + step_title + '</h3></div>');
        }
        else {
            $("#wizard-step-title h3").text(step_title);
        }
      
        /*if (StepNumber == 3) {
            if ($("#age-country-group").length == 0) {

                $(".eddy-form-wizard-container .field-holder.Age").before('<div id="age-country-group"></div');
                $("#age-country-group").append($(".eddy-form-wizard-container .field-holder.Age"));
                $("#age-country-group").append($(".eddy-form-wizard-container .field-holder.Country"));
            } 
        } Not a good idea!*/

        if (StepNumber == 4) {
            $('.eddy-form-wizard-body #eddynexusform-wizard .Preferred_Methods_of_Contact fieldset label').addClass('col col-xs-3');
            $('.eddy-form-wizard-body #eddynexusform-wizard .Preferred_Methods_of_Contact fieldset input[type="checkbox"]').addClass('col col-xs-1');
        }

        if ($("#screen-button:visible").length == 0 && !fe_hasSubmitButtonLabelTextLastAlreadyBeenSet()) {
            FormsEngine.SubmitButtonLabelTextLast = "Request Info";
        }
    }

    function updateProgramWizardSelectionCategorySubCategorySpecialty(data) {

        if (data.ProgramDetails.CategoryList != undefined && data.ProgramDetails.CategoryList != null && data.ProgramDetails.CategoryList.length > 0) {
            var itemCategory = { "id": data.ProgramDetails.CategoryList[0].ItemId, "text": data.ProgramDetails.CategoryList[0].ItemValue, "operation": true, "type": "Categories" };
            FormsEngine.LastCategory = data.ProgramDetails.CategoryList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemCategory, true);
            if ($('#categories_selections').length == 0) {
                $('<input>').attr({
                    type: 'hidden',
                    id: 'categories_selections',
                    name: 'categories_selections',
                    value: FormsEngine.LastCategory
                }).appendTo(FormsEngine.DefaultFormTag);

            } else {
                $('#categories_selections').val(FormsEngine.LastCategory);
            }

            fe_pushToGTMDataLayer({ 'category': itemCategory.text });
        }

        if (data.ProgramDetails.SubjectList != undefined && data.ProgramDetails.SubjectList != null && data.ProgramDetails.SubjectList.length > 0) {
            var itemSubCategory = { "id": data.ProgramDetails.SubjectList[0].ItemId, "text": data.ProgramDetails.SubjectList[0].ItemValue, "operation": true, "type": "SubCategories" };
            FormsEngine.LastSubCategory = data.ProgramDetails.SubjectList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemSubCategory, true);
            if ($('#subcategories_selections').length == 0) {
                $('<input>').attr({
                    type: 'hidden',
                    id: 'subcategories_selections',
                    name: 'subcategories_selections',
                    value: FormsEngine.LastSubCategory
                }).appendTo(FormsEngine.DefaultFormTag);

            } else {
                $('#subcategories_selections').val(FormsEngine.LastSubCategory);
            }

            fe_pushToGTMDataLayer({ 'subject': itemSubCategory.text });
        }

        if (data.ProgramDetails.SpecialtyList != undefined && data.ProgramDetails.SpecialtyList != null && data.ProgramDetails.SpecialtyList.length > 0) {
            var itemSpecialty = { "id": data.ProgramDetails.SpecialtyList[0].ItemId, "text": data.ProgramDetails.SpecialtyList[0].ItemValue, "operation": true, "type": "Specialties" };
            FormsEngine.LastSpecialty = data.ProgramDetails.SpecialtyList[0].ItemId;
            fe_updateSelectionCategorySubCategorySpecialtyIntoCookie(itemSpecialty, true);
            if ($('#specialties_selections').length == 0) {
                $('<input>').attr({
                    type: 'hidden',
                    id: 'specialties_selections',
                    name: 'specialties_selections',
                    value: FormsEngine.LastSpecialty
                }).appendTo(FormsEngine.DefaultFormTag);

            } else {
                $('#specialties_selections').val(FormsEngine.LastSpecialty);
            }

            fe_pushToGTMDataLayer({ 'specialty': itemSpecialty.text });
        }



    }
    
    $(document).ready(function () {
        
        // Constants
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        
        // Internals events
        FormsEngine.OnStepLoaded = onStepLoaded;
        onStepLoaded(1);

        fe_wiz_showSelectAllButtons();

        fe_wiz_focusOnNextFocusable('null');


        if (typeof FormsEngine.DisableAutoTab === "undefined") {
            $(FormsEngine.DefaultFormTag).keydown(function (e) {
                if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) {
                    fe_wiz_keyTabAndEnterEvents(e);
                }
            });

            //NextStep on Enter and tab management
            jQuery(document).keypress(function (e) {
                if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
            });

            jQuery(document).keydown(function (e) {
                if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
            });

            jQuery(FormsEngine.DefaultFormTag).keypress(function (e) {
                if (e.keyCode == 9 || e.which == 9 || e.keyCode == 13 || e.which == 13) { e.preventDefault ? e.preventDefault() : e.returnValue = false; return false; }
            });
        }
        else {
            $(FormsEngine.DefaultFormTag).keydown(function (e) {
                if (e.keyCode == 13 || e.which == 13) {
                    fe_wiz_keyTabAndEnterEvents(e);
                }
            });
        }

        //Top nav buttons support
        $('#next-top-img').click(function (event) {
            $(FormsEngine.SubmitButton).trigger("click");
        });

        $('#prev-top-img').click(function (event) {
            $(FormsEngine.BackButton).trigger("click");
        });

        
        $(FormsEngine.DefaultFormTag).find('.UseDayTimePhone').on("click",function () {
            $(FormsEngine.DefaultFormTag).find(':input[code="Alternate_Phone"]').val($(FormsEngine.DefaultFormTag).find(':input[name="Phone"]').val());
        });


        if ($(FormsEngine.DefaultFormTag).find(":input[code='K12']").exists()
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").isControlBefore($(FormsEngine.DefaultFormTag).find(":input[code='K12']")
                && $(FormsEngine.DefaultFormTag).find(":input[code='Highest_Level_of_Education_Completed']").val() == "")
              ) {
            $("div[data-controlcode='K12']").hide();
        }
        
        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#wizard-form-submit-button"));
        }

        FormsEngine.ProgramWizardDetailChangedEvent = updateProgramWizardSelectionCategorySubCategorySpecialty;
        
    });

})(jQuery);