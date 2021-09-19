using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class LookupDetail : BaseEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public bool IsEditable { get; set; }
		public int LookupId { get; set; }
		public virtual Lookup Lookup { get; set; }

		public LookupDetail() { }

		public LookupDetail(string name, string value, bool isEditable, Lookup lookup) 
		{
			Name = name;
			Value = value;
			IsEditable = IsEditable;
			Lookup = lookup;
		}
	}
}
