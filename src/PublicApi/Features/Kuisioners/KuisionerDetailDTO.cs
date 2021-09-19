using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCoreApi.PublicApi.Features.Soals;


namespace AppCoreApi.PublicApi.Features.Kuisioners
{
	public class KuisionerDetailDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string KuisionerId { get; set; }
		public string KuisionerJudul { get; set; }
		public KuisionerDTO Kuisioner { get; set; }
		public string SoalId { get; set; }
		public string SoalKonten { get; set; }
		public SoalDTO Soal { get; set; }
		public string KontenSoal { get; set; }
		public string Pilihan1 { get; set; }
		public string PIlihan2 { get; set; }
		public string Pilihan3 { get; set; }
		public int? KunciJawaban { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
