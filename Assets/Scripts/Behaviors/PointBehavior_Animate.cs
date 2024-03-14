using UnityEngine;

/*
    TODO:
*/
public abstract class PointBehavior_Animate : PointBehavior
{
    protected virtual void Awake()
    {
        if (pointManager == null)
        {
            // TODO: Improve safety
            pointManager = GameObject.Find("Points Parent").GetComponent<ManagerPointSet>();
            if (pointManager == null)
            {
                Debug.LogError("ManagerPointSet could not be found. Disabling point animation behavior.");
                gameObject.SetActive(false);
                return;
            }
        }
    }

    public abstract Vector2 UpdateBehavior(Vector2 inVector);
    public abstract Vector3 UpdateBehavior(Vector3 inVector);
}
