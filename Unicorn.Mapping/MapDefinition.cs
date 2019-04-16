using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mapping
{
    public class MapDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<string> BaseLayers { get; set; }
        public string BaseLayersSerialized
        {
            get { return string.Join(",", BaseLayers); }
            set
            {
                if (value == null)
                    BaseLayers = new List<string>();
                else
                    BaseLayers = value.Split(',').ToList();
            }
        }

        public List<GeoJsonFileInfo> GeoJsonFiles { get; set; }
    }
}
