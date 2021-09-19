using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class JobConfiguration : BaseEntity
	{
		#region appgen: generated constructor
		public JobConfiguration() { }

		public JobConfiguration(string interfaceName, string jobName, string jobDescription, bool? isStoredProcedure)
		{
			InterfaceName = interfaceName;
			JobName = jobName;
			JobDescription = jobDescription;
			IsStoredProcedure = isStoredProcedure;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public string InterfaceName { get; set; }
		public string JobName { get; set; }
		public string JobDescription { get; set; }
		public bool? IsStoredProcedure { get; set; }

		public new int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
