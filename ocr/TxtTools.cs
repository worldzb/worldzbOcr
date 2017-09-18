using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ocr
{
    /// <summary>
    /// 字符串处理工具集
    /// </summary>
    class TxtTools
    {
        public string IdentityString(string ocrString) {
            string res = null;
            //去头去尾
            if (ocrString == null || ocrString == "")
            {
                return res;
            }
            ocrString = ocrString.Trim();
            //ocrString = ocrString.Replace(' ', '-');
            //分割
            string[] str = ocrString.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str.Length; i++)
            {
                string[] arr = str[i].Split(' ');
                str[i] = string.Concat(arr);
                res += str[i] + "\r\n";
            }

            return res;
        }
        //除去字符串中的标点符号
        public string RemoveBd(string str) {
            Regex reg = new Regex(@"\p{P}");  //正则当中 \p{P} 代表所有的标点符号
            str = reg.Replace(str, "");
            return str;
        }
        //去除空白
        public string RemoveNull(string str) {
            Regex reg = new Regex(@"\s");  //正则当中 \p{P} 代表所有的标点符号
            str = reg.Replace(str, "");
            return str;
        }
        //将易混淆的字母字符转成数字
        private string LetterToNumber(params string[] s) {
            string str = s[0];
            string regPlus = null;
            if (s.Length == 2)
            {
                regPlus = s[1];
            }
            if (str == "" || str == null)
            {
                return "";
            }
            //容错处理 错误修改
            string listReg = @"(i|l|z|Z|o|O|b|A|D|s|S|B|m|三|〇)";
            Regex reg = new Regex(listReg);
            int icount = reg.Matches(str).Count;
            if (icount != 0)
            {
                for (int i = 0; i < icount; i++)
                {
                    //校正列表
                    switch (reg.Match(str).Value)
                    {
                        case "i":
                            str = Regex.Replace(str, @"i", "1");
                            break;
                        case "l":
                            str = Regex.Replace(str, @"l", "1");
                            break;
                        case "z":
                            str = Regex.Replace(str, @"z", "2");
                            break;
                        case "Z":
                            str = Regex.Replace(str, @"Z", "2");
                            break;
                        case "o":
                            str = Regex.Replace(str, @"o", "0");
                            break;
                        case "O":
                            str = Regex.Replace(str, @"O", "0");
                            break;
                        case "b":
                            str = Regex.Replace(str, @"b", "6");
                            break;
                        case "A":
                            str = Regex.Replace(str, @"A", "4");
                            break;
                        case "D":
                            str = Regex.Replace(str, @"D", "0");
                            break;
                        case "s":
                            str = Regex.Replace(str, @"s", "5");
                            break;
                        case "S":
                            str = Regex.Replace(str, @"S", "5");
                            break;
                        case "B":
                            str = Regex.Replace(str, @"B", "8");
                            break;
                        case "m":
                            str = Regex.Replace(str, @"m", "3");
                            break;
                        case "三":
                            str = Regex.Replace(str, @"三", "3");
                            break;
                        case "〇":
                            str = Regex.Replace(str, @"〇", "0");
                            break;
                    }
                }
            }
            return str;
        }

    }
}
