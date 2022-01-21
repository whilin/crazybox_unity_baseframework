#define USE_BSON_TYPE

using UnityEngine;
using System.Collections;
//using GameDB;
//using Noriatti;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

#if !USE_BSON_TYPE
public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    protected static SQLiteDB db = null;
    string dbKey = "rjsaos!@#";
    const string dbTemplateName = "ProjectD.db";
    //const string dbPersistentName = "ProjectDv2.db";
    const string dbPersistentName = "gunmantheduel_v1.db";
    

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(CreateDatabase());
    }

    IEnumerator CreateDatabase()
    {
        Logger.Log("CreateDatabase", "DB");

        string filename = Application.persistentDataPath + "/"+dbPersistentName;

        if (!File.Exists(filename))
        {
            string dbfilename = dbTemplateName;
            byte[] bytes = null;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            string dbpath = "file://" + Application.streamingAssetsPath + "/" + dbfilename;
            WWW www = new WWW(dbpath);
            yield return www;
            bytes = www.bytes;
#elif UNITY_IPHONE
			string dbpath = Application.dataPath + "/Raw/" + dbfilename;
            try
            {
			    using ( FileStream fs = new FileStream(dbpath, FileMode.Open, FileAccess.Read, FileShare.Read) )
			    {
				    bytes = new byte[fs.Length];
				    fs.Read(bytes,0,(int)fs.Length);
			    }
			}
			catch (Exception e)
			{
				Debug.Log("DB Create failed :" + e.ToString());
			}
#elif UNITY_ANDROID
			string dbpath = Application.streamingAssetsPath + "/" + dbfilename;	            
			WWW www = new WWW(dbpath);
            yield return www;
			bytes = www.bytes;
#endif
            if (bytes != null)
            {
                try
                {
                    //Debug.Log("DB Create 0");

                    using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        //Debug.Log("DB Create 1");
                        fs.Write(bytes, 0, bytes.Length);
                    }

                    db = new SQLiteDB();
                    db.Open(filename);
                    db.Rekey(dbKey);
                    
                    Logger.Log("DB Create Successfully :" + filename, "DB");
                }
                catch (Exception e)
                {
                    Logger.Log("DB Create failed :" + e.ToString(), "DB");
                }
            }
            else
            {
                Logger.Log("DB Template-file open failed :" + dbpath, "DB");
            }
        }
        else
        {
            Logger.Log("DB Already Exist :" + filename, "DB");
        }

        yield break;
    }

    IEnumerator DataBaseConnect()
    {
        string filename = Application.persistentDataPath + "/"+dbPersistentName;
        if (!File.Exists(filename))
        {
            yield break;
        }
        else
        {
            db = new SQLiteDB();
            db.Open(filename);
            db.Key(dbKey);
            Debug.Log("DB DataBaseConnect :" + filename);
        }
    }

    [ContexMenu("ClearDatabase")]
    public void ClearDatabase()
    {
        Logger.Log("ClearDatabase", "DB");

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            SQLiteQuery qr;

            string querySelect = "DELETE FROM SavedData";
            qr = new SQLiteQuery(db, querySelect);
            qr.Step();
            qr.Release();
        }
    }

    void CreateTable()
    {
       SQLiteQuery qr;
       string querySelect = "CREATE TABLE SavedData(section TEXT, jsonData TEXT);";
       qr = new SQLiteQuery(db, querySelect);
       qr.Step();
       qr.Release();
    }

    string LoadSection(string section)
    {
        string jsonData = null;

        Logger.Verbose("LoadSection :" + section, "DB");

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            SQLiteQuery qr;
            string querySelect = "SELECT jsonData FROM SavedData WHERE section=?;";
            qr = new SQLiteQuery(db, querySelect);
            qr.Bind(section);

            if (qr.Step())
            {
                jsonData = qr.GetString("jsonData");
                
                Logger.Log("Database, Section Found :" + section);
                //Logger.Verbose("Database, Section Found :" + section + ", jsonData:" + jsonData);
            }
            else
            {
                Logger.Log("Database, Section Not Found :" + section);
            }

            qr.Release();
            db.Close();
        }

        return jsonData;
    }

    bool SaveSection(string section, string jsonData)
    {
        bool ret = false;

        Logger.Verbose("SaveSection :" + section, "DB");

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            int exist = 0;

            SQLiteQuery qr;
            string querySelect = "SELECT COUNT(*) as Exist FROM SavedData WHERE section=?;";

            qr = new SQLiteQuery(db, querySelect);
            qr.Bind(section);
            if (qr.Step())
                exist = qr.GetInteger("Exist");
            qr.Release();

            if (exist != 0)
            {
                querySelect = "UPDATE SavedData SET jsonData=? WHERE section=?;";
                qr = new SQLiteQuery(db, querySelect);
                qr.Bind(jsonData);
                qr.Bind(section);

                qr.Step();

                ret = true;

                qr.Release();
            }
            else
            {
                querySelect = "INSERT INTO SavedData(section,jsonData) VALUES(?,?);";
                qr = new SQLiteQuery(db, querySelect);
                qr.Bind(section);
                qr.Bind(jsonData);
                

                qr.Step();

                ret = true;

                qr.Release();
            }

            db.Close();
        }

        return ret;
    }

    ////////////////////////////////////////////////////

    public bool SaveSectionObject<T>(string section, T saveObject) where T : class
    {
        bool ret = false;

        //string jsonData = LitJson.JsonMapper.ToJson(saveObject);
        //string jsonData = JsonFx.Json.JsonWriter.Serialize(saveObject);
        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(saveObject);

        if (!string.IsNullOrEmpty(jsonData))
            ret = SaveSection(section, jsonData);

        return ret;
    }

    public bool LoadSectionObject<T>(string section, out T savedObject) where T : class
    {
        bool ret = false;
        string jsonData = LoadSection(section);

        if (!string.IsNullOrEmpty(jsonData))
        {
            //savedObject = LitJson.JsonMapper.ToObject<T>(jsonData);
            //savedObject = JsonFx.Json.JsonReader.Deserialize<T>(jsonData);
            savedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
            ret = true;
        }
        else
        {
            savedObject = default(T);
            ret = false;
        }

        return ret;
    }

    public class ListObject<T>
    {
        public List<T> listitems;
    }

    public bool LoadSectionObjectList<T>(string section, out List<T> list)
    {
        string jsonData = LoadSection(section);

        if (!string.IsNullOrEmpty(jsonData))
        {
            //ListObject<T> listObj=JsonFx.Json.JsonReader.Deserialize<ListObject<T>>(jsonData);

            //ListObject<T> listObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ListObject<T>>(jsonData);
            ListObject<T> listObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ListObject<T>>(jsonData);

            list = listObj.listitems;

            return true;
        }
        else
        {
            list = null;
            return false;
        }
    }

    public bool SaveSectionObjectList<T>(string section, List<T> list)
    {
        bool ret = false;

        //ios 오류 코드
        /*
        ListObject<T> listobj = new ListObject<T>();
        listobj.listitems = list;
        string jsonData = JsonFx.Json.JsonWriter.Serialize(listobj);
        */

        /* 이것도 ios aot 오류
        //새로운 빌드 코드
        System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder(1024);

        jsonBuilder.Append("{ \"listitems\" : [");
        for (int i = 0; i < list.Count; i++)
        {
            string objJson = JsonFx.Json.JsonWriter.Serialize(list[i]);
            jsonBuilder.Append(objJson);
            if (i != list.Count - 1)
                jsonBuilder.Append(",");
        }
        jsonBuilder.Append("] }");
        string jsonData = jsonBuilder.ToString();
        */

        ListObject<T> listobj = new ListObject<T>();
        listobj.listitems = list;
        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(listobj);

        if (!string.IsNullOrEmpty(jsonData))
            ret = SaveSection(section, jsonData);

        return ret;
    }

    /*
    ////// DataStore 형태
    public bool LoadSectionDS(string section, DataStore ds)
    {
        string jsonData = LoadSection(section);

        if (!string.IsNullOrEmpty(jsonData))
        {
            ds.ImportFromJSON(jsonData);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SaveSectionDS(string section, DataStore ds)
    {
        bool ret = false;

        string jsonData = ds.ExportToJSON();
        if (!string.IsNullOrEmpty(jsonData))
            ret = SaveSection(section, jsonData);

        return ret;
    }

    public bool LoadSectionDSList<T>(string section, out List<T> list) where T : NoriattiObejct
    {
        string jsonData = LoadSection(section);

        if (!string.IsNullOrEmpty(jsonData))
        {
            DataStore parent = new DataStore();
            parent.ImportFromJSON(jsonData);

            list = DataParser.ParseList<T>(parent["listitems"]);

            return true;
        }
        else
        {
            list = null;
            return false;
        }
    }

    public bool SaveSectionDSList<T>(string section, List<T> list) where T : NoriattiObejct
    {
        bool ret = false;

        System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder(1024);

        jsonBuilder.Append("{ \"listitems\" : [");
        for (int i = 0; i < list.Count; i++)
        {
            string objJson=list[i].GetSrc().ExportToJSON();
            jsonBuilder.Append(objJson);
            if(i != list.Count -1)
                jsonBuilder.Append(",");
        }

        jsonBuilder.Append("] }");

        string jsonData = jsonBuilder.ToString();

        if (!string.IsNullOrEmpty(jsonData))
            ret = SaveSection(section, jsonData);

        return ret;
    }
    */

    byte[] EncryptObjectToDB(object objectBody)
    {
        byte[] bsonData = new byte[] { };
        using (var stream = new MemoryStream())
        {
            using (BsonWriter writer = new BsonWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, objectBody);
                bsonData = stream.ToArray();
            }
        }

        byte[] compressData = CLZF2.Compress(bsonData);
        byte[] encryptData = CLZF2.Encrypt(compressData);

        return encryptData;
    }

    T DecryptDbToObject<T> (byte[] encryptDb)
    {
        T obj;
        var decryptData = CLZF2.Decrypt(encryptDb);
        var decompressData = CLZF2.Decompress(decryptData);

        using (var stream = new MemoryStream(decompressData))
        {
            using (BsonReader reader = new BsonReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                obj = serializer.Deserialize<T>(reader);
            }
        }

        return obj;
    }


    public void UnitTest_1()
    {
        DateTime begin = DateTime.Now;

        for (int i = 0; i < 1000; i++)
        {            
            SaveSectionObject<PlayRecord>("test", RecordManager.Instance.scoreRecord);
        }

        for (int i = 0; i < 1000; i++)
        {
            PlayRecord p;
            LoadSectionObject<PlayRecord>("test", out p);
        }

        DateTime end = DateTime.Now;

        TimeSpan span= end - begin;
        
        double time= span.TotalMilliseconds;

        Debug.LogError("UnitTest_1:" + time);
    }

}
#else

public class DatabaseManager : MonoSingleton<DatabaseManager>
{
    protected static SQLiteDB db = null;
    string dbKey = "rjsaos!@#";
    const string dbTemplateName = "DBTemplate.db";
    public string dbPersistentName = "gunmantheduel_v2.db";

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(CreateDatabase());
    }

    IEnumerator CreateDatabase()
    {
        Debug.Log("[DB] CreateDatabase");

        string filename = Application.persistentDataPath + "/" + dbPersistentName;

        if (!File.Exists(filename))
        {
            string dbfilename = dbTemplateName;
            byte[] bytes = null;
#if UNITY_WEBGL || UNITY_SERVER
            string dbpath = "";
            throw new Exception("cxSQLDatabase not supported in webgl");
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            string dbpath = "file://" + Application.streamingAssetsPath + "/" + dbfilename;
            WWW www = new WWW(dbpath);
            yield return www;
            bytes = www.bytes;
#elif UNITY_IPHONE
			string dbpath = Application.dataPath + "/Raw/" + dbfilename;
            try
            {
			    using ( FileStream fs = new FileStream(dbpath, FileMode.Open, FileAccess.Read, FileShare.Read) )
			    {
				    bytes = new byte[fs.Length];
				    fs.Read(bytes,0,(int)fs.Length);
			    }
			}
			catch (Exception e)
			{
				Debug.Log("[DB] Create failed :" + e.ToString());
			}
#elif UNITY_ANDROID
			string dbpath = Application.streamingAssetsPath + "/" + dbfilename;	            
			WWW www = new WWW(dbpath);
            yield return www;
			bytes = www.bytes;
#endif
            if (bytes != null)
            {
                try
                {
                    //Debug.Log("DB Create 0");

                    using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        //Debug.Log("DB Create 1");
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    
                    // db = new SQLiteDB();
                    // db.Open(filename);
                    //  db.Rekey(dbKey);

                    //CreateTable();

                    Debug.Log("[DB] Create Successfully :" + filename);
                }
                catch (Exception e)
                {
                    Debug.Log("[DB] Create failed :" + e.ToString());
                }
            }
            else
            {
                Debug.Log("[DB] Template-file open failed :" + dbpath);
            }
        }
        else
        {
            Debug.Log("[DB] Already Exist :" + filename);
        }

        yield break;
    }

    IEnumerator DataBaseConnect()
    {
        string filename = Application.persistentDataPath + "/" + dbPersistentName;
        if (!File.Exists(filename))
        {
            yield break;
        }
        else
        {
            db = new SQLiteDB();
            db.Open(filename);
           // db.Key(dbKey);
            Debug.Log("DB DataBaseConnect :" + filename);
        }
    }

    [ContextMenu("ClearDatabase")]
    public void ClearDatabase()
    {
       Debug.Log("[DB] ClearDatabase");

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            SQLiteQuery qr;

            string querySelect = "DELETE FROM SavedData";
            qr = new SQLiteQuery(db, querySelect);
            qr.Step();
            qr.Release();
        }
    }

    void CreateTable()
    {
        SQLiteQuery qr;
        string querySelect = "CREATE TABLE SavedData(section TEXT, jsonData BLOB);";
        qr = new SQLiteQuery(db, querySelect);
        qr.Step();
        qr.Release();
    }

    byte [] LoadSection(string section)
    {
        byte [] jsonData = null;

        Debug.Log("[DB] LoadSection :" + section);

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            SQLiteQuery qr;
            string querySelect = "SELECT jsonData FROM SavedData WHERE section=?;";
            qr = new SQLiteQuery(db, querySelect);
            qr.Bind(section);

            if (qr.Step())
            {
                //jsonData = qr.GetString("jsonData");
                jsonData = qr.GetBlob("jsonData");

                 Debug.Log("[DB] Database, Section Found :" + section);
             }
            else
            {
                 Debug.Log("[DB] Database, Section Not Found :" + section);
            }

            qr.Release();
            db.Close();
        }

        return jsonData;
    }

    bool SaveSection(string section, byte [] jsonData)
    {
        bool ret = false;

        Debug.Log("[DB] SaveSection :" + section);

        StartCoroutine(DataBaseConnect());
        if (db != null)
        {
            int exist = 0;

            SQLiteQuery qr;
            string querySelect = "SELECT COUNT(*) as Exist FROM SavedData WHERE section=?;";

            qr = new SQLiteQuery(db, querySelect);
            qr.Bind(section);
            if (qr.Step())
                exist = qr.GetInteger("Exist");
            qr.Release();

            if (exist != 0)
            {
                querySelect = "UPDATE SavedData SET jsonData=? WHERE section=?;";
                qr = new SQLiteQuery(db, querySelect);
                qr.Bind(jsonData);
                qr.Bind(section);

                qr.Step();

                ret = true;

                qr.Release();
            }
            else
            {
                querySelect = "INSERT INTO SavedData(section,jsonData) VALUES(?,?);";
                qr = new SQLiteQuery(db, querySelect);
                qr.Bind(section);
                qr.Bind(jsonData);


                qr.Step();

                ret = true;

                qr.Release();
            }

            db.Close();
        }

        return ret;
    }

    ////////////////////////////////////////////////////

    public bool SaveSectionObject<T>(string section, T saveObject) where T : class
    {
        bool ret = false;

        // string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(saveObject);
        byte[] jsonData = EncryptObjectToDB(saveObject);

        //if (!string.IsNullOrEmpty(jsonData))
        if(jsonData !=null)
            ret = SaveSection(section, jsonData);

        return ret;
    }

    public bool LoadSectionObject<T>(string section, out T savedObject) where T : class
    {
        bool ret = false;
        byte [] jsonData = LoadSection(section);

        if (jsonData !=null)
        {
            //savedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData); 
            savedObject = DecryptDbToObject<T>(jsonData);
            
            ret = true;
        }
        else
        {
            savedObject = default(T);
            ret = false;
        }

        return ret;
    }

    public class ListObject<T>
    {
        public List<T> listitems;
    }

    public bool LoadSectionObjectList<T>(string section, out List<T> list)
    {
        //string jsonData = LoadSection(section);
        byte[] jsonData = LoadSection(section);

        if (jsonData !=null)
        {
            //ListObject<T> listObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ListObject<T>>(jsonData);
            ListObject<T> listObj = DecryptDbToObject<ListObject<T>>(jsonData);

            list = listObj.listitems;

            return true;
        }
        else
        {
            list = null;
            return false;
        }
    }

    public bool SaveSectionObjectList<T>(string section, List<T> list)
    {
        bool ret = false;

        ListObject<T> listobj = new ListObject<T>();
        listobj.listitems = list;

        //string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(listobj);
        byte[] jsonData = EncryptObjectToDB(listobj);

        if (jsonData !=null)
            ret = SaveSection(section, jsonData);

        return ret;
    }


    byte[] EncryptObjectToDB(object objectBody)
    {
        byte[] bsonData = new byte[] { };
        using (var stream = new MemoryStream())
        {
            using (BsonWriter writer = new BsonWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, objectBody);
                bsonData = stream.ToArray();
            }
        }

        byte[] compressData = CLZF2.Compress(bsonData);
        byte[] encryptData = CLZF2.Encrypt(compressData);

        return encryptData;
    }

    T DecryptDbToObject<T>(byte[] encryptDb)
    {
        T obj;
        var decryptData = CLZF2.Decrypt(encryptDb);
        var decompressData = CLZF2.Decompress(decryptData);

        using (var stream = new MemoryStream(decompressData))
        {
            using (BsonReader reader = new BsonReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                obj = serializer.Deserialize<T>(reader);
            }
        }

        return obj;
    }

    /*
    public void UnitTest_1()
    {
        DateTime begin = DateTime.Now;

        for (int i = 0; i < 1000; i++)
        {            
            SaveSectionObject<PlayRecord>("test", RecordManager.Instance.playRecord);
        }

        for (int i = 0; i < 1000; i++)
        {
            PlayRecord p;
            LoadSectionObject<PlayRecord>("test", out p);
        }

        DateTime end = DateTime.Now;

        TimeSpan span= end - begin;
        
        double time= span.TotalMilliseconds;

        Debug.LogError("UnitTest_1:" + time);
    }
    */
}
#endif
