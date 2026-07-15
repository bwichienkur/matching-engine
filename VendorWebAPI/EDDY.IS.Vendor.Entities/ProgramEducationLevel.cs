using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class ProgramEducationLevel
    {
        private int programId;
        private int educationLevelId;
        private string educationLevelName;
        [JsonIgnore]
        public int ProgramId
        {
            get
            {
                return programId;
            }

            set
            {
                programId = value;
            }
        }

        public int EducationLevelId
        {
            get
            {
                return educationLevelId;
            }

            set
            {
                educationLevelId = value;
            }
        }

        public string EducationLevelName
        {
            get
            {
                return educationLevelName;
            }

            set
            {
                educationLevelName = value;
            }
        }
    }
}
