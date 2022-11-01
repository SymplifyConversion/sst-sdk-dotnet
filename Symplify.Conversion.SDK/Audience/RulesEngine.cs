using System;
using System.Collections.Generic;
using System.Linq;

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
        public static dynamic Parse(dynamic ast, IList<string> primitives)
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
        public static dynamic CheckSyntax(dynamic ast, IList<string> primitives)
        {
            CheckSyntaxInner(ast, primitives);

            // Since there can be different reasons for the AST being invalid we use
            // exceptions instead of proper true | false branches. This lets us capture
            // messages in the caller.
            return true;
        }

        /// <summary>
        ///  The evaluation method for the rules.
        /// </summary>
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

            throw new InvalidOperationException(string.Format("message: Cannot evaluate {0}", ast));
        }

        /// <summary>
        /// Trace the evaluation of the given rules expression.
        /// The syntax tree is evaluated node for node, and function calls get
        /// annotated in-place with their result.
        /// This can be used for debugging or testing expressions in SST development.
        /// </summary>
        public static dynamic TraceEvaluate(dynamic ast, JObject environment, bool isTrace = true)
        {
            var returnTrace = new List<object> { };

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
                    { "result", value },
                });

                List<object> traceEval = new List<object>();
                foreach (dynamic arg in cdr)
                {
                    traceEval.Add(TraceEvaluate(arg, environment, isTrace));
                }

                return returnTrace.Concat(traceEval);
            }

            return Evaluate(ast, environment, isTrace);
        }

        private static dynamic EvalApply(string car, dynamic cdr, JObject environment, bool isTrace = false)
        {
            if (!Primitives.PrimitivesList.Contains(car))
            {
                throw new InvalidOperationException(FormattableString.Invariant($"message: {car} is not a primitive"));
            }

            List<dynamic> evaledArgs = new List<dynamic>();
            foreach (dynamic arg in cdr)
            {
                dynamic evaluateResult = Evaluate(arg, environment, isTrace);

                if (isTrace && evaluateResult is RulesEngineError)
                {
                    return evaluateResult;
                }

                evaledArgs.Add(evaluateResult);
            }

            return Primitives.PrimitiveFunction(car, evaledArgs, environment, isTrace);
        }

        private static void CheckSyntaxInner(dynamic ast, IList<string> primitives)
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
                    throw new InvalidOperationException(string.Format("message: can only apply strings, {0} is not a string", car));
                }

                if (!primitives.Contains(car))
                {
                    throw new InvalidOperationException(string.Format("message: {0} is not a primitive", car));
                }

                var cdr = ast.AfterSelf();

                foreach (var elem in cdr)
                {
                    CheckSyntaxInner(elem, primitives);
                }

                return;
            }

            throw new InvalidOperationException(string.Format("rules syntax error at {0}", ast));
        }
    }
}
