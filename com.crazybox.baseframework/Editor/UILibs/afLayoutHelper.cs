using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.EditorTools;

//[EditorTool("UI Layout Helper")]
public class afLayoutHelper : EditorTool
{
    GUIContent iconContent;

    void OnEnable()
    {
        iconContent = new GUIContent()
        {
            //image = toolIcon,
            text = "UI Layout Helper",
            tooltip = "UI Layout Helper"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return iconContent; }
    }

    public override void OnActivated()
    {

    }


    public override void OnWillBeDeactivated()
    {
       
    }

    public override void OnToolGUI(EditorWindow window)
    {
        // ...
        Handles.BeginGUI();
        {

            GUILayout.BeginArea(new Rect(0, 0, 200, 300), new GUIStyle("box"));
            GUILayout.BeginVertical(GUILayout.MaxWidth(200));


           // GUILayout.Be
            if (GUILayout.Button("Stop Editing", GUILayout.Height(30)))
                SetupAnchor(0);
            
           
            if (GUILayout.Button("Start Editing", GUILayout.Height(30)))
                SetupAnchor(1);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    void SetupAnchor(int anchor){

    } 

}
