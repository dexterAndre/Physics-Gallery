using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TODO:
    - Make 3D version
*/
public abstract class PointBehaviorAnimation2 : MonoBehaviour
{
    [SerializeField] protected ManagerPointSet pointManager;


    protected void Awake()
    {
        if (pointManager == null)
        {
            pointManager = transform.parent.GetComponent<ManagerPointSet>();
            if (pointManager == null)
            {
                Debug.LogError("ManagerPointSet could not be found. Disabling point animation behavior.");
                gameObject.SetActive(false);
                return;
            }
        }
    }

    public abstract Vector2 UpdateBehavior();
}
