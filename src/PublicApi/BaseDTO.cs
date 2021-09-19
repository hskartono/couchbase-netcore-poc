using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi
{
	public class BaseDTO
	{
		public virtual string UploadValidationStatus { get; set; }
		public virtual string UploadValidationMessage { get; set; }
		public virtual string UploadValidationMessageHtml
		{
			get
			{
				if (string.IsNullOrEmpty(UploadValidationMessage))
					UploadValidationMessage = "";
				return UploadValidationMessage.Replace(Environment.NewLine, "<br/>");
			}
		}
		public virtual bool IsFromUpload { get; set; }
		public int? MainRecordId { get; set; }
	}
}
