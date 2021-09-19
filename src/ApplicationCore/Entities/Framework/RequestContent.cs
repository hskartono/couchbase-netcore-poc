using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class RequestContent
	{
		public string Document { get; set; }
		public string FileName { get; set; }
		public string PrinterName { get; set; }
		public string UserName { get; set; }
	}
}
