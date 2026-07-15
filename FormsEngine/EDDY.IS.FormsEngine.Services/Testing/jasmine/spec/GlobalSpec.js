
describe("Global.js functions", function () {

    describe("The isLastStep Method", function () {

        var FormsEngine;

        beforeEach(function () {
            FormsEngine = {};
        });

        it("returns true when ShowAllQuestionsOnFirstStep is enabled and CurrentStep equals LastStep", function () {

            var testDataParameters = [
                { currentStep: 1, stepLast: 1 },
                { currentStep: "1", stepLast: "1" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = true;
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(true);
            });
        });

        it("returns false when ShowAllQuestionsOnFirstStep is enabled and CurrentStep doesnt equal LastStep", function () {

            var testDataParameters = [
                { currentStep: 1, stepLast: 2, stepDynamicQuestions: 3 },
                { currentStep: "1", stepLast: "2", stepDynamicQuestions: "3" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = true;
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(false);
            });
        });

        it("returns true when there is no additional questions and CurrentStep equals LastStep", function () {

            var testDataParameters = [
                { currentStep: 1, stepLast: 1 },
                { currentStep: "1", stepLast: "1" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.ShowAllQuestionsOnFirstStep = false;
                FormsEngine.HasAdditionalQuestions = false;
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(true);
            });
        });

        it("returns true when CurrentStep equals StepDynamicQuestions and has additional questions", function () {

            var testDataParameters = [
                { currentStep: 2, stepDynamicQuestions: 2 },
                { currentStep: "2", stepDynamicQuestions: "2" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = false;

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(true);
            });
        });

        it("returns false when CurrentStep doesnt equal StepDynamicQuestions and has additional questions", function () {

            var testDataParameters = [
                { currentStep: 1, stepLast: 1, stepDynamicQuestions: 2 },
                { currentStep: "1", stepLast: "1", stepDynamicQuestions: "2" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = false;

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(false);
            });
        });

        it("returns true when CurrentStep equals LastStep, rendering strategy is SchoolPicker, and has addditional questions", function () {

            var testDataParameters = [
                { currentStep: 3, stepLast: 3, stepDynamicQuestions: 2 },
                { currentStep: "3", stepLast: "3", stepDynamicQuestions: "2" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = false;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(true);
            });
        });

        it("returns false when CurrentStep equals StepDynamicQuestions, rendering strategy is SchoolPicker, and has addditional questions", function () {

            var testDataParameters = [
                { currentStep: 2, stepLast: 3, stepDynamicQuestions: 2 },
                { currentStep: "2", stepLast: "3", stepDynamicQuestions: "2" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.HasAdditionalQuestions = true;
                FormsEngine.ShowAllQuestionsOnFirstStep = false;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(false);
            });
        }); 

        it("returns false when CurrentStep equals StepDynamicQuestions, rendering strategy is SchoolPicker, and doesnt have addditional questions", function () {

            var testDataParameters = [
                { currentStep: 2, stepLast: 3, stepDynamicQuestions: 2 },
                { currentStep: "2", stepLast: "3", stepDynamicQuestions: "2" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.HasAdditionalQuestions = false;
                FormsEngine.ShowAllQuestionsOnFirstStep = false;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isLastStep = fe_isLastStep(FormsEngine);

                expect(isLastStep).toBe(false);
            });

        }); 

    });    

    describe("The isGoingToStepBeforeAdditionalQuestions Method", function () {

        var FormsEngine;

        beforeEach(function () {
            FormsEngine = {};
        });

        it("returns true when current step is one step less than the last step and rendering strategy is not SchoolPickerWizard", function () {

            var testDataParameters = [
                { currentStep: 2, stepDynamicQuestions: 4, stepLast: 3 },
                { currentStep: "2", stepDynamicQuestions: "4", stepLast: "3" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.StepLast = testDataParameter.stepLast;

                var isStepBeforeAdditionalQuestions = fe_isGoingToStepBeforeAdditionalQuestions(FormsEngine);

                expect(isStepBeforeAdditionalQuestions).toBe(true);
            });
        });

        it("returns false when current step is one step less than the last step and rendering strategy is SchoolPickerWizard", function () {

            var testDataParameters = [
                { currentStep: 2, stepDynamicQuestions: 4, stepLast: 3 },
                { currentStep: "2", stepDynamicQuestions: "4", stepLast: "3" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isStepBeforeAdditionalQuestions = fe_isGoingToStepBeforeAdditionalQuestions(FormsEngine);

                expect(isStepBeforeAdditionalQuestions).toBe(false);
            });
        });

        it("returns false when current step equals the last step", function () {

            var testDataParameters = [
                { currentStep: 3, stepDynamicQuestions: 4, stepLast: 3 },
                { currentStep: "3", stepDynamicQuestions: "4", stepLast: "3" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.StepLast = testDataParameter.stepLast;

                var isStepBeforeAdditionalQuestions = fe_isGoingToStepBeforeAdditionalQuestions(FormsEngine);

                expect(isStepBeforeAdditionalQuestions).toBe(false);
            });
        });

        it("returns true when the current step is the one before the additional questions step, and rendering strategy is SchoolPickerWizard", function () {

            var testDataParameters = [
                { currentStep: 2, stepDynamicQuestions: 3, stepLast: 4 },
                { currentStep: "2", stepDynamicQuestions: "3", stepLast: "4" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isStepBeforeAdditionalQuestions = fe_isGoingToStepBeforeAdditionalQuestions(FormsEngine);

                expect(isStepBeforeAdditionalQuestions).toBe(true);
            });
        });

        it("returns false when the current step is the additional questions step, and rendering strategy is SchoolPickerWizard", function () {
            var testDataParameters = [
                { currentStep: 3, stepDynamicQuestions: 3, stepLast: 4 },
                { currentStep: "3", stepDynamicQuestions: "3", stepLast: "4" }
            ];

            testDataParameters.forEach(function (testDataParameter) {
                FormsEngine.CurrentStep = testDataParameter.currentStep;
                FormsEngine.StepDynamicQuestions = testDataParameter.stepDynamicQuestions;
                FormsEngine.StepLast = testDataParameter.stepLast;
                FormsEngine.RenderingStrategy = "SCHOOLPICKERWIZARD";

                var isStepBeforeAdditionalQuestions = fe_isGoingToStepBeforeAdditionalQuestions(FormsEngine);

                expect(isStepBeforeAdditionalQuestions).toBe(false);
            });
        });

    });    

    describe("google tag manager dataLayer tests", function () {

        var piiCodes = ["First_Name", "Last_Name", "Phone", "Alternate_Phone", "Email", "Address", "Address_2"];

        // small subset of allowed fields
        var allowedCodes = ["Age", "City", "Country", "Desired_Start_Date", "GPA", "Highest_Level_of_Education_Completed", "Military_Affiliation", "Postal_Code", "Program_Of_Interest"];

        describe("fe_isCodePII", function () {

            it("returns true for pii fields", function () {
                for (var i = 0; i < piiCodes.length; i++) {

                    var code = piiCodes[i];

                    var codeIsPII = fe_isCodePII(code);

                    expect(codeIsPII).toEqual(true);
                }
            });

            it("returns false for allowed fields", function () {
                for (var i = 0; i < allowedCodes.length; i++) {

                    var code = allowedCodes[i];

                    var codeIsPII = fe_isCodePII(code);

                    expect(codeIsPII).toEqual(false);
                }
            });

        });


        describe("fe_pushSingleFieldToGTMDataLayer", function () {

            var testValue = "test value";

            beforeEach(function () {
                window.dataLayer = [];
            });

            it("doesnt allow PII fields to be pushed", function () {
                for (var i = 0; i < piiCodes.length; i++) {
                    var code = piiCodes[i];

                    fe_pushSingleFieldToGTMDataLayer(code, testValue);

                    expect(window.dataLayer.length).toEqual(0);
                }
            });

            it("allows non PII fields to be pushed", function () {
                for (var i = 0; i < allowedCodes.length; i++) {
                    var code = allowedCodes[i];

                    fe_pushSingleFieldToGTMDataLayer(code, testValue);

                    expect(window.dataLayer.length).toEqual(i + 1);
                }
            });

        });
    });

    describe("tests for moving controls to the last step", function () {

        describe("fe_moveControlAlongsideTCPA", function () {
            var dummySibling = $("<div id='dummySibling'><div>");
            var phoneControl = $('<div class="field-holder form-group input-group-field" data-controlcode="Phone"> <label>Phone Number*</label> <div class="input-group"> <span class="input-group-addon"></span> <input type="tel" controltypename="Text Box" step="1" section="1" code="Phone" name="Phone" class="text-field form-control valid" label-name="Phone Number" required="required"></div></div>');
            var tcpaControl = $('<div class="field-holder form-group input-group-field" data-controlcode="UserAgreement"></div>');
            var dynamicQuestionsSection = $("<div id='DynamicQuestionsUserAgreementSection'></div>");
            var formId = "defaultForm";

            function setupForm() {
                FormsEngine.DefaultFormTag = "#" + formId;

                var form = $("<form></form>").attr("id", formId);
                var stepOne = $("<div id='Step1' class='steps' data-step='1'></div>");
                var stepTwo = $("<div id='Step2' class='steps' data-step='2'></div>");
                var stepThree = $("<div id='Step3'class='steps' data-step='3'></div>");

                $(document.body).append(form);
                $(FormsEngine.DefaultFormTag).append(stepOne);
                $(FormsEngine.DefaultFormTag).append(stepTwo);
                $(FormsEngine.DefaultFormTag).append(stepThree);
            }

            function tearDownForm() {
                $("#" + formId).remove();
            }

            beforeEach(function () {
                setupForm();
                FormsEngine.StepLast = 3;
            });

            afterEach(function () {
                tearDownForm();
            });

            describe("tcpa control after phone control", function () {

                beforeEach(function () {
                    $(FormsEngine.DefaultFormTag).find("#Step1").append(phoneControl);
                    $(FormsEngine.DefaultFormTag).find("#Step1").append(dummySibling);
                    $(FormsEngine.DefaultFormTag).find("#Step3").append(tcpaControl);
                });


                describe("control not in ControlsToShowAlongsideTCPA", function () {
                    beforeEach(function () {
                        FormsEngine.CurrentStep = 2;
                    });

                    it("doesnt move the control if control is not in ControlsToShowAlongsideTCPA", function () {
                        FormsEngine.ControlsToShowAlongsideTCPA = ["Email"];

                        fe_moveControlAlongsideTCPA("Phone");

                        var parentId = phoneControl.parent().attr("id");
                        expect(parentId).toEqual("Step1");
                        expect(phoneControl.next().attr("id")).toEqual("dummySibling");
                    });


                    it("doesnt move the control if ControlsToShowAlongsideTCPA is null", function () {
                        FormsEngine.ControlsToShowAlongsideTCPA = null;

                        fe_moveControlAlongsideTCPA("Phone");

                        var parentId = phoneControl.parent().attr("id");
                        expect(parentId).toEqual("Step1");
                        expect(phoneControl.next().attr("id")).toEqual("dummySibling");
                    });
                });


                describe("control in ControlsToShowAlongsideTCPA", function () {
                    beforeEach(function () {
                        FormsEngine.ControlsToShowAlongsideTCPA = ["Phone"];
                    });

                    it("moves the control in front of tcpa if current step is past step with control", function () {
                        FormsEngine.CurrentStep = 2;

                        fe_moveControlAlongsideTCPA("Phone");

                        var parentId = phoneControl.parent().attr("id");
                        expect(parentId).toEqual("Step3");
                        expect(phoneControl.next().attr("data-controlcode")).toEqual("UserAgreement");
                    });

                    it("doesnt move the control in front of tcpa if current step is not past step with control", function () {
                        FormsEngine.CurrentStep = 1;

                        fe_moveControlAlongsideTCPA("Phone");

                        var parentId = phoneControl.parent().attr("id");
                        expect(parentId).toEqual("Step1");
                        expect(phoneControl.next().attr("id")).toEqual("dummySibling");
                    });
                });

            });

            describe("tcpa control before phone control", function () {

                beforeEach(function () {
                    $(FormsEngine.DefaultFormTag).find("#Step2").append(tcpaControl);
                    $(FormsEngine.DefaultFormTag).find("#Step3").append(phoneControl);

                    FormsEngine.ControlsToShowAlongsideTCPA = ["Phone"];
                });

                it("moves the control in front of the tcpa if the tcpa is on a step before the control", function () {
                    FormsEngine.CurrentStep = 2;

                    fe_moveControlAlongsideTCPA("Phone");

                    var parentId = phoneControl.parent().attr("id");
                    expect(parentId).toEqual("Step2");
                    expect(phoneControl.next().attr("data-controlcode")).toEqual("UserAgreement");
                });
            });
        });
    });

    describe("fe_determineIfHeaderDirectionButtonsShouldBeHidden", function () {

        var hiddenClass = "hidden";
        var prevButtonId = "prev-top-img";
        var nextButtonId = "next-top-img";

        beforeEach(function () {
            var prevButton = $("<span></span>").attr("id", prevButtonId);
            var nextButton = $("<span></span>").attr("id", nextButtonId);

            $(document.body).append(prevButton);
            $(document.body).append(nextButton);
        });

        afterEach(function () {
            $("#" + prevButtonId).remove();
            $("#" + nextButtonId).remove();
        });

        describe("truthy tests", function () {

            beforeEach(function () {
                FormsEngine.ShowArrowsInMobileHeader = false;
            });

            it("adds hidden class to previous arrow button when FormsEngine.ShowArrowsInMobileHeader is false", function () {
                fe_determineIfHeaderDirectionButtonsShouldBeHidden();

                var hasHiddenClass = $("#" + prevButtonId).hasClass(hiddenClass);
                expect(hasHiddenClass).toBe(true);
            });

            it("adds hidden class to next arrow button when FormsEngine.ShowArrowsInMobileHeader is false", function () {
                fe_determineIfHeaderDirectionButtonsShouldBeHidden();

                var hasHiddenClass = $("#" + nextButtonId).hasClass(hiddenClass);
                expect(hasHiddenClass).toBe(true);
            });
        });

        describe("falsey tests", function () {

            beforeEach(function () {
                FormsEngine.ShowArrowsInMobileHeader = true;
            });

            it("doesnt add hidden class to previous arrow button when FormsEngine.ShowArrowsInMobileHeader is true", function () {
                fe_determineIfHeaderDirectionButtonsShouldBeHidden();

                var hasHiddenClass = $("#" + prevButtonId).hasClass(hiddenClass);
                expect(hasHiddenClass).toBe(false);
            });

            it("doesnt add hidden class to next arrow button when FormsEngine.ShowArrowsInMobileHeader is true", function () {
                fe_determineIfHeaderDirectionButtonsShouldBeHidden();

                var hasHiddenClass = $("#" + nextButtonId).hasClass(hiddenClass);
                expect(hasHiddenClass).toBe(false);
            });
        });
    });

    describe("fe_getInstitutionDetail", function () {

        var makeGetInstitutionRequestSpy;

        beforeEach(function () {
            makeGetInstitutionRequestSpy = spyOn(window, "fe_makeGetInstitutionRequest");
        });

        it("calls the InstitutionDetailLoaded event handler if defined", function () {
            FormsEngine.InstitutionDetailLoaded = function (data) { };

            fe_getInstitutionDetail();

            expect(makeGetInstitutionRequestSpy).toHaveBeenCalledWith(FormsEngine.InstitutionDetailLoaded);
        });

        it("doesnt call the InstitutionDetailLoaded event handler if not defined", function () {
            FormsEngine.InstitutionDetailLoaded = null;

            fe_getInstitutionDetail();

            expect(makeGetInstitutionRequestSpy).not.toHaveBeenCalled();
        });

    });

    describe("fe_makeGetInstitutionRequest", function () {

        var callback;

        beforeEach(function () {
            jasmine.Ajax.install();

            callback = jasmine.createSpy('callback');
        });

        afterEach(function () {
            jasmine.Ajax.uninstall();
        });

        it("makes request with correct params and calls callback", function () {
            FormsEngine.ServiceBaseURL = "www.dummyformserviceurl.local";
            FormsEngine.InstitutionId = 10;
            FormsEngine.IsBeta = false;
            FormsEngine.TrackId = "351C687F-E7A2-42F8-A3AA-31B4D4C792F2";
            FormsEngine.ApplicationId = 7;

            var arguments = "?InstitutionId=" + FormsEngine.InstitutionId;
            arguments += "&IsBeta=" + FormsEngine.IsBeta;
            arguments += "&TrackId=" + FormsEngine.TrackId;
            arguments += "&ApplicationId=" + FormsEngine.ApplicationId;
            var url = FormsEngine.ServiceBaseURL + "/Institution/GetInstitution" + arguments;

            fe_makeGetInstitutionRequest(callback);

            var urlIndex = jasmine.Ajax.requests.mostRecent().url.indexOf(url);

            console.log(jasmine.Ajax.requests.mostRecent().response)

            expect(urlIndex).toBeGreaterThan(-1);
        });

        it("calls the callback", function () {
            spyOn($, 'ajax').and.callFake(function (params) {
                params.success("response data");
            });

            fe_makeGetInstitutionRequest(callback);

            expect(callback).toHaveBeenCalledWith("response data");            
        });

    });

    describe("fe_addLastStepClassToBackButtonIfOnLastStep", function () {

        var isLastStepSpy;
        var backButtonId = "back-button";
        var lastStepClass = "last-step";

        function addBackButtonToDom() {
            var button = $("<div></div>").attr("id", "back-button");
            $(document.body).append(button);
        }

        function removeBackButtonFromDom() {
            $("#" + backButtonId).remove();
        }

        beforeEach(function () {
            addBackButtonToDom();
            isLastStepSpy = spyOn(window, "fe_isLastStep");
            FormsEngine.BackButton = "#" + backButtonId;
        });

        afterEach(function () {
            removeBackButtonFromDom();
            FormsEngine.BackButton = null;
        });

        it("adds the 'last-step' class to the back button when form is on the last step", function () {
            isLastStepSpy.and.returnValue(true);
            $(FormsEngine.BackButton).removeClass(lastStepClass);

            fe_addLastStepClassToBackButtonIfOnLastStep();

            var hasLastStepClass = $(FormsEngine.BackButton).hasClass(lastStepClass);
            expect(hasLastStepClass).toBe(true);
        });

        it("doesnt add the 'last-step' class to the back button when form isnt on the last step", function () {
            isLastStepSpy.and.returnValue(false);
            $(FormsEngine.BackButton).addClass(lastStepClass);

            fe_addLastStepClassToBackButtonIfOnLastStep();

            var hasLastStepClass = $(FormsEngine.BackButton).hasClass(lastStepClass);
            expect(hasLastStepClass).toBe(false);
        });

    });

});