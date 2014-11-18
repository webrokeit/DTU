using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Lib {
    public static class Fnc {
        public static IEnumerable<T> Values<T>() {
            var t = typeof(T);
            return t.IsEnum ? Enum.GetValues(t).Cast<T>() : new T[] { };
        }

        public static T NthLast<T>(this T[] arr, int n) {
            return arr[arr.Length - n];
        }

        public static string EscapePath(this string path, char escapeCharacter = '_') {
            return Path.GetInvalidPathChars().Aggregate(path, (current, c) => current.Replace(c, escapeCharacter));
        }

        public static string GetUnityApplicationName() {
            return Application.dataPath.Split('/').NthLast(2).EscapePath();
        }

        public static void Restart(this Stopwatch self) {
            self.Reset();
            self.Start();
        }
    }
}
