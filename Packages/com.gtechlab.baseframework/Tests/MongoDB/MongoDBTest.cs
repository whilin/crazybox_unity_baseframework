using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

public class MongoDBTest
{
    public class TestDoc : cxMongoDocument
    {
        public string content;
        public int myValue;
        public DateTime dateTime;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MongoDBConnectionTest()
    {
        yield return null;
        yield return RunMongo().ToObservable().ToYieldInstruction();
    }

    [UnityTest]
    public IEnumerator MongoDBReadTest()
    {
        yield return null;
        yield return ReadMongo().ToObservable().ToYieldInstruction();
    }

    [UnityTest]
    public IEnumerator MongoDBQueryTest()
    {
        yield return null;
        yield return QueryMongo().ToObservable().ToYieldInstruction();
    }

    async Task RunMongo()
    {
        cxMongoDatabase database = new cxMongoDatabase();

        var connStr = "mongodb://adminMetaverse111:metaverse12345@13.125.167.222:27017/admin?authSource=admin";

        await database.TryConnect(connStr, "Test");

        var collection  = await database.GetCollection<TestDoc>("TestCollection");

       // collection.CheckValidCollection();

        TestDoc testDoc = new TestDoc()
        {
            content = "my test",
            myValue = 10
        };

        await collection.Upsert(testDoc);
    }

    async Task ReadMongo()
    {
        var collection = await ConnectMongo();

        var doc = await collection.FindOne();
        doc.content = "my new content";
        doc.myValue = 100;

        await collection.UpdateDoc(doc);

        TestDoc testDoc = new TestDoc()
        {
            content = "my test 1000",
            myValue = 10
        };
        await collection.UpdateDoc(testDoc);

        
        
        // Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(docs));
    }

    async Task QueryMongo()
    {
        var collection = await ConnectMongo();

        var filter=Builders<TestDoc>.Filter.Eq(q => q.myValue, 100);
         var update = Builders<TestDoc>.Update.Set(q => q.content, "변경합니다.");

        var doc = await collection.FindOne(filter);
        doc.content = "변경합니다.";
        await collection.UpdateDoc(doc);

    }

    async Task<cxMongoCollection<TestDoc>> ConnectMongo()
    {
        cxMongoDatabase database = new cxMongoDatabase();

        var connStr = "mongodb://adminMetaverse:metaverse12345@13.125.167.222:27017/admin?authSource=admin";

        await database.TryConnect(connStr, "Test");
        var collection = await database.GetCollection<TestDoc>("TestCollection");

        return collection;
    }
}
