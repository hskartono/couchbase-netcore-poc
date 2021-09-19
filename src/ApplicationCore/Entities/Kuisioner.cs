using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class Kuisioner : BaseEntity
	{
		#region appgen: generated constructor
		public Kuisioner() { }

		public Kuisioner(string judul, DateTime? aktifDari, DateTime? aktifSampai)
		{
			Judul = judul;
			AktifDari = aktifDari;
			AktifSampai = aktifSampai;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public string Judul { get; set; }
		public DateTime? AktifDari { get; set; }
		public DateTime? AktifSampai { get; set; }
		private IList<KuisionerDetail> _kuisionerDetail = new List<KuisionerDetail>();
		public IList<KuisionerDetail> KuisionerDetail { get => _kuisionerDetail; set => _kuisionerDetail = value; }

		public void AddOrReplaceKuisionerDetail(KuisionerDetail entity)
		{
			KuisionerDetail selectedItem = null;
			int index = 0;
			foreach(var item in _kuisionerDetail)
			{
				if(item.Id == entity.Id)
				{
					selectedItem = item;
					break;
				}
				index++;
			}

			if(selectedItem == null)
			{
				entity.Kuisioner = this;
				entity.KuisionerId = this.Id;
				_kuisionerDetail.Add(entity);
			} else
			{
				entity.Id = selectedItem.Id;
				entity.Kuisioner = this;
				entity.KuisionerId = this.Id;

				entity.CompanyId = selectedItem.CompanyId;
				entity.CreatedBy = selectedItem.CreatedBy;
				entity.CreatedDate = selectedItem.CreatedDate;
				entity.UpdatedBy = selectedItem.UpdatedBy;
				entity.UpdatedDate = selectedItem.UpdatedDate;

				selectedItem = entity;
				_kuisionerDetail[index] = selectedItem;
			}
		}

		public void AddKuisionerDetail(string soalId, string kontenSoal, string pilihan1, string pIlihan2, string pilihan3, int? kunciJawaban)
		{
			var newItem = new KuisionerDetail(soalId, kontenSoal, pilihan1, pIlihan2, pilihan3, kunciJawaban, this);
			_kuisionerDetail.Add(newItem);
		}

		public void RemoveKuisionerDetail(KuisionerDetail entity)
		{
			var selectedItem = _kuisionerDetail.FirstOrDefault(e => e.Id == entity.Id);
			_kuisionerDetail.Remove(selectedItem);
		}

		public void ClearKuisionerDetails()
		{
			_kuisionerDetail.Clear();
		}

		public void AddRangeKuisionerDetails(IList<KuisionerDetail> kuisionerDetails)
		{
			//this.ClearKuisionerDetail();
			((List<KuisionerDetail>)_kuisionerDetail).AddRange(kuisionerDetails);
		}

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
