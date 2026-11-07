// Builds the thin-wall Present-scene maze inside Unity (no Blender needed).
// Put this file in a folder called "Editor" inside Assets.
// Then use the menu: Tools > Build Thin Maze
//
// Layout (top view, dorm is on the right, N = nurse, no exit):
// +--+--+--+--+
// |           |
// +  +--+  +--+
// |     |      <- in from dorm
// +--+  +--+--+
// |  |  | N   |
// +  +  +  +  +
// |        |  |
// +--+--+--+--+

using UnityEngine;
using UnityEditor;

public class ThinMazeBuilder : EditorWindow
{
    float cell = 1.2f;      // path width (m)
    float wallT = 0.05f;    // wall thickness (m)
    float wallH = 2.4f;     // wall height (m)
    bool addCeiling = true;
    Material wallMat;
    Material floorMat;

    [MenuItem("Tools/Build Thin Maze")]
    static void Open()
    {
        GetWindow<ThinMazeBuilder>("Thin Maze");
    }

    void OnGUI()
    {
        cell = EditorGUILayout.FloatField("Path width (m)", cell);
        wallT = EditorGUILayout.FloatField("Wall thickness (m)", wallT);
        wallH = EditorGUILayout.FloatField("Wall height (m)", wallH);
        addCeiling = EditorGUILayout.Toggle("Add ceiling", addCeiling);
        wallMat = (Material)EditorGUILayout.ObjectField("Wall material", wallMat, typeof(Material), false);
        floorMat = (Material)EditorGUILayout.ObjectField("Floor material", floorMat, typeof(Material), false);

        GUILayout.Space(10);
        if (GUILayout.Button("Build", GUILayout.Height(30)))
        {
            Build();
        }
    }

    void Build()
    {
        const int ROWS = 4, COLS = 4, ENTRY_ROW = 1;
        const int NURSE_ROW = 2, NURSE_COL = 3;

        // true = wall present
        bool[,] h = new bool[ROWS + 1, COLS];   // wall on top edge of each cell
        bool[,] v = new bool[ROWS, COLS + 1];   // wall on left edge of each cell
        for (int r = 0; r <= ROWS; r++) for (int c = 0; c < COLS; c++) h[r, c] = true;
        for (int r = 0; r < ROWS; r++) for (int c = 0; c <= COLS; c++) v[r, c] = true;

        // Openings traced from the current maze
        int[,] hOpen = { { 1, 0 }, { 1, 2 }, { 2, 1 }, { 3, 0 }, { 3, 1 }, { 3, 2 }, { 3, 3 } };
        int[,] vOpen = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 1, 1 }, { 1, 3 }, { 2, 3 }, { 3, 1 }, { 3, 2 } };
        for (int i = 0; i < hOpen.GetLength(0); i++) h[hOpen[i, 0], hOpen[i, 1]] = false;
        for (int i = 0; i < vOpen.GetLength(0); i++) v[vOpen[i, 0], vOpen[i, 1]] = false;
        v[ENTRY_ROW, COLS] = false; // way in from the dorm

        float W = COLS * cell, D = ROWS * cell;
        // The way in sits at this object's origin, easy to line up with the dorm door
        float ox = -W;
        float oz = (ENTRY_ROW + 0.5f) * cell;

        GameObject root = new GameObject("Maze_Thin");
        Undo.RegisterCreatedObjectUndo(root, "Build Thin Maze");

        for (int r = 0; r <= ROWS; r++)
            for (int c = 0; c < COLS; c++)
                if (h[r, c])
                    Box(root, "Wall",
                        new Vector3(ox + (c + 0.5f) * cell, wallH / 2f, oz - r * cell),
                        new Vector3(cell + wallT, wallH, wallT), wallMat);

        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c <= COLS; c++)
                if (v[r, c])
                    Box(root, "Wall",
                        new Vector3(ox + c * cell, wallH / 2f, oz - (r + 0.5f) * cell),
                        new Vector3(wallT, wallH, cell + wallT), wallMat);

        Box(root, "Maze_Floor",
            new Vector3(ox + W / 2f, -0.01f, oz - D / 2f),
            new Vector3(W + wallT, 0.02f, D + wallT), floorMat);

        if (addCeiling)
            Box(root, "Maze_Ceiling",
                new Vector3(ox + W / 2f, wallH + 0.01f, oz - D / 2f),
                new Vector3(W + wallT, 0.02f, D + wallT), wallMat);

        GameObject nurse = new GameObject("Nurse_Spot");
        nurse.transform.SetParent(root.transform, false);
        nurse.transform.localPosition = new Vector3(ox + (NURSE_COL + 0.5f) * cell, 0f, oz - (NURSE_ROW + 0.5f) * cell);

        Selection.activeGameObject = root;
        Debug.Log("Thin maze built: " + W + " m x " + D + " m");
    }

    static void Box(GameObject parent, string name, Vector3 pos, Vector3 size, Material mat)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name;
        g.transform.SetParent(parent.transform, false);
        g.transform.localPosition = pos;
        g.transform.localScale = size;
        if (mat != null) g.GetComponent<Renderer>().sharedMaterial = mat;
    }
}
