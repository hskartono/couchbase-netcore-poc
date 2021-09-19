using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class EmailFilterSpecification : Specification<Email>
	{
		public EmailFilterSpecification(int skip, int take, Email.EmailStatus? emailStatus, string createdById, bool? withChild = false)
		{
			InitiateFilterData(skip, take, emailStatus, createdById, withChild: (withChild.HasValue && withChild.Value));
		}

		public EmailFilterSpecification(Email.EmailStatus? emailStatus, string createdById, bool? withChild = false)
		{
			InitiateFilterData(emailStatus: emailStatus, createdById: createdById, withChild: (withChild.HasValue && withChild.Value));
		}

		public EmailFilterSpecification(int id, bool? withChild = false)
		{
			InitiateFilterData(id: id, withChild: (withChild.HasValue && withChild.Value));
		}

		protected void InitiateFilterData(int? skip = null, int? take = null, 
			Email.EmailStatus? emailStatus = null, string createdById = null, int? id = null, bool withChild = false)
		{
			Query
				.Where(
					e => (!emailStatus.HasValue || e.Status == emailStatus) &&
					(string.IsNullOrEmpty(createdById) || e.CreatedBy == createdById) &&
					(!id.HasValue || e.Id == id)
				);

			if(skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withChild == true)
				Query
					.Include(e => e.EmailAttachments)
					.ThenInclude(d => d.Attachment);
		}
	}
}
