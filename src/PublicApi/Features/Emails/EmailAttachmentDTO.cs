using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Emails
{
	public class EmailAttachmentDTO
	{
		public int Id { get; set; }
		public int AttachmentId { get; set; }

		public string OriginalFileName { get; set; }
		public string SavedFileName { get; set; }
		public string FileExtension { get; set; }
		public long FileSize { get; set; }

	}
}
