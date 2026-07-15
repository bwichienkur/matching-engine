using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;



namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class LeadUpdateValidationFilter : ActionFilterBase
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.ActionArguments["leadUpdateRequest"] as LeadUpdateRequest;

            if (request == null)
                return;

            VendorResponseBase responseContent = new VendorResponseBase
            {
                IsSuccessful = false,
                Body = "Validation Failed",
                ResponseGuid = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                ResponseDateTime = DateTime.Now,
                Messages = new List<VendorResponseMessage>()
            };

            if (!request.EMSInstitutionId.HasValue)
                AddMessage(responseContent, InputValidation.MessageCodes.InstitutionIdIsRequired);

            if (request.LeadStateId.HasValue &&
                request.LeadStateId != 1 &&
                request.LeadStateId != 2)
                AddMessage(responseContent, InputValidation.MessageCodes.LeadStateIdIsInvalid);

            if (request.LookUpKeyList == null || !request.LookUpKeyList.Any())
            {
                AddMessage(responseContent, InputValidation.MessageCodes.LookUpKeyListIsRequired);
                SetBadRequest(actionContext, responseContent);
                return;
            }

            bool hasAtLeastOneValidLookupValue = false;

            foreach (int lookupKeyValue in request.LookUpKeyList)
            {
                if (!Enum.IsDefined(typeof(ExchangeLeadUniqueKey), lookupKeyValue))
                {
                    AddMessage(responseContent, InputValidation.MessageCodes.LookUpKeyIsInvalid);
                    continue;
                }

                var lookupKey = (ExchangeLeadUniqueKey)lookupKeyValue;
                switch (lookupKey)
                {
                    case ExchangeLeadUniqueKey.ExternalId:
                        if (!string.IsNullOrWhiteSpace(request.ExternalId))
                            hasAtLeastOneValidLookupValue = true;
                        break;

                    case ExchangeLeadUniqueKey.EmailAddress:
                        if (!string.IsNullOrWhiteSpace(request.Email))
                            hasAtLeastOneValidLookupValue = true;
                        break;

                    case ExchangeLeadUniqueKey.ISLeadId:
                        if (request.ISLeadId.HasValue)
                            hasAtLeastOneValidLookupValue = true;
                        break;

                    case ExchangeLeadUniqueKey.Phone1:
                        if (!string.IsNullOrWhiteSpace(request.Phone))
                            hasAtLeastOneValidLookupValue = true;
                        break;

                    case ExchangeLeadUniqueKey.EMSLeadId:
                        if (request.EMSLeadId.HasValue)
                            hasAtLeastOneValidLookupValue = true;
                        break;

                    case ExchangeLeadUniqueKey.FirstLastName:
                        if (!string.IsNullOrWhiteSpace(request.FirstName) &&
                            !string.IsNullOrWhiteSpace(request.LastName))
                        {
                            hasAtLeastOneValidLookupValue = true;
                        }
                        break;

                    case ExchangeLeadUniqueKey.NameAndEmailorPhone:
                        if (!string.IsNullOrWhiteSpace(request.FirstName) &&
                            !string.IsNullOrWhiteSpace(request.LastName) &&
                            (!string.IsNullOrWhiteSpace(request.Email) ||
                             !string.IsNullOrWhiteSpace(request.Phone)))
                        {
                            hasAtLeastOneValidLookupValue = true;
                        }
                        break;
                }
            }

            if (!hasAtLeastOneValidLookupValue)
                AddMessage(responseContent, InputValidation.MessageCodes.LookUpValueIsRequired);

            if (responseContent.Messages.Any())
                SetBadRequest(actionContext, responseContent);
        }

        private void AddMessage(VendorResponseBase responseContent, string messageCode)
        {
            VendorResponseMessage message =
                vendorResponseMessages.GetVendorResponseMessageByMessageCode(messageCode);

            if (message != null)
                responseContent.Messages.Add(message);
        }

        private void SetBadRequest(
            HttpActionContext actionContext,
            VendorResponseBase responseContent)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new ObjectContent(
                    responseContent.GetType(),
                    responseContent,
                    new JsonMediaTypeFormatter())
            };
        }
    }
}