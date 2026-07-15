(function ($) {
    // QDF_Start.js

    function onStepLoaded(StepNumber) {
        //Hide back and Continue since menu is there
        if (StepNumber == 1) {
            $('#StartNow').show();
            $('#BSProgressBarContainer, .progress-text').hide();
            $('.eddy-form-wizard-header .wizard-header-content').removeClass('col-xs-12 col-xs-offset-0 col-sm-11 col-sm-offset-1');
            $('.eddy-form-wizard-header .wizard-header-content').addClass('col-xs-7 col-xs-offset-0 col-sm-7 col-sm-offset-0');
            $('#prev-top-img').hide();

        }
        else {
            $('#StartNow').hide();
            $('#BSProgressBarContainer, .progress-text').show();
            $('.eddy-form-wizard-header .wizard-header-content').removeClass('col-xs-7 col-xs-offset-0 col-sm-7 col-sm-offset-0');
            $('.eddy-form-wizard-header .wizard-header-content').addClass('col-xs-12 col-xs-offset-0 col-sm-11 col-sm-offset-1');
            $('#prev-top-img').show();
        }
    }



    $(document).ready(function () {

        // Constants
        FormsEngine.DefaultFormTag = FormsEngine.DefaultFormTag || "#eddynexusform-wizard";

        // Internals events
        FormsEngine.OnStepLoadedInternal = onStepLoaded;
        onStepLoaded(1);

        fe_wiz_showSelectAllButtons();
        console.log("checks");
        fe_wiz_selectAllMultiCheckBoxField('Categories', true);

        fe_wiz_selectAllMultiCheckBoxField('SubCategories', true);
        console.log("checks");
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

        //Top nav buttons support
        $('#next-top-img').click(function (event) {
            $(FormsEngine.SubmitButton).trigger("click");
        });

        $('#prev-top-img').click(function (event) {
            $(FormsEngine.BackButton).trigger("click");
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

        //see more buttons
        $('button#Categories_SeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulCategories').toggleClass('expand');
        });
        $('button#SubCategories_SeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulSubCategories').toggleClass('expand');
        });
        $('button#Specialties_SeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulSpecialties').toggleClass('expand');
        });

        $('button#Categories_MobileSeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulCategories').toggleClass('expand');
        });
        $('button#SubCategories_MobileSeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulSubCategories').toggleClass('expand');
        });
        $('button#Specialties_MobileSeeMoreBtn').bind('click', function (e) {
            e.preventDefault();

            $('#ulSpecialties').toggleClass('expand');
        });

        $('button.SeeMoreBtn').bind('click', function (e) {
            e.preventDefault();
            var name = $(this).attr('id').replace('_SeeMoreBtn', '');
            $('.' + name).toggleClass('expand');
        });

        $('button.mobileSeeMore').bind('click', function (e) {
            e.preventDefault();
            var name = $(this).attr('id').replace('_MobileSeeMoreBtn', '');
            $('.' + name).toggleClass('expand');
        });

        $("button.SeeMoreBtn").click(function () {
            $(this).text() === 'See More'
                ? $(this).text('See Less')
                : $(this).text('See More')
        })

    });

})(jQuery);