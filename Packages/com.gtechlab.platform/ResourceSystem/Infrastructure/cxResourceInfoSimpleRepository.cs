
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class cxResourceInfoSimpleRepository : cxIResourceInfoRepository
{
    cxNetDriver netDriver;
    
    static string GetResouceInfoApi (string resourceId) {
        return $"{resourceId}/resource.json";
    }
    
    public cxResourceInfoSimpleRepository(string resourceLocationURL){
        netDriver = new cxNetDriver(resourceLocationURL);
    }
    
    public async Task<cxResourceDescModel> FindResourceInfo(string resourceId)
    {
        string resouceApi = GetResouceInfoApi(resourceId);
        string json = await netDriver.RequestGet(resouceApi);

        cxResourceDescModel desc = Newtonsoft.Json.JsonConvert.DeserializeObject<cxResourceDescModel>(json);
        return desc;
    }
}