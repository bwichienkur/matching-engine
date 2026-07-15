using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class InstitutionService : IInstitutionService
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly ILogoUrlFormattingService _logoUrlFormattingService;

        public InstitutionService(IInstitutionRepository institutionRepository, ILogoUrlFormattingService logoUrlFormattingService)
        {
            _institutionRepository = institutionRepository;
            _logoUrlFormattingService = logoUrlFormattingService;
        }

        public Institution GetInstitution(FormRequest formRequest)
        {
            Institution institution = _institutionRepository.GetInstitution(formRequest);
            FormatInstitutionLogoUrl(institution);
            return institution ?? new Institution();
        }

        public List<Institution> GetInstitutions(FormInput formInput)
        {
            List<Institution> institutions = _institutionRepository.GetInstitutions(formInput) ?? new List<Institution>();
            FormatInstitutionLogoUrls(institutions);
            return institutions;
        }
        
        private void FormatInstitutionLogoUrls(List<Institution> institutions)
        {
            foreach (Institution institution in institutions)
            {
                FormatInstitutionLogoUrl(institution);
            }
        }

        private void FormatInstitutionLogoUrl(Institution institution)
        {
            if (institution != null)
                institution.InstitutionLogoUrl = _logoUrlFormattingService.GetLargeLogoFormattedUrl(institution.InstitutionLogoUrl);
        }

    }
}
