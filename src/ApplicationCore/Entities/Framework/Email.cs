using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class Email : BaseEntity
	{
		public enum EmailStatus
		{
			READY = 0,
			SENT = 1,
			ERROR = 2
		};

		private readonly List<EmailAttachment> emailAttachments = new List<EmailAttachment>();

		public IReadOnlyList<EmailAttachment> EmailAttachments => emailAttachments;

		public int Id { get; set; }
		public string Subject { get; set; }
		public string Sender { get; set; }
		public string Receiver { get; set; }
		public string ReceiverCC { get; set; }
		public string MailContent { get; set; }
		public DateTime SendDate { get; set; }
		public EmailStatus Status { get; set; }
		public string ErrorMessage { get; set; }
		public int RetryCount { get; set; }

		public Email()
		{
			Status = EmailStatus.READY;
			ErrorMessage = "";
			RetryCount = 0;
		}

		public Email(string receiver, string subject, string content)
		{
			Status = EmailStatus.READY;
			ErrorMessage = "";
			RetryCount = 0;
			Receiver = receiver;
			Subject = subject;
			MailContent = content;
		}

		public void AddAttachment(Attachment attachment)
		{
			emailAttachments.Add(new EmailAttachment(this, attachment));
		}

		public void AddAttachment(EmailAttachment emailAttachment)
		{
			emailAttachment.Email = this;
			emailAttachments.Add(emailAttachment);
		}

		public void RemoveAttachment(EmailAttachment emailAttachment)
		{
			if (!emailAttachments.Contains(emailAttachment))
				return;

			emailAttachments.Remove(emailAttachment);
		}

		public void ReplaceAttachment(EmailAttachment emailAttachment)
		{
			var selectedItem = emailAttachments.SingleOrDefault(p => p.Id == emailAttachment.Id);
			if (selectedItem == null) return;
			selectedItem.Attachment = emailAttachment.Attachment;
			selectedItem.AttachmentId = emailAttachment.AttachmentId;
			selectedItem.CompanyId = emailAttachment.CompanyId;
			selectedItem.CreatedBy = emailAttachment.CreatedBy;
			selectedItem.CreatedDate = emailAttachment.CreatedDate;
			emailAttachment.UpdatedBy = emailAttachment.UpdatedBy;
			emailAttachment.UpdatedDate = emailAttachment.UpdatedDate;
		}

		public void ReplaceAttachment(Attachment attachment)
		{
			var selectedItem = emailAttachments.Single(p => p.AttachmentId == attachment.Id);
			if (selectedItem == null) return;
			BindAttachment(selectedItem, attachment);
		}

		public void ReplaceOrAddAttachment(Attachment attachment)
		{
			var selectedItem = emailAttachments.Single(p => p.AttachmentId == attachment.Id);
			if (selectedItem == null)
			{
				AddAttachment(attachment);
				return;
			}

			BindAttachment(selectedItem, attachment);
		}

		private void BindAttachment(EmailAttachment emailAttachment, Attachment attachment)
		{
			emailAttachment.Email = this;
			emailAttachment.EmailId = this.Id;
			emailAttachment.Attachment = attachment;
			emailAttachment.AttachmentId = attachment.Id;
		}

		public void ClearAttachment()
		{
			emailAttachments.Clear();
		}
	}
}
