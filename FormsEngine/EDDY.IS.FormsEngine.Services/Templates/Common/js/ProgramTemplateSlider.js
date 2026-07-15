(function ($) {
    var CurrentStep = 1;
    $("div[class^='Step']").hide();
    $(".Step1").show();

    function checkNavigation() {
        if (CurrentStep == 1) {
            $("#go-left").hide();
        }
        else {
            $("#go-left").show();
        }

        if ($(".Step" + (CurrentStep + 1)).length > 0) {
            $("#eddynexusformSubmit").hide();
            $("#go-right").show();
        }
        else {
            $("#eddynexusformSubmit").show();
            $("#go-right").hide();
        }
    }
    checkNavigation();

    function validStep() {
        var valid = true;
        $(".Step" + CurrentStep).find(":input").not("[type=hidden]").not("[type=radio]").each(function () {
            valid &= $(FormsEngine.DefaultFormTag).validate().element(this);
        });
        return valid;
    }

    // use buttons to change slide
    $("#go-left").bind("click", function () {
        if (CurrentStep > 1) {
            $(".Step" + CurrentStep).fadeOut(200, function () {
                $("#ErrorBoxSection" + CurrentStep).hide();
                CurrentStep--;
                $("#ErrorBoxSection" + CurrentStep).show();
                checkNavigation();
                $(".Step" + CurrentStep).fadeIn(100, function () {
                })
            });
        }
    });
    $("#go-right").bind("click", function () {
        if ($(".Step" + (CurrentStep + 1)).length > 0) {
            if (validStep()) {
                $(".Step" + CurrentStep).fadeOut(200, function () {
                    CurrentStep++;
                    $("#ErrorBoxSection" + CurrentStep).show();
                    checkNavigation();
                    $(".Step" + CurrentStep).fadeIn(100, function () {

                    })
                });
            }
        }
    });
})(jQuery);