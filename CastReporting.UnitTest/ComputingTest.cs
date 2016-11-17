using System.Collections.Generic;
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
        public void GetDCGradesTest()
        {
         
            Snapshot selectedSnapshot = new Snapshot();
            selectedSnapshot.Name = "Test";
            selectedSnapshot.Href = "AED1/applications/3/snapshots/3";
            selectedSnapshot.Annotation = new Annotation() { Version = "2.1" };
           // selectedSnapshot.BusinessCriteriaResults = GetSampleResult(@"Data\JSonTest3.txt").SelectMany(_ => _.ApplicationResults);

            var result = BusinessCriteriaUtility.GetBusinessCriteriaGradesModules(selectedSnapshot, true);
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleFile"></param>
        /// <returns></returns>
        private IEnumerable<Result> GetSampleResult(string sampleFile)
        {
            var jsonString = File.ReadAllText(sampleFile);

            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Result>));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));


            return serializer.ReadObject(ms) as IEnumerable<Result>;
        }
    }
}
