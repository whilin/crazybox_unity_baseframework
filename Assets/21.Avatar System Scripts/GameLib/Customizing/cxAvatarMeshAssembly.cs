using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class cxAvatarMeshAssembly {

    static async Task<GameObject> FindAvatarItem (int itemCode, int defaultItemCode) {

        var itemDesc = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (itemCode != 0 ? itemCode : defaultItemCode);
        if (itemDesc != null) {
            var prefabs = await cxGetIt.Get<cxIGameItemBloc> ().LoadAvatarMeshPrefab (itemDesc);
            return prefabs;
        }

        return null;
    }

    public static async void AssemblyAvatar (GameObject avatarGameObject, TAvatarEquipSetModel equipSet, int layer = 6) {

        TAvatarItemDescModel[] items = new TAvatarItemDescModel[6];
        GameObject[] prefabs = new GameObject[6];

        var config = cxDefaultAvatarConfig.GetAvatarConfig ();

        prefabs[0] = await FindAvatarItem (equipSet.hairItemCode, config.defaultEquipSet.hairItemCode);
        prefabs[1] = await FindAvatarItem (equipSet.faceAccessoryItemCode, config.defaultEquipSet.faceAccessoryItemCode);
        prefabs[2] = await FindAvatarItem (equipSet.wearItemCode, config.defaultEquipSet.wearItemCode);
        prefabs[3] = await FindAvatarItem (equipSet.faceItemCode, config.defaultEquipSet.faceItemCode);
        prefabs[4] = await FindAvatarItem (equipSet.shoesItemCode, config.defaultEquipSet.shoesItemCode);
        prefabs[5] = await FindAvatarItem (equipSet.bodyItemCode, config.defaultEquipSet.bodyItemCode);

        // prefabs[1] = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (equipSet.faceItemCode);
        // prefabs[2] = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (equipSet.faceAccessoryItemCode);
        // prefabs[3] = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (equipSet.wearItemCode);
        // prefabs[4] = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (equipSet.shoesItemCode);
        // prefabs[5] = cxGetIt.Get<cxIGameItemBloc> ().FindAvatarItemDesc (equipSet.bodyItemCode);
        // for (int i = 0; i < items.Length; i++) {
        //     if (items[i] != null) 
        //         prefabs[i] = await cxGetIt.Get<cxIGameItemBloc> ().LoadAvatarMeshPrefab (items[i]);
        // }

        for (int i = 0; i < prefabs.Length; i++) {
            if (prefabs[i])
                AssemblyAvatar (avatarGameObject, prefabs[i], layer);
        }

        avatarGameObject.GetComponent<Animator> ()?.Rebind ();
#if UNITY_EDITOR
        foreach (Transform t in avatarGameObject.GetComponentsInChildren<Transform> ()) {
            if (t.gameObject.GetComponent<Renderer> () != null) {
                Material[] myMaterials = t.gameObject.GetComponent<Renderer> ().materials;
                foreach (Material material in myMaterials) {
                    material.shader = Shader.Find (material.shader.name);
                }
            }
        }
#endif
    }

    public static void AssemblyAvatar (GameObject avatarGameObject, List<GameObject> skinedMeshSources, int layer = 0) {
        foreach (var prefab in skinedMeshSources)
            AssemblyAvatar (avatarGameObject, prefab, layer);

        avatarGameObject.GetComponent<Animator> ()?.Rebind ();
#if UNITY_EDITOR
        foreach (Transform t in avatarGameObject.GetComponentsInChildren<Transform> ()) {
            if (t.gameObject.GetComponent<Renderer> () != null) {
                Material[] myMaterials = t.gameObject.GetComponent<Renderer> ().materials;
                foreach (Material material in myMaterials) {
                    material.shader = Shader.Find (material.shader.name);
                }
            }
        }
#endif
    }

    public static void AssemblyAvatar (GameObject gameObject, GameObject skinedMeshSource, int layer = 0) {

        //메쉬 오브젝트 생성 및 할당
        GameObject skinedMeshObj = GameObject.Instantiate (skinedMeshSource);

        //위치 설정
        skinedMeshObj.transform.localPosition = gameObject.transform.localPosition;

        //스킨 메쉬데이터 획득
        foreach (SkinnedMeshRenderer smr in skinedMeshObj.GetComponentsInChildren<SkinnedMeshRenderer> ()) {
            //캐릭터 본 데이터 설정 (원래 되어있던 방식)
            To_CharacterBoneDataSetting (gameObject, smr, layer);
        }

        //할당된 오브젝트 제거
        if (isEditingMode)
            GameObject.DestroyImmediate (skinedMeshObj);
        else
            GameObject.Destroy (skinedMeshObj);

        gameObject.GetComponent<Animator> ()?.Rebind ();
    }

    private static void To_CharacterBoneDataSetting (GameObject gameObject, SkinnedMeshRenderer thisRenderer, int layer) {
        //새로운 오브젝트 생성
        GameObject tempEquipment = new GameObject (thisRenderer.name);
        tempEquipment.gameObject.SetActive (true);
        tempEquipment.transform.parent = gameObject.transform;

        SkinnedMeshRenderer NewRenderer = tempEquipment.AddComponent (typeof (SkinnedMeshRenderer)) as SkinnedMeshRenderer;

        Transform[] bones = new Transform[thisRenderer.bones.Length];

        for (int i = 0; i < thisRenderer.bones.Length; i++) {
            //본Trasnform 할당 
            bones[i] = Get_ChildByName (thisRenderer.bones[i].name, gameObject.transform);
        }

        //할당된 본 데이터 새로운 메쉬렌더에 설정
        NewRenderer.bones = bones;
        NewRenderer.rootBone = Get_ChildByName (thisRenderer.rootBone.name, gameObject.transform);
        //메쉬 렌더의 스키닝을 일치화 처리
        NewRenderer.sharedMesh = thisRenderer.sharedMesh;
        NewRenderer.sharedMaterials = thisRenderer.sharedMaterials;
        if (NewRenderer.sharedMesh.blendShapeCount > 0) {
            NewRenderer.SetBlendShapeWeight (0, 100f);
        }

        if (layer != 0)
            NewRenderer.gameObject.layer = layer;
    }

    public static bool isEditingMode {
        get {
            bool isEditingMode = false;
#if UNITY_EDITOR
            isEditingMode = !UnityEditor.EditorApplication.isPlaying;
#endif
            return isEditingMode;
        }
    }

    public static void ClearSkinnedMeshs (GameObject gameObject) {

        SkinnedMeshRenderer[] boneObjs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer> ();
        foreach (SkinnedMeshRenderer newSmr in boneObjs) {
            if (isEditingMode)
                GameObject.DestroyImmediate (newSmr.gameObject);
            else
                GameObject.Destroy (newSmr.gameObject);
        }
    }

    private static Transform Get_ChildByName (string objName, Transform trasnform) {
        Transform ReturnObj;

        if (trasnform.name == objName)
            return trasnform.transform;

        foreach (Transform child in trasnform) {
            ReturnObj = Get_ChildByName (objName, child);

            if (ReturnObj != null)
                return ReturnObj;
        }
        return null;
    }
}