using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xlmap
{
    public class ObjectMapConfig
    {
        public string TableName { get; set; }
        public string PrimaryKeyField { get; set; }
        public string DestinationPrimaryKeyProperty { get; set; }
        public string FullAssemblyName { get; set; }
        public string WorksheetName { get; set; }
        public List<ObjectMapDetailConfig> ParentFields { get; set; } = new List<ObjectMapDetailConfig>();
        public string ChildTableName { get; set; }
        public string ForeignKeyField { get; set; }
        public string DestinationForeignProperty { get; set; }
        public string DestinationChildPropertyName { get; set; }
        public string ChildFullAssemblyName { get; set; }
        public string ChildWorksheetName { get; set; }

        [DefaultValue(null)]
        public string OrderBy { get; set; } = null;

        [DefaultValue(null)]
        public string OrderByType { get; set; } = null;
        public List<ObjectMapDetailConfig> ChildFields { get; set; } = new List<ObjectMapDetailConfig>();
    }
}
