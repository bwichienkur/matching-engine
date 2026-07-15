(function (jQuery) {
    jQuery(document).ready(function () {
        var SentPhoneList = new Array();
        var SentEmailList = new Array();
        var serverUrl = '//leadping.educationdynamics.com/';
        var VERSION = 'V3';

        jQuery("[name*=Email]").blur(LeadPing_FireEmailPing);
        jQuery("[name*=email]").blur(LeadPing_FireEmailPing);
        jQuery("[name*=Phone]").blur(LeadPing_FireSingleFieldPhonePing);
        jQuery("[name*=phone]").blur(LeadPing_FireSingleFieldPhonePing);
        jQuery("#night_phone_suffix").blur(LeadPing_night_phone_suffix_FireMultipleFieldPhonePing);
        jQuery("#day_phone_suffix").blur(LeadPing_day_phone_suffix_FireMultipleFieldPhonePing);

        jQuery('input[type=button]').click(ResetPhoneFocus);
        jQuery('input[type=submit]').click(ResetPhoneFocus);


        function LeadPing_FireEmailPing() {
            try {
                var jQueryinput = jQuery(this);
                var Email = jQueryinput.val();
                var LeadID = jQuery(FormsEngine.DefaultFormTag).find('input[code="leadid_token"]').attr('value');

                if (LeadID == null) {

                    LeadID = "0";

                }

                var length = SentEmailList.length;
                var Sent;
                while (length--) {
                    if (Email.indexOf(SentEmailList[length]) != -1) {
                        Sent = true;
                    }
                }
                if (!Sent) {
                    if (Email.length > 0) {
                        jQuery.getJSON(serverUrl + VERSION + '/Client/Service.svc/json/FireEmailPing?Email=' + Email + '&LeadID=' + LeadID + '&callback=?', null, function (ResponseMessage) {
                            SentEmailList.push(jQueryinput.val());
                        });
                    }
                }
            }
            catch (err) {
                //Don't break page!
            }
        }

        function LeadPing_FireSingleFieldPhonePing() {
            try {
                var jQueryinput = jQuery(this);
                var Phone = jQueryinput.val();
                var length = SentPhoneList.length;
                var Sent;
                while (length--) {
                    if (Phone.indexOf(SentPhoneList[length]) != -1) {
                        Sent = true;
                    }
                }
                if (!Sent) {
                    if (Phone.length >= 10) {
                        jQuery.getJSON(serverUrl + VERSION + '/Client/Service.svc/json/FirePhonePing?Phone=' + Phone + '&callback=?', null, function (ResponseMessage) {
                            SentPhoneList.push(jQueryinput.val());
                        });
                    }
                }
            }
            catch (err) {
                //Don't break page!
            }
        }


        function LeadPing_night_phone_suffix_FireMultipleFieldPhonePing() {
            try {
                var Phone = jQuery("#night_phone_area").val();
                Phone = Phone + jQuery("#night_phone_prefix").val();
                Phone = Phone + jQuery("#night_phone_suffix").val();
                var length = SentPhoneList.length;
                var Sent;
                while (length--) {
                    if (Phone.indexOf(SentPhoneList[length]) != -1) {
                        Sent = true;
                    }
                }
                if (!Sent) {
                    if (Phone.length >= 10) {
                        jQuery.getJSON(serverUrl + VERSION + '/Client/Service.svc/json/FirePhonePing?Phone=' + Phone + '&callback=?', null, function (ResponseMessage) {
                            SentPhoneList.push(Phone);
                        });
                    }
                }
            }
            catch (err) {
                //Don't break page!
            }
        }


        function LeadPing_day_phone_suffix_FireMultipleFieldPhonePing() {
            try {
                var Phone = jQuery("#day_phone_area").val();
                Phone = Phone + jQuery("#day_phone_prefix").val();
                Phone = Phone + jQuery("#day_phone_suffix").val();
                var length = SentPhoneList.length;
                var Sent;
                while (length--) {
                    if (Phone.indexOf(SentPhoneList[length]) != -1) {
                        Sent = true;
                    }
                }
                if (!Sent) {
                    if (Phone.length >= 10) {
                        jQuery.getJSON(serverUrl + VERSION + '/Client/Service.svc/json/FirePhonePing?Phone=' + Phone + '&callback=?', null, function (ResponseMessage) {
                            SentPhoneList.push(Phone);
                        });
                    }
                }
            }
            catch (err) {
                //Don't break page!
            }

        }

        function ResetPhoneFocus() {
            jQuery("[name*=phone]").trigger("blur");
            jQuery("[name*=Phone]").trigger("blur");
        }




    });
}(jQuery));