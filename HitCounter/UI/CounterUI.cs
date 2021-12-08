using System;
using UnityEngine;

namespace HitCounter.UI
{
    public class CounterUI
    {
        private static CanvasPanel _panel;
        private static bool _compactMode;
        // Default
        private const string EmptyImage = "Empty";
        private const string BackgroundTopImage = "BackgroundTop";
        private const string BackgroundSplitImage = "BackgroundSplit";
        private const string BackgroundBottomImage = "BackgroundBottom";
        private const string SelectedSplitImage = "SelectedSplit";
        private const string SplitImage = "Split";
        private const float BackgroundWidth = 360f;
        private const float TitleHeight = 60f;
        private const float SplitHeight = 80f;
        private const float SplitImageSize = 55f;
        private const float SplitHitsWidth = 120f;
        private const float Margin = 25f;
        private const float Center = SplitHeight - 1.7f * Margin;
        private const int FontSizeNormal = 20;
        private const int FontSizeSmall = 16;
        private const int FontSizeMini = 14;
        // Compact mode
        private const string SelectedSplitCompactImage = "SelectedSplitCompact";
        private const string BackgroundTopCompactImage = "BackgroundTopCompact";
        private const string BackgroundSplitCompactImage = "BackgroundSplitCompact";
        private const string BackgroundBottomCompactImage = "BackgroundBottomCompact";
        private const string SplitCompactImage = "SplitCompact";
        private const float TitleHeightCompact = 60f;
        private const float SplitHeightCompact = 30f;
        private const float CenterCompact = 0.32f * SplitHeightCompact;
        
        public static void BuildMenu(GameObject canvas, Counter counter)
        {
            if (HitCounter.Instance.GlobalData.compactMode) BuildCompactUI(canvas, counter);
            else BuildDefaultUI(canvas, counter);

            _panel.FixRenderOrder();
            UpdateUI(counter);
        }

        private static void BuildDefaultUI(GameObject canvas, Counter counter)
        {
            // Background
            var backgroundTop = ResourcesLoader.Instance.images[BackgroundTopImage];
            var backgroundSplit = ResourcesLoader.Instance.images[BackgroundSplitImage];
            var backgroundBottom = ResourcesLoader.Instance.images[BackgroundBottomImage];
            _panel = new CanvasPanel(canvas, backgroundTop, new Vector2(1920 - backgroundTop.width, 0), Vector2.zero, new Rect(0, 0, backgroundTop.width, backgroundTop.height));
            // Title
            _panel.AddPanel("HC-Title", null, new Vector2(0, TitleHeight), Vector2.zero, new Rect(0, 0, BackgroundWidth, TitleHeight));
            var titlePanel = _panel.GetPanel("HC-Title");
            titlePanel.AddText("PantheonName", "-", new Vector2(1.2f * Margin, 0), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
            titlePanel.AddText("PantheonSplit", "-/-", new Vector2(1.2f * Margin, TitleHeight - 1.2f * Margin), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
            // Splits Split
            var splitImg = ResourcesLoader.Instance.images[SplitImage];
            var selectedSplitImg = ResourcesLoader.Instance.images[SelectedSplitImage];
            var emptyImg = ResourcesLoader.Instance.images[EmptyImage];
            var max = Math.Min(HitCounter.Instance.GlobalData.compactMode ? HitCounter.Instance.GlobalData.totalSplits : Math.Min(HitCounter.Instance.GlobalData.totalSplits, 10), counter.splits.Count);
            for (var b = 0; b < max; b++)
            {
                var splitHeight = 2 * TitleHeight + b * SplitHeight;
                _panel.AddPanel("HC-Split-" + b, null, new Vector2(Margin, splitHeight), Vector2.zero, new Rect(0, TitleHeight, BackgroundWidth, SplitHeight));
                var splitPanel = _panel.GetPanel("HC-Split-" + b);
                splitPanel.AddImage("SplitBorderImage", splitImg, Vector2.zero, new Vector2(splitImg.width, splitImg.height), new Rect(0, 0, splitImg.width, splitImg.height));
                splitPanel.AddImage("SplitImage", emptyImg, new Vector2(Margin, 0.8f * Margin), new Vector2(SplitImageSize, SplitImageSize), new Rect(0, 0, emptyImg.width, emptyImg.height));
                splitPanel.AddText("SplitName", "", new Vector2(SplitImageSize + 1.3f * Margin, Margin), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
                splitPanel.AddText("SplitHits", "", new Vector2(BackgroundWidth - SplitHitsWidth - Margin, SplitHeight - Margin), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
                splitPanel.AddText("SplitHitsPB", "", new Vector2(BackgroundWidth - SplitHitsWidth + Margin, SplitHeight - Margin), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
                splitPanel.AddImage("SplitSelected", selectedSplitImg, new Vector2(-Margin, 0), new Vector2(selectedSplitImg.width, selectedSplitImg.height), new Rect(0, 0, selectedSplitImg.width, selectedSplitImg.height));
                splitPanel.AddImage("SplitBackground-" + b, backgroundSplit, new Vector2(-Margin, 0), new Vector2(backgroundSplit.width, backgroundSplit.height), new Rect(0, 0, backgroundSplit.width, backgroundSplit.height));
            }
            // Total panel
            var maxHeight = 2 * TitleHeight + max * SplitHeight;
            _panel.AddPanel("HC-Total", null, new Vector2(Margin, maxHeight), Vector2.zero, new Rect(0, TitleHeight, BackgroundWidth, SplitHeight));
            var totalPanel = _panel.GetPanel("HC-Total");
            totalPanel.AddImage("TotalSplitImage", splitImg, Vector2.zero, new Vector2(splitImg.width, splitImg.height), new Rect(0, 0, splitImg.width, splitImg.height));
            totalPanel.AddText("TotalText", "Total", new Vector2(1.5f * Margin, Center), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
            totalPanel.AddText("TotalHits", "-", new Vector2(BackgroundWidth - SplitHitsWidth - Margin, Center), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
            totalPanel.AddText("TotalHitsPB", "-", new Vector2(BackgroundWidth - SplitHitsWidth + Margin, Center), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeNormal);
            totalPanel.AddImage("BackgroundBottom", backgroundBottom, new Vector2(-Margin, 0), new Vector2(backgroundBottom.width, backgroundBottom.height), new Rect(0, 0, backgroundBottom.width, backgroundBottom.height));

            _compactMode = false;
        }

        private static void BuildCompactUI(GameObject canvas, Counter counter)
        {
            // Background
            var backgroundTop = ResourcesLoader.Instance.images[BackgroundTopCompactImage];
            var backgroundSplit = ResourcesLoader.Instance.images[BackgroundSplitCompactImage];
            var backgroundBottom = ResourcesLoader.Instance.images[BackgroundBottomCompactImage];
            _panel = new CanvasPanel(canvas, backgroundTop, new Vector2(1920 - backgroundTop.width, 0), Vector2.zero, new Rect(0, 0, backgroundTop.width, backgroundTop.height));
            // Title
            _panel.AddPanel("HC-Title", null, new Vector2(SplitHeightCompact, TitleHeightCompact - SplitHeightCompact), Vector2.zero, new Rect(0, 0, BackgroundWidth - 2 * SplitHeightCompact, TitleHeightCompact));
            var titlePanel = _panel.GetPanel("HC-Title");
            titlePanel.AddText("CounterName", "-", new Vector2(CenterCompact, 0), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
            titlePanel.AddText("CounterSplits", "-/-", new Vector2(BackgroundWidth - SplitHitsWidth, 0), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
            var breakImg = ResourcesLoader.Instance.images[SplitCompactImage];
            titlePanel.AddImage("TitleBreak", breakImg, new Vector2(SplitHeightCompact, SplitHeightCompact - CenterCompact), new Vector2(breakImg.width, breakImg.height), new Rect(0, 0, breakImg.width, breakImg.height));
            var emptyImg = ResourcesLoader.Instance.images[EmptyImage];
            var selectedSplitImg = ResourcesLoader.Instance.images[SelectedSplitCompactImage];
            var max = Math.Min(HitCounter.Instance.GlobalData.totalSplits, counter.splits.Count);
            for (var b = 0; b < max; b++)
            {
                _panel.AddPanel("HC-Split-" + b, null, new Vector2(SplitHeightCompact, b * SplitHeightCompact + TitleHeightCompact), Vector2.zero, new Rect(0, 0, BackgroundWidth - 2 * SplitHeightCompact, SplitHeightCompact));
                var splitPanel = _panel.GetPanel("HC-Split-" + b);
                splitPanel.AddText("SplitName", "", new Vector2(SplitHeightCompact + CenterCompact, CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeMini);
                splitPanel.AddText("SplitHits", "", new Vector2(BackgroundWidth - SplitHitsWidth, CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
                splitPanel.AddText("SplitHitsPB", "", new Vector2(BackgroundWidth - SplitHitsWidth + SplitHeightCompact, CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
                splitPanel.AddImage("SplitImage", emptyImg, Vector2.zero, new Vector2(SplitHeightCompact, SplitHeightCompact), new Rect(0, 0, emptyImg.width, emptyImg.height));
                splitPanel.AddImage("SplitSelected", selectedSplitImg, new Vector2(- 1.5f * SplitHeightCompact, 0), new Vector2(selectedSplitImg.width, selectedSplitImg.height), new Rect(0, 0, selectedSplitImg.width, selectedSplitImg.height));
                splitPanel.AddImage("SplitBackground-" + b, backgroundSplit, new Vector2(-SplitHeightCompact, 0), new Vector2(backgroundSplit.width, backgroundSplit.height), new Rect(0, 0, backgroundSplit.width, backgroundSplit.height));
            }
            // Total panel
            _panel.AddPanel("HC-Total", null, new Vector2(SplitHeightCompact, max * SplitHeightCompact + TitleHeightCompact), Vector2.zero, new Rect(0, 0, BackgroundWidth - 2 * SplitHeightCompact, SplitHeightCompact));
            var totalPanel = _panel.GetPanel("HC-Total");
            totalPanel.AddText("TotalText", "Total", new Vector2(SplitHeightCompact - CenterCompact, 2 * CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
            totalPanel.AddText("TotalHits", "-", new Vector2(BackgroundWidth - SplitHitsWidth, 2 * CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
            totalPanel.AddText("TotalHitsPB", "-", new Vector2(BackgroundWidth - SplitHitsWidth + SplitHeightCompact, 2 * CenterCompact), Vector2.zero, Modding.CanvasUtil.TrajanBold, FontSizeSmall);
            totalPanel.AddImage("TotalBreak", breakImg, new Vector2(SplitHeightCompact, 0.17f * CenterCompact), new Vector2(breakImg.width, breakImg.height), new Rect(0, 0, breakImg.width, breakImg.height));
            totalPanel.AddImage("BackgroundBottom", backgroundBottom, new Vector2(-SplitHeightCompact, 0), new Vector2(backgroundBottom.width, backgroundBottom.height), new Rect(0, 0, backgroundBottom.width, backgroundBottom.height));

            _compactMode = true;
        }

        private static Texture2D TryGetImage(string name)
        {
            Texture2D img = null;
            try
            {
                img = ResourcesLoader.Instance.images[name];
            }
            catch (Exception) { /**/ }
            return img;
        }

        public static void UpdateUI(Counter counter)
        {
            if (_panel == null) return;

            if (_compactMode) UpdateCompactModeUI(counter);
            else UpdateDefaultUI(counter);
        }

        private static void UpdateDefaultUI(Counter counter)
        {
            // Title
            var splits = counter.splits;
            var titlePanel = _panel.GetPanel("HC-Title");
            titlePanel.GetText("PantheonName").UpdateText(counter.title);
            titlePanel.GetText("PantheonSplit").UpdateText($"{counter.currentSplit + 1}/{splits.Count}");
            // Splits
            var max = Math.Min(HitCounter.Instance.GlobalData.compactMode ? HitCounter.Instance.GlobalData.totalSplits : Math.Min(HitCounter.Instance.GlobalData.totalSplits, 10), splits.Count);
            for (var b = 0; b < max; b++)
            {
                var currentSplitNumber = GetSplitNumber(counter.currentSplit, splits.Count, max);
                var splitPanel = _panel.GetPanel("HC-Split-" + b);
                Split split = null;
                if (b + currentSplitNumber < counter.splits.Count) split = splits[b + currentSplitNumber];
                Texture2D splitImg = null;
                if (split != null) splitImg = TryGetImage(split.name.Replace(" ", "_"));
                if (splitImg != null) splitPanel.GetImage("SplitImage").UpdateImage(splitImg, new Rect(0, 0, splitImg.width, splitImg.height));
                splitPanel.GetImage("SplitImage").SetActive(splitImg != null && _panel.Active);
                splitPanel.GetText("SplitName").UpdateText(split != null ? split.name : "");
                splitPanel.GetText("SplitHits").UpdateText(split != null ? split.hits + "" : "");
                splitPanel.GetText("SplitHitsPB").UpdateText(split != null ? split.hitsPb < 0 ? "-" : split.hitsPb + "" : "");
                splitPanel.GetImage("SplitSelected").SetActive(b == GetSelectedSplit(counter.currentSplit, splits.Count, max) && _panel.Active);
            }
            // Total
            _panel.GetPanel("HC-Total").GetText("TotalHits").UpdateText(counter.TotalHits + "");
            _panel.GetPanel("HC-Total").GetText("TotalHitsPB").UpdateText(counter.TotalHitsPb < 0 ? "-" : counter.TotalHitsPb + "");
        }

        private static void UpdateCompactModeUI(Counter counter)
        {
            // Title
            var splits = counter.splits;
            var titlePanel = _panel.GetPanel("HC-Title");
            titlePanel.GetText("CounterName").UpdateText(counter.title);
            titlePanel.GetText("CounterSplits").UpdateText($"{counter.currentSplit + 1}/{splits.Count}");
            // Splits
            var max = Math.Min(HitCounter.Instance.GlobalData.totalSplits, splits.Count);
            for (var b = 0; b < max; b++)
            {
                var currentSplitNumber = GetSplitNumber(counter.currentSplit, splits.Count, max);
                var splitPanel = _panel.GetPanel("HC-Split-" + b);
                Split split = null;
                if (b + currentSplitNumber < counter.splits.Count) split = splits[b + currentSplitNumber];
                Texture2D splitImg = null;
                if (split != null) splitImg = TryGetImage(split.name.Replace(" ", "_"));
                if (splitImg != null) splitPanel.GetImage("SplitImage").UpdateImage(splitImg, new Rect(0, 0, splitImg.width, splitImg.height));
                splitPanel.GetImage("SplitImage").SetActive(splitImg != null && _panel.Active);
                splitPanel.GetText("SplitName").UpdateText(split != null ? split.name : "");
                splitPanel.GetText("SplitHits").UpdateText(split != null ? split.hits + "" : "");
                splitPanel.GetText("SplitHitsPB").UpdateText(split != null ? split.hitsPb < 0 ? "-" : split.hitsPb + "" : "");
                splitPanel.GetImage("SplitSelected").SetActive(b == GetSelectedSplit(counter.currentSplit, splits.Count, max) && _panel.Active);
            }
            // Total
            var totalPanel = _panel.GetPanel("HC-Total");
            totalPanel.GetText("TotalHits").UpdateText(counter.TotalHits + "");
            totalPanel.GetText("TotalHitsPB").UpdateText(counter.TotalHitsPb < 0 ? "-" : counter.TotalHitsPb + "");
        }

        private static int GetSplitNumber(int currentSplit, int splitsCount, int max)
        {
            var totalLength = Math.Min(max, splitsCount);
            var endLength = totalLength / 2;
            var startLength = totalLength - endLength - 1;

            if (currentSplit <= startLength) return 0;
            if (currentSplit >= splitsCount - endLength - 1) return splitsCount - totalLength;
            return currentSplit - startLength;
        }
        
        private static int GetSelectedSplit(int currentSplit, int splitsCount, int max)
        {
            var totalLength = Math.Min(max, splitsCount);
            var endLength = totalLength / 2;
            var startLength = totalLength - endLength - 1;
            var middleLength = splitsCount - startLength - endLength;

            if (currentSplit < startLength) return currentSplit;
            if (currentSplit >= splitsCount - endLength) return currentSplit - Math.Max(0, middleLength - 1);
            return startLength;
        }

        public static void Toggle(Counter currentCounter)
        {
            if (_panel == null) return;
            _panel.TogglePanel();
            if (_panel.Active) UpdateUI(currentCounter);
        }

        public static void Destroy()
        {
            _panel?.Destroy();
        }
    }
}