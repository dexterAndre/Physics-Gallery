using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TODO:
    - Make 3D version
*/
public abstract class PointBehavior_Animation : MonoBehaviour
{
    [SerializeField] protected Manager_PointSet pointManager;


    protected void Awake()
    {
        if (pointManager == null)
        {
            pointManager = transform.parent.GetComponent<Manager_PointSet>();
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
