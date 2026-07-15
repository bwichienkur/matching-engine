using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class DisabledSubjectFilter : SubjectFilter
    {
        protected override void PerformValidation(string subjectIds, HttpActionContext actionContext, ref VendorResponseBase responseContent)
        {
            base.PerformValidation(subjectIds, actionContext, ref responseContent);
            if (!string.IsNullOrEmpty(subjectIds))
            {
                var subjectIdsArray = JsonConvert.DeserializeObject<string[]>(subjectIds);
                if (subjectIdsArray != null)
                {
                    foreach (string subjectIdString in subjectIdsArray)
                    {
                        int subjectId;
                        if (int.TryParse(subjectIdString, out subjectId))
                        {
                            var subject = Subjects.GetSubject(subjectId);
                            var subjectAvailable = subject == null ? false : subject.IsEnabled;
                            if (!subjectAvailable)
                            {
                                responseContent.MessageCodes.Add(InputValidation.MessageCodes.SubjectUnavailable);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}