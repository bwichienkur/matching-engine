using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchResponseReplay.Console.Data
{
    public class MELogDataService
    {
        public MatchResponse GetMatchResponse(int MatchResponseId)
        {
            MatchResponse Result = null;
            using (MELog dbContext = new MELog())
            {
                Result = dbContext.MatchResponses.FirstOrDefault(t => t.MatchResponseId == MatchResponseId);
            }

            return Result;
        }
    }
}
