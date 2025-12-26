using UnityEditor;
using Combat.Actors;
using UnityEngine;
using Combat.Data;

[CustomEditor(typeof(Actor))]
public class ActorViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Collider"))
        {
            Actor actor = (Actor)target;
            TryAddCollider(actor);
        }

        if (GUILayout.Button("Save View Data"))
        {
            Actor actor = (Actor)target;
            SaveViewData(actor);
        }
    }

    private void SaveViewData(Actor actor)
    {
        if (actor.ViewData == null)
        {
            Debug.LogError("ActorViewData is not assigned.");
            return;
        }

        switch (actor.CollisionAreaType)
        {
            case Core.Units.CollisionAreaType.Circle:

                actor.ViewData.UnitCollisionData.AreaType = Core.Units.CollisionAreaType.Circle;
                actor.TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider);
                if (capsuleCollider != null)
                {
                    actor.ViewData.ModelView.Height = capsuleCollider.height;
                    actor.ViewData.ModelView.CenterOffset = capsuleCollider.center.y;
                    actor.ViewData.ModelView.Radius = capsuleCollider.radius;
                    actor.ViewData.UnitCollisionData.Radius = capsuleCollider.radius;
                    actor.ViewData.UnitCollisionData.Offset = new Vector2(capsuleCollider.center.x, capsuleCollider.center.z);
                }
                break;
            case Core.Units.CollisionAreaType.Rectangle:
                actor.ViewData.UnitCollisionData.AreaType = Core.Units.CollisionAreaType.Rectangle;
                actor.TryGetComponent<BoxCollider>(out BoxCollider boxCollider);
                if (boxCollider != null)
                {
                    actor.ViewData.ModelView.Height = boxCollider.size.y;
                    actor.ViewData.ModelView.CenterOffset = boxCollider.center.y;
                    actor.ViewData.UnitCollisionData.Radius = 0f;
                    actor.ViewData.UnitCollisionData.Size = new Vector2(boxCollider.size.x, boxCollider.size.z);
                    actor.ViewData.UnitCollisionData.Offset = new Vector2(boxCollider.center.x, boxCollider.center.z);
                    actor.ViewData.ModelView.Radius = (boxCollider.size.x + boxCollider.size.z) * 0.5f;
                }
                break;
            default:
                Debug.LogWarning("Unsupported collider type: " + actor.CollisionAreaType);
                break;
        }
        EditorUtility.SetDirty(actor.ViewData);
        AssetDatabase.SaveAssets();
        Debug.Log("ActorViewData saved successfully.");
    }

    private void TryAddCollider(Actor actor)
    {
        if (!actor.TryGetComponent<Collider>(out Collider targetCollider))
        {
            AddCollider(actor);
        }
        else
        {
            EditorUtility.DisplayDialog("Info",
            "GameObject already has a Collider component.This action will remove the existing collider.", "OK", "Cancel");
            DestroyImmediate(targetCollider);
            AddCollider(actor);
        }
    }

    private void AddCollider(Actor actor)
    {
        var colliderType = actor.CollisionAreaType;
        switch (colliderType)
        {
            case Core.Units.CollisionAreaType.Circle:
                Undo.AddComponent<CapsuleCollider>(actor.gameObject);
                break;
            case Core.Units.CollisionAreaType.Rectangle:
                Undo.AddComponent<BoxCollider>(actor.gameObject);
                break;
            default:
                Debug.LogWarning("Unsupported collider type: " + colliderType);
                break;
        }
        EditorUtility.SetDirty(actor.gameObject);
    }
}