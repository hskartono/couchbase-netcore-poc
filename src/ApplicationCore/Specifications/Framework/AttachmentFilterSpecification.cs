using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System.Collections.Generic;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class AttachmentFilterSpecification : Specification<Attachment>
	{
		public AttachmentFilterSpecification(int id)
		{
			InitializeSpecification(id: id);
		}

		public AttachmentFilterSpecification(List<int> ids, bool withChild = false)
		{
			Query
				.Where(e => ids.Contains(e.Id));
		}

		public AttachmentFilterSpecification(int skip, int take)
		{
			InitializeSpecification(skip, take);
		}

		public AttachmentFilterSpecification(int skip, int take, string fileName, string fileExtension)
		{
			InitializeSpecification(skip, take, fileName, fileExtension);
		}

		public AttachmentFilterSpecification(int skip, int take, List<string> fileNames, List<string> fileExtensions)
		{
			InitializeSpecification(skip, take, fileNames: fileNames, fileExtensions: fileExtensions);
		}

		public AttachmentFilterSpecification(string fileName, string fileExtension)
		{
			InitializeSpecification(fileName: fileName, fileExtension: fileExtension);
		}

		public AttachmentFilterSpecification(List<string> fileNames, List<string> fileExtensions)
		{
			InitializeSpecification(fileNames: fileNames, fileExtensions: fileExtensions);
		}

		private void InitializeSpecification(int? skip=null, int? take=null, 
			string fileName = "", string fileExtension="", 
			List<string> fileNames = null, List<string> fileExtensions = null,
			int? id = null, bool? isExact = false)
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(fileName) || e.OriginalFileName.Contains(fileName)) &&
					(string.IsNullOrEmpty(fileExtension) || e.FileExtension == fileExtension) &&
					(!id.HasValue || e.Id == id)
				);

			if (isExact.HasValue && isExact.Value == true)
			{
				Query.Where(
					e => (fileNames == null || fileNames.Contains(e.OriginalFileName)) &&
					(fileExtensions == null || fileExtension.Contains(e.FileExtension))
				);
			} else
			{
				if (fileNames?.Count > 0)
					foreach (var item in fileNames)
						Query.Where(e => e.OriginalFileName.Contains(item));
			}

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
