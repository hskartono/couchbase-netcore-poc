using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Utility
{
	public class ColumnToPropertyMapConfig
	{
		public ColumnToPropertyMapConfig()
		{

		}

		public ColumnToPropertyMapConfig(string property, int colIndex, string type)
		{
			this.property = property;
			this.colIndex = colIndex;
			this.type = type;
		}

		public ColumnToPropertyMapConfig(string property, int colIndex, string type, bool isRequired)
		{
			this.property = property;
			this.colIndex = colIndex;
			this.type = type;
			this.isRequired = isRequired;
		}

		public string property { get; set; }
		public bool PK { get; set; } = false;
		public bool FK { get; set; } = false;
		public int colIndex { get; set; }
		public string type { get; set; }
		public bool isRequired { get; set; } = false;
		public bool defaultNull { get; set; } = false;
		public string assembly { get; set; }
		public string mapFile { get; set; }
		public string title { get; set; }
		public string format { get; set; }

		private ExcelToObjectMapConfig _excelToObjectMapConfig = null;
		public ExcelToObjectMapConfig LoadRelatedCollection()
		{
			if (_excelToObjectMapConfig != null) return _excelToObjectMapConfig;
			if (type?.Trim()?.ToLower() != "collection") return null;
			if (string.IsNullOrEmpty(mapFile)) return null;

			_excelToObjectMapConfig = ExcelMapper.LoadConfig(mapFile);
			return _excelToObjectMapConfig;
		}

		public ExcelToObjectMapConfig LoadRelatedCollection(bool forceReload)
		{
			if (forceReload) _excelToObjectMapConfig = null;
			return LoadRelatedCollection();
		}
	}
}
