using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class ComputingTest
    {
        [TestMethod]
        [DeploymentItem(@".\Data\ComputingTest1.json", "Data")]
        public void GetDCGradesTest()
        {

            Snapshot selectedSnapshot = new Snapshot
            {
                Name = "Test",
                Href = "AED1/applications/3/snapshots/3",
                Annotation = new Annotation() {Version = "2.1"}
            };

            var jsonString = File.ReadAllText(@".\Data\ComputingTest1.json");
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<ApplicationResult>));
            selectedSnapshot.BusinessCriteriaResults = serializer.ReadObject(ms) as IEnumerable<ApplicationResult>;

            // ReSharper disable once UnusedVariable
            var result = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(selectedSnapshot, Constants.BusinessCriteria.TechnicalQualityIndex, true);

            Debug.Assert(result != null, "result != null");
            Assert.AreEqual(3.4, result.Value);
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleFile"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private IEnumerable<Result> GetSampleResult(string sampleFile)
        {
            var jsonString = File.ReadAllText(sampleFile);

            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Result>));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));


            return serializer.ReadObject(ms) as IEnumerable<Result>;
        }
    }
}
