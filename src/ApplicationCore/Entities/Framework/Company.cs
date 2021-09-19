using System;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class Company : BaseEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }

		//public int CreatedBy { get; set; }
		//public DateTime CreatedDate { get; set; }
		//public int? UpdatedBy { get; set; }
		//public DateTime? UpdatedDate { get; set; }
	}
}
