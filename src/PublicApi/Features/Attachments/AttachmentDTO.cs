using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Attachments
{
	public class AttachmentDTO
	{
		public int Id { get; set; }
		public string OriginalFilename { get; set; }
		public string SavedFileName { get; set; }
		public string FileExtension { get; set; }
		public long FileSize { get; set; }
		public string DownloadUrl { get; set; }

        public AttachmentDTO(string originalFilename)
        {
            OriginalFilename = originalFilename;
        }

    }
}
