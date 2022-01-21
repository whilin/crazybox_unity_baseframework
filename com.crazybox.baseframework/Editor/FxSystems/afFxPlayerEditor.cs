using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


#if UNITY_EDITOR

[CustomEditor(typeof(afFxPlayer))]
[CanEditMultipleObjects]
public class afFxPlayerEditor : afInspectorEditor
{
    ReorderableList m_compOrder;

    List<ReorderableList> m_composerOrderList;
    List<SerializedProperty> m_composerProps;

    afFxPlayer player;
    SerializedProperty m_compListProp;

    public void OnEnable()
    {
        player = (afFxPlayer)target;
        ListBuild();

        m_selectedFx = null;
    }

    void ListBuild()
    {
        m_composerOrderList = new List<ReorderableList>();
        m_composerProps = new List<SerializedProperty>();

        m_compListProp = serializedObject.FindProperty("m_composerList");

        // m_compOrder = new ReorderableList(player.m_composerList, typeof(afFxPlayer.FxComposer));

        m_compOrder = new ReorderableList(serializedObject, m_compListProp, true, true, true, true);//(player.m_composerList, typeof(afFxPlayer.FxComposer));
        m_compOrder.drawElementCallback += List_DrawElementCallback;
        m_compOrder.drawHeaderCallback += List_DrawHeaderCallback;
        // m_compOrder.onRemoveCallback += M_CompOrder_OnRemoveCallback;
        m_compOrder.elementHeight = EditorGUIUtility.singleLineHeight;

        /*
        for (int i = 0; i < player.m_composerList.Length; i++)
        {
            var comp = player.m_composerList[i];
            var compProp = compListProp.GetArrayElementAtIndex(i);//.serializedObject.FindProperty("fxList");

            var list = new ReorderableList(comp.fxList, typeof(afFxPlayer.FxComposer));
            list.drawElementCallback += M_FxList_DrawElementCallback;

            list.drawHeaderCallback += List_DrawHeaderCallback;
            list.elementHeight = EditorGUIUtility.singleLineHeight ;//* FxUnitDrawer.DRAW_LINE;

            m_composerOrderList.Add(list);
            m_composerProps.Add(compProp);
        }
        */
    }


    afFxPlayer.FxComposer m_selectedFx;
    SerializedProperty m_selectedProp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();


        // m_selectedFx = null;

        m_compOrder.DoLayoutList();

        if (m_selectedFx != null)
        {
            DrawFxComposer(m_selectedFx);

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play " + m_selectedFx.composerName, GUILayout.Width(240)))
                {
                    player.Play(m_selectedFx.composerName, "1234");
                }

                if (GUILayout.Button("Stop " + m_selectedFx.composerName, GUILayout.Width(240)))
                {
                    player.StopPlay(m_selectedFx);
                }
            }
        }


        EditorUtility.SetDirty(player);
        serializedObject.ApplyModifiedProperties();
    }

    void M_CompOrder_OnRemoveCallback(ReorderableList list)
    {
        m_selectedFx = null;
    }


    void List_DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "ComposerList");
    }


    void List_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        var composer = player.m_composerList[index];

        InitRect(rect);

        NextRect(120);
        composer.composerName = EditorGUI.TextField(rect, composer.composerName);

        if (isFocused)
        {
            m_selectedFx = composer;
            m_selectedProp = m_compListProp.GetArrayElementAtIndex(index);
        }

        //if (GUILayout.Button("Play", GUILayout.Width(120)))
        //{
        //	player.Play(composer.composerName, "TEST");
        //}

        //var composer = player.m_composerList[drawIndx];
        //var fxUnit = composer.fxList[index];

        //var fxUnitProp = m_composerProps[drawIndx].FindPropertyRelative("fxList").GetArrayElementAtIndex(index);

        //EditorGUI.PropertyField(rect, fxUnitProp);
    }

    void DrawFxComposer(afFxPlayer.FxComposer fx)
    {
        //fx.composerName =  EditorGUILayout.TextField("Composer Name", fx.composerName, GUILayout.Width(240));
        //EditorGUILayout.ObjectField(m_selectedProp);//(fx, typeof(afFxPlayer.FxComposer));

        List<string> pointNameList = new List<string>();
        foreach (var p in player.m_pointList)
            pointNameList.Add(p.pointName);

        fx.composerName = EditorGUILayout.TextField("Composer Name", fx.composerName);

        //  fx.fxPointId = EditorGUILayout.IntField("PointId", fx.fxPointId);
        fx.fxPointId = EditorGUILayout.Popup("Fx Point", fx.fxPointId, pointNameList.ToArray());

        fx.managed = EditorGUILayout.Toggle("Managed", fx.managed);

        foreach (var f in fx.fxList)
        {
            DrawFxUnit(fx, f);
        }


        if (GUILayout.Button("Add", GUILayout.Width(240)))
        {
            fx.fxList.Add(new afFxPlayer.FxUnit());
        }
    }

    void DrawFxUnit(afFxPlayer.FxComposer fx, afFxPlayer.FxUnit fxUnit)
    {
        EditorGUILayout.BeginVertical("Box");

        fxUnit.fxType = (afFxPlayer.FxType)EditorGUILayout.EnumPopup("FxType", fxUnit.fxType);
        fxUnit.delay = EditorGUILayout.FloatField("Delay", fxUnit.delay);

        if (fxUnit.fxType == afFxPlayer.FxType.Particle ||
            fxUnit.fxType == afFxPlayer.FxType.TextMesh ||
            fxUnit.fxType == afFxPlayer.FxType.Object)
        {
            fxUnit.attachTo = EditorGUILayout.Toggle("Attach To", fxUnit.attachTo);
            fxUnit.attachOffset = EditorGUILayout.Vector3Field("Pos Offset", fxUnit.attachOffset);
            fxUnit.instance = EditorGUILayout.Toggle("Once Play", fxUnit.instance);
            fxUnit.emitDir = (afFxPlayer.EmitDir)EditorGUILayout.EnumPopup("EmitDir", fxUnit.emitDir);
        }

        if (fxUnit.fxType == afFxPlayer.FxType.Particle|| fxUnit.fxType == afFxPlayer.FxType.Object)
        {
            fxUnit.prefabFx = EditorGUILayout.ObjectField("Particle Prefab", fxUnit.prefabFx, typeof(GameObject), false) as GameObject;
        }

        if (fxUnit.fxType == afFxPlayer.FxType.TextMesh)
        {
            fxUnit.prefabTextFx = EditorGUILayout.ObjectField("TextMesh Prefab", fxUnit.prefabTextFx, typeof(GameObject), false) as GameObject;
        }

        if (fxUnit.fxType == afFxPlayer.FxType.Audio)
        {
            fxUnit.audioClip = EditorGUILayout.ObjectField("Audio Clip", fxUnit.audioClip, typeof(AudioClip), false) as AudioClip;
        }

        if (fxUnit.fxType == afFxPlayer.FxType.TimelinePlay)
        {
            fxUnit.timelinePlayer = EditorGUILayout.ObjectField("Timeline Player", fxUnit.timelinePlayer, typeof(PlayableDirector), true) as PlayableDirector;
            fxUnit.timelineAsset = EditorGUILayout.ObjectField("Timeline Assets", fxUnit.timelineAsset, typeof(TimelineAsset), false) as TimelineAsset;
        }

        // if (fxUnit.fxType == afFxPlayer.FxType.HapticFx)
        //{
        //    fxUnit.hapticFx = (afHapticFxType) EditorGUILayout.EnumPopup("Haptic Fx", fxUnit.hapticFx);
        //}


        if (fxUnit.fxType == afFxPlayer.FxType.AnimationPlay)
        {
            fxUnit.animator = EditorGUILayout.ObjectField("Animator", fxUnit.animator, typeof(Animator), true) as Animator;
            
            if (fxUnit.animator != null && fxUnit.animator.runtimeAnimatorController !=null)
            {
                string[] aniList = null;
                afGameObjectUtil.FindAniClips(fxUnit.animator.runtimeAnimatorController, 0, ref aniList);

                int selectedIndex = 0;

                for (int i = 0; i < aniList.Length; i++)
                    if (aniList[i] == fxUnit.animationName)
                    {
                        selectedIndex = i;
                        break;
                    };

                selectedIndex = EditorGUILayout.Popup("Animation Name", selectedIndex, aniList);
                fxUnit.animationName = aniList[selectedIndex];
            }

            //fxUnit.animationName = EditorGUILayout.TextField("Animation Name", fxUnit.animationName);//, typeof(TimelineAsset), false) as TimelineAsset;       
        }

        if (fxUnit.fxType == afFxPlayer.FxType.Active || fxUnit.fxType == afFxPlayer.FxType.Inactive)
            fxUnit.activeTarget = EditorGUILayout.ObjectField("Active Target", fxUnit.activeTarget, typeof(GameObject), true) as GameObject;

        if (fxUnit.fxType == afFxPlayer.FxType.EventCall)
       {
            fxUnit.activeTarget = EditorGUILayout.ObjectField("Target Object", fxUnit.activeTarget, typeof(GameObject), true) as GameObject;
            fxUnit.eventFuncName = EditorGUILayout.TextField("FunctionName", fxUnit.eventFuncName);// , typeof(UnityEvent), false) as UnityEvent;
       }

        if (GUILayout.Button("Remove", GUILayout.Width(240)))
        {
            fx.fxList.Remove(fxUnit);
        }

        EditorGUILayout.EndVertical();


    }
}

#endif