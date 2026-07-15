using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.ProspectService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class ProspectRepository : IProspectRepository
    {
        private readonly SaveProspectRequestMapper _saveProspectRequestMapper;
        private readonly ProspectServiceClient _prospectServiceClient;

        public ProspectRepository()
        {
            _saveProspectRequestMapper = new SaveProspectRequestMapper();
            _prospectServiceClient = new ProspectServiceClient();
        }

        public int SaveProspect(FormInput formInput)
        {
            int prospectId = 0;

            SaveProspectRequest request = _saveProspectRequestMapper.MapFormInputToSaveProspectRequest(formInput);

            try
            {
                SaveProspectResponse response = _prospectServiceClient.SaveProspect(request);
                prospectId = response?.ProspectId ?? 0;
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, new Exception("Exception Thrown from Prospect Service", ex)).Save();
            }

            return prospectId;
        }

        public void SaveProspectAsync(FormInput formInput)
        {
            var mapper = new SaveProspectRequestMapper();
            SaveProspectRequest request = mapper.MapFormInputToSaveProspectRequest(formInput);

            try
            {
                Task.Run(() => _prospectServiceClient.SaveProspect(request));
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, new Exception("Exception Thrown from Prospect Service", ex)).Save();
            }
        }
    }
}
