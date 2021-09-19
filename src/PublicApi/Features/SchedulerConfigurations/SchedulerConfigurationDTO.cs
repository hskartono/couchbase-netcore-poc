using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCoreApi.PublicApi.Features.SchedulerCronIntervals;
using AppCoreApi.PublicApi.Features.JobConfigurations;


namespace AppCoreApi.PublicApi.Features.SchedulerConfigurations
{
	public class SchedulerConfigurationDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string IntervalTypeId { get; set; }
		public string IntervalTypeName { get; set; }
		public SchedulerCronIntervalDTO IntervalType { get; set; }
		public int? IntervalValue { get; set; }
		public int? IntervalValue2 { get; set; }
		public int? IntervalValue3 { get; set; }
		public string CronExpression { get; set; }
		public int? JobTypeId { get; set; }
		public string JobTypeJobName { get; set; }
		public JobConfigurationDTO JobType { get; set; }
		public string RecurringJobId { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
