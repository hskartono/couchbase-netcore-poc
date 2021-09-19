using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCoreApi.PublicApi.Features.Soals;


namespace AppCoreApi.PublicApi.Features.Kuisioners
{
	public class KuisionerDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string Judul { get; set; }
		public DateTime? AktifDari { get; set; }
		public DateTime? AktifSampai { get; set; }
		public List<KuisionerDetailDTO> KuisionerDetail { get; set; } = new List<KuisionerDetailDTO>();

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
