using System.Collections.Generic;
using UnityEngine;

// https://github.com/seresharp/DebugMod/blob/master/Source/CanvasPanel.cs
namespace HitCounter.UI
{
    public class CanvasPanel
    {
        private GameObject _canvas;
        private CanvasImage _background;
        private Vector2 _position;
        private Dictionary<string, CanvasPanel> _panels = new Dictionary<string, CanvasPanel>();
        private Dictionary<string, CanvasImage> _images = new Dictionary<string, CanvasImage>();
        private Dictionary<string, CanvasText> _texts = new Dictionary<string, CanvasText>();
        public bool Active { get; private set; }

        public CanvasPanel(GameObject parent, Texture2D texture, Vector2 pos, Vector2 size, Rect bgSubSection)
        {
            if (parent == null) return;

            _canvas = parent;
            if (texture != null) _background = new CanvasImage(parent, texture, pos, size, bgSubSection);
            _position = pos;

            Active = true;
        }

        public void AddPanel(string name, Texture2D texture, Vector2 pos, Vector2 size, Rect bgSubSection)
        {
            var panel = new CanvasPanel(_canvas, texture, _position + pos, size, bgSubSection);

            _panels.Add(name, panel);
        }

        public void AddImage(string name, Texture2D texture, Vector2 pos, Vector2 size, Rect subSprite)
        {
            var image = new CanvasImage(_canvas, texture, _position + pos, size, subSprite);

            _images.Add(name, image);
        }

        public void AddText(string name, string text, Vector2 pos, Vector2 size, Font font, int fontSize = 13, FontStyle style = FontStyle.Normal, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var t = new CanvasText(_canvas, _position + pos, size, font, text, fontSize, style, alignment);

            _texts.Add(name, t);
        }

        public CanvasImage GetImage(string imageName, string panelName = null)
        {
            if (panelName != null && _panels.ContainsKey(panelName))
                return _panels[panelName].GetImage(imageName);

            return _images.ContainsKey(imageName) ? _images[imageName] : null;
        }

        public CanvasPanel GetPanel(string panelName)
        {
            return _panels.ContainsKey(panelName) ? _panels[panelName] : null;
        }

        public CanvasText GetText(string textName, string panelName = null)
        {
            if (panelName != null && _panels.ContainsKey(panelName))
                return _panels[panelName].GetText(textName);

            return _texts.ContainsKey(textName) ? _texts[textName] : null;
        }

        public void FixRenderOrder()
        {
            foreach (var image in _images.Values)
                image.SetRenderIndex(0);

            foreach (var t in _texts.Values)
                t.MoveToTop();

            foreach (var panel in _panels.Values)
                panel.FixRenderOrder();

            _background?.SetRenderIndex(0);
        }

        public void Destroy()
        {
            _background?.Destroy();

            foreach (var image in _images.Values)
            {
                image.Destroy();
            }

            foreach (var t in _texts.Values)
            {
                t.Destroy();
            }

            foreach (var p in _panels.Values)
            {
                p.Destroy();
            }
        }

        public void TogglePanel()
        {
            Active = !Active;
            SetActive(Active);
        }

        private void SetActive(bool b)
        {
            _background?.SetActive(b);

            foreach (var p in _panels.Values)
                p?.SetActive(b);

            foreach (var i in _images.Values)
                i?.SetActive(b);

            foreach (var t in _texts.Values)
                t?.SetActive(b);

            Active = b;
        }
    }
}