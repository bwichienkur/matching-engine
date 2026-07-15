using System;

using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Common.ExceptionHandler;
using AutoMapper;

namespace EDDY.IS.Vendor.Business
{
    public class Logs
    {

        private LogsDAO logsDAO = new LogsDAO();

        public VendorResponseLog InitializeVendorResponseLogObject(VendorResponseBase vendorResponseBase)
        {
            VendorResponseLog vendorResponseLog = null;
            try
            {
                MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<VendorResponseBase, VendorResponseLog>());
                IMapper mapper = new Mapper(mapperConfiguration);
                vendorResponseLog = mapper.Map<VendorResponseBase, VendorResponseLog>(vendorResponseBase);

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseLog;
        }

        public VendorResponseBase LogVendorResponse(VendorResponseLog log)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                logsDAO.LogVendorResponse(log);

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }

        public VendorResponseBase LogEddyApiResponse(VendorResponseLog log)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                logsDAO.LogEddyApiResponse(log);

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
    }
}
