using EDDY.IS.Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                                                    , new InputRequired[] { InputRequired.Email, InputRequired.FirstName, InputRequired.LastName, InputRequired.Phone1, InputRequired.PostalCode, InputRequired.City }
                                                    , new RuleAttribute[] { RuleAttribute.ExecuteEMSRules }
                           )]
    public class SpamCheck : Rule, ICRProgramProductRule
    {
        public SpamCheck(RuleInput ri)
            : base(ri)
        {}

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
            var removed = new List<ProgramProductRuleInput>();

            HashSet<int> spamCheckSchools = new HashSet<int> { 9171, 9176 };
            bool isEnabled = bool.Parse(ConfigurationManager.AppSettings["SpamRuleEnabled"].ToString());
            if (input.Any() && isEnabled)
            {
                var inputToCheck = input.Values.FirstOrDefault();

                if (spamCheckSchools.Contains(inputToCheck.ClientRelationshipId))
                {
                    try
                    {
                        if (IsSpam(this.ruleInput.prospectData))
                        {
                            foreach (var i in input.Values)
                            {
                                i.BaseRuleType = BaseRuleDefinitionType.Spam;
                                i.RuleId = (int)BaseRuleDefinitionType.Spam;
                                i.RuleName = "Spam";

                                removed.Add(i);
                            }
                        }
                        else if (IsReportingOnlySpam(this.ruleInput.prospectData))
                        {
                            foreach (var i in input.Values)
                            {
                                i.BaseRuleType = BaseRuleDefinitionType.SpamReportingOnly;
                                i.RuleId = (int)BaseRuleDefinitionType.SpamReportingOnly;
                                i.RuleName = "Spam - Reporting Only";

                                removed.Add(i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EDDY.IS.Core.Logging.ISException e = new ISException(ex);
                    }
                }
            }

            output = removed;
        }

        public bool IsSpam(Prospect p)
        {
            bool result = false;

            //if first name or last name doesn't contain a vowel, email is in format of first middle initial lastname, or the phone number is a toll free number then mark as spam
            if (HasSpamEmailFormat(p))
                return true;
            
            return result;
        }

        private bool HasLessThanThreeCharactersInFirstAndLastName(string firstname, string lastname)
        {
            return (firstname.Length < 3 && lastname.Length < 3);
        }

        private bool ContainsVowel(string str)
        {
            if (String.IsNullOrEmpty(str))
                return false;

            //add logic so that if the length of the string is 2 or less then we allow it to go through
            if (str.Length <= 2)
                return true;
  
            string lcStr = str.ToLower();
            HashSet<char> vowels = new HashSet<char>() { 'a', 'e', 'i', 'o', 'u', 'y' };

            foreach (var vowel in vowels)
            {
                if (lcStr.Contains(vowel))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasFirstNameLastNameEmail(Prospect p)
        {
            bool result = false;

            if (!IsOnlyFirstCharacterUppercase(p.FirstName) || !IsOnlyFirstCharacterUppercase(p.LastName))
                return false;

            if (Regex.IsMatch(p.Email, string.Format($"^{Regex.Escape(p.FirstName)}[A-Z]{Regex.Escape(p.LastName)}@.*")))
                result = true;

            return result;
        }

        private bool HasSpamEmailFormat(Prospect p)
        {
            bool result = false;

            if (Regex.IsMatch(p.Email, string.Format($"^[A-Z][a-z]*[A-Z][A-Z][a-z]*@.*")))
                result = true;

            return result;
        }

        private bool IsOnlyFirstCharacterUppercase(string str)
        {
            bool result = true;

            if (string.IsNullOrEmpty(str))
                return true;

            if (!char.IsUpper(str[0]))
                return false;

            for (int i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                    return false;
            }

            return result;
        }
        public bool IsReportingOnlySpam(Prospect p)
        {
            bool result = false;

            //if first name or last name doesn't contain a vowel, email is in format of first middle initial lastname, or the phone number is a toll free number then mark as spam
            if (IsTollFreeNumber(p.Phone1) ||
                !ContainsVowel(p.FirstName) ||
                !ContainsVowel(p.LastName) ||
                HasLessThanThreeCharactersInFirstAndLastName(p.FirstName, p.LastName))
                return true;

            return result;
        }
        private bool IsTollFreeNumber(string phone)
        {
            if (String.IsNullOrEmpty(phone) || phone.Length < 4)
                return false;

            HashSet<string> tollFreeNumbers = new HashSet<string>() { "800", "888", "877", "866", "855", "844", "833" };

            string numToCheck = string.Empty;

            //if first digit is 1 then remove first digit, otherwise remove the last digit
            if (phone.First() == '1')
            {
                numToCheck = phone.Substring(1, 3);
            }
            else
            {
                numToCheck = phone.Substring(0, 3);
            }

            foreach (var tfn in tollFreeNumbers)
            {
                if (numToCheck.Contains(tfn))
                {
                    return true;
                }
            }

            return false;
        }


        //public Dictionary<string, List<Dictionary<string, string>>> CreateSpamCheckRequest(Prospect p, ProgramProductRuleInput ppr)
        //{
        //    MatchItem mi = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase).GetMatchItemByProgramProductId(ppr.ProgramProductId);

        //    return new Dictionary<string, List<Dictionary<string, string>>>()
        //    {
        //        {
        //            "data",
        //            new List<Dictionary<string, string>>()
        //            { 
        //                new Dictionary<string, string>()
        //                {
        //                    {
        //                        "ChannelId", ruleInput.Campaign.ChannelId.ToString()
        //                    },
        //                    {
        //                        "ProgramId", ppr.ProgramId.ToString()
        //                    },
        //                    {
        //                        "CampusId", mi.Match.CampusId.ToString()
        //                    },
        //                    {
        //                        "CampusTypeId", mi.Match.CampusCampusTypeId.ToString()
        //                    },
        //                    {
        //                        "FirstName", p.FirstName
        //                    },
        //                    {
        //                        "LastName", p.LastName
        //                    },
        //                    {
        //                        "City", p.City
        //                    },
        //                    {
        //                        "ZipCode", p.PostalCode
        //                    },
        //                    {
        //                        "StateProvince", null
        //                    },
        //                    {
        //                        "EmailAddress", p.Email
        //                    },
        //                    {
        //                        "Phone1", p.Phone1
        //                    },
        //                    {
        //                        "YearHighestEduCompleted", p.HSGraduationYear.HasValue ? p.HSGraduationYear.ToString() : "0"
        //                    },
        //                    {
        //                        "HighestLevelOfEdu", p.EducationLevelId.HasValue ? p.EducationLevelId.ToString() : "0"
        //                    },
        //                    {
        //                        "TrackId", ruleInput.Campaign.TrackId.ToString()
        //                    },
        //                    {
        //                        "LandingUrl", p.LandingURL
        //                    },
        //                    {
        //                        "utm_source", p.utm_source
        //                    },
        //                    {
        //                        "utm_medium", p.utm_medium
        //                    },
        //                    {
        //                        "utm_campaign", p.utm_campaign
        //                    },
        //                    {
        //                        "LeadUserAgent", null
        //                    },
        //                    {
        //                        "EddyIPAddress", null
        //                    },
        //                    {
        //                        "ClientRelationshipId", ppr.ClientRelationshipId.ToString()
        //                    },
        //                    {
        //                        "HasFirstNameLastNameEMail", GetHasFirstNameLastNameEmail(p)
        //                    }
        //                }
        //            }
        //        } 
        //    };
        //}

        //private string GetHasFirstNameLastNameEmail(Prospect p)
        //{
        //    string result = "0";

        //    if (Regex.IsMatch(p.Email, string.Format($"{Regex.Escape(p.FirstName)}.*{Regex.Escape(p.LastName)}@.*")))
        //        result = "1";

        //    return result;
        //}
    }
}
