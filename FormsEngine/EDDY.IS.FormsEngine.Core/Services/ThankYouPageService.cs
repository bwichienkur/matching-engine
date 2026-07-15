using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Enums;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ThankYouPageService : IThankYouPageService
    {
        private readonly ISessionService _sessionService;
        private readonly IProgramService _programService;
        private readonly ILogoUrlFormattingService _logoUrlFormattingService;

        public ThankYouPageService(ISessionService sessionService, IProgramService programService, ILogoUrlFormattingService logoUrlFormattingService)
        {
            _sessionService = sessionService;
            _programService = programService;
            _logoUrlFormattingService = logoUrlFormattingService;
        }

        public ThankYouPage GetThankYouPage(FormRequest formRequest)
        {
            string userFullName = _sessionService.GetUserFullName(formRequest.FESessionId);
            var programIdsFromSession = _sessionService.GetProgramIdsFromLeads(formRequest.FESessionId);
            var programs = _programService.GetPrograms(formRequest, programIdsFromSession);

            var thankYouPage = new ThankYouPage
            {
                Theme = formRequest.Theme,
                UserFullName = userFullName
            };

            for (int i = 0; i < programs?.Count; i++)
            {
                Program program = programs[i];
                ThankYouPageLineItem thankYouPageLineItem = MapProgramToThankYouPageLineItem(program);

                if (thankYouPageLineItem != null)
                {
                    thankYouPage.LineItems.Add(thankYouPageLineItem);
                }

                if (program?.ProgramId > 0)
                {
                    thankYouPage.SchoolPickerMatchProgramIdList.Add(program.ProgramId);
                }
            }

            IEnumerable<string> leadIdsFromSession = _sessionService.GetLeadIds(formRequest.FESessionId)?.Select(l => l.ToString()) ?? Enumerable.Empty<string>();
            thankYouPage.LeadList = string.Join(",", leadIdsFromSession);

            // online campuses first
            thankYouPage.LineItems = thankYouPage.LineItems.OrderBy(l => l.CampusType).ToList();

            return thankYouPage;
        }

        private ThankYouPageLineItem MapProgramToThankYouPageLineItem(Program program)
        {
            ThankYouPageLineItem thankYouPageLineItem = null;

            if (program != null)
            {
                thankYouPageLineItem = new ThankYouPageLineItem
                {
                    CampusName = program.InstitutionName,
                    CampusDescription = program.InstitutionDescription,
                    CampusType = program.CampusType ?? CampusType.Ground,
                    ProgramName = program.ProgramName,
                    ProgramDescription = RemoveHtmlTags(program.ProgramDescription),
                    LogoUrl = GetFormattedUrlPath(program),
                    ShouldShowLogo = LogoIsAllowedToBeShownForProduct(program.ProductId)
                };
            }

            return thankYouPageLineItem;
        }

        private string RemoveHtmlTags(string valueWithHtmlTags)
        {
            string valueWithoutHtmlTags = string.Empty;

            if (!string.IsNullOrWhiteSpace(valueWithHtmlTags))
            {
                valueWithoutHtmlTags = Regex.Replace(valueWithHtmlTags, @"<(.|\n)*?>", string.Empty);
            }

            return valueWithoutHtmlTags;
        }

        private string GetFormattedUrlPath(Program program)
        {
            string unformattedLogoUrl = string.Empty;

            if (!string.IsNullOrWhiteSpace(program.CampusLogoUrl))
            {
                unformattedLogoUrl = program.CampusLogoUrl;
            }
            else if (!string.IsNullOrWhiteSpace(program.InstitutionLogoUrl))
            {
                unformattedLogoUrl = program.InstitutionLogoUrl;
            }

            return _logoUrlFormattingService.GetSmallLogoFormattedUrl(unformattedLogoUrl);
        }

        private bool LogoIsAllowedToBeShownForProduct(int? productId)
        {
            bool logoIsAllowedToBeShown = true;

            if (productId > 0)
            {
                logoIsAllowedToBeShown = !Constants.ProductIdsToBeExcludedFromLogoPresentation.Contains(productId.GetValueOrDefault());
            }

            return logoIsAllowedToBeShown;
        }

    }
}
