using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Network
{
    public static class NetRequester
    {
        public static IEnumerator GetRequest(
            string url, Action<long, string> onFulfilled=null, 
            Action<string> onRejected=null)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) 
                onRejected?.Invoke(www.error);
            else 
                onFulfilled?.Invoke(www.responseCode, www.downloadHandler.text);
        }

        public static IEnumerator PostRequest(
            string url, Dictionary<string, string> fields, 
            Action<long, string> onFulfilled=null, Action<string> onRejected=null)
        {
            List<string> formFields = new List<string>();
            foreach (var field in fields)
                formFields.Add($"{field.Key}={field.Value}");
            
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(string.Join("&", formFields)));
            
            UnityWebRequest www = UnityWebRequest.Post(url, formData);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) 
                onRejected?.Invoke(www.error);
            else
                onFulfilled?.Invoke(www.responseCode, www.downloadHandler.text);
        }
        
        public static IEnumerator PostRequest(
            string url, string json, 
            Action<long, string> onFulfilled=null, Action<string> onRejected=null)
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            var www = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(jsonToSend),
                downloadHandler = new DownloadHandlerBuffer()
            };
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) 
                onRejected?.Invoke(www.error);
            else
                onFulfilled?.Invoke(www.responseCode, www.downloadHandler.text);
        }
    }
}