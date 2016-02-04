/*
 *   Copyright (c) 2016 CAST
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
using System;
using System.IO;
using System.Xml.Serialization;

namespace CastReporting.Repositories.Util
{
    public static class SerializerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="filePath"></param>
        static public void SerializeToFile<T>(T instance, string filePath)
        {
            FileStream stream = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
               
                stream = File.Open(filePath, FileMode.Create);

                serializer.Serialize(stream, instance);

                stream.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        static public string SerializeToString<T>(T instance)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                StringWriter sw = new StringWriter();

                serializer.Serialize(sw, instance);

                return sw.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static public T DeserializeFromFile<T>(string filePath, XmlRootAttribute overrides=null)
        {
            FileStream stream = null;

            try
            {
                
                XmlSerializer serializer = (overrides==null)?new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T),overrides);

                stream = File.OpenRead(filePath);

                return (T)serializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        static public T DeserializeFromString<T>(string value)
        {
            FileStream stream = null;

            try
            {
                StringReader textReader = new StringReader(value);

                XmlSerializer serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(textReader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
    }
}
