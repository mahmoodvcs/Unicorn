using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mapping.Utilities
{
    public static class LocationUtils
    {
        public static Tuple<DbGeography, DbGeography> GetBounds(IEnumerable<ILocationBased> locations)
        {
            return GetBounds(locations.Select(a => a.Location));
        }
        public static Tuple<DbGeography, DbGeography> GetBounds(IEnumerable<DbGeography> locations)
        {
            double minx = 1000, miny = 1000, maxx = -1000, maxy = -1000;
            foreach (var l in locations)
            {
                if (l.Longitude > maxx)
                    maxx = l.Longitude.Value;
                if (l.Longitude < minx)
                    minx = l.Longitude.Value;
                if (l.Latitude > maxy)
                    maxy = l.Latitude.Value;
                if (l.Latitude < miny)
                    miny = l.Latitude.Value;
            }

            return new Tuple<DbGeography, DbGeography>(GeoUtils.CreatePoint(minx, miny), GeoUtils.CreatePoint(maxx, maxy));
        }
    }
}
