(function ($) {
    // WizardDrip_ManagedChoice.js

    // Constants
    
    // Internals variables


    function displayInitialTabs(OnlineCampusTabs, GroundTab, OnlineTab, USGroundSchools, USOnlineSchools) {
        //if ($(USOnlineSchools).length > 0) { $(OnlineTab).show(); };
        //if ($(USGroundSchools).length > 0) { $(GroundTab).show(); };

        if ($(USOnlineSchools).length > 0) {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
            } else {
                switchToOnlineTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
                $(OnlineTab).parent().addClass("OnlineSingle");
            }
        } else {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
                $(GroundTab).parent().addClass("CampusSingle");
            } else {
                $(OnlineCampusTabs).hide();
            }
        }
    }

    function switchToOnlineTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools) {
        $(GroundTab).addClass("inactive");
        $(OnlineTab).removeClass("inactive");
        $(USGroundSchools).css("display", "none");
        $(USOnlineSchools).css("display", "block");
    }

    function switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools) {
        $(OnlineTab).addClass("inactive");
        $(GroundTab).removeClass("inactive");
        $(USOnlineSchools).css("display", "none");
        $(USGroundSchools).css("display", "block");
    }

    //move loader to fit school select page
    function moveLoader() {
        if ($("[id='FEmodalOverlayBG']").length == 1) {

            $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("top", ($("#mc-us-choice-list").offset().top - $('#eddy-form-container').offset().top) + "px");
            $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("height", $("#mc-us-choice-list").height());
            $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("width", "95%");
            $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("left", "8px");

            $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEloader']").css("top", ($(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("top") + $(FormsEngine.WorkflowManagedChoiceDivTag).find("[id='FEmodalOverlayBG']").css("top")/2 + 25) + "px");

        }
    }

    function displayUSProgramInfoLink(ddl) {
        var selectedItem = $(ddl).find('option:selected');
        var usProgramInfoLink = $(ddl).parents('li').find('[name="us-program-popup-link"]');
        $(usProgramInfoLink).hide();
        if ($(selectedItem).exists() && $(selectedItem).val() != '' && $(selectedItem).attr('data-hasprogramdescription').toLowerCase() == 'true') {
            $(usProgramInfoLink).text("About " + $(ddl).find('option:selected').attr('data-program-name'));
            $(usProgramInfoLink).show();
        }
    }

    $(document).ready(function () {

		if ($(".bg-content").css("width") == "265px" )  {
			if ($('#eddy-form-wizard-managedchoice-container').length != 0) {
				$( ".group-left" ).remove();
				$( ".group-right" ).addClass( "managed" );
			}
		}
		
        FormsEngine.WorkflowManagedChoiceDivTag = FormsEngine.WorkflowManagedChoiceDivTag || $('#eddy-form-wizard-managedchoice-container');
        FormsEngine.MoveLoader = FormsEngine.MoveLoader || function () { moveLoader(); };

        // needed for the data model retrieved for SchoolSelection page
        FormsEngine.SplitCampusTypeInResults = true;
        fe_saveWorkflowData(function () { }); // update the SessionDTO

        FormsEngine.LimitNumberOfUserSelections = true;

        //tab switch code starts here
        var USOnlineSchools = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#mc-US-Area li[data-isgroundcampus="False"]');
        var USGroundSchools = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#mc-US-Area li[data-isgroundcampus="True"]');
        var OnlineCampusTabs = $(FormsEngine.WorkflowManagedChoiceDivTag).find('#OnlineCampusTabs');
        var OnlineTab = $(OnlineCampusTabs).find('#OnlineTab');
        var GroundTab = $(OnlineCampusTabs).find('#CampusTab');

        $(OnlineTab).click(function () {
            switchToOnlineTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
        });

        $(GroundTab).click(function () {
            switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
        });

        //initial display schools
        displayInitialTabs(OnlineCampusTabs, GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);

        //if($('div[name="form-nothanks-button"]:visible'){
        //    $(FormsEngine.ManagedChoiceSubmitTag).addClass("forceMatch");
        //}else{
        //    $(FormsEngine.ManagedChoiceSubmitTag).removeClass("forceMatch");
        //}

        // user select checkboxes click event
        $(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]').on('click', function (event) {
            // Limit the number of programs a user can submit to
            fe_mc_limitUserSelectionCount();

            // if there is at least one checkbox checked/selected, enable the Request Info button
            if ($(FormsEngine.WorkflowManagedChoiceDivTag).find('input[name=ms-us-school-checkbox]:checked').first().exists()) {
                fe_mc_removeErrorMsgForSubmit();
                fe_mc_removeExpressConsentErrorMsg();
                fe_mc_updateExpressConsent();
            }
            else {
                fe_mc_updateExpressConsent();
            }
        });

        $(FormsEngine.WorkflowManagedChoiceDivTag).find('select[name="mc-us-program-ddl"]').each(function(){
            displayUSProgramInfoLink(this);
        });

        $(FormsEngine.WorkflowManagedChoiceDivTag).find('select[name="mc-us-program-ddl"]').on("change", function () {
            displayUSProgramInfoLink(this);
        });

        // Initialize Continue Mobile Button..
        if (FormsEngine.ShowContinueMobileButton) {
            ContinueMobileButtonInit($("#managechoice-form-submit-button"));
        }
     
        if (FormsEngine.ClickableUSSchool) {
            initializeUSClickableBoxes();
        }

    }); //end of document.ready



})(jQuery);

function initializeUSClickableBoxes() {
    jQuery(".eddy-form-wizard-container .field-holder.mc-US-Area .mc-us-school").addClass("clickable");

    jQuery(".mc-us-school input:checkbox:checked").closest(".mc-us-school").addClass("mc-us-schoolcontainer-selected");
    jQuery(".mc-us-school input:checkbox").not(":checked").closest(".mc-us-school").removeClass("mc-us-schoolcontainer-selected");

    jQuery(".mc-us-school").find("div.school-name").append('<img src="' +
        FormsEngine.ServiceBaseURL + '/Templates/Common/images/info.png" class="mc-us-more-info">');

    jQuery(document).on("click", ".ms-us-school-checkbox, .mc-us-school select, .us-program-popup-link, .mc-us-more-info, div[name='mc-SM-additional-questions']", function (e) {
        e.stopImmediatePropagation();

        if (jQuery(this).hasClass("ms-us-school-checkbox")) {
            if (jQuery(this).prop("checked")) {
                jQuery(this).closest(".mc-us-school:not(.disable)").addClass("mc-us-schoolcontainer-selected");
            }
            else {
                jQuery(this).closest(".mc-us-school:not(.disable)").removeClass("mc-us-schoolcontainer-selected");
            }
        }
    })

    jQuery(document).on("click", ".mc-us-school:not(.disable)", function (e) {
        
        var alreadySelected = jQuery(this).find("input:checkbox").prop("checked");
        if (alreadySelected){
            jQuery(this).removeClass("mc-us-schoolcontainer-selected");
        }
        else{
            jQuery(this).addClass("mc-us-schoolcontainer-selected");
        }
        
        jQuery(this).find("input:checkbox").prop("checked", !alreadySelected).trigger("change");
    })

    jQuery(FormsEngine.WorkflowManagedChoiceDivTag).on('click', '.mc-us-more-info', fe_mc_showUSPopup);
}