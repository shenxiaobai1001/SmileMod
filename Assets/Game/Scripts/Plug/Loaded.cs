using Game.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using YooAsset;

public static class Loaded
{
    public static string resourcePath = "Assets/Game/Resources_moved/";
    public static string videoFilePath = "Assets/Game/Resources_video/";
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam 类型="T"></typeparam>
    /// <param 路径="url"></param>
    public static T Load<T>(string url) where T : UnityEngine.Object
    {
        string filename = "";
        if (typeof(T) == typeof(GameObject))      //预制体
        {
            filename = $"{resourcePath}Prefabs/{url}"; 
        }
        else if (typeof(T) == typeof(TextAsset))//表格
        {
            filename = $"{resourcePath}Config/{url}";
        }
        else if (typeof(T) == typeof(Sprite))//图片
        {
            filename = $"{resourcePath}Textures/{url}";
        }
        else if (typeof(T) == typeof(AudioClip))//音频
        {
            filename = $"{resourcePath}Media/{url}";
        }
        else if (typeof(T) == typeof(VideoClip))//视频
        {
            filename = $"{resourcePath}Media/video/{url}";
        }
        else if (typeof(T) == typeof(ScriptableObject)) //配置
        {
            filename = $"{resourcePath}ScriptableObject/{url}";
        }

        var handle = YooUpdateManager.MainPackage.LoadAssetSync<T>(filename/*.ToLower()*/);

        if (handle?.AssetObject != null)
        {
            return handle.AssetObject as T;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 加载所有资源
    /// </summary>
    public static YooAsset.AllAssetsHandle LoadAll<T>(string url) where T : UnityEngine.Object
    {
        var filename = resourcePath + url;
        var handle = YooUpdateManager.MainPackage.LoadAllAssetsAsync<T>(filename/*.ToLower()*/);

        if (handle.AllAssetObjects != null)
        {
            return handle;
        }
        else
        {
            return null;
        }
    }

    /// <summary>加载场景 </summary>
    public static SceneHandle OnLoadScence(string location)
    {
        if (YooUpdateManager.MainPackage != null)
          return  YooUpdateManager.MainPackage.LoadSceneAsync(location);
        return null;
    }
}
