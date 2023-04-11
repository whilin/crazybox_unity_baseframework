using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class cxWebCommandLineParser {
    public static T GetCommandLineObject<T> (string startURL) {

        var queryStrings = ParseQueryString (startURL);

        if (queryStrings.TryGetValue ("decodedToken", out string decodedToken)) {
            byte[] byte64 = Convert.FromBase64String (DecodeSafeBase64(decodedToken));
            string json = Encoding.UTF8.GetString (byte64);

            var parameter = Newtonsoft.Json.JsonConvert.DeserializeObject<T> (json);
            return parameter;
        } else {
            JObject obj = JObject.FromObject (queryStrings);
            var paramter = obj.ToObject<T> ();
            return paramter;
        }

        throw new ArgumentException ("cxWebStartupParser Exception, paramter not found:" + startURL);
    }
    static string DecodeSafeBase64 (string url) {
        string incoming = url
            .Replace ('_', '/').Replace ('-', '+');
        switch (url.Length % 4) {
            case 2:
                incoming += "==";
                break;
            case 3:
                incoming += "=";
                break;
        }
        return incoming;
    }
    public static Dictionary<string, string> ParseQueryString (string startURL) {
        Debug.Log ("ParseQueryString url:" + startURL);

        string[] parts = startURL.Split (new char[] { '?', '&' });
        Dictionary<string, string> queryStrings = new Dictionary<string, string> ();

        for (int i = 1; i < parts.Length; i++) {
            string[] keyValue = parts[i].Split ('=');
            string values = string.Empty;
            for (int j = 1; j < keyValue.Length; j++)
                values += keyValue[j];

            queryStrings.Add (keyValue[0], values);
        }

        return queryStrings;
    }
}