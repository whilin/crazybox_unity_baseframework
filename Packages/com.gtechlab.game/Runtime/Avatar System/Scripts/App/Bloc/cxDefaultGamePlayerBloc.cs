using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class cxDefaultGamePlayerBloc : cxIGamePlayerBloc {
    BehaviorSubject<cxTGameDataLoadState> loadState = new BehaviorSubject<cxTGameDataLoadState> (cxTGameDataLoadState.Unloaded);
    public override IObservable<cxTGameDataLoadState> LoadStateAsObservable  => loadState.AsObservable ();

    public override IObservable<TAvatarProfileModel> ProfileAsObservable => throw new NotImplementedException();

    public override IObservable<TAvatarEquipSetModel> AvatarEquipSetAsObservale => throw new NotImplementedException();

    public override TAvatarEquipSetModel CurrentEquipSet => myProfile.equipSet;

    public override TAvatarProfileModel CurrentGameProfile => myProfile;

    TAvatarProfileModel myProfile;

    public cxDefaultGamePlayerBloc () {
        LoadMyProfile ();
    }

    async void LoadMyProfile () {

        loadState.OnNext (cxTGameDataLoadState.Loading);

        try {
            myProfile = LoadMyProfileEx ();
            if (myProfile == null) {
                myProfile = new TAvatarProfileModel () {
                userKey = Guid.NewGuid ().ToString (),
                nickname = "Guest",
                greeting = "Hello, I'm new to here",
                photo = string.Empty,
                // gender = 0,
                equipSet = new TAvatarEquipSetModel () {

                }
                };

                SaveMyProfile (myProfile);
            }

            loadState.OnNext (cxTGameDataLoadState.Loaded);
        } catch (Exception ex) {
            Debug.LogException (ex);
            loadState.OnNext (cxTGameDataLoadState.Failed);
        }
    }

    public override async Task<TAvatarProfileModel> GetMyPlayerProfile () {
        return myProfile;
    }

    TAvatarProfileModel LoadMyProfileEx () {
        var json = PlayerPrefs.GetString ("saved_my_profile");
        if (!string.IsNullOrEmpty (json)) {
            var obj = JsonConvert.DeserializeObject<TAvatarProfileModel> (json);
            return obj;
        }

        return null;
    }

    void SaveMyProfile (TAvatarProfileModel myProfile) {
        var json = JsonConvert.SerializeObject (myProfile);
        PlayerPrefs.SetString ("saved_my_profile", json);
        PlayerPrefs.Save ();
    }

    public override void ChangeAvatarEquipSet(TAvatarEquipSetModel equipSet, bool preview = false)
    {
        throw new NotImplementedException();
    }

    public override void ChangeUserProfile(TAvatarProfileModel profile)
    {
        throw new NotImplementedException();
    }
}