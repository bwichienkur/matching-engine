namespace EDDY.IS.Vendor.Entities
{
    public class ProgramCountry :Country
    {
        private int programProductId;

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
    }
}
