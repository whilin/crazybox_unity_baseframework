using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository 
{
    Task<TUserProfileModel> SignIn (TSocialUserModel socialUser);
    Task<TUserProfileModel> SignUp (TSocialUserModel socialUser, Dictionary<string, string> userAuxProfileInfo);
    Task<TUserProfileModel> LinkToSocialUser (string userKey, TSocialUserModel socialUser,  Dictionary<string, string> userAuxProfileInfo); //게스트 사용자를 소셜 유저 프로필로 전환

    Task<TUserProfileModel> UpdateProfile (string userKey, Dictionary<string, object> profileModel);
    Task<string> UploadProfileImage (string userKey, byte[] bytes);

    Task SignOut (string userKey);
    Task WithdrawAccount (string userKey);

    Task<TUserOpenProfileModel> FindUserProfile (string userKey);
}
