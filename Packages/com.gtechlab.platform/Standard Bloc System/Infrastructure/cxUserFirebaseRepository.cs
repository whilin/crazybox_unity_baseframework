using System.Collections.Generic;
using System.Threading.Tasks;

public class cxUserFirebaseRepository : IUserRepository {

    cxIFirebaseDBDriver dbDriver;
    cxFirebaseDBRef userCollectionRef;

    public cxUserFirebaseRepository (cxIFirebaseDBDriver dbDriver) {
        this.dbDriver = dbDriver;
        this.userCollectionRef = dbDriver.GetDataReference ("users");
    }

    public async Task<TUserProfileModel> SignUp (TSocialUserModel socialUser, Dictionary<string, string> userAuxProfileInfo) {
        var dbRef = userCollectionRef.Child (socialUser.socialKey);
        var profileData = await dbDriver.GetValueAsync (dbRef.Path);

        if (profileData.Exists) {
            throw new System.Exception ("User already exists");
        }

        TUserProfileModel profile = new TUserProfileModel {
            userKey = socialUser.socialKey,
            email = socialUser.email,
            nickname = socialUser.nickname,
            photoURL = socialUser.photoURL,
            socialKey = socialUser.socialKey,
            socialType = socialUser.socialType,
            auxProfileInfo = userAuxProfileInfo
        };

        await dbDriver.SetValueAsync (dbRef.Path, Newtonsoft.Json.JsonConvert.SerializeObject (profile));

        return profile;
    }

    public async Task<TUserProfileModel> SignIn (TSocialUserModel socialUser) {
        var dbRef = userCollectionRef.Child (socialUser.socialKey);
        var profileData = await dbDriver.GetValueAsync (dbRef.Path);

        var rawValue = profileData.GetRawJsonValue ();
        UnityEngine.Debug.Log ("SignIn RawValue:" + rawValue);
        UnityEngine.Debug.Log ("SignIn profileData Exist:" + profileData.Exists);

        if (!profileData.Exists) {
            throw new cxBlocException ((int) cxMessageCode.UserNotFound, "User not found");
        }

        var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<TUserProfileModel> (profileData.GetRawJsonValue ());
        return profile;
    }

    public async Task<TUserOpenProfileModel> FindUserProfile (string userKey) {
        var dbRef = userCollectionRef.Child (userKey);

        var profileData = await dbDriver.GetValueAsync (dbRef.Path);
        if (!profileData.Exists) {
            return null;
        }

        var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<TUserProfileModel> (profileData.GetRawJsonValue ());
        return new TUserOpenProfileModel {
            userKey = profile.userKey,
                nickname = profile.nickname,
                photoURL = profile.photoURL
        };
    }

    public Task<TUserProfileModel> LinkToSocialUser (string userKey, TSocialUserModel socialUser, Dictionary<string, string> userAuxProfileInfo) {
        throw new System.NotImplementedException ();
    }

    public async Task<TUserProfileModel> UpdateProfile (string userKey, Dictionary<string, object> profileModel) {
        var dbRef = userCollectionRef.Child (userKey);
        await dbDriver.UpdateValueAsync (dbRef.Path, profileModel);
        var profileData = await dbDriver.GetValueAsync (dbRef.Path);
        var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<TUserProfileModel> (profileData.GetRawJsonValue ());
        return profile;
    }

    public Task<string> UploadProfileImage (string userKey, byte[] bytes) {
        throw new System.NotImplementedException ();
    }

    public Task SignOut (string userKey) {
        throw new System.NotImplementedException ();
    }

    public Task WithdrawAccount (string userKey) {
        throw new System.NotImplementedException ();
    }
}