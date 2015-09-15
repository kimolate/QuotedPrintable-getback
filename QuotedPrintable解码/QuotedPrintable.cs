using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuotedPrintable解码
{
    public class QuotedPrintable
    {
        private const string QpSinglePattern = "(\\=([0-9A-F][0-9A-F]))";

        private const string QpMutiplePattern = @"((\=[0-9A-F][0-9A-F])+=?\s*)+";

        public static string Decode(string contents, Encoding encoding)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            // 替换被编码的内容
            string result = Regex.Replace(contents, QpMutiplePattern, new MatchEvaluator(delegate(Match m)
            {
                List<byte> buffer = new List<byte>();
                // 把匹配得到的多行内容逐个匹配得到后转换成byte数组
                MatchCollection matches = Regex.Matches(m.Value, QpSinglePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                foreach (Match match in matches)
                {
                    buffer.Add((byte)HexToByte(match.Groups[2].Value.Trim()));
                }
                return encoding.GetString(buffer.ToArray());
            }), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // 替换多余的链接=号
            result = Regex.Replace(result, @"=\s+", "");

            return result;
        }

        private static int HexToByte(string hex)
        {
            int num1 = 0;
            string text1 = "0123456789ABCDEF";
            for (int num2 = 0; num2 < hex.Length; num2++)
            {
                if (text1.IndexOf(hex[num2]) == -1)
                {
                    return -1;
                }
                num1 = (num1 * 0x10) + text1.IndexOf(hex[num2]);
            }
            return num1;
        }
    }
}
