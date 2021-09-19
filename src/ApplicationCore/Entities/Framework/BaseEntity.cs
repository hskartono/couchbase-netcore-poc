using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class BaseEntity : CoreEntity
	{
		public enum BasisPartType
		{
			PART_NUMBER=1,
			KATASHIKI_SUFFIX=2
		}

		public enum DraftStatus
		{
			All = -1,
			MainRecord = 0,
			DraftMode = 1,
			Saved = 2,
			Discarded = 3
		}

		public enum SlocType
		{
			GOOD_SLOC = 1,
			LINE_SLOC = 2,
			NG_SLOC = 3,
			OTHER_SLOC = 4
		}
		public int CompanyId { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }

		// untuk penanda bahwa ini adalah record draft, baik itu ketika create, atau ketika edit mode.
		// 0 - main record / record normal, seluruh select harus mencari yang ```IsDraftRecord == 0```
		// 1 - record duplikat sedang di create/edit (draft mode)
		// 2 - record duplikat sudah di save
		// 3 - record duplikat sudah di discard
		public int? IsDraftRecord { get; set; } = 0;

		// tanggal ketika terjadi DRAFT/COMMIT/DISCARD
		public DateTime? RecordActionDate { get; set; }

		// id record induk (jika mode edit)
		public int? MainRecordId { get; set; } = null;

		// user_name dari pemilik record yang sedang di edit
		public string RecordEditedBy { get; set; }

		// untuk penanda bahwa ini adalah record baru yang akan di add, yang di input melalui file upload
		public bool? DraftFromUpload { get; set; } = false;


		// field pembantu untuk penanda validasi dan referensi foreign key / primary key
		public virtual string UploadValidationStatus { get; set; } = "Success";
		public virtual string UploadValidationMessage { get; set; }

		[NotMapped]
		public virtual string _RefPrimaryKey { get; set; }

		[NotMapped]
		public virtual string _RefForeignKey { get; set; }

		[NotMapped]
		public virtual bool isFromUpload { get; set; } = false;

		[NotMapped]
        public virtual int UploadRowIndex { get; set; }

        public DateTime? DeletedAt { get; set; }

		public void AddValidationMessage(string message)
		{
			if (!string.IsNullOrEmpty(UploadValidationMessage)) UploadValidationMessage += Environment.NewLine;
			UploadValidationMessage += message;
			if (string.IsNullOrEmpty(UploadValidationMessage))
				UploadValidationStatus = "Success";
			else
				UploadValidationStatus = "Failed";
		}

	}
}
