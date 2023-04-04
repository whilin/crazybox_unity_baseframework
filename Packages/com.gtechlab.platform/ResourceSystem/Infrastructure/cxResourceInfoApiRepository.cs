using System;
using System.Threading.Tasks;

class TGetResourceInfoRequest : cxNetPacket {
    public string resourceId;
}

class TGetResourceInfoResponse : cxNetPacket {
    public int resultCode;
    public cxResourceDescModel resourceDesc;
}

public class cxResourceInfoApiRepository : cxIResourceInfoRepository {

    cxNetDriver netDriver;

    public cxResourceInfoApiRepository (string serverUrl, string authToken) {
        netDriver = new cxNetDriver (serverUrl);
        if(!string.IsNullOrEmpty(authToken))
            netDriver.SetApiAuthToken(authToken);
    }

    public cxResourceInfoApiRepository (cxNetDriver netDriver) {
       this.netDriver = netDriver;
    }
        
    public async Task<cxResourceDescModel> FindResourceInfo(string resourceId) {
        TGetResourceInfoRequest req = new TGetResourceInfoRequest(){
            resourceId = resourceId
        };

        var res = await netDriver.RequestAsync<TGetResourceInfoResponse>(cxNetDriver.HTTP_METHOD.POST, "resource/GetResourceInfo", req);

        if(res.resultCode <= 0)
            throw new Exception("GetResourceInfo resultCode:"+res.resultCode);

        return res.resourceDesc;
    }


}