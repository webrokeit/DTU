/*
 * Extension methods for easily parsing the numbers read from the input stream
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
 */

namespace MFMSTProject.Util {
    public static class StringExtensions {
        public static int ToInt(this string str) {
            return ToInt(str, 0);
        }

        public static int ToInt(this string str, int fallback) {
            int read;
            return int.TryParse(str, out read) ? read : fallback;
        }
    }
}
