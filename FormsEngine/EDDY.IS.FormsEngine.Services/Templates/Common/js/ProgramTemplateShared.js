(function ($) {
    $(document).ready(function () {
        $("#uab-popup-close,#uab-popup-nothankyou").on("click", function (event) {
            event.preventDefault();
            $("#eddyuab").fadeOut(200);
            $("#FEmodalOverlayBG").fadeOut(200);
        });
        FormsEngine.CountryValidationFailureEvent = function (e) {
            $("#eddyuab").fadeIn(200);
            $("#FEmodalOverlayBG").fadeTo(200, 0.5);
            $("#eddyuab").removeClass('hide');
            $('html, body').animate({ scrollTop: $("#eddyuab").offset().top - 10 }, 1000);
        }
    });
})(jQuery);