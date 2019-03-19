/*
 *   Copyright (c) 2013 CAST
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
using System.Reflection;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.Reporting.Block.Text
{
	[Block("REPGEN_VERSION"), Block("EMP_VERSION")]
	public class RepGenVersion : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
        	var ver = GetVersion(Assembly.GetExecutingAssembly());
        	if (string.IsNullOrEmpty(ver))
        		ver = Domain.Constants.No_Value;
        	
        	return ver;
        }
        
        private static string GetVersion(Assembly asm) {
        	if (asm == null)
        		return null;
        	var n = asm.GetName();
        	return n.Version == null ? null : n.Version.ToString();
        }
        #endregion METHODS
    }
}
