using System.Collections.Generic;
using System.IO;
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
        public void GetDCGradesTest()
        {

            Snapshot selectedSnapshot = new Snapshot
            {
                Name = "Test",
                Href = "AED1/applications/3/snapshots/3",
                Annotation = new Annotation() {Version = "2.1"}
            };
            // selectedSnapshot.BusinessCriteriaResults = GetSampleResult(@"Data\JSonTest3.txt").SelectMany(_ => _.ApplicationResults);

            // ReSharper disable once UnusedVariable
            var result = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(selectedSnapshot, true);
            
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
