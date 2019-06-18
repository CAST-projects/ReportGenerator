/*
 * Created by SharpDevelop.
 * User: DMA
 * Date: 15/01/2016
 * Time: 14:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace CastReporting.Repositories
{
    /// <summary>
    /// CsvSerializer to load CSV content into T objects
    /// the CSV header can be used to map CSV columns to properties from T (either by property names of by using DataMember attributes)
    /// </summary>
    public class CsvSerializer<T> where T : new()
    {
        #region property mapping

        private readonly List<PropertyInfo> _props = new List<PropertyInfo>();

        private static bool IsMatch(PropertyInfo candidate, string propName)
        {
            if (candidate.Name.ToLower() == propName)
                return true;

            foreach (var attr in candidate.GetCustomAttributes(typeof(DataMemberAttribute), true))
            {
                DataMemberAttribute dataMemberAttr = attr as DataMemberAttribute;
                if (dataMemberAttr?.Name != null && dataMemberAttr.Name.Trim().ToLower() == propName)
                    return true;
            }

            return false;
        }

        private static PropertyInfo ResolveProperty(IEnumerable<PropertyInfo> candidates, string propName)
        {
            return propName.Length == 0 ? null : candidates.FirstOrDefault(_ => IsMatch(_, propName));
        }

        private void SetTargetProps(IEnumerable<string> propNames)
        {
            _props.Clear();

            List<PropertyInfo> candidates = typeof(T).GetProperties().Where(_ => _.CanWrite && _.CanRead).ToList();
            foreach (var propName in propNames)
            {
                string pn = string.IsNullOrEmpty(propName) ? string.Empty : propName.Trim().ToLower();
                _props.Add(ResolveProperty(candidates, pn));
            }
        }

        #endregion

        #region read values from CSV

        private static string ExtractEscapedValue(string input, ref int pos, int len)
        {
            string quote = input[pos++].ToString();
            int start = pos;
            while (pos < len)
            {
                var ch = input[pos];
                if (ch == quote[0])
                {
                    if (pos + 1 < len)
                    {
                        if (input[pos + 1] == quote[0])
                        {
                            // escaped quote --> skip and continue
                            pos += 2;
                        }
                        else
                        {
                            // closing quote
                            pos++;
                            string value = input.Substring(start, pos - start - 1).Replace(quote + quote, quote);
                            // skip to next ";"
                            while (pos < len && input[pos] != ';')
                            {
                                pos++;
                            }
                            pos++;
                            return value;
                        }
                    }
                    else
                    {
                        // end of string
                        pos++;
                        return input.Substring(start, pos - start - 1).Replace(quote + quote, quote);
                    }
                }
                else
                {
                    // continue
                    pos++;
                }
            }

            // return rest of string
            return input.Substring(start);
        }

        private static string ExtractRawValue(string input, ref int pos, int len)
        {
            int start = pos;
            while (pos < len)
            {
                if (input[pos] == ';')
                {
                    // done
                    pos++;
                    return input.Substring(start, pos - start - 1);
                }
                // continue
                pos++;
            }

            // return rest of string
            return input.Substring(start);
        }

        private static IList<string> GetValues(string input)
        {
            List<string> values = new List<string>();

            int pos = 0;
            int len = input.Length;
            while (pos < len)
            {
                string value;

                char ch = input[pos];
                switch (ch)
                {
                    case ';':
                        // no value
                        value = null;
                        pos++;
                        break;
                    case '"':
                        // escaped string
                        value = ExtractEscapedValue(input, ref pos, len);
                        break;
                    default:
                        value = ExtractRawValue(input, ref pos, len);
                        break;
                }

                values.Add(value);
            }

            return values;
        }

        #endregion

        #region object creation

        private T BuildObject(IList<string> values)
        {
            var item = new T();

            int count = _props.Count;
            for (var i = 0; i < values.Count; i++)
            {
                if (0 > i || i >= count) continue;
                PropertyInfo pi = _props[i];
                try
                {
                    if (pi != null)
                        pi.SetValue(item, values[i], null);
                }
                catch (Exception ex)
                {
                    // trace and ignore
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            }

            return item;
        }

        #endregion

        public IEnumerable<T> ReadObjects(string input, int count, params string[] propMapping)
        {
            List<T> results = new List<T>();

            using (StringReader sr = new StringReader(input))
            {
                string s;

                // read header
                try
                {
                    s = sr.ReadLine();
                    if (propMapping == null || propMapping.Length == 0)
                    {
                        // use header as mapping information
                        SetTargetProps(GetValues(s));
                    }
                    else
                    {
                        // forced mapping info: ignore header
                        SetTargetProps(propMapping);
                    }
                }
                catch (Exception ex)
                {
                    // trace and return empty list
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                    return results;
                }

                // load data
                try
                {
                    while ((s = sr.ReadLine()) != null && count != 0)
                    {
                        results.Add(BuildObject(GetValues(s)));
                        if (count > 0) count--;
                    }
                }
                catch (Exception ex)
                {
                    // trace and ignore
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            }

            return results;
        }
    }
}
