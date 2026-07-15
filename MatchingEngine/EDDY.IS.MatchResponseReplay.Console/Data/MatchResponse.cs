namespace EDDY.IS.MatchResponseReplay.Console.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MatchResponse")]
    public partial class MatchResponse
    {
        public int MatchResponseId { get; set; }

        public Guid MatchResponseGUID { get; set; }

        public bool IsRollup { get; set; }

        public int BusinessModelWeightSubjectId { get; set; }

        public int ProcessRequestMilliseconds { get; set; }

        public int RequestType { get; set; }

        [Required]
        [StringLength(30)]
        public string RequestMethodName { get; set; }

        [StringLength(4000)]
        public string RequestInput { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(30)]
        public string ServerName { get; set; }

        public Guid? TrackingDeviceGUID { get; set; }

        public Guid? TrackingSessionGUID { get; set; }

        public Guid? LeadScoringGuid { get; set; }

        public bool? PremierUpsellEligible { get; set; }

        public bool? ProspectZipAvailable { get; set; }

        public int? DistanceCliffValue { get; set; }

        public int? DistanceCapMultiplier { get; set; }

        public int? InstitutionGroupingType { get; set; }
    }
}
