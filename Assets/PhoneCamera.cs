using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System.Net;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
public class PhoneCamera : MonoBehaviour
{
    /*public SnapshotCamera snapCam;*/
    public string fileName = "webcam_image.png";
    public int width = 1920;
    public int height = 1080;

    private static readonly HttpClient client = new HttpClient();

    private WebCamTexture camTexture;
    private Texture defaultbackground;

    public RawImage backround; //To display our image
    private bool camAviable;
    public AspectRatioFitter fit;
    public Button btn;
    public GameObject test;

    void Start()
    {

        defaultbackground = backround.texture;

        // Get all available cameras
        WebCamDevice[] devices = WebCamTexture.devices;


        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAviable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            }
        }

        if (camTexture == null)
        {
            Debug.Log("Unable to log back camera");
            return;
        }


        // Assign the camera texture to a material
        /*GetComponent<Renderer>().material.mainTexture = camTexture;*/

        // Start the camera
        camTexture.Play();
        backround.texture = camTexture;
        camAviable = true;

        btn.onClick.AddListener(SavePhoto);
    }

    void Update()
    {
        if (!camAviable)
            return;

        float ratio = (float)camTexture.width / (float)camTexture.height;
        fit.aspectRatio = ratio;

        float scaleY = camTexture.videoVerticallyMirrored ? -1f : 1f;
        backround.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -camTexture.videoRotationAngle;
        backround.rectTransform.localEulerAngles = new Vector3(1, 1, orient);
        /* if (Input.GetKeyDown(KeyCode.Space))
         {
             snapCam.callTakeSnapshot();
             Debug.Log("space entered");
         }*/
    }

    /*void ClickPhoto()
    {
        Debug.Log("Button clicked");
        Debug.Log(Application.dataPath);
        StartCoroutine();
    }

    IEnumerator StartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(camTexture.GetPixels());
        texture.Apply();

        Debug.Log("Hello");

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "photo.png", bytes);
       
    }*/

    public void SavePhoto()
    {
        /*print("lol");*/
        /*test.SetActive(true);*/
        /*Debug.Log(Application.dataPath);*/
        Texture2D photo = new Texture2D(camTexture.width, camTexture.height);
        photo.SetPixels(camTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        System.IO.File.WriteAllBytes(Application.dataPath + "/photo.png", bytes);
        predictImage(bytes);
    }

    public void predictImage(byte[] bytes)
    {
        string base64Image = Convert.ToBase64String(bytes);


        /*  var post = new Dictionary<string, string>()
          {
              {
                  "imgstr",base64Image
              }
          };

          var newpostjson = JsonConvert.SerializeObject(post);
          Console.WriteLine(post);

          var payload = new StringContent(newpostjson, Encoding.UTF8, "application/json");
          Console.WriteLine(payload);

        
          var endpoint = new Uri("http://74.235.83.70:80/predict/");

          var result = client.PostAsync(endpoint, payload).Result.Content.ReadAsStringAsync().Result;
          Debug.Log(endpoint);
          Console.WriteLine(result);
          Console.WriteLine(base64Image);*/

        string url = "http://74.235.83.70:80/predict/";

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.SetRequestHeader("Content-Type", "application/json");

        JObject jsonobject = new JObject();

     
        jsonobject["imgstr"] = base64Image;

        byte[] bodyRaw = new System.Text.UTF8Encoding(true).GetBytes(jsonobject.ToString());
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        request.SendWebRequest();

        print(bodyRaw);

    }
    
}





