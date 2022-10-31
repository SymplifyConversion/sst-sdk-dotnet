using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symplify.Conversion.SDK.Audience
{
    /**
 * Evaluating rules can yield a runtime error.
 */
    public class RulesEngineError
    {
        [JsonPropertyName("message")]
        public string message;

        public RulesEngineError(string message)
        {
            this.message = message;
        }

        public override string ToString()
        {
            return this.message;
        }
    }

    /// <summary>
    /// SymplifyAudience contains rules to be evaluated for activating projects.
    /// </summary>
    public class SymplifyAudience
    {
        /// <summary>
        /// All the rules.
        /// </summary>
        private readonly JArray rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyAudience"/> class.
        /// Constructor.
        /// </summary>
        public SymplifyAudience(dynamic rules)
        {
            if (rules is JValue)
            {
                try
                {
                    rules = JsonConvert.DeserializeObject(rules.Value);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("rules syntax error at {0}", rules));
                }

                if (!(rules is JArray))
                {
                    throw new Exception("AST root must be a list");
                }
            }

            RulesEngine.Parse(rules, Primitives.PrimativesList);

            this.rules = rules;
        }

        public JArray Rules
        {
            get { return rules; }
        }

        public dynamic Eval(dynamic environment)
        {
            dynamic result;
            try
            {
                result = RulesEngine.Evaluate(this.Rules, environment);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            if (!(result is bool))
            {
                return new RulesEngineError(string.Format("audience result was not boolean ({0})", result));
            }

            return result;
        }

        /// <summary>
        /// trace interprets the rules in the given environment, and annotates each
        /// sub-expression with their partial value.
        /// </summary>
        public dynamic Trace(dynamic environment)
        {
            // Since the constructor can't return an error message we must have this
            // errorMessage checker and return the error message.
            dynamic result;
            try
            {
                result = RulesEngine.TraceEvaluate(this.Rules, environment);
            }
            catch (Exception e){
                return e.Message;
            }

            return result;
        }
    }
}
