using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

namespace Symplify.Conversion.SDK.Audience
{
    /// <summary>
    /// RulesEngine is the engine for validating the json.
    /// </summary>
    public static class RulesEngine
    {
        /// <summary>
        /// Checks that the given rules AST is valid.
        /// </summary>
        public static dynamic Parse(dynamic ast, List<string> primitives)
        {
            if (CheckSyntax(ast, primitives))
            {
                return ast;
            }

            return null;
        }

        /// <summary>
        ///  Check AST syntax, throws exception if invalid.
        /// </summary>
        public static dynamic CheckSyntax(dynamic ast, List<string> primitives)
        {
            CheckSyntaxInner(ast, primitives);

            // Since there can be different reasons for the AST being invalid we use
            // exceptions instead of proper true | false branches. This lets us capture
            // messages in the caller.
            return true;
        }

        public static dynamic Evaluate(dynamic ast, JObject environment, bool isTrace = false)
        {
            if (ast is JValue)
            {
                ast = ast.Value;
            }

            if (ast is int)
            {
                return ast;
            }

            if (ast is long)
            {
                return ast;
            }

            if (ast is string)
            {
                return ast;
            }

            if (ast is bool)
            {
                return ast;
            }

            if (ast is JArray)
            {
                dynamic car = ast[0];
                if (car is JValue)
                {
                    car = car.Value;
                }

                if (car is string)
                {
                    JArray cdr = new JArray();
                    for (int i = 0; i < ast.Count; i++)
                    {
                        if (i != 0)
                        {
                            cdr.Add(ast[i]);
                        }
                    }

                    return EvalApply(car, cdr, environment, isTrace);
                }
            }

            throw new Exception(string.Format("message: Cannot evaluate {0}", ast));
        }

        private static void CheckSyntaxInner(dynamic ast, List<string> primitives)
        {
            if (ast is int)
            {
                return;
            }

            if (ast is long)
            {
                return;
            }

            if (ast is string)
            {
                return;
            }

            if (ast is bool)
            {
                return;
            }

            if (ast is JArray)
            {
                var car = ast[0].Value;
                if (car.GetType() != typeof(string))
                {
                    throw new Exception(string.Format("message: can only apply strings, {0} is not a string", car));
                }

                if (!primitives.Contains(car))
                {
                    throw new Exception(string.Format("message: {0} is not a primitive", car));
                }

                var cdr = ast.AfterSelf();

                foreach (var elem in cdr)
                {
                    CheckSyntaxInner(elem, primitives);
                }

                return;
            }

            throw new Exception(string.Format("rules syntax error at {0}", ast));
        }

        public static dynamic EvalApply(string car, dynamic cdr, JObject environment, bool isTrace = false)
        {
            if (!Primitives.PrimativesList.Contains(car))
            {
                throw new Exception(string.Format("message: {0} is not a primitive", car));
            }

            List<dynamic> evaledArgs = new List<dynamic>();
            foreach (dynamic arg in cdr)
            {
                dynamic evaluateResult = Evaluate(arg, environment, isTrace);

                if (isTrace == true && evaluateResult is RulesEngineError)
                {
                    return evaluateResult;
                }

                evaledArgs.Add(evaluateResult);
            }

            return Primitives.PrimitiveFunction(car, evaledArgs, environment, isTrace);
        }

        public static dynamic TraceEvaluate(dynamic ast, JObject environment, bool isTrace = true)
        {
            var returnTrace = new List<Object> {};

            if (ast is JArray && ast[0].Value is string)
            {
                string car = ast[0].Value;

                JArray cdr = new JArray();
                for (int i = 0; i < ast.Count; i++)
                {
                    if (i != 0)
                    {
                        cdr.Add(ast[i]);
                    }
                }

                dynamic value = EvalApply(car, cdr, environment, isTrace);

                returnTrace.Add(new Dictionary<string, dynamic>
                {
                    { "call", car },
                    { "result", value }
                });

                List<Object> traceEval = new List<Object>();
                foreach (dynamic arg in cdr)
                {
                    traceEval.Add(TraceEvaluate(arg, environment, isTrace));
                }

                return returnTrace.Concat(traceEval);
            }

            return Evaluate(ast, environment, isTrace);
        }
    }
}
