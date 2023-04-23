using System;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;
using System.Collections.Generic;


public interface cxIGameAvatarItemBloc {
   TAvatarItemDescModel FindAvatarItemDesc(int itemCode);
   Task<GameObject> LoadAvatarMeshPrefab(TAvatarItemDescModel desc);
}

public abstract class cxIGameItemBloc  : cxIGameAvatarItemBloc {
    
    public abstract TAvatarItemDescModel FindAvatarItemDesc(int itemCode);
    public abstract Task<GameObject> LoadAvatarMeshPrefab(TAvatarItemDescModel desc);
    public abstract Task<GameObject> LoadAvatarSetupPrefab();
    public abstract Task<GameObject> LoadAvatarViewerSetupPrefab();
    
    
    public abstract IObservable<cxTGameDataLoadState> LoadStateAsObservable();
    public virtual List<TAvatarItemDescModel> GetItemList(ItemType itemType) {
        throw new NotImplementedException();
    }
}