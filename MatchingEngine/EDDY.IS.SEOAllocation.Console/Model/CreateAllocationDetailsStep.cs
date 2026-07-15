using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.DTO;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.SEOAllocation.Console.Data;
using EDDY.IS.MatchingEngine;
using EDDY.IS.SEOAllocation.Console.Enum;

namespace EDDY.IS.SEOAllocation.Console.Model
{
    public class CreateAllocationDetailsStep : IAllocationProcessStep
    {
        protected InstitutionResponse InstitutionResponse { get; set; }
        protected GSAllocationMaster AllocationMaster { get; set; }
        protected gsNode Node { get; set; }


        public CreateAllocationDetailsStep(InstitutionResponse response, GSAllocationMaster allocationMaster, gsNode node)
        {
            InstitutionResponse = response;
            AllocationMaster = allocationMaster;
            Node = node;
        }

        public void DoProcessStep(AllocationWorkingData workingData, IEddyLoggingDataService loggingDataService, IEddyTrackingDataService trackingDataService)
        {
            try
            {
                if (InstitutionResponse != null && InstitutionResponse.InstitutionList.Any())
                {
                    foreach (InstitutionWithCampus institution in InstitutionResponse.InstitutionList)
                    {
                        foreach (Campus campus in institution.CampusList)
                        {
                            foreach (MatchingEngine.DTO.Program program in campus.ProgramList)
                            {
                                //Only recording information on paid or fraid programs
                                if (program.PaidStatusTypeId != PaidStatusType.Free)
                                {
                                    GSAllocationDetail detail = new GSAllocationDetail();
                                    detail.ProductId = program.ProductId;
                                    detail.ProgramId = program.ProgramId;
                                    detail.ProgramLevelId = program.ProgramLevelId;

                                    detail.ProgramProductId = program.ProgramProductId;
                                    detail.ProgramCampusTypeId = (int)program.ProgramCampusType;
                                    detail.InstitutionId = institution.InstitutionId;
                                    detail.ClickLeadProgramTypeId = (int)GetProgramClickType(program);
                                    detail.PaidStatusTypeId = (int)program.PaidStatusTypeId;

                                    if (workingData.eRPLList.ContainsKey(detail.ProgramProductId))
                                    {
                                        eRPL rpl = workingData.eRPLList[detail.ProgramProductId];
                                        detail.RPL = decimal.Round(rpl.eRPLFinal, 4);
                                        detail.ClickPrice = decimal.Round(rpl.ClickPrice, 4);
                                    }
                                    else
                                    {
                                        detail.RPL = 0;
                                        detail.ClickPrice = 0;
                                    }

                                    AllocationMaster.GSAllocationDetails.Add(detail);
                                }
                                else
                                {
                                    //Still counting free programs
                                    AllocationMaster.FreeProgramCount++;
                                }
                            }
                        }
                    }

                    //Get master detail record count                
                    AllocationMaster.InstitutionCount = InstitutionResponse.InstitutionList.GroupBy(x => x.InstitutionId).Select(g => new { InstitutionId = g.Key }).Count();
                    AllocationMaster.PaidClickProgramCount = AllocationMaster.GSAllocationDetails.Where(x => x.ClickLeadProgramTypeId == (int)EnumProgramClickLeadType.Click).Count();
                    AllocationMaster.PaidLeadProgramCount = AllocationMaster.GSAllocationDetails.Where(x => x.ClickLeadProgramTypeId == (int)EnumProgramClickLeadType.Lead).Count();
                    AllocationMaster.PaidClickLeadProgramCount = AllocationMaster.GSAllocationDetails.Where(x => x.ClickLeadProgramTypeId == (int)EnumProgramClickLeadType.Both).Count();
                    AllocationMaster.PaidProgramCount = AllocationMaster.GSAllocationDetails.Where(x => x.PaidStatusTypeId == 3).Count();
                    AllocationMaster.FraidProgramCount = AllocationMaster.GSAllocationDetails.Where(x => x.PaidStatusTypeId == 2).Count();
                    AllocationMaster.ProgramCount = AllocationMaster.PaidProgramCount + AllocationMaster.FraidProgramCount + AllocationMaster.FreeProgramCount;
                    AllocationMaster.RPLSum = decimal.Round(AllocationMaster.GSAllocationDetails.Sum(x => x.RPL), 4);
                    AllocationMaster.ClickPriceSum = AllocationMaster.GSAllocationDetails.Sum(x => x.ClickPrice);
                }
                else
                {
                    //Get master detail record count                
                    AllocationMaster.InstitutionCount = 0;
                    AllocationMaster.PaidClickProgramCount = 0;
                    AllocationMaster.PaidLeadProgramCount = 0;
                    AllocationMaster.PaidProgramCount = 0;
                    AllocationMaster.FraidProgramCount = 0;
                    AllocationMaster.ProgramCount = 0;
                    AllocationMaster.RPLSum = 0;
                    AllocationMaster.ClickPriceSum = 0;
                }
                
                //Next step
                IAllocationProcessStep nextStep = new RecordAllocationDetailsStep(AllocationMaster);
                nextStep.DoProcessStep(workingData, loggingDataService, trackingDataService);
            }
            catch (System.Exception ex)
            {
                loggingDataService.LogException(ex);
            }            
        }

        public EnumProgramClickLeadType GetProgramClickType(MatchingEngine.DTO.Program program)
        {
            if (String.IsNullOrEmpty(program.ClickThroughUrl) && (program.InquiryDisabled != null) && (bool)program.InquiryDisabled)
                return EnumProgramClickLeadType.None;
            else if (!String.IsNullOrEmpty(program.ClickThroughUrl) && (program.InquiryDisabled != null) && (bool)program.InquiryDisabled)
                return EnumProgramClickLeadType.Click;
            else if (String.IsNullOrEmpty(program.ClickThroughUrl) && (program.InquiryDisabled != null) && !(bool)program.InquiryDisabled)
                return EnumProgramClickLeadType.Lead;
            else if (!String.IsNullOrEmpty(program.ClickThroughUrl) && (program.InquiryDisabled != null) && !(bool)program.InquiryDisabled)
                return EnumProgramClickLeadType.Both;
            else
                return EnumProgramClickLeadType.None;
        }
    }
}
