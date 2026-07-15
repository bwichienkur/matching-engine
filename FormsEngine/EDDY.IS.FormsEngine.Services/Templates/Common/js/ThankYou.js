(function ($) {
    // ThankYou.js
    // Used by Wizard flow
    //-------------------
    $(document).ready(function () {

        FormsEngine.InfoPopup = '#info-popup';
        FormsEngine.ThankYouDivTag = '#thank-you-page';


        $(FormsEngine.ThankYouDivTag).find('.school-info .school-name a, .school-info .program-name a').on('click', function (event) {
            event.preventDefault();
            var dataholder = $(this).parents('li');
            var popup = $(FormsEngine.InfoPopup);

            var y = event.screenY;

            $(popup).css('top', event.pageY);
            $(popup).css('left', '0');

            // get data
            var data = $(this).parent().next().html();
            //Helen add hidden institution logo for popup info
            var InstitutionLogo = $(dataholder).find('.school-logo').html();
            // construct html
            var infoPopup = '';
            infoPopup += '<div id="progDescTop" class="hide"></div>';
            infoPopup += '<div class="popup-info-wrapper">';
            infoPopup += '<a class="info-popup-close popup-close" href="#"></a>';
            infoPopup += '<div class="popup-content">';
            infoPopup += '<div class="us-institution-logo hide">' + InstitutionLogo + '</div>';
            infoPopup += '<h4 class="popup-title">' + $(this).text() + '</h4>';
            infoPopup += '<div class="Description">' + data + '</div>';
            infoPopup += '</div></div>';
            infoPopup += '<div id="progDescBtm" class="hide"></div>';

            $(popup).html(infoPopup);
            $(popup).fadeIn('slow');
        });


        $('.popup-close').on('click', function (event) {
            event.preventDefault();
            $(this).parents('.popup-info').fadeOut('slow');
        });

        $('.popup-info').scroll(function () {
            var el = $(this).find('.popup-close');
            var elpos_original = 10;
            var y = $(this).scrollTop();
            var finaldestination = y;
            if (y < elpos_original) {
                finaldestination = elpos_original;
                el.stop().css({ 'top': 10 });
            } else {
                el.stop().animate({ 'top': finaldestination - elpos_original + 15 }, 100);
            }
        });
        
    });

})(jQuery);
