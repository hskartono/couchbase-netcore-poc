using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure
{
    public class UriComposer : IUriComposer
    {
        private readonly LookupSettings _lookupSettings;
        private readonly PathConfig _pathConfig;
        private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _baseStoragePath;
        private readonly EmailConfiguration _emailConfig;

        public UriComposer(LookupSettings lookupSettings)
        {
            _lookupSettings = lookupSettings;
            _pathConfig = _lookupSettings.PathConfig;
            _baseStoragePath = _pathConfig.BaseStoragePath.Replace("~", _basePath);

            _emailConfig = _lookupSettings.EmailConfiguration;
        }

        // application URL
        public string ComposeAppUri(string url)
        {
            var uri = new Uri(_pathConfig.AppURI);
            var appUri = new Uri(uri, url);
            return appUri.AbsoluteUri;
        }

        // backend API URL
        public string ComposeBaseUri(string url)
        {
            var uri = new Uri(_pathConfig.BaseURI);
            var baseUri = new Uri(uri, url);
            return baseUri.AbsoluteUri;
        }

        public string ComposeDownloadUri(string url)
        {
            var completeUrl = ComposeBaseUri(_pathConfig.DownloadURI);
            var completeUri = new Uri(completeUrl);
            var downloadUri = new Uri(completeUri, url);
            return downloadUri.AbsoluteUri;
        }

        public string ComposeUploadUri(string url)
        {
            var completeUrl = ComposeBaseUri(_pathConfig.UploadURI);
            var completeUri = new Uri(completeUrl);
            var downloadUri = new Uri(completeUri, url);
            return downloadUri.AbsoluteUri;
        }

        public string ComposeBaseStoragePath(string path)
        {
            return Path.Combine(_baseStoragePath, path);
        }

        public string ComposeDownloadPath(string path)
        {
            return Path.Combine(_pathConfig.DownloadPath.Replace("~", _baseStoragePath), path);
        }

        public string ComposeUploadPath(string path)
        {
            return Path.Combine(_pathConfig.UploadPath.Replace("~", _baseStoragePath), path);
        }

        public string ComposeLogsPath(string path)
        {
            return Path.Combine(_pathConfig.LogsPath.Replace("~", _baseStoragePath), path);
        }

        public string ComposeTemplatePath(string path)
        {
            return Path.Combine(_pathConfig.TemplatePath.Replace("~", _baseStoragePath), path);
        }

        public string ComposeTempPath(string path)
        {
            return Path.Combine(_pathConfig.TempPath.Replace("~", _baseStoragePath), path);
        }

        public EmailConfiguration GetEmailConfiguration()
        {
            return _emailConfig;
        }
    }
}
