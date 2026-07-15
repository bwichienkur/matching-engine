using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.TestData
{
    public class FailedMatchReplacementServiceTestData
    {

        public List<Institution> GetMockResponse(int maxSchoolPickerMatches)
        {
            List<Institution> institutions = GetFullMockResponse();
            return institutions.Take(maxSchoolPickerMatches).ToList();
        }

        private List<Institution> GetFullMockResponse()
        {
            return new List<Institution>
            {
                new Institution
                {
                    InstitutionId = 8618,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Category",
                    ProgramRankScore = 0.999964422m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302601,
                            ProgramName = "Paid_Category_Program",
                            ProgramProductId = 634949,
                            ProgramRankScore = 0.999964422m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8620,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Specialty_A",
                    ProgramRankScore = 0.899966056m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302603,
                            ProgramName = "Paid_Specialty_A_Program",
                            ProgramProductId = 634951,
                            ProgramRankScore = 0.899966056m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8614,
                    InstitutionLogoUrl = "",
                    InstitutionName = "GSMatch1",
                    ProgramRankScore = 0.649964511m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302597,
                            ProgramName = "Program_GSMatch1",
                            ProgramProductId = 634945,
                            ProgramRankScore = 0.649964511m,
                            ProgramTemplateId = 1
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8617,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Match1Plus",
                    ProgramRankScore = 0.649963482m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302600,
                            ProgramName = "Program_Match1Plus",
                            ProgramProductId = 634948,
                            ProgramRankScore = 0.649963482m,
                            ProgramTemplateId = 1
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8616,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Match1",
                    ProgramRankScore = 0.649963351m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302599,
                            ProgramName = "Program_Match1",
                            ProgramProductId = 634947,
                            ProgramRankScore = 0.649963351m,
                            ProgramTemplateId = 1
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8324,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/8324/Logo_240x80.gif?1531394702",
                    InstitutionName = "QA National",
                    ProgramRankScore = 0.62396609108m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302512,
                            ProgramName = "Years_Teaching_Experience_Minimum 2 years",
                            ProgramProductId = 634862,
                            ProgramRankScore = 0.649967791m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 302494,
                            ProgramName = "HS Grad Year Last Year",
                            ProgramProductId = 634845,
                            ProgramRankScore = 0.649967698m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302511,
                            ProgramName = "USCitizen_Required",
                            ProgramProductId = 634861,
                            ProgramRankScore = 0.649967244m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302489,
                            ProgramName = "GPA Minimum 3.0",
                            ProgramProductId = 634840,
                            ProgramRankScore = 0.649967154m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302505,
                            ProgramName = "Registered_And_Possess_Licensure",
                            ProgramProductId = 634855,
                            ProgramRankScore = 0.649966969m,
                            ProgramTemplateId = 5
                        },
                        new Program
                        {
                            ProgramId = 302496,
                            ProgramName = "Maximum Age 40",
                            ProgramProductId = 634847,
                            ProgramRankScore = 0.64996682m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302491,
                            ProgramName = "HS Grad Year 1980 and Above",
                            ProgramProductId = 634842,
                            ProgramRankScore = 0.649966551m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302497,
                            ProgramName = "Military_Required",
                            ProgramProductId = 634848,
                            ProgramRankScore = 0.649966479m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302510,
                            ProgramName = "Undergrad_Teaching_Degree_Yes",
                            ProgramProductId = 634860,
                            ProgramRankScore = 0.649966108m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 302499,
                            ProgramName = "No_Military_Required",
                            ProgramProductId = 634850,
                            ProgramRankScore = 0.649965778m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302506,
                            ProgramName = "RNLicense Required",
                            ProgramProductId = 634856,
                            ProgramRankScore = 0.649965232m,
                            ProgramTemplateId = 3
                        },
                        new Program
                        {
                            ProgramId = 302493,
                            ProgramName = "HS Grad Year Current",
                            ProgramProductId = 634844,
                            ProgramRankScore = 0.649965071m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302504,
                            ProgramName = "Radiology_Registered_Yes",
                            ProgramProductId = 634854,
                            ProgramRankScore = 0.649964614m,
                            ProgramTemplateId = 5
                        },
                        new Program
                        {
                            ProgramId = 302508,
                            ProgramName = "Teaching_Certificate_Yes",
                            ProgramProductId = 634858,
                            ProgramRankScore = 0.649964225m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 302509,
                            ProgramName = "Undergrad_Nursing_Degree_Yes",
                            ProgramProductId = 634859,
                            ProgramRankScore = 0.649963712m,
                            ProgramTemplateId = 3
                        },
                        new Program
                        {
                            ProgramId = 302502,
                            ProgramName = "Radiology_Clinical_Experience_1600Hours_Yes",
                            ProgramProductId = 634852,
                            ProgramRankScore = 0.649963495m,
                            ProgramTemplateId = 5
                        },
                        new Program
                        {
                            ProgramId = 302488,
                            ProgramName = "ChristianLearningEnvironment_Yes_NotSure",
                            ProgramProductId = 634839,
                            ProgramRankScore = 0.649963143m,
                            ProgramTemplateId = 370
                        },
                        new Program
                        {
                            ProgramId = 302490,
                            ProgramName = "GraduateDegree_UndergradInField_Required",
                            ProgramProductId = 634841,
                            ProgramRankScore = 0.649962943m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302492,
                            ProgramName = "HS Grad Year 2000 and Below",
                            ProgramProductId = 634843,
                            ProgramRankScore = 0.649962732m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302495,
                            ProgramName = "LPNLicense_Required",
                            ProgramProductId = 634846,
                            ProgramRankScore = 0.649962048m,
                            ProgramTemplateId = 257
                        },
                        new Program
                        {
                            ProgramId = 302503,
                            ProgramName = "Radiology_Employment_or_RecentGrad_Yes",
                            ProgramProductId = 634853,
                            ProgramRankScore = 0.64996186m,
                            ProgramTemplateId = 5
                        },
                        new Program
                        {
                            ProgramId = 302513,
                            ProgramName = "Years_Work_Experience_Minimum 3 years",
                            ProgramProductId = 634863,
                            ProgramRankScore = 0.649961822m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302487,
                            ProgramName = "CCNE_Required_Yes",
                            ProgramProductId = 634838,
                            ProgramRankScore = 0.649961434m,
                            ProgramTemplateId = 368
                        },
                        new Program
                        {
                            ProgramId = 302507,
                            ProgramName = "StartDateLess Than 3 Months",
                            ProgramProductId = 634857,
                            ProgramRankScore = 0.649961354m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302498,
                            ProgramName = "Minimum Age 25",
                            ProgramProductId = 634849,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8635,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Eplite ",
                    ProgramRankScore = 0.599961510m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302658,
                            ProgramName = "Accounting Premier",
                            ProgramProductId = 635010,
                            ProgramRankScore = 0.599961510m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 4462,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/4462/Logo_240x80.gif?1549449710",
                    InstitutionName = "University of Miami",
                    ProgramRankScore = 0.575497943m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 244982,
                            ProgramName = "Master of Arts in International Administration (MAIA)",
                            ProgramProductId = 626396,
                            ProgramRankScore = 0.575498082m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 243996,
                            ProgramName = "Master of Arts in Liberal Studies (MALS)",
                            ProgramProductId = 359259,
                            ProgramRankScore = 0.575497804m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 272,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/272/Logo_240x80.gif?1346189645",
                    InstitutionName = "Northcentral University",
                    ProgramRankScore = 0.569129902345m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 177449,
                            ProgramName = "Master of Education - International Education",
                            ProgramProductId = 633838,
                            ProgramRankScore = 1.440839937m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3646,
                            ProgramName = "Master of Education - Global Training and Development",
                            ProgramProductId = 633822,
                            ProgramRankScore = 1.440839691m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274349,
                            ProgramName = "Master of Science in Organizational Leadership - Nonprofit Management",
                            ProgramProductId = 634030,
                            ProgramRankScore = 1.440839673m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 297099,
                            ProgramName = "Master of Education in Educational Leadership",
                            ProgramProductId = 633886,
                            ProgramRankScore = 1.440839375m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3644,
                            ProgramName = "Master of Education - Sport and Athletic Management",
                            ProgramProductId = 633878,
                            ProgramRankScore = 1.440839275m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274351,
                            ProgramName = "Master of Science in Organizational Leadership - Public Administration",
                            ProgramProductId = 634046,
                            ProgramRankScore = 1.440839164m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301804,
                            ProgramName = "MPA Management Information Systems",
                            ProgramProductId = 634134,
                            ProgramRankScore = 1.440838620m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3648,
                            ProgramName = "Master of Education - Early Childhood Education ",
                            ProgramProductId = 633790,
                            ProgramRankScore = 1.440838594m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 299767,
                            ProgramName = "Master of Science in Educational Psychology",
                            ProgramProductId = 633950,
                            ProgramRankScore = 1.440838519m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274379,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - Military Family Therapy",
                            ProgramProductId = 633598,
                            ProgramRankScore = 1.440838505m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3620,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - Child and Adolescent Family Therapy",
                            ProgramProductId = 633558,
                            ProgramRankScore = 1.440838482m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274375,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - Couple Therapy",
                            ProgramProductId = 633566,
                            ProgramRankScore = 1.440838389m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 192453,
                            ProgramName = "Master of Education - Corporate Wellness",
                            ProgramProductId = 633774,
                            ProgramRankScore = 1.440838210m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3624,
                            ProgramName = "Master of Education - Organizational Leadership ",
                            ProgramProductId = 633854,
                            ProgramRankScore = 1.440838137m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 296532,
                            ProgramName = "Master of Science in Information Technology",
                            ProgramProductId = 633982,
                            ProgramRankScore = 1.440838112m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3645,
                            ProgramName = "Master of Education - E-Learning ",
                            ProgramProductId = 633798,
                            ProgramRankScore = 1.440837615m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 293379,
                            ProgramName = "Master of Science in Technology and Innovation Management - Information Systems",
                            ProgramProductId = 634062,
                            ProgramRankScore = 1.440837597m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301798,
                            ProgramName = "MPA Criminal Justice ",
                            ProgramProductId = 634206,
                            ProgramRankScore = 1.440837454m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274346,
                            ProgramName = "Master of Science in Organizational Leadership - Criminal Justice",
                            ProgramProductId = 633998,
                            ProgramRankScore = 1.440837438m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3642,
                            ProgramName = "Master of Arts in Psychology - Gender Diversity Studies ",
                            ProgramProductId = 633606,
                            ProgramRankScore = 1.440837375m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 177385,
                            ProgramName = "Master of Business Administration - Entrepreneurship ",
                            ProgramProductId = 633646,
                            ProgramRankScore = 1.440837342m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 292932,
                            ProgramName = "Master of Science in Organizational Leadership - General Organizational Leadership",
                            ProgramProductId = 634006,
                            ProgramRankScore = 1.440837213m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274376,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - General Family Therapy",
                            ProgramProductId = 633574,
                            ProgramRankScore = 1.440836592m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300733,
                            ProgramName = "Master of Science in Nursing – Management and Organizational Leadership",
                            ProgramProductId = 634190,
                            ProgramRankScore = 1.440836532m,
                            ProgramTemplateId = 3
                        },
                        new Program
                        {
                            ProgramId = 22272,
                            ProgramName = "Master of Business Administration - General Business ",
                            ProgramProductId = 633662,
                            ProgramRankScore = 1.440836525m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301807,
                            ProgramName = "MPA Project Management",
                            ProgramProductId = 634158,
                            ProgramRankScore = 1.440836500m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 22276,
                            ProgramName = "Master of Education - Athletic Coaching ",
                            ProgramProductId = 633766,
                            ProgramRankScore = 1.440836459m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 295474,
                            ProgramName = "Master of Science in Child and Adolescent Developmental Psychology",
                            ProgramProductId = 633918,
                            ProgramRankScore = 1.440836393m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293387,
                            ProgramName = "Master of Science in Forensic Psychology",
                            ProgramProductId = 633958,
                            ProgramRankScore = 1.440836229m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3625,
                            ProgramName = "Master of Business Administration - Applied Computer Science",
                            ProgramProductId = 633622,
                            ProgramRankScore = 1.440836192m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 299768,
                            ProgramName = "Master of Social Work",
                            ProgramProductId = 634078,
                            ProgramRankScore = 1.440836039m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274370,
                            ProgramName = "Master of Business Administration - Strategic Management",
                            ProgramProductId = 633750,
                            ProgramRankScore = 1.440836009m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3633,
                            ProgramName = "Master of Business Administration - Management ",
                            ProgramProductId = 633702,
                            ProgramRankScore = 1.440835895m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3635,
                            ProgramName = "Master of Business Administration - Management of Engineering and Technology ",
                            ProgramProductId = 633718,
                            ProgramRankScore = 1.440835876m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 22274,
                            ProgramName = "Master of Education - General Education ",
                            ProgramProductId = 633814,
                            ProgramRankScore = 1.440835871m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 300136,
                            ProgramName = "Master of Human Resource Management",
                            ProgramProductId = 633894,
                            ProgramRankScore = 1.440835653m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 296533,
                            ProgramName = "Master of Science in Computer Science",
                            ProgramProductId = 633926,
                            ProgramRankScore = 1.440835533m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3622,
                            ProgramName = "Master of Education - Special Education ",
                            ProgramProductId = 633870,
                            ProgramRankScore = 1.440835456m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274348,
                            ProgramName = "Master of Science in Organizational Leadership - Human Resources Management",
                            ProgramProductId = 634022,
                            ProgramRankScore = 1.440835394m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301808,
                            ProgramName = "MPA Strategic Management",
                            ProgramProductId = 634166,
                            ProgramRankScore = 1.440835390m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300138,
                            ProgramName = "Master of Public Administration",
                            ProgramProductId = 633910,
                            ProgramRankScore = 1.440835203m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295475,
                            ProgramName = "Master of Science in Health Psychology",
                            ProgramProductId = 633966,
                            ProgramRankScore = 1.440835172m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301801,
                            ProgramName = "MPA Homeland Security ",
                            ProgramProductId = 634110,
                            ProgramRankScore = 1.44083517m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3619,
                            ProgramName = "Master of Education - English Language Learning",
                            ProgramProductId = 633806,
                            ProgramRankScore = 1.440834986m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3630,
                            ProgramName = "Master of Business Administration - Homeland Security ",
                            ProgramProductId = 633678,
                            ProgramRankScore = 1.440834842m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293378,
                            ProgramName = "Master of Science in Technology and Innovation Management - Engineering Management",
                            ProgramProductId = 634054,
                            ProgramRankScore = 1.44083471m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298377,
                            ProgramName = "Master of Legal Studies",
                            ProgramProductId = 633902,
                            ProgramRankScore = 1.440834643m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 296531,
                            ProgramName = "Master of Science in Cybersecurity",
                            ProgramProductId = 633934,
                            ProgramRankScore = 1.440834600m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3629,
                            ProgramName = "Master of Business Administration - Health Care Administration ",
                            ProgramProductId = 633670,
                            ProgramRankScore = 1.44083455m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301805,
                            ProgramName = "MPA Management of Engineering and Technology ",
                            ProgramProductId = 634142,
                            ProgramRankScore = 1.44083440m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298378,
                            ProgramName = "Master of Science in Instructional Design (MSID)",
                            ProgramProductId = 633990,
                            ProgramRankScore = 1.440834200m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274347,
                            ProgramName = "Master of Science in Organizational Leadership - Health Care Administration",
                            ProgramProductId = 634014,
                            ProgramRankScore = 1.440833934m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293380,
                            ProgramName = "Master of Science in Technology and Innovation Management - IT Project Management",
                            ProgramProductId = 634070,
                            ProgramRankScore = 1.440833797m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 291215,
                            ProgramName = "Master of Education - Reading Education",
                            ProgramProductId = 633862,
                            ProgramRankScore = 1.440833683m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 297097,
                            ProgramName = "Master of Science in Data Science",
                            ProgramProductId = 633942,
                            ProgramRankScore = 1.440833660m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3623,
                            ProgramName = "Master of Education - Leadership in Higher Education ",
                            ProgramProductId = 633846,
                            ProgramRankScore = 1.440833477m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3647,
                            ProgramName = "Master of Education - Instructional Leadership ",
                            ProgramProductId = 633830,
                            ProgramRankScore = 1.440833360m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 301803,
                            ProgramName = "MPA Management ",
                            ProgramProductId = 634126,
                            ProgramRankScore = 1.440833286m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295476,
                            ProgramName = "Master of Science in Industrial/Organizational Psychology",
                            ProgramProductId = 633974,
                            ProgramRankScore = 1.440833172m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301800,
                            ProgramName = "MPA Health Care Administration",
                            ProgramProductId = 634102,
                            ProgramRankScore = 1.440833166m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301806,
                            ProgramName = "MPA Nonprofit Management",
                            ProgramProductId = 634150,
                            ProgramRankScore = 1.440833042m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3637,
                            ProgramName = "Master of Business Administration - Public Administration ",
                            ProgramProductId = 633742,
                            ProgramRankScore = 1.440832658m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3638,
                            ProgramName = "Master of Arts in Psychology - General Psychology ",
                            ProgramProductId = 633614,
                            ProgramRankScore = 1.440832491m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3634,
                            ProgramName = "Master of Business Administration - Management Information Systems ",
                            ProgramProductId = 633710,
                            ProgramRankScore = 1.440832318m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3632,
                            ProgramName = "Master of Business Administration - International Business ",
                            ProgramProductId = 633694,
                            ProgramRankScore = 1.440832305m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274350,
                            ProgramName = "Master of Science in Organizational Leadership - Project Management",
                            ProgramProductId = 634038,
                            ProgramRankScore = 1.440832046m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3641,
                            ProgramName = "Master of Education - Curriculum and Teaching ",
                            ProgramProductId = 633782,
                            ProgramRankScore = 1.440832021m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3649,
                            ProgramName = "Master of Education - Adult Learning and Workforce Education ",
                            ProgramProductId = 633758,
                            ProgramRankScore = 1.440831713m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 301799,
                            ProgramName = "MPA Financial Management ",
                            ProgramProductId = 634094,
                            ProgramRankScore = 1.440831602m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301797,
                            ProgramName = "MPA Computer and Information Security ",
                            ProgramProductId = 634086,
                            ProgramRankScore = 1.440831471m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3631,
                            ProgramName = "Master of Business Administration - Human Resources Management ",
                            ProgramProductId = 633686,
                            ProgramRankScore = 1.440831425m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3640,
                            ProgramName = "Master of Business Administration - Computer and Information Security ",
                            ProgramProductId = 633630,
                            ProgramRankScore = 1.440831414m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274377,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - LGBTQ Couple and Family Therapy",
                            ProgramProductId = 633582,
                            ProgramRankScore = 1.440831404m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274378,
                            ProgramName = "Master of Arts in Marriage & Family Therapy - Medical Family Therapy",
                            ProgramProductId = 633590,
                            ProgramRankScore = 1.440831388m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301802,
                            ProgramName = "MPA Human Resources Management ",
                            ProgramProductId = 634118,
                            ProgramRankScore = 1.440831365m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3628,
                            ProgramName = "Master of Business Administration - Financial Management ",
                            ProgramProductId = 633654,
                            ProgramRankScore = 1.440831294m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3643,
                            ProgramName = "Master of Business Administration - Project Management ",
                            ProgramProductId = 633734,
                            ProgramRankScore = 1.440831176m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3636,
                            ProgramName = "Master of Business Administration - Marketing",
                            ProgramProductId = 633726,
                            ProgramRankScore = 1.440831044m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3626,
                            ProgramName = "Master of Business Administration - Criminal Justice ",
                            ProgramProductId = 633638,
                            ProgramRankScore = 1.440831026m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300137,
                            ProgramName = "Doctor of Philosophy in Human Resource Management ",
                            ProgramProductId = 623769,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300141,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Culture, Diversity & Social Justice in a Global Context",
                            ProgramProductId = 623778,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300140,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Education and Supervision",
                            ProgramProductId = 623787,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300139,
                            ProgramName = "Doctor of Public Administration",
                            ProgramProductId = 623809,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300142,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Systemic Leadership",
                            ProgramProductId = 623796,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3392,
                            ProgramName = "Doctor of Business Administration - Advanced Accounting ",
                            ProgramProductId = 621451,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 185295,
                            ProgramName = "Doctor of Business Administration - Applied Computer Science ",
                            ProgramProductId = 621452,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3365,
                            ProgramName = "Doctor of Business Administration - Computer and Information Security ",
                            ProgramProductId = 621453,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3368,
                            ProgramName = "Doctor of Business Administration - Criminal Justice ",
                            ProgramProductId = 621454,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3370,
                            ProgramName = "Doctor of Business Administration - Financial Management ",
                            ProgramProductId = 621455,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 22273,
                            ProgramName = "Doctor of Business Administration - General Business ",
                            ProgramProductId = 621456,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3371,
                            ProgramName = "Doctor of Business Administration - Health Care Administration ",
                            ProgramProductId = 621457,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3372,
                            ProgramName = "Doctor of Business Administration - Homeland Security -  Leadership & Policy ",
                            ProgramProductId = 621458,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3373,
                            ProgramName = "Doctor of Business Administration - Human Resources Management ",
                            ProgramProductId = 621459,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3374,
                            ProgramName = "Doctor of Business Administration - International Business ",
                            ProgramProductId = 621461,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298366,
                            ProgramName = "Doctor of Criminal Justice - Homeland Security",
                            ProgramProductId = 621469,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298367,
                            ProgramName = "Doctor of Criminal Justice - Policing",
                            ProgramProductId = 621470,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298368,
                            ProgramName = "Doctor of Criminal Justice - White Collar Crime",
                            ProgramProductId = 621471,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3383,
                            ProgramName = "Doctor of Education - Curriculum and Teaching ",
                            ProgramProductId = 621472,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3400,
                            ProgramName = "Doctor of Education - Early Childhood Education  ",
                            ProgramProductId = 621473,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3393,
                            ProgramName = "Doctor of Business Administration - Industrial Organizational Psychology",
                            ProgramProductId = 621460,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3375,
                            ProgramName = "Doctor of Business Administration - Management ",
                            ProgramProductId = 621462,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3376,
                            ProgramName = "Doctor of Business Administration - Management Information Systems ",
                            ProgramProductId = 621463,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3378,
                            ProgramName = "Doctor of Business Administration - Marketing ",
                            ProgramProductId = 621465,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3366,
                            ProgramName = "Doctor of Business Administration - Organizational Leadership ",
                            ProgramProductId = 621466,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3394,
                            ProgramName = "Doctor of Business Administration - Project Management ",
                            ProgramProductId = 621467,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3379,
                            ProgramName = "Doctor of Business Administration - Public Administration ",
                            ProgramProductId = 621468,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3386,
                            ProgramName = "Doctor of Education - English Language Learning",
                            ProgramProductId = 621475,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 22275,
                            ProgramName = "Doctor of Education - General Education ",
                            ProgramProductId = 621476,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3387,
                            ProgramName = "Doctor of Education - Global Training and Development ",
                            ProgramProductId = 621477,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3384,
                            ProgramName = "Doctor of Education - Instructional Leadership ",
                            ProgramProductId = 621478,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3385,
                            ProgramName = "Doctor of Education - E-Learning ",
                            ProgramProductId = 621640,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177450,
                            ProgramName = "Doctor of Philosophy in Psychology - Mental Health Policy and Practice",
                            ProgramProductId = 621641,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3377,
                            ProgramName = "Doctor of Business Administration - Management of Engineering and Technology ",
                            ProgramProductId = 621639,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274372,
                            ProgramName = "Doctor of Philosophy in Psychology - Addictions",
                            ProgramProductId = 621532,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3389,
                            ProgramName = "Doctor of Philosophy in Psychology - Gender Diversity Studies ",
                            ProgramProductId = 621533,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3404,
                            ProgramName = "Doctor of Philosophy in Education - Sport and Athletic Management",
                            ProgramProductId = 621523,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 297102,
                            ProgramName = "Doctor of Philosophy in Educational Leadership",
                            ProgramProductId = 621524,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 298375,
                            ProgramName = "Doctor of Philosophy in Instructional Design (PhD-ID)",
                            ProgramProductId = 621525,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3339,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Child and Adolescent Therapy ",
                            ProgramProductId = 621526,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3340,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Couple Therapy ",
                            ProgramProductId = 621527,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3406,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - General Family Therapy",
                            ProgramProductId = 621528,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3341,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Medical Family Therapy ",
                            ProgramProductId = 621529,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3342,
                            ProgramName = "Doctor of Philosophy in Marriage & Family Therapy - Therapy with Military Families ",
                            ProgramProductId = 621530,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274345,
                            ProgramName = "Doctor of Philosophy in Organizational Leadership",
                            ProgramProductId = 621531,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3351,
                            ProgramName = "Doctor of Philosophy in Psychology - General Psychology ",
                            ProgramProductId = 621534,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3390,
                            ProgramName = "Doctor of Philosophy in Psychology - Health Psychology ",
                            ProgramProductId = 621536,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 177867,
                            ProgramName = "Education Specialist - Global Training and Development",
                            ProgramProductId = 621558,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177869,
                            ProgramName = "Education Specialist - Instructional Leadership",
                            ProgramProductId = 621559,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177870,
                            ProgramName = "Education Specialist - International Education",
                            ProgramProductId = 621560,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177868,
                            ProgramName = "Education Specialist - Leadership in Higher Education",
                            ProgramProductId = 621561,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292924,
                            ProgramName = "Education Specialist - Learning Analytics in Higher Education",
                            ProgramProductId = 621562,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292927,
                            ProgramName = "Education Specialist - Learning Analytics in K-12 Education",
                            ProgramProductId = 621563,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177871,
                            ProgramName = "Education Specialist - Organizational Leadership",
                            ProgramProductId = 621565,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177872,
                            ProgramName = "Education Specialist - Special Education",
                            ProgramProductId = 621566,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177873,
                            ProgramName = "Education Specialist - Sport and Athletic Management",
                            ProgramProductId = 621567,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 297101,
                            ProgramName = "Education Specialist in Educational Leadership",
                            ProgramProductId = 621568,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292929,
                            ProgramName = "Education Specialist - General Education",
                            ProgramProductId = 621557,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274373,
                            ProgramName = "Doctor of Philosophy in Psychology - Gerontology",
                            ProgramProductId = 621535,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 177866,
                            ProgramName = "Education Specialist - English Language Learning",
                            ProgramProductId = 621556,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177863,
                            ProgramName = "Education Specialist - Early Childhood Education",
                            ProgramProductId = 621554,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3391,
                            ProgramName = "PhD in Psychology - Industrial Organizational Psychology",
                            ProgramProductId = 621537,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274374,
                            ProgramName = "Doctor of Philosophy in Psychology - Trauma and Disaster Relief",
                            ProgramProductId = 621539,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293381,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - Computer Science",
                            ProgramProductId = 621540,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293382,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - Cybersecurity",
                            ProgramProductId = 621541,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293383,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - Data Science",
                            ProgramProductId = 621542,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293384,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - Engineering Management",
                            ProgramProductId = 621543,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293385,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - Information Systems",
                            ProgramProductId = 621544,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293386,
                            ProgramName = "Doctor of Philosophy in Technology and Innovation Management - IT Project Management",
                            ProgramProductId = 621545,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274357,
                            ProgramName = "Doctorate of Marriage & Family Therapy - Child and Adolescent Therapy",
                            ProgramProductId = 621546,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274358,
                            ProgramName = "Doctorate of Marriage & Family Therapy - Couple Therapy",
                            ProgramProductId = 621547,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274359,
                            ProgramName = "Doctorate of Marriage & Family Therapy - General Family Therapy",
                            ProgramProductId = 621548,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274361,
                            ProgramName = "Doctorate of Marriage & Family Therapy - Mental Health Administration",
                            ProgramProductId = 621550,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274362,
                            ProgramName = "Doctorate of Marriage & Family Therapy - Small Business Development and Entrepreneurship",
                            ProgramProductId = 621551,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274363,
                            ProgramName = "Doctorate of Marriage & Family Therapy - Therapy with Military Families",
                            ProgramProductId = 621552,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 177862,
                            ProgramName = "Education Specialist - Curriculum and Teaching",
                            ProgramProductId = 621553,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 177865,
                            ProgramName = "Education Specialist - E-Learning",
                            ProgramProductId = 621555,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3362,
                            ProgramName = "Doctor of Philosophy in Education - Special Education",
                            ProgramProductId = 621522,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 298372,
                            ProgramName = "Doctor of Health Administration",
                            ProgramProductId = 621489,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3401,
                            ProgramName = "Doctor of Philosophy in Business Administration - Advanced Accounting ",
                            ProgramProductId = 621490,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3343,
                            ProgramName = "Doctor of Philosophy in Business Administration - Applied Computer Science ",
                            ProgramProductId = 621491,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3380,
                            ProgramName = "Doctor of Philosophy in Business Administration - Computer and Information Security ",
                            ProgramProductId = 621492,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3344,
                            ProgramName = "Doctor of Philosophy in Business Administration - Criminal Justice ",
                            ProgramProductId = 621493,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298369,
                            ProgramName = "Doctor of Education in Instructional Design (EDD-ID)",
                            ProgramProductId = 621488,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3346,
                            ProgramName = "Doctor of Philosophy in Business Administration - Financial Management ",
                            ProgramProductId = 621494,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 297100,
                            ProgramName = "Doctor of Education in Educational Leadership",
                            ProgramProductId = 621487,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3388,
                            ProgramName = "Doctor of Education - International Education",
                            ProgramProductId = 621479,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3359,
                            ProgramName = "Doctor of Education - Leadership in Higher Education",
                            ProgramProductId = 621480,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 291216,
                            ProgramName = "Doctor of Education - Learning Analytics in Higher Education",
                            ProgramProductId = 621481,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292926,
                            ProgramName = "Doctor of Education - Learning Analytics in K-12 Education",
                            ProgramProductId = 621482,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 274371,
                            ProgramName = "Doctor of Education - Nursing Education",
                            ProgramProductId = 621483,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 3
                        },
                        new Program
                        {
                            ProgramId = 3360,
                            ProgramName = "Doctor of Education - Organizational Leadership ",
                            ProgramProductId = 621484,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3358,
                            ProgramName = "Doctor of Education - Special Education ",
                            ProgramProductId = 621485,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3399,
                            ProgramName = "Doctor of Education - Sport and Athletic Management",
                            ProgramProductId = 621486,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3364,
                            ProgramName = "Doctor of Philosophy in Education - Organizational Leadership",
                            ProgramProductId = 621521,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 297098,
                            ProgramName = "Doctor of Philosophy in Data Science",
                            ProgramProductId = 621508,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3382,
                            ProgramName = "Doctor of Philosophy in Education - Curriculum and Teaching",
                            ProgramProductId = 621509,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3405,
                            ProgramName = "Doctor of Philosophy in Education - Early Childhood Education",
                            ProgramProductId = 621510,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3395,
                            ProgramName = "Doctor of Philosophy in Education - E-Learning",
                            ProgramProductId = 621511,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3396,
                            ProgramName = "Doctor of Philosophy in Education - English Language Learning",
                            ProgramProductId = 621512,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292928,
                            ProgramName = "Doctor of Philosophy in Education - General Education",
                            ProgramProductId = 621513,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3397,
                            ProgramName = "Doctor of Philosophy in Education - Global Training and Development",
                            ProgramProductId = 621514,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3398,
                            ProgramName = "Doctor of Philosophy in Education - Instructional Leadership",
                            ProgramProductId = 621515,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3367,
                            ProgramName = "Doctor of Philosophy in Education - International Education",
                            ProgramProductId = 621516,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 3363,
                            ProgramName = "Doctor of Philosophy in Education - Leadership in Higher Education",
                            ProgramProductId = 621517,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292923,
                            ProgramName = "Doctor of Philosophy in Education - Learning Analytics in Higher Education",
                            ProgramProductId = 621518,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 292925,
                            ProgramName = "Doctor of Philosophy in Education - Learning Analytics in K-12 Education",
                            ProgramProductId = 621519,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 296534,
                            ProgramName = "Doctor of Philosophy in Computer Science",
                            ProgramProductId = 621507,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3352,
                            ProgramName = "Doctor of Philosophy in Business Administration - Health Care Administration ",
                            ProgramProductId = 621495,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3350,
                            ProgramName = "Doctor of Philosophy in Business Administration - Public Administration ",
                            ProgramProductId = 621506,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3381,
                            ProgramName = "Doctor of Philosophy in Business Administration - Organizational Leadership ",
                            ProgramProductId = 621504,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3353,
                            ProgramName = "Doctor of Philosophy in Business Administration - Homeland Security -  Leadership & Policy ",
                            ProgramProductId = 621496,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3354,
                            ProgramName = "Doctor of Philosophy in Business Administration - Human Resources Management ",
                            ProgramProductId = 621497,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3402,
                            ProgramName = "PhD in Business Administration - Industrial Organizational Psychology",
                            ProgramProductId = 621498,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3347,
                            ProgramName = "Doctor of Philosophy in Business Administration - International Business ",
                            ProgramProductId = 621499,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3348,
                            ProgramName = "Doctor of Philosophy in Business Administration - Management ",
                            ProgramProductId = 621500,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3355,
                            ProgramName = "Doctor of Philosophy in Business Administration - Management Information Systems ",
                            ProgramProductId = 621501,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3356,
                            ProgramName = "Doctor of Philosophy in Business Administration - Management of Engineering and Technology ",
                            ProgramProductId = 621502,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3349,
                            ProgramName = "Doctor of Philosophy in Business Administration - Marketing ",
                            ProgramProductId = 621503,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3403,
                            ProgramName = "Doctor of Philosophy in Business Administration - Project Management ",
                            ProgramProductId = 621505,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 300732,
                            ProgramName = "Doctor of Nursing Practice in Executive Leadership",
                            ProgramProductId = 626285,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 3
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 1571,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/1571/Logo_240x80.gif?1424796370",
                    InstitutionName = "Emerson College",
                    ProgramRankScore = 0.506032149m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 298453,
                            ProgramName = "Master of Science in Communication Disorders",
                            ProgramProductId = 617425,
                            ProgramRankScore = 0.506032149m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8634,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Select University",
                    ProgramRankScore = 0.499968787m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302656,
                            ProgramName = "Premier Program",
                            ProgramProductId = 635008,
                            ProgramRankScore = 0.499968787m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8632,
                    InstitutionLogoUrl = "",
                    InstitutionName = "MatchExclusive",
                    ProgramRankScore = 0.499967149m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302654,
                            ProgramName = "MatchExclusive Program",
                            ProgramProductId = 635006,
                            ProgramRankScore = 0.499967149m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8622,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Specialty_C",
                    ProgramRankScore = 0.499967021m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302605,
                            ProgramName = "Paid_Specialty_C_Program",
                            ProgramProductId = 634953,
                            ProgramRankScore = 0.499967021m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 497,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/497/Logo_240x80.gif?1337633975",
                    InstitutionName = "Concordia University - Nebraska",
                    ProgramRankScore = 0.4999661425m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302475,
                            ProgramName = "Program_PremierBusiness",
                            ProgramProductId = 634819,
                            ProgramRankScore = 0.499969542m,
                            ProgramTemplateId = 1
                        },
                        new Program
                        {
                            ProgramId = 302476,
                            ProgramName = "Program_SelectBusiness",
                            ProgramProductId = 634820,
                            ProgramRankScore = 0.499962743m,
                            ProgramTemplateId = 1
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8623,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Subject",
                    ProgramRankScore = 0.499963952m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302606,
                            ProgramName = "Paid_Subject_Program",
                            ProgramProductId = 634954,
                            ProgramRankScore = 0.499963952m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8624,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Subject_B",
                    ProgramRankScore = 0.499963379m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302607,
                            ProgramName = "Paid_Subject_B_Program",
                            ProgramProductId = 634955,
                            ProgramRankScore = 0.499963379m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8621,
                    InstitutionLogoUrl = "",
                    InstitutionName = "Paid_Specialty_B",
                    ProgramRankScore = 0.499961013m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302604,
                            ProgramName = "Paid_Specialty_B_Program",
                            ProgramProductId = 634952,
                            ProgramRankScore = 0.499961013m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 3314,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/3314/Logo_240x80.gif?1558363393",
                    InstitutionName = "Rider University",
                    ProgramRankScore = 0.49771561475m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302140,
                            ProgramName = "Masters in Accountancy - Business Analytics",
                            ProgramProductId = 632485,
                            ProgramRankScore = 0.497719066m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302142,
                            ProgramName = "Masters in Accountancy - Forensic Accounting",
                            ProgramProductId = 632487,
                            ProgramRankScore = 0.4977178m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302146,
                            ProgramName = "Master of Business Administration - Global Business",
                            ProgramProductId = 632483,
                            ProgramRankScore = 0.497717653m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302139,
                            ProgramName = "Masters in Accountancy",
                            ProgramProductId = 632484,
                            ProgramRankScore = 0.497717477m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302141,
                            ProgramName = "Masters in Accountancy - Finance",
                            ProgramProductId = 632486,
                            ProgramRankScore = 0.49771432m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302144,
                            ProgramName = "Master of Business Administration - Business Analytics",
                            ProgramProductId = 632481,
                            ProgramRankScore = 0.497714114m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302145,
                            ProgramName = "Master of Business Administration - Finance",
                            ProgramProductId = 632482,
                            ProgramRankScore = 0.497713362m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302143,
                            ProgramName = "Master of Business Administration",
                            ProgramProductId = 632480,
                            ProgramRankScore = 0.497711126m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 3336,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/3336/Logo_240x80.gif?1323756153",
                    InstitutionName = "Rockhurst University",
                    ProgramRankScore = 0.4899655973333333333333333333m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302123,
                            ProgramName = "Master of Science in Business Intelligence and Analytics",
                            ProgramProductId = 632455,
                            ProgramRankScore = 0.489966827m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302121,
                            ProgramName = "Master of Arts in Management",
                            ProgramProductId = 632453,
                            ProgramRankScore = 0.489966229m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302122,
                            ProgramName = "Master of Business Administration",
                            ProgramProductId = 632454,
                            ProgramRankScore = 0.489963736m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 4317,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/4317/Logo_240x80.gif?1491589005",
                    InstitutionName = "University of Delaware",
                    ProgramRankScore = 0.489965093m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 301885,
                            ProgramName = "Master of Arts in Strategic Communication",
                            ProgramProductId = 630993,
                            ProgramRankScore = 0.489965093m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 1934,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/1934/Logo_240x80.gif?1553607826",
                    InstitutionName = "Hofstra University",
                    ProgramRankScore = 0.485803981m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 273189,
                            ProgramName = "Master of Arts in Health Law & Policy",
                            ProgramProductId = 478371,
                            ProgramRankScore = 0.485806537m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 273190,
                            ProgramName = "Master of Laws in Health Law & Policy",
                            ProgramProductId = 478372,
                            ProgramRankScore = 0.485801425m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8607,
                    InstitutionLogoUrl = "",
                    InstitutionName = "EMS Native JSON",
                    ProgramRankScore = 0.479963373m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302583,
                            ProgramName = "Native_JSON_Premier_Gold",
                            ProgramProductId = 634931,
                            ProgramRankScore = 0.479963373m,
                            ProgramTemplateId = 1
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 5128,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/5128/Logo_240x80.gif?1337876871",
                    InstitutionName = "Wake Forest University",
                    ProgramRankScore = 0.4789158965m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 176008,
                            ProgramName = "Master of Arts in Counseling - Clinical Mental Health",
                            ProgramProductId = 180441,
                            ProgramRankScore = 0.478919055m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 176009,
                            ProgramName = "Master of Arts in Counseling - School Counseling",
                            ProgramProductId = 180443,
                            ProgramRankScore = 0.478912738m,
                            ProgramTemplateId = 4
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 2539,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/2539/Logo_240x80.gif?1559816412",
                    InstitutionName = "Loyola Marymount University",
                    ProgramRankScore = 0.4764642495m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 302058,
                            ProgramName = "Master of Laws (Tax LLM)",
                            ProgramProductId = 631654,
                            ProgramRankScore = 0.476466652m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302059,
                            ProgramName = "Master of Tax Law (MT)",
                            ProgramProductId = 631656,
                            ProgramRankScore = 0.476461847m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 2706,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/2706/Logo_240x80.gif?1553699088",
                    InstitutionName = "Michigan State University",
                    ProgramRankScore = 0.4736072865m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 301674,
                            ProgramName = "Master of Science in Law Enforcement Intelligence and Analysis",
                            ProgramProductId = 630472,
                            ProgramRankScore = 0.47360966m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301673,
                            ProgramName = "Master of Science in Criminal Justice",
                            ProgramProductId = 630471,
                            ProgramRankScore = 0.473604913m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 143,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/143/Logo_240x80.gif?1497895922",
                    InstitutionName = "Saint Joseph's University",
                    ProgramRankScore = 0.4676956880769230769230769231m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 6174,
                            ProgramName = "MS in Criminal Justice",
                            ProgramProductId = 477497,
                            ProgramRankScore = 0.467699347m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298353,
                            ProgramName = "MS in Health Administration - General Concentration",
                            ProgramProductId = 616977,
                            ProgramRankScore = 0.467698733m,
                            ProgramTemplateId = 230
                        },
                        new Program
                        {
                            ProgramId = 178652,
                            ProgramName = "MS in Criminal Justice - Intelligence and Crime Analysis Specialization",
                            ProgramProductId = 477501,
                            ProgramRankScore = 0.467698121m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 298354,
                            ProgramName = "MS in Health Administration - Organizational Development and Leadership",
                            ProgramProductId = 616978,
                            ProgramRankScore = 0.467698068m,
                            ProgramTemplateId = 230
                        },
                        new Program
                        {
                            ProgramId = 178651,
                            ProgramName = "MS in Criminal Justice - Federal Law Enforcement Specialization",
                            ProgramProductId = 477499,
                            ProgramRankScore = 0.467696796m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 6170,
                            ProgramName = "MS in Business Intelligence and Analytics",
                            ProgramProductId = 479556,
                            ProgramRankScore = 0.467696406m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 6173,
                            ProgramName = "MS in Special Education",
                            ProgramProductId = 482368,
                            ProgramRankScore = 0.467696089m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 6175,
                            ProgramName = "MS in Criminal Justice - Homeland Security Specialization",
                            ProgramProductId = 477500,
                            ProgramRankScore = 0.467694366m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 6176,
                            ProgramName = "MS in Criminal Justice - Behavior Analysis Specialization",
                            ProgramProductId = 477498,
                            ProgramRankScore = 0.467694164m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 6180,
                            ProgramName = "MS in Health Administration - Informatics Specialization",
                            ProgramProductId = 477503,
                            ProgramRankScore = 0.467694046m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 293164,
                            ProgramName = "MS in Strategic Human Resource Management",
                            ProgramProductId = 572476,
                            ProgramRankScore = 0.46769353m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 6172,
                            ProgramName = "MS in Secondary Education (with OATCERT)",
                            ProgramProductId = 482367,
                            ProgramRankScore = 0.467692390m,
                            ProgramTemplateId = 4
                        },
                        new Program
                        {
                            ProgramId = 6181,
                            ProgramName = "MS in Organization Development and Leadership",
                            ProgramProductId = 479557,
                            ProgramRankScore = 0.467691889m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 8585,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/8585/Logo_240x80.gif?1556118509",
                    InstitutionName = "University of Tennessee - Knoxville",
                    ProgramRankScore = 0.459961843m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 301922,
                            ProgramName = "MS Supply Chain Managment",
                            ProgramProductId = 631437,
                            ProgramRankScore = 0.459961843m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 1645,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/1645/Logo_240x80.gif?1555332968",
                    InstitutionName = "Fairfield University",
                    ProgramRankScore = 0.456338059m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 301861,
                            ProgramName = "Master of Science in Finance",
                            ProgramProductId = 630951,
                            ProgramRankScore = 0.456338887m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301860,
                            ProgramName = "Master of Science in Business Analytics",
                            ProgramProductId = 630950,
                            ProgramRankScore = 0.456337231m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 244,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/244/Logo_240x80.gif?1497968722",
                    InstitutionName = "Lasell College",
                    ProgramRankScore = 0.4558701394m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 21156,
                            ProgramName = "Master of Science in Sport Management: Sport Leadership",
                            ProgramProductId = 265830,
                            ProgramRankScore = 0.471969746m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 271598,
                            ProgramName = "Master of Science in Management: Management",
                            ProgramProductId = 468590,
                            ProgramRankScore = 0.471969642m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 271594,
                            ProgramName = "Master of Science in Communication: Health Communication",
                            ProgramProductId = 468586,
                            ProgramRankScore = 0.471969281m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 271603,
                            ProgramName = "Master of Science in Communication: Integrated Marketing Communication",
                            ProgramProductId = 518362,
                            ProgramRankScore = 0.471968673m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295478,
                            ProgramName = "Master of Science in Sport Management: Athletic Administration",
                            ProgramProductId = 596604,
                            ProgramRankScore = 0.471967888m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295479,
                            ProgramName = "Master of Science in Sport Management: Parks and Recreation",
                            ProgramProductId = 596606,
                            ProgramRankScore = 0.471967816m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295892,
                            ProgramName = "Master of Science in Project Management",
                            ProgramProductId = 597872,
                            ProgramRankScore = 0.471967666m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 270950,
                            ProgramName = "Master of Science in Criminal Justice: Violence Prevention and Advocacy",
                            ProgramProductId = 467208,
                            ProgramRankScore = 0.471967203m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 193543,
                            ProgramName = "Master of Science in Communication: Public Relations",
                            ProgramProductId = 597822,
                            ProgramRankScore = 0.471967012m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 296374,
                            ProgramName = "Master of Business Administration (MBA)",
                            ProgramProductId = 600540,
                            ProgramRankScore = 0.471966776m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 270947,
                            ProgramName = "Master of Science in Criminal Justice: Emergency and Crisis Management",
                            ProgramProductId = 467205,
                            ProgramRankScore = 0.471966637m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 274365,
                            ProgramName = "Master of Science in Rehabilitation Science",
                            ProgramProductId = 512426,
                            ProgramRankScore = 0.471966073m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 270951,
                            ProgramName = "Master of Science in Management: Hospitality and Event Management",
                            ProgramProductId = 467209,
                            ProgramRankScore = 0.471966054m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 3993,
                            ProgramName = "Master of Science in Human Resources",
                            ProgramProductId = 265810,
                            ProgramRankScore = 0.471966019m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 21155,
                            ProgramName = "Master of Science in Sport Management: Sport Tourism & Hospitality Management",
                            ProgramProductId = 265829,
                            ProgramRankScore = 0.471965126m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 270948,
                            ProgramName = "Master of Science in Criminal Justice: Homeland Security and Global Justice",
                            ProgramProductId = 467206,
                            ProgramRankScore = 0.471964937m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295875,
                            ProgramName = "Master of Science in Nutrition for Human Performance",
                            ProgramProductId = 597826,
                            ProgramRankScore = 0.471961114m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 301931,
                            ProgramName = "Master of Science in Athletic Training",
                            ProgramProductId = 631470,
                            ProgramRankScore = 0.421668327m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 40541,
                            ProgramName = "Master of Science in Human Resources",
                            ProgramProductId = 265803,
                            ProgramRankScore = 0.421666511m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 296375,
                            ProgramName = "Master of Science in Communication: Public Relations ",
                            ProgramProductId = 600543,
                            ProgramRankScore = 0.421665821m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 193540,
                            ProgramName = "Master of Science in Communication: Health Communication",
                            ProgramProductId = 265805,
                            ProgramRankScore = 0.421664962m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 193541,
                            ProgramName = "Master of Science in Communication: Integrated Marketing Communication",
                            ProgramProductId = 265806,
                            ProgramRankScore = 0.421663641m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 26231,
                            ProgramName = "Master of Science in Management: Management",
                            ProgramProductId = 283583,
                            ProgramRankScore = 0.421663591m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 299615,
                            ProgramName = "Master of Science in Marketing",
                            ProgramProductId = 620733,
                            ProgramRankScore = 0.421661855m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 270953,
                            ProgramName = "Master of Science in Management: Hospitality and Event Management",
                            ProgramProductId = 467204,
                            ProgramRankScore = 0.421661114m,
                            ProgramTemplateId = 2
                        }
                    }
                },
                new Institution
                {
                    InstitutionId = 239,
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/239/Logo_240x80.gif?1491916297",
                    InstitutionName = "Lewis University",
                    ProgramRankScore = 0.4509383127777777777777777778m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 295394,
                            ProgramName = "MS in Data Science - Computational Biology and Bioinformatics",
                            ProgramProductId = 596267,
                            ProgramRankScore = 0.477468504m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295395,
                            ProgramName = "Master of Science in Data Science - Computer Science",
                            ProgramProductId = 596269,
                            ProgramRankScore = 0.477467866m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 291282,
                            ProgramName = "MS in Computer Science - Cyber Security",
                            ProgramProductId = 557573,
                            ProgramRankScore = 0.477467826m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295997,
                            ProgramName = "MA in Organizational Leadership",
                            ProgramProductId = 598201,
                            ProgramRankScore = 0.477465272m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 178649,
                            ProgramName = "MA in Organizational Leadership - Professional and Executive Coaching",
                            ProgramProductId = 549370,
                            ProgramRankScore = 0.477465206m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 4228,
                            ProgramName = "MA in Organizational Leadership - Nonprofit Management",
                            ProgramProductId = 510160,
                            ProgramRankScore = 0.477464806m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 4231,
                            ProgramName = "MA in Organizational Leadership - Higher Education - Student Services",
                            ProgramProductId = 510163,
                            ProgramRankScore = 0.477464459m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 4230,
                            ProgramName = "MA in Organizational Leadership - Training and Development",
                            ProgramProductId = 510162,
                            ProgramRankScore = 0.477463963m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302229,
                            ProgramName = "MS in Computer Science - Digital Forensics",
                            ProgramProductId = 634264,
                            ProgramRankScore = 0.477463501m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 192850,
                            ProgramName = "MS in Data Science",
                            ProgramProductId = 549380,
                            ProgramRankScore = 0.477462843m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 291284,
                            ProgramName = "MS in Computer Science - Software Engineering",
                            ProgramProductId = 557575,
                            ProgramRankScore = 0.477462570m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302231,
                            ProgramName = "MS in Computer Science - Game and Simulation Programming",
                            ProgramProductId = 634266,
                            ProgramRankScore = 0.477462443m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302230,
                            ProgramName = "MS in Computer Science - Enterprise and Cloud Computing",
                            ProgramProductId = 634265,
                            ProgramRankScore = 0.477462365m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 4227,
                            ProgramName = "MA in Organizational Leadership - Organizational Management",
                            ProgramProductId = 510159,
                            ProgramRankScore = 0.477462313m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 295393,
                            ProgramName = "MS in Computer Science - Non-Declared",
                            ProgramProductId = 596265,
                            ProgramRankScore = 0.477462283m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 302232,
                            ProgramName = "MS in Computer Science - Artificial Intelligence",
                            ProgramProductId = 634267,
                            ProgramRankScore = 0.477461814m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 22043,
                            ProgramName = "MS in Criminal Justice",
                            ProgramProductId = 510015,
                            ProgramRankScore = 0.477461596m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 257060,
                            ProgramName = "MS in Public Safety Administration",
                            ProgramProductId = 510016,
                            ProgramRankScore = 0m,
                            ProgramTemplateId = 2
                        }
                    }
                }
            };

        }

    }
}
