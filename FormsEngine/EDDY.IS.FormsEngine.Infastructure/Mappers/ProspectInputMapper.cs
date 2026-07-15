using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class ProspectInputMapper
    {
        public ProspectInput MapProspectToProspectInput(Prospect prospect)
        {
            var prospectInput = new ProspectInput();
            prospectInput.StreetAddress = prospect.Address1;
            prospectInput.AddressLine2 = prospect.Address2;
            prospectInput.City = prospect.City;
            prospectInput.CountryId = prospect.CountryId;
            prospectInput.StateId = prospect.StateId;
            prospectInput.Age = prospect.Age;
            prospectInput.EducationLevelId = prospect.EducationLevelId;
            prospectInput.Email = prospect.Email;
            prospectInput.ExternalLeadId = prospect.ExternalLeadId;
            prospectInput.FirstName = prospect.FirstName;
            prospectInput.LastName = prospect.LastName;
            prospectInput.GPAKeyValueId = prospect.GPAId;
            prospectInput.HSGraduationYear = prospect.HSGraduationYear;
            prospectInput.IsCitizen = prospect.IsUSCitizen;
            prospectInput.IsMilitary = prospect.IsMilitary;
            prospectInput.KVCodeData = prospect.KVCodeData;
            prospectInput.MilitaryStatusId = prospect.MilitaryStatusId;
            prospectInput.Phone1 = prospect.Phone1;
            prospectInput.Phone2 = prospect.Phone2;
            prospectInput.PostalCode = prospect.PostalCode;
            prospectInput.YearsTeachingExperienceKeyValueId = prospect.YearsTeachingExperienceKeyValueId;
            prospectInput.YearsWorkExperienceKeyValueId = prospect.YearsWorkExperienceKeyValueId;
            return prospectInput;
        }
    }
}
