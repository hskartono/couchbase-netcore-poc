using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppCoreApi.PublicApi.Features.JobConfigurations
{
	public class JobConfigurationDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string InterfaceName { get; set; }
		public string JobName { get; set; }
		public string JobDescription { get; set; }
		public bool? IsStoredProcedure { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
