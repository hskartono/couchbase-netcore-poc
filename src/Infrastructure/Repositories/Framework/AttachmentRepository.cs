using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class AttachmentRepository : AsyncRepository<Attachment>, IAttachmentRepository
	{
		public AttachmentRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
