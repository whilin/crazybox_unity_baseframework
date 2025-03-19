using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class cxUserOpenProfileBloc {
    private Dictionary<string, TUserOpenProfileModel> UserProfiles { get; set; }

    public cxUserOpenProfileBloc() {
        UserProfiles = new Dictionary<string, TUserOpenProfileModel>();
    }

    public async Task<TUserOpenProfileModel> FindUserProfile(string userKey){

        if(string.IsNullOrEmpty(userKey)){
            return null;
        }
        
        if(UserProfiles.TryGetValue(userKey, out var model))
            return model;

        var userProfile = await cxGetIt.Get<IUserRepository>().FindUserProfile(userKey);
        if(userProfile != null){
            UserProfiles.Add(userKey, userProfile);
        }

        return userProfile;
    }
}