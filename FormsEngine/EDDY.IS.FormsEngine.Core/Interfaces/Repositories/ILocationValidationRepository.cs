using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces.Repositories
{
    public interface ILocationValidationRepository
    {
        Tuple<int?, int?> GetValidCountryIdAndStateId(string countryCode, string stateCode);
        string GetPostalCode(string ipAddress);
    }
}
