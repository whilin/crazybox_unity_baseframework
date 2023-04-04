using System;
using System.Threading.Tasks;

interface cxIResourcePackageRepository {
    Task<cxResourceDescModel> UploadResourcePackage(string resourceId, string pkgFilePath);
}