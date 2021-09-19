using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class PrintRequest
	{
		public PrintRequest()
		{

		}

		public PrintRequest(string base64Content, string fileName, string printerName, string userName)
		{
			request.Document = base64Content;
			request.FileName = fileName;
			request.PrinterName = printerName;
			request.UserName = userName;
		}

		public RequestContent request { get; set; } = new RequestContent();
	}
}
