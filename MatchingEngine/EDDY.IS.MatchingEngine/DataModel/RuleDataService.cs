using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class RuleDataService
    {
        //public static List<VW_Matching_ClickProgramProduct> GetAllClickProgramProducts()
        //{
        //    List<DataModel.Entity.VW_Matching_ClickProgramProduct> zList = new List<VW_Matching_ClickProgramProduct>();

        //    SqlDataReader dr = null;

        //    try
        //    {
        //        dr = DataHelper.GetDataReader("VW_Matching_ClickProgramProduct");
        
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                zList.Add(new Entity.VW_Matching_ClickProgramProduct(dr));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ISException isEx = new ISException(ex);
        //        isEx.Save();
        //    }
        //    finally
        //    {
        //        if (dr != null)
        //        {
        //            dr.Close();
        //            dr.Dispose();
        //        }
        //    }

        //    return zList;
        //}

        public static Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> GetDuplicateLeadsByProgramProduct(string email)
        {
            Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> dupes = new Dictionary<int, List<VW_Matching_EMSDuplicateInfo>>();
            SqlDataReader dr = null;

            try
            {
                if (string.IsNullOrEmpty(email))
                    return dupes;

                dr = DataHelper.GetDataReaderFromSp(email);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_EMSDuplicateInfo dupe = new VW_Matching_EMSDuplicateInfo(dr);

                        if (dupes.ContainsKey(dupe.ProgramProductId))
                            dupes[dupe.ProgramProductId].Add(dupe);
                        else
                            dupes.Add(dupe.ProgramProductId, new List<VW_Matching_EMSDuplicateInfo> { dupe });
                    }
                }
            }
            catch (Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return dupes;
        }

        public static Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> GetDuplicateLeadsByClientRelationship(string email)
        {
            Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> dupes = new Dictionary<int, List<VW_Matching_EMSDuplicateInfo>>();
            SqlDataReader dr = null;

            try
            {
                if (string.IsNullOrEmpty(email))
                    return dupes;

                dr = DataHelper.GetDataReaderFromSp(email);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_EMSDuplicateInfo dupe = new VW_Matching_EMSDuplicateInfo(dr);

                        if (dupes.ContainsKey(dupe.ClientRelationshipId))
                            dupes[dupe.ClientRelationshipId].Add(dupe);
                        else
                            dupes.Add(dupe.ClientRelationshipId, new List<VW_Matching_EMSDuplicateInfo> { dupe });
                    }
                }
            }
            catch (Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return dupes;
        }

        public static Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>> GetClientRelationshipSchedules()
        {
            Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>> zList = new Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>>();
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationshipSchedule");

                if(dr.HasRows)
                {
                    while(dr.Read())
                    {
                        VW_Matching_ClientRelationshipSchedule schedule = new VW_Matching_ClientRelationshipSchedule(dr);

                        if (zList.ContainsKey(schedule.ClientRelationshipId))
                            zList[schedule.ClientRelationshipId].Add(schedule);
                        else
                            zList.Add(schedule.ClientRelationshipId, new List<VW_Matching_ClientRelationshipSchedule> { schedule });
                    }
                }
            }
            catch (Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>> GetClientRelationshipEntitySchedules()
        {
            Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>> zList = new Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>>();
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationshipEntitySchedule");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_ClientRelationshipEntitySchedule schedule = new VW_Matching_ClientRelationshipEntitySchedule(dr);

                        if (zList.ContainsKey(schedule.ClientRelationProductAutomationId))
                            zList[schedule.ClientRelationProductAutomationId].Add(schedule);
                        else
                            zList.Add(schedule.ClientRelationProductAutomationId, new List<VW_Matching_ClientRelationshipEntitySchedule> { schedule });
                    }
                }
            }
            catch (Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ZipCodeCache> GetZipCodesByZipCodeId(int zipCodeId)
        {
            List<DataModel.Entity.VW_Matching_ZipCodeCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ZipCodeCache", "zipCodeId = " + zipCodeId);

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ZipCodeCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ZipCodeCache(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }
        public static List<VW_Matching_ClientRelationAgentDisallowedLiveTransfer> GetAgentDisallowedList()
        {
            List<VW_Matching_ClientRelationAgentDisallowedLiveTransfer> zList = null;
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationAgentDisallowedLiveTransfer");

                if (dr.HasRows)
                {
                    zList = new List<VW_Matching_ClientRelationAgentDisallowedLiveTransfer>();

                    while (dr.Read())
                        zList.Add(new VW_Matching_ClientRelationAgentDisallowedLiveTransfer(dr));
                }
            }
            catch (Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ClientRelationshipChannelCaps> GetClientRelationshipChannelCaps()
        {
            List<VW_Matching_ClientRelationshipChannelCaps> zList = null;
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationshipChannelCaps");

                if(dr.HasRows)
                {
                    zList = new List<VW_Matching_ClientRelationshipChannelCaps>();

                    while (dr.Read())
                        zList.Add(new VW_Matching_ClientRelationshipChannelCaps(dr));
                }
            }
            catch(Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if(dr !=null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CapDistribution_NormalizationOverride> GetAllCapNormalizationOverrides()
        {
            List<VW_Matching_CapDistribution_NormalizationOverride> zList = null;
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CapDistribution_NormalizationOverride");

                if(dr.HasRows)
                {
                    zList = new List<VW_Matching_CapDistribution_NormalizationOverride>();

                    while (dr.Read())
                        zList.Add(new VW_Matching_CapDistribution_NormalizationOverride(dr));
                }
            }
            catch(Exception e)
            {
                ISException isEx = new ISException(e);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CampusZipCode> GetAllCampusZipCodes()
        {
            List<DataModel.Entity.VW_Matching_CampusZipCode> zList = new List<Entity.VW_Matching_CampusZipCode>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampusZipCode");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CampusZipCode(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_InstitutionGroup> GetAllInstitutionGroupProd()
        {
            List<DataModel.Entity.VW_Matching_InstitutionGroup> zList = new List<Entity.VW_Matching_InstitutionGroup>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_InstitutionGroup_Prod");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_InstitutionGroup(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        //public static List<VW_Matching_ProgramProductTemplateAssignment> GetAllProgramProductTemplateAssignmentProd()
        //{
        //    List<DataModel.Entity.VW_Matching_ProgramProductTemplateAssignment> zList = null;

        //    SqlDataReader dr = null;
            
        //    try
        //    {
        //        dr = DataHelper.GetDataReader("Prod.VW_Matching_ProgramProductTemplateAssignment_Prod");

        //        if (dr.HasRows)
        //        {
        //            zList = new List<Entity.VW_Matching_ProgramProductTemplateAssignment>();

        //            while (dr.Read())
        //            {
        //                zList.Add(new Entity.VW_Matching_ProgramProductTemplateAssignment(dr));
        //    }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ISException isEx = new ISException(ex);
        //        isEx.Save();
        //    }
        //    finally
        //    {
        //        if (dr != null)
        //        {
        //            dr.Close();
        //            dr.Dispose();
        //        }
        //    }

        //    return zList;
        //}

        //public static List<VW_Matching_ProgramProductTemplateAssignment> GetAllProgramProductTemplateAssignmentBeta()
        //{
        //    List<DataModel.Entity.VW_Matching_ProgramProductTemplateAssignment> zList = null;

        //    SqlDataReader dr = null;
            
        //    try
        //    {
        //        dr = DataHelper.GetDataReader("VW_Matching_ProgramProductTemplateAssignment");

        //        if (dr.HasRows)
        //        {
        //            zList = new List<Entity.VW_Matching_ProgramProductTemplateAssignment>();

        //            while (dr.Read())
        //            {
        //                zList.Add(new Entity.VW_Matching_ProgramProductTemplateAssignment(dr));
        //    }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ISException isEx = new ISException(ex);
        //        isEx.Save();
        //    }
        //    finally
        //    {
        //        if (dr != null)
        //        {
        //            dr.Close();
        //            dr.Dispose();
        //        }
        //    }

        //    return zList;
        //}

        public static List<VW_Matching_CRCallCenterHours> GetAllCRCallCenterHours()
        {
            List<DataModel.Entity.VW_Matching_CRCallCenterHours> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CRCallCenterHours");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CRCallCenterHours>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CRCallCenterHours(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CampusCallCenterHours> GetAllCampusCallCenterHours()
        {
            List<DataModel.Entity.VW_Matching_CampusCallCenterHours> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampusCallCenterHours");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CampusCallCenterHours>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CampusCallCenterHours(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_WarmTransferInfo> GetAllWarmTransferInfo()
        {
            List<DataModel.Entity.VW_Matching_WarmTransferInfo> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_WarmTransferInfo");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_WarmTransferInfo>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_WarmTransferInfo(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_KVCodeDataCache> GetAllKVCodeDataCache()
        {
            List<DataModel.Entity.VW_Matching_KVCodeDataCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_KVCodeDataCache");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_KVCodeDataCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_KVCodeDataCache(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ProgramValidationRuleCache> GetAllProgramValidationRuleCache()
        {
            List<DataModel.Entity.VW_Matching_ProgramValidationRuleCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramValidationRuleCache");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ProgramValidationRuleCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramValidationRuleCache(dr));
            }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CountryCache> GetAllCountryCacheProd()
        {
            List<DataModel.Entity.VW_Matching_CountryCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_CountryCache_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CountryCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CountryCache(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_StateCache> GetAllStateCacheProd()
        {
            List<DataModel.Entity.VW_Matching_StateCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_StateCache_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_StateCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_StateCache(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ClientRelationProductMappingCache> GetAllClientRelationProductMappingCacheBeta()
        {
            List<DataModel.Entity.VW_Matching_ClientRelationProductMappingCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationProductMappingCache");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ClientRelationProductMappingCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ClientRelationProductMappingCache(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ClientRelationProductMappingCache> GetAllClientRelationProductMappingCacheProd()
        {
            List<DataModel.Entity.VW_Matching_ClientRelationProductMappingCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_ClientRelationProductMappingCache_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ClientRelationProductMappingCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ClientRelationProductMappingCache(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ClientCampusProductMappingCache> GetAllClientCampusProductMappingCacheProd()
        {
            List<DataModel.Entity.VW_Matching_ClientCampusProductMappingCache> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_ClientCampusProductMappingCache_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ClientCampusProductMappingCache>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ClientCampusProductMappingCache(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ProgramToEdLevelMapping> GetAllProgramToEdLevelMappingProd()
        {
            List<DataModel.Entity.VW_Matching_ProgramToEdLevelMapping> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_ProgramToEdLevelMapping_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ProgramToEdLevelMapping>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramToEdLevelMapping(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ProgramLevelToEdLevelMapping> GetAllProgramLevelToEdLevelMapping()
        {
            List<DataModel.Entity.VW_Matching_ProgramLevelToEdLevelMapping> zList = new List<VW_Matching_ProgramLevelToEdLevelMapping>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramLevelToEdLevelMapping");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ProgramLevelToEdLevelMapping>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramLevelToEdLevelMapping(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CapHierarchy> GetAllCapHierarchyBeta()
        {
            List<DataModel.Entity.VW_Matching_CapHierarchy> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CapHierarchy");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CapHierarchy>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CapHierarchy(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CapHierarchy> GetAllCapHierarchyProd()
        {
            List<DataModel.Entity.VW_Matching_CapHierarchy> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_CapHierarchy_Prod");

                if (dr.HasRows)
        {
                    zList = new List<Entity.VW_Matching_CapHierarchy>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CapHierarchy(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
            {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_CampaignCapHierarchy> GetAllCampaignCapHierarchyProd()
        {
            List<DataModel.Entity.VW_Matching_CampaignCapHierarchy> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_CampaignCapHierarchy_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CampaignCapHierarchy>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CampaignCapHierarchy(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }
    }
}
