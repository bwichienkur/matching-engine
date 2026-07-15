using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{   
    public class eRPL
    {
        public int ProgramProductId { get; set; }
        public decimal RPL { get; set; }
        public decimal ClickPrice { get; set; }
        public decimal ScrubRate { get; set; }
        public decimal eRPLFinal { get; set; }
    }

    public class StatePricing
    {
        public int StateId { get; set; }
        public decimal RPL { get; set; }
        public decimal ClickPrice { get; set; }
        public decimal ScrubRate { get; set; }
        public decimal eRPLFinal { get; set; }
    }

    //public class LeadCap
    //{
    //    public int ClientRelationProductMappingId { get; set; }
    //    public int LeadsDelivered { get; set; }
    //    public int CapTotal { get; set; }
    //    public decimal CapFinal { get; set; }
    //}

    public class Strategic
    {
        public int ProgramProductId { get; set; }
        public bool StrategicValue { get; set; }
    }

    public class FactorValueSet
    {
        public decimal eRPL { get; set; }
        public decimal ClickPrice { get; set; }
        public decimal CapRoom { get; set; }
        public decimal? CampusDistance { get; set; }
        public bool Strategic { get; set; }
    }
}
