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
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "component")]
    public class ComponentWithProperties
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "name")]
        public string Name {get;set;}

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "shortName")]
        public string ShortName { get; set; }

        [DataMember(Name = "propagationRiskIndex")]
        public double PropagationRiskIndex { get; set; }

        [DataMember(Name = "codeLines")]
        public double? CodeLines { get; set; }
        
        [DataMember(Name = "commentedCodeLines")]
        public double? CommentedCodeLines { get; set; }

        [DataMember(Name = "commentLines")]
        public double? CommentLines { get; set; }

        [DataMember(Name = "coupling")]
        public double? Coupling { get; set; }

        [DataMember(Name = "fanIn")]
        public double? FanIn { get; set; }

        [DataMember(Name = "fanOut")]
        public double? FanOut { get; set; }

        [DataMember(Name = "cyclomaticComplexity")]
        public double? CyclomaticComplexity { get; set; }

        [DataMember(Name = "ratioCommentLinesCodeLines")]
        public double? RatioCommentLinesCodeLines { get; set; }

        [DataMember(Name = "halsteadProgramLength")]
        public double? HalsteadProgramLength { get; set; }

        [DataMember(Name = "halsteadProgramVocabulary")]
        public double? HalsteadProgramVocabulary { get; set; }

        [DataMember(Name = "halsteadVolume")]
        public double? HalsteadVolume { get; set; }

        [DataMember(Name = "distinctOperators")]
        public double? DistinctOperators { get; set; }

        [DataMember(Name = "distinctOperands")]
        public double? DistinctOperands { get; set; }

        [DataMember(Name = "integrationComplexity")]
        public double? IntegrationComplexity { get; set; }

        [DataMember(Name = "essentialComplexity")]
        public double? EssentialComplexity { get; set; }


        public double? GetPropertyValue(string prop)
        {
            switch (prop)
            {
                case "codeLines":
                    return CodeLines;
                case "commentedCodeLines":
                    return CommentedCodeLines;
                case "commentLines":
                    return CommentLines;
                case "coupling":
                    return Coupling;
                case "fanIn":
                    return FanIn;
                case "fanOut":
                    return FanOut;
                case "cyclomaticComplexity":
                    return CyclomaticComplexity;
                case "ratioCommentLinesCodeLines":
                    return RatioCommentLinesCodeLines;
                case "halsteadProgramLength":
                    return HalsteadProgramLength;
                case "halsteadProgramVocabulary":
                    return HalsteadProgramVocabulary;
                case "halsteadVolume":
                    return HalsteadVolume;
                case "distinctOperators":
                    return DistinctOperators;
                case "distinctOperands":
                    return DistinctOperands;
                case "integrationComplexity":
                    return IntegrationComplexity;
                case "essentialComplexity":
                    return EssentialComplexity;
                default:
                    return null;
            }
        }

        public string GetPropertyValueString(string prop)
        {
            switch (prop)
            {
                case "codeLines":
                    return CodeLines?.ToString("N0") ?? string.Empty;
                case "commentedCodeLines":
                    return CommentedCodeLines?.ToString("N0") ?? string.Empty;
                case "commentLines":
                    return CommentLines?.ToString("N0") ?? string.Empty;
                case "coupling":
                    return Coupling?.ToString("N0") ?? string.Empty;
                case "fanIn":
                    return FanIn?.ToString("N0") ?? string.Empty;
                case "fanOut":
                    return FanOut?.ToString("N0") ?? string.Empty;
                case "cyclomaticComplexity":
                    return CyclomaticComplexity?.ToString("N0") ?? string.Empty;
                case "ratioCommentLinesCodeLines":
                    return RatioCommentLinesCodeLines?.ToString("N2") ?? string.Empty;
                case "halsteadProgramLength":
                    return HalsteadProgramLength?.ToString("N0") ?? string.Empty;
                case "halsteadProgramVocabulary":
                    return HalsteadProgramVocabulary?.ToString("N0") ?? string.Empty;
                case "halsteadVolume":
                    return HalsteadVolume?.ToString("N2") ?? string.Empty;
                case "distinctOperators":
                    return DistinctOperators?.ToString("N0") ?? string.Empty;
                case "distinctOperands":
                    return DistinctOperands?.ToString("N0") ?? string.Empty;
                case "integrationComplexity":
                    return IntegrationComplexity?.ToString("N0") ?? string.Empty;
                case "essentialComplexity":
                    return EssentialComplexity?.ToString("N0") ?? string.Empty;
                default:
                    return string.Empty;
            }
        }

    }

}
