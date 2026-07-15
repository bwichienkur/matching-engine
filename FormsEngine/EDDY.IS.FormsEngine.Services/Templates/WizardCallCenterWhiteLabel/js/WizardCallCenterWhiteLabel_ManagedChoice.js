(function ($) {
    // WizardCallCenterWhiteLabel_ManagedChoice.js

    // Constants

    // Internals variables


    function displayInitialTabs(OnlineCampusTabs, GroundTab, OnlineTab, USGroundSchools, USOnlineSchools) {
        if ($(USOnlineSchools).length > 0) { $(OnlineTab).show(); };
        if ($(USGroundSchools).length > 0) { $(GroundTab).show(); };

        if ($(USOnlineSchools).length > 0) {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
            } else {
                switchToOnlineTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
                $(OnlineTab).addClass("OnlineSingle");
            }
        } else {
            if ($(USGroundSchools).length > 0) {
                switchToGroundTab(GroundTab, OnlineTab, USGroundSchools, USOnlineSchools);
                $(GroundTab).addClass("CampusSingle");
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

    $(document).ready(function () {

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
             
    }); //end of document.ready



})(jQuery);