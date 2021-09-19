using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class Lookup : BaseEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		private readonly List<LookupDetail> _lookupDetails = new List<LookupDetail>();
		public IReadOnlyList<LookupDetail> LookupDetails => _lookupDetails;

		public void AddOrReplaceLookupDetail(string name, string value, bool isEditable)
		{
			var lookupDetail = new LookupDetail(name, value, isEditable, this);
			_lookupDetails.Add(lookupDetail);
		}

		public void AddOrReplaceLookupDetail(LookupDetail entity)
		{
			LookupDetail selectedItem = null;
			int index = 0;
			foreach(var item in _lookupDetails)
			{
				if(item.Id == entity.Id)
				{
					selectedItem = item;
					break;
				}
				index++;
			}

			if (selectedItem == null)
			{
				entity.Lookup = this;
				entity.LookupId = this.Id;
				_lookupDetails.Add(entity);
			}
			else
			{
				entity.Id = selectedItem.Id;
				entity.Lookup = this;
				entity.LookupId = this.Id;

				entity.CompanyId = selectedItem.CompanyId;
				entity.CreatedBy = selectedItem.CreatedBy;
				entity.CreatedDate = selectedItem.CreatedDate;
				entity.UpdatedBy = selectedItem.UpdatedBy;
				entity.UpdatedDate = selectedItem.UpdatedDate;

				selectedItem = entity;
				_lookupDetails[index] = selectedItem;
			}
			
		}

		public void RemoveLookupDetail(LookupDetail entity)
		{
			var selectedItem = _lookupDetails.FirstOrDefault(e => e.Id == entity.Id);
			_lookupDetails.Remove(selectedItem);
		}

		public void ClearLookupDetail()
		{
			_lookupDetails.Clear();
		}
	}
}
