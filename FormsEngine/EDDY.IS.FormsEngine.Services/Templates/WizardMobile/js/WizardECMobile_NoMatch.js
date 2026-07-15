(function ($) {
    // WizardECMobile_NoMatch.js

    $(document).ready(function () {
        
        if (typeof Drupal != 'undefined') {

            if (Drupal.settings.pageContent != null) {

                mobile_logo = document.createElement('img');
                mobile_logo.src = Drupal.settings.pageContent.logo;
                $("#mobile_header_image").html(mobile_logo);
                
                if (Drupal.settings.pageContent.content.field_mobile_text != null) {
                    $("#mobile_header_text").html(Drupal.settings.pageContent.content.field_mobile_text[0].markup);
                }

                if (Drupal.settings.pageContent.content.field_mobile_copy != null) {
                    $("#mobile_copy").html(Drupal.settings.pageContent.content.field_mobile_copy[0].markup);
                    $("#mobile_copy").prepend("<br />");
                    $("#mobile_copy").append("<br /><br />");
                }

                if (Drupal.settings.pageContent.content.field_mobile_success_kit != null) {
                    $("#mobile_success_kit").html(Drupal.settings.pageContent.content.field_mobile_success_kit[0].markup);
                }

                if (Drupal.settings.pageContent.content.field_mobile_disclaimer != null) {

                    $("#mobile_disclaimer").html(Drupal.settings.pageContent.content.field_mobile_disclaimer[0].markup);
                    $("#mobile_disclaimer").prepend("<br />");
                }

                if (Drupal.settings.pageContent.content.copyright != null) {
                    $("#mobile_copyr").html('Drupal.settings.pageContent.content.field_copyright[0].markup');
                }
            }
        }
       
        $(function () {
            $('#mobile_header_image img').data('size', 'big');
        });

        $(window).scroll(function () {
            if ($(document).scrollTop() > 0) {
                if ($('#mobile_header_image img').data('size') == 'big') {

                    $('#mobile_header_image img').data('size', 'small');
                    $('#mobile_header_image img').stop().animate({
                        height: '36px'
                    }, 300);
                    $("#mobile_header_text").css({ "font-size": "12px" });
                }
            }
            else {
                if ($('#mobile_header_image img').data('size') == 'small') {
                    $('#mobile_header_image img').data('size', 'big');
                    $('#mobile_header_image img').stop().animate({
                        height: '53px'
                    }, 300);
                    $("#mobile_header_text").css({ "font-size": "16px" });
                }
            }
        });

         

    });


})(jQuery);
