#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB;
using MongoDB.Bson;
using UnityEngine;
//using m9Mongo;

public class MongoUnitTester : MonoBehaviour {

	public class PlayerInfo : cxMongoDocument {
		public int level;
		public string name;
		public int scores;
		public object email;
		public DateTime fix_date;
		public DateTime current_date;

		public DateTime date_utcNow;

	}

	public cxMongoDatabase testDB;

	cxMongoCollection<PlayerInfo> playerCollection;

	// Use this for initialization
	void Start () {

		//testDB.TryConnect();
		//playerCollection=testDB.GetCollection<PlayerInfo>("players");

	}

	[ContextMenu ("TestQuery")]
	void TestQuery () {
		//playerCollection.FindAllAsync(
		//		delegate(List<PlayerInfo> result)
		//		{
		//			foreach(var p in result)
		//			{
		//				string debug = JsonUtility.ToJson(p);
		//				Debug.Log(p._id.ToString());
		//				Debug.Log(debug);
		//			}
		//		},
		//		delegate()
		//		{
		//			Debug.Log("FindAllAsync  Failed");
		//		});
	}

	// Update is called once per frame
	void Update () {

	}
}

#endif