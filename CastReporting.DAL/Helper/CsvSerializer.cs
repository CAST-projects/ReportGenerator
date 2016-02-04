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
        public CsvSerializer()
        {
        }

        #region property mapping

        private readonly List<PropertyInfo> _props = new List<PropertyInfo>();

        private static bool IsMatch(PropertyInfo Candidate, string PropName)
        {
            if (Candidate.Name.ToLower() == PropName)
                return true;

            foreach (var attr in Candidate.GetCustomAttributes(typeof(DataMemberAttribute), true))
            {
                DataMemberAttribute dataMemberAttr = attr as DataMemberAttribute;
                if (dataMemberAttr != null && dataMemberAttr.Name != null && dataMemberAttr.Name.Trim().ToLower() == PropName)
                    return true;
            }

            return false;
        }

        private PropertyInfo ResolveProperty(IEnumerable<PropertyInfo> Candidates, string PropName)
        {
            return (PropName.Length == 0) ? null : Candidates.FirstOrDefault(_ => IsMatch(_, PropName));
        }

        private void SetTargetProps(IEnumerable<string> PropNames)
        {
            _props.Clear();

            List<PropertyInfo> candidates = typeof(T).GetProperties().Where(_ => _.CanWrite && _.CanRead).ToList();
            foreach (var propName in PropNames)
            {
                string pn = string.IsNullOrEmpty(propName) ? string.Empty : propName.Trim().ToLower();
                _props.Add(ResolveProperty(candidates, pn));
            }
        }

        #endregion

        #region read values from CSV

        private string ExtractEscapedValue(string Input, ref int Pos, int Len)
        {
            string quote = Input[Pos++].ToString();
            int start = Pos;
            while (Pos < Len)
            {
                var ch = Input[Pos];
                if (ch == quote[0])
                {
                    if (Pos + 1 < Len)
                    {
                        if (Input[Pos + 1] == quote[0])
                        {
                            // escaped quote --> skip and continue
                            Pos += 2;
                        }
                        else
                        {
                            // closing quote
                            Pos++;
                            string value = Input.Substring(start, Pos - start - 1).Replace(quote + quote, quote);
                            // skip to next ";"
                            while (Pos < Len && Input[Pos] != ';')
                            {
                                Pos++;
                            }
                            Pos++;
                            return value;
                        }
                    }
                    else
                    {
                        // end of string
                        Pos++;
                        return Input.Substring(start, Pos - start - 1).Replace(quote + quote, quote);
                    }
                }
                else
                {
                    // continue
                    Pos++;
                }
            }

            // return rest of string
            return Input.Substring(start);
        }

        private string ExtractRawValue(string Input, ref int Pos, int Len)
        {
            int start = Pos;
            while (Pos < Len)
            {
                if (Input[Pos] == ';')
                {
                    // done
                    Pos++;
                    return Input.Substring(start, Pos - start - 1);
                }
                else
                {
                    // continue
                    Pos++;
                }
            }

            // return rest of string
            return Input.Substring(start);
        }

        private IList<string> GetValues(string Input)
        {
            List<string> values = new List<string>();

            int pos = 0;
            int len = Input.Length;
            while (pos < len)
            {
                string value;

                char ch = Input[pos];
                if (ch == ';')
                {
                    // no value
                    value = null;
                    pos++;
                }
                else if (ch == '"')
                {
                    // escaped string
                    value = ExtractEscapedValue(Input, ref pos, len);
                }
                else
                {
                    // raw value
                    value = ExtractRawValue(Input, ref pos, len);
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
                if (0 <= i && i < count)
                {
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
            }

            return item;
        }

        #endregion

        public IEnumerable<T> ReadObjects(string Input, int count, params string[] PropMapping)
        {
            List<T> results = new List<T>();

            using (StringReader sr = new StringReader(Input))
            {
                string s;

                // read header
                try
                {
                    s = sr.ReadLine();
                    if (PropMapping == null || PropMapping.Length == 0)
                    {
                        // use header as mapping information
                        SetTargetProps(GetValues(s));
                    }
                    else
                    {
                        // forced mapping info: ignore header
                        SetTargetProps(PropMapping);
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
