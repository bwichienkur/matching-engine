using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Logging.DataModel
{
    public class MatchResponseDataService
    {
        public static void SaveMatchResponse(MatchResponse matchResponse)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.MatchResponses.Add(matchResponse);
                context.SaveChanges();
                //context.BulkInsert(new List<MatchResponse> { matchResponse }, options => options.IncludeGraph = true);
            }
        }

        public static void SaveMatchResponseDisplayed(ICollection<MatchResponseDisplayed> mrds)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.SaveChanges();
            }
        }

        public static void SaveMatchResponseAvailablePrograms(ICollection<MatchResponseAvailableProgram> mraps)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.BulkInsert(mraps, options => options.IncludeGraph = true);
            }
        }

        public static void SaveMatchResponseRemovalReasons(ICollection<MatchResponseRemovalReason> mrrs)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.BulkInsert(mrrs, options => options.IncludeGraph = true);
            }
        }

        public static void SaveMatchResponseRemovalReasonJsons(ICollection<MatchResponseRemovalReasonJson> mrrjs)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.BulkInsert(mrrjs, options => options.IncludeGraph = true);
            }
        }

        public static void SaveMatchResponseSearch(ICollection<MatchResponseSearch> mrs)
        {
            using (MatchLoggingModelContainer context = new MatchLoggingModelContainer())
            {
                context.BulkInsert(mrs, options => options.IncludeGraph = true);
            }
        }
    }
}
