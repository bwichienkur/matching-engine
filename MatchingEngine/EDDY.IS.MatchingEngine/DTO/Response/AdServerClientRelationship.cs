using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class AdServerClientRelationship
    {
        [DataMember]
        public int ClientRelationshipId { get; set; }

        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public List<AdServerPsi> PsiList { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }
    }

    [DataContract]
    public class AdServerPsi
    {
        [DataMember]
        public int PsiId { get; set; }

        [DataMember]
        public List<AdServerClickProgramProduct> ClickProgramProductList { get; set; }

        public AdServerPsi()
        {
            ClickProgramProductList = new List<AdServerClickProgramProduct>();
        }
    }

    [DataContract]
    public class AdServerClickProgramProduct
    {
        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public int ProgramId { get; set; }

        [DataMember]
        public decimal ClickPrice { get; set; }

        [DataMember]
        public string ClickThroughURL { get; set; }
    }

}
