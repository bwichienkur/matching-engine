using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class CRInvalidProgramOverride
    {
        private int? programProductId;
        private int  clientRelationshipId;
        private int productId;

        public int? ProgramProductId
        {
            get
            {
                return programProductId;
            }

            set
            {
                programProductId = value;
            }
        }

        public int ClientRelationshipId
        {
            get
            {
                return clientRelationshipId;
            }

            set
            {
                clientRelationshipId = value;
            }
        }

        public int ProductId
        {
            get
            {
                return productId;
            }

            set
            {
                productId = value;
            }
        }
    }
}
