using EDDY.IS.MatchingEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Data.SqlClient;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DTO.Response;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class ContentDataService
    {

        public static List<VW_Matching_ProgramDisplayGroup> GetAllProgramDisplayGroup()
        {
            List<VW_Matching_ProgramDisplayGroup> zList = new List<VW_Matching_ProgramDisplayGroup>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramDisplayGroup");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramDisplayGroup(dr));
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

        public static List<VW_Matching_ClientCampusRelationshipInfo> GetAllClientCampusRelationshipInfo()
        {
            List<DataModel.Entity.VW_Matching_ClientCampusRelationshipInfo> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientCampusRelationshipInfo");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ClientCampusRelationshipInfo>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ClientCampusRelationshipInfo(dr));
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
        public static async Task<Dictionary<int, VW_Matching_SchoolLogo>> GetAllSchoolLogos()
        {
            Dictionary<int, DataModel.Entity.VW_Matching_SchoolLogo> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("VW_Matching_SchoolLogo"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Entity.VW_Matching_SchoolLogo>();

                    while (dr.Read())
                    {
                        VW_Matching_SchoolLogo pc = new VW_Matching_SchoolLogo(dr);

                        if (!zList.ContainsKey(pc.SchoolId))
                            zList.Add(pc.SchoolId, pc);
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

        public static async Task<Dictionary<int, VW_Matching_ProgramLogo>> GetAllProgramLogos()
        {
            Dictionary<int, DataModel.Entity.VW_Matching_ProgramLogo> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("VW_Matching_ProgramLogo"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Entity.VW_Matching_ProgramLogo>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramLogo pc = new VW_Matching_ProgramLogo(dr);

                        if (!zList.ContainsKey(pc.ProgramId))
                            zList.Add(pc.ProgramId, pc);
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

        public static async Task<Dictionary<int, VW_Matching_ProgramShortDescription>> GetAllProgramShortDescription()
        {
            Dictionary<int, DataModel.Entity.VW_Matching_ProgramShortDescription> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("VW_Matching_ProgramShortDescription"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Entity.VW_Matching_ProgramShortDescription>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramShortDescription pc = new VW_Matching_ProgramShortDescription(dr);

                        if (!zList.ContainsKey(pc.ProgramId))
                            zList.Add(pc.ProgramId, pc);
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

        public static async Task<Dictionary<int, VW_Matching_ProgramLongDescription>> GetAllProgramLongDescription()
        {
            Dictionary<int, DataModel.Entity.VW_Matching_ProgramLongDescription> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("VW_Matching_ProgramLongDescription"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Entity.VW_Matching_ProgramLongDescription>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramLongDescription pc = new VW_Matching_ProgramLongDescription(dr);

                        if (!zList.ContainsKey(pc.ProgramId))
                            zList.Add(pc.ProgramId, pc);
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

        public static async Task<Dictionary<int, VW_Matching_ProgramContent>> GetAllProgramContentProd()
        {
            Dictionary<int, DataModel.Entity.VW_Matching_ProgramContent> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("Prod.VW_Matching_ProgramContent_Prod"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Entity.VW_Matching_ProgramContent>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramContent pc = new VW_Matching_ProgramContent(dr);

                        if (!zList.ContainsKey(pc.ProgramId))
                            zList.Add(pc.ProgramId, pc);
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

        public static List<VW_Matching_CampusContent> GetAllCampusContentBeta()
        {
            List<DataModel.Entity.VW_Matching_CampusContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampusContent");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CampusContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CampusContent(dr));
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

        public static List<VW_Matching_CampusContent> GetAllCampusContentProd()
        {
            List<DataModel.Entity.VW_Matching_CampusContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_CampusContent_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CampusContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CampusContent(dr));
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

        public static List<VW_Matching_InstitutionContentDetail> GetAllInstitutionContentDetailProd()
        {
            List<DataModel.Entity.VW_Matching_InstitutionContentDetail> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_InstitutionContentDetail_Prod");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_InstitutionContentDetail>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_InstitutionContentDetail(dr));
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

        public static List<VW_Matching_ProgramLevelContent> GetAllProgramLevelContent()
        {
            List<DataModel.Entity.VW_Matching_ProgramLevelContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramLevelContent");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ProgramLevelContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramLevelContent(dr));
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

        public static List<VW_Matching_SpecialtyContent> GetAllSpecialtyContent()
        {
            List<DataModel.Entity.VW_Matching_SpecialtyContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_SpecialtyContent");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_SpecialtyContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_SpecialtyContent(dr));
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

        public static List<VW_Matching_SubjectContent> GetAllSubjectContent()
        {
            List<DataModel.Entity.VW_Matching_SubjectContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_SubjectContent");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_SubjectContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_SubjectContent(dr));
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

        public static List<VW_Matching_CategoryContent> GetAllCategoryContent()
        {
            List<DataModel.Entity.VW_Matching_CategoryContent> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CategoryContent");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CategoryContent>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CategoryContent(dr));
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

		public static List<VW_Matching_CampusOptionGroupContent> GetAllCampusOptionGroupContent()
		{
			List<DataModel.Entity.VW_Matching_CampusOptionGroupContent> zList = null;

			SqlDataReader dr = null;

			try
			{
				dr = DataHelper.GetDataReader("VW_Matching_CampusOptionGroupContent");

				if (dr.HasRows)
				{
					zList = new List<Entity.VW_Matching_CampusOptionGroupContent>();

					while (dr.Read())
					{
						zList.Add(new Entity.VW_Matching_CampusOptionGroupContent(dr));
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

		public static async Task<Dictionary<int, List<Address>>> GetAllProgramAddresses()
        {
            Dictionary<int, List<Address>> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = await Task.Run(() => DataHelper.GetDataReader("VW_Matching_ProgramAddress"));

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, List<Address>>();

                    while (dr.Read())
                    {
                        var progAddr = new VW_Matching_ProgramAddress(dr);

                        if (zList.ContainsKey(progAddr.ProgramId))
                            zList[progAddr.ProgramId].Add(new Address() { Address1 = progAddr.Address1, Address2 = progAddr.Address2, City = progAddr.City, CityId = progAddr.CityId, CountryCode = progAddr.CountryCode, CountryId = progAddr.CountryId, CountryName = progAddr.CountryName, Fax = progAddr.Fax, Phone = progAddr.Phone, PostalCode = progAddr.PostalCode, StateProvinceCode = progAddr.StateProvinceCode, StateId = progAddr.StateId });
                        else
                            zList.Add(progAddr.ProgramId, new List<Address> { new Address() { Address1 = progAddr.Address1, Address2 = progAddr.Address2, City = progAddr.City, CityId = progAddr.CityId, CountryCode = progAddr.CountryCode, CountryId = progAddr.CountryId, CountryName = progAddr.CountryName, Fax = progAddr.Fax, Phone = progAddr.Phone, PostalCode = progAddr.PostalCode, StateProvinceCode = progAddr.StateProvinceCode, StateId = progAddr.StateId } });
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

        public static Dictionary<int, List<SchoolAgent>> GetClientCampusContacts()
        {
            Dictionary<int, List<SchoolAgent>> zList = new Dictionary<int, List<SchoolAgent>>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientCampusContact");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var row = new VW_Matching_ClientCampusContact(dr);
                        SchoolAgent agent = new SchoolAgent() { AgentId = row.ClientCampusContactId, AgentName = row.ContactName };

                        if (zList.ContainsKey(row.ClientCampusRelationshipId))
                            zList[row.ClientCampusRelationshipId].Add(agent);
                        else
                            zList.Add(row.ClientCampusRelationshipId, new List<SchoolAgent> { agent });
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

        public static Dictionary<int, List<SchoolAgent>> GetClientRelationContacts()
        {
            Dictionary<int, List<SchoolAgent>> zList = new Dictionary<int, List<SchoolAgent>>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ClientRelationContact");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var row = new VW_Matching_ClientRelationContact(dr);
                        SchoolAgent agent = new SchoolAgent() { AgentId = row.ClientRelationContactId, AgentName = row.ContactName };

                        if (zList.ContainsKey(row.ClientRelationshipId))
                            zList[row.ClientRelationshipId].Add(agent);
                        else
                            zList.Add(row.ClientRelationshipId, new List<SchoolAgent> { agent });
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

        public static Dictionary<int, List<DateTime>> GetAllProgramStartDates()
        {
            Dictionary<int, List<DateTime>> zList = new Dictionary<int, List<DateTime>>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramStartDate");

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, List<DateTime>>();

                    while (dr.Read())
                    {
                        var pSD = new VW_Matching_ProgramStartDate(dr);

                        if (zList.ContainsKey(pSD.ProgramId))
                            zList[pSD.ProgramId].Add(pSD.StartDate);

                        else
                            zList.Add(pSD.ProgramId, new List<DateTime> { pSD.StartDate});

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

        public static Dictionary<int, List<ProgramTag>> GetAllProgramTags()
        {
            Dictionary<int, List<ProgramTag>> zList = new Dictionary<int, List<ProgramTag>>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramTag");

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, List<ProgramTag>>();
                    while (dr.Read())
                    {
                        var programId = System.Convert.ToInt32(dr["ProgramId"]);
                        var pSD = new VW_Matching_ProgramTag(dr);

                        if (zList.ContainsKey(pSD.ProgramId))
                            zList[pSD.ProgramId].Add(new ProgramTag(pSD));

                        else
                            zList.Add(pSD.ProgramId, new List<ProgramTag> { new ProgramTag(pSD) });
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

        public static Dictionary<int, List<DateTime>> GetAllInstitutionStartDates()
        {
            Dictionary<int, List<DateTime>> zList = new Dictionary<int, List<DateTime>>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_InstitutionStartDate");

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, List<DateTime>>();

                    while (dr.Read())
                    {
                        var pSD = new VW_Matching_InstitutionStartDate(dr);

                        if (zList.ContainsKey(pSD.InstitutionId))
                            zList[pSD.InstitutionId].Add(pSD.StartDate);

                        else
                            zList.Add(pSD.InstitutionId, new List<DateTime> { pSD.StartDate });

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
        public static ProgramDetail GetProgramContentDetail(int applicationId, int programId)
        {
            ProgramDetail programDetail = null;

            //if (StaticSettings.IsBeta)
            //{
            //    SqlDataReader dr = null;
                
            //    try
            //    {
            //        dr = DataHelper.GetDataReader("VW_Matching_ProgramContentDetail", "programId = " + programId);

            //        if (dr.HasRows)
            //        {
            //            dr.Read();
            //            programDetail = Convert(new Entity.VW_Matching_ProgramContentDetail(dr));
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
            //}
            //else
            //{
                SqlDataReader dr = null;
                
                try
                {
                    dr = DataHelper.GetDataReader("Prod.VW_Matching_ProgramContentDetail_Prod", "programId = " + programId);

                    if (dr.HasRows)
                    {
                        dr.Read();
                        programDetail = Convert(new Entity.VW_Matching_ProgramContentDetail(dr));
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
            //}
            return programDetail;
        }

        public static List<VW_Matching_Image> GetAllImageContent()
        {
            List<DataModel.Entity.VW_Matching_Image> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Image");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_Image>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_Image(dr));
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

        private static ProgramDetail Convert(DataModel.Entity.VW_Matching_ProgramContentDetail item)
        {
            ProgramDetail programDetail = new ProgramDetail();

            programDetail.ProgramId = item.ProgramId;
            programDetail.ProgramName = item.ProgramName;
            programDetail.ProgramTypeId = item.ProgramTypeId;
            programDetail.ProgramLevelId = item.ProgramLevelId;
            programDetail.InstitutionId = item.InstitutionId;
            programDetail.ProgramDescription = item.ProgramDescription;
            programDetail.Requirements = item.Requirements;
            programDetail.LearningFormat = item.LearningFormat;
            programDetail.CreditsRequired = item.CreditsRequired;
            programDetail.Language = item.LanguageName;
            programDetail.AccreditationDescription = item.AccreditationDescription;
            programDetail.InternationalRequirements = item.InternationalRequirements;          
            programDetail.FinancialAidInfo = item.FinancialAidInfo;
            programDetail.AdditionalFinancialAidInfo = item.AdditionalFinancialAidInfo;
            programDetail.OnCampusRequirements = item.OnCampusRequirements;
            programDetail.NumberOfStartDates = item.NumberofstartsDates;
            programDetail.CourseLength = item.CourseLength;
            programDetail.CostPerCredit = item.CostPerCredit;
            programDetail.ProgramDisclaimer = item.ProgramDisclaimer;
            programDetail.ProgramDisclaimerType = MatchingContentData.ConvertDisclaimerTypeTextToEnum(item.ProgramDisclaimerType);
            programDetail.SchoolId = item.SchoolId;
            programDetail.SchoolName = item.SchoolName;

            //programDetail.HasSchoolLogo = item.HasSchoolLogo; //Remove this later
            programDetail.SchoolLogoURL = item.SchoolLogoURL;

            //programDetail.HasProgramLogo = item.HasProgramLogo; //Remove This later
            programDetail.ProgramLogoURL = item.ProgramLogoURL;

            //added by SAB
            programDetail.Scholarships = item.Scholarships;
            programDetail.ScholarshipsDescription = item.ScholarshipsDescription;
            programDetail.SettingDescription = item.SettingDescription;
            programDetail.ShortDescription = item.ShortDescription;
            programDetail.EligibilityRequirements = item.EligibilityRequirements;

            //tuition currencycode. additional
            if (item.InternationalTuition != null && item.InternationalTuition != "" && item.CurrencyCode != null && item.CurrencyCode != "")
                programDetail.InternationalTuition = item.InternationalTuition + " " + item.CurrencyCode + ". " + item.AdditionalTuitionInfo;
            else
                programDetail.InternationalTuition = item.AdditionalTuitionInfo;

            if (!String.IsNullOrEmpty(item.CostPerCreditGrad) && !String.IsNullOrEmpty(item.CostPerCreditCurrencyCode))
                programDetail.CostPerCreditGrad = item.CostPerCreditGrad + " " + item.CostPerCreditCurrencyCode;
            else
                programDetail.CostPerCreditGrad = item.CostPerCreditGrad;

            if (!String.IsNullOrEmpty(item.OutStateTuition) && !String.IsNullOrEmpty(item.OutOfStateTuitionCurrencyCode))
                programDetail.OutStateTuition = item.OutStateTuition + " " + item.OutOfStateTuitionCurrencyCode;
            else
                programDetail.OutStateTuition = item.OutStateTuition;

            if (!String.IsNullOrEmpty(item.InstateTuition) && !String.IsNullOrEmpty(item.InStateTuitionCurrencyCode))
                programDetail.InstateTuition = item.InstateTuition + " " + item.InStateTuitionCurrencyCode;
            else
                programDetail.InstateTuition = item.InstateTuition;

            programDetail.DomesticFinancialAidInfo = item.DomesticFinancialAidInfo;
            programDetail.DomesticFinancialAidDescription = item.DomesticFinancialAidDescription;

            return programDetail;
        }

        private static ProgramAddress ConvertProgramAddress(DataModel.Entity.VW_Matching_ProgramAddress item)
        {
            ProgramAddress programAddress = new ProgramAddress();

            programAddress.ProgramId = item.ProgramId;
            programAddress.Address1 = item.Address1;
            programAddress.Address2 = item.Address2;
            programAddress.City = item.City;
            programAddress.StateProvinceCode = item.StateProvinceCode;
            programAddress.CountryCode = item.CountryCode;
            programAddress.CountryName = item.CountryName;
            programAddress.Phone = item.Phone;
            programAddress.Fax = item.Fax;
            programAddress.CountryId = item.CountryId;
            programAddress.CityId = item.CityId;

            return programAddress;
        }



    }
}
