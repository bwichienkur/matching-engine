using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IIPAddressService
    {
        string GetIPAddress(string HttpVia, string HttpXForwardedFor, string RemoteAddr);
    }
}
