using System;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;


public abstract class cxIGameItemBloc {

    public abstract IObservable<cxTGameDataLoadState> LoadStateAsObservable();
    public abstract TAvatarItemDescModel FindAvatarItemDesc(int itemCode);
    public abstract Task<GameObject> LoadAvatarMeshPrefab(TAvatarItemDescModel desc);
    public abstract Task<GameObject> LoadAvatarSetupPrefab();
    public abstract Task<GameObject> LoadAvatarViewerSetupPrefab();
}