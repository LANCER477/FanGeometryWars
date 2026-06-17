using UnityEngine;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private GameObject doubleShotParent;
    private GameObject spreadShotParent;
    private GameObject tailGunParent;

    private void Start()
    {
        CreateVisuals();
        
        PlayerShooting ps = GetComponent<PlayerShooting>();
        if (ps != null)
        {
            UpdateVisuals(ps.currentWeapon);
        }
    }

    private void CreateVisuals()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));

        // 1. DoubleShot Visuals (Two small squares on the sides)
        doubleShotParent = new GameObject("DoubleShotVisuals");
        doubleShotParent.transform.SetParent(transform, false);
        
        GameObject dsLeft = CreatePrimitive(PrimitiveType.Quad, "LeftGun", doubleShotParent.transform, new Vector3(0f, 0.4f, 0f), 0.2f, Color.cyan, mat);
        GameObject dsRight = CreatePrimitive(PrimitiveType.Quad, "RightGun", doubleShotParent.transform, new Vector3(0f, -0.4f, 0f), 0.2f, Color.cyan, mat);
        doubleShotParent.SetActive(false);

        // 2. SpreadShot Visuals (Three tiny circles in front)
        spreadShotParent = new GameObject("SpreadShotVisuals");
        spreadShotParent.transform.SetParent(transform, false);
        
        GameObject ss1 = CreatePrimitive(PrimitiveType.Sphere, "Spread1", spreadShotParent.transform, new Vector3(0.5f, 0f, 0f), 0.15f, Color.yellow, mat);
        GameObject ss2 = CreatePrimitive(PrimitiveType.Sphere, "Spread2", spreadShotParent.transform, new Vector3(0.45f, 0.3f, 0f), 0.15f, Color.yellow, mat);
        GameObject ss3 = CreatePrimitive(PrimitiveType.Sphere, "Spread3", spreadShotParent.transform, new Vector3(0.45f, -0.3f, 0f), 0.15f, Color.yellow, mat);
        spreadShotParent.SetActive(false);

        // 3. TailGun Visuals (Triangle facing back)
        // Unity's primitive triangle doesn't exist easily, so we use a squashed quad or sphere to represent it.
        // Or simply a different colored small block.
        tailGunParent = new GameObject("TailGunVisuals");
        tailGunParent.transform.SetParent(transform, false);
        
        GameObject tg = CreatePrimitive(PrimitiveType.Quad, "TailGun", tailGunParent.transform, new Vector3(-0.5f, 0f, 0f), 0.3f, Color.cyan, mat);
        // Make it look slightly different (rectangle)
        tg.transform.localScale = new Vector3(0.2f, 0.4f, 1f);
        tailGunParent.SetActive(false);
    }

    private GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPos, float scale, Color color, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = new Vector3(scale, scale, 1f);
        
        // Remove 3D colliders added by CreatePrimitive
        Collider col = obj.GetComponent<Collider>();
        if (col != null) Destroy(col);

        // Assign material and color
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = mat;
            rend.material.color = color;
        }

        return obj;
    }

    public void UpdateVisuals(WeaponType type)
    {
        if (doubleShotParent != null) doubleShotParent.SetActive(type == WeaponType.DoubleShot);
        if (spreadShotParent != null) spreadShotParent.SetActive(type == WeaponType.SpreadShot);
        if (tailGunParent != null) tailGunParent.SetActive(type == WeaponType.TailGun);
    }
}
