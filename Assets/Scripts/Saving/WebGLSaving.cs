//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;

//public class WebGLSaving
//{
//    [DllImport("__Internal")]
//    private static extern void SyncFiles();

//    [DllImport("__Internal")]
//    private static extern void WindowAlert(string message);

//    public static void Save(string gameDetails)
//    {
//        string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);
//        BinaryFormatter binaryFormatter = new BinaryFormatter();
//        FileStream fileStream;

//        
//        {
//            if (File.Exists(dataPath))
//            {
//                File.WriteAllText(dataPath, string.Empty);
//                fileStream = File.Open(dataPath, FileMode.Open);
//            }
//            else
//            {
//                fileStream = File.Create(dataPath);
//            }

//            binaryFormatter.Serialize(fileStream, gameDetails);
//            fileStream.Close();

//            if (Application.platform == RuntimePlatform.WebGLPlayer)
//            {
//                SyncFiles();
//            }
//        }
//        catch (Exception e)
//        {
//            PlatformSafeMessage("Failed to Save: " + e.Message);
//        }
//    }

//    public static string Load()
//    {
//        string gameDetails = null;
//        string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);

//        
//        {
//            if (File.Exists(dataPath))
//            {
//                BinaryFormatter binaryFormatter = new BinaryFormatter();
//                FileStream fileStream = File.Open(dataPath, FileMode.Open);

//                gameDetails = (string)binaryFormatter.Deserialize(fileStream);
//                fileStream.Close();
//            }
//        }
//        catch (Exception e)
//        {
//            PlatformSafeMessage("Failed to Load: " + e.Message);
//        }

//        return gameDetails;
//    }

//    private static void PlatformSafeMessage(string message)
//    {
//        if (Application.platform == RuntimePlatform.WebGLPlayer)
//        {
//            WindowAlert(message);
//        }
//        else
//        {
//            Debug.Log(message);
//        }
//    }
//}

