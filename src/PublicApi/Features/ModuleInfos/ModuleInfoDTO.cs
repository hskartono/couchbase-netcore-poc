using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppCoreApi.PublicApi.Features.ModuleInfos
{
	public class ModuleInfoDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string Name { get; set; }
		public string IconName { get; set; }
		public int? ParentModuleId { get; set; }

		#endregion

		//TAMBAHAN REZA
		public int? OrderPosition { get; set; }

		#region appgen: property collection list

		#endregion
	}
}
