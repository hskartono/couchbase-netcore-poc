using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi
{
	public class PaginatedItemsViewModel<TEntity> where TEntity : class
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public int PageCount { get; set; }
		public long DataCount { get; set; }
		public IEnumerable<TEntity> Data { get; set; }
		public PaginatedItemsViewModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			DataCount = count;
			Data = data;
			PageCount = (pageSize > 0) ? (int.Parse(Math.Ceiling((decimal)count / pageSize).ToString())) : 0;
		}
	}
}
