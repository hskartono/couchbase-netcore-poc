using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.DownloadProcesses
{
	public class DownloadProcessDTO
	{
		public int Id { get; set; }
		public string JobId { get; set; }
		public string FunctionId { get; set; }
		public string FunctionName { get; set; }
		public string FileName { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Status { get; set; }  // IN PROGRESS, SUCCESS, FAILED
		public string ErrorMessage { get; set; }
	}
}
