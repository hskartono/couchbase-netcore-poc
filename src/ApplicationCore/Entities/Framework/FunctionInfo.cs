using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class FunctionInfo : BaseEntity
	{

	public FunctionInfo() { }

	public FunctionInfo(string name, string uri,string iconName, bool? isEnable, int? moduleInfoId, string moduleName)
	{
			Name = name;
			Uri = uri;
			IconName = iconName;
			IsEnabled = isEnable;
			ModuleInfoId = moduleInfoId;
			ModuleName = moduleName;
	}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Uri { get; set; }
		public string IconName { get; set; }
		public bool? IsEnabled { get; set; }
		public int? ModuleInfoId { get; set; }
		public virtual ModuleInfo ModuleInfo { get; set; }

		[NotMapped]
		public string ModuleName { get; set; }
		
	
	}
}
