using System;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CSharp;
using System.Reflection;

namespace Cast.Util
{
    public class ExpressionEvaluator
    {
        public static string Eval(string parameters, string expression, object[] values, string format)
        {
            /*
            parameters = "double a, double b, double c, double d, double e";
            expression = "a * b - (c + d) / e";
            values = new object[] { 1.32, 2, 3.65, 2.6, 10 };
            format="N2";
            */

            string source = @"
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
            double? result = null;
            string res = string.Empty;

            try
            {
                result = (double)method.Invoke(null, values);
                res = result.Value.ToString(format);
            }
            catch (ArgumentException ex)
            {
                res = "Error in arguments";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.GetType().FullName == "System.DivideByZeroException" )
                {
                    res = "Error divide by zero";
                }
                else
                {
                    res = "Error in evaluation";
                }
            }
            return res;
        }
    }
}
