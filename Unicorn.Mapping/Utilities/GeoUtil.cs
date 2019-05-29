using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mapping.Utilities
{
    public static class GeoUtils
    {
        public static DbGeography CreatePolygon(params Point[] points)
        {
            var ss = new List<string>();
            foreach (var p in points)
            {
                ss.Add(string.Format("{0} {1}", p.X, p.Y));
            }
            return DbGeography.PolygonFromText(string.Format("POLYGON (({0}))", string.Join(",", ss)), 4326);
        }
        
        /// <summary>
        /// Create a GeoLocation point based on latitude and longitude
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static DbGeography CreatePoint(double latitude, double longitude)
        {
            var text = string.Format("POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(text, 4326);
        }

        /// <summary>
        /// Create a GeoLocation point based on latitude and longitude
        /// </summary>
        /// <param name="latitudeLongitude">
        /// String should be two values either single comma or space delimited
        /// 45.710030,-121.516153
        /// 45.710030 -121.516153
        /// </param>
        /// <returns></returns>
        public static DbGeography CreatePoint(string latitudeLongitude)
        {
            var tokens = latitudeLongitude.Split(',', ' ');
            if (tokens.Length != 2)
                throw new ArgumentException($"Invalid location string {latitudeLongitude}.");
            var text = string.Format("POINT({0} {1})", tokens[1], tokens[0]);
            return DbGeography.PointFromText(text, 4326);
        }

    }
}
