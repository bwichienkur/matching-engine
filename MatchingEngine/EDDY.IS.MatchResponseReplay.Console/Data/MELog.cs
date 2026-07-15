using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EDDY.IS.MatchResponseReplay.Console.Data
{
    public partial class MELog : DbContext
    {
        public MELog()
            : base("name=MELog")
        {
        }

        public virtual DbSet<MatchResponse> MatchResponses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchResponse>()
                .Property(e => e.RequestMethodName)
                .IsUnicode(false);

            modelBuilder.Entity<MatchResponse>()
                .Property(e => e.RequestInput)
                .IsUnicode(false);

            modelBuilder.Entity<MatchResponse>()
                .Property(e => e.ServerName)
                .IsUnicode(false);
        }
    }
}
