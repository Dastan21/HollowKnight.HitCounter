using UnityEngine;
using UnityEngine.UI;

// https://github.com/seresharp/DebugMod/blob/master/Source/CanvasText.cs
namespace HitCounter.UI
{
    public class CanvasText
    {
        private GameObject _textObj;

        public CanvasText(GameObject parent, Vector2 pos, Vector2 size, Font font, string text, int fontSize = 13, FontStyle style = FontStyle.Normal, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            Vector2 size1;
            if (size.x == 0 || size.y == 0) size1 = new Vector2(1920f, 1080f);
            else size1 = size;

            _textObj = new GameObject();
            _textObj.AddComponent<CanvasRenderer>();
            var textTransform = _textObj.AddComponent<RectTransform>();
            textTransform.sizeDelta = size1;

            var group = _textObj.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;

            var t = _textObj.AddComponent<Text>();
            t.text = text;
            t.font = font;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.alignment = alignment;

            _textObj.transform.SetParent(parent.transform, false);

            var position = new Vector2((pos.x + size1.x / 2f) / 1920f, (1080f - (pos.y + size1.y / 2f)) / 1080f);
            textTransform.anchorMin = position;
            textTransform.anchorMax = position;

            Object.DontDestroyOnLoad(_textObj);
        }

        public void UpdateText(string text)
        {
            if (_textObj == null) return;
            _textObj.GetComponent<Text>().text = text;
        }

        public void MoveToTop()
        {
            if (_textObj == null) return;
            _textObj.transform.SetAsLastSibling();
        }

        public void SetActive(bool b)
        {
            if (_textObj == null) return;
            
            _textObj.SetActive(b);
        }
        
        public void Destroy() => Object.Destroy(_textObj);
    }
}