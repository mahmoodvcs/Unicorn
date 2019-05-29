using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mapping
{
    public interface ILocationBased
    {
        DbGeography Location { get; set; }
    }
}
