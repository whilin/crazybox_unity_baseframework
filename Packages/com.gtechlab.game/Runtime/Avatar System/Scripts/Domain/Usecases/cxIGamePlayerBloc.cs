using System;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;

public abstract class cxIGamePlayerBloc {
    public abstract IObservable<cxTGameDataLoadState> LoadStateAsObservable { get;}
    public abstract IObservable<TAvatarProfileModel> ProfileAsObservable { get;}
    public abstract IObservable<TAvatarEquipSetModel> AvatarEquipSetAsObservale { get;}

    public abstract Task<TAvatarProfileModel> GetMyPlayerProfile();
    public abstract void ChangeAvatarEquipSet(TAvatarEquipSetModel equipSet, bool preview = false);
    public abstract void ChangeUserProfile(TAvatarProfileModel profile);
}