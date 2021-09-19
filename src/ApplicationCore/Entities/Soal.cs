using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class Soal : BaseEntity
	{
		#region appgen: generated constructor
		public Soal() { }

		public Soal(string konten)
		{
			Konten = konten;
		}


		#endregion

		#region appgen: generated property
		public string Id { get; set; }
		public string Konten { get; set; }

		public string MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
