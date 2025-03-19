using System.Collections.Generic;
using UnityEngine;

public class cxUserProfileRootEntity {

    private string userKey;
    private TUserProfileModel userProfile;

    public TUserProfileModel UserProfile => userProfile;
    public string UserKey => userProfile.userKey;
    public bool IsGuest => userProfile !=null && userProfile.socialType == TSocialType.Guest;
    
    public cxUserProfileRootEntity(){

    }

    
    public void SignIn(TUserProfileModel userProfile) {
        this.userProfile = userProfile;

        Debug.LogFormat("SignIn key:{0}, nickname:{1}", userProfile.userKey, userProfile.nickname);
    }

    public void SignOut(){
         this.userProfile  = null;
    }

    public void UpdateUserProfile(TUserProfileModel userProfile){
        this.userProfile  = userProfile;
    }
}