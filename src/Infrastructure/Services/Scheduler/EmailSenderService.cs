using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class EmailSenderService : IBackgroundService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUriComposer _uriComposer;
		private IEmailService _emailService;

		public EmailSenderService(IUnitOfWork unitOfWork, IUriComposer uriComposer)
		{
			_unitOfWork = unitOfWork;
			_uriComposer = uriComposer;
			_emailService = new EmailService(_unitOfWork, null, _uriComposer);
		}

		public async Task ExecuteService()
		{
			if (_emailService == null)
				_emailService = new EmailService(_unitOfWork, null, _uriComposer);

			await _emailService.ProcessEmailBatch();
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ExecuteService(params object[] list)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
			throw new NotImplementedException();
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ExecuteStoredProcedure(string spName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
			throw new NotImplementedException();
		}
	}
}
