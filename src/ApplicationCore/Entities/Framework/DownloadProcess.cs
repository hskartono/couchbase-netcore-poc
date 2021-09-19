using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class DownloadProcess : BaseEntity
	{
		public DownloadProcess(string functionId)
		{
			JobId = Guid.NewGuid().ToString();
			FunctionId = functionId;
			StartTime = DateTime.Now;
			Status = "IN PROGRESS";
			ErrorMessage = "";
		}

		public int Id { get; set; }
		public string JobId { get; set; }
		public string FunctionId { get; set; }
		public virtual FunctionInfo FunctionInfo { get; set; }
		public string FileName { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Status { get; set; }	// IN PROGRESS, SUCCESS, FAILED
		public string ErrorMessage { get; set; }

	}
}
