using System.Text.Json;

namespace Bloxstrap.Integrations
{
    /// <summary>
    /// Keeps the last few launches (mode and time) next to Bloxstrap's own data, so the
    /// settings window can show when Roblox or Studio was last opened.
    /// </summary>
    public static class LaunchHistory
    {
        private const int MaxEntries = 20;

        private static string FilePath => Path.Combine(Paths.Base, "LaunchHistory.json");

        public record Entry(LaunchMode Mode, DateTime LaunchedAt);

        public static IReadOnlyList<Entry> Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return Array.Empty<Entry>();

                return JsonSerializer.Deserialize<List<Entry>>(File.ReadAllText(FilePath)) ?? new List<Entry>();
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("LaunchHistory::Load", ex);
                return Array.Empty<Entry>();
            }
        }

        public static void Record(LaunchMode mode)
        {
            var entries = Load().Prepend(new Entry(mode, DateTime.UtcNow)).Take(MaxEntries).ToList();

            try
            {
                File.WriteAllText(FilePath, JsonSerializer.Serialize(entries));
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("LaunchHistory::Record", ex);
            }
        }
    }
}
