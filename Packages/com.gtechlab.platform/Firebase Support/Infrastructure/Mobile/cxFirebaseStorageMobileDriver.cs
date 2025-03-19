#if UNITY_EDITOR || !UNITY_WEBGL

using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class cxFirebaseStorageMobileDriver : cxIFirebaseStorageDriver {

    private string storageUrl;

    public cxFirebaseStorageMobileDriver (string storageUrl) {
        this.storageUrl = storageUrl;
    }

    public async Task<string> UploadImage (string path, string key, byte[] imageBytes) {
         try {
            var storage = FirebaseStorage.DefaultInstance;
            string fileName = key;
            StorageReference storageRef = storage.GetReferenceFromUrl (storageUrl).Child (path + "/" + key);

            var result = await storageRef.PutBytesAsync (imageBytes);
            var url = await storageRef.GetDownloadUrlAsync ();
            return url.ToString ();
        } catch (Exception ex) {
            Debug.LogException (ex);
            return null;
        }
    }
}

#endif