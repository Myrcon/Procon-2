﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace Procon.Net.Shared.Utils {
    public static class StringExtensions {

        /// <summary>
        /// Generates a random string 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length) {
            byte[] randBuffer = new byte[length];
            RandomNumberGenerator.Create().GetBytes(randBuffer);
            return System.Convert.ToBase64String(randBuffer).Remove(length);
        }

        public static List<String> Wordify(this string data) {
            List<String> returnList = new List<String>();

            String fullWord = String.Empty;
            int quoteStack = 0;
            bool isEscaped = false;

            foreach (char input in data) {

                if (input == ' ') {
                    if (quoteStack == 0) {
                        returnList.Add(fullWord);
                        fullWord = String.Empty;
                    }
                    else {
                        fullWord += ' ';
                    }
                }
                else if (input == 'n' && isEscaped == true) {
                    fullWord += '\n';
                    isEscaped = false;
                }
                else if (input == 'r' && isEscaped == true) {
                    fullWord += '\r';
                    isEscaped = false;
                }
                else if (input == 't' && isEscaped == true) {
                    fullWord += '\t';
                    isEscaped = false;
                }
                else if (input == '"') {
                    if (isEscaped == false) {
                        if (quoteStack == 0) {
                            quoteStack++;
                        }
                        else {
                            quoteStack--;
                        }
                    }
                    else {
                        fullWord += '"';
                    }
                }
                else if (input == '\\') {
                    if (isEscaped == true) {
                        fullWord += '\\';
                        isEscaped = false;
                    }
                    else {
                        isEscaped = true;
                    }
                }
                else {
                    fullWord += input;
                    isEscaped = false;
                }
            }

            returnList.Add(fullWord);

            return returnList;
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
        public static List<String> WordWrap(this string text, int column) {
            List<String> result = new List<String>(text.Split(' '));

            for (int i = 0; i < result.Count - 1; i++) {
                if (result[i].Length + result[i + 1].Length + 1 <= column) {
                    result[i] = String.Format("{0} {1}", result[i], result[i + 1]);
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
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char t in normalizedString.Where(t => CharUnicodeInfo.GetUnicodeCategory(t) != UnicodeCategory.NonSpacingMark)) {
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
            return StringExtensions.LeetRules.Aggregate(s, (current, x) => current.Replace(x.Key, x.Value));
        }

        /// <summary>
        /// Strips both leet and diacritic from a string so it is represented
        /// in basic ASCII.
        /// </summary>
        /// <param name="s">String containing leet speak and diacritics</param>
        /// <returns>Normalized s</returns>
        public static string Strip(this string s) {
            string stripped = s;

            if (s != null) {
                stripped = stripped.RemoveLeetSpeek().RemoveDiacritics();
                stripped = new string(stripped.Where(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)).ToArray());
            }

            return stripped;
        }

        public static String SanitizeDirectory(this String s) {
            s = Regex.Replace(s, "[/\\\\]+", "_").Trim('_');
            s = Regex.Replace(s, "[^\\w]+", "");

            return s;
        }

        public static String UrlSlug(this String s) {
            String combined = s;
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
        public static String ReplaceFirst(this String text, String search, String replace) {
            int pos = text.IndexOf(search, System.StringComparison.Ordinal);
            if (pos < 0) {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}