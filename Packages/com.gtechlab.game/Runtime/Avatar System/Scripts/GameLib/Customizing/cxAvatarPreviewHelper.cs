using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cxAvatarPreviewHelper : MonoBehaviour
{
    #if UNITY_EDITOR
    public List<GameObject> previewParts;

    [ContextMenu("Build")]
    void Build(){
        cxAvatarMeshAssembly.ClearSkinnedMeshs(gameObject);
        cxAvatarMeshAssembly.AssemblyAvatar(gameObject, previewParts);
    }

    [ContextMenu("Clear")]
    void Clear(){
        cxAvatarMeshAssembly.ClearSkinnedMeshs(gameObject);
    }
    
    #endif
}
