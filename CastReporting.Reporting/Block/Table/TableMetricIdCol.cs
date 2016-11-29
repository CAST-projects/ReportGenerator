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
using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using System.Linq;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TABLE_METRIC_ID_COL")]
    class TableMetricIdCol : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS

            var rowData = new List<string>();
            int cntRow = 0;
            const string gradeFormat = "N2";
            const string valueFormat = "N0";

            // by default, short names
            bool displayShortHeader = options.GetBoolOption("HEADER", true);

            string[] qidList = options.GetOption("QID")?.Split('|');
            string[] sidList = options.GetOption("SID")?.Split('|');
            string[] bidList = options.GetOption("BID")?.Split('|');

            string _snapshot = options.GetOption("SNAPSHOT", "BOTH"); // SNAPSHOT can be CURRENT, PREVIOUS or BOTH
            string _level = options.GetOption("LEVEL", "APPLICATION"); // LEVEL can be APPLICATION, MODULES or TECHNOLOGIES
            string _variation = options.GetOption("VARIATION", "PERCENT"); // VARIATION can be VALUE, PERCENT or BOTH


            if (reportData?.CurrentSnapshot?.BusinessCriteriaResults == null) return null;

            bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

            #region Get Names

            int cntMetric = 0;
            Dictionary<string, string> names = new Dictionary<string, string>();
            Dictionary<string,Result> bfResults = new Dictionary<string, Result>();
            if (qidList != null)
            {
                foreach (string id in qidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    names[id] = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(id), displayShortHeader);
                    cntMetric++;
                }
            }
            if (sidList != null)
            {
                foreach (string id in sidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    names[id] = MeasureUtility.GetSizingMeasureName(reportData.CurrentSnapshot, int.Parse(id), displayShortHeader);
                    cntMetric++;
                }
            }
            // No background facts for technologies
            if (bidList != null && _level != "TECHNOLOGIES")
            {
                foreach (string id in bidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, id,true, true).FirstOrDefault();
                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                    string bfName = (displayShortHeader
                        ? bfResult.ApplicationResults[0].Reference.ShortName
                        : bfResult.ApplicationResults[0].Reference.Name)
                        ?? bfResult.ApplicationResults[0].Reference.Name;
                    names[id] = bfName;
                    cntMetric++;
                    if (bfResults.Keys.Contains(id)) continue;
                    bfResults[id] = bfResult;
                }
            }

            rowData.Add(" ");
            rowData.AddRange(names.Values);

            #endregion

            #region CurrentSnapshot
            Dictionary<string, string> currentValues = new Dictionary<string, string>();
            Dictionary<int, Dictionary<string, string>> modCurValues = new Dictionary<int, Dictionary<string, string>>();
            var curModules = reportData.CurrentSnapshot.Modules.Distinct().ToList();
            var curTechnos = reportData.CurrentSnapshot.Technologies.ToList();
            Dictionary<string, Dictionary<string, string>> technoCurValues = new Dictionary<string, Dictionary<string, string>>();

            if (_snapshot == "CURRENT" || _snapshot == "BOTH")
            {
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                rowData.Add(currSnapshotLabel);
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (_level)
                {
                    case "APPLICATION":
                        if (qidList != null)
                        {
                            foreach (string id in qidList)
                            {
                                if (currentValues.Keys.Contains(id)) continue;
                                var _metricValue = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, int.Parse(id));
                                currentValues[id] = _metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                            }
                        }
                        if (sidList != null)
                        {
                            foreach (string id in sidList)
                            {
                                if (currentValues.Keys.Contains(id)) continue;
                                var _metricValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, int.Parse(id));
                                currentValues[id] = _metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                            }
                        }
                        if (bidList != null)
                        {
                            foreach (string id in bidList)
                            {
                                if (currentValues.Keys.Contains(id)) continue;
                                Result bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                double? bfValue = bfResult.ApplicationResults[0].DetailResult.Value;
                                currentValues[id] = bfValue?.ToString(valueFormat) ?? Constants.No_Value;
                            }
                        }
                        rowData.AddRange(currentValues.Values);
                        cntRow++;
                        break;
                    case "MODULES":
                        for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                        foreach (Module module in curModules)
                        {
                            Dictionary<string, string> currValues = new Dictionary<string, string>();
                            rowData.Add(module.Name);
                            if (qidList != null)
                            {
                                foreach (string id in qidList)
                                {
                                    if (currValues.Keys.Contains(id)) continue;
                                    var metricValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.CurrentSnapshot, module.Id, int.Parse(id));
                                    currValues[id] = metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                                }
                            }
                            if (sidList != null)
                            {
                                foreach(string id in sidList)
                                {
                                    if (currValues.Keys.Contains(id)) continue;
                                    var metricValue = MeasureUtility.GetSizingMeasureModule(reportData.CurrentSnapshot, module.Id, int.Parse(id));
                                    currValues[id] = metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            if (bidList != null)
                            {
                                foreach (string id in bidList)
                                {
                                    if (currValues.Keys.Contains(id)) continue;
                                    Result bfResult = bfResults.Keys.Contains(id) ? bfResults[id] : null;
                                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                    double? bfValue = bfResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id).DetailResult.Value;
                                    currValues[id] = bfValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            rowData.AddRange(currValues.Values);
                            cntRow++;
                            modCurValues[module.Id] = currValues;
                        }
                        break;
                    case "TECHNOLOGIES":
                        for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                        foreach (string techno in curTechnos)
                        {
                            Dictionary<string, string> currValues = new Dictionary<string, string>();
                            rowData.Add(techno);
                            if (qidList != null)
                            {
                                foreach (string id in qidList)
                                {
                                    if (currValues.Keys.Contains(id)) continue;
                                    var metricValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.CurrentSnapshot, techno, int.Parse(id));
                                    currValues[id] = metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                                }
                            }
                            if (sidList != null)
                            {
                                foreach (string id in sidList)
                                {
                                    if (currValues.Keys.Contains(id)) continue;
                                    var metricValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.CurrentSnapshot,techno, int.Parse(id));
                                    currValues[id] = metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            rowData.AddRange(currValues.Values);
                            cntRow++;
                            technoCurValues[techno] = currValues;
                        }
                        break;
                }
            }

            #endregion

            if (_snapshot == "BOTH")
            {
                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                cntRow++;
            }

            #region PreviousSnapshot
            Dictionary<string, string> previousValues = new Dictionary<string, string>();
            Dictionary<string, Result> bfPrevResults = new Dictionary<string, Result>();
            Dictionary<int, Dictionary<string, string>> modPrevValues = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> technoPrevValues = new Dictionary<string, Dictionary<string, string>>();

            if (hasPreviousSnapshot && (_snapshot == "PREVIOUS" || _snapshot == "BOTH"))
            {
                string prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                rowData.Add(prevSnapshotLabel);

                if (bidList != null && _level != "TECHNOLOGIES")
                {
                    foreach (string id in bidList)
                    {
                        if (bfPrevResults.Keys.Contains(id)) continue;
                        Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.PreviousSnapshot.Href, id, true, true).FirstOrDefault();
                        if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                        bfPrevResults[id] = bfResult;
                    }
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (_level)
                {
                    case "APPLICATION":
                        if (qidList != null)
                        {
                            foreach (string id in qidList)
                            {
                                if (previousValues.Keys.Contains(id)) continue;
                                var _metricValue = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, int.Parse(id));
                                previousValues[id] = _metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                            }
                        }
                        if (sidList != null)
                        {
                            foreach (string id in sidList)
                            {
                                if (previousValues.Keys.Contains(id)) continue;
                                var _metricValue = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, int.Parse(id));
                                previousValues[id] = _metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                            }
                        }
                        if (bidList != null)
                        {
                            foreach (string id in bidList)
                            {
                                if (previousValues.Keys.Contains(id)) continue;
                                Result bfResult = bfPrevResults.Keys.Contains(id) ? bfPrevResults[id] : null;
                                if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                double? bfValue = bfResult.ApplicationResults[0].DetailResult.Value;
                                previousValues[id] = bfValue?.ToString(valueFormat) ?? Constants.No_Value;
                            }
                        }
                        rowData.AddRange(previousValues.Values);
                        cntRow++;
                        break;
                    case "MODULES":
                        for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                        var prevModules = reportData.PreviousSnapshot.Modules.Distinct().ToList();
                        foreach (Module module in prevModules)
                        {
                            Dictionary<string, string> prevValues = new Dictionary<string, string>();
                            rowData.Add(module.Name);
                            if (qidList != null)
                            {
                                foreach (string id in qidList)
                                {
                                    if (prevValues.Keys.Contains(id)) continue;
                                    var metricValue = BusinessCriteriaUtility.GetQualityIndicatorModuleGrade(reportData.PreviousSnapshot, module.Id, int.Parse(id));
                                    prevValues[id] = metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                                }
                            }
                            if (sidList != null)
                            {
                                foreach (string id in sidList)
                                {
                                    if (prevValues.Keys.Contains(id)) continue;
                                    var metricValue = MeasureUtility.GetSizingMeasureModule(reportData.PreviousSnapshot, module.Id, int.Parse(id));
                                    prevValues[id] = metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            if (bidList != null)
                            {
                                foreach (string id in bidList)
                                {
                                    if (prevValues.Keys.Contains(id)) continue;
                                    Result bfResult = bfPrevResults.Keys.Contains(id) ? bfPrevResults[id] : null;
                                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                                    double? bfValue = bfResult.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id).DetailResult.Value;
                                    prevValues[id] = bfValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            rowData.AddRange(prevValues.Values);
                            cntRow++;
                            modPrevValues[module.Id] = prevValues;
                        }
                        break;
                    case "TECHNOLOGIES":
                        for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                        foreach (string techno in curTechnos)
                        {
                            Dictionary<string, string> prevValues = new Dictionary<string, string>();
                            rowData.Add(techno);
                            if (qidList != null)
                            {
                                foreach (string id in qidList)
                                {
                                    if (prevValues.Keys.Contains(id)) continue;
                                    var metricValue = BusinessCriteriaUtility.GetQualityIndicatorTechnologyGrade(reportData.PreviousSnapshot, techno, int.Parse(id));
                                    prevValues[id] = metricValue?.ToString(gradeFormat) ?? Constants.No_Value;
                                }
                            }
                            if (sidList != null)
                            {
                                foreach (string id in sidList)
                                {
                                    if (prevValues.Keys.Contains(id)) continue;
                                    var metricValue = MeasureUtility.GetSizingMeasureTechnologie(reportData.PreviousSnapshot, techno, int.Parse(id));
                                    prevValues[id] = metricValue?.ToString(valueFormat) ?? Constants.No_Value;
                                }
                            }
                            rowData.AddRange(prevValues.Values);
                            cntRow++;
                            technoPrevValues[techno] = prevValues;
                        }
                        break;
                }
            }

            #endregion

            #region Variation

            if (_snapshot == "BOTH")
            {
                for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                cntRow++;

                if (_variation == "VALUE" || _variation == "BOTH")
                {
                    rowData.Add(Labels.Evol);
                    Dictionary<string, string> variationValuesList = new Dictionary<string, string>();
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (_level)
                    {
                        case "APPLICATION":
                            if (qidList != null)
                            {
                                foreach (string id in qidList)
                                {
                                    if (variationValuesList.Keys.Contains(id)) continue;
                                    if (!currentValues.ContainsKey(id)) continue;
                                    double cur;
                                    if (double.TryParse(currentValues[id], out cur))
                                    {
                                        double prev;
                                        if (previousValues.ContainsKey(id) && double.TryParse(previousValues[id], out prev))
                                        {
                                            double? evol = cur - prev;
                                            variationValuesList[id] = evol?.ToString(gradeFormat) ?? Constants.No_Value;
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                    else
                                    {
                                        variationValuesList[id] = Constants.No_Value;
                                    }
                                }
                            }
                            if (sidList != null)
                            {
                                foreach (string id in sidList)
                                {
                                    if (variationValuesList.Keys.Contains(id)) continue;
                                    if (!currentValues.ContainsKey(id)) continue;
                                    double cur;
                                    if (double.TryParse(currentValues[id], out cur))
                                    {
                                        double prev;
                                        if (previousValues.ContainsKey(id) && double.TryParse(previousValues[id], out prev))
                                        {
                                            double? evol = cur - prev;
                                            variationValuesList[id] = evol?.ToString(valueFormat) ?? Constants.No_Value;
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                    else
                                    {
                                        variationValuesList[id] = Constants.No_Value;
                                    }
                                }
                            }
                            if (bidList != null)
                            {
                                foreach (string id in bidList)
                                {
                                    if (variationValuesList.Keys.Contains(id)) continue;
                                    if (!currentValues.ContainsKey(id)) continue;
                                    double cur;
                                    if (double.TryParse(currentValues[id], out cur))
                                    {
                                        double prev;
                                        if (previousValues.ContainsKey(id) && double.TryParse(previousValues[id], out prev))
                                        {
                                            double? evol = cur - prev;
                                            variationValuesList[id] = evol?.ToString(valueFormat) ?? Constants.No_Value;
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                    else
                                    {
                                        variationValuesList[id] = Constants.No_Value;
                                    }
                                }
                            }
                            rowData.AddRange(variationValuesList.Values);
                            cntRow++;
                            break;
                        case "MODULES":
                            for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                            foreach (Module module in curModules)
                            {
                                rowData.Add(module.Name);
                                var curModValues = modCurValues[module.Id];
                                var prevModValues = modPrevValues[module.Id];
                                if (qidList != null)
                                {
                                    foreach (string id in qidList)
                                    {
                                        if (variationValuesList.Keys.Contains(id)) continue;
                                        if (!curModValues.ContainsKey(id)) continue;
                                        double cur;
                                        if (double.TryParse(curModValues[id], out cur))
                                        {
                                            double prev;
                                            if (prevModValues.ContainsKey(id) && double.TryParse(prevModValues[id], out prev))
                                            {
                                                double? evol = cur - prev;
                                                variationValuesList[id] = evol?.ToString(gradeFormat) ?? Constants.No_Value;
                                            }
                                            else
                                            {
                                                variationValuesList[id] = Constants.No_Value;
                                            }
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                }
                                if (sidList != null)
                                {
                                    foreach (string id in sidList)
                                    {
                                        if (variationValuesList.Keys.Contains(id)) continue;
                                        if (!curModValues.ContainsKey(id)) continue;
                                        double cur;
                                        if (double.TryParse(curModValues[id], out cur))
                                        {
                                            double prev;
                                            if (prevModValues.ContainsKey(id) && double.TryParse(prevModValues[id], out prev))
                                            {
                                                double? evol = cur - prev;
                                                variationValuesList[id] = evol?.ToString(valueFormat) ?? Constants.No_Value;
                                            }
                                            else
                                            {
                                                variationValuesList[id] = Constants.No_Value;
                                            }
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                }
                                if (bidList != null)
                                {
                                    foreach (string id in bidList)
                                    {
                                        if (variationValuesList.Keys.Contains(id)) continue;
                                        if (!curModValues.ContainsKey(id)) continue;
                                        double cur;
                                        if (double.TryParse(curModValues[id], out cur))
                                        {
                                            double prev;
                                            if (prevModValues.ContainsKey(id) && double.TryParse(prevModValues[id], out prev))
                                            {
                                                double? evol = cur - prev;
                                                variationValuesList[id] = evol?.ToString(valueFormat) ?? Constants.No_Value;
                                            }
                                            else
                                            {
                                                variationValuesList[id] = Constants.No_Value;
                                            }
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                }
                                cntRow++;
                                rowData.AddRange(variationValuesList.Values);
                                variationValuesList.Clear();
                            }
                            break;
                        case "TECHNOLOGIES":
                            for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                            foreach (string techno in curTechnos)
                            {
                                rowData.Add(techno);
                                var curTechnoValues = technoCurValues[techno];
                                var prevTechnoValues = technoPrevValues[techno];
                                if (qidList != null)
                                {
                                    foreach (string id in qidList)
                                    {
                                        if (variationValuesList.Keys.Contains(id)) continue;
                                        if (!curTechnoValues.ContainsKey(id)) continue;
                                        double cur;
                                        if (double.TryParse(curTechnoValues[id], out cur))
                                        {
                                            double prev;
                                            if (prevTechnoValues.ContainsKey(id) && double.TryParse(prevTechnoValues[id], out prev))
                                            {
                                                double? evol = cur - prev;
                                                variationValuesList[id] = evol?.ToString(gradeFormat) ?? Constants.No_Value;
                                            }
                                            else
                                            {
                                                variationValuesList[id] = Constants.No_Value;
                                            }
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                }
                                if (sidList != null)
                                {
                                    foreach (string id in sidList)
                                    {
                                        if (variationValuesList.Keys.Contains(id)) continue;
                                        if (!curTechnoValues.ContainsKey(id)) continue;
                                        double cur;
                                        if (double.TryParse(curTechnoValues[id], out cur))
                                        {
                                            double prev;
                                            if (prevTechnoValues.ContainsKey(id) && double.TryParse(prevTechnoValues[id], out prev))
                                            {
                                                double? evol = cur - prev;
                                                variationValuesList[id] = evol?.ToString(valueFormat) ?? Constants.No_Value;
                                            }
                                            else
                                            {
                                                variationValuesList[id] = Constants.No_Value;
                                            }
                                        }
                                        else
                                        {
                                            variationValuesList[id] = Constants.No_Value;
                                        }
                                    }
                                }
                                cntRow++;
                                rowData.AddRange(variationValuesList.Values);
                                variationValuesList.Clear();
                            }
                            break;
                    }
                }

                if (_variation == "BOTH")
                {
                    for (int i = 0; i < cntMetric + 1; i++) rowData.Add(" ");
                    cntRow++;
                }

                if (_variation == "PERCENT" || _variation == "BOTH")
                {
                    rowData.Add(Labels.EvolPercent);
                    Dictionary<string, string> variationPercentList = new Dictionary<string, string>();
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (_level)
                    {
                        case "APPLICATION":
                            foreach (string id in currentValues.Keys)
                            {
                                double cur;
                                if (double.TryParse(currentValues[id], out cur))
                                {
                                    double prev;
                                    if (previousValues.ContainsKey(id) && double.TryParse(previousValues[id], out prev))
                                    {
                                        double? evol = Math.Abs(prev) > 0 ? (cur - prev) / prev : 1;
                                        variationPercentList[id] = evol.HasValue ? FormatPercent(evol) : Constants.No_Value;
                                    }
                                    else
                                    {
                                        variationPercentList[id] = Constants.No_Value;
                                    }
                                }
                                else
                                {
                                    variationPercentList[id] = Constants.No_Value;
                                }
                            }
                            rowData.AddRange(variationPercentList.Values);
                            cntRow++;
                            break;
                        case "MODULES":
                            for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                            foreach (Module module in curModules)
                            {
                                rowData.Add(module.Name);
                                var curModValues = modCurValues[module.Id];
                                var prevModValues = modPrevValues[module.Id];
                                foreach (var id in curModValues.Keys)
                                {
                                    double cur;
                                    if (double.TryParse(curModValues[id], out cur))
                                    {
                                        double prev;
                                        if (prevModValues.ContainsKey(id) && double.TryParse(prevModValues[id], out prev))
                                        {
                                            double? evol = Math.Abs(prev) > 0 ? (cur - prev) / prev : 1;
                                            variationPercentList[id] = evol.HasValue ? FormatPercent(evol) : Constants.No_Value;
                                        }
                                        else
                                        {
                                            variationPercentList[id] = Constants.No_Value;
                                        }
                                    }
                                    else
                                    {
                                        variationPercentList[id] = Constants.No_Value;
                                    }
                                }
                                cntRow++;
                                rowData.AddRange(variationPercentList.Values);
                                variationPercentList.Clear();
                            }
                            break;
                        case "TECHNOLOGIES":
                            for (int i = 0; i < cntMetric; i++) rowData.Add(" ");
                            foreach (string techno in curTechnos)
                            {
                                rowData.Add(techno);
                                var curTechnoValues = technoCurValues[techno];
                                var prevTechnoValues = technoPrevValues[techno];
                                foreach (var id in curTechnoValues.Keys)
                                {
                                    double cur;
                                    if (double.TryParse(curTechnoValues[id], out cur))
                                    {
                                        double prev;
                                        if (prevTechnoValues.ContainsKey(id) && double.TryParse(prevTechnoValues[id], out prev))
                                        {
                                            double? evol = Math.Abs(prev) > 0 ? (cur - prev) / prev : (Math.Abs(cur) > 0 ? cur : 0);
                                            variationPercentList[id] = evol.HasValue ? FormatPercent(evol) : Constants.No_Value;
                                        }
                                        else
                                        {
                                            variationPercentList[id] = Constants.No_Value;
                                        }
                                    }
                                    else
                                    {
                                        variationPercentList[id] = Constants.No_Value;
                                    }
                                }
                                cntRow++;
                                rowData.AddRange(variationPercentList.Values);
                                variationPercentList.Clear();
                            }
                            break;
                    }
                }
            }

            #endregion 


            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = cntRow + 1,
                NbColumns = cntMetric + 1,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
