using EDDY.IS.FormsEngine.Core.Services;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Services.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Infastructure.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.Web;

namespace EDDY.IS.FormsEngine.Services
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Newtonsoft.Json.JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = jSettings;
            RegisterDIContainer();

            Task.Run(() => WarmupCache());
            Task.Run(() => WarmupComponentRenderingServiceCache());
        }

        protected void Application_End()
        {

        }

        private void WarmupCache()
        {
            try
            {
                new FormsEngine().WarmupCache();
                var val = new Validation.ValidationEngine();
                val.WarmupCache();
                val.WarmupEmailCache();
                
            }
            catch(Exception ex)
            {
                new IS.Core.Logging.ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
        }

        private void WarmupComponentRenderingServiceCache()
        {
            var fileReaderService = new FileReaderService();
            var configurationService = new ConfigurationService();
            var componentTemplateFilePathService = new ComponentTemplateService(configurationService);
            var templatingEngineService = new TemplatingEngineService();
            var componentCompilingService = new ComponentCompilingService(fileReaderService, componentTemplateFilePathService, templatingEngineService);

            componentCompilingService.CompileAllComponents(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void RegisterDIContainer()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.Register<ILogoUrlFormattingService, LogoUrlFormattingService>(Lifestyle.Scoped);
            container.Register<IInstitutionRepository, InstitutionRepository>(Lifestyle.Scoped);
            container.Register<IInstitutionService, InstitutionService>(Lifestyle.Scoped);
            container.Register<IFlagService, FlagService>(Lifestyle.Scoped);
            container.Register<IConfigurationService, ConfigurationService>(Lifestyle.Scoped);
            container.Register<IFailedMatchReplacementService, FailedMatchReplacementService>(Lifestyle.Scoped);
            container.Register<IProgramValidationService, ProgramValidationService>(Lifestyle.Scoped);
            container.Register<IProgramValidationRepository, ProgramValidationRepository>(Lifestyle.Scoped);
            container.Register<IComponentRenderingService, ComponentRenderingService>(Lifestyle.Scoped);
            container.Register<IFileReaderService, FileReaderService>(Lifestyle.Scoped);
            container.Register<IUserSelectionRepository, UserSelectionRepository>(Lifestyle.Scoped);
            container.Register<IUserSelectionService, UserSelectionService>(Lifestyle.Scoped);
            container.Register<IMatchValidationService, MatchValidationService>(Lifestyle.Scoped);
            container.Register<IIPAddressService, IPAddressService>(Lifestyle.Scoped);
            container.Register<ISubmissionService, SubmissionService>(Lifestyle.Scoped);
            container.Register<IFormValidationService, FormValidationService>(Lifestyle.Scoped);
            container.Register<IFormValidationRepository, FormValidationRepository>(Lifestyle.Scoped);
            container.Register<IProspectService, Core.Services.ProspectService>(Lifestyle.Scoped);
            container.Register<IProspectRepository, ProspectRepository>(Lifestyle.Scoped);
            container.Register<ILeadSubmissionService, LeadSubmissionService>(Lifestyle.Scoped);
            container.Register<ILeadRepository, LeadRepository>(Lifestyle.Scoped);
            container.Register<ILocationValidationService, LocationValidationService>(Lifestyle.Scoped);
            container.Register<ILocationValidationRepository, LocationValidationRepository>(Lifestyle.Scoped);
            container.Register<ISessionService, SessionService>(Lifestyle.Scoped);
            container.Register<ISessionRepository, SessionRepository>(Lifestyle.Scoped);
            container.Register<IComponentTemplateService, ComponentTemplateService>(Lifestyle.Scoped);
            container.Register<ITemplatingEngineService, TemplatingEngineService>(Lifestyle.Scoped);
            container.Register<IProgramService, ProgramService>(Lifestyle.Scoped);
            container.Register<IProgramRepository, ProgramRepository>(Lifestyle.Scoped);
            container.Register<IThankYouPageService, ThankYouPageService>(Lifestyle.Scoped);
            container.Register<IHtmlRenderingStrategyService, HtmlRenderingStrategyService>(Lifestyle.Scoped);
            container.Register<IHtmlRenderingStrategyRepository, HtmlRenderingStrategyRepository>(Lifestyle.Scoped);
            container.Register<IMetaDataRepository, MetaDataRepository>(Lifestyle.Scoped);
            container.Register<IMetaDataService, MetaDataService>(Lifestyle.Scoped);
            container.Register<ICCPAMessageService, CCPAMessageService>(Lifestyle.Scoped);

            container.Register<StaticController>(Lifestyle.Scoped);
            container.Register<DataBindController>(Lifestyle.Scoped);

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }


    }
}