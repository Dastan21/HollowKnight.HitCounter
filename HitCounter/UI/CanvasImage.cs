using UnityEngine;
using UnityEngine.UI;

// https://github.com/seresharp/DebugMod/blob/master/Source/CanvasImage.cs
namespace HitCounter.UI
{
    public class CanvasImage
    {
        private GameObject _imageObj;

        public CanvasImage(GameObject parent, Texture2D tex, Vector2 pos, Vector2 size, Rect subSprite)
        {
            if (size.x == 0 || size.y == 0)
            {
                size = new Vector2(subSprite.width, subSprite.height);
            }

            _imageObj = new GameObject();
            _imageObj.AddComponent<CanvasRenderer>();
            var imageTransform = _imageObj.AddComponent<RectTransform>();
            imageTransform.sizeDelta = new Vector2(subSprite.width, subSprite.height);
            _imageObj.AddComponent<Image>().sprite = Sprite.Create(tex, new Rect(subSprite.x, tex.height - subSprite.height, subSprite.width, subSprite.height), Vector2.zero);

            var group = _imageObj.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;

            _imageObj.transform.SetParent(parent.transform, false);

            var position = new Vector2((pos.x + ((size.x / subSprite.width) * subSprite.width) / 2f) / 1920f, (1080f - (pos.y + ((size.y / subSprite.height) * subSprite.height) / 2f)) / 1080f);
            imageTransform.anchorMin = position;
            imageTransform.anchorMax = position;
            imageTransform.SetScaleX(size.x / subSprite.width);
            imageTransform.SetScaleY(size.y / subSprite.height);

            Object.DontDestroyOnLoad(_imageObj);
        }

        public void UpdateImage(Texture2D tex, Rect subSection)
        {
            if (_imageObj == null) return;
            _imageObj.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(subSection.x, tex.height - subSection.height, subSection.width, subSection.height), Vector2.zero);
        }

        public void SetRenderIndex(int idx)
        {
            _imageObj.transform.SetSiblingIndex(idx);
        }

        public void Destroy()
        {
            Object.Destroy(_imageObj);
        }

        public void SetActive(bool b)
        {
            if (_imageObj == null) return;
            
            _imageObj.SetActive(b);
        }
    }
}