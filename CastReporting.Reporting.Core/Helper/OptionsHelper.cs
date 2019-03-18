/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System.Collections.Generic;

namespace CastReporting.Reporting.Helper
{
    public static class OptionsHelper
    {
        public static string GetOption(this Dictionary<string, string> options, string key, string defaultValue = default(string)) {
            string value;
            if (options == null || !options.TryGetValue(key, out value)) {
                value = defaultValue;
            }
            return value?.Replace("\t", "").Replace("\n", "").Replace("\r", "");
        }

        public static int GetIntOption(this Dictionary<string, string> options, string key, int defaultValue = default(int)) {
            int value;
            var s = options.GetOption(key);
            if (string.IsNullOrWhiteSpace(s) || !int.TryParse(s, out value)) {
                value = defaultValue;
            }
            return value;
        }

        public static bool GetBoolOption(this Dictionary<string, string> options, string key, bool defaultValue = default(bool)) {
            bool value;
            var s = options.GetOption(key);
            if (string.IsNullOrWhiteSpace(s)) {
                value = defaultValue;
            } else {
                int v;
                if (int.TryParse(s, out v)) {
                    value = (v != 0);
                } else {
                    s = s.ToLower();
                    switch (s) {
                        case "1":
                        case "y":
                        case "yes":
                        case "true":
                            value = true;
                            break;
                        case "0":
                        case "n":
                        case "no":
                        case "false":
                            value = false;
                            break;
                        default:
                            value = defaultValue;
                            break;
                    }
                }
            }
            return value;
        }

    }
}