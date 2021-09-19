using Ardalis.Specification;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class EmailService : AsyncBaseService<Email>, IEmailService
	{
		private readonly IUriComposer _uriComposer;
		public EmailService(IUnitOfWork unitOfWork, ILogger<EmailService> logger, IUriComposer uriComposer) : base(unitOfWork) 
		{
			_uriComposer = uriComposer;
		}

		public async Task<Email> AddAsync(Email entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			await _unitOfWork.Emails.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Emails.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(Email entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.Emails.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			var itemToDelete = await GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
			{
				AddError($"Item with id {id} is not found.");
				return false;
			}

			return await DeleteAsync(itemToDelete);
		}

		public async Task<Email> FirstAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Emails.FirstAsync(spec, cancellationToken);
		}

		public async Task<Email> FirstOrDefaultAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Emails.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<Email> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			var specFilter = new EmailFilterSpecification(id, true);
			var results = await _unitOfWork.Emails.ListAsync(specFilter, null, cancellationToken);
			if (results?.Count > 0) return results[0];
			return null;
		}

		public async Task<IReadOnlyList<Email>> ListAllAsync(List<SortingInformation<Email>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Emails.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<Email>> ListAsync(ISpecification<Email> spec, List<SortingInformation<Email>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Emails.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task ProcessEmailBatch()
		{
			EmailConfiguration emailConfig = _uriComposer.GetEmailConfiguration();
			if (emailConfig == null) return;
			if (emailConfig.BatchLimit <= 0) emailConfig.BatchLimit = 10;

			// ambil email yang belum di proses sesuai threshold
			var filter = new EmailFilterSpecification(0, emailConfig.BatchLimit, Email.EmailStatus.READY, null, true);
			var emails = await _unitOfWork.Emails.ListAsync(filter);

			// loop dan kirim email
			if(emails != null && emails.Count > 0)
			{
				// load configuration
				string stmpAddress = emailConfig.SmtpAddress;
				int port = emailConfig.PortNumber;
				bool useSSL = emailConfig.UseSSL;
				string userName = emailConfig.UserName;
				string password = emailConfig.Password;
				string senderName = emailConfig.SenderName;
				string senderAddress = emailConfig.SenderAddress;

				// prepare smtp client
				SmtpClient client = new SmtpClient();
				client.Connect(stmpAddress, port, useSSL);
                if (!string.IsNullOrEmpty(userName))
                {
					client.Authenticate(userName, password);
				}

				foreach(var item in emails)
				{
					try
					{
						MimeKit.MimeMessage message = new MimeKit.MimeMessage();
						MimeKit.MailboxAddress from = new MimeKit.MailboxAddress(senderName, senderAddress);
						message.From.Add(from);

                        if (string.IsNullOrEmpty(item.Receiver))
                        {
							item.Status = Email.EmailStatus.ERROR;
							item.ErrorMessage = "Receiver cannot be empty";
							item.UpdatedDate = DateTime.Now;
							item.RecordActionDate = DateTime.Now;
							await _unitOfWork.Emails.UpdateAsync(item);
							continue;
						}

						var multipleReceivers = item.Receiver.Split(";");
						foreach(var receiver in multipleReceivers)
                        {
							MimeKit.MailboxAddress to = new MimeKit.MailboxAddress(receiver.Trim(), receiver.Trim());
							message.To.Add(to);
						}

						if (!string.IsNullOrEmpty(item.ReceiverCC))
						{
							var multipleCCReceivers = item.Receiver.Split(";");
							foreach (var receiver in multipleCCReceivers)
                            {
								MimeKit.MailboxAddress cc = new MimeKit.MailboxAddress(receiver, receiver);
								message.Cc.Add(cc);
							}
						}

						message.Subject = item.Subject;

						MimeKit.BodyBuilder bodyBuilder = new MimeKit.BodyBuilder();
						bodyBuilder.HtmlBody = item.MailContent;

						if (item.EmailAttachments?.Count > 0)
						{
							foreach(var att in item.EmailAttachments)
							{
								string filename = att.Attachment.SavedFileName;
								string attachmentFile = _uriComposer.ComposeDownloadPath(filename);
								if (!System.IO.File.Exists(attachmentFile))
									attachmentFile = _uriComposer.ComposeUploadPath(filename);
								bodyBuilder.Attachments.Add(attachmentFile);
							}
						}

						message.Body = bodyBuilder.ToMessageBody();

						await client.SendAsync(message);

						item.SendDate = DateTime.Now;
						item.Status = Email.EmailStatus.SENT;
						item.UpdatedDate = DateTime.Now;
						item.RecordActionDate = DateTime.Now;
						await _unitOfWork.Emails.UpdateAsync(item);

					} catch(Exception ex)
					{
						item.Status = Email.EmailStatus.ERROR;
						item.ErrorMessage = ex.Message;
						item.UpdatedDate = DateTime.Now;
						item.RecordActionDate = DateTime.Now;
						await _unitOfWork.Emails.UpdateAsync(item);
					}
				}

				await _unitOfWork.CommitAsync();

				// disconnect & close
				client.Disconnect(true);
				client.Dispose();
			}
		}

		public async Task<Email> SendEmail(List<string> receiver, string subject, string templateFileName, Dictionary<string, string> contentMap, List<Attachment> attachments, CancellationToken cancellationToken = default)
		{
			return await SendEmail(receiver, null, subject, templateFileName, contentMap, attachments);
		}

		public async Task<Email> SendEmail(List<string> receiver, List<string> carbonCopy, string subject, string templateFileName, Dictionary<string, string> contentMap, List<Attachment> attachments, CancellationToken cancellationToken = default)
		{
			if (receiver == null) throw new ArgumentNullException(nameof(receiver));
			if (contentMap == null) throw new ArgumentException(nameof(contentMap));
			if (contentMap.Count <= 0) throw new Exception("Informasi mapping email tidak boleh kosong");

			string emailReceiver = string.Join(";", receiver);
			string emailCC = string.Empty;
			if (carbonCopy?.Count > 0)
				emailCC = string.Join(",", carbonCopy);

			string fileTemplate = _uriComposer.ComposeTemplatePath("email/" + templateFileName);
			if (!System.IO.File.Exists(fileTemplate)) 
				throw new Exception($"File email template tidak ditemukan. File template: {fileTemplate}");

			string template = System.IO.File.ReadAllText(fileTemplate);
			foreach(var key in contentMap.Keys)
			{
				var value = contentMap[key];
				template = template.Replace($"{{{key}}}", value);
			}

			var newEmail = new Email(emailReceiver, subject, template)
			{
				ReceiverCC = emailCC
			};

			if (attachments?.Count > 0)
			{
				foreach (var item in attachments)
					newEmail.AddAttachment(item);
			}

			AssignCreatorAndCompany(newEmail);

			var result = await _unitOfWork.Emails.AddAsync(newEmail, cancellationToken);
			if(result == null)
			{
				AddError("Gagal menyimpan email ke dalam antrian");
				return null;
			}

			// di comment karena commit hanya dilakukan oleh service utama
			//await _unitOfWork.CommitAsync();
			return result;
		}

		public async Task<bool> UpdateAsync(Email entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			// update header
			AssignUpdater(entity);
			await _unitOfWork.Emails.ReplaceAsync(entity, entity.Id, cancellationToken);

			// ambil old list of email attachment
			var oldEntity = await _unitOfWork.Emails.GetByIdAsync(entity.Id, cancellationToken);
			List<EmailAttachment> oldEntityTobeDeleted = new List<EmailAttachment>();
			if (oldEntity.EmailAttachments.Count > 0)
			{
				foreach (var item in oldEntity.EmailAttachments)
					oldEntityTobeDeleted.Add(item);
			}

			// smart update
			if (entity.EmailAttachments.Count > 0)
			{
				foreach (var item in entity.EmailAttachments)
				{
					var hasUpdate = false;
					if (oldEntity.EmailAttachments.Count > 0)
					{
						var data = oldEntity.EmailAttachments.SingleOrDefault(o => o.Id == item.Id);
						if (data != null)
						{
							// cek dulu apakah attachment sudah tersimpan atau belum
							if (item.AttachmentId == 0)
							{
								await _unitOfWork.AttachmentRepository.AddAsync(data.Attachment);
							}

							// data.AttachmentId = item.AttachmentId;
							AssignUpdater(item);
							await _unitOfWork.EmailAttachments.ReplaceAsync(item, item.Id, cancellationToken);

							// remove dari daftar yang akan di delete
							oldEntityTobeDeleted.Remove(data);

							hasUpdate = true;
						}
					}

					if (!hasUpdate)
					{
						if (item.AttachmentId == 0)
							await _unitOfWork.AttachmentRepository.AddAsync(item.Attachment);

						// add sebagai child baru
						oldEntity.AddAttachment(item.Attachment);
					}
				}
			}

			// delete data yang tidak di create / update
			if (oldEntityTobeDeleted.Count > 0)
			{
				foreach (var item in oldEntityTobeDeleted)
				{
					oldEntity.RemoveAttachment(item);
				}
			}

			// update & commit changes
			await _unitOfWork.Emails.UpdateAsync(oldEntity);
			await _unitOfWork.CommitAsync();

			return true;
		}

		private bool ValidateBase(Email email)
		{
			if (string.IsNullOrEmpty(email.Subject))
				AddError("Subject cannot be empty");

			if (string.IsNullOrEmpty(email.Receiver))
				AddError("Receiver cannot be empty");

			if (string.IsNullOrEmpty(email.Sender))
				AddError("Sender cannot be empty");

			return ServiceState;
		}

		private bool ValidateOnInsert(Email email)
		{
			ValidateBase(email);

			return ServiceState;
		}

		private bool ValidateOnUpdate(Email email)
		{
			ValidateBase(email);

			return ServiceState;
		}
	}
}
