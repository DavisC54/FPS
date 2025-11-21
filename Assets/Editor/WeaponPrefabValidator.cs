using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.FPS.Game;

public class WeaponPrefabValidator : EditorWindow
{
    [MenuItem("Tools/Weapon Prefab Validator/Validate & Fix WeaponRoot")]
    public static void ValidateAndFixWeaponRoots()
    {
        // Find all prefabs in the project
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        int checkedCount = 0;
        int fixedCount = 0;
        int warnCount = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefabContents = null;
            try
            {
                prefabContents = PrefabUtility.LoadPrefabContents(path);
                if (prefabContents == null)
                    continue;

                WeaponController wc = prefabContents.GetComponentInChildren<WeaponController>(true);
                if (wc != null)
                {
                    checkedCount++;

                    if (wc.WeaponRoot == null)
                    {
                        // Try common patterns: child named "WeaponRoot" or first child with a Renderer
                        Transform candidate = prefabContents.transform.Find("WeaponRoot");
                        if (candidate == null)
                        {
                            candidate = FindFirstRenderable(prefabContents.transform);
                        }

                        if (candidate != null)
                        {
                            wc.WeaponRoot = candidate.gameObject;
                            PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
                            fixedCount++;
                            Debug.Log($"[WeaponPrefabValidator] Assigned WeaponRoot for prefab '{path}' -> '{GetTransformPath(candidate)}'");
                        }
                        else
                        {
                            warnCount++;
                            Debug.LogWarning($"[WeaponPrefabValidator] No suitable WeaponRoot found for prefab '{path}'. Please assign WeaponRoot manually.");
                        }
                    }
                }
            }
            finally
            {
                if (prefabContents != null)
                    PrefabUtility.UnloadPrefabContents(prefabContents);
            }
        }

        EditorUtility.DisplayDialog("Weapon Prefab Validator",
            $"Checked weapon prefabs: {checkedCount}\nAuto-fixed: {fixedCount}\nWarnings: {warnCount}", "OK");
    }

    [MenuItem("Tools/Weapon Prefab Validator/List Problematic WeaponPrefabs")]
    public static void ListProblematicWeaponPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        var problematic = new List<string>();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefabContents = null;
            try
            {
                prefabContents = PrefabUtility.LoadPrefabContents(path);
                if (prefabContents == null)
                    continue;

                WeaponController wc = prefabContents.GetComponentInChildren<WeaponController>(true);
                if (wc != null && wc.WeaponRoot != null)
                {
                    // Check if WeaponRoot or its children have any renderers
                    var rends = wc.WeaponRoot.GetComponentsInChildren<Renderer>(true);
                    if (rends == null || rends.Length == 0)
                    {
                        problematic.Add(path);
                    }
                }
            }
            finally
            {
                if (prefabContents != null)
                    PrefabUtility.UnloadPrefabContents(prefabContents);
            }
        }

        if (problematic.Count == 0)
            Debug.Log("[WeaponPrefabValidator] No problematic weapon prefabs found.");
        else
        {
            Debug.LogWarning($"[WeaponPrefabValidator] Found {problematic.Count} problematic weapon prefabs. See console for list.");
            foreach (var p in problematic)
                Debug.LogWarning(p);
        }
    }

    static Transform FindFirstRenderable(Transform root)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.GetComponent<Renderer>() != null)
                return t;
        }

        return null;
    }

    static string GetTransformPath(Transform t)
    {
        var names = new List<string>();
        Transform cur = t;
        while (cur != null)
        {
            names.Add(cur.name);
            cur = cur.parent;
        }
        names.Reverse();
        return string.Join("/", names.ToArray());
    }
}
