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
    public class LocationValidationService : ILocationValidationService
    {
        private readonly ILocationValidationRepository _locationValidationRepository;
        
        public LocationValidationService(ILocationValidationRepository locationValidationRepository)
        {
            _locationValidationRepository = locationValidationRepository;
        }

        public Location GetValidLocation(string countryCode, string stateCode)
        {
            Tuple<int?, int?> countryIdAndStateId = _locationValidationRepository.GetValidCountryIdAndStateId(countryCode, stateCode);

            return new Location
            {
                CountryId = countryIdAndStateId.Item1,
                StateId = countryIdAndStateId.Item2
            };
        }

        public string GetPostalCode(string ipAddress)
        {
            var postalCode = _locationValidationRepository.GetPostalCode(ipAddress);
            return postalCode ?? string.Empty;
        }
    }
}
