using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.LeadEngine.DataModel
{
    class RawPostDataService
    {
        public RawPostDataDTO SaveRawPostData(RawPostDataDTO Request)
        {
            RawPostData rpd = Converter.ConvertToRPDDataModel(Request);
            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                Context.RawPostDatas.Add(rpd);
                Context.SaveChanges();
            }

            return Converter.ConvertRPDToDTO(rpd);
        }
    }
}
