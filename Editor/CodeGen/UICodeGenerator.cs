namespace HannibalUI.Editor.CodeGen
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The single Generate entry point: validates a <see cref="UIProjectConfig"/>, writes the
    /// generator-owned files (enums + director), writes screen stubs only when absent, removes the
    /// legacy hand-written enums the generator now owns, and refreshes the AssetDatabase.
    /// </summary>
    public readonly struct GenerationResult
    {
        public readonly bool Success;
        public readonly int Created;
        public readonly int Overwritten;
        public readonly int Skipped;
        public readonly IReadOnlyList<string> Errors;

        public GenerationResult(bool success, int created, int overwritten, int skipped, IReadOnlyList<string> errors)
        {
            Success = success;
            Created = created;
            Overwritten = overwritten;
            Skipped = skipped;
            Errors = errors;
        }
    }

    public static class UICodeGenerator
    {
        private const string GeneratedFileExtension = ".g.cs";

        // Legacy hand-written enums the generator supersedes (deleted on first successful generate).
        private static readonly string[] LegacyEnumAssets =
        {
            "Assets/HannibalUI/Runtime/Base/CanvasTypeContainer.cs",
            "Assets/HannibalUI/Runtime/Base/VP_UIEvents.cs"
        };

        [MenuItem("HannibalUI/Generate From Selected Config")]
        private static void GenerateFromSelection()
        {
            var config = Selection.activeObject as UIProjectConfig;

            if (config == null)
            {
                Debug.LogError("HannibalUI: select a UIProjectConfig asset, then run Generate.");
                return;
            }

            Generate(config);
        }

        public static GenerationResult Generate(UIProjectConfig config)
        {
            var errors = ConfigValidator.Validate(config);

            if (errors.Count > 0)
            {
                Debug.LogError("HannibalUI generation aborted:\n - " + string.Join("\n - ", errors));
                return new GenerationResult(false, 0, 0, 0, errors);
            }

            int created = 0;
            int overwritten = 0;
            int skipped = 0;

            EnsureFolder(config.GeneratedFolder);
            WriteOwned(GeneratedPath(config, "CanvasType"), EnumGenerator.GenerateCanvasType(config), ref created, ref overwritten);
            WriteOwned(GeneratedPath(config, "UIEvents"), EnumGenerator.GenerateUIEvents(config), ref created, ref overwritten);
            WriteOwned(GeneratedPath(config, "VP_GeneratedDirector"), RouteGenerator.GenerateDirector(config), ref created, ref overwritten);

            EnsureFolder(config.ScreensFolder);

            foreach (var screen in config.Screens)
            {
                var path = Path.Combine(config.ScreensFolder, ScreenStubGenerator.FileName(screen));

                if (File.Exists(path))
                {
                    skipped++;
                    continue;
                }

                File.WriteAllText(path, ScreenStubGenerator.Generate(screen));
                created++;
            }

            foreach (var legacy in LegacyEnumAssets)
            {
                if (File.Exists(legacy))
                {
                    AssetDatabase.DeleteAsset(legacy);
                }
            }

            AssetDatabase.Refresh();

            Debug.Log($"HannibalUI generation complete: {created} created, {overwritten} overwritten, " +
                      $"{skipped} stub(s) skipped (already present).");

            return new GenerationResult(true, created, overwritten, skipped, Array.Empty<string>());
        }

        private static string GeneratedPath(UIProjectConfig config, string typeName)
        {
            return Path.Combine(config.GeneratedFolder, typeName + GeneratedFileExtension);
        }

        private static void WriteOwned(string path, string content, ref int created, ref int overwritten)
        {
            bool existed = File.Exists(path);
            File.WriteAllText(path, content);

            if (existed)
            {
                overwritten++;
            }
            else
            {
                created++;
            }
        }

        private static void EnsureFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
