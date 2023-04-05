using System;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;

public abstract class cxIGamePlayerBloc {
    public abstract IObservable<cxTGameDataLoadState> LoadStateAsObservable();
    public abstract Task<TAvatarProfileModel> GetMyPlayerProfile();
}