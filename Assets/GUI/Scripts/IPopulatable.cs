using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;



public interface IPopulatable
{
    public abstract void Populate();

    protected static void Populate_Dropdown<T>(TMP_Dropdown dropdown, Dictionary<T, string> nameList)
    {
        dropdown.ClearOptions();
        List<string> names = nameList.Values.ToList();
        dropdown.AddOptions(names);
        dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdown);
    }
}
