using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    public static ImageDownloader Instance;
    public Transform createPos4;
    public GameObject roleStar;

    Dictionary<string ,Sprite> userSprites=new Dictionary<string, Sprite> ();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void OnRoleStar(DataInfo dataInfo)
    {
        if (userSprites == null || userSprites.Count <= 0) 
        {
            StartCoroutine(DownloadImageCoroutine(dataInfo));
        }
        else
        {
            if (userSprites.ContainsKey(dataInfo.user))
            {
                Sprite useSprite = userSprites[dataInfo.userAvatar];
                OnCreateRoleStar(dataInfo, useSprite);
            }
            else 
            {
                StartCoroutine(DownloadImageCoroutine(dataInfo)); 
            }
        }
    }
    public void OnCreateRoleStar(DataInfo dataInfo,Sprite sprite)
    {
        int allDuck = dataInfo.count * dataInfo.time;
        for (int i = 0; i < allDuck; i++)
        {
            int x = Random.Range(-5, 5);
            int y = Random.Range(-5, 5);
            Vector3 starCPos = new Vector3(createPos4.position.x + x, createPos4.position.y + y);
            GameObject star = SimplePool.Spawn(roleStar, starCPos, Quaternion.identity);
            star.transform.SetParent(this.transform);
            RoleStar star1 = star.GetComponent<RoleStar>();
            star1.StartMove(sprite);
        }
        allDuck = 0;
    }
    IEnumerator DownloadImageCoroutine(DataInfo dataInfo)
    {
        // string url = dataInfo.userAvatar;
        string url = "https://p26.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-avt-0015_c82fb87ae4b005b32e9af941ce39ec38.jpeg?from=3067671334";
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            // 发送请求
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"图片下载失败: {webRequest.error}");
            }
            else
            {
                // 获取下载的纹理
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);
                // 创建精灵
                Sprite newSprite = Sprite.Create(
                    downloadedTexture,
                    new Rect(0, 0, downloadedTexture.width, downloadedTexture.height),
                    new Vector2(0.5f, 0.5f) // 中心点
                );
                if (newSprite != null) {

                    if (userSprites.ContainsKey(dataInfo.user))
                    {
                        userSprites.Add(dataInfo.user, newSprite);
                   
                    }
                }
                OnCreateRoleStar(dataInfo, newSprite);
            }
        }
    }

}