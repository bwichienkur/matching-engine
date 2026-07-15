(function ($) {
    // InstitutionWizard_Start.js

    function onStepLoaded(stepNumber, direction) {
        //Hide back and Continue since menu is there
        if (stepNumber == 1) {
            $('#prev-top-img').hide();
        } else {
            $('#prev-top-img').show();
        }

        fe_sp_selectVisitedStepIndicators();
        fe_sp_displayFutureSteps(3);
        fe_sp_initializeMatchReplacementEventListeners(stepNumber, direction);
        fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown();
        fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();
        fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions(stepNumber);
    }

    $(document).ready(function () {

        // Constants
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";
        FormsEngine.RestartButton = "#form-startover-button";

        // Internals events
        FormsEngine.OnStepLoadedInternal = onStepLoaded;
        onStepLoaded(1);

        fe_wiz_showSelectAllButtons();

        fe_wiz_focusOnNextFocusable('null');

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

        $(FormsEngine.DefaultFormTag).find('.UseDayTimePhone').on("click", function () {
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

        jQuery(FormsEngine).on("loadFormWizard", function () {
            fe_sp_loadFormWizardCallBack();
        });

        jQuery(FormsEngine).on("subcategoriesLoaded", function () {
            if (window.innerWidth <= 670) {
                fe_sp_buildSubcategoryCarousel();
            }
        });

        jQuery(FormsEngine.DefaultFormTag).on("controlLoadedFromSession", function () {
            fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown();
        });

        jQuery(FormsEngine.DefaultFormTag).find(':input').change(function () {
            fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown();
        });

        jQuery(FormsEngine.RestartButton).on("click", function () {
            FormsEngine.LoadWorkflowStep('START', '');
        });

        // touch gestures
        jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel").on("touchstart", function (event) {
            handleTouchStart(event);
        });

        jQuery(FormsEngine.DefaultFormTag + " #school-picker-carousel").on("touchmove", function () {
            handleTouchMove(event, "#school-picker-carousel", "#school-picker-carousel-indicators");
        });

        jQuery(FormsEngine.DefaultFormTag + " #ulSubCategories").on("touchstart", function (event) {
            handleTouchStart(event);
        });

        jQuery(FormsEngine.DefaultFormTag + " #ulSubCategories").on("touchmove", function () {
            handleTouchMove(event, "#subcategory-carousel", "#subcategories-carousel-indicators");
        });

        var xDown = null;
        var yDown = null;

        function getTouches(event) {
            return event.touches ||             // browser API
                event.originalEvent.touches; // jQuery
        }

        function handleTouchStart(event) {
            const firstTouch = getTouches(event)[0];
            xDown = firstTouch.clientX;
            yDown = firstTouch.clientY;
        }

        function handleTouchMove(event, carouselId, dotContainerId) {
            if (!xDown || !yDown) {
                return;
            }

            var xUp = event.touches[0].clientX;
            var yUp = event.touches[0].clientY;

            var xDiff = xDown - xUp;
            var yDiff = yDown - yUp;

            var absXDiff = Math.abs(xDiff);
            var absYDiff = Math.abs(yDiff);

            if (absXDiff > absYDiff) {/*most significant*/
                if (xDiff > 0) {
                    /* left swipe */
                    fe_sp_moveToNextSlide(carouselId, dotContainerId);
                } else {
                    /* right swipe */
                    fe_sp_moveToPrevSlide(carouselId, dotContainerId);
                }
            } 

            /* reset values */
            xDown = null;
            yDown = null;
        }


    });

})(jQuery);