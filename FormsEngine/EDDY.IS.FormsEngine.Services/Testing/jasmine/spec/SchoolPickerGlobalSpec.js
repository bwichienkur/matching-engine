var additionalQuestionsSection;
var form;
var formStep;
var stepNumber = 6;
var disabledSubmitButtonClassOverlay = "wizard-form-disabled-submit-button-overlay";

var buttonDefinitions = [
    {
        value: "Yes",
        key: "22",
        class: "yes"
    },
    {
        value: "No",
        key: "23",
        class: "no"
    }
];

var radioControlElements = [];
var dropdownControlElements = [];

var radioControlDefinitions = [
    {
        fieldHolderClass: "Undergraduate_Degree_Grad",
        controlCode: "Undergraduate_Degree_Grad",
        name: "Do_you_have_an_undergraduate_degree_in_the_fields_you_have_selected",
        buttons: buttonDefinitions
    },
    {
        fieldHolderClass: "RN_license",
        controlCode: "RN_license",
        name: "Do_you_have_an_rn_license",
        buttons: buttonDefinitions
    },
    {
        fieldHolderClass: "Undergraduate_Degree_Nursing",
        controlCode: "Undergraduate_Degree_Nursing",
        name: "Undergraduate_Degree_Nursing",
        buttons: buttonDefinitions
    }
];

var dropdownControlDefinitions = [
    {
        fieldHolderClass: "GPA",
        controlCode: "GPA",
        options: [
            {
                value: "Less than 2.0",
                key: "7"
            },
            {
                value: "3.0-3.4",
                key: "57"
            },
            {
                value: "3.5 or Higher",
                key: "11"
            }
        ]
    },
    {
        controlCode: "Years_of_Work_Experience",
        options: [
            {
                value: "None",
                key: "17"
            },
            {
                value: "1 Year",
                key: "18"
            },
            {
                value: "2-4 Years",
                key: "19"
            }
        ]
    }
];

function buildSectionElement() {
    var sectionString = "<fieldset name='section' class='sections' id='Section1'>";
    sectionString += "<div id='DynamicQuestions' data-title='DynamicQuestions' data-step='6'>";
    sectionString += "<fieldset id='Section1' class='sections' name='section'></fieldset>";
    sectionString += "</div>";
    sectionString += "</fieldset>";

    additionalQuestionsSection = $(sectionString);
}

function buildRadioButtonElements() {
    radioControlElements = [];

    for (var i = 0; i < radioControlDefinitions.length; i++) {
        var radioElement = radioControlDefinitions[i];

        var htmlString = "<div class='field-holder form-group " + radioElement.fieldHolderClass + "' data-rowsequence='3' data-controlcode='" + radioElement.controlCode + "' data-controltypename='Radio Buttons'>";
        htmlString += "<fieldset>";

        for (var x = 0; x < radioElement.buttons.length; x++) {
            var button = radioElement.buttons[x];
            htmlString += "<div class='radio-inline'><input type='radio' controltypename='Radio Buttons' value='" + button.value + "' key='" + button.key + "' code='" + radioElement.controlCode + "' name='" + radioElement.name + "' class='radio-field " + button.class + "' required='required' me-filter='true'></div>";
        }

        htmlString += "</fieldset>";
        htmlString += "</div>";

        var newElement = $(htmlString);
        additionalQuestionsSection.find("#Section1").append(newElement);
        radioControlElements.push(newElement);
    }
}

function buildDropDownElements() {
    for (var i = 0; i < dropdownControlDefinitions.length; i++) {

        var dropdownElement = dropdownControlDefinitions[i];

        var htmlString = "<div class='field-holder form-group " + dropdownElement.controlCode + "' data-rowsequence='7' data-controlcode='" + dropdownElement.controlCode + "' data-controltypename='Drop-Down'>";
        htmlString += "<select controltypename='Drop-Down' code='" + dropdownElement.controlCode + "' name='" + dropdownElement.controlCode + "' class='select-field form-control' required='required' me-filter='true'>";

        htmlString += "<option value=''>-- SELECT --</option>";

        for (var x = 0; x < dropdownElement.options.length; x++) {
            var option = dropdownElement.options[x];
            htmlString += "<option value='" + option.value + "' key='" + option.key + "'>" + option.value + "</option>";
        }

        htmlString += "</select>";
        htmlString += "</div>";

        var newElement = $(htmlString);
        additionalQuestionsSection.find("#Section1").append(newElement);
        dropdownControlElements.push(newElement);
    }
}

function buildFormStep() {
    buildSectionElement();
    buildRadioButtonElements();
    buildDropDownElements();
    formStep = $("<div id='Step6'></div>");
    formStep.append(additionalQuestionsSection);
}

function getSubmitButton() {
    var buttonId = "wizard-form-submit-button";
    FormsEngine.SubmitButton = "#" + buttonId;

    var button = $("<div>Submit</div>");
    button.attr("id", buttonId);

    return button;
}

function buildForm() {
    buildFormStep();
    form = $("<form id='form' role='form' name='form'></form>");
    form.append(formStep);

    var submitButton = getSubmitButton();
    form.append(submitButton);
}

function addFormToDom() {
    var formContainer = $("<div class='eddy-form-wizard-container'></div>");
    formContainer.append(form);
    $(document.body).append(formContainer);
    FormsEngine.DefaultFormTag = "#" + form.attr("id");
    $(FormsEngine.DefaultFormTag).validate();
}

function buildFormAndAddToDom() {
    buildForm();
    addFormToDom();
}

function tearDownFormElement() {
    form.remove();
    form = null;
    additionalQuestionsSection = null;
    formStep = null;
    radioControlElements = [];
    dropdownControlElements = [];
}

describe("schoolPicker_GlobalFunctions.js functions", function () {

    beforeEach(function () {
        FormsEngine.SchoolPickerSelections = {};
        buildFormAndAddToDom();
    });

    afterEach(function () {
        tearDownFormElement();
    });

    describe("fe_sp_createSchoolPicker", function () {

        var component = '<div class="institution-carousel-slide"><button class="school-picker-selection-btn">select</button></div>';
        var response;

        beforeEach(function () {
            var carouselHtml = "<div><div id='school-picker-carousel-message'></div><div id='school-picker-carousel'></div><div id='school-picker-carousel-indicators'></div></div>";
            $(FormsEngine.DefaultFormTag).append(carouselHtml);

            response = {
                components: [],
                metaDataMessages: {
                    "SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE": "Scroll through selections, pick up to {maxsubmissioncount} schools that meet your needs.",
                    "SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE": "You can only select up to {maxsubmissioncount} schools."
                }
            };
        });

        afterEach(function () {
            $(FormsEngine.DefaultFormTag + " #school-picker-carousel").html("");
        });

        it("adds match components to school picker carousel", function () {

            // run multiple times while increasing the amount of components
            for (var x = 0; x < 10; x++) {
                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var slides = $(FormsEngine.DefaultFormTag + " #school-picker-carousel").find(".institution-carousel-slide");

                expect(slides.length).toEqual(response.components.length);
            }
        });

        it("adds slide indicators to school picker carousel", function () {
            
            // run multiple times while increasing the amount of components
            for (var x = 0; x < 10; x++) {
                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var indicatorCount = $(FormsEngine.DefaultFormTag + " #school-picker-carousel-indicators .dot").length;
                expect(indicatorCount).toEqual(response.components.length);
            }

        });

        it("adds click event handlers to school picker carousel buttons", function () {

            // run multiple times while increasing the amount of components
            for (var x = 0; x < 10; x++) {

                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var handlerCount = 0;
                var buttons = $(FormsEngine.DefaultFormTag + " #school-picker-carousel").find("button");

                for (var i = 0; i < buttons.length; i++) {
                    var events = $._data(buttons[i], "events");

                    if (!events) {
                        fail("No events bound to button");
                        break;
                    }

                    var clickEvents = events.click;
                    for (var c = 0; c < clickEvents.length; c++) {
                        handlerCount++;
                    }
                }

                expect(handlerCount).not.toEqual(0);
                expect(handlerCount).toEqual(buttons.length);
            }

        });

        it("adds change event handlers to school picker carousel slides", function () {

            // run multiple times while increasing the amount of components
            for (var x = 0; x < 10; x++) {

                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var handlerCount = 0;
                var slides = $(FormsEngine.DefaultFormTag + " #school-picker-carousel").find(".institution-carousel-slide");

                for (var i = 0; i < slides.length; i++) {
                    var events = $._data(slides[i], "events");

                    if (!events) {
                        fail("No events bound to slide");
                        break;
                    }

                    var changeEvents = events.change;
                    for (var c = 0; c < changeEvents.length; c++) {
                        handlerCount++;
                    }
                }

                expect(handlerCount).not.toEqual(0);
                expect(handlerCount).toEqual(slides.length);
            }

        });

        it("calls method to enable submit button", function () {
            var enableSubmitButtonSpy = spyOn(window, "fe_sp_enableSubmitButton");
            response.components.push(component);

            fe_sp_createSchoolPicker(response);

            expect(enableSubmitButtonSpy).toHaveBeenCalled();
        });

        it("removes loading spinner once school picker is loaded", function () {

            var component = $("<div></div>");
            $(FormsEngine.DefaultFormTag).append(component);
            fe_sp_addLoadingSpinnerToComponent(component);

            // run multiple times while increasing the amount of components
            for (var x = 0; x < 10; x++) {

                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var formDoesntContainsLoadingSpinner = $(FormsEngine.DefaultFormTag).find("#component-loader-overlay").length < 1 && $(FormsEngine.DefaultFormTag).find("#component-loader").length < 1;
                expect(formDoesntContainsLoadingSpinner).toBe(true);
            }
        });

        it("sets FormsEngine.SchoolPickerCarouselLoaded to true when there are components", function () {
            FormsEngine.SchoolPickerCarouselLoaded = false;
            response.components.push(component);

            fe_sp_createSchoolPicker(response);
            
            expect(FormsEngine.SchoolPickerCarouselLoaded).toEqual(true);
        });

        it("sets FormsEngine.SchoolPickerCarouselLoaded to false when there arent components", function () {
            FormsEngine.SchoolPickerCarouselLoaded = true;
            
            fe_sp_createSchoolPicker(response);

            expect(FormsEngine.SchoolPickerCarouselLoaded).toEqual(false);
        });

        it("selects the first school in the carousel if FormsEngine.SelectFirstSchoolInCarousel is true", function () {

            FormsEngine.SelectFirstSchoolInCarousel = true;

            for (var x = 0; x < 10; x++) {
                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var firstButtonSelectedAttr = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-btn").first().attr("data-selected");

                expect(firstButtonSelectedAttr).not.toEqual(undefined);
            }
        });

        it("doesnt select the first school in the carousel if FormsEngine.SelectFirstSchoolInCarousel is false", function () {
            FormsEngine.SelectFirstSchoolInCarousel = false;

            for (var x = 0; x < 10; x++) {
                for (var j = 0; j <= x; j++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var firstButtonSelectedAttr = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-btn").first().attr("data-selected");

                expect(firstButtonSelectedAttr).toEqual(undefined);
            }
        });

        it("clears any existing selections", function () {
            FormsEngine.SchoolPickerSelections = {
                1: {
                    institutionId: 1
                }
            };

            fe_sp_createSchoolPicker(response);

            expect(FormsEngine.SchoolPickerSelections).toEqual({});
        });

        it("sets the match response guid", function () {
            response.matchResponseGuid = "b5f1b2ea-73ef-4a2b-9f4e-11da3fee8ac3";

            fe_sp_createSchoolPicker(response);

            expect(FormsEngine.MatchResponseGuid).toEqual(response.matchResponseGuid);
        });

        it("redirects to the no match page when components in the response has length of 0", function () {
            var goToNoMatchPageSpy = spyOn(window, "fe_sp_goToNoMatchPage");

            fe_sp_createSchoolPicker(response);

            expect(goToNoMatchPageSpy).toHaveBeenCalled();
        });

        it("sets the FormsEngine.SchoolPickerCarouselCount to 0  when components in the response has length of 0", function () {
            FormsEngine.SchoolPickerCarouselCount = null;

            fe_sp_createSchoolPicker(response);

            expect(FormsEngine.SchoolPickerCarouselCount).toEqual(0);
        });

        describe("components array is null", function () {

            beforeEach(function () {
                response.components = null;
            });

            it("redirects to the no match page when components in the response is null", function () {
                var goToNoMatchPageSpy = spyOn(window, "fe_sp_goToNoMatchPage");

                fe_sp_createSchoolPicker(response);

                expect(goToNoMatchPageSpy).toHaveBeenCalled();
            });

            it("sets the FormsEngine.SchoolPickerCarouselCount to 0 when components in the response is null", function () {
                FormsEngine.SchoolPickerCarouselCount = null;

                fe_sp_createSchoolPicker(response);

                expect(FormsEngine.SchoolPickerCarouselCount).toEqual(0);
            });
        });

        describe("response is null", function () {

            beforeEach(function () {
                response = null;
            });

            it("redirects to the no match page when response is null", function () {
                var goToNoMatchPageSpy = spyOn(window, "fe_sp_goToNoMatchPage");

                fe_sp_createSchoolPicker(response);

                expect(goToNoMatchPageSpy).toHaveBeenCalled();
            });

            it("sets the FormsEngine.SchoolPickerCarouselCount to 0 when the response is null", function () {
                FormsEngine.SchoolPickerCarouselCount = null;

                fe_sp_createSchoolPicker(response);

                expect(FormsEngine.SchoolPickerCarouselCount).toEqual(0);
            });

        });

        describe("school picker carousel message tests", function () {

            beforeEach(function () {
                FormsEngine.SchoolPickerCarouselMessages = {};
                FormsEngine.CampaignDetail = {
                    MaxSubmissionCount: 5
                };
            });

            it("adds school picker carousel meta data messages to global session object", function () {

                for (var i = 0; i < 5; i++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var expectedResult = {
                    "SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE": "Scroll through selections, pick up to five schools that meet your needs.",
                    "SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE": "You can only select up to five schools."
                };

                expect(FormsEngine.SchoolPickerCarouselMessages).toEqual(expectedResult);
            });

            it("sets the default message on the school picker carousel; uses MaxSubmissionCount", function () {
                
                for (var i = 0; i < 5; i++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var carouselMessageText = $(FormsEngine.DefaultFormTag + " #school-picker-carousel-message").text();
                expect(carouselMessageText).toEqual("Scroll through selections, pick up to five schools that meet your needs.");
            });

            it("sets the default message on the school picker carousel; uses SchoolPickerCarouselCount", function () {

                for (var i = 0; i < 3; i++) {
                    response.components.push(component);
                }

                fe_sp_createSchoolPicker(response);

                var carouselMessageText = $(FormsEngine.DefaultFormTag + " #school-picker-carousel-message").text();
                expect(carouselMessageText).toEqual("Scroll through selections, pick up to three schools that meet your needs.");
            });
        });

    });

    describe("fe_sp_getSchoolPickerCarouselMessage", function () {
        FormsEngine.SchoolPickerCarouselMessages = {
            "SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE": "Scroll through selections, pick up to five schools that meet your needs.",
            "SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE": "You can only select up to five schools."
        };

        it("returns the limit reached message", function () {
            var message = fe_sp_getSchoolPickerCarouselMessage(fe_sp_schoolPickerCarouselMessageType.limitReached);
            expect(message).toEqual(FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE"]);
        });

        it("returns the default message", function () {
            var message = fe_sp_getSchoolPickerCarouselMessage(fe_sp_schoolPickerCarouselMessageType.default);
            expect(message).toEqual(FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE"]);
        });
    });

    describe("fe_sp_institutionSchoolPickerComponentChange", function () {

        var slideHtml = "<div class='mySlides row institution-carousel-slide text-center' data-institutionid='4462'>\
                            <div>\
                                <button type='button' class='btn btn-default school-picker-selection-btn' data-institutionid='4462'>Click if Interested</button>\
                            </div>\
                            <div>\
                                <select class='form-control valid school-picker-selection-dropdown' data-institutionid='4462'>\
                                        <option value='244982' data-programid='244982' data-programproductid='626396' data-programtemplateid='2' data-institutionname='University of Miami'>Master of Arts in International Administration (MAIA)</option>\
                                        <option value='243996' data-programid='243996' data-programproductid='359259' data-programtemplateid='2' data-institutionname='University of Miami'>Master of Arts in Liberal Studies (MALS)</option>\
                                </select>\
                            </div>\
                        </div>";

        var slide;
        var button;


        beforeEach(function () {
            slide = $(slideHtml);
            button = $(slide).find("button[data-institutionid='4462']").first();
        });

        describe("tests that add selections", function () {

            var expectedSelectionResult = {
                4462: {
                    institutionId: 4462,
                    institutionName: "University of Miami",
                    programId: 244982,
                    programProductId: 626396,
                    programTemplateId: 2
                }
            };

            beforeEach(function () {
                FormsEngine.SchoolPickerSelections = {};
            });
            
            it("adds the school to selections and selects the button when the program dropdown triggers the change event", function () {
                var selectControl = $(slide).find("select[data-institutionid='4462']").first();

                fe_sp_institutionSchoolPickerComponentChange(selectControl);

                var buttonSelectedAttribute = button.attr("data-selected");
                var buttonIsSelected = buttonSelectedAttribute !== null && buttonSelectedAttribute !== undefined;

                expect(buttonIsSelected).toEqual(true);
                expect(FormsEngine.SchoolPickerSelections).toEqual(expectedSelectionResult);
            });

            it("adds the school to selections when the button is selected and triggers the change event", function () {
                // select button
                button.attr("data-selected", "");

                fe_sp_institutionSchoolPickerComponentChange(button);

                expect(FormsEngine.SchoolPickerSelections).toEqual(expectedSelectionResult);
            });


            it("enables the submit button where there are selections", function () {

                var enableSubmitButtonSpy = spyOn(window, "fe_sp_enableSubmitButton");

                // select button
                button.attr("data-selected", "");

                fe_sp_institutionSchoolPickerComponentChange(button);

                expect(enableSubmitButtonSpy).toHaveBeenCalled();
            });
        });


        describe("tests that remove selections", function () {

            
            beforeEach(function () {
                FormsEngine.SchoolPickerSelections = {
                    4462: {
                        institutionId: 4462,
                        institutionName: "University of Miami",
                        programId: 244982,
                        programProductId: 626396,
                        programTemplateId: 2
                    }
                };
            });

            it("removes the school from selections when the button triggers the change event and isnt selected", function () {
                fe_sp_institutionSchoolPickerComponentChange(button);
                expect(FormsEngine.SchoolPickerSelections).toEqual({});
            });


            it("enables the submit button when there is no selections", function () {
                var enableSubmitButtonSpy = spyOn(window, "fe_sp_enableSubmitButton");

                fe_sp_institutionSchoolPickerComponentChange(button);

                expect(enableSubmitButtonSpy).toHaveBeenCalled();
            });
        });
    });

    describe("fe_sp_updateSchoolPickerSelections", function () {

        var elementTagName;
        var slideHtml = '<div class="institution-carousel-slide"></div>';
        var buttonHtml = '<button class="school-picker-selection-btn">select</button>';
        var dropdownHtml = "<select class='form-control school-picker-selection-dropdown'></select>";
        var carouselHtml = "<div><div id='school-picker-carousel-message'></div><div id='school-picker-carousel'></div><div id='school-picker-carousel-indicators'></div></div>";

        var slide;
        var button;
        var dropdown;
        var slideToPassAsParam;

        var institutionId = 1;

        describe("TCPA message tests", function () {

            var institutionIdAttribute;
            var institutionNameAttribute;
            var programIdAttribute;
            var programProductIdAttribute;
            var programTemplateIdAttribute;

            beforeEach(function () {
                elementTagName = "button";

                institutionIdAttribute = 3;
                institutionNameAttribute = "institutionThree";
                programIdAttribute = 30;
                programProductIdAttribute = 300;
                programTemplateIdAttribute = 3000;

                slideToPassAsParam = $(slideHtml);

                button = $(buttonHtml);
                button.attr("data-institutionid", institutionIdAttribute);
                button.attr("data-institutionname", institutionNameAttribute);
                slideToPassAsParam.append(button);

                var option = $("<option></option>");
                option.attr("data-institutionid", institutionIdAttribute);
                option.attr("data-institutionname", institutionNameAttribute);
                option.attr("data-programid", programIdAttribute);
                option.attr("data-programproductid", programProductIdAttribute);
                option.attr("data-programtemplateid", programTemplateIdAttribute);

                dropdown = $(dropdownHtml);
                dropdown.attr("data-institutionid", institutionIdAttribute);
                dropdown.append(option);
                slideToPassAsParam.append(dropdown);

                $(FormsEngine.DefaultFormTag + " #school-picker-carousel").append(slideToPassAsParam);

                FormsEngine.SchoolPickerSelections = {
                    1: {
                        institutionId: 1,
                        institutionName: "institutionOne"
                    },
                    2: {
                        institutionId: 2,
                        institutionName: "institutionTwo"
                    }
                };
            });


            it("sets the school names property for the TCPA message when a school is added", function () {
                // select button so selection is added
                button.attr("data-selected", "");

                fe_sp_updateSchoolPickerSelections(institutionIdAttribute, slideToPassAsParam, elementTagName);

                expect(FormsEngine.SmartMatchSchoolNames).toEqual("institutionOne, institutionTwo, institutionThree, ");
            });

            it("sets the school names property for the TCPA message when a school is removed", function () {

                FormsEngine.SchoolPickerSelections[3] = {
                    institutionId: 3,
                    institutionName: "institutionThree"
                };

                fe_sp_updateSchoolPickerSelections(institutionIdAttribute, slideToPassAsParam, elementTagName);

                expect(FormsEngine.SmartMatchSchoolNames).toEqual("institutionOne, institutionTwo, ");
            });

            it("sets the school names property to an empty string for the TCPA message when no selections", function () {

                FormsEngine.SchoolPickerSelections = null;

                fe_sp_updateSchoolPickerSelections(institutionIdAttribute, slideToPassAsParam, elementTagName);

                expect(FormsEngine.SmartMatchSchoolNames).toEqual("");
            });
        });


        describe("max submission count tests", function () {

            var totalSlides = 10;
            var selectedSlides = unselectedSlides = totalSlides / 2;

            function addSelectedSlidesToCarousel() {
                for (var x = 1; x <= selectedSlides; x++) {
                    slide = $(slideHtml);

                    button = $(buttonHtml);
                    button.attr("data-selected", "");
                    button.attr("data-institutionid", x);
                    slide.append(button);

                    dropdown = $(dropdownHtml);
                    dropdown.attr("data-institutionid", x);
                    slide.append(dropdown);

                    $(FormsEngine.DefaultFormTag + " #school-picker-carousel").append(slide);
                }
            }

            beforeEach(function () {
                elementTagName = "select";

                FormsEngine.SchoolPickerSelections = {
                    1000: { institutionId: 1000 },
                    2000: { institutionId: 2000 },
                    3000: { institutionId: 3000 },
                    4000: { institutionId: 4000 },
                    5000: { institutionId: 5000 }
                };

                FormsEngine.SchoolPickerCarouselMessages = {
                    "SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE": "Scroll through selections, pick up to five schools that meet your needs.",
                    "SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE": "You can only select up to five schools."
                };

                var carousel = $(carouselHtml);
                $(FormsEngine.DefaultFormTag).append(carousel);

                addSelectedSlidesToCarousel();

                slideToPassAsParam = $(FormsEngine.DefaultFormTag).find(".institution-carousel-slide").first();
            });

            describe("enabling the selection controls", function () {

                function addDisabledSlidesToCarousel() {
                    for (var i = 1; i <= unselectedSlides; i++) {
                        var institutionId = i + selectedSlides;

                        slide = $(slideHtml);

                        button = $(buttonHtml);
                        button.attr("disabled", "");
                        button.attr("data-institutionid", institutionId);
                        slide.append(button);

                        dropdown = $(dropdownHtml);
                        dropdown.attr("disabled", "");
                        dropdown.attr("data-institutionid", institutionId);
                        slide.append(dropdown);

                        $(FormsEngine.DefaultFormTag + " #school-picker-carousel").append(slide);
                    }
                }

                beforeEach(function () {
                    FormsEngine.CampaignDetail = {
                        MaxSubmissionCount: 6
                    };

                    addDisabledSlidesToCarousel();
                });


                it("enables all unselected school picker selection buttons when the max submission count has not been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var numberOfDisabledButtons = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-btn:disabled").length;
                    expect(numberOfDisabledButtons).toEqual(0);
                });

                it("enables all unselected school picker selection dropdowns when the max submission count has not been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var numberOfDisabledDropdowns = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-dropdown:disabled").length;
                    expect(numberOfDisabledDropdowns).toEqual(0);
                });

                it("sets carousel message to be the default message when the max submission count has not been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var carouselMessageText = $(FormsEngine.DefaultFormTag + " #school-picker-carousel-message").text();

                    expect(carouselMessageText).toEqual(FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.CAROUSEL.INSTRUCTIONAL.MESSAGE"]);
                });
            });

            describe("disabling the selection controls", function () {

                function addUnselectedSlidesToCarousel() {
                    for (var i = 0; i < unselectedSlides; i++) {
                        var institutionId = i + selectedSlides;

                        slide = $(slideHtml);

                        button = $(buttonHtml);
                        button.attr("data-institutionid", institutionId);
                        slide.append(button);

                        dropdown = $(dropdownHtml);
                        dropdown.attr("data-institutionid", institutionId);
                        slide.append(dropdown);

                        $(FormsEngine.DefaultFormTag + " #school-picker-carousel").append(slide);
                    }
                }

                beforeEach(function () {
                    FormsEngine.CampaignDetail = {
                        MaxSubmissionCount: 5
                    };

                    addUnselectedSlidesToCarousel();
                });

                it("disables all unselected school picker selection buttons when the max submission count has been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var numberOfDisabledButtons = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-btn:disabled").length;
                    expect(numberOfDisabledButtons).toEqual(unselectedSlides);
                });

                it("disables all unselected school picker selection dropdowns when the max submission count has been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var numberOfDisabledDropdowns = $(FormsEngine.DefaultFormTag).find(".school-picker-selection-dropdown:disabled").length;
                    expect(numberOfDisabledDropdowns).toEqual(unselectedSlides);
                });

                it("sets carousel message to be the submission limit reached message when the max submission count has been met", function () {
                    fe_sp_updateSchoolPickerSelections(institutionId, slideToPassAsParam, elementTagName);

                    var carouselMessageText = $(FormsEngine.DefaultFormTag + " #school-picker-carousel-message").text();

                    expect(carouselMessageText).toEqual(FormsEngine.SchoolPickerCarouselMessages["SCHOOLPICKERWIZARD.MAX.SELECTIONS.MESSAGE"]);
                });
            });
        });
    });

    describe("fe_sp_getUniqueProgramTemplateIdsFromSchoolPickerSelections", function () {

        it("gets a list of unique ProgramTemplateIds", function () {

            FormsEngine.SchoolPickerSelections = {
                1: {
                    programTemplateId: 2
                },
                2: {
                    programTemplateId: 2
                },
                3: {
                    programTemplateId: 3
                },
                4: {
                    programTemplateId: 3
                },
                5: {
                    programTemplateId: 4
                }
            };

            var expectedResult = [2, 3, 4];

            var programTemplateIds = fe_sp_getUniqueProgramTemplateIdsFromSchoolPickerSelections();
            
            expect(programTemplateIds).toEqual(expectedResult);
        });

    });

    describe("fe_sp_validateStepWithoutErrorMessages", function () {

        describe("test for non required fields", function () {

            beforeEach(function () {
                jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find(":input").attr("required", false);
            });

            it("step is valid with no fields complete when fields arent required", function () {
                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });

            it("step is valid with one field complete when fields arent required", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });

            it("step is valid with two fields complete when fields arent required", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });

            it("step is valid with three fields complete when fields arent required", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");
                dropdownControlElements[0].find("option[value='3.5 or Higher']").attr("selected", true);

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });

            it("step is valid with four fields complete when fields arent required", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");
                radioControlElements[2].find(".yes").attr("checked", "checked");
                dropdownControlElements[0].find("option[value='3.5 or Higher']").attr("selected", true);

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });
        });

        describe("tests for required fields", function () {
            it("step is invalid when step only contains radio buttons that arent selected", function () {
                // remove all fields from step and add only unselected radio buttons
                jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find(":input").remove();
                buildRadioButtonElements();

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);

                expect(isValid).toEqual(false);
            });

            it("step is invalid with no fields complete", function () {
                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(false);
            });

            it("step is invalid with one field complete", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(false);
            });

            it("step is invalid with two fields complete", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(false);
            });

            it("step is invalid with three fields complete", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");
                dropdownControlElements[0].find("option[value='3.5 or Higher']").attr("selected", true);

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(false);
            });

            it("step is invalid with four fields complete", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");
                radioControlElements[2].find(".yes").attr("checked", "checked");
                dropdownControlElements[0].find("option[value='3.5 or Higher']").attr("selected", true);

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(false);
            });

            it("step is valid with all fields complete", function () {

                radioControlElements[0].find(".yes").attr("checked", "checked");
                radioControlElements[1].find(".yes").attr("checked", "checked");
                radioControlElements[2].find(".yes").attr("checked", "checked");
                dropdownControlElements[0].find("option[value='3.5 or Higher']").attr("selected", true);
                dropdownControlElements[1].find("option[value='None']").attr("selected", true);

                var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);
                expect(isValid).toEqual(true);
            });


            it("returns false if no stepNumber parameter is passed", function () {
                var isValid = fe_sp_validateStepWithoutErrorMessages(null);
                expect(isValid).toEqual(false);
            });

            describe("checkbox field tests", function () {

                beforeEach(function () {
                    jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find(":input").remove();

                    var html = "<div> \
                                <input type='checkbox' required='required' step='1' controltypename='Multi Check Box List' value='2' key='2' code='Desired_Degree_Level' id='92032_2' id-sort='7' name='Desired_Degree_Level_ST_CB' class='checkbox-field' label-name='Desired Degree Level'>\
                                <input type='checkbox' required='required' step='1' controltypename='Multi Check Box List' value='3' key='3' code='Desired_Degree_Level' id='92032_3' id-sort='7' name='Desired_Degree_Level_ST_CB' class='checkbox-field' label-name='Desired Degree Level'>\
                                <input type='checkbox' required='required' step='1' controltypename='Multi Check Box List' value='8' key='8' code='Desired_Degree_Level' id='92032_8' id-sort='7' name='Desired_Degree_Level_ST_CB' class='checkbox-field' label-name='Desired Degree Level'>\
                            </div>";

                    jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).html(html);
                });

                it("step is invalid when it only contains checkboxes that arent selected", function () {
                    var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);

                    expect(isValid).toEqual(false);
                });

                it("step is valid when it only contains checkboxes that are all selected", function () {
                    jQuery(FormsEngine.DefaultFormTag + " #Step" + stepNumber).find(":input[name='Desired_Degree_Level_ST_CB']").attr("checked", "checked");

                    var isValid = fe_sp_validateStepWithoutErrorMessages(stepNumber);

                    expect(isValid).toEqual(true);
                });

            });
        });
    });

    describe("fe_sp_initializeMatchReplacementEventListeners", function () {

        var bindChangeEventsToAdditionalQuestionsSpy;
        var loadFailedMatchReplacementsWhenAdditionalQuestionsCompleteSpy;

        beforeEach(function () {
            bindChangeEventsToAdditionalQuestionsSpy = spyOn(window, "fe_sp_bindChangeEventsToAdditionalQuestions");
            loadFailedMatchReplacementsWhenAdditionalQuestionsCompleteSpy = spyOn(window, "fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete");

            FormsEngine.StepDynamicQuestions = 5;
        });

        it("calls binding method and replacement retrieval method when current step is the additional questions step and direction is forward", function () {
            var stepNumber = 5;
            var direction = 1;

            fe_sp_initializeMatchReplacementEventListeners(stepNumber, direction);

            expect(bindChangeEventsToAdditionalQuestionsSpy).toHaveBeenCalled();
            expect(loadFailedMatchReplacementsWhenAdditionalQuestionsCompleteSpy).toHaveBeenCalledWith(false);
        });

        it("calls binding method only when current step is the additional questions step and direction is backward", function () {
            var stepNumber = 5;
            var direction = -1;
            fe_sp_initializeMatchReplacementEventListeners(stepNumber, direction);

            expect(bindChangeEventsToAdditionalQuestionsSpy).toHaveBeenCalled();
            expect(loadFailedMatchReplacementsWhenAdditionalQuestionsCompleteSpy).toHaveBeenCalledTimes(0);
        });

        it("doesnt call binding method or replacement retrieval method when current step is not the additional questions step", function () {
            var stepNumber = 4;
            fe_sp_initializeMatchReplacementEventListeners(stepNumber);

            expect(bindChangeEventsToAdditionalQuestionsSpy).toHaveBeenCalledTimes(0);
            expect(loadFailedMatchReplacementsWhenAdditionalQuestionsCompleteSpy).toHaveBeenCalledTimes(0);
        });

    });

    describe("fe_sp_bindChangeEventsToAdditionalQuestions", function () {

        var inputFields;
        
        beforeEach(function () {
            FormsEngine.StepDynamicQuestions = stepNumber;
            inputFields = form.find(":input");
        });

        describe("change event binding method tests", function () {

            var changeEventBindingMethods = [
                fe_sp_bindFailedMatchReplacementChangeEventToAdditionalQuestion,
                fe_sp_bindSubmitButtonShouldBeShownOrNotEventToAdditionalQuestion
            ];

            it("should call each change event binding method for how ever many fields exist on the step", function () {
                for (var i = 0; i < changeEventBindingMethods.length; i++) {
                    var changeEventBindingMethod = changeEventBindingMethods[i];
                    var methodName = changeEventBindingMethod.toString().match(/^function\s*([^\s(]+)/)[1];
                    var bindingMethodSpy = spyOn(window, methodName);

                    fe_sp_bindChangeEventsToAdditionalQuestions();

                    expect(bindingMethodSpy).toHaveBeenCalledTimes(inputFields.length);
                }
            });
        });

        describe("change event count tests", function () {

            var handlerCount;
            var changeEventHandlers = [
                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete,
                fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown
            ];

            it("should add only one of each change event handler to each field", function () {

                for (var z = 0; z < changeEventHandlers.length; z++) {
                    var changeEventHandler = changeEventHandlers[z];
                    handlerCount = 0;

                    // try and bind hanlder multitple times
                    for (var j = 0; j < 3; j++) {
                        fe_sp_bindChangeEventsToAdditionalQuestions();
                    }

                    for (var i = 0; i < inputFields.length; i++) {
                        var events = $._data(inputFields[i], "events");
                        var changeEvents = events.change;

                        for (var x = 0; x < changeEvents.length; x++) {
                            if (changeEvents[x].handler === changeEventHandler) {
                                handlerCount++;
                            }
                        }
                    }

                    expect(handlerCount).toBe(inputFields.length, "expected changeEventHandler: " + changeEventHandler.name);
                }
            });
        });
    });

    describe("fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete", function () {

        var validateStepWithoutErrorMessagesSpy;
        
        beforeEach(function () {
            validateStepWithoutErrorMessagesSpy = spyOn(window, "fe_sp_validateStepWithoutErrorMessages");
        });

        describe("fe_sp_loadFailedMatchReplacements called tests", function () {
            var loadFailedMatchReplacementsSpy;

            beforeEach(function () {
                loadFailedMatchReplacementsSpy = spyOn(window, "fe_sp_loadFailedMatchReplacements");
            });

            it("loads failed match replacements when step is complete", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(true);

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(loadFailedMatchReplacementsSpy).toHaveBeenCalled();
            });


            it("doesnt load failed match replacements when step is incomplete", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(false);

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(loadFailedMatchReplacementsSpy).toHaveBeenCalledTimes(0);
            });

        });

        describe("triggers click test", function () {

            var clickCounter;

            beforeEach(function () {
                clickCounter = 0;

                $(FormsEngine.SubmitButton).one("click", function () {
                    $(FormsEngine).trigger("OnStepLoaded");
                    clickCounter++;
                });

                FormsEngine.FormsHasBeenRecovered = undefined;
            });

            it("triggers a click on the submit button to move the step forward when the step is complete, auto forward step is on, moveForward flag is true, and form hasnt been recovered", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(true);
                FormsEngine.DontAllowAutoForwardStep = false;

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(clickCounter).toEqual(1);
            });

            it("doesnt trigger a click on the submit button to move the step forward when the step isnt complete, auto forward step is on, moveForward flag is true, and form hasnt been recovered", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(false);
                FormsEngine.DontAllowAutoForwardStep = false;

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(clickCounter).toEqual(0);
            });

            it("doesnt trigger a click on the submit button to move the step forward when the step is complete, auto forward step is off, moveForward flag is true, and form hasnt been recovered", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(true);
                FormsEngine.DontAllowAutoForwardStep = true;

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(clickCounter).toEqual(0);
            });

            it("doesnt trigger a click on the submit button to move the step forward when the step is complete, auto forward step is on, moveForward flag is false, and form hasnt been recovered", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(true);
                FormsEngine.DontAllowAutoForwardStep = false;
                var moveForward = false;

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete(moveForward);

                expect(clickCounter).toEqual(0);
            });

            it("doesnt trigger a click on the submit button to move the step forward when the step is complete, auto forward step is on, moveForward flag is true, and form has been recovered", function () {
                validateStepWithoutErrorMessagesSpy.and.returnValue(true);
                FormsEngine.DontAllowAutoForwardStep = false;
                FormsEngine.FormsHasBeenRecovered = true;

                fe_sp_loadFailedMatchReplacementsWhenAdditionalQuestionsComplete();

                expect(clickCounter).toEqual(0);
            });
        });
    });

    describe("fe_sp_processFailedMatchReplacementsResponse", function () {

        var component = "<div><button>select</button></div>";

        beforeEach(function () {
            FormsEngine.CurrentStep = stepNumber;
            $(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep).append("<div id='school-picker-failures'></div>");

            FormsEngine.SchoolPickerSelections = {
                4462: {
                    institutionId: 4462,
                    institutionName: "University of Miami",
                    programId: 243996,
                    programProductId: 359259,
                    programTemplateId: 2
                },
                8324: {
                    institutionId: 8324,
                    institutionName: "QA National",
                    programId: 302506,
                    programProductId: 634856,
                    programTemplateId: 3
                },
                8616: {
                    institutionId: 8616,
                    institutionName: "Match1",
                    programId: 302599,
                    programProductId: 634947,
                    programTemplateId: 1
                }
            };
        });

        it("adds new replacement matches to school picker selections and sets FormsEngine.SmartMatchSchoolNames", function () {

            var failedMatchReplacments = {
                FailedMatches: [
                    {
                        InstitutionId: 8324,
                        InstitutionName: "QA National",
                        ProgramId: 302506,
                        ProgramProductId: 634856,
                        ProgramTemplateId: 3
                    },
                    {
                        InstitutionId: 8616,
                        InstitutionName: "Match1",
                        ProgramId: 302599,
                        ProgramProductId: 634947,
                        ProgramTemplateId: 1
                    }
                ],
                ReplacementMatches: [
                    {
                        InstitutionId: 272,
                        InstitutionName: "Northcentral University",
                        ProgramId: 3646,
                        ProgramName: "Master of Education - Global Training and Development",
                        ProgramProductId: 633822,
                        ProgramTemplateId: 4
                    },
                    {
                        InstitutionId: 497,
                        InstitutionName: "Concordia University - Nebraska",
                        ProgramId: 302475,
                        ProgramName: "Program_PremierBusiness",
                        ProgramProductId: 634819,
                        ProgramTemplateId: 1
                    }
                ],
                ReplacementHtmlComponents: []
            };


            fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacments);

            var expectedResult = {
                4462: {
                    institutionId: 4462,
                    institutionName: "University of Miami",
                    programId: 243996,
                    programProductId: 359259,
                    programTemplateId: 2
                },
                272: {
                    institutionId: 272,
                    institutionName: "Northcentral University",
                    programId: 3646,
                    programProductId: 633822,
                    programTemplateId: 4,
                    isReplacementMatch: true
                },
                497: {
                    institutionId: 497,
                    institutionName: "Concordia University - Nebraska",
                    programId: 302475,
                    programProductId: 634819,
                    programTemplateId: 1,
                    isReplacementMatch: true
                },
                8616: {
                    institutionId: 8616,
                    institutionName: "Match1",
                    programId: 302599,
                    programProductId: 634947,
                    programTemplateId: 1
                },
                8324: {
                    institutionId: 8324,
                    institutionName: "QA National",
                    programId: 302506,
                    programProductId: 634856,
                    programTemplateId: 3
                }
            };

            expect(FormsEngine.SchoolPickerSelections).toEqual(expectedResult);


            var expectedSmartMatchSchoolNames = "";
            for (var key in FormsEngine.SchoolPickerSelections) {
                expectedSmartMatchSchoolNames += FormsEngine.SchoolPickerSelections[key].institutionName + ", ";
            }

            expect(FormsEngine.SmartMatchSchoolNames).toEqual(expectedSmartMatchSchoolNames);
        });

        it("adds failed match replacement components to the dom", function () {

            var failedMatches = [
                {
                    InstitutionId: 272,
                    InstitutionName: "Northcentral University",
                    InstitutionLogoUrl: "https://logo.educationdynamics.com/272/Logo_240x80.gif?1346189645",
                    ProgramId: 3646,
                    ProgramName: "Master of Education - Global Training and Development",
                    ProgramProductId: 633822,
                    ProgramTemplateId: 4
                },
                {
                    InstitutionId: 497,
                    InstitutionName: "Concordia University - Nebraska",
                    InstitutionLogoUrl: "https://logo.educationdynamics.com/497/Logo_240x80.gif?1337633975",
                    ProgramId: 302475,
                    ProgramName: "Program_PremierBusiness",
                    ProgramProductId: 634819,
                    ProgramTemplateId: 1
                },
                {
                    InstitutionId: 8324,
                    InstitutionName: "QA National",
                    InstitutionLogoUrl: "https://logo.educationdynamics.com/8324/Logo_240x80.gif?1337633975",
                    ProgramId: 302506,
                    ProgramName: "RN License",
                    ProgramProductId: 634856,
                    ProgramTemplateId: 1
                }
            ];

            // run multiple times while increasing the amount of components
            for (var j = 0; j < 2; j++) {
                var failedMatchReplacements = {
                    FailedMatches: [],
                    ReplacementMatches: [],
                    ReplacementHtmlComponents: []
                };

                for (var x = 0; x <= j; x++) {
                    failedMatchReplacements.ReplacementHtmlComponents.push(component);
                    failedMatchReplacements.FailedMatches.push(failedMatches[x]);
                }

                failedMatchReplacements.Message = "Programs Failed";

                fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements);

                var html = $(FormsEngine.DefaultFormTag + " #school-picker-failures").html();

                var expectedResult = "";
                expectedResult += "<h3>" + failedMatchReplacements.Message + "</h3>";

                for (var i = 0; i < failedMatchReplacements.ReplacementHtmlComponents.length; i++) {
                    expectedResult += failedMatchReplacements.ReplacementHtmlComponents[i];
                }


                expect(html).toEqual(expectedResult);
            }
        });

        it("doesnt add any components to the dom when there are no match replacements", function () {
            var failedMatchReplacements = {
                FailedMatches: [],
                ReplacementMatches: [],
                ReplacementHtmlComponents: []
            };

            fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements);

            var html = $(FormsEngine.DefaultFormTag + " #school-picker-failures").html();

            var expectedResult = "";

            expect(html).toEqual(expectedResult);
        });

        it("binds click event handlers to buttons in match replacement components", function () {

            // run multiple times while increasing the amount of components
            for (var j = 0; j < 2; j++) {
                var failedMatchReplacements = {
                    FailedMatches: [],
                    ReplacementMatches: [],
                    ReplacementHtmlComponents: []
                };

                for (var x = 0; x <= j; x++) {
                    failedMatchReplacements.ReplacementHtmlComponents.push(component);
                }

                fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements);

                var handlerCount = 0;
                var buttons = $(FormsEngine.DefaultFormTag + " #school-picker-failures").find("button");

                for (var i = 0; i < buttons.length; i++) {
                    var events = $._data(buttons[i], "events");

                    if (!events) {
                        fail("No events bound to button");
                        break;
                    }

                    var clickEvents = events.click;
                    for (var c = 0; c < clickEvents.length; c++) {
                        handlerCount++;
                    }
                }

                expect(handlerCount).not.toEqual(0);
                expect(handlerCount).toEqual(buttons.length);
            }

        });

        it("triggers click on submit button to move to the next step when there are no replacements and current step contains the match replacements", function () {

            var failedMatchReplacements = {
                FailedMatches: [],
                ReplacementMatches: [],
                ReplacementHtmlComponents: []
            };

            var clickCounter = 0;

            $(FormsEngine.SubmitButton).on("click", function () {
                clickCounter++;
            });

            fe_sp_processFailedMatchReplacementsResponse(failedMatchReplacements);

            expect(clickCounter).toEqual(1);
        });

    });

    describe("fe_sp_matchReplacementButtonClicked", function () {

        var matchReplacementElement;

        beforeEach(function () {
            var matchReplacementElementHtml = "<div data-institutionid='1000' data-programid='10001' data-programproductid='100001' data-programtemplateid='1000001' data-institutionname='test' data-institutionlogourl='testurl'> \
                                            <button type='button' class='btn yes-btn'>Yes</button> \
                                            <button type='button' class='btn no-btn'>No</button> \
                                       </div>";

            matchReplacementElement = $(matchReplacementElementHtml);
        });


        it("selects a match replacement", function () {

            var button = $(matchReplacementElement).find(".yes-btn").first();

            fe_sp_matchReplacementButtonClicked(button);

            var expectedResult = {
                1000: {
                    institutionId: 1000,
                    institutionName: "test",
                    programId: 10001,
                    programProductId: 100001,
                    programTemplateId: 1000001,
                    isReplacementMatch: true
                }
            };

            expect(FormsEngine.SchoolPickerSelections).toEqual(expectedResult);
        });

        it("unselects a match replacement", function () {

            FormsEngine.SchoolPickerSelections = {
                1000: {
                    "institutionId": 1000,
                    "programId": 10001,
                    "programProductId": 100001,
                    "programTemplateId": 1000001
                }
            };

            var button = $(matchReplacementElement).find(".no-btn").first();

            fe_sp_matchReplacementButtonClicked(button);

            expect(FormsEngine.SchoolPickerSelections).toEqual({});
        });

        it("adds selected class to yes button and unselected class to no button when selected", function () {
            var yesButton = $(matchReplacementElement).find(".yes-btn").first();
            var noButton = $(matchReplacementElement).find(".no-btn").first();

            fe_sp_matchReplacementButtonClicked(yesButton);

            expect(yesButton.hasClass("btn-primary")).toBe(true);
            expect(noButton.hasClass("btn-default")).toBe(true);
        });

        it("adds selected class to no button and unselected class to yes button when unselected", function () {
            var yesButton = $(matchReplacementElement).find(".yes-btn").first();
            var noButton = $(matchReplacementElement).find(".no-btn").first();

            fe_sp_matchReplacementButtonClicked(noButton);

            expect(yesButton.hasClass("btn-default")).toBe(true);
            expect(noButton.hasClass("btn-primary")).toBe(true);
        });
    });

    describe("fe_sp_upsertSchoolPickerSelection", function () {

        it("doesnt add school picker selection if any value is not an integer", function () {
            var selections = [
                {
                    institutionId: 1,
                    element: "<div data-programid='12' data-programproductid='123' data-programtemplateid='nonintegervalue'></div>"
                },
                {
                    institutionId: 3,
                    element: "<div data-programid='12' data-programproductid='nonintegervalue' data-programtemplateid='1234'></div>"
                },
                {
                    institutionId: 4,
                    element: "<div data-programid='nonintegervalue' data-programproductid='123' data-programtemplateid='1234'></div>"
                },
                {
                    institutionId: "non integer value",
                    element: "<div data-programid='12' data-programproductid='123' data-programtemplateid='1234'></div>"
                }
            ];

            selections.forEach(function (selection) {
                fe_sp_upsertSchoolPickerSelection(selection.institutionId, selections.element);
                expect(FormsEngine.SchoolPickerSelections[selection.institutionId]).toEqual(undefined);
            });
        });

    });

    describe("fe_sp_addLoadingSpinnerToComponent", function () {

        it("adds loading spinner to specified component", function () {
            var component = $("<div></div>");
            $(FormsEngine.DefaultFormTag).append(component);

            fe_sp_addLoadingSpinnerToComponent(component);

            var componentContainsLoadingSpinner = component.find("#component-loader-overlay").length > 0 && component.find("#component-loader").length > 0;
            var spinnerHasLoaderOnClass = $("#component-loader").hasClass("loaderOn");
            var componentLoaderIsVisible = $("#component-loader").is(":visible"); 
            var componentLoaderOverlayIsVisible = $("#component-loader-overlay").is(":visible"); 

            expect(componentContainsLoadingSpinner).toBe(true);
            expect(spinnerHasLoaderOnClass).toBe(true);
            expect(componentLoaderIsVisible).toBe(true);
            expect(componentLoaderOverlayIsVisible).toBe(true);
        });

        it("doesnt loading spinner to specified component when component passed is null", function () {
            
            fe_sp_addLoadingSpinnerToComponent(null);

            var formDoesntContainsLoadingSpinner = $(FormsEngine.DefaultFormTag).find("#component-loader-overlay").length < 1 && $(FormsEngine.DefaultFormTag).find("#component-loader").length < 1;

            expect(formDoesntContainsLoadingSpinner).toBe(true);
        });

    });

    describe("fe_sp_removeLoadingSpinner", function () {

        beforeEach(function () {
            var component = $("<div></div>");
            $(FormsEngine.DefaultFormTag).append(component);
            fe_sp_addLoadingSpinnerToComponent(component);
        });

        it("removes loading spinner from dom", function () {
            fe_sp_removeLoadingSpinner();

            var formDoesntContainsLoadingSpinner = $(FormsEngine.DefaultFormTag).find("#component-loader-overlay").length < 1 && $(FormsEngine.DefaultFormTag).find("#component-loader").length < 1;
            expect(formDoesntContainsLoadingSpinner).toBe(true);
        });

    });

    describe("fe_sp_addPlaceholderInputToMatchReplacementContainer", function () {

        beforeEach(function () {
            var schoolPickerFailuresContainer = $("<div id='school-picker-failures'></div>");
            jQuery(FormsEngine.DefaultFormTag).append(schoolPickerFailuresContainer);
        });

        it("adds invisible placeholder input to match replacements container so the step isnt skipped while loading", function () {
            //var placeHolderInput = $('<input type="text" name="school-picker-failures-placeholder" value="" disabled />');

            fe_sp_addPlaceholderInputToMatchReplacementContainer();

            console.log(jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").html());

            var placeholderInput = jQuery(FormsEngine.DefaultFormTag + " #school-picker-failures").find("[name='school-picker-failures-placeholder']").first();
            var opacity = placeholderInput.css("opacity");
            var disabledAttr = placeholderInput.attr("disabled");

            expect(placeholderInput).not.toEqual(undefined, "Placeholder input field should not equal undefined");
            expect(opacity).toEqual("0", "Opacity should equal 0");
            expect(disabledAttr).not.toEqual(undefined, "Input field should have disabled attribute");
        });

    });

    describe("fe_sp_loadFailedMatchReplacements", function () {

        var addPlaceholderInputToMatchReplacementContainerSpy;
        var addLoadingSpinnerToComponentSpy;
        var makeGetFailedMatchReplacementsRequestSpy;
        var removeReplacementMatchesFromSchoolPickerSelectionsSpy;

        beforeEach(function () {
            addPlaceholderInputToMatchReplacementContainerSpy = spyOn(window, "fe_sp_addPlaceholderInputToMatchReplacementContainer");
            addLoadingSpinnerToComponentSpy = spyOn(window, "fe_sp_addLoadingSpinnerToComponent");
            makeGetFailedMatchReplacementsRequestSpy = spyOn(window, "fe_sp_makeGetFailedMatchReplacementsRequest");
            removeReplacementMatchesFromSchoolPickerSelectionsSpy = spyOn(window, "fe_sp_removeReplacementMatchesFromSchoolPickerSelections");
        });

        it("calls the fe_sp_addPlaceholderInputToMatchReplacementContainer method before the fe_sp_addLoadingSpinnerToComponent method", function () {
            fe_sp_loadFailedMatchReplacements();
            expect(addPlaceholderInputToMatchReplacementContainerSpy).toHaveBeenCalledBefore(addLoadingSpinnerToComponentSpy);
        });

        it("calls the fe_sp_addLoadingSpinnerToComponent method before the fe_sp_removeReplacementMatchesFromSchoolPickerSelections method", function () {
            fe_sp_loadFailedMatchReplacements();
            expect(addLoadingSpinnerToComponentSpy).toHaveBeenCalledBefore(removeReplacementMatchesFromSchoolPickerSelectionsSpy);
        });

        it("calls the fe_sp_removeReplacementMatchesFromSchoolPickerSelections before the fe_sp_makeGetFailedMatchReplacementsRequest method", function () {
            fe_sp_loadFailedMatchReplacements();
            expect(removeReplacementMatchesFromSchoolPickerSelectionsSpy).toHaveBeenCalledBefore(makeGetFailedMatchReplacementsRequestSpy);
        });

        it("calls the fe_sp_makeGetFailedMatchReplacementsRequest method", function () {
            fe_sp_loadFailedMatchReplacements();
            expect(makeGetFailedMatchReplacementsRequestSpy).toHaveBeenCalledTimes(1);
        });
    });

    describe("fe_sp_setPostalCodeFieldFromIpAddress", function () {

        var postalCodeField;

        beforeEach(function () {
            postalCodeField = $("<input/>");
            postalCodeField.addClass("postal-code-test-field");
            postalCodeField.attr("type", "text");
        });

        afterEach(function () {
            formStep.find(".postal-code-test-field").remove();
        });

        it("sets the postal code field", function () {
            postalCodeField.attr("code", "Postal_Code");
            formStep.append(postalCodeField);
            var postalCode = "33441";

            fe_sp_setPostalCodeFieldFromIpAddress(postalCode);

            expect(postalCodeField.val()).toEqual(postalCode);
        });

        it("sets the postal code duplicate field", function () {
            postalCodeField.attr("code", "Postal_Code_Duplicate");
            formStep.append(postalCodeField);
            var postalCode = "33467";

            fe_sp_setPostalCodeFieldFromIpAddress(postalCode);

            expect(postalCodeField.val()).toEqual(postalCode);
        });
    });

    describe("fe_sp_toggleSubmitButton", function () {

        it("enables the submit button when there are selections", function () {
            var enableSubmitButtonSpy = spyOn(window, "fe_sp_enableSubmitButton");

            FormsEngine.SchoolPickerSelections = {
                1000: {
                    "institutionId": 1000,
                    "programId": 10001,
                    "programProductId": 100001,
                    "programTemplateId": 1000001
                }
            };

            fe_sp_toggleSubmitButton();

            expect(enableSubmitButtonSpy).toHaveBeenCalled();
        });

        it("disables the submit button when the selections are no selections", function () {

            // multiple test cases
            var selections = [{}, null];
            var disableSubmitButtonSpy = spyOn(window, "fe_sp_disableSubmitButton");

            for (var i = 0; i < selections.length; i++) {
                FormsEngine.SchoolPickerSelections = selections[i];

                fe_sp_toggleSubmitButton();

                expect(disableSubmitButtonSpy).toHaveBeenCalledTimes(i + 1);
            }
        });


    });

    describe("fe_sp_enableSubmitButton", function () {

        beforeEach(function () {
            fe_sp_disableSubmitButton();
        });

        it("removes the overlay", function () {
            fe_sp_enableSubmitButton();

            var overlayCount = $(FormsEngine.SubmitButton).find("." + disabledSubmitButtonClassOverlay).length;
            expect(overlayCount).toEqual(0);
        });
    });

    describe("fe_sp_disableSubmitButton", function () {

        function overlayCss(style) {
            return $(FormsEngine.SubmitButton + " ." + disabledSubmitButtonClassOverlay).css(style);
        }

        beforeEach(function () {
            fe_sp_enableSubmitButton();
        });

        it("adds overlay div to button", function () {
            fe_sp_disableSubmitButton();

            var overlayCount = $(FormsEngine.SubmitButton).find("." + disabledSubmitButtonClassOverlay).length;
            expect(overlayCount).toEqual(1);
        });

        it("removes any existing overlay div before adding new overlay div to button", function () {
            for (var i = 0; i < 5; i++) {
                fe_sp_disableSubmitButton();
            }

            var overlayCount = $(FormsEngine.SubmitButton).find("." + disabledSubmitButtonClassOverlay).length;
            expect(overlayCount).toEqual(1);
        });

        it("stops event propagation", function () {
            fe_sp_disableSubmitButton();

            var clickCounter = 0;
            $(FormsEngine.SubmitButton).on("click", function () {
                clickCounter++;
            });

            $(FormsEngine.SubmitButton + " ." + disabledSubmitButtonClassOverlay).trigger("click");

            expect(clickCounter).toEqual(0);
        });

        it("has position: absolute on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualPosition = overlayCss("position");
            expect(actualPosition).toEqual("absolute");
        });

        it("has position: relative on submit button", function () {
            fe_sp_disableSubmitButton();

            var actualPosition = $(FormsEngine.SubmitButton).css("position");
            expect(actualPosition).toEqual("relative");
        });

        it("has top: -1px on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualTop = overlayCss("top");
            expect(actualTop).toEqual("-1px");
        });

        it("has left: -1px on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualLeft = overlayCss("left");
            expect(actualLeft).toEqual("-1px");
        });

        it("has right: -1px on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualTop = overlayCss("right");
            expect(actualTop).toEqual("-1px");
        });

        it("has bottom: -1px on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualLeft = overlayCss("bottom");
            expect(actualLeft).toEqual("-1px");
        });

        it("has background-color: white on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualBackgroundColor = overlayCss("background-color");
            expect(actualBackgroundColor).toEqual("rgb(255, 255, 255)");
        });

        it("has z-index: 10 on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualZIndex = overlayCss("z-index");
            expect(actualZIndex.toString()).toEqual("10");
        });

        it("has opacity: 0.4 on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualOpacity = overlayCss("opacity");
            expect(actualOpacity).toEqual("0.4");
        });

        it("has cursor default on overlay", function () {
            fe_sp_disableSubmitButton();

            var actualCursor = overlayCss("cursor");
            expect(actualCursor).toEqual("default");
        });

    });

    describe("fe_sp_toggleSubmitButtonOnStepMovements", function () {

        it("toggles the submit button if the step contains the school picker", function () {
            var toggleSubmitButtonSpy = spyOn(window, "fe_sp_toggleSubmitButton");

            var carouselHtml = "<div><div id='school-picker-carousel'></div><div id='school-picker-carousel-indicators'></div></div>";
            $(formStep).append(carouselHtml);

            fe_sp_toggleSubmitButtonOnStepMovements(stepNumber);

            expect(toggleSubmitButtonSpy).toHaveBeenCalled();
        });

        it("enables the submit button if the step does not contain the school picker", function () {
            var enableSubmitButtonSpy = spyOn(window, "fe_sp_enableSubmitButton");

            fe_sp_toggleSubmitButtonOnStepMovements(stepNumber);

            expect(enableSubmitButtonSpy).toHaveBeenCalled();
        });

    });

    describe("fe_sp_maxSubmissionLimitReached", function () {

        beforeEach(function () {
            FormsEngine.SchoolPickerSelections = {
                1: { institutionId: 1 },
                2: { institutionId: 2 },
                3: { institutionId: 3 }
            };
        });

        it("returns true if the number of school picker selections is equal to the max submission limit", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: 3
            };

            var limitReached = fe_sp_maxSubmissionLimitReached();

            expect(limitReached).toBe(true);
        });

        it("returns true if the number of school picker selections is greater than the max submission limit", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: 2
            };

            var limitReached = fe_sp_maxSubmissionLimitReached();

            expect(limitReached).toBe(true);
        });

        it("returns true if the campaign details is null", function () {
            FormsEngine.CampaignDetail = null;

            var limitReached = fe_sp_maxSubmissionLimitReached();

            expect(limitReached).toBe(true);
        });

        it("returns true if the campaign details max submission count is null", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: null
            };

            var limitReached = fe_sp_maxSubmissionLimitReached();

            expect(limitReached).toBe(true);
        });

        it("returns false if the number of school picker selections is less than the max submission limit", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: 4
            };

            var limitReached = fe_sp_maxSubmissionLimitReached();

            expect(limitReached).toBe(false);
        });
    });

    describe("fe_sp_getSchoolPickerSelectionCount", function () {

        it("returns the number of school picker selections", function () {
            for (var i = 0; i < 5; i++) {
                FormsEngine.SchoolPickerSelections = {};

                for (var x = 0; x < i; x++) {
                    FormsEngine.SchoolPickerSelections[x] = {};
                }

                var numberOfSelections = fe_sp_getSchoolPickerSelectionCount();
                expect(numberOfSelections).toEqual(i);
            }
        });

        it("returns 0 if school picker selections is null", function () {
            FormsEngine.SchoolPickerSelections = null;

            var numberOfSelections = fe_sp_getSchoolPickerSelectionCount();

            expect(numberOfSelections).toEqual(0);
        });

    });

    describe("fe_sp_selectButton", function () {

        var button;

        beforeEach(function () {
            button = $("<button class='btn-default'>Click if Interested</button>");
        });

        it("adds primary class to button", function () {
            fe_sp_selectButton(button);
            expect(button.hasClass("btn-primary")).toEqual(true);
        });

        it("doesnt add default class to button", function () {
            fe_sp_selectButton(button);
            expect(button.hasClass("btn-default")).toEqual(false);
        });

        it("sets the selected text on the button", function () {
            fe_sp_selectButton(button);
            expect(button.text()).toEqual("Yes, I'm Interested");
        });

        it("adds the selected attribute to the button", function () {
            fe_sp_selectButton(button);
            expect(button.attr("data-selected")).not.toEqual(undefined);
        });
    });

    describe("fe_sp_unselectButton", function () {

        var button;

        beforeEach(function () {
            button = $("<button class='btn-primary' data-selected>Yes, I'm Interested</button>");
        });

        it("adds the default class to the button", function () {
            fe_sp_unselectButton(button);
            expect(button.hasClass("btn-default")).toEqual(true);
        });

        it("doesnt add the primary class to the button", function () {
            fe_sp_unselectButton(button);
            expect(button.hasClass("btn-primary")).toEqual(false);
        });

        it("sets the unselected text on the button", function () {
            fe_sp_unselectButton(button);
            expect(button.text()).toEqual("Click if Interested");
        });

        it("removes the selected attribute from the button", function () {
            fe_sp_unselectButton(button);
            expect(button.attr("data-selected")).toEqual(undefined);
        });

    });

    describe("fe_sp_convertNumberToWord", function () {

        it("returns the number converted to a word", function () {
            expect(fe_sp_convertNumberToWord(1)).toEqual("one");
            expect(fe_sp_convertNumberToWord(5)).toEqual("five");
            expect(fe_sp_convertNumberToWord(10)).toEqual("ten");
            expect(fe_sp_convertNumberToWord(15)).toEqual("fifteen");
            expect(fe_sp_convertNumberToWord(21)).toEqual("twenty one");
            expect(fe_sp_convertNumberToWord(68)).toEqual("sixty eight");
            expect(fe_sp_convertNumberToWord(100)).toEqual("one hundred");
            expect(fe_sp_convertNumberToWord(557)).toEqual("five hundred fifty seven");
            expect(fe_sp_convertNumberToWord(1121)).toEqual("one thousand one hundred twenty one");
        });

    });

    describe("fe_sp_getMaxSubmissionCount", function () {

        it("returns the max submission count", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: 5
            };

            var maxSubmissionCount = fe_sp_getMaxSubmissionCount();

            expect(maxSubmissionCount).toEqual(FormsEngine.CampaignDetail.MaxSubmissionCount);
        });

        it("returns 0 when CampaignDetail is null", function () {
            FormsEngine.CampaignDetail = null;

            var maxSubmissionCount = fe_sp_getMaxSubmissionCount();

            expect(maxSubmissionCount).toEqual(0);
        });

        it("returns 0 when MaxSubmissionCount is null", function () {
            FormsEngine.CampaignDetail = {
                MaxSubmissionCount: null
            };

            var maxSubmissionCount = fe_sp_getMaxSubmissionCount();

            expect(maxSubmissionCount).toEqual(0);
        });

    });

    describe("fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown", function () {

        FormsEngine.RestartButton = "#form-startover-button";
        FormsEngine.BackButton = "#form-navback-button";
        
        beforeEach(function () {
            FormsEngine.CurrentStep = stepNumber;
            
            $(FormsEngine.DefaultFormTag).append($('<div class="form-submit-button previous clearfix" id="form-navback-button"></div>'));
            $(FormsEngine.DefaultFormTag).append($('<div class="form-submit-button startover clearfix" id="form-startover-button"></div>'));
        });

        afterEach(function () {
            $(FormsEngine.BackButton).remove();
            $(FormsEngine.RestartButton).remove();
        });

        describe("passed school picker carousel step", function () {

            beforeEach(function () {
                FormsEngine.SchoolPickerCarouselLoaded = true;
            });

            it("hides back button if passed school picker carousel", function () {
                $(FormsEngine.BackButton).show();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.BackButton).css("display")).toEqual("none");
            });

            it("shows restart button if passed school picker carousel", function () {
                $(FormsEngine.RestartButton).hide();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.RestartButton).css("display")).not.toEqual("none");
            });
        });


        describe("not passed school picker or on school picker carousel step", function () {

            beforeEach(function () {
                FormsEngine.SchoolPickerCarouselLoaded = false;
            });

            it("hides restart button if not passed school picker carousel", function () {
                $(FormsEngine.RestartButton).show();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.RestartButton).css("display")).toEqual("none");
            });

            it("shows back button if not passed school picker carousel", function () {
                $(FormsEngine.BackButton).hide();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.BackButton).css("display")).not.toEqual("none");
            });
        });

        describe("on school picker step", function () {

            beforeEach(function () {
                var schoolPickerCarouselHtml = "<div id='school-picker-carousel'></div>";
                formStep.append($(schoolPickerCarouselHtml));
                FormsEngine.SchoolPickerCarouselLoaded = false;
            });

            afterEach(function () {
                $("#school-picker-carousel").remove();
            });

            it("hides back button if on school picker carousel step", function () {
                $(FormsEngine.BackButton).show();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.BackButton).css("display")).toEqual("none");
            });

            it("shows restart button if on school picker carousel step", function () {
                $(FormsEngine.RestartButton).hide();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.RestartButton).css("display")).not.toEqual("none");
            });
        });

        describe("on first step", function () {

            beforeEach(function () {
                FormsEngine.CurrentStep = 1;
            });

            it("hides the back button if on the first step", function () {
                $(FormsEngine.BackButton).show();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.BackButton).is(":visible")).toEqual(false);
            });

            it("hides the restart button if on the first step", function () {
                $(FormsEngine.RestartButton).show();

                fe_sp_determineIfBackButtonOrRestartButtonShouldBeShown();

                expect($(FormsEngine.RestartButton).is(":visible")).toEqual(false);
            });

        });
    });

    describe("fe_sp_loadFormWizardCallBack", function () {

        it("calls the fe_sp_buildStepIndicatorBar method", function () {
            var buildStepIndicatorBarSpy = spyOn(window, "fe_sp_buildStepIndicatorBar");

            fe_sp_loadFormWizardCallBack();

            expect(buildStepIndicatorBarSpy).toHaveBeenCalled();
        });

        it("calls the fe_sp_hideStepTitles method", function () {
            var hideStepTitlesSpy = spyOn(window, "fe_sp_hideStepTitles");

            fe_sp_loadFormWizardCallBack();

            expect(hideStepTitlesSpy).toHaveBeenCalled();
        });

        it("calls the fe_sp_getPostalCodeFromIpAddress method", function () {
            var getPostalCodeFromIpAddressSpy = spyOn(window, "fe_sp_getPostalCodeFromIpAddress");

            fe_sp_loadFormWizardCallBack();

            expect(getPostalCodeFromIpAddressSpy).toHaveBeenCalled();
        });

        it("calls the fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown method", function () {
            var determineWhetherOrNotSubmitButtonShouldBeShownSpy = spyOn(window, "fe_sp_determineWhetherOrNotSubmitButtonShouldBeShown");

            fe_sp_loadFormWizardCallBack();

            expect(determineWhetherOrNotSubmitButtonShouldBeShownSpy).toHaveBeenCalled();
        });

        it("shows back button and hides restart button", function () {
            FormsEngine.RestartButton = "#form-startover-button";
            FormsEngine.BackButton = "#form-navback-button";
            FormsEngine.CurrentStep = stepNumber;

            $(FormsEngine.DefaultFormTag).append($('<div class="form-submit-button previous clearfix" id="form-navback-button"></div>'));
            $(FormsEngine.DefaultFormTag).append($('<div class="form-submit-button startover clearfix" id="form-startover-button"></div>'));
            $(FormsEngine.RestartButton).show();
            $(FormsEngine.BackButton).hide();

            fe_sp_loadFormWizardCallBack();

            expect($(FormsEngine.RestartButton).css("display")).toEqual("none");
            expect($(FormsEngine.BackButton).css("display")).not.toEqual("none");

            $(FormsEngine.BackButton).remove();
            $(FormsEngine.RestartButton).remove();
        });

        it("sets FormsEngine.SchoolPickerCarouselLoaded to false", function () {
            FormsEngine.SchoolPickerCarouselLoaded = true;

            fe_sp_loadFormWizardCallBack();

            expect(FormsEngine.SchoolPickerCarouselLoaded).toEqual(false);
        });
    });

    describe("fe_sp_removeExistingStepIndicators", function () {

        var headerClass = "eddy-form-wizard-header";
        var stepIndicatorId = "institution-wizard-step-indicator-container";

        beforeEach(function () {

            var formHeaderHtml = "<div class='" + headerClass + "'>\
                                    <div id='" + stepIndicatorId + "'>\
                                        NON-EMPTY-HTML\
                                    </div>\
                                  </div>";

            $(document.body).append(formHeaderHtml);
        });

        afterEach(function () {
            $("." + headerClass).remove();
        });

        it("removes existing step indicators from the dom", function () {
            fe_sp_removeExistingStepIndicators();

            var stepIndicatorHtml = jQuery("." + headerClass + " #" + stepIndicatorId).html();
            expect(stepIndicatorHtml).toEqual("");
        });

    });

    describe("fe_sp_getPostalCodeFromIpAddress", function () {

        var makeRequestToGetPostalCodeFromIpAddressSpy;

        beforeEach(function () {
            makeRequestToGetPostalCodeFromIpAddressSpy = spyOn(window, "fe_sp_makeRequestToGetPostalCodeFromIpAddress");
        });

        it("doesnt make ajax request when FormsEngine.ShouldGetZipCodeFromIp is false", function () {
            FormsEngine.ShouldGetZipCodeFromIp = false;

            fe_sp_getPostalCodeFromIpAddress();

            expect(makeRequestToGetPostalCodeFromIpAddressSpy).not.toHaveBeenCalled();
        });

        it("does make ajax request when FormsEngine.ShouldGetZipCodeFromIp is true", function () {
            FormsEngine.ShouldGetZipCodeFromIp = true;

            fe_sp_getPostalCodeFromIpAddress();

            expect(makeRequestToGetPostalCodeFromIpAddressSpy).toHaveBeenCalled();
        });

    });

    describe("fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions", function () {

        var clickCounter;

        beforeEach(function () {
            clickCounter = 0;

            $(FormsEngine.SubmitButton).one("click", function () {
                clickCounter++;
            });

            FormsEngine.CurrentStep = stepNumber;

            $(FormsEngine.DefaultFormTag + " #DynamicQuestions").remove();
        });

        afterEach(function () {
            $(FormsEngine.DefaultFormTag + " #DynamicQuestions").remove();
        });

        it("triggers click on submit button when on the additional questions step and no questions are present", function () {
            FormsEngine.StepDynamicQuestions = stepNumber;
            $(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep).append("<div id='DynamicQuestions'></div>");

            fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions(FormsEngine.CurrentStep);

            expect(clickCounter).toEqual(1);
        });

        it("doesnt trigger click on submit button when on the additional questions step and questions are present", function () {
            FormsEngine.StepDynamicQuestions = stepNumber;
            $(FormsEngine.DefaultFormTag + " #Step" + FormsEngine.CurrentStep).append("<div id='DynamicQuestions'>questions</div>");

            fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions(FormsEngine.CurrentStep);

            expect(clickCounter).toEqual(0);
        });

        it("doesnt trigger click on submit button when not on the additional questions step", function () {
            FormsEngine.StepDynamicQuestions = stepNumber - 1;
            
            fe_sp_skipAdditionalQuestionStepIfNoAdditionalQuestions(FormsEngine.CurrentStep);

            expect(clickCounter).toEqual(0);
        });

    });

    describe("fe_sp_removeReplacementMatchesFromSchoolPickerSelections", function () {

        it("removes the replacement matches from school picker selections", function () {

            FormsEngine.SchoolPickerSelections = {
                4462: {
                    institutionId: 4462
                },
                272: {
                    institutionId: 272,
                    isReplacementMatch: true
                }
            };

            fe_sp_removeReplacementMatchesFromSchoolPickerSelections();

            var expectedResult = {
                4462: { institutionId: 4462 }
            };

            expect(FormsEngine.SchoolPickerSelections).toEqual(expectedResult);
        });

    });

    describe("fe_se_getMaxSchoolPickerSelectionCount", function () {

        var getMaxSubmissionCountSpy;

        beforeEach(function () {
            getMaxSubmissionCountSpy = spyOn(window, "fe_sp_getMaxSubmissionCount");
            getMaxSubmissionCountSpy.and.returnValue(5);
        });

        it("returns the max submission count if the school picker carousel count is greater than the max submission count", function () {
            FormsEngine.SchoolPickerCarouselCount = 10;

            var maxSelectionCount = fe_se_getMaxSchoolPickerSelectionCount();

            expect(maxSelectionCount).toEqual(5);
        });

        it("returns the max submission count if the school picker carousel count is  equal to the max submission count", function () {
            FormsEngine.SchoolPickerCarouselCount = 5;

            var maxSelectionCount = fe_se_getMaxSchoolPickerSelectionCount();

            expect(maxSelectionCount).toEqual(5);
        });

        it("returns the school picker carousel count if the school picker carousel count is less than the max submission count", function () {
            FormsEngine.SchoolPickerCarouselCount = 3;

            var maxSelectionCount = fe_se_getMaxSchoolPickerSelectionCount();

            expect(maxSelectionCount).toEqual(3);
        });

        it("returns 0 if FormsEngine.SchoolPickerCarouselCount is null", function () {
            FormsEngine.SchoolPickerCarouselCount = null;

            var maxSelectionCount = fe_se_getMaxSchoolPickerSelectionCount();

            expect(maxSelectionCount).toEqual(0);
        });

    });

});