using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.FormsEngine.Tests.TestData;
using FluentAssertions;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.Mappers;
using System.Web;
using System.Collections.Specialized;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound.Tests
{
    public class LeadCreateRequestMapperTests
    {
        private readonly MockMatchesTestData _mockMatchesTestData;

        public LeadCreateRequestMapperTests()
        {
            _mockMatchesTestData = new MockMatchesTestData();
        }

        [Fact]
        public void MapFormInputToLeadCreateRequestTest_NotNull()
        {
            var formInput = new FormInput();

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.NotNull(leadCreateRequests);
        }

        [Theory]
        [InlineData(100)]
        public void MapFormInputToLeadCreateRequestTest_TemplateIdMapped(int templateId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.TemplateId = templateId;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach(var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(templateId, leadCreateRequest.TemplateId);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void MapFormInputToLeadCreateRequestTest_ProgramProductIdMapped(int? alternativeProgramProductId)
        {
            var formInput = GetFormInputWithMatches();

            foreach (var match in formInput.Matches)
            {
                match.AlternativeProgramProductId = alternativeProgramProductId;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<int> expected = formInput.Matches.Select(m => m.ProgramProductId).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, programProductId) => leadCreateRequest.ProgramProductId == programProductId);
        }

        [Fact]
        public void MapFormInputToLeadCreateRequestTest_AlternativeProgramProductIdsMapped()
        {
            var formInput = GetFormInputWithMatches();

            foreach (var match in formInput.Matches)
            {
                match.AlternativeProgramProductId = match.ProgramProductId + 1000;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<int?> expected = formInput.Matches.Select(m => m.AlternativeProgramProductId).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, alternativeProgramProductId) => leadCreateRequest.ProgramProductId == alternativeProgramProductId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToLeadCreateRequestTest_IsBetaMapped(bool isBeta)
        {
            var formInput = GetFormInputWithMatches();
            formInput.IsBeta = isBeta;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(isBeta, leadCreateRequest.IsBeta);
            }
        }

        [Theory]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormInputToLeadCreateRequestTest_TrackIdMapped(string trackId)
        {
            var trackGuid = new Guid(trackId);

            var formInput = GetFormInputWithMatches();
            formInput.TrackId = trackGuid;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(trackGuid, leadCreateRequest.TrackId);
            }
        }

        [Theory]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        [InlineData("invalid guid")]
        public void MapFormInputToLeadCreateRequestTest_TrackingSessionGuidMapped(string sessionId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.SessionId = sessionId;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            Guid.TryParse(sessionId, out Guid sessionGuid);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(sessionGuid, leadCreateRequest.TrackingSessionGUID);
            }
        }

        [Theory]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        [InlineData("invalid guid")]
        public void MapFormInputToLeadCreateRequestTest_MatchResponseGuidMapped(string matchResponseId)
        {
            Guid.TryParse(matchResponseId, out Guid matchResponseGuid);

            var formInput = GetFormInputWithMatches();
            formInput.MatchResponseGuid = matchResponseGuid;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(matchResponseGuid, leadCreateRequest.MatchResponseGuid);
            }
        }

        [Theory]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        [InlineData("invalid guid")]
        public void MapFormInputToLeadCreateRequestTest_LimboAlternativeCampaignTrackidMapped(string limboAlternativeCampaignTrackId)
        {
            Guid.TryParse(limboAlternativeCampaignTrackId, out Guid limboAlternativeCampaignTrackIdGuid);

            var formInput = GetFormInputWithMatches();
            formInput.LimboAlternativeCampaignTrackId = limboAlternativeCampaignTrackIdGuid;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(limboAlternativeCampaignTrackIdGuid, leadCreateRequest.LimboAlternativeCampaignTrackid);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void MapFormInputToLeadCreateRequestTest_LimboAlternativeCampaignTrackidUtilizedMapped(bool limboAlternativeCampaignTrackIdUtilized)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LimboAlternativeCampaignTrackIdUtilized = limboAlternativeCampaignTrackIdUtilized;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(limboAlternativeCampaignTrackIdUtilized, leadCreateRequest.LimboAlternativeCampaignTrackidUtilized);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(645486)]
        public void MapFormInputToLeadCreateRequestTest_ProspectIdMapped(int? prospectId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Core.Models.Prospect
            {
                ProspectId = prospectId
            };

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(prospectId, leadCreateRequest.ProspectId);
            }
        }

        [Theory]
        [InlineData("Postal_Code=33449&Military_Affiliation=100&Age=25")]
        [InlineData("Postal_Code=33449&Military_Affiliation=100")]
        [InlineData("Postal_Code=33449")]
        [InlineData("Military_Affiliation=100&Age=25")]
        [InlineData("Age=25")]
        [InlineData("Military_Affiliation=100")]
        public void MapFormInputToLeadCreateRequestTest_LeadDataMapped(string leadData)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadData = leadData;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Dictionary<string,string> parameters = SplitQueryStringParams(leadData);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                foreach (var parameter in parameters)
                {
                    Assert.True(leadCreateRequest.LeadData.TryGetValue(parameter.Key, out string paramVal));
                    Assert.Equal(parameter.Value, paramVal);
                }
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(LeadCreationType.HostAndPost)]
        [InlineData(LeadCreationType.Apollo)]
        [InlineData(LeadCreationType.InstitutionFormInitial)]
        [InlineData(LeadCreationType.InstitutionFormCrossSell)]
        [InlineData(LeadCreationType.ProgramWizardInitial)]
        [InlineData(LeadCreationType.HomeSecurity)]
        [InlineData(LeadCreationType.WizardUserSelection)]
        [InlineData(LeadCreationType.WizardSmartMatch)]
        [InlineData(LeadCreationType.ThirdPartySmartMatch)]
        [InlineData(LeadCreationType.ProgramWizardUserSelection)]
        public void MapFormInputToLeadCreateRequestTest_LeadCreationTypeIdMapped(LeadCreationType? leadCreationType)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadCreationType = leadCreationType;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal((int?)leadCreationType, leadCreateRequest.LeadCreationTypeId);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("10")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormInputToLeadCreateRequestTest_InitialLeadIdMapped(string initialLeadId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.InitialLeadId = initialLeadId;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            string expected = string.IsNullOrWhiteSpace(initialLeadId) ? null : initialLeadId;

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(expected, leadCreateRequest.InitialLeadId);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(645486)]
        public void MapFormInputToLeadCreateRequestTest_PClientRelationContactIdMapped(int? advisorId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.AdvisorId = advisorId;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(advisorId, leadCreateRequest.ClientRelationContactId);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(645)]
        public void MapFormInputToLeadCreateRequestTest_TemplateIdAddedToLeadDataDictionary(int templateId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.TemplateId = templateId;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(leadCreateRequest.LeadData["templateid"], templateId.ToString());
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("Program_Of_Interest=189789")]
        public void MapFormInputToLeadCreateRequestTest_RemovesProgramOfInterestFromLeadDataDictionary(string leadData)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadData = leadData;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.False(leadCreateRequest.LeadData.ContainsKey("Program_Of_Interest"));
            }
        }

        [Theory]
        [InlineData("Postal_Code=33449&Military_Affiliation=100&Age=25")]
        [InlineData("Postal_Code=33449&Military_Affiliation=100")]
        [InlineData("Postal_Code=33449")]
        [InlineData("Military_Affiliation=100&Age=25")]
        [InlineData("Age=25")]
        [InlineData("Military_Affiliation=100")]
        public void MapFormInputToLeadCreateRequestTest_LeadAdditionalDataMapped(string leadAdditonalData)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadAdditionalData = leadAdditonalData;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            var expectedResult = SplitQueryStringParams(leadAdditonalData);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                Assert.Equal(expectedResult, leadCreateRequest.LeadAdditionalData);
            }
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, null)]
        [InlineData(null, 2)]
        [InlineData(null, null)]
        public void MapFormInputToLeadCreateRequestTest_CountryKey_And_StateKey_Added_To_LeadCreateRequestLeadAdditionalData_When_FormInputLeadAdditionalData_Is_Null(int? countryId, int? stateId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Prospect
            {
                CountryId = countryId,
                StateId = stateId
            };

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                var actualCountryKey = leadCreateRequest.LeadAdditionalData.TryGetValue("country-key", out string countryKey) ? countryKey : null;
                var actualStateKey = leadCreateRequest.LeadAdditionalData.TryGetValue("state-key", out string stateKey) ? stateKey : null;

                Assert.Equal(countryId?.ToString(), actualCountryKey);
                Assert.Equal(stateId?.ToString(), actualStateKey);
            }
        }

        [Theory]
        [InlineData("", null)]
        [MemberData(nameof(GetPhoneNumberTestCases), "phone")]
        public void MapFormInputToLeadCreateRequestTest_PhoneNumberInLeadDataDictionaryIsClean(string leadData, string expectedResult)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadData = leadData;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                leadCreateRequest.LeadData.TryGetValue("phone", out string phone);
                Assert.Equal(expectedResult, phone);
            }
        }

        [Theory]
        [InlineData("", null)]
        [MemberData(nameof(GetPhoneNumberTestCases), "alternate_phone")]
        public void MapFormInputToLeadCreateRequestTest_AlternatePhoneNumberInLeadDataDictionaryIsClean(string leadData, string expectedResult)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadData = leadData;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                leadCreateRequest.LeadData.TryGetValue("alternate_phone", out string phone);
                Assert.Equal(expectedResult, phone);
            }
        }

        [Theory]
        [InlineData("Postal_Code=334677", "US", "33467")]
        [InlineData("Postal_Code=VA3 4G75", "CA", "VA3 4G7")]
        [InlineData("Postal_Code=121546556", "CI", "121546556")]
        public void MapFormInputToLeadCreateRequestTest_PostalCodeInLeadDataDictionaryIsClean(string leadData, string countryCode, string expectedResult)
        {
            var formInput = GetFormInputWithMatches();
            formInput.LeadData = leadData;
            formInput.CountryCode = countryCode;

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                leadCreateRequest.LeadData.TryGetValue("postal_code", out string postalCode);
                Assert.Equal(expectedResult, postalCode);
            }
        }

        [Fact]
        public void MapFormInputToLeadCreateRequestTest_PaidStatusTypeMapped()
        {
            var formInput = GetFormInputWithMatches();

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<int?> expected = formInput.Matches.Select(m => m.PaidStatusType).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, paidStatusType) => leadCreateRequest.PaidStatusType == paidStatusType);
        }

        [Fact]
        public void MapFormInputToLeadCreateRequestTest_IsExternalDuplicateMapped()
        {
            var formInput = GetFormInputWithMatches();

            foreach (var match in formInput.Matches)
            {
                match.PassedValidation = false;
                match.IsExternalDuplicate = true;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<bool> expected = formInput.Matches.Select(m => m.IsExternalDuplicate).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, isExternalDuplicate) => leadCreateRequest.IsExternalDuplicate == isExternalDuplicate);
        }

        [Fact]
        public void MapFormInputToLeadCreateRequestTest_IsInternalDuplicateMapped()
        {
            var formInput = GetFormInputWithMatches();

            foreach (var match in formInput.Matches)
            {
                match.PassedValidation = false;
                match.IsInternalDuplicate = true;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<bool> expected = formInput.Matches.Select(m => m.IsInternalDuplicate).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, isInternalDuplicate) => leadCreateRequest.IsInternalDuplicate == isInternalDuplicate);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public void MapFormInputToLeadCreateRequestTest_PassedValidationMapped(bool matchPassedValidation, bool formPassedValidation)
        {
            var formInput = GetFormInputWithMatches();
            formInput.FormValidationResult = new FormValidationResult { Valid = formPassedValidation };

            foreach (var match in formInput.Matches)
            {
                match.PassedValidation = matchPassedValidation;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<bool> expected = formInput.Matches.Select(m => m.PassedValidation && formInput.FormValidationResult.Valid).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, passedValidation) => leadCreateRequest.PassedValidation == passedValidation);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToLeadCreateRequestTest_InitialLeadValidationFailedReasonAddedToLeadDataDictionary(bool formPassedValidation)
        {
            var formInput = GetFormInputWithMatches();
            formInput.FormValidationResult = new FormValidationResult { Valid = formPassedValidation };

            foreach (var match in formInput.Matches)
            {
                match.PassedValidation = false;
                match.RuleFailures = GetShuffledRuleFailures();
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            List<KeyValuePair<string, string>> expected = formInput.Matches.Select(m => m.RuleFailures.Last()).ToList();

            leadCreateRequests.Should().Equal(expected, (leadCreateRequest, ruleFailure) => leadCreateRequest.LeadData["InitialLeadValidationFailedReason"] == $"{ruleFailure.Key}-{ruleFailure.Value}");
        }

        [Theory]
        [InlineData(true, true, "0")]
        [InlineData(false, true, "1")]
        [InlineData(true, false, "1")]
        [InlineData(false, false, "1")]
        public void MapFormInputToLeadCreateRequestTest_InitialLeadValidationFailedAddedToLeadDataDictionary(bool matchPassedValidation, bool formPassedValidation, string expectedResult)
        {
            var formInput = GetFormInputWithMatches();
            formInput.FormValidationResult = new FormValidationResult { Valid = formPassedValidation };

            foreach (var match in formInput.Matches)
            {
                match.PassedValidation = matchPassedValidation;
            }

            List<LeadCreateRequest> leadCreateRequests = GetLeadCreateRequests(formInput);

            Assert.Equal(formInput.Matches.Count, leadCreateRequests.Count);

            foreach (var leadCreateRequest in leadCreateRequests)
            {
                leadCreateRequest.LeadData.TryGetValue("InitialLeadValidationFailed", out string initialLeadValidationFailed);
                Assert.Equal(expectedResult, initialLeadValidationFailed);
            }
        }

        private List<KeyValuePair<string, string>> GetShuffledRuleFailures()
        {
            var ruleFailures = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("ProgramNotAvailable", "Program Not Available"),
                new KeyValuePair<string, string>("ExternalDuplicate", "External Duplicate"),
                new KeyValuePair<string, string>("InternalDuplicate", "Internal Duplicate")
            };

            return ruleFailures.OrderBy(r => Guid.NewGuid()).ToList();
        }

        private FormInput GetFormInputWithMatches()
        {
            return new FormInput
            {
                Matches = _mockMatchesTestData.GetMockMatches(5)
            };
        }

        private List<LeadCreateRequest> GetLeadCreateRequests(FormInput formInput)
        {
            var mapper = new LeadCreateRequestMapper();
            List<LeadCreateRequest> leadCreateRequests = mapper.MapFormInputToLeadCreateRequest(formInput);

            return leadCreateRequests;
        }

        private Dictionary<string, string> SplitQueryStringParams(string parameters)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            List<string> fieldList = parameters.Split('&').ToList();

            foreach (var field in fieldList)
            {
                string[] splitField = field.Split('=');
                if (splitField.Length == 2)
                {
                    result.Add(splitField[0], splitField[1]);
                }
            }

            return result;
        }

        public static IEnumerable<object[]> GetPhoneNumberTestCases(string phoneParamName)
        {
            return new List<object[]>
            {
                new object[] { $"{phoneParamName}=5611234567", "5611234567" },
                new object[] { $"{phoneParamName}=561-123-4567", "5611234567" },
                new object[] { $"{phoneParamName}=561/123/4567", "5611234567" },
                new object[] { $"{phoneParamName}=(561)-123-4567", "5611234567" },
                new object[] { $"{phoneParamName}=(561)(123)(4567)", "5611234567" },
                new object[] { $"{phoneParamName}=15611234567", "15611234567" },
                new object[] { $"{phoneParamName}=1561-123-4567", "15611234567" },
                new object[] { $"{phoneParamName}=1561/123/4567", "15611234567" },
                new object[] { $"{phoneParamName}=1(561)-123-4567", "15611234567" },
                new object[] { $"{phoneParamName}=1(561)(123)(4567)", "15611234567" }
            };
        }
    }
}