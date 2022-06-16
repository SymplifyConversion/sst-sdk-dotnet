using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    /// <summary>
    /// ProjectState identifies the state of a (project or variation), should it be used or not.
    /// </summary>
    [JsonConverter(typeof(ProjectStateStringConverter))]
    public enum ProjectState
    {
        /// <summary>
        /// Paused means a project or variation is not currently to be allocated.
        /// </summary>
        Paused,

        /// <summary>
        /// Active means a project or variation should be used when allocating.
        /// </summary>
        Active,
    }

    /// <summary>
    /// ProjectStateStringConverter is a helper to convert the enum to and from JSON.
    /// </summary>
    public class ProjectStateStringConverter : JsonConverter<ProjectState>
    {
        /// <summary>
        /// See <see cref="JsonConverter"/>.
        /// </summary>
        public override ProjectState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.GetString().ToLower(CultureInfo.InvariantCulture) == "active")
            {
                return ProjectState.Active;
            }

            // fall back on Paused for unknown states, it's the safest option
            return ProjectState.Paused;
        }

        /// <summary>
        /// See <see cref="JsonConverter"/>.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, ProjectState value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case ProjectState.Active:
                    writer.WriteStringValue("active");
                    break;
                default:
                    writer.WriteStringValue("paused");
                    break;
            }
        }
    }
}
