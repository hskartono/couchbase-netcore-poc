namespace AppCoreApi.ApplicationCore.Entities
{
	public class Attachment : BaseEntity
	{
		public int Id { get; set; }
		public string OriginalFileName { get; set; }
		public string SavedFileName { get; set; }		
		public string FileExtension { get; set; }
		public long FileSize { get; set; }

		private string downloadUrl = "";
		public string DownloadURL
		{
			get
			{
				return downloadUrl;
			}
		}

		public void ComposeDownloadUrl(string composedUri)
		{
			downloadUrl = composedUri;
		}

		public Attachment()
		{

		}

		public Attachment(string originalFilename, string savedFilename, string fileExtension, long fileSize)
		{
			OriginalFileName = originalFilename;
			SavedFileName = savedFilename;
			FileExtension = fileExtension;
			FileSize = fileSize;
		}
	}
}
