using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.ValidationService;
using EDDY.IS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class LocationValidationRepository : ILocationValidationRepository
    {
        private readonly ValidationServiceClient _validationServiceProd;

        public LocationValidationRepository()
        {
            _validationServiceProd = new ValidationServiceClient("BasicHttpBinding_IValidationService");
        }

        public Tuple<int?, int?> GetValidCountryIdAndStateId(string countryCode, string stateCode)
        {
            var validationEngine = new ValidationEngine();
            Tuple<int?, int?> countryIdAndStateId = validationEngine.GetCountryIdStateId(countryCode, stateCode);
            return countryIdAndStateId;
        }

        public string GetPostalCode(string ipAddress)
        {
            var location = _validationServiceProd.GetLocationByIP(ipAddress);
            return location?.PostalCode ?? string.Empty;
        }
    }
}
