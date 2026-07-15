using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.Mapper;
using System.Data.SqlClient;

namespace EDDY.IS.LeadEngine.DataModel
{
    public class LeadDataService
    {
        const string LEAD_UPDATE_PROD_SP = "EXEC Prod.EDDY_FE_Lead_Update @LeadId, @CampaignTrackId, @SubmissionId, @ProgramProductId, @RawPostDataId,@DateEntered, @ProductIdOut OUTPUT";
        const string LEAD_UPDATE_BETA_SP = "EXEC dbo.EDDY_FE_Lead_Update @LeadId, @CampaignTrackId, @SubmissionId, @ProgramProductId, @RawPostDataId,@DateEntered, @ProductIdOut OUTPUT";

        const string LEAD_CREATIVE_PROD_SP = "EXEC dbo.EDDY_FE_Lead_CreativeInsert @LeadId, @UrlStart, @SubmissionId";
        const string LEAD_CREATIVE_BETA_SP = "EXEC dbo.EDDY_FE_Lead_CreativeInsert_Beta @LeadId, @UrlStart, @SubmissionId";

        public LeadDTO SaveLead(LeadDTO Entity)
        {
            Lead l = Converter.ConvertLeadToDataModel(Entity);
            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                l.BillingDate = DateTime.Now;
                Context.Leads.Add(l);
                Context.SaveChanges();
            }

            return Converter.ConvertToLeadDTO(l);
        }

        public void SaveLeadCreative(int leadId, bool isBeta, int submissionId)
        {
            //EDDY_FE_Lead_CreativeInsert 17937045, 'http://ceco-issvc1/EDDY.IS.CompliancePortal' 
            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                object[] spParams = new object[3];

                spParams[0] = new SqlParameter("@LeadId", leadId);
                spParams[1] = new SqlParameter("@UrlStart", ConfigurationManager.AppSettings.Get("CreativePortalUrl"));
                spParams[2] = new SqlParameter("@SubmissionId", submissionId);

                if (isBeta)
                    Context.Database.ExecuteSqlCommand(LEAD_CREATIVE_BETA_SP, spParams);
                else
                    Context.Database.ExecuteSqlCommand(LEAD_CREATIVE_PROD_SP, spParams);
            }
        }

        public int UpdateLead(int leadId, Guid campaignTrackId, int? submissionId, int programProductId, long rawPostDataId, DateTime? DateEntered, bool isBeta)
        {
            int productId = 0;

            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                var output = new SqlParameter("@ProductIdOut", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };

                object[] spParams = new object[7];

                spParams[0] = new SqlParameter("@LeadId", leadId);
                spParams[1] = new SqlParameter("@CampaignTrackId", campaignTrackId);
                spParams[2] = new SqlParameter("@SubmissionId", submissionId);
                spParams[3] = new SqlParameter("@ProgramProductId", programProductId);
                //spParams[4] = new SqlParameter("@ApplicationId", ApplicationId);
                spParams[4] = new SqlParameter("@RawPostDataId", rawPostDataId);
               
                //Update created to EDUMax Created Date
                if (DateEntered.HasValue)
                {
                    spParams[5] = new SqlParameter("@DateEntered", DateEntered.Value.ToString());
                }
                else {
                    spParams[5] = new SqlParameter("@DateEntered", DBNull.Value);
                }
                spParams[6] = output;
                if (isBeta)
                    Context.Database.ExecuteSqlCommand(LEAD_UPDATE_BETA_SP, spParams);
                else
                    Context.Database.ExecuteSqlCommand(LEAD_UPDATE_PROD_SP, spParams);
                    

                productId = output.Value as int? ?? 0;
            }

            return productId;
        }

        /// <summary>
        /// this method primarily used for the unit test to assert submission id and rawpostdata id which are updated by
        /// stored proc after lead save.
        /// </summary>
        /// <param name="LeadId"></param>
        /// <returns></returns>
        public Lead GetLead(int LeadId)
        {
            Lead l = null;

            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                l = Context.Leads.Where(Lead => Lead.LeadId == LeadId).FirstOrDefault();
            }

            return l;
        }

        public LeadDTO UpdateLead(LeadDTO Entity)
        {
            Lead l = null;

            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                l = Context.Leads.Where(Lead => Lead.LeadId == Entity.LeadId).FirstOrDefault();

                l.Address1 = Entity.Address1;
                l.Age = Entity.Age;
                l.City = Entity.City;
                l.ClientRelationContactId = Entity.ClientRelationContactId;
                l.CountryCode = Entity.CountryCode;
                l.EmailAddress = Entity.EmailAddress;
                l.FirstName = Entity.FirstName;
                l.HighestLevelOfEdu = Entity.HighestLevelOfEdu;
                l.LastName = Entity.LastName;
                l.LeadCreationTypeId = Entity.LeadCreationTypeId;
                l.Military = Entity.Military;
                l.Phone1 = Entity.Phone1;
                l.Phone2 = Entity.Phone2;
                l.ProgramProductId = Entity.ProgramProductId;
                l.ProspectId = Entity.ProspectId;
                l.RealtimeDeliveryStatusId = (int)LeadEngine.RealtimeDeliveryStatusValue.NEW;
                l.StateProvince = Entity.StateProvince;
                l.YearHighestEduCompleted = Entity.YearHighestEduCompleted;
                l.ZipCode = Entity.ZipCode;
                //For ensuring billing happens for leads released the following month. For Warm Transfer Titanium
                l.BillingDate = DateTime.Now;
      
               


                Context.SaveChanges();
            }

            UpdateLead(Convert.ToInt32(Entity.LeadId), l.TrackId.Value, l.SubmissionId, l.ProgramProductId.Value, l.RawPostDataId,null, false);

            return Converter.ConvertToLeadDTO(l);
        }
    }


}
