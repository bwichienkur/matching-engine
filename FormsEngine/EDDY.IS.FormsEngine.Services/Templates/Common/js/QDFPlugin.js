(function ($) {
    $.fn.extend({

        //Eddy QDF  service plugin 
        // v 1.0
        eddyQDF: function (options) {

            //Default values (Jonathan define as you wish default settings when not passed)
            var defaults = {
                placementToken: "00000000-0000-0000-0000-0000-000000000000",
                applicationId: 2,
                trackId: '00000000-0000-0000-0000-0000-000000000000',
                debugMode: true,
                testMode: false,
                serviceUrl: '[SERVICEBASE]',
                targetUrl: 'https://devqs.educationconnection.local', //the url we will route to
                targetLocation: 'tab', //tab or popup
                feTemplateid: 1,
                isBeta: false,
                ignoreTemplateCache: false,
                renderingStrategy: 'QDFPlugin',
                theme: 'default',
                extendedFields: {},
                passThruItems: [
                    //{QuestionName: '', Answers: ''}
                ],
                customcss: "",
                questions: [],
                stringToRecover: "",
                buttonText: "CONTINUE",
                subSource2: "",
                leadInitiatingUrl: "",
                sub_1: ""
            };

            var debugmode = getQuerystringValue("debugmode") === 'true'; // Sample to read from querystring

            if (debugmode) {
                defaults.debugMode = true;
            }

            //Merge options passed/defaults, passed arguments have precedence
            var opt = $.extend(defaults, options);

            // Container selectors
            var selectors = this;

            //Call Main entry point to get the form/template and inject into the passed in div
            pluginMain();
            $('head').append('<link rel="stylesheet" id="BaseCSS" href="' + opt.serviceUrl + '/Static/GetBundledQDFPluginCSS?basePath=' + opt.renderingStrategy + '&theme=' + opt.theme + '" type="text/css" />');


            //Your functions go here
            function getQuestions(formObj) {
                var questionArr = [];
                //loop through every question on the form with the name we left for questions on the form
                jQuery.each($(formObj).find("form[name='eddynexusform-qdfplugin']").serializeArray(), function (index, item) {
                    //using the name find the object
                    log(item.name);

                    var theInput = $(formObj).find('[name="' + item.name + '"]');
                    //make a question object similar to the one done by the controller
                    var question = {
                        Code: $(theInput).attr('code'),
                        TemplateControlId: $(theInput).attr('data-id'),
                        Step: 1,
                        Filters: $(theInput).attr('data-filters') ? $(theInput).attr('data-filters').split(',') : [],
                        DataBind: window['qpd_' + $(theInput).attr('data-jsDataBind')],
                        LastDataBindFilters: 'none',
                        Initialized: false
                    };

                    questionArr.push(question);
                });
                return questionArr;
            }

            // Shared functions
            function log(value, printObject) {

                if (opt.debugMode === true && value !== null && typeof console === "object") {
                    if (typeof console.dir === "function" && printObject === true) {
                        console.dir(value);
                    }
                    else if (typeof console.log === "function") {
                        console.log("eddyQDF:" + value);
                    }
                }
            }

            function getQuerystringValue(name) {
                match = new RegExp('[?&]' + name + '=([^&]*)', 'i').exec(window.location.search);
                return match ? decodeURIComponent(match[1].replace(/\+/g, ' ')).toLowerCase() : '';
            }

            //Detect if given guid is valid
            function isguid(str) {
                var rgex = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/;
                return rgex.test(str);
            }

            //End of Shared functions

            //Plugin Main logic
            function pluginMain() {

                // Required arguments checkup
                if (!selectors.length) {
                    log("Can't display qdf on empty selector.");
                    return;
                }

                //Container Selectors
                return selectors.each(function () {
                    var obj = $(this);

                    var serviceArgs;
                    serviceArgs = "?RenderingStrategy=" + opt.renderingStrategy;
                    serviceArgs += "&IsBeta=" + opt.isBeta;
                    serviceArgs += "&TemplateId=" + opt.templateId;
                    serviceArgs += "&IgnoreTemplateCache=" + opt.ignoreTemplateCache;
                    serviceArgs += "&Theme=" + opt.theme;
                    serviceArgs += "&TrackId=" + opt.trackId;

                    log(serviceArgs, true);

                    $.ajax({
                        async: true,
                        type: 'GET',
                        dataType: 'jsonp',
                        cache: false,
                        url: opt.serviceUrl + '/TemplateManager/GetQDFTemplate' + serviceArgs,
                        success: function (data) {
                            log(data, true);
                            //inject html in form
                            renderedHtml = decodeURIComponent((data.Template + '').replace(/\+/g, '%20'));
                            $(obj).html(renderedHtml);

                            //bind click event to submit button for this plugin
                            $(obj).find("span[data-id='form-submit-button-label']").html(opt.buttonText);
                            $(obj).find("div[data-id='qdfplugin-form-submit-button']").click(function () {
                                if ($(obj).find('form').valid()) {
                                    var args = "Trackid=" + opt.trackId;
                                    if (opt.subSource2 && opt.subSource2 != "") {
                                        args += "&SubSource2=" + encodeURIComponent(opt.subSource2);
                                    }
                                    if (opt.leadInitiatingUrl && opt.leadInitiatingUrl != "") {
                                        args += "&LeadSourceUrl=" + encodeURIComponent(opt.leadInitiatingUrl);
                                    }
                                    if (opt.sub_1 && opt.sub_1 != "") {
                                        args += "&aid=" + encodeURIComponent(opt.sub_1);
                                    }
                                    if (opt.widgetName != null) {
                                        args += "&WidgetName=" + encodeURIComponent(opt.widgetName);
                                    }
                                    if (opt.widgetRequestGuid != null) {
                                        args += "&WidgetRequestGuid=" + opt.widgetRequestGuid;
                                    }
                                    jQuery.each(opt.questions, function (index, item) {
                                        var controlValue = fep_getControlValue(obj, opt, item.Code);
                                        if (controlValue && controlValue.value != "") {
                                            args += '&' + encodeURIComponent(item.Code) + '=' + encodeURIComponent(controlValue.value);
                                        }
                                        if (controlValue && controlValue.value != "" && controlValue.valueKey != "") {
                                            args += '&' + encodeURIComponent(item.Code) + '_Name=' + encodeURIComponent(controlValue.valueKey);
                                        }
                                    });
                                    log(args, true);
                                    switch (opt.targetLocation) {
                                        case "tab":
                                            window.open(opt.targetUrl + '?' + args);
                                            break;
                                        default:
                                            window.open(opt.targetUrl + '?' + args);
                                            break;
                                    }
                                }
                            });

                            opt.questions = getQuestions(obj);
                            //bind dynamic controls
                            qpd_bindDynamicPluginControls(obj, opt);
                        }
                        ,
                        error: function (request, textStatus, errorThrown) {
                            log(arguments + "\n" + " Error: ");
                            log(request, true);
                        }
                    });

                });
            }

        }
    });


    jQuery(document).ready(function () {
        jQuery('head').append('<link rel="stylesheet" id="PlainCSS" href="' + '[SERVICEBASE]' + '/Static/GetCommonCSS?fileName=PlainPlugin" type="text/css" />');
    });

})(jQuery);
