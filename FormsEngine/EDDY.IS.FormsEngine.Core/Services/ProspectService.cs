using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ProspectService : IProspectService
    {
        private readonly IProspectRepository _prospectRepository;

        public ProspectService(IProspectRepository prospectRepository)
        {
            _prospectRepository = prospectRepository;
        }

        public int SaveProspect(FormInput formInput)
        {
            return _prospectRepository.SaveProspect(formInput);
        }

        public void SaveProspectAsync(FormInput formInput)
        {
            _prospectRepository.SaveProspectAsync(formInput);
        }
    }
}
