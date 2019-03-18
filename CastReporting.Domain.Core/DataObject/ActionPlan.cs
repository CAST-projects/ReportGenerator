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
    [DataContract(Name = "ActionPlan")]
    public class ActionPlan
    {
        [DataMember(Name = "rulePattern")]
        public RulePattern RulePattern { get; set; }

        [DataMember(Name = "addedIssues")]
        public int AddedIssues { get; set; }

        [DataMember(Name = "pendingIssues")]
        public int PendingIssues { get; set; }

        [DataMember(Name = "solvedIssues")]
        public int SolvedIssues { get; set; }
    }
}
