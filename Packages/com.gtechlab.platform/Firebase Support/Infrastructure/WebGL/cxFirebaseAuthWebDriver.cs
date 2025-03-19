using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using UnityEngine;

public class cxFirebaseAuthWebGLDriver : cxIFirebaseAuthDriver {

    public override async Task<TSocialUserModel> HasAuthSession () {

        var user = await FirebaseAuth.CheckCurrentAuthUser ();
        if (user == null) {
            return null;
        } else {
            //TODO
            TSocialType socialType = FirebaseUtils.ProviderIdToSocialType (user.providerId);

            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = socialType,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        }
    }

    public override async Task<TSocialUserModel> SignInAnonymously () {
        try {
            var user = await FirebaseAuth.SignInAnonymously ();
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.Guest,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> CreateUserWithEmailAndPassword (string email, string password) {
        try {
            var user = await FirebaseAuth.CreateUserWithEmailAndPassword (email, password);
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.EMail,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> SignInWithEmailAndPassword (string email, string password) {
        try {
            var user = await FirebaseAuth.SignInWithEmailAndPassword (email, password);
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.EMail,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }


    public  override async Task ResetEmailPassword () {
        try {
            //await FirebaseAuth.SendPasswordResetEmail(email);
            throw new Exception("Not implemented");

        }  catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> SignInWithGoogle () {
        try {
            var user = await FirebaseAuth.SignInWithGoogle ();
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.Guest,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task SignOut () {
        try {
            FirebaseAuth.SignOut ();
        } catch (Exception e) {
            throw HandleException (e);
        }
    }


    private Exception HandleException (Exception e) {
        try {
            var firebaseError = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseError> (e.Message);
            return new cxBlocException (0, firebaseError.message);
        } catch (Exception) {
            return e;
        }
    }
}