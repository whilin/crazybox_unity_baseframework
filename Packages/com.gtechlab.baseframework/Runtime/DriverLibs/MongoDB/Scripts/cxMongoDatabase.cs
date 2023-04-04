/**
 * Documentation of C# driver 1.10:
 * http://mongodb.github.io/mongo-csharp-driver/1.11/getting_started/
 * c# Driver for MongoDBprovided by http://answers.unity3d.com/questions/618708/unity-and-mongodb-saas.html
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UniRx;

using MongoDB.Bson;
using MongoDB.Driver;


public class cxMongoDocument
{
    public ObjectId _id;
}


public class cxMongoDatabase
{

    //private string connectionUrl;
    //private int port;
    private string databaseName;

    public bool isConnected { get; private set; }

    public MongoClient client { get; private set; }
    public IMongoDatabase database { get; private set; }

    public readonly Dictionary<string, object> collections = new Dictionary<string, object>();

    public static string GetConnectionString(string connectionUrl, int port)
    {
        return string.Format("mongodb://{0}:{1}", connectionUrl, port);
    }

    public async Task<bool> TryConnect(string connectionString, string dbName)
    {
        /*
        Unhandled log message:
        The GuidRepresentation for the reader is CSharpLegacy,
        which requires the binary sub type to be UuidLegacy, not UuidStandard.'. Use UnityEngine.TestTools.LogAssert.Expect
        */
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

        var task = Task.Run(() =>
        {
            string conn = connectionString;
            client = new MongoClient(conn);
            database = client.GetDatabase(dbName);

            var collectionList = database.ListCollections().ToList();

            isConnected = true;
            Debug.Log("Database Connected : " + connectionString);
            //Debug.LogFormat("Database Status: ok:{0} Error:{1}", state.Ok, state.ErrorMessage);
        });

        try
        {
            await task;
            return true;
        }
        catch (Exception ex)
        {
            ReportException(ex);
            return false;
        }
    }

    public void TryDisconnect()
    {
        
    }

    public async Task<cxMongoCollection<TDocumentClass>> GetCollection<TDocumentClass>(string collectionName) where TDocumentClass : cxMongoDocument
    {
        if (collections.ContainsKey(collectionName))
            return collections[collectionName] as cxMongoCollection<TDocumentClass>;
        else
        {
            var c = new cxMongoCollection<TDocumentClass>(this, collectionName);
            collections[collectionName] = c;

#if UNITY_EDITOR || UNITY_SERVER
          //  await c.CheckValidCollection();
#endif
            return c;
        }
    }


    //internal static void ReportCommandResult(CommandResult result)
    //{
    //    if (result.Code.HasValue)
    //    {
    //        Debug.Log("CommandResult Code:" + result.Code.Value);
    //    }
    //}

    internal static void ReportException(Exception ex)
    {
        Debug.LogException(ex);
    }
}
