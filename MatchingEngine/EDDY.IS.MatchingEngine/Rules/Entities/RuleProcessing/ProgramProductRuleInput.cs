using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class ProgramProductRuleInput : EntityRemovalReason
    {
        public int ClientRelationProductMappingId { get; set; }
        public int ClientCampusRelationshipId { get; set; }
        public int ProgramProductId { get; set; }
        public int PsiId { get; set; }
        public int ProgramId { get; set; }
        public int ProductId { get; set; }
        public int InstitutionId { get; set; }
        public int ClientRelationshipId { get; set; }

        public int ProgramLevelId { get; set; }

        //Geo Data
        public bool IncludeAllZipCodes { get; set; }
        public bool IsZipCodeInclusionExclusionActive { get; set; }
        public int AllowableRadius { get; set; }
        public string RadiusZipCode { get; set; }
        public bool IsZipCodeInclusion { get; set; }
        public bool IsZipCodeExclusion { get; set; }

        public int Key 
        {
            get
            {
                //if (!String.IsNullOrEmpty(_key))
                //    return _key;
                //else if (ProgramId > 0 && ProductId > 0)
                //    _key = ProgramId + "_" + ProductId;

                //return _key;
                return ProgramProductId;
            }
        }

        public ProgramProductRuleInput(int programId, int productId)
        {
            ProgramId = programId;
            ProductId = productId;
        }
        
    }
}
