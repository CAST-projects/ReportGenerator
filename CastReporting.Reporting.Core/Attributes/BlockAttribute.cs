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

namespace CastReporting.Reporting.Atrributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class BlockAttribute : Attribute
    {
        #region PROPERTIES
        /// <summary>
        /// Get/Set the block name.
        /// </summary>
        public string Name { get; set; }
        #endregion PROPERTIES

        #region CONSTRUCTORS
        /// <summary>
        /// Initialize a new instance of a block attribute with the name given in parameter.
        /// </summary>
        /// <param name="name">Block name.</param>
        public BlockAttribute(string name)
        {
            Name = name;
        }
        #endregion CONSTRUCTORS
    }
}