using UnityEngine;
using UnityEditor;

public class PortalPointCreator
{
    [MenuItem("GameObject/Create PortalPoint", false, 10)]
    public static void CreatePortalPoint()
    {
        GameObject portal = new GameObject("PortalPoint");

        // Sprite (옵션)
        SpriteRenderer spriteRenderer = portal.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0, 1, 1, 0.5f); // 연한 시안색

        // Collider
        BoxCollider2D collider = portal.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 1f);

        // PortalTrigger 컴포넌트
        PortalTrigger trigger = portal.AddComponent<PortalTrigger>();
        SpawnPoint spawn = portal.AddComponent<SpawnPoint>();
        trigger.name = "PortalTrigger";



        // Layer 설정
        portal.layer = LayerMask.NameToLayer("Interactable");

        Selection.activeGameObject = portal;
    }
}
