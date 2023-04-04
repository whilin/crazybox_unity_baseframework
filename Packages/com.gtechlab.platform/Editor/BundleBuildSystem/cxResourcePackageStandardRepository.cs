using System;
using System.Threading.Tasks;

class TUploadResourcePackageResponse : cxNetPacket {
    public int resultCode;
    public cxResourceDescModel resourceDesc;
}

public class cxResourcePackageStandardRepository {

    cxNetEditorDriver netDriver;

    public cxResourcePackageStandardRepository (string serverUrl, string authToken) {
        netDriver = new cxNetEditorDriver (serverUrl);
        if(!string.IsNullOrEmpty(authToken))
            netDriver.SetApiAuthToken(authToken);
    }

    public void UploadResourcePackage (string resourceId, string pkgFilePath, Action<cxResourceDescModel> callback, Action<string> onError) {
        cxNetFormFields netFormFields = new cxNetFormFields ();
        netFormFields.Add ("resourceId", resourceId);
        netFormFields.Add ("file", new cxNetFormFile () {
            FilePath = pkgFilePath,
                Name = resourceId,
                ContentType = "application/zip"
        });

        netDriver.PostMultipartCallback<TUploadResourcePackageResponse> ("/resource/UploadResourcePackage", netFormFields, (res) => {
            if (res.resultCode > 0) {
                callback (res.resourceDesc);
            } else {
                onError ($"[{resourceId}] Resource Package Upload Failed Code:{res.resultCode} - " + pkgFilePath);
            }
        }, (error) => {
            onError (error);
        });
    }
}