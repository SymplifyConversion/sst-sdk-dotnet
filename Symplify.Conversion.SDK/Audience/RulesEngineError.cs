using System.Text.Json.Serialization;

namespace Symplify.Conversion.SDK.Audience
{
    /// <summary>
    /// RulesEngineError custom error.
    /// </summary>
    public class RulesEngineError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RulesEngineError"/> class.
        /// </summary>
        public RulesEngineError(string message)
        {
            this.message = message;
        }

#pragma warning disable SA1300, IDE1006
        /// <summary>
        /// Gets the message property.
        /// </summary>
        [JsonPropertyName("message")]
        public string message { get; }
#pragma warning disable SA1300, IDE1006

        /// <summary>
        /// The to string of RuleEngineError.
        /// </summary>
        public override string ToString()
        {
            return message;
        }
    }
}