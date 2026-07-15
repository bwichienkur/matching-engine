using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO.Responses;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class UserSelectionService : IUserSelectionService
    {
        private readonly IUserSelectionRepository _userSelectionRepository;
        private readonly ILogoUrlFormattingService _logoUrlFormattingService;
        public UserSelectionService(IUserSelectionRepository userSelectionRepository, ILogoUrlFormattingService logoUrlFormattingService)
        {
            _userSelectionRepository = userSelectionRepository;
            _logoUrlFormattingService = logoUrlFormattingService;
        }

        public UserSelectionResponse GetUserSelectionsForSchoolPicker(FormInput formInput, IEnumerable<int> excludedInstitutionIds = null)
        {
            var userSelectionResponse = _userSelectionRepository.GetUserSelectionsForSchoolPicker(formInput, excludedInstitutionIds);
            FormatCampusLogoUrls(userSelectionResponse?.UserSelections);
            return userSelectionResponse;
        }

        private void FormatCampusLogoUrls(List<Campus> campuses)
        {
            for (int i = 0; i < campuses?.Count; i++)
            {
                Campus campus = campuses[i];
                campus.InstitutionLogoUrl = _logoUrlFormattingService.GetLargeLogoFormattedUrl(campus.InstitutionLogoUrl);
                campus.CampusLogoUrl = _logoUrlFormattingService.GetLargeLogoFormattedUrl(campus.CampusLogoUrl);
            }
        }
    }
}
