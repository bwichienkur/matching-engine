using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class BaseMatchRequestMapper
    {
        public void MapFormInputToBaseMatchRequest(BaseMatchRequest baseMatchRequest, FormInput formInput)
        {
            MapSimpleFields(baseMatchRequest, formInput);
            MapProspect(baseMatchRequest, formInput.Prospect);
            MapCampusType(baseMatchRequest, formInput);
            MapProgramLevelList(baseMatchRequest, formInput);
        }

        private void MapSimpleFields(BaseMatchRequest baseMatchRequest, FormInput formInput)
        {
            baseMatchRequest.Application = MatchingEngine.ISApplication.FormsEngine;
            baseMatchRequest.ApplicationId = formInput.ApplicationId;
            baseMatchRequest.TrackGuid = formInput.TrackId;
            baseMatchRequest.CategoryList = formInput.Categories?.ToArray();
            baseMatchRequest.SubjectList = formInput.Subcategories?.ToArray();
            baseMatchRequest.SpecialtyList = formInput.Specialties?.ToArray();
            baseMatchRequest.IsBeta = formInput.IsBeta;
        }

        private void MapProspect(BaseMatchRequest baseMatchRequest, Prospect prospect)
        {
            var prospectInput = new ProspectInput();

            if (prospect != null)
            {
                prospectInput.Age = prospect.Age;
                prospectInput.EducationLevelId = prospect.EducationLevelId;
                prospectInput.GPAKeyValueId = prospect.GPAId;
                prospectInput.HSGraduationYear = prospect.HSGraduationYear;
                prospectInput.ExternalLeadId = prospect.ExternalLeadId;
                prospectInput.IsCitizen = prospect.IsUSCitizen;
                prospectInput.IsMilitary = prospect.IsMilitary;
                prospectInput.MilitaryStatusId = prospect.MilitaryStatusId;
                prospectInput.CountryId = prospect.CountryId;
                prospectInput.PostalCode = prospect.PostalCode;
                prospectInput.StateId = prospect.StateId;
                prospectInput.City = prospect.City;
                prospectInput.StreetAddress = prospect.Address1;
                prospectInput.AddressLine2 = prospect.Address2;
                prospectInput.Email = prospect.Email;
                prospectInput.FirstName = prospect.FirstName;
                prospectInput.LastName = prospect.LastName;
                prospectInput.Phone1 = prospect.Phone1;
                prospectInput.Phone2 = prospect.Phone2;
                prospectInput.KVCodeData = prospect.KVCodeData;
                prospectInput.YearsTeachingExperienceKeyValueId = prospect.YearsTeachingExperienceKeyValueId;
                prospectInput.YearsWorkExperienceKeyValueId = prospect.YearsWorkExperienceKeyValueId;
            }

            baseMatchRequest.ProspectInput = prospectInput;
        }

        private void MapCampusType(BaseMatchRequest baseMatchRequest, FormInput formInput)
        {
            if (!string.IsNullOrWhiteSpace(formInput.CampusPreference))
            {
                if (formInput.CampusPreference.ToLower() == "online")
                {
                    baseMatchRequest.CampusType = CampusType.Online;
                }
                else if (formInput.CampusPreference.ToLower() == "hybrid")
                {
                    baseMatchRequest.CampusType = null;
                    baseMatchRequest.IsHybrid = true;
                }
                else if (formInput.CampusPreference.ToLower() == "campus")
                {
                    baseMatchRequest.CampusType = CampusType.Ground;
                }
            }
        }

        private void MapProgramLevelList(BaseMatchRequest baseMatchRequest, FormInput formInput)
        {
            if (formInput.ProgramLevelId.HasValue)
            {
                baseMatchRequest.ProgramLevelList = new int[] { formInput.ProgramLevelId.Value };
            }
            else
            {
                baseMatchRequest.ProgramLevelList = formInput.ProgramLevelIds?.ToArray();
            }
        }

    }
}
