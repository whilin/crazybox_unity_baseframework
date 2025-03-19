using System;
using System.Collections.Generic;

public class TUserOpenProfileModel{
    public string userKey;
    public string nickname;
    public string photoURL;
}

public class TUserProfileModel
{
    //Note. V1.1
    public string userKey;

    public TSocialType socialType;
    public string socialKey;
   
    public string nickname;
    public string email;
    public string photoURL;

    public bool withdrawFlag;
    public DateTime withdrawDate;

    //Note. V2.2
    public DateTime regDate;
    public bool allowPush;
    public bool allowEmail;
    public string platformId;
    public string pushToken;

    public Dictionary<string, string> auxProfileInfo;
}

