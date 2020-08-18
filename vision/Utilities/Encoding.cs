using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.International.Converters.PinYinConverter;

namespace Vision.Utilities {

    public class Encoding {
        public static string ToAscii(string str) {

            //这里我们将采用2字节一个汉字的方法来取出汉字的16进制码

            byte[] textbuf = System.Text.Encoding.Default.GetBytes(str);

            //用来存储转换过后的ASCII码

            string textAscii = string.Empty;

            for (int i = 0; i < textbuf.Length; i++) {
                textAscii += textbuf[i].ToString("X");
            }
            return textAscii;
        }

        public static string GetCapitalSpellCode(string firstChar) {
            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(firstChar);
            if (ZW.Length == 1) {
                return firstChar.ToUpper();
            } else {
                // get the array of byte from the single char 
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }

            ChineseChar cc = new ChineseChar(firstChar[0]);
            return cc.Pinyins[0].ToUpper().Substring(0,1);
        }

        public static string GetSpaces(int number) {
            switch (number) {
                case 0:return "";
                case 1: return " ";
                case 2: return "  ";
                case 3: return "   ";
                case 4: return "    ";
                case 5: return "     ";
                case 6: return "      ";
                case 7: return "       ";
                case 8: return "        ";
                case 9: return "         ";
                case 10: return "          ";
                case 11: return "           ";
                case 12: return "            ";
                case 13: return "             ";
                case 14: return "              ";
                case 15: return "               ";
                case 16: return "                ";
                case 17: return "                 ";
                case 18: return "                  ";
                case 19: return "                   ";
                default: return "    ";
            }
        }
    }
}
