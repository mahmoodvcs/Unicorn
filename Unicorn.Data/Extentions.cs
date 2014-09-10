using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Unicorn
{
    public static class MultilangExtentions
    {
        public static string ToArabicNumerals(this string input)
        {
            System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
            System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
            System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
            char[] convertedChar = new char[1];
            byte[] bytes = new byte[] { 217, 160 };
            char[] inputCharArray = input.ToCharArray();
            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    convertedChars.Append(convertedChar[0]);
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();
        }

        public static int ParseInt(string s)
        {
            string EnglishNumbers = "";
            for (int i = 0; i < s.Length; i++)
            {
                EnglishNumbers += char.GetNumericValue(s, i);
            }
            return Convert.ToInt32(EnglishNumbers);
        }
    }
}