﻿#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace Potato.Net.Shared.Utils {
    /// <summary>
    /// A variety of string extenstions to help with various tasks
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Generates a random string 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length) {
            var randBuffer = new byte[length];
            RandomNumberGenerator.Create().GetBytes(randBuffer);
            return System.Convert.ToBase64String(randBuffer).Remove(length);
        }

        /// <summary>
        /// Splits a string into words, preserving words inside quotes
        /// </summary>
        public static List<string> Wordify(this string data) {
            var list = new List<string>();

            var word = string.Empty;
            var stack = 0;
            var escaped = false;

            foreach (var input in data) {

                if (input == ' ') {
                    if (stack == 0) {
                        list.Add(word);
                        word = string.Empty;
                    }
                    else {
                        word += ' ';
                    }
                }
                else if (input == 'n' && escaped == true) {
                    word += '\n';
                    escaped = false;
                }
                else if (input == 'r' && escaped == true) {
                    word += '\r';
                    escaped = false;
                }
                else if (input == 't' && escaped == true) {
                    word += '\t';
                    escaped = false;
                }
                else if (input == '"') {
                    if (escaped == false) {
                        if (stack == 0) {
                            stack++;
                        }
                        else {
                            stack--;
                        }
                    }
                    else {
                        word += '"';
                    }
                }
                else if (input == '\\') {
                    if (escaped == true) {
                        word += '\\';
                        escaped = false;
                    }
                    else {
                        escaped = true;
                    }
                }
                else {
                    word += input;
                    escaped = false;
                }
            }

            list.Add(word);

            return list;
        }

        /// <summary><![CDATA[
        /// Takes a string and splits it on words based on characters 
        /// string testString = "this is a string with some words in it";
        /// testString.WordWrap(10) == List<string>() { "this is a", "string", "with some", "words in", "it" }
        ///        
        /// Useful if you want to output a long string to the game and want all of the data outputed without
        /// losing any data.
        /// ]]></summary>
        /// <param name="text"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static List<string> WordWrap(this string text, int column) {
            var result = new List<string>(text.Split(' '));

            for (var i = 0; i < result.Count - 1; i++) {
                if (result[i].Length + result[i + 1].Length + 1 <= column) {
                    result[i] = string.Format("{0} {1}", result[i], result[i + 1]);
                    result.RemoveAt(i + 1);
                    i--;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes diacritics, such as the umlaut (ë => e).  Though not linguistically
        /// correct it does allow UTF8 to be represented in ASCII, at least to a degree
        /// that will alert translators that further translation is required.
        /// </summary>
        /// <param name="s">String containing diacritics</param>
        /// <returns>Normalized s</returns>
        public static string RemoveDiacritics(this string s) {
            var normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var t in normalizedString.Where(t => CharUnicodeInfo.GetUnicodeCategory(t) != UnicodeCategory.NonSpacingMark)) {
                stringBuilder.Append(t);
            }

            return stringBuilder.ToString();
        }

        private static readonly Dictionary<string, string> LeetRules = new Dictionary<string, string>() {
            { "4", "A" },
            { @"/\", "A" },
            { "@", "A" },
            { "^", "A" },
            { "13", "B" },
            { "/3", "B" },
            { "|3", "B" },
            { "8", "B" },
            { "><", "X" },
            { "<", "C" },
            { "(", "C" },
            { "|)", "D" },
            { "|>", "D" },
            { "3", "E" },
            { "6", "G" },
            { "/-/", "H" },
            { "[-]", "H" },
            { "]-[", "H" },
            { "!", "I" },
            { "|_", "L" },
            { "_/", "J" },
            { "_|", "J" },
            { "1", "L" },
            { "0", "O" },
            { "5", "S" },
            { "7", "T" },
            { @"\/\/", "W" },
            { @"\/", "V" },
            { "2", "Z" }
        };

        /// <summary>
        /// Removes leet speak from a name
        /// 
        /// P]-[0gu3 => Phogue
        /// </summary>
        /// <param name="s">String containing leet speak</param>
        /// <returns>Normalized s</returns>
        public static string RemoveLeetSpeek(this string s) {
            return LeetRules.Aggregate(s, (current, x) => current.Replace(x.Key, x.Value));
        }

        /// <summary>
        /// Strips both leet and diacritic from a string so it is represented
        /// in basic ASCII.
        /// </summary>
        /// <param name="s">String containing leet speak and diacritics</param>
        /// <returns>Normalized s</returns>
        public static string Strip(this string s) {
            var stripped = s;

            if (s != null) {
                stripped = stripped.RemoveLeetSpeek().RemoveDiacritics();
                stripped = new string(stripped.Where(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)).ToArray());
            }

            return stripped;
        }

        /// <summary>
        /// Sanitizes a directory name (replaces all non-word chars with hyphens)
        /// </summary>
        public static string SanitizeDirectory(this string s) {
            s = Regex.Replace(s, "[^\\w]+", "-").Trim('-');

            return s;
        }

        /// <summary>
        /// Strips the query string from a url and sanitizes the remaining text
        /// </summary>
        public static string Slug(this string s) {
            var combined = s;
            Uri uri;

            if (Uri.TryCreate(s, UriKind.Absolute, out uri) == true) {
                combined = uri.Host + uri.PathAndQuery;
            }

            return combined.SanitizeDirectory();
        }
        
        /// <summary>
        /// Replaces the first occurance of a string within another string
        /// </summary>
        /// <param name="text">The source text to find replacements</param>
        /// <param name="search">The string to search for</param>
        /// <param name="replace">The text to replace with</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string search, string replace) {
            var pos = text.IndexOf(search, System.StringComparison.Ordinal);
            if (pos < 0) {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
