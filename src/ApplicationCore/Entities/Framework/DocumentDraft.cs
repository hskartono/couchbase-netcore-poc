using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class DocumentDraft
	{
		public string Message { get; set; }
		public string DocumentId { get; set; }

		public DocumentDraft(string message, int id)
		{
			Message = message;
			DocumentId = id.ToString();
		}

		public DocumentDraft(string message, string id)
		{
			Message = message;
			DocumentId = id;
		}
	}
}
