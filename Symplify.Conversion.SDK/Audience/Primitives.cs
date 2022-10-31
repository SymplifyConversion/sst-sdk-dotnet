using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

namespace Symplify.Conversion.SDK.Audience
{
    /// <summary>
    /// This class contains the logic for evaluating each of the primitives in audience rules.
    /// </summary>
    public static class Primitives
    {
        private static readonly ReadOnlyCollection<string> PrivatePrimitivesList = new ReadOnlyCollection<string>(
            new List<string>
        {
        "not",
        "all",
        "any",
        "equals",
        "contains",
        "matches",
        "==",
        "!=",
        "<",
        "<=",
        ">",
        ">=",
        "number-attribute",
        "string-attribute",
        "bool-attribute",
        });

        /// <summary>
        /// Gets the list of all the accepted primitives.
        /// </summary>
        public static ReadOnlyCollection<string> PrimitivesList
        {
            get { return PrivatePrimitivesList; }
        }

        /// <summary>
        /// Validation functions for the primitives.
        /// </summary>
        public static dynamic PrimitiveFunction(string primitive, List<dynamic> args, JObject environment, bool isTrace = false)
        {
            if (!PrimitivesList.Contains(primitive))
            {
                throw new InvalidOperationException(FormattableString.Invariant($"Primitive {primitive} is not a valid primitive. Available primitives are: {PrimitivesList}"));
            }

            switch (primitive)
            {
                case "not":
                    if (args[0] is bool)
                    {
                        return !args[0];
                    }

                    return IsError(string.Format("{0} is not a boolean", args[0]), isTrace);

                case "any":
                    foreach (dynamic arg in args)
                    {
                        if (!(arg is bool))
                        {
                            return IsError(string.Format("{0} is not a boolean", arg), isTrace);
                        }

                        if (arg)
                        {
                            return true;
                        }
                    }

                    return false;

                case "all":
                    foreach (dynamic arg in args)
                    {
                        if (!(arg is bool))
                        {
                            return IsError(string.Format("{0} is not a boolean", arg), isTrace);
                        }

                        if (!arg)
                        {
                            return false;
                        }
                    }

                    return true;

                case "equals":
                    Func<string, string, dynamic> equals = (a, b) => { return a == b; };
                    return StringFun(args[0], args[1], equals);

                case "contains":
#pragma warning disable CA1310
                    Func<string, string, dynamic> contains = (a, b) => { return a.IndexOf(b) != -1; };
#pragma warning disable CA1310
                    return StringFun(args[0], args[1], contains);

                case "matches":
                    Func<string, string, dynamic> matches = (a, b) =>
                    {
                        Match m = Regex.Match(a, b);
                        return m.Success;
                    };
                    return StringFun(args[0], args[1], matches, isTrace);

                case "==":
                    Func<float, float, dynamic> equalFloat = (a, b) => { return a == b; };
                    return NumberFun(args[0], args[1], equalFloat, isTrace);

                case "!=":
                    Func<float, float, dynamic> notEqual = (a, b) => { return a != b; };
                    return NumberFun(args[0], args[1], notEqual, isTrace);

                case "<":
                    Func<float, float, dynamic> lessThan = (a, b) => { return a < b; };
                    return NumberFun(args[0], args[1], lessThan, isTrace);

                case "<=":
                    Func<float, float, dynamic> lessOrEqualThan = (a, b) => { return a <= b; };
                    return NumberFun(args[0], args[1], lessOrEqualThan, isTrace);

                case ">":
                    Func<float, float, dynamic> greaterThan = (a, b) => { return a > b; };
                    return NumberFun(args[0], args[1], greaterThan, isTrace);

                case ">=":
                    Func<float, float, dynamic> greaterOrEqualThan = (a, b) => { return a >= b; };
                    return NumberFun(args[0], args[1], greaterOrEqualThan, isTrace);

                case "number-attribute":
                    return GetInEnvNumber(args[0], environment, isTrace);

                case "string-attribute":
                    return GetInEnvString(args[0], environment, isTrace);

                case "bool-attribute":
                    return GetInEnvBool(args[0], environment, isTrace);

                default:
                    throw new InvalidOperationException(FormattableString.Invariant($"Primitive '{primitive}' is not an implemented primitive."));
            }
        }

        /// <summary>
        /// Check if the value is a number.
        /// </summary>
        public static bool IsNumeric(dynamic value)
        {
            if (value is long || value is float || value is int)
            {
                return true;
            }

            return false;
        }

        private static dynamic StringFun(dynamic a, dynamic b, Func<string, string, dynamic> method, bool isTrace = false)
        {
            if (!(a is string) || !(b is string))
            {
                return IsError("expected string arguments", isTrace);
            }

            return method(a, b);
        }

        private static dynamic NumberFun(dynamic a, dynamic b, Func<float, float, dynamic> method, bool isTrace = false)
        {
            if (!IsNumeric(a) || !IsNumeric(b))
            {
                return IsError("expected number arguments", isTrace);
            }

            return method(a, b);
        }

        private static dynamic GetInEnvNumber(dynamic name, JObject environment, bool isTrace = false)
        {
            if (!(name is string))
            {
                return IsError("can only look up string names", isTrace);
            }

            JToken elem = environment[name];
            if (
                elem == null || !IsNumeric(environment[name].Value))
            {
                return IsError(string.Format("'{0}' is not a number", name), isTrace);
            }

            return environment[name].Value;
        }

        private static dynamic GetInEnvString(dynamic name, JObject environment, bool isTrace = false)
        {
            if (!(name is string))
            {
                return IsError("can only look up string names", isTrace);
            }

            JToken elem = environment[name];
            if (elem == null || !(environment[name].Value is string))
            {
                return IsError(string.Format("'{0}' is not a string", name), isTrace);
            }

            return environment[name].Value;
        }

        private static dynamic GetInEnvBool(dynamic name, JObject environment, bool isTrace = false)
        {
            if (!(name is string))
            {
                return IsError("can only look up string names", isTrace);
            }

            JToken elem = environment[name];
            if (elem == null || !(environment[name].Value is bool))
            {
                return IsError(string.Format("'{0}' is not a boolean", name), isTrace);
            }

            return environment[name].Value;
        }

        private static RulesEngineError IsError(string message, bool isTrace = false)
        {
            if (isTrace)
            {
                return new RulesEngineError(message);
            }

            throw new InvalidOperationException(message);
        }
    }
}
