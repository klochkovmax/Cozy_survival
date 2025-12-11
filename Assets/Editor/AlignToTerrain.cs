using UnityEngine;
using UnityEditor;

public static class AlignToTerrain
{
    // Max vertical distance to search for terrain
    private const float MaxVerticalDistance = 100f;

    [MenuItem("Tools/Align Selected To Terrain %#t")] // Ctrl/Cmd + Shift + T
    private static void AlignSelectedToTerrain()
    {
        Transform[] transforms = Selection.transforms;

        if (transforms == null || transforms.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        int movedCount = 0;

        foreach (Transform t in transforms)
        {
            if (AlignTransformToTerrain(t))
                movedCount++;
        }

        if (movedCount > 0)
        {
            Debug.Log($"AlignToTerrain: moved {movedCount} object(s).");
            SceneView.RepaintAll();
        }
        else
        {
            Debug.Log("AlignToTerrain: no terrain within 100m for selected objects.");
        }
    }

    /// <summary>
    /// Tries to move transform along Y so that it intersects with a TerrainCollider,
    /// but only if the terrain is found within MaxVerticalDistance from current Y.
    /// </summary>
    private static bool AlignTransformToTerrain(Transform t)
    {
        Vector3 originalPos = t.position;

        // Start ray above the object so we can search both up and down in one cast
        Vector3 origin = originalPos + Vector3.up * MaxVerticalDistance;
        float rayLength = MaxVerticalDistance * 2f;

        // Raycast through everything and then pick the closest TerrainCollider hit
        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            Vector3.down,
            rayLength,
            ~0, // all layers
            QueryTriggerInteraction.Ignore
        );

        if (hits == null || hits.Length == 0)
            return false;

        bool foundTerrain = false;
        float closestDistance = float.PositiveInfinity;
        Vector3 bestPoint = originalPos;

        foreach (RaycastHit hit in hits)
        {
            // We only care about TerrainCollider
            if (!(hit.collider is TerrainCollider))
                continue;

            // Check vertical distance from original position
            float verticalDelta = Mathf.Abs(hit.point.y - originalPos.y);
            if (verticalDelta > MaxVerticalDistance)
                continue;

            // Keep the closest terrain point to the original position
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                bestPoint = new Vector3(originalPos.x, hit.point.y, originalPos.z);
                foundTerrain = true;
            }
        }

        if (!foundTerrain)
            return false;

        // Record for undo and move object
        Undo.RecordObject(t, "Align To Terrain");
        t.position = bestPoint;
        EditorUtility.SetDirty(t);

        return true;
    }
}
