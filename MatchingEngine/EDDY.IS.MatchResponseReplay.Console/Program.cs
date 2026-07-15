using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDDY.IS.MatchResponseReplay.Console
{
    class Program
    {
        const string USAGE = "Usage: MatchResponseReplay [MatchResponseID]\n";
        const string USAGE_OK = "MatchResponseReplay\n";
        static void Main(string[] args)
        {
            int MatchResponseId = 0;

            if (args.Count() != 1)
            {
                System.Console.WriteLine(USAGE);
                return;
            }
            
            System.Console.WriteLine(USAGE_OK);
           
            if (int.TryParse(args[0], out MatchResponseId))
            {
                new ReplayApp().Replay(MatchResponseId);
            }
            else
            {
                System.Console.WriteLine("\nERROR: Not able to parse [MatchResponseID] as integer");
            }
        }
                
    }
}
