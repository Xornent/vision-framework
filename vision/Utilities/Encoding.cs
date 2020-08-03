using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
