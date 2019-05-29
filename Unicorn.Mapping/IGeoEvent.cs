﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mapping
{
    public interface IGeoEvent : ILocationBased
    {
        int Id { get; set; }
        DateTime DateTime { get; set; }

    }
}
