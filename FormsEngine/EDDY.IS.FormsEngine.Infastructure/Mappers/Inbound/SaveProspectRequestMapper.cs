using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.ProspectService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class SaveProspectRequestMapper
    {
        public SaveProspectRequest MapFormInputToSaveProspectRequest(FormInput formInput)
        {
            var saveProspectRequest = new SaveProspectRequest();

            if (formInput?.Prospect != null)
            {
                MapFormInputToSaveProspectRequestProspectFlowDetails(formInput, saveProspectRequest);
                MapFormInputProspectToSaveProspectRequestProspect(formInput.Prospect, saveProspectRequest);
            }

            return saveProspectRequest;
        }

        private void MapFormInputProspectToSaveProspectRequestProspect(Prospect prospect, SaveProspectRequest saveProspectRequest)
        {
            saveProspectRequest.Prospect = new ProspectDTO();
            saveProspectRequest.Prospect.FirstName = prospect.FirstName;
            saveProspectRequest.Prospect.LastName = prospect.LastName;
            saveProspectRequest.Prospect.Email = string.IsNullOrWhiteSpace(prospect.Email) ? null : prospect.Email;
            saveProspectRequest.Prospect.Address1 = prospect.Address1;
            saveProspectRequest.Prospect.Address2 = prospect.Address2;
            saveProspectRequest.Prospect.Age = prospect.Age;
            saveProspectRequest.Prospect.Phone = prospect.Phone1;
            saveProspectRequest.Prospect.OtherPhone = prospect.Phone2;
            saveProspectRequest.Prospect.City = prospect.City;
            saveProspectRequest.Prospect.CountryID = prospect.CountryId;
            saveProspectRequest.Prospect.StateID = prospect.StateId;
            saveProspectRequest.Prospect.EducationLevelID = prospect.EducationLevelId;
            saveProspectRequest.Prospect.GraduationYear = prospect.HSGraduationYear ?? prospect.GenericGraduationYear;
            saveProspectRequest.Prospect.MilitaryStatusID = prospect.MilitaryStatusId;
            saveProspectRequest.Prospect.PostalCode = prospect.PostalCode;
            saveProspectRequest.Prospect.PreferEmail = prospect.PreferEmail;
            saveProspectRequest.Prospect.PreferPhone = prospect.PreferPhone;
            saveProspectRequest.Prospect.PreferText = prospect.PreferText;
            saveProspectRequest.Prospect.IsUsCitizen = prospect.IsUSCitizen;
            saveProspectRequest.Prospect.NeedsFinancialAid = prospect.NeedsFinancialAid;
            saveProspectRequest.Prospect.DesiredStartDate = string.IsNullOrWhiteSpace(prospect.DesiredStartDate) ? null : prospect.DesiredStartDate;
        }

        private void MapFormInputToSaveProspectRequestProspectFlowDetails(FormInput formInput, SaveProspectRequest saveProspectRequest)
        {
            saveProspectRequest.ProspectFlowDetails = new ProspectFlowDetailsDTO();
            saveProspectRequest.ProspectFlowDetails.TrackId = formInput.TrackId;
            saveProspectRequest.ProspectFlowDetails.SessionGuid = Guid.TryParse(formInput.SessionId, out Guid sessionGuid) ? sessionGuid : (Guid?)null;
            saveProspectRequest.ProspectFlowDetails.ProspectFlowTypeId = formInput.ProspectFlowTypeId;

            if (formInput.ProspectFlowTypeId == (int)ProspectFlowTypes.Advising)
            {
                saveProspectRequest.ProspectFlowDetails.AreaofInterest = string.Join(",", formInput.Categories);
            }
        }
    }
}
