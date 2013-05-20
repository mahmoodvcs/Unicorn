using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Unicorn.Data
{
    public static class Utility
    {
        /// <summary>
        /// Serializes the object to it's JSON representation.
        /// </summary>
        /// <typeparam name="T">It's not used.</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeString<T>(T obj)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(obj);
            //XmlSerializer ser = new XmlSerializer(typeof(T));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    ser.Serialize(ms, obj);
            //    ms.Position = 0;
            //    return Encoding.UTF8.GetString(ms.ToArray());
            //}
        }
        /// <summary>
        /// Serializes the object to it's JSON representation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeString(object obj)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(obj);
        }
        /// <summary>
        /// Deseializes from JSON to object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedString"></param>
        /// <returns></returns>
        public static T DeserializeString<T>(string serializedString)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(serializedString);
            //XmlSerializer ser = new XmlSerializer(typeof(T));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    byte[] bytes = Encoding.UTF8.GetBytes(serializedString);
            //    ms.Write(bytes, 0, bytes.Length);
            //    ms.Position = 0;
            //    return (T)ser.Deserialize(ms);
            //}
        }

        public static string RandomString(string allowedChars, int minLength, int maxLength)
        {
            return RandomStrings(allowedChars, minLength, maxLength, 1).First();
        }
        public static IEnumerable<string> RandomStrings(string allowedChars, int minLength, int maxLength, int count)
        {
            return RandomStrings(allowedChars, minLength, maxLength, count, ThreadLocalRandom.Instance);
        }
        public static IEnumerable<string> RandomStrings(string allowedChars, int minLength, int maxLength, int count, Random rng)
        {
            char[] chars = new char[maxLength];
            int setLength = allowedChars.Length;

            while (count-- > 0)
            {
                int length = rng.Next(minLength, maxLength + 1);

                for (int i = 0; i < length; ++i)
                {
                    chars[i] = allowedChars[rng.Next(setLength)];
                }

                yield return new string(chars, 0, length);
            }
        }
    }
}
