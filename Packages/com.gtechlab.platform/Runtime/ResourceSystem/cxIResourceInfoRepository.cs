using System;
using System.Threading.Tasks;

public interface cxIResourceInfoRepository {
    Task<cxResourceDescModel> FindResourceInfo(string resourceId);
}