using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
	public interface IUriComposer
	{
		string ComposeAppUri(string url);
		string ComposeBaseUri(string url);
		string ComposeDownloadUri(string url);
		string ComposeUploadUri(string url);
		string ComposeBaseStoragePath(string path);
		string ComposeDownloadPath(string path);
		string ComposeLogsPath(string path);
		string ComposeTempPath(string path);
		string ComposeTemplatePath(string path);
		string ComposeUploadPath(string path);
		EmailConfiguration GetEmailConfiguration();
	}
}
