using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class SchedulerCronInterval : BaseEntity
	{
		#region appgen: generated constructor
		public SchedulerCronInterval() { }

		public SchedulerCronInterval(string name)
		{
			Name = name;
		}


		#endregion

		#region appgen: generated property
		public string Id { get; set; }
		public string Name { get; set; }

		public new string MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
