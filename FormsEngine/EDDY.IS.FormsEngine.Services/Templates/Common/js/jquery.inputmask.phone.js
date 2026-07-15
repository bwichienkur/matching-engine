(function ($) {
    $.phonemask = function (controlName) {

        var mask = {

            controlName: controlName,
            controlMask: ['(XXX) XXX-XXXX'],
            isInEditMode: false,
            cleanText: function (control) {//console.log($(control).val());            

                $(control).val($(control).val().replace(/[^0-9]/g, ""));
            },
            onKeydown: function (e, control) {//console.log('onKeydown'); 

                var key = e.charCode || e.keyCode || 0;
                if (key !== 8 && key !== 9)
                    $(control).val(this.maskValue($(control).val()));

                return (key == 8 || key == 9 || key == 46 || (key >= 48 && key <= 57) || (key >= 96 && key <= 105));
            },
            onBlur: function (control) {//console.log('onBlur');               

                this.cleanText(control);
                this.isInEditMode = false;
            },
            onFocus: function (control) {//console.log('onFocus');

                $(control).val(this.maskValue($(control).val()));
                this.isInEditMode = true;
            },
            onMouseover: function (control) {//console.log('onMouseover');

                var controlText = $(control).val();
                $(control).val(controlText.length > 0 ? this.maskValue(controlText) : this.controlMask);
            },
            onMouseout: function (control) {//console.log('onMouseout');            

                if (!this.isInEditMode) this.cleanText(control);
            },
            maskValue: function (text) {

                var controlText = text.replace(/[^0-9]/g, "");
                
                if (controlText.length < 3)
                    return ("(" + controlText.substring(0, 3));
                else if (controlText.length < 6)
                    return ("("
                        + controlText.substring(0, 3) + ") "
                        + controlText.substring(3, 6));
                else
                    return ("("
                        + controlText.substring(0, 3) + ") "
                        + controlText.substring(3, 6) + "-"
                        + controlText.substring(6, controlText.length));

            },
            load: function () {

                var self = this;
                var control = $('#' + controlName);

                self.cleanText(control);
                $(control)
                  .keydown(function (e) { self.onKeydown(e, this); })
                  .bind('focus click', function () { self.onFocus(this); })
                  .blur(function () { self.onBlur(this); })
                  .mouseover(function () { self.onMouseover(this); })
                  .mouseout(function () { self.onMouseout(this); });
            }
        };

        return mask.load();
    };


    $(document).ready(function () {

    });

})(jQuery);