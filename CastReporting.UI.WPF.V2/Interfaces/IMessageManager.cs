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

using CastReporting.BLL;
using System;
namespace CastReporting.UI.WPF
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsBusy {get;}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void OnErrorOccured(Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeSpan"></param>
        void OnReportGenerated(string fileName, TimeSpan timeSpan);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="state"></param>
        void OnServiceAdded(string message, StatesEnum state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        void OnServiceActivated(string url);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        void OnServiceRemoved(string url);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resultTest"></param>
        void OnServiceChecked(string url, bool resultTest);

        /// <summary>
        /// 
        /// </summary>
        void OnSettingsSaved();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBusy"></param>
        void SetBusyMode(bool isBusy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="message"></param>
        /// <param name="timeSpan"></param>
        void OnStepDone(double percentage, string message, TimeSpan timeSpan);
    }
}
