using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace HitCounter.UI
{
    public class ResourcesLoader : MonoBehaviour
    {
        private static ResourcesLoader _instance;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        
        public GameObject canvas;
        public Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        public void BuildMenus(Counter counter)
        {
            canvas = new GameObject();
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvas.AddComponent<GraphicRaycaster>();

            CounterUI.BuildMenu(canvas, counter);

            DontDestroyOnLoad(canvas);
        }

        public void LoadResources()
        {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (var res in resourceNames)
            {
                if (!res.StartsWith("HitCounter.UI.Images.")) continue;
                
                try
                {
                    var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
                    var buffer = new byte[imageStream.Length];
                    imageStream.Read(buffer, 0, buffer.Length);

                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(buffer.ToArray());

                    var split = res.Split('.');
                    var internalName = split[split.Length - 2];
                    images.Add(internalName, texture);
                }
                catch (Exception e)
                {
                    HitCounter.Instance.LogError($"Failed to load image \"{res}\" ({e.Message})");
                }
            }
            
            HitCounter.Instance.Log("Resources loaded");
        }

        public void LoadImages(DirectoryInfo info)
        {
            var folder = new DirectoryInfo(info.FullName + "/Images");
            if (!folder.Exists) return;

            var files = folder.GetFiles().Where(f => AllowedExtensions.Contains(f.Extension)).ToArray();
            if (files.Length == 0) return;
            
            foreach (var image in files)
            {
                var imageName = image.Name.Replace(image.Extension, "");

                var imageStream = new FileStream(image.FullName, FileMode.Open);
                try
                {
                    var buffer = new byte[imageStream.Length];
                    imageStream.Read(buffer, 0, buffer.Length);

                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(buffer.ToArray());

                    images.Add(imageName.Replace(" ", "_"), texture);
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("An item with the same key has already been added."))
                        HitCounter.Instance.LogWarn($"Failed to load image \"{imageName}\" ({e})");
                }
                finally
                {
                    imageStream.Close();
                }
            }
            
            HitCounter.Instance.Log($"Images loaded for \"{info.Name}\"");
        }

        public static ResourcesLoader Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType<ResourcesLoader>();
                if (_instance != null) return _instance;

                var guiObj = new GameObject();
                _instance = guiObj.AddComponent<ResourcesLoader>();
                DontDestroyOnLoad(guiObj);
                return _instance;
            }
        }

        public void Destroy()
        {
            CounterUI.Destroy();
        }
    }
}