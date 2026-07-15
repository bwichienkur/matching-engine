namespace EDDY.IS.Vendor.Entities
{
    public class PostalCodeInclusion
    {
        private int programProductId;
        private bool isZipCodeInclusion;

        public int ProgramProductId
        {
            get
            {
                return programProductId;
            }

            set
            {
                programProductId = value;
            }
        }

        public bool IsZipCodeInclusion
        {
            get
            {
                return isZipCodeInclusion;
            }

            set
            {
                isZipCodeInclusion = value;
            }
        }
    }
}
