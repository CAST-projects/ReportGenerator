using System;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Globalization;
using Microsoft.CSharp;
using System.Reflection;
using Cast.Util.Log;

namespace Cast.Util
{
    public class ExpressionEvaluator
    {
        private ExpressionEvaluator()
        {
            // Avoid instanciation of the class
        }

        /// <summary>
        /// examples of parameters :
        /// parameters = "double a, double b, double c, double d, double e";
        /// expression = "a * b - (c + d) / e";
        /// values = new object[] { 1.32, 2, 3.65, 2.6, 10 };
        /// format="N2";
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="expression"></param>
        /// <param name="values"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Eval(string parameters, string expression, object[] values, string format)
        {

            const string source = @"
class MyType
{
    public static double Evaluate(<!parameters!>)
    {
        return <!expression!>;
    }
}
";

            string finalSource = source.Replace("<!parameters!>", parameters).Replace("<!expression!>", expression);

            CodeSnippetCompileUnit compileUnit = new CodeSnippetCompileUnit(finalSource);
            CodeDomProvider provider = new CSharpCodeProvider();

            CompilerParameters cparameters = new CompilerParameters();

            CompilerResults results = provider.CompileAssemblyFromDom(cparameters, compileUnit);

            Type type = results.CompiledAssembly.GetType("MyType");
            MethodInfo method = type.GetMethod("Evaluate");

            // The first parameter is the instance to invoke the method on. Because our Evaluate method is static, we pass null.
            string res;

            try
            {
                double? result = (double)method.Invoke(null, values);
                res = string.IsNullOrEmpty(format) ? result.Value.ToString(CultureInfo.CurrentCulture) : result.Value.ToString(format);
            }
            catch (ArgumentException ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                res = "Error in arguments";
            }
            catch (Exception ex)
            {
                res = ex.InnerException?.GetType().FullName == "System.DivideByZeroException" ? "Error divide by zero" : "Error in evaluation";
            }
            return res;
        }
    }
}
