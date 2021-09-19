using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class KuisionerDetail : BaseEntity
	{
		#region appgen: generated constructor
		public KuisionerDetail() { }

		public KuisionerDetail(string soalId, string kontenSoal, string pilihan1, string pIlihan2, string pilihan3, int? kunciJawaban, Kuisioner parent)
		{
			SoalId = soalId;
			KontenSoal = kontenSoal;
			Pilihan1 = pilihan1;
			PIlihan2 = pIlihan2;
			Pilihan3 = pilihan3;
			KunciJawaban = kunciJawaban;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public int? KuisionerId { get; set; }
		public virtual Kuisioner Kuisioner { get; set; }
		public string SoalId { get; set; }
		public virtual Soal Soal { get; set; }
		public string KontenSoal { get; set; }
		public string Pilihan1 { get; set; }
		public string PIlihan2 { get; set; }
		public string Pilihan3 { get; set; }
		public int? KunciJawaban { get; set; }

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
