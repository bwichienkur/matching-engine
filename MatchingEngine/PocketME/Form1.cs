using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EDDY.IS.MatchingEngine;

namespace PocketME
{
    public partial class PocketME : Form
    {
        List<ProgramContent> programContent;
        Dictionary<int, InstitutionContent> institutionContent;
        List<CampusContent> campusContent;
        List<ProgramLevelContent> programLevelContent;
        List<SubjectContent> subjectContent;
        List<CategoryContent> categoryContent;
        Dictionary<int, Cap> caps;
        List<SpecialtyContent> specialtyContent;
        MatchDatabase md;

        public PocketME()
        {
            InitializeComponent();
        }

        private void PocketME_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Loading data...";
            textBox1.Enabled = false;

            programContent = MatchingContentData.GetProgramContent();
            institutionContent = MatchingContentData.GetInstitutionContent();
            campusContent = MatchingContentData.GetCampusContent();
            programLevelContent = MatchingContentData.GetProgramLevelContent();
            subjectContent = MatchingContentData.GetSubjectContent();
            categoryContent = MatchingContentData.GetCategoryContent();
            caps = RuleCacheProcessor.CreateHierarchicalCapList();
            specialtyContent = MatchingContentData.GetSpecialtyContent();
            
            md = MatchDatabase.LoadMatchDatabase();
            JoinContentData(md.MatchItems.Values.ToList());

            textBox1.Text = "";
            textBox1.Enabled = true;
        }

        public List<MatchItem> JoinContentData(List<MatchItem> matchItemList)
        {
            if (campusContent != null)
            {
                var pairedCampuses = (from mi in matchItemList
                                      join cc in campusContent
                                      on mi.CampusId equals cc.CampusId
                                      select new { mi, cc }).ToList();

                pairedCampuses.ForEach(pc =>
                {
                    pc.mi.CampusName = pc.cc.CampusName;
                    pc.mi.CampusAddress1 = pc.cc.Address1;
                    pc.mi.CampusAddress2 = pc.cc.Address2;
                    pc.mi.CampusCity = pc.cc.City;
                    pc.mi.CampusStateCode = pc.cc.StateProvinceCode;
                    pc.mi.CampusPostalCode = pc.cc.PostalCode;
                    pc.mi.CampusCountry = pc.cc.CountryCode;
                    pc.mi.CampusCountryName = pc.cc.CountryName;
                    pc.mi.HasCampusLogo = pc.cc.HasLogo;
                    pc.mi.CampusCountryId = pc.cc.CountryId;
                    pc.mi.CampusStateId = pc.cc.StateId;
                    pc.mi.CampusFax = pc.cc.CampusFax;
                    pc.mi.CampusPhone = pc.cc.CampusPhone;
                });
            }

            if (institutionContent != null)
            {
                var pairedInstitutions = (from mi in matchItemList
                                          join ic in institutionContent
                                          on mi.InstitutionId equals ic.Value.InstitutionId
                                          select new { mi, ic }).ToList();

                pairedInstitutions.ForEach(pi =>
                {
                    //KeyValuePair<int, InstitutionDescription> descriptions = pi.ic.ApplicationInstitutionDescription.Where(d => d.Key == applicationId).FirstOrDefault();
                    InstitutionDescription description = pi.ic.Value.InstitutionDescription;
                    string onlineDesc = null;
                    string campusDesc = null;

                    //if (!descriptions.Equals(default(KeyValuePair<int, InstitutionDescription>)))
                    if (description != null)
                    {
                        onlineDesc = description.Online;
                        campusDesc = description.Ground;
                    }

                    pi.mi.InstitutionName = pi.ic.Value.InstitutionName;
                    pi.mi.InstitutionDisclaimerType = pi.ic.Value.DisclaimerType;
                    pi.mi.InstitutionDisclaimer = pi.ic.Value.DisclaimerText;
                    pi.mi.InstitutionDescriptionOnline = onlineDesc;
                    pi.mi.InstitutionDescriptionCampus = campusDesc;
                });
            }

            if (programContent != null)
            {
                var pairedPrograms = (from mi in matchItemList
                                      join pc in programContent
                                      on mi.ProgramId equals pc.ProgramId
                                      select new { mi, pc }).ToList();

                pairedPrograms.ForEach(pi =>
                {
                    //KeyValuePair<int, string> descriptions = pi.pc.ApplicationProgramDescription.Where(d => d.Key == applicationId).FirstOrDefault();
                    string desc = pi.pc.ProgramDescription;

                    //if (!descriptions.Equals(default(KeyValuePair<int, string>)))
                    //    desc = descriptions.Value;

                    pi.mi.ProgramName = pi.pc.ProgramName;
                    pi.mi.ProgramDisclaimerType = pi.pc.DisclaimerType;
                    pi.mi.ProgramDisclaimer = pi.pc.DisclaimerText;
                    pi.mi.ProgramDescription = desc;
                    pi.mi.ProgramShortDescription = pi.pc.ShortDescription;
                    pi.mi.GradSchoolsDegreeName = pi.pc.GradSchoolsDegreeName;
                    pi.mi.HasSchoolLogo = pi.pc.HasSchoolLogo;
                    pi.mi.SchoolId = pi.pc.SchoolId;
                });
            }

            if (programLevelContent != null)
            {
                var pairedProgramLevels = (from mi in matchItemList
                                           join plc in programLevelContent
                                           on mi.ProgramLevelId equals plc.ProgramLevelId
                                           select new { mi, plc }).ToList();

                pairedProgramLevels.ForEach(pi => pi.mi.ProgramLevelName = pi.plc.ProgramLevelName);
            }

            if (subjectContent != null)
            {
                var pairedSubjects = (from mi in matchItemList
                                      join sc in subjectContent
                                      on mi.SubjectId equals sc.SubjectId
                                      select new { mi, sc }).ToList();

                pairedSubjects.ForEach(pi => pi.mi.SubjectName = pi.sc.SubjectName);
            }

            if (categoryContent != null)
            {
                var pairedCategories = (from mi in matchItemList
                                        join cc in categoryContent
                                        on mi.CategoryId equals cc.CategoryId
                                        select new { mi, cc }).ToList();

                pairedCategories.ForEach(pi => pi.mi.CategoryName = pi.cc.CategoryName);
            }

            if (specialtyContent != null)
            {
                var pairedSpecialties = (from mi in matchItemList
                                         join sc in specialtyContent
                                         on mi.SpecialtyId equals sc.SpecialtyId
                                         select new { mi, sc }).ToList();

                pairedSpecialties.ForEach(pi => pi.mi.SpecialtyName = pi.sc.SpecialtyName);
            }

            return matchItemList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                int crId = Convert.ToInt32(textBox1.Text);
                Dictionary<int, Cap> lowestLevelCap = new Dictionary<int, Cap>();

                List<MatchItem> matches = md.MatchItems.Values.
                                                Where(m => m.ClientRelationshipId == crId
                                                        && m.IsPrimarySubject == true).ToList();

                foreach(var mi in matches)
                {
                    Cap c = caps[mi.ClientRelationProductMappingId];

                    lowestLevelCap.Add(mi.ProgramProductId, LowestLevelCap(mi, c));
                }

                var results = from m in matches
                              join l in lowestLevelCap
                                   on m.ProgramProductId equals l.Key
                              select new
                              {
                                  m.CategoryName,
                                  m.ProgramLevelName,
                                  m.ProductId,
                                  m.ProgramName,
                                  l.Value.TotalCapAmount,
                                  l.Value.TransactionCount
                              };
            }
        }

        private Cap LowestLevelCap(MatchItem mi, Cap cap)
        {
            if(cap.Children == null || cap.Children.Count == 0)
            {
                switch(cap.CapType)
                {
                    case EDDY.IS.MatchingEngine.DTO.EntityMeta.PSI:
                        if (cap.EntityIDSet.Contains(mi.PsiId))
                            return cap;

                        break;
                    case EDDY.IS.MatchingEngine.DTO.EntityMeta.ClientCampusRelationship:
                        if (cap.EntityIDSet.Contains(mi.ClientCampusRelationshipId))
                            return cap;

                        break;
                    case EDDY.IS.MatchingEngine.DTO.EntityMeta.ClientRelationProductMapping:
                        if (cap.EntityIDSet.Contains(mi.ClientRelationProductMappingId))
                            return cap;

                        break;
                }
            }
            else if(cap.Children != null || cap.Children.Count > 0)
            {
                foreach (Cap c in cap.Children)
                    return LowestLevelCap(mi, c);
            }

            return null;
        }
    }
}
