using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class SponsoredListingModel
    {
        public bool IsEms { get; set; }
        public SponsoredListingModel(FormEngineModel formEngineModel)
        {
            if (formEngineModel != null)
            {
                if (formEngineModel.Template != null)
                {
                    this.IsEms = (formEngineModel.Template.BusinessDivisionId == 2);
                }
            }
        }
        public SponsoredListingModel(ManagedChoiceModel managedChoiceModel)
        {
            if (managedChoiceModel != null)
            {
                if (managedChoiceModel.ApplicationId.HasValue)
                {
                    this.IsEms = (managedChoiceModel.ApplicationId.Value == 27);
                }
            }
        }

    }
}