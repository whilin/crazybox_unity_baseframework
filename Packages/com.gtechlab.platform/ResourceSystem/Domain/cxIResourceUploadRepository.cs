using System;
using System.Threading.Tasks;

interface cxIResourceUploadRepository {
    void UploadResourcePackage(string resourceId, string pkgFilePath, Action<cxResourceDescModel> callback, Action<string> onError);
}