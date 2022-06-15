using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    [JsonConverter(typeof(ProjectStateStringConverter))]
    public enum ProjectState
    {
        Paused,
        Active,
    }

    public class ProjectStateStringConverter : JsonConverter<ProjectState>
    {
        public override ProjectState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.GetString().ToLower(CultureInfo.InvariantCulture) == "active")
            {
                return ProjectState.Active;
            }

            // fall back on Paused for unknown states, it's the safest option
            return ProjectState.Paused;
        }

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
