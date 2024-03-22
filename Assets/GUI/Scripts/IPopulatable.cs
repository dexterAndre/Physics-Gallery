using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;



public interface IPopulatable
{
    public abstract void Populate();

    protected static void Populate_Dropdown(TMP_Dropdown dropdown, List<string> nameList)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(nameList);
        dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdown);
    }

    protected static void Populate_Dropdown<T>(TMP_Dropdown dropdown, Dictionary<T, string> nameList)
    {
        Populate_Dropdown(dropdown, nameList.Values.ToList());
    }
}
