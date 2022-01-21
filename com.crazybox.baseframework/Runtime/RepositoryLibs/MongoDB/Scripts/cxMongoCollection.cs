using UnityEngine;
using System; //
using System.Collections;
using System.Collections.Generic;  // Lists

using MongoDB.Bson;
using MongoDB.Driver;
using UniRx;
using System.Threading.Tasks;


public class cxMongoCollection<TDocumentClass>  where TDocumentClass : cxMongoDocument  
{
    cxMongoDatabase dbManager;

    IMongoCollection<TDocumentClass> collection;

   
    string collectionName;

    public cxMongoCollection(cxMongoDatabase _dbManager, string _collectionName)
    {
        dbManager = _dbManager;
        collectionName = _collectionName;
        collection = dbManager.database.GetCollection<TDocumentClass>(collectionName);
    }

    //Note. 존재하지 않는다면 첫 사용시 자동으로 생성이됨!
    /*
    public async Task<bool> CheckValidCollection()
    {
        try
        {
            await dbManager.database.CreateCollectionAsync(collectionName);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return false;
    }
    */

    public async Task<long> Count(FilterDefinition<TDocumentClass> filter = null)
    { 
        long count = await collection.CountAsync(filter ?? new BsonDocument());
        return count;
    }

    public async Task<TDocumentClass> FindOne(FilterDefinition<TDocumentClass> filter = null)
    {
        var result = await collection.Find(filter ?? new BsonDocument()).FirstAsync();

        return result;
    }

    public async Task<List<TDocumentClass>> FindAll(FilterDefinition<TDocumentClass> filter = null )
    {
        var resultList = await collection.Find(filter ?? new BsonDocument()).ToListAsync();
        return resultList;
    }

    public async Task UpdateDoc(TDocumentClass doc)
    {
        var filter = Builders<TDocumentClass>.Filter.Eq( d => d._id , doc._id);

        ReplaceOneResult result = await collection.ReplaceOneAsync(filter, doc);
       
    }

    public async Task Upsert(TDocumentClass doc)
    {
        var filter = Builders<TDocumentClass>.Filter.Eq(q => q._id, doc._id);
        var options = new UpdateOptions { IsUpsert = true };
        ReplaceOneResult result = await collection.ReplaceOneAsync(filter, doc, options);
    }

    public async Task UploadDocuments(List<TDocumentClass> docs)
    {
        await collection.InsertManyAsync(docs);
    }

    public async Task InsertDocument(TDocumentClass docs)
    {
        await collection.InsertOneAsync(docs);
    }

    //internal static void ReportCommandResult(CommandResult result)
    //{
    //    if (result.Code.HasValue)
    //    {
    //        Debug.Log("CommandResult Code:" + result.Code.Value);
    //    }
    //}

    static void ReportWriteResult(WriteConcernResult result)
    {
        if (result.HasLastErrorMessage)
        {
            Debug.LogFormat("Result Error:{0}",
           result.LastErrorMessage );
        }
        else
        {
            Debug.LogFormat("Result Upserted:{0}, UpdatedExisting:{1}, DocumentsAffected:{2}",
                    result.Upserted,
                    result.UpdatedExisting,
                    result.DocumentsAffected          
           );
        }
    }

}
