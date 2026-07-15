using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class LogoUrlFormattingService : ILogoUrlFormattingService
    {
        private readonly string _eddyLogoImageSizeSmall;
        private readonly string _eddyLogoImageSizeMedium;
        private readonly string _eddyLogoImageSizeLarge;
        private readonly string _eddyLogoImagePathDomain;
        private readonly string _eddyLogoImageFileName;

        public LogoUrlFormattingService(IConfigurationService configurationService)
        {
            _eddyLogoImageSizeSmall = configurationService.EddyLogoImageSizeSmall;
            _eddyLogoImageSizeMedium = configurationService.EddyLogoImageSizeMedium;
            _eddyLogoImageSizeLarge = configurationService.EddyLogoImageSizeLarge;
            _eddyLogoImagePathDomain = configurationService.EddyLogoImagePathDomain;
            _eddyLogoImageFileName = configurationService.EddyLogoImageFileName;
        }

        public string GetSmallLogoFormattedUrl(string unformattedPath)
        {
            return GetLogoFormattedUrl(unformattedPath, _eddyLogoImageSizeSmall);
        }

        public string GetMediumLogoFormattedUrl(string unformattedPath)
        {
            return GetLogoFormattedUrl(unformattedPath, _eddyLogoImageSizeMedium);
        }

        public string GetLargeLogoFormattedUrl(string unformattedPath)
        {
            return GetLogoFormattedUrl(unformattedPath, _eddyLogoImageSizeLarge);
        }

        private string GetLogoFormattedUrl(string unformattedPath, string fileSize)
        {
            var formattedPath = ReplaceFilenamePlaceholderWithFileNameAndFileSize(unformattedPath, fileSize);
            bool formattedPathOrDomainIsNullOrBlank = string.IsNullOrWhiteSpace(formattedPath) || string.IsNullOrWhiteSpace(_eddyLogoImagePathDomain);
            return formattedPathOrDomainIsNullOrBlank ? string.Empty : $"{_eddyLogoImagePathDomain}{formattedPath}";
        }

        private string ReplaceFilenamePlaceholderWithFileNameAndFileSize(string unformattedPath, string fileSize)
        {
            string formattedPath = string.Empty;

            bool unformattedPathIsntNullOrBlank = !string.IsNullOrWhiteSpace(unformattedPath);
            bool fileSizeIsntNullOrBlank = !string.IsNullOrWhiteSpace(fileSize);
            bool fileNameIsntNullOrBlank = !string.IsNullOrWhiteSpace(_eddyLogoImageFileName);
            bool unformattedPathContainsPlaceholder = unformattedPath?.Contains("{FILENAME}") ?? false;

            if (unformattedPathIsntNullOrBlank && fileSizeIsntNullOrBlank && fileNameIsntNullOrBlank && unformattedPathContainsPlaceholder)
            {
                formattedPath = unformattedPath.Replace("{FILENAME}", string.Format(_eddyLogoImageFileName, fileSize));
            }

            return formattedPath;
        }

    }
}
