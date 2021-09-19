using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Utility
{
	public class ExcelToObjectMapConfig
	{
		public ExcelToObjectMapConfig()
		{

		}

		public ExcelToObjectMapConfig(string assembly, string worksheet)
		{
			this.assembly = assembly;
			this.worksheet = worksheet;
		}

		public string assembly { get; set; }
		public string worksheet { get; set; }
		public List<ColumnToPropertyMapConfig> columnsMap { get; set; } = new List<ColumnToPropertyMapConfig>();

		public string primaryKeyPropertyName
		{
			get
			{
				if (columnsMap?.Count > 0) return columnsMap[0].property;
				return string.Empty;
			}
		}

		public string AssemblyName { 
			get {
				return assembly.Split(',')[0];
			}
		}

		public string TypeName
		{
			get
			{
				return assembly.Split(',')[1];
			}
		}
	}
}
