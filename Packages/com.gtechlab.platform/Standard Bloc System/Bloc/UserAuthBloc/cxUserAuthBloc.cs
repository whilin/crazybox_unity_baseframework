using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public enum cxUserAuthBlocState {
    Init,
    SignOut,
    SignIn,
    RequiredSingUp, //소셜로그인 성공후, 신규 가입 필요 상태
}

public enum cxUserAuthBlocEvent {
    SignInCompleted,
    SignUpCompleted,
    SignOutCompleted,
    SignInFailed,
    SignOutFailed,
    SignUpCancelled,
    SignUpFailed,

    ProfileUpdated,
    ProfileUpdateFailed
}

public class cxUserAuthBloc {

    private Subject<cxUserAuthBlocEvent> authBlocEvent = new Subject<cxUserAuthBlocEvent> ();
    private BehaviorSubject<cxUserAuthBlocState> authBlocState = new BehaviorSubject<cxUserAuthBlocState> (cxUserAuthBlocState.Init);

    public IObservable<cxUserAuthBlocState> AuthBlocStateAsObservable => authBlocState.AsObservable ();
    public IObservable<cxUserAuthBlocEvent> AuthBlocEventAsObservable => authBlocEvent.AsObservable ();

    public TSocialUserModel signUpSocialUserInfo { get; private set; }

    public cxUserAuthBloc () {

    }

    public async void AddAppInitEvent (bool autoLoginWithGuest = false) {
        cxLog.Verbose ("AddAppInitEvent");

        try {
            await new WaitForSecondsRealtime (2.0f);

            var socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().HasAuthSession ();
            if (socialUser != null) {
                cxLog.Log ("AddAppInitEvent Has Auth Session socialUser type:{0}, key:{1}", socialUser.socialType, socialUser.socialKey);
                await SignIn (socialUser, autoLoginWithGuest);
            } else if(autoLoginWithGuest) {
                cxLog.Log ("AddAppInitEvent Has not auth session, try to guest login");
                SignUpAsGuest ();
            } else {
                cxLog.Log ("AddAppInitEvent Has not auth session, stay at init state");
                authBlocState.OnNext (cxUserAuthBlocState.SignOut);
            }
        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignInFailed);
        }
    }

    public async void AddSignInWithGoogleEvent () {
        cxLog.Verbose ("AddSignInWithGoogleEvent");

        try {
            var socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().SignInWithGoogle ();
            if (socialUser != null) {
                await SignIn (socialUser, false);
            } else
                throw new Exception ("AddSignInWithGoogleEvent return null");

        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignInFailed);
        }
    }

    public async void AddSignInWithEmailEventEvent (string email, string password) {
        cxLog.Verbose ("AddSignInWithEmailEventEvent");

        try {
            var socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().SignInWithEmailAndPassword (email, password);
            if (socialUser != null) {
                await SignIn (socialUser, false);
            } else
                throw new Exception ("AddSignInWithEmailEventEvent return null");

        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignInFailed);
        }
    }

    public async void AddSignInWithKakaoEvent () {
        cxLog.Verbose ("AddSignInWithKakaoEvent");
        throw new NotImplementedException ("AddSignInWithKakaoEvent");

        // try {
        //     var socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().LoginWithKakao ();
        //     if (socialUser != null) {
        //         await SignIn (socialUser, false);
        //     } else 
        //         throw new Exception("AddSignInWithKakaoEvent return null");
        // } catch (Exception ex) {
        //     Debug.LogException (ex);
        //     authBlocEvent.OnNext (sdUserAuthBlocEvent.SignInFailed);
        // }
    }

    public async void AddSignInWithGuestEvent () {
        cxLog.Verbose ("AddSignInWithGuestEvent");

        try {
            var socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().SignInAnonymously ();
            if (socialUser != null) {
                await SignIn (socialUser, false);
            } else
                throw new Exception ("AddSignInWithGuestEvent return null ");

        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignInFailed);
        }
    }

    public async void AddSignOutEvent () {
        cxLog.Verbose ("AddSignOutEvent");

        try {
            await SignOut ();
        } catch (Exception ex) {
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignOutFailed);
        }
    }

    public async void AddSignUpWithEmailPasswordEvent (string nickname, string email, string password, Dictionary<string, string> userAuxProfileInfo) {
        cxLog.Verbose ("AddSignUpWithEmailPasswordEvent");

        try {
            var userInfo =await cxGetIt.Get<cxIFirebaseAuthDriver> ().CreateUserWithEmailAndPassword (email, password);
            if(userInfo == null) {
               authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpFailed);
               return;
            }

            userInfo.nickname = nickname;
            var userProfile =await cxGetIt.Get<IUserRepository> ().SignUp (userInfo, userAuxProfileInfo);
            cxGetIt.Get<cxUserProfileRootEntity> ().SignIn (userProfile);
            authBlocState.OnNext (cxUserAuthBlocState.SignIn);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpCompleted);
        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpFailed);
        }
    }

/*
    public async void AddProfileUpdateEvent (string nickname, Dictionary<string, string> userAuxProfileInfo) {
        cxLog.Verbose ("AddProfileUpdateEvent");

        try {
            var userProfile = cxGetIt.Get<cxUserProfileRootEntity> ().UserProfile;

            userProfile.nickname = nickname;
            userProfile.auxProfileInfo = userAuxProfileInfo;

            Dictionary<string, object> updated = new Dictionary<string, object> ();
            updated["nickname"] = nickname;
            updated["auxProfileInfo"] = userAuxProfileInfo;

            //TODO: Save Profile!!!
            var updatedProfile = await cxGetIt.Get<IUserRepository> ().UpdateProfile (userProfile.userKey, updated);
            //var updatedProfile = userProfile;
            cxGetIt.Get<cxUserProfileRootEntity> ().UpdateUserProfile (updatedProfile);

            authBlocEvent.OnNext (cxUserAuthBlocEvent.SaveProfileCompleted);
        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SaveProfileFailed);
        }
    }
    */

    readonly List<string> AllowedUpdateFields = new List<string>{ "photoURL", "nickname", "pushToken", "allowEmail", "allowPush"};

    public async void AddProfileUpdateEvent (Dictionary<string, object> updateFields) {
        cxLog.Verbose ("AddProfileUpdateEvent");

        try {
            var userProfile = cxGetIt.Get<cxUserProfileRootEntity> ().UserProfile;

            foreach(var key in AllowedUpdateFields) {
                if(!AllowedUpdateFields.Contains(key)){
                    Debug.LogWarning ("AddProfileUpdateEvent, not allowed field:" + key);
                    return;
                }
            }

            //TODO: Save Profile!!!
            var updatedProfile = await cxGetIt.Get<IUserRepository> ().UpdateProfile (userProfile.userKey, updateFields);
            //var updatedProfile = userProfile;
            cxGetIt.Get<cxUserProfileRootEntity> ().UpdateUserProfile (updatedProfile);

            authBlocEvent.OnNext (cxUserAuthBlocEvent.ProfileUpdated);
        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.ProfileUpdateFailed);
        }
    }

    public async void AddSignUpWithAdditionalInfoEvent (string nickname, Dictionary<string, string> userAuxProfileInfo) {
        try {
            var userProfile = cxGetIt.Get<cxUserProfileRootEntity> ().UserProfile;
            bool atGuestState = userProfile != null && userProfile.socialType == TSocialType.Guest;

            if (atGuestState) {
                signUpSocialUserInfo.nickname = nickname;
                userProfile = await cxGetIt.Get<IUserRepository> ().LinkToSocialUser (userProfile.userKey, signUpSocialUserInfo, userAuxProfileInfo);

                cxGetIt.Get<cxUserProfileRootEntity> ().SignIn (userProfile);

                //TODO: 계정 전환일 경우!!!!, SignIn 이벤트는 위험하다... UpdateProfile Event 등이 필요하다!!!
                authBlocState.OnNext (cxUserAuthBlocState.SignIn);
                authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpCompleted);

            } else {
                signUpSocialUserInfo.nickname = nickname;
                userProfile = await cxGetIt.Get<IUserRepository> ().SignUp (signUpSocialUserInfo, userAuxProfileInfo);

                cxGetIt.Get<cxUserProfileRootEntity> ().SignIn (userProfile);
                authBlocState.OnNext (cxUserAuthBlocState.SignIn);
                authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpCompleted);
            }

        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocState.OnNext (cxUserAuthBlocState.SignIn); //Guest 로그인 상태로 남겨두기 위해서, But 이벤트로 인해 릴로딩 이슈 발생함
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpFailed);
        }
    }

    public void AddSignUpCancelEvent () {
        try {
            signUpSocialUserInfo = null;
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpCancelled);
        } catch (Exception ex) {

        }
    }

    async Task SignIn (TSocialUserModel socialUser, bool autoLoginWithGuest) {
        try {
            var userProfile = await cxGetIt.Get<IUserRepository> ().SignIn (socialUser);
            cxGetIt.Get<cxUserProfileRootEntity> ().SignIn (userProfile);
            authBlocState.OnNext (cxUserAuthBlocState.SignIn);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignInCompleted);

            cxLog.Log ("SignIn Completed");

        } catch (cxBlocException ex) {
            if (ex.Code == (int) cxMessageCode.UserNotFound) {
                cxLog.Log ("SignIn failed: UserNotFound in CachedMode:" + autoLoginWithGuest);

                if (autoLoginWithGuest) {
                    SignUpAsGuest ();
                } else {
                    var userProfile = cxGetIt.Get<cxUserProfileRootEntity> ().UserProfile;
                    // userProfile.nickname = socialUser.nickname;

                    signUpSocialUserInfo = socialUser;
                    authBlocState.OnNext (cxUserAuthBlocState.RequiredSingUp);
                }
            } else {
                Debug.LogException (ex);
                throw ex;
            }
        }
    }

    async void SignUpAsGuest () {
        cxLog.Log ("SignUpAsGuest");

        try {

            TSocialUserModel socialUser = await cxGetIt.Get<cxIFirebaseAuthDriver> ().SignInAnonymously ();
            var info = new Dictionary<string, string> () { };
            socialUser.nickname = "GUEST";
            var userProfile = await cxGetIt.Get<IUserRepository> ().SignUp (socialUser, info);
            cxGetIt.Get<cxUserProfileRootEntity> ().SignIn (userProfile);
            authBlocState.OnNext (cxUserAuthBlocState.SignIn);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpCompleted);
        } catch (Exception ex) {
            Debug.LogException (ex);
            authBlocEvent.OnNext (cxUserAuthBlocEvent.SignUpFailed);
        }
    }

    async Task SignOut () {
        await cxGetIt.Get<cxIFirebaseAuthDriver> ().SignOut ();
        cxGetIt.Get<cxUserProfileRootEntity> ().SignOut ();
        authBlocState.OnNext (cxUserAuthBlocState.SignOut);
        authBlocEvent.OnNext (cxUserAuthBlocEvent.SignOutCompleted);
    }
}