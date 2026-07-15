using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO.Responses
{
    public class UserSelectionResponse
    {
        public Guid MatchResponseGuid { get; set; }
        public List<Campus> UserSelections { get; set; }
    }
}
