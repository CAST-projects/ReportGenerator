using Cast.Util;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CastReporting.UnitTest
{
    /// <summary>
    /// Summary description for ExpressionEvaluatorTests
    /// </summary>
    [TestClass]
    public class ExpressionEvaluatorTests
    {
        public ExpressionEvaluatorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestEval()
        {
            string result = string.Empty;
            
            result = ExpressionEvaluator.Eval("double a, double b, double c, double d, double e", "a * b - (c + d) / e", new object[] { 1.32, 2, 3.65, 2.6, 10 }, "N2");
            Assert.AreEqual("2.02", result);
            
            result = ExpressionEvaluator.Eval("double a, double b, double c, double d, double e", "a * b - (c + d) / e", new object[] { 1.32, 2, 3.65, 2.6, 10 }, "N0");
            Assert.AreEqual("2", result);
            
            result = ExpressionEvaluator.Eval("int a, int b", "a / b", new object[] { 36500000, 5100 }, "N0");
            Assert.AreEqual("7,156", result);
            
            result = ExpressionEvaluator.Eval("int a, int b", "a / b", new object[] { 36500000, 0 }, "N0");
            Assert.AreEqual("Error divide by zero", result);
            
            result = ExpressionEvaluator.Eval("int a, int b", "a - b", new object[] { 2700, 3000 }, "N0");
            Assert.AreEqual("-300", result);

            result = ExpressionEvaluator.Eval("double a, double b", "(a + b)/2", new object[] { 2.64, 3.12 }, "N2");
            Assert.AreEqual("2.88", result);

            result = ExpressionEvaluator.Eval("int a, int b", "a * b", new object[] { 2, 3 }, "N0");
            Assert.AreEqual("6", result);

            
        }
    }
}
