using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

public class cxDatabaseDriver
{
    const string dbTemplateName = "DBTemplate.db";

    private string dbKey = "rjsaos!@#";
    private string dbPersistentName = "gunmantheduel_v2.db";

    protected static SQLiteDB db = null;

    public cxDatabaseDriver()
    {

    }

    public async Task OpenDatabase(string dbName, string dbKey)
    {
        this.dbPersistentName = dbName;
        this.dbKey = dbKey;

        await CreateDatabase();
        await ConnectDatabase();

        CreateTable();

        Debug.Log("cxSQLDatabase:OpenDatabase Sucess");
    }

    async Task CreateDatabase()
    {
        Debug.Log("cxSQLDatabase:CreateDatabase");

        string filename = Application.persistentDataPath + "/" + dbPersistentName;

        if (!File.Exists(filename))
        {
            string dbfilename = dbTemplateName;

            byte[] bytes = null;
#if UNITY_WEBGL || UNITY_SERVER
            string dbpath = "";
            throw new Exception("cxSQLDatabase not supported in webgl");

#elif UNITY_IPHONE && !UNITY_EDITOR
            string dbpath =  Path.Combine(Application.streamingAssetsPath, dbfilename);
			//string dbpath = Application.dataPath + "/Raw/" + dbfilename;
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
#elif UNITY_ANDROID && !UNITY_EDITOR
			string dbpath = Application.streamingAssetsPath + "/" + dbfilename;	            
			WWW www = new WWW(dbpath);
            await www;
			bytes = www.bytes;

#else //#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || 
            string dbpath = "file://" + Application.streamingAssetsPath + "/" + dbfilename;
            WWW www = new WWW(dbpath);
            await www;

            bytes = www.bytes;
#endif
            if (bytes != null)
            {
                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }

                    Debug.LogFormat("DB Create Successfully :" + filename);
                }
                catch (Exception e)
                {
                    throw new Exception("DB Create failed :" + e.ToString());
                }
            }
            else
            {
                throw new Exception("DB Template-file open failed :" + dbpath);
            }
        }
    }

    async Task ConnectDatabase()
    {
        string filename = Application.persistentDataPath + "/" + dbPersistentName;
        if (!File.Exists(filename))
        {
            throw new Exception("Database File Not Found:"+filename);
        }
        else
        {
            db = new SQLiteDB();
            db.Open(filename);
            Debug.Log("DB DataBaseConnect :" + filename);
        }
    }

    void CloseDatabase()
    {
        if(db != null)
        {
            db.Close();
            db = null;
        }
    }

    [ContextMenu("ClearDatabase")]
    public void ClearDatabase()
    {
        Debug.Log("ClearDatabase");

        if (db == null)
            throw new Exception("cxDatabase:ClearDatabase db not opend");

            SQLiteQuery qr;

            string querySelect = "DELETE FROM SavedData";
            qr = new SQLiteQuery(db, querySelect);
            qr.Step();
            qr.Release();
       
    }

    void CreateTable()
    {
        if(db == null)
            throw new Exception("cxDatabase:CreateTable db not opend");

        try{
            SQLiteQuery qr;
            string querySelect = "CREATE TABLE SavedData(section TEXT, jsonData BLOB);";
            qr = new SQLiteQuery(db, querySelect);
            qr.Step();
            qr.Release();
        } catch(Exception ex){
            UnityEngine.Debug.LogException(ex);
        }
    }

    byte[] LoadSection(string section)
    {
        byte[] jsonData = null;

         Debug.Log("[DB] LoadSection :" + section);

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
        }
        else
        {
            throw new Exception("cxDatabase:LoadSection db not opend");
        }

        return jsonData;
    }

    bool SaveSection(string section, byte[] jsonData)
    {
        bool ret = false;

         Debug.Log("[DB] SaveSection :" + section);

        if (db == null)
           throw new Exception("cxDatabase:SaveSection db not opend");

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
        if (jsonData != null)
            ret = SaveSection(section, jsonData);

        return ret;
    }

    public bool LoadSectionObject<T>(string section, out T savedObject) where T : class
    {
        bool ret = false;
        byte[] jsonData = LoadSection(section);

        if (jsonData != null)
        {
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

        if (jsonData != null)
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

        if (jsonData != null)
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
