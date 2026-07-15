namespace EDDY.IS.Vendor.Entities
{
    public class FormTemplateFieldOption
    {
        private string optionValue;
        private string optionText;
        //private bool isSelected;

        public string OptionValue
        {
            get
            {
                return optionValue;
            }

            set
            {
                optionValue = value;
            }
        }
        public string OptionText
        {
            get
            {
                return optionText;
            }

            set
            {
                optionText = value;
            }
        }

        //public bool IsSelected
        //{
        //    get
        //    {
        //        return isSelected;
        //    }

        //    set
        //    {
        //        isSelected = value;
        //    }
        //}
    }
}
