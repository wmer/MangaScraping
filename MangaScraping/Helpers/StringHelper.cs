using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Helpers {
    internal class StringHelper {
        public static string RemoveSpecialCharacters(String str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
