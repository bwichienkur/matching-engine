
describe("Wizard.js functions", function () {

    describe("currentStepIsEmpty", function () {

        var form;
        var formStep;

        beforeEach(function () {
            FormsEngine.CurrentStep = 1;
            formStep = $("<div></div>").attr("id", "Step" + FormsEngine.CurrentStep);
            form = $("<form id='form' role='form' name='form'></form>");
            form.append(formStep);
            $(document.body).append(form);
            FormsEngine.DefaultFormTag = "#" + form.attr("id");
        });

        afterEach(function () {
            form.remove();
            form = null;
            formStep = null;
        });

        it("current step is not empty if it contains a school picker carousel", function () {
            var schoolPickerCarousel = $("<div></div>").attr("id", "school-picker-carousel");
            formStep.append(schoolPickerCarousel);

            var isEmpty = fe_wizard.currentStepIsEmpty();

            expect(isEmpty).toEqual(false);
        });
    });
    
});