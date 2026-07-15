using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using EDDY.IS.SEOAllocation.Console.Data;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Model
{
    public class DrupalNodeProcessStep : IAllocationProcessStep
    {
        protected int NodeId { get; set; }
        protected gsNode Node { get; set; }

        public DrupalNodeProcessStep(int nodeID, gsNode node)
        {
            NodeId = nodeID;
            Node = node;
        }

        public void DoProcessStep(AllocationWorkingData workingData, IEddyLoggingDataService loggingDataService, IEddyTrackingDataService trackingDataService)
        {
            try
            {
                GSAllocationMaster allocationMaster = new GSAllocationMaster();
                allocationMaster.NodeId = NodeId;
                allocationMaster.URL = Node.url;
                allocationMaster.RunTime = DateTime.Now;

                DirectoryMatchRequest request = new DirectoryMatchRequest();
                //From GS Site
                //Hybrid vs Online vs Ground
                if (Node.campus_type != null)
                {
                    if(Node.campus_type.FirstOrDefault() == 1)
                    {
                        request.IsHybrid = false;
                        request.CampusType = CampusType.Online;
                    }
                    if (Node.campus_type.FirstOrDefault() == 2)
                    {
                        request.IsHybrid = false;
                        request.CampusType = CampusType.Ground;
                    }
                    if (Node.campus_type.FirstOrDefault() == 3)
                    {
                        request.IsHybrid = true;                        
                    }
                }                
                request.TrackGuid = Guid.Parse(ConfigurationManager.AppSettings["GS_CampaignTrackId"]);
                request.SortMethod = EntitySortMethod.Alphabetical;
                request.ApplicationId = 7;
                request.ProgramLevelList = new List<int>(Node.level.ToList());
                request.CategoryList = new List<int>(Node.category.ToList());
                request.SubjectList = new List<int>(Node.subject.ToList());
                request.SpecialtyList = new List<int>(Node.specialty.ToList());

                //Added
                request.ProgramTypeList = new List<ProgramType>() { ProgramType.Certificates, ProgramType.FullDegree };
                request.MaxNestedProgramCount = 25;
                request.IncludeProgramGroupRollup = true;

                //Get node institution response
                IMatchingService matchingService = new EDDY.IS.MatchingEngine.Service.MatchingService();
                InstitutionResponse institutionResponse = matchingService.GetInstitutions(request, GetInstitutionCampusOption.CampusOn2ndLevel);

                //Next step
                IAllocationProcessStep nextStep = new CreateAllocationDetailsStep(institutionResponse, allocationMaster, Node);
                nextStep.DoProcessStep(workingData, loggingDataService, trackingDataService);
            }
            catch (System.Exception ex)
            {
                loggingDataService.LogException(ex);
            }            
        }
    }
}
