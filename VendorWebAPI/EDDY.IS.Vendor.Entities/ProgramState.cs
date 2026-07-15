namespace EDDY.IS.Vendor.Entities
{
    public class ProgramState : State
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
