using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Entities
{
	[DataContract]
	public class ProgramMicrosite
	{
		private int programId;
		private string programName;
		private int? optionGroupDisplayOrder;
		private string optionGroupDisplayValue;
		private string categoryName;
		private string subjectName;
		private int programLevelId;
		private string programDelimitedValue;
		private string programLevelName;
		public List<CampusMicrosite> campusList;
        public List<string> startDateList;
        public List<ProgramTag> tagList;

        [DataMember(Order = 0)]
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

		[DataMember(Order = 1)]
		public string ProgramDisplayValue
		{
			get
			{
				return programName;
			}

			set
			{
				programName = value;
			}
		}

		[DataMember(Order = 2)]
		public int? OptionGroupDisplayOrder
		{
			get
			{
				return optionGroupDisplayOrder;
			}
			set
			{
				optionGroupDisplayOrder = value;
			}
		}

		[DataMember(Order = 3)]
		public string OptionGroupDisplayValue
		{
			get
			{
				return optionGroupDisplayValue;
			}

			set
			{
				optionGroupDisplayValue = value;
			}
		}

		[DataMember(Order = 4)]
		public string CategoryName
		{
			get
			{
				return categoryName;
			}

			set
			{
				categoryName = value;
			}
		}

		[DataMember(Order = 5)]
		public string SubjectName
		{
			get
			{
				return subjectName;
			}

			set
			{
				subjectName = value;
			}
		}

		[DataMember(Order = 6)]
		public int ProgramLevelId
		{
			get
			{
				return programLevelId;
			}

			set
			{
				programLevelId = value;
			}
		}

		[DataMember(Order = 7)]
		public string ProgramLevelName
		{
			get
			{
				return programLevelName;
			}

			set
			{
				programLevelName = value;
			}
		}
		[DataMember(Order = 8)]
		public string ProgramDelimitedValue
		{
			get
			{
				return programDelimitedValue;
			}

			set
			{
				programDelimitedValue = value;
			}
		}
		[DataMember(Order = 9)]
		public List<CampusMicrosite> CampusList
		{
			get
			{
				return campusList;
			}

			set
			{
				campusList = value;
			}
		}

        [DataMember(Order = 10)]
        public List<string> StartDateList
        {
            get
            { 
				return startDateList; 
			}

            set 
			{ 
				startDateList = value; 
			}
        }

        [DataMember(Order = 11)]
        public List<ProgramTag> TagList
        {
            get
            {
                return tagList;
            }

            set
            {
                tagList = value;
            }
        }
    }
}
