using UnityEngine;

public class GridBackground : MonoBehaviour
{
    public int gridWidth = 100;
    public int gridHeight = 80;
    public float cellSize = 1f;
    // Made the grid slightly darker as requested
    public Color gridColor = new Color(0.015f, 0.03f, 0.1f, 0.25f);
    public Color borderColor = new Color(0.1f, 0.4f, 1f, 1f);

    private void Start()
    {
        Material lineMat = new Material(Shader.Find("Sprites/Default"));

        float hw = gridWidth * cellSize / 2f;
        float hh = gridHeight * cellSize / 2f;

        // Vertical grid lines
        for (int x = 0; x <= gridWidth; x++)
        {
            float xPos = x * cellSize - hw;
            CreateLine("GV_" + x, 
                new Vector3(xPos, -hh, 1f), 
                new Vector3(xPos, hh, 1f), 
                0.04f, gridColor, lineMat, -10);
        }

        // Horizontal grid lines
        for (int y = 0; y <= gridHeight; y++)
        {
            float yPos = y * cellSize - hh;
            CreateLine("GH_" + y, 
                new Vector3(-hw, yPos, 1f), 
                new Vector3(hw, yPos, 1f), 
                0.04f, gridColor, lineMat, -10);
        }

        // Border (thicker, brighter) - with colliders
        CreateBorderLine("BorderTop", new Vector3(-hw, hh, 0.5f), new Vector3(hw, hh, 0.5f), 0.1f, borderColor, lineMat, -5, true);
        CreateBorderLine("BorderBot", new Vector3(-hw, -hh, 0.5f), new Vector3(hw, -hh, 0.5f), 0.1f, borderColor, lineMat, -5, true);
        CreateBorderLine("BorderLeft", new Vector3(-hw, -hh, 0.5f), new Vector3(-hw, hh, 0.5f), 0.1f, borderColor, lineMat, -5, false);
        CreateBorderLine("BorderRight", new Vector3(hw, -hh, 0.5f), new Vector3(hw, hh, 0.5f), 0.1f, borderColor, lineMat, -5, false);
    }

    private void CreateLine(string name, Vector3 start, Vector3 end, float width, Color color, Material mat, int sortOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = color;
        lr.endColor = color;
        lr.material = mat;
        lr.sortingOrder = sortOrder;
        lr.useWorldSpace = true;
        lr.numCapVertices = 0;
        lr.numCornerVertices = 0;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.allowOcclusionWhenDynamic = false;
    }

    private void CreateBorderLine(string name, Vector3 start, Vector3 end, float width, Color color, Material mat, int sortOrder, bool isHorizontal)
    {
        CreateLine(name, start, end, width, color, mat, sortOrder);

        // Add an invisible collider for the border
        GameObject colObj = new GameObject(name + "_Collider");
        colObj.transform.SetParent(transform);
        colObj.tag = "Wall";

        BoxCollider2D box = colObj.AddComponent<BoxCollider2D>();
        
        float length = Vector3.Distance(start, end);
        Vector3 center = (start + end) / 2f;
        colObj.transform.position = center;

        if (isHorizontal)
        {
            box.size = new Vector2(length, 1f); // 1 unit thick invisible wall
        }
        else
        {
            box.size = new Vector2(1f, length);
        }
    }
}
