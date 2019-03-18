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
using System;
using System.Linq;
using System.Reflection;
using CastReporting.Reporting.Atrributes;

namespace CastReporting.Reporting.Helper
{
    /// <summary>
    /// Common Helper Class
    /// </summary>
    public static class BlockHelper
    {      
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="blockName"></param>
        /// <returns></returns>
        public static T GetAssociatedBlockInstance<T>(string blockName) where T : class
        {
            return GetAssociatedBlockInstance<T>(Assembly.GetExecutingAssembly(), blockName);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="blockName"></param>
        /// <returns></returns>
        public static T GetAssociatedBlockInstance<T>(Assembly assembly, string blockName) where T : class
        {                       
            Type type = assembly.GetTypes()
                                .FirstOrDefault(_ => !_.IsAbstract &&
                                                      _.IsSubclassOf(typeof(T)) &&
                                                      _.GetCustomAttributes(typeof(BlockAttribute), true)
                                                       .Cast<BlockAttribute>()
                                                       .Any(a => a.Name.Equals(blockName)));
           

            if (null != type)
            {
                return Activator.CreateInstance(type) as T;
            }
           
            return null;
        }
        
       

       
    }
}
