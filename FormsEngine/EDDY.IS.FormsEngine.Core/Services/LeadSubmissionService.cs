using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class LeadSubmissionService : ILeadSubmissionService
    {

        private readonly ILeadRepository _leadRepository;
        private readonly ISessionService _sessionService;

        public LeadSubmissionService(ILeadRepository leadRepository, ISessionService sessionService)
        {
            _leadRepository = leadRepository;
            _sessionService = sessionService;
        }

        public SubmissionResponse SaveSchoolPickerWizardLeads(FormInput formInput)
        {
            var result = new SubmissionResponse();

            if (formInput?.Matches != null && formInput?.Prospect != null)
            {
                Dictionary<int, Match> matchMap = GetProgramProductIdMatchMap(formInput.Matches);
                List<Lead> leads = _leadRepository.SaveLeads(formInput);
                IEnumerable<Lead> successfulLeads = leads.Select(l => l).Where(l => l.Successful);
                List<int> programIds = GetProgramIdsFromLeads(successfulLeads, matchMap);

                _sessionService.SetProgramIdsFromLeads(formInput.FESessionId, programIds);
                _sessionService.SetLeadIds(formInput.FESessionId, successfulLeads?.Select(l => l.LeadId).ToList());

                result.ProspectId = formInput.Prospect.ProspectId;
                result.UserFullName = $"{formInput.Prospect.FirstName} {formInput.Prospect.LastName}";

                int successfulLeadCount = successfulLeads.Count();

                result.SchoolPickerLeadsCreatedCount = successfulLeadCount;
                result.Success = successfulLeadCount > 0;
                result.MoveToThankYou = result.Success;
                result.MoveToNoMatch = !result.Success;
            }

            return result;
        }

        private Dictionary<int, Match> GetProgramProductIdMatchMap(IEnumerable<Match> matches)
        {
            var matchMap = new Dictionary<int, Match>();

            if (matches?.Count() > 0)
            {
                foreach (var match in matches)
                {
                    matchMap.Add(match.ProgramProductId, match);
                }
            }

            return matchMap;
        }

        private List<int> GetProgramIdsFromLeads(IEnumerable<Lead> leads, Dictionary<int, Match> matchMap)
        {
            List<int> programIds = new List<int>();

            if (leads?.Count() > 0 && matchMap != null)
            {
                foreach (var lead in leads)
                {
                    if (lead.ProgramProductId.HasValue && matchMap.TryGetValue(lead.ProgramProductId.Value, out Match match))
                    {
                        programIds.Add(match.ProgramId);
                    }
                }
            }

            return programIds;
        }


    }
}
