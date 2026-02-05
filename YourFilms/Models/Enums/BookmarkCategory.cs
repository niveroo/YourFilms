using System.Text.Json.Serialization;

namespace YourFilms.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookmarkCategory
    {
        Watchlist,
        Watching,
        Watched,
        Dropped
    }
}
