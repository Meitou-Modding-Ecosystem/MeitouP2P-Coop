using System.Text.Json;
using Meitou.Content;
using Meitou.Data;
using Meitou.Data.Fcs;
using Meitou.Modding;

namespace MeitouP2P.Coop;

internal static class RoleLabels
{
    private sealed record Pack(int SchemaVersion, string PackId, Role[] Roles);
    private sealed record Role(string Id, string Label, string[] Characters);
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static Dictionary<string,string> Load(ModContext context)
    {
        var labels = new Dictionary<string,string>(StringComparer.Ordinal);
        if (GameInstall.Locate() is not { } install) return labels;
        var mods = ModList.FindMods(install);
        foreach (string file in context.Data.Files)
        {
            if (!mods.TryGetValue(Path.GetFileNameWithoutExtension(file), out var path)) continue;
            string content = Path.Combine(Path.GetDirectoryName(path)!, "coop-roles.json");
            if (!File.Exists(content)) continue;
            try
            {
                using var stream = File.OpenRead(content);
                var pack = JsonSerializer.Deserialize<Pack>(stream, Json);
                if (pack is not { SchemaVersion: 1, Roles: not null } || string.IsNullOrWhiteSpace(pack.PackId))
                    throw new InvalidDataException("Expected a version-1 role pack.");
                var additions = new Dictionary<string,string>(StringComparer.Ordinal);
                foreach (var role in pack.Roles)
                {
                    if (role is null || string.IsNullOrWhiteSpace(role.Id) || string.IsNullOrWhiteSpace(role.Label) ||
                        role.Label.Length > 80 || role.Characters is not { Length: > 0 })
                        throw new InvalidDataException("Invalid role or label (maximum 80 characters).");
                    foreach (string id in role.Characters)
                        if (string.IsNullOrEmpty(id) || context.Data.Find(id)?.Type != FcsRecordType.Character || !additions.TryAdd(id, role.Label))
                            throw new InvalidDataException($"Unknown or duplicate character template '{id}'.");
                }
                foreach (var entry in additions) labels[entry.Key] = entry.Value;
                context.Log($"{file} applied {additions.Count} co-op role label(s).");
            }
            catch (Exception e) when (e is IOException or JsonException or InvalidDataException or UnauthorizedAccessException)
            {
                context.LogError($"Skipped {file}/coop-roles.json ({e.Message})");
            }
        }
        return labels;
    }
}
