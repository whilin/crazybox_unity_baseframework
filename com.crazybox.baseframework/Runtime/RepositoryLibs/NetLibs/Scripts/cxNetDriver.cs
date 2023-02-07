using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class cxNetFormFile {
    public string Name { get; set; }
    public string ContentType { get; set; }
    public string FilePath { get; set; }
    public Stream Stream { get; set; }
}

public class cxNetFormFields {
    public List<KeyValuePair<string, object>> fields { get; private set; }

    public cxNetFormFields () {
        fields = new List<KeyValuePair<string, object>> ();
    }
    public void Add (string key, string data) {
        fields.Add (new KeyValuePair<string, object> (key, data));
    }
    public void Add (string key, cxNetFormFile file) {
        fields.Add (new KeyValuePair<string, object> (key, file));
    }
}

public class cxNetDriver {

    public enum HTTP_METHOD {
        GET,
        POST,
        PUT,
        DELETE,
    }

    private String ApiEndPoint = "";
    private String jwt;
    private bool verbose = false;

    public cxNetDriver (String apiEndPoint, bool _verbose = false) {
        ApiEndPoint =apiEndPoint.EndsWith("/") ? apiEndPoint.Remove(apiEndPoint.Length -1) : apiEndPoint;
        verbose = _verbose;
    }

    public void SetApiAuthToken (string _jwt) {
        jwt = _jwt;
    }

    protected String GetURL (String apiName) {
        return ApiEndPoint + ( apiName.StartsWith("/") ? apiName : "/"+apiName);
    }

    public async void Request<ResT> (String apiName, object reqPacket, Action<NetRequestState, ResT> callback) where ResT : cxNetPacket {

        callback (NetRequestState.Pending, null);

        try {
            var res = await RequestAsync<ResT> (HTTP_METHOD.GET, apiName, reqPacket);
            callback (NetRequestState.Completed, res);
        } catch (Exception ex) {
            callback (NetRequestState.Error, null);
        }
    }

    public async Task<ResT> RequestAsync<ResT> (HTTP_METHOD method, String apiName, object reqPacket) where ResT : cxNetPacket {
        String json = Newtonsoft.Json.JsonConvert.SerializeObject (reqPacket);

        String url = GetURL(apiName);

        UnityWebRequest request = new UnityWebRequest (url, method.ToString ());

        if (!string.IsNullOrEmpty (jwt))
            request.SetRequestHeader ("Authorization", "Bearer " + jwt);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes (json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw (bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer ();
        request.uploadHandler.contentType = "application/json";

        if (verbose)
            Debug.Log ("SendNetRequest Call:" + apiName + ", body:" + json);

        await request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            throw new Exception ("ConnectionError");
        } else if (request.result == UnityWebRequest.Result.ProtocolError) {
            string msg = request.downloadHandler.text ?? string.Empty;
            Debug.Log ($"RequestAsync Error:{request.error}, msg:{msg}");
            throw new Exception (request.error + ":" + msg);
        } else {
            if (verbose)
                Debug.LogFormat ("SendNetRequest Response:{0}, Document:{1}", request.responseCode, request.downloadHandler.text);

            String res = request.downloadHandler.text;

            ResT respond = Newtonsoft.Json.JsonConvert.DeserializeObject<ResT> (res);
            return respond;
        }
    }

    public async Task<Texture2D> FetchImage (String apiName, object reqPacket) {
        try {
            var result = await _SendDownloadRequest (HTTP_METHOD.GET, apiName, reqPacket);
            return result;
            //callback (NetRequestState.Completed, result);
        } catch (Exception ex) {
            // callback (NetRequestState.Error, null);
            throw ex;
        }
    }

    protected IObservable<Texture2D> FetchImage2 (String apiName, object reqPacket) {

        var observer = Observable.Create<Texture2D> ((ob) => {
            var task = _SendDownloadRequest (HTTP_METHOD.GET, apiName, reqPacket);
            task.ContinueWith ((t) => {
                if (t.IsFaulted)
                    ob.OnError (t.Exception);
                else
                    ob.OnNext (t.Result);

                ob.OnCompleted ();
            });

            return task;
        });

        return observer;
    }

    protected async Task<Texture2D> _SendDownloadRequest (HTTP_METHOD method, String apiName, object reqPacket) {
        String json = Newtonsoft.Json.JsonConvert.SerializeObject (reqPacket);

        String url =  GetURL(apiName);

        UnityWebRequest request = new UnityWebRequest (url, method.ToString ());
        if (!string.IsNullOrEmpty (jwt))
            request.SetRequestHeader ("Authorization", "Bearer " + jwt);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes (json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw (bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerTexture ();
        request.uploadHandler.contentType = "application/json";
        // request.SetRequestHeader ("Content-Type", "application/json");

        if (verbose)
            Debug.Log ("SendNetRequest Call:" + apiName);

        await request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError) {

            Debug.Log ("SendNetRequest Error:" + request.error);
            throw new Exception ("SendNetRequest Error:" + request.error);
        } else {
            if (verbose) {
                Debug.LogFormat ("SendNetRequest Response:{0}, Document:{1}", request.responseCode, request.downloadHandler.text);
                Debug.Log ("_GetPoseImage Response:" + request.responseCode);
            }

            var downloadTex = DownloadHandlerTexture.GetContent (request);
            return downloadTex;
        }
    }

    public async Task<String> RequestGet (String fileaname) {
        String url =  GetURL(fileaname);

        UnityWebRequest request = new UnityWebRequest (url, HTTP_METHOD.GET.ToString ());
        if (!string.IsNullOrEmpty (jwt))
            request.SetRequestHeader ("Authorization", "Bearer " + jwt);

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = (DownloadHandler)new Download();
        //request.uploadHandler.contentType = "application/json";
        // request.SetRequestHeader ("Content-Type", "application/json");

        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer ();
        //request.downloadHandler.a = "application/json";

        if (verbose)
            Debug.Log ("RequestGet Call:" + url);

        await request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log ("RequestGet Error:" + request.error);
            throw new Exception ("RequestGet Error:" + request.error);
            //return null;
        } else {
            if (verbose) {
                Debug.LogFormat ("RequestGet Response:{0}, Document:{1}", request.responseCode, request.downloadHandler.text);
                Debug.Log ("RequestGet Response:" + request.responseCode);
            }
            // var downloadTex = DownloadHandlerTexture.GetContent(request);
            // callback (NetRequestState.Completed, downloadTex);
            return request.downloadHandler.text;
        }
    }

    public async Task<ResT> PostMultipartNot<ResT> (string apiName, cxNetFormFields parameters) where ResT : cxNetPacket {
        String url = GetURL(apiName);

        if (verbose)
            Debug.Log ("RequestGet Call:" + url);

        var task = new Task<ResT> (() => {

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString ("x");
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create (url);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            if (parameters != null && parameters.fields.Count > 0) {
                using (Stream requestStream = request.GetRequestStream ()) {

                    foreach (KeyValuePair<string, object> pair in parameters.fields) {

                        requestStream.Write (boundaryBytes, 0, boundaryBytes.Length);
                        if (pair.Value is cxNetFormFile) {
                            cxNetFormFile file = pair.Value as cxNetFormFile;
                            string header = "Content-Disposition: form-data; name=\"" + pair.Key + "\"; filename=\"" + file.Name + "\"\r\nContent-Type: " + file.ContentType + "\r\n\r\n";
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes (header);
                            requestStream.Write (bytes, 0, bytes.Length);
                            byte[] buffer = new byte[32768];
                            int bytesRead;
                            if (file.Stream == null) {
                                // upload from file
                                using (FileStream fileStream = File.OpenRead (file.FilePath)) {
                                    while ((bytesRead = fileStream.Read (buffer, 0, buffer.Length)) != 0)
                                        requestStream.Write (buffer, 0, bytesRead);
                                    fileStream.Close ();
                                }
                            } else {
                                // upload from given stream
                                while ((bytesRead = file.Stream.Read (buffer, 0, buffer.Length)) != 0)
                                    requestStream.Write (buffer, 0, bytesRead);
                            }
                        } else {
                            string data = "Content-Disposition: form-data; name=\"" + pair.Key + "\"\r\n\r\n" + pair.Value;
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes (data);
                            requestStream.Write (bytes, 0, bytes.Length);
                        }
                    }

                    byte[] trailer = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "--\r\n");
                    requestStream.Write (trailer, 0, trailer.Length);
                    requestStream.Close ();
                }
            }

            string jsonText = string.Empty;

            using (WebResponse response = request.GetResponse ()) {
                using (Stream responseStream = response.GetResponseStream ())
                using (StreamReader reader = new StreamReader (responseStream))
                jsonText = reader.ReadToEnd ();
            }

            ResT respond = Newtonsoft.Json.JsonConvert.DeserializeObject<ResT> (jsonText);
            return respond;
        });

        task.Start ();

        return await task;
    }

    public async Task<ResT> PostMultipart<ResT> (string apiName, cxNetFormFields parameters) {
        String url = GetURL(apiName);

        List<IMultipartFormSection> requestData = new List<IMultipartFormSection> ();
        foreach (KeyValuePair<string, object> pair in parameters.fields) {
            IMultipartFormSection part = null;
            if (pair.Value is cxNetFormFile) {

                var file = (cxNetFormFile) pair.Value;

                MemoryStream byteStream = new MemoryStream ();

                byte[] buffer = new byte[32768];
                int bytesRead;
                if (file.Stream == null) {
                    // upload from file
                    using (FileStream fileStream = File.OpenRead (file.FilePath)) {
                        while ((bytesRead = fileStream.Read (buffer, 0, buffer.Length)) != 0)
                            byteStream.Write (buffer, 0, bytesRead);
                        fileStream.Close ();
                    }
                } else {
                    // upload from given stream
                    while ((bytesRead = file.Stream.Read (buffer, 0, buffer.Length)) != 0)
                        byteStream.Write (buffer, 0, bytesRead);
                }

                byte[] bytes = byteStream.ToArray ();
                byteStream.Close ();

                part = new MultipartFormFileSection (
                    pair.Key,
                    bytes,
                    file.Name,
                    file.ContentType
                );
            } else {
                part = new MultipartFormDataSection (pair.Key, pair.Value.ToString ());
            }

            requestData.Add (part);
        }

        if (verbose)
            Debug.Log ("PostMultipart Call:" + apiName);

        UnityWebRequest request = UnityWebRequest.Post (url, requestData);
        if (!string.IsNullOrEmpty (jwt))
            request.SetRequestHeader ("Authorization", "Bearer " + jwt);

        await request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            throw new Exception ("ConnectionError");
        } else if (request.result == UnityWebRequest.Result.ProtocolError) {
            string msg = request.downloadHandler.text ?? string.Empty;
            Debug.Log ($"RequestAsync Error:{request.error} msg:{msg}");
            throw new Exception (request.error + ":" + msg);
        } else {
            if (verbose)
                Debug.LogFormat ("PostMultipart Response:{0}, Document:{1}", request.responseCode, request.downloadHandler.text);

            String res = request.downloadHandler.text;

            ResT respond = Newtonsoft.Json.JsonConvert.DeserializeObject<ResT> (res);
            return respond;
        }
    }

}