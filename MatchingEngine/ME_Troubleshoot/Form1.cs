using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ME_Troubleshoot.MatchingService;
using Newtonsoft.Json;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ME_Troubleshoot
{
    public partial class Form1 : Form
    {
        private List<Record> records;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            foreach (Record r in records)
            {
                WizardMatchRequest tempRequest = JsonConvert.DeserializeObject<WizardMatchRequest>(r.MatchRequest);
                MatchingServiceClient msc = new MatchingServiceClient("CustomBinding_IMatchingService");
                ProgramValidateRequest pvr = new ProgramValidateRequest();
                pvr.Application = ISApplication.FormsEngine;
                pvr.ProgramProductId = r.ProgramProductId;
                pvr.ProspectInput = tempRequest.ProspectInput;
                pvr.TrackGuid = tempRequest.TrackGuid;
                pvr.LeadScoringTierLevel = tempRequest.LeadScoringInput != null ? tempRequest.LeadScoringInput.LeadScoringTierLevel : null;

                ProgramValidateResponse resp = msc.ValidateProgram(pvr);

                if (resp.RuleFailures != null && resp.RuleFailures.Count() > 0)
                    r.FailureReason = resp.RuleFailures[0].RuleFailureName;
                else if (resp.PassedValidation == false)
                    r.FailureReason = "Failed but Unknown";
                else
                    r.FailureReason = "No Rule Failure";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            records = new List<Record>();

            SqlConnection conn = new SqlConnection("Server=isdb.eddyprod.local;database=nexus;Integrated Security=SSPI");
            SqlCommand cmd = new SqlCommand("ME_Troubleshoot_Input", conn);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            SqlDataReader dr = null;

            try
            {
                dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Record r = new Record();
                        r.CampaignName = dr["CampaignName"].ToString();
                        r.LeadId = Convert.ToInt32(dr["LeadId"]);
                        r.MarketingUnit = dr["Name"].ToString();
                        r.MatchRequest = dr["RequestInput"].ToString();
                        r.MatchResponseGuid = dr["matchresponseguid"].ToString();
                        r.ProgramProductId = Convert.ToInt32(dr["ProgramProductId"]);
                        r.TrackId = dr["TrackId"].ToString();

                        records.Add(r);
                    }
                }
            }
            finally
            {
                dataGridView1.DataSource = records;

                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtWizardResponse.Text = "";

            if (dataGridView1.SelectedRows.Count == 1)
            {
                Record r = records[dataGridView1.SelectedRows[0].Index];

                WizardMatchRequest tempRequest = JsonConvert.DeserializeObject<WizardMatchRequest>(r.MatchRequest);
                MatchingServiceClient msc = new MatchingServiceClient("CustomBinding_IMatchingService");
                WizardMatchResponse resp = msc.GetWizardMatches(tempRequest);
                txtWizardResponse.Text = JsonConvert.SerializeObject(resp);
            }
        }
    }
}
