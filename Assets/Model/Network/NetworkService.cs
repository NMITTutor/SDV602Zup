using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

/*
 * This class is experimental, its purpose is to explore 
 * the UnityEngine.Networking facilities and transfer of 
 * data using JSON  with UnityEngine.JsonUtility.
 * 
 * UnityWebRequest object
 * First off having a look at the UnityWebRequest object.
 * https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html
 * 
 * - Really interesting that after Send a number of properties can not be set
 *   I think that means a new UnityWebRequest is needed for each web request.
 * - The download and update handlers have to Unity3D's handlers. I've not used the upload handler explicity.
 *   I am using DownloadHandlerBuffer with seems useful for the JSON experiment.
 * - a UnityWebRequestAsyncOperation is returned, 
 *    when you ask the UnityWebRequest to send, lcWebReq.SendWebRequest().  
 *   UnityWebRequestAsyncOperation has an "event" listener "completed", which 
 *   is set AFTER the web request is made. This is "weird" asynchronous coding.
 *   Why set the handler after making the request? 
 *   
 *   The AsyncOperation then runs your completed handler when '
 *   there is a response from the webserver..
 *   
 * Sending and Receiving JSON
 * UnityEngine.JsonUtility has a number of methods, of interest
 * https://docs.unity3d.com/ScriptReference/JsonUtility.html
 * https://docs.unity3d.com/Manual/JSONSerialization.html
 * 
 * I've used FromJson and ToJson
 * FromJson
 * What's really interesting about this is FromJson is generic.
 * It returns an object of the Type T
 * 
 * ToJson
 * Takes an object and figures out the type from that object
 * to produce Json.
 * 
 * I was not able to ask these methods to produce a Json Array
 * so I wrote a couple of methods that convert a List<T> of 
 * "simple data type" objects into JSON and from JSON. 
 * 1. using the ToJson method, see ListToJson. 
 * 2. and to retrieve the List<T>,see FromJsonArrayToList<T>(
 * 
 */
public delegate void ReceiveRecordDelegate<T>(T pStrRecObj);
public delegate void ReceiveRecordDelegateList <T>(List<T> pStrRecObjList);
public class NetworkService {

	public NetworkService()
    {

       // Debug.Log("NetworkService : To be implemented");
    }


    public void GetJson<T>(string pStrURI, ReceiveRecordDelegate<T> pDelReceiveRecord)
    {
        
         System.Uri lcURI = new System.Uri(pStrURI);

        UnityWebRequest lcWebReq = new UnityWebRequest(lcURI, "GET")
        {
            downloadHandler = new DownloadHandlerBuffer(),

        };

        // VERY WEIRD, because Send returns an object that
        // you then add a "completed" event handler to the
        // completed listeners
        var lcAsyncOp = lcWebReq.SendWebRequest();
        lcAsyncOp.completed +=  ( x =>{
            GameModel.DebugDisplay ="COMPLETED "+lcWebReq.downloadHandler.text;
            pDelReceiveRecord(JsonUtility.FromJson<T>(lcWebReq.downloadHandler.text));
        });
        
    }

    public void PutJson<T>(T pRecord, string pStrURI, ReceiveRecordDelegate<T> pDelReceiveRecord)
    {
        string strJSONURI = pStrURI +"?put="+ JsonUtility.ToJson(pRecord);

        System.Uri lcURI = new System.Uri(strJSONURI);

        UnityWebRequest lcWebReq = new UnityWebRequest(lcURI, "GET")
        {
            downloadHandler = new DownloadHandlerBuffer(),

        };

        var lcAsyncOp = lcWebReq.SendWebRequest();
        lcAsyncOp.completed += (x => {
            GameModel.DebugDisplay = "COMPLETED " + lcWebReq.downloadHandler.text;
            pDelReceiveRecord(JsonUtility.FromJson<T>(lcWebReq.downloadHandler.text));
        });

    }
    private string ListToJson<T>(List<T> pRecordList)
    {
        string lcResult = "[";
        int lcCount = 0;

        foreach ( var record in pRecordList)
        {
            if ((lcCount != pRecordList.Count) && (lcCount != 0))
                lcResult += ",";
            lcResult += JsonUtility.ToJson(record);

            lcCount++;
        }
        lcResult += "]";
        return lcResult;
    }

    private List<T> FromJsonArrayToList<T>(string pStrJsonAry) {
        string[] sep = { "},{" };
        string[] aryOfJSON = (pStrJsonAry.Substring(1, pStrJsonAry.Length - 2)).Split(sep,StringSplitOptions.RemoveEmptyEntries);
        List<string> lstOfJSONClean = new List<string>(); ;
        foreach (string aString in aryOfJSON)
        {
            string bString = aString;
            if (aString.Substring(0, 1) != "{") {
                 bString = "{" + aString;
            }
            if (aString.Substring(aString.Length - 1, 1) != "}")
            {
                bString = aString + "}";
            }
            lstOfJSONClean.Add(bString);

        }
        aryOfJSON = lstOfJSONClean.ToArray();

        List<T> lcResult = new List<T>();
        foreach(string aJSONStr in aryOfJSON)
        {

            lcResult.Add(JsonUtility.FromJson<T>(aJSONStr));
        }

        return lcResult;
    }
    public void PutJsonList <T>(List <T> pRecordList, string pStrURI, ReceiveRecordDelegateList<T> pDelReceiveRecordList)
    {
        string strJSONURI = pStrURI + "?put=" + ListToJson<T>(pRecordList);

        System.Uri lcURI = new System.Uri(strJSONURI);

        UnityWebRequest lcWebReq = new UnityWebRequest(lcURI, "GET")
        {
            downloadHandler = new DownloadHandlerBuffer(),

        };

        var lcAsyncOp = lcWebReq.SendWebRequest();
        lcAsyncOp.completed += (x => {
            string lcText = lcWebReq.downloadHandler.text;
            GameModel.DebugDisplay = "COMPLETED " + lcText ;
           
            List<T> lcList = FromJsonArrayToList<T>(lcText);

            pDelReceiveRecordList(lcList);
        });

    }
}
