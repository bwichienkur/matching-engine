namespace EDDY.IS.Vendor.Entities
{
    public class Level : LevelBase
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }
    }
}
