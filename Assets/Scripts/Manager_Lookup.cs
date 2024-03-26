using UnityEngine;

[ExecuteAlways]
public sealed class Manager_Lookup : MonoBehaviour
{
    // Singleton implementation
    private static Manager_Lookup instance;
    public static Manager_Lookup Instance { get { return instance; } }

    private void OnEnable()
    {
        MaintainInstance();
    }

    private void OnValidate()
    {
        MaintainInstance();
    }

    private void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }

    private void MaintainInstance()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] private Manager_GUI managerGUI;
    public Manager_GUI ManagerGUI { get { return managerGUI; } }
    [SerializeField] private Manager_PointSet managerPointSet;
    public Manager_PointSet ManagerPointSet { get {  return managerPointSet; } }
}
