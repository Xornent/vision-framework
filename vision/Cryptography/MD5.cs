using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Cryptography {

    public static class MD5 {

        public static System.Security.Cryptography.MD5 MD5Provider 
            = System.Security.Cryptography.MD5.Create();

        public static string Encrypt(string str) {
            try {
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(str);
                bytHash = MD5Provider.ComputeHash(bytValue);

                StringBuilder builder = new StringBuilder();
                foreach(byte b in bytHash) {
                    builder.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                return builder.ToString().ToLower();

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return "00000000000000000000000000000000";
        }
    }
}
