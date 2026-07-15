using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Util.Memory;
using EDDY.IS.Util.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{

    public class SubmissionDataService
    {

        public Submission SaveSubmission(Submission Entity)
        {
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Context.Submissions.Add(Entity);
                Context.SaveChanges();
            }

            return Entity;
        }


        public SubmissionDetail SaveSubmissionDetail(SubmissionDetail Entity)
        {
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Context.SubmissionDetails.Add(Entity);
                Context.SaveChanges();
            }

            return Entity;
        }


        public SubmissionDetailAdditional SaveSubmissionDetailAdditional(SubmissionDetailAdditional Entity)
        {
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Context.SubmissionDetailAdditionals.Add(Entity);
                Context.SaveChanges();
            }

            return Entity;
        }


        public SubmissionValidationError SaveSubmissionValidationError(SubmissionValidationError Entity)
        {
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Context.SubmissionValidationErrors.Add(Entity);
                Context.SaveChanges();
            }

            return Entity;
        }


        public SubmissionMatchResponse SaveSubmissionMatchResponse(SubmissionMatchResponse Entity)
        {
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Context.SubmissionMatchResponses.Add(Entity);
                Context.SaveChanges();
            }

            return Entity;
        }

        public List<VW_ProspectResubmissions> GetProspectResubmissions(List<int> submissionIds)
        {
            List<VW_ProspectResubmissions> Result = new List<VW_ProspectResubmissions>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from pr in Context.VW_ProspectResubmissions
                          where submissionIds.Contains(pr.SubmissionId)
                          select pr).Distinct().ToList();
            }

            return Result;
        }

        public EDDY_FE_GetCreativeURLs_Result GetCreativeUrls(string urlStart, Nullable<System.Guid> campaignTrackId, Nullable<int> leadCreationTypeId, string submissionFormUrl, string submissionLeadSourceUrl, string submissionLeadInitiatingUrl, string firstName, string phone, string emailAddress, string videoUrl) {

            EDDY_FE_GetCreativeURLs_Result Result = new EDDY_FE_GetCreativeURLs_Result();

            using (FEEntitiesContainer Context = new FEEntitiesContainer()) {
               Result = Context.EDDY_FE_GetCreativeURLs(urlStart, campaignTrackId, leadCreationTypeId, submissionFormUrl, 
                   submissionLeadSourceUrl, submissionLeadInitiatingUrl,firstName,phone,emailAddress, videoUrl).FirstOrDefault();
            }
            if (String.IsNullOrEmpty(Result.InitiatingURL)) Result.InitiatingURL = "";
            if (String.IsNullOrEmpty(Result.LandingURL)) Result.LandingURL = "";
            if (String.IsNullOrEmpty(Result.CallCenterURL)) Result.CallCenterURL = "";

            return Result;
        }

    }
}
