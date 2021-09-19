using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class SchedulerConfiguration : BaseEntity
	{
		#region appgen: generated constructor
		public SchedulerConfiguration() { }

		public SchedulerConfiguration(string intervalTypeId, int? intervalValue, int? intervalValue2, int? intervalValue3, string cronExpression, int? jobTypeId, string recurringJobId)
		{
			IntervalTypeId = intervalTypeId;
			IntervalValue = intervalValue;
			IntervalValue2 = intervalValue2;
			IntervalValue3 = intervalValue3;
			CronExpression = cronExpression;
			JobTypeId = jobTypeId;
			RecurringJobId = recurringJobId;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public string IntervalTypeId { get; set; }
		public virtual SchedulerCronInterval IntervalType { get; set; }
		public int? IntervalValue { get; set; }
		public int? IntervalValue2 { get; set; }
		public int? IntervalValue3 { get; set; }
		public string CronExpression { get; set; }
		public int? JobTypeId { get; set; }
		public virtual JobConfiguration JobType { get; set; }
		public string RecurringJobId { get; set; }

		public new int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
