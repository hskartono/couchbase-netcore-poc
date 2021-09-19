using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Emails
{
	public class EmailDTO
	{
		public int Id { get; set; }
		public string Subject { get; set; }
		public string Sender { get; set; }
		public string Receiver { get; set; }
		public string ReceiverCC { get; set; }
		public string MailContent { get; set; }
		public DateTime SendDate { get; set; }
		public int Status { get; set; }
		public string ErrorMessage { get; set; }
		public int RetryCount { get; set; }
		public List<EmailAttachmentDTO> EmailAttachments { get; set; }

	}
}
