using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class RawPostDataMapper
    {
        public RawPostDataDTO MapFormInputToRawPostData(FormInput formInput)
        {
            return new RawPostDataDTO
            {
                RemoteIp = formInput.IpAddress,
                BrowserInfo = formInput.UserAgent,
                Referer = formInput.HttpReferer,
                PostData = formInput.LeadData
            };
        }
    }
}
