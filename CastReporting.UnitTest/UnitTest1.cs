using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CSharp;
using System.Reflection;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            
            double pValue = 1.1401;
            string sign = (pValue > 0) ? "+" : "";
            var roundedValue = Math.Round(pValue, 4);
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            var tmp = roundedValue * 100;
            nfi.PercentDecimalDigits = (tmp % 1 == 0 || tmp >= 100) ? 0 : (tmp >= 0.1) ? 2 : 1;
            var r =  sign + roundedValue.ToString("P", nfi);
            

            /*
            string source = @"
class MyType
{
    public static double Evaluate(<!parameters!>)
    {
        return <!expression!>;
    }
}
";

            string parameters = "double a, double b, double c, double d, double e";
            string expression = "a * b - (c + d) / e";

            string finalSource = source.Replace("<!parameters!>", parameters).Replace("<!expression!>", expression);

            CodeSnippetCompileUnit compileUnit = new CodeSnippetCompileUnit(finalSource);
            CodeDomProvider provider = new CSharpCodeProvider();

            CompilerParameters cparameters = new CompilerParameters();

            CompilerResults results = provider.CompileAssemblyFromDom(cparameters, compileUnit);

            Type type = results.CompiledAssembly.GetType("MyType");
            MethodInfo method = type.GetMethod("Evaluate");

            // The first parameter is the instance to invoke the method on. Because our Evaluate method is static, we pass null.
            double? result = (double)method.Invoke(null, new object[] { 1.32, 2, 3.65, 2.6, 10 });
            Console.WriteLine(result.Value.ToString("N0"));
            
    */
        }
    }
}
