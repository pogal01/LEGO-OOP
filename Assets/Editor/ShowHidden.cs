using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor-only script: keep this file inside a folder named "Editor" (for example Assets/Editor/ShowHidden.cs).
public static class ShowHidden
{
    const string k_menu = "I wish to see all objects/Show Hidden Objects";

    // Remembers the original flags of everything we un-hid, so it can be hidden again.
    // Keys are GameObjects AND Transforms: the LEGO package hides bricks by setting the flag on the
    // Transform component, not on the GameObject.
    // Note: cleared on script recompile; objects then stay visible until the scene is reloaded.
    static readonly Dictionary<Object, HideFlags> s_OriginalFlags = new Dictionary<Object, HideFlags>();

    static bool s_Showing;

    [MenuItem(k_menu)]
    static void ToggleShowHidden()
    {
        s_Showing = !s_Showing;

        if (s_Showing)
        {
            s_OriginalFlags.Clear();
            int count = 0;

            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                GameObject go = t.gameObject;

                // Only objects in loaded scenes (skips prefab assets and Unity's internal objects).
                if (!go.scene.IsValid())
                    continue;

                if ((t.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    s_OriginalFlags[t] = t.hideFlags;
                    t.hideFlags &= ~HideFlags.HideInHierarchy;
                    count++;
                }

                if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    s_OriginalFlags[go] = go.hideFlags;
                    go.hideFlags &= ~HideFlags.HideInHierarchy;
                    count++;
                }
            }

            Debug.Log("[ShowHidden] Un-hid " + count + " object(s).");
        }
        else
        {
            foreach (KeyValuePair<Object, HideFlags> entry in s_OriginalFlags)
            {
                if (entry.Key != null)
                    entry.Key.hideFlags = entry.Value;
            }

            Debug.Log("[ShowHidden] Re-hid " + s_OriginalFlags.Count + " object(s).");
            s_OriginalFlags.Clear();
        }

        EditorApplication.DirtyHierarchyWindowSorting();
        EditorApplication.RepaintHierarchyWindow();
    }

    [MenuItem(k_menu, true)]
    static bool ToggleShowHiddenValidate()
    {
        Menu.SetChecked(k_menu, s_Showing);
        return true;
    }
}