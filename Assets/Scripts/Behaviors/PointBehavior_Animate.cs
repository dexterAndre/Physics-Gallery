using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TODO:
*/
public abstract class PointBehavior_Animate : MonoBehaviour
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

    public abstract Vector3 UpdateBehavior(Vector3 inVector);
}
