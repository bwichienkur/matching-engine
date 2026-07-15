(function ($) {
    // ClientBase.js (Program templates loader)
    //----------------------

    //dynamic javascript
    function loadJs(filename) {
        var script = document.createElement('script');
        script.type = "text/javascript";
        script.language = "javascript";
        if (FormsEngine.CompressJs) {
            script.src = FormsEngine.ServiceBaseURL + '/Static/GetJs?BasePath=Common&FileName=' + filename + '&CompressJs=true';
        } else {
            script.src = FormsEngine.ServiceBaseURL + '/Templates/Common/js/' + filename + '.js'
        }
        document.body.appendChild(script);
    }

    //Alternative templates helper
    function serializeAlternativeTemplates() {
        var r = [];
        if (FormsEngine.AlternativeTemplates) {
            for (var i = 0; i < FormsEngine.AlternativeTemplates.length; i++) {
                r.push(FormsEngine.AlternativeTemplates[i].TemplateId.toString() + "=" + FormsEngine.AlternativeTemplates[i].AlternativeId.toString());
            }
        }
        return r.join(',');
    }


    function trackEvent(event) {

        var Gevent = event;
        if (FormsEngine.TemplateId != undefined && FormsEngine.TemplateId > 0) {
            Gevent = event + ".TemplateId." + FormsEngine.TemplateId;
        }

        //If Google Tag manager is detected fire same events there else fire regular gaq
        try {
            if (typeof dataLayer != 'undefined') {
                dataLayer.push({
                    'event': 'virtualPageView',
                    'url': Gevent
                });
            }
            else if (typeof _gaq != 'undefined') {
                _gaq.push(['_trackPageview', Gevent]);
            }
        } catch (e) { }

        try {
            if (typeof _etq != 'undefined') {
                _etq.push(['_etEvent', 'workflow', event, 'form-programtemplate']);
            }
        } catch (e) { }

    }

    $(document).ready(function () {

        if (window['FormsEngine'] == undefined
            || FormsEngine == null
            || !FormsEngine.hasOwnProperty('RenderingStrategy')
            || !FormsEngine.hasOwnProperty('RenderingDiv')
            || !(FormsEngine.hasOwnProperty('ThankYouPage') || (FormsEngine.hasOwnProperty('ThankYouApplyNowURL') && FormsEngine.hasOwnProperty('ThankYouRequestInfoURL')))
            || !FormsEngine.hasOwnProperty('IsBeta')
            || !FormsEngine.hasOwnProperty('InstitutionId')
            || !FormsEngine.hasOwnProperty('TrackId')
        ) {
            $(document.body).append("<div>Error: FormsEngine object has to be defined before Client Script is included with the appropriate settings.<br/><br/>"
                + " i.e.<br/> var FormsEngine = FormsEngine || {}; <br/>"
                + " FormsEngine.RenderingStrategy ='[RenderingStrategy]'; <br/>"
                + " FormsEngine.RenderingDiv = '[RenderingDiv]'; <br/>"
                + " FormsEngine.ThankYouPage = '[ThankYouPage]'; <br/>"
                + " FormsEngine.IsBeta = '[IsBeta]'; <br/>"
                + " FormsEngine.InstitutionId ='[InstitutionId]'; <br/>"
                + " FormsEngine.TrackId ='[TrackId]'; <br/>");
            return;
        }
        else if (typeof window.FormsEngineGlobal === "undefined") {
            $(document.body).append("<div>FormsEngine Error: FormsEngine Global javascript must be included first. <br/><be/>"
                + " i.e. <br/> &lt;script type='text/javascript' src='//server/Static/GetGlobal'>&lt;/script> </div>");
            return;
        }

        FormsEngine.ServiceBaseURL = "[SERVICEBASE]";
        FormsEngine.ClientServiceURL = FormsEngine.ServiceBaseURL + '/TemplateManager/GetProgramTemplate';
        FormsEngine.IsBeta = FormsEngine.IsBeta == undefined || FormsEngine.IsBeta === "" ? false : FormsEngine.IsBeta;
        FormsEngine.DebugMode = FormsEngine.DebugMode == undefined || FormsEngine.DebugMode === "" ? false : FormsEngine.DebugMode;
        FormsEngine.TemplateName = FormsEngine.TemplateName == undefined ? "" : FormsEngine.TemplateName;
        FormsEngine.TemplateId = FormsEngine.TemplateId == undefined ? "" : FormsEngine.TemplateId;
        FormsEngine.ProgramProductId = FormsEngine.ProgramProductId == undefined ? "" : FormsEngine.ProgramProductId;
        FormsEngine.CompressJs = false; //FormsEngine.CompressJs == undefined ? false : FormsEngine.CompressJs;
        FormsEngine.Theme = FormsEngine.Theme == undefined || FormsEngine.Theme === "" ? "default" : FormsEngine.Theme;
        FormsEngine.RenderingStrategy = FormsEngine.AlternativeRenderingStrategy == undefined ? FormsEngine.RenderingStrategy : FormsEngine.AlternativeRenderingStrategy;
        FormsEngine.InstitutionName = $('<div />').html(FormsEngine.InstitutionName == undefined ? '' : FormsEngine.InstitutionName).text();
        FormsEngine.TrackEvent = function (event) { trackEvent(event); };


        var serviceArgs = "?RenderingStrategy=" + FormsEngine.RenderingStrategy;
        serviceArgs += "&IsBeta=" + FormsEngine.IsBeta;
        serviceArgs += "&ProgramProductId=" + FormsEngine.ProgramProductId;
        serviceArgs += "&ProgramId=" + FormsEngine.ProgramId;
        serviceArgs += "&TemplateId=" + FormsEngine.TemplateId;
        serviceArgs += "&InstitutionId=" + FormsEngine.InstitutionId;
        serviceArgs += "&AlternativeTemplates=" + serializeAlternativeTemplates();
        serviceArgs += "&IgnoreTemplateCache=" + FormsEngine.IgnoreTemplateCache;
        serviceArgs += "&Theme=" + FormsEngine.Theme;
        serviceArgs += "&ApplicationId=" + FormsEngine.ApplicationId;
        serviceArgs += "&TrackId=" + FormsEngine.TrackId;
        serviceArgs += "&DeviceId=" + FormsEngine.DeviceId;

        var CSSBasePath = FormsEngine.RenderingStrategy == undefined || FormsEngine.RenderingStrategy == "" ? "ORIGINAL" : FormsEngine.RenderingStrategy;
        $('head').append('<link rel="stylesheet" id="BaseCSS" href="' + FormsEngine.ServiceBaseURL + '/Static/GetBundledWizardCSS?basePath=' + CSSBasePath + '&theme=' + FormsEngine.Theme + '&cachebuster=true type="text/css" />');

        if (FormsEngine.ApplicationId == 7) {
            $('#' + FormsEngine.RenderingDiv).addClass('GradSchool');
        }


        $.ajax({
            async: false,
            type: 'GET',
            dataType: 'jsonp',
            cache: false,
            url: FormsEngine.ClientServiceURL + serviceArgs,
            success: function (data) {
                var template = decodeURIComponent((data.Template + '').replace(/\+/g, '%20'))
                $('#' + FormsEngine.RenderingDiv).html(template);
                FormsEngine.TemplateId = data.TemplateId;
                FormsEngine.DefaultTemplateId = data.DefaultTemplateId;
                loadJs("ProgramTemplate");
            },
            error: function (request, error) {
                fe_consolelog(arguments + "\n" + " Error: " + request.responseText);
            }
        });

    });
})(jQuery);
