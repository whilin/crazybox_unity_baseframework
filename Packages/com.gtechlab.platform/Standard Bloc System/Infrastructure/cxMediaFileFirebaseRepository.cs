
using System.Threading.Tasks;

public class cxMediaFileFirebaseRepository : IMediaFileRepository
{
    cxIFirebaseStorageDriver storageDriver;
    public cxMediaFileFirebaseRepository(cxIFirebaseStorageDriver storageDriver) {
        this.storageDriver = storageDriver;
    }

    public async Task<string> UploadImage(string repoPath, string filename, byte[] imageBytes)
    {
        return await storageDriver.UploadImage(repoPath, filename ,imageBytes);
    }
}