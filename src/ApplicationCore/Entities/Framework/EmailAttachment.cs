namespace AppCoreApi.ApplicationCore.Entities
{
	public class EmailAttachment : BaseEntity
	{
		public int Id { get; set; }
		public int EmailId { get; set; }
		public int AttachmentId { get; set; }
		public virtual Email Email { get; set; }
		public virtual Attachment Attachment { get; set; }

		public EmailAttachment()
		{

		}

		public EmailAttachment(Email email)
		{
			Email = email;
			EmailId = email.Id;
		}

		public EmailAttachment(Email email, Attachment attachment)
		{
			Email = email;
			EmailId = email.Id;
			Attachment = attachment;
			AttachmentId = attachment.Id;
		}
	}
}
