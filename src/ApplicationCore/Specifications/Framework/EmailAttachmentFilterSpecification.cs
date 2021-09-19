using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class EmailAttachmentFilterSpecification : Specification<EmailAttachment>
	{
		public EmailAttachmentFilterSpecification(int? emailId, bool? withHeader = false)
		{
			InitializeSpecification(emailId: emailId, withHeader: (withHeader.HasValue && withHeader.Value));
		}

		public EmailAttachmentFilterSpecification(int skip, int take, bool? withHeader = false)
		{
			InitializeSpecification(skip, take, withHeader: (withHeader.HasValue && withHeader.Value));
		}

		public EmailAttachmentFilterSpecification(int? emailId, int? attachmentId, bool? withHeader = false)
		{
			InitializeSpecification(emailId: emailId, attachmentId: attachmentId, withHeader: (withHeader.HasValue && withHeader.Value));
		}

		public EmailAttachmentFilterSpecification(int skip, int take, int? emailId, int? attachmentId, bool? withHeader = false)
		{
			InitializeSpecification(skip, take, emailId, attachmentId, (withHeader.HasValue && withHeader.Value));
		}

		private void InitializeSpecification(int? skip = null, int? take = null, int? emailId = null, int? attachmentId = null, bool withHeader = false)
		{
			Query
				.Where(
					e => (!emailId.HasValue || e.EmailId == emailId) &&
					(!attachmentId.HasValue || e.AttachmentId == attachmentId)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withHeader)
				Query
					.Include(e => e.Email);
		}
	}
}
