using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xlmap
{
    public class ObjectMapDetailConfig
    {
        public enum DataType
        {
            INTEGER,
            DOUBLE,
            STRING,
            DATETIME,
            OBJECT,
            BOOLEAN
        }
        public bool isRequired { get; set; } = false;
        public bool IsPrimaryKey { get; set; } = false;
        public string ExcelFieldTitle { get; set; }
        public string RefTableName { get; set; }
        public string RefFieldName { get; set; }
        public string FieldName { get; set; }
        public string SourceProperty { get; set; }
        public string ObjectPrimaryKeyProperty { get; set; }
        public string DestinationKeyProperty { get; set; }
        public string DestinationProperty { get; set; }
        public DataType DestinationPropertyDataType { get; set; }
        public string DestinationAssembly { get; set; }

    }
}
