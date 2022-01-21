using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;

public class afImageSaverLoader : MonoSingleton<afImageSaverLoader>
{
    class IOWorkerContext
    {
        public bool isDone;
        public string filename;
        public byte[] datas;

        public Action<byte[]> completedCallback;

		public IOWorkerContext()
		{
			isDone = false;
		}
    }

    static string GetImageFilePath(string filename)
    {
        string savePath = Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "images");
        savePath = Path.Combine(savePath, filename);

        return savePath;
    }

    static void CheckValidPath(string path)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
    }

    IEnumerator WaitForDone(IOWorkerContext t)
    {
        yield return new WaitUntil(() => { return t.isDone; });

        t.completedCallback(t.datas);
    }

   	public void SaveImage(string filename, byte[] imageBytes, Action<byte[]> callback)
    {
		IOWorkerContext t = new IOWorkerContext();

        t.filename =   GetImageFilePath(filename);
		t.datas = imageBytes;
        t.completedCallback = callback;

        ThreadPool.QueueUserWorkItem(new WaitCallback(SaveImageThreadProc), t);

        StartCoroutine(WaitForDone(t));
	}

	static void SaveImageThreadProc(object state)
    {
        IOWorkerContext t = (IOWorkerContext)state;

        SaveImage_Impl(t.filename, t.datas);

        t.isDone = true;
    }
	
 	static void SaveImage_Impl(string fullPathFilename, byte[] imageBytes)
    {
        //string fullPathFilename = GetImageFilePath(filename);

        try
        {
            CheckValidPath(fullPathFilename);
            File.WriteAllBytes(fullPathFilename, imageBytes);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

    }
    public void LoadImage(string filename, Action<byte[]> callback)
    {
        IOWorkerContext t = new IOWorkerContext();
        
        t.filename =   GetImageFilePath(filename);
        t.completedCallback = callback;

        ThreadPool.QueueUserWorkItem(new WaitCallback(LoadImageThreadProc), t);

        StartCoroutine(WaitForDone(t));
    }


    static void LoadImageThreadProc(object state)
    {
        IOWorkerContext t = (IOWorkerContext)state;

        t.datas = LoadImage_Impl(t.filename);

        t.isDone = true;
    }

    static byte[] LoadImage_Impl(string fullPathFilename)
    {
        byte[] dataByte = null;

      //  string fullPathFilename = GetImageFilePath(filename);

        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(fullPathFilename)))
            {
                Debug.LogWarning("Directory does not exist");
                return null;
            }

            if (!File.Exists(fullPathFilename))
            {
                Debug.Log("File does not exist");
                return null;
            }

			//var file = System.IO.File.OpenRead(fullPathFilename);// f = new FileStream() File f = new File();
			
            dataByte = File.ReadAllBytes(fullPathFilename);
            Debug.Log("Loaded Data from: " + fullPathFilename.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + fullPathFilename.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        return dataByte;
    }
}
