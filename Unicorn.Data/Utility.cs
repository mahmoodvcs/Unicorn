using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Unicorn.Data
{
    public static class Utility
    {
        public static string SerializeString<T>(T obj)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.Serialize(ms, obj);
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public static T DeserializeString<T>(string serializedString)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(serializedString);
                ms.Write(bytes,0, bytes.Length);
                ms.Position = 0;
                return (T)ser.Deserialize(ms);
            }
        }
    }
}
