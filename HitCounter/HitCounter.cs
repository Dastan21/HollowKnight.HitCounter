using HitCounter.UI;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HitCounter
{
    public class HitCounter : Mod, IGlobalSettings<GlobalData>, ICustomMenuMod, ITogglableMod
    {
        public bool ToggleButtonInsideMenu => true;
        public GlobalData GlobalData = new GlobalData();
        private bool _loaded;
        public List<Counter> Counters;
        private Counter _currentCounter;
        internal static HitCounter Instance;

        public static readonly string CountersPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Counters";
        private const string SplitsJsonName = "splits.json";
        public const int SplitsMin = 3;
        public const int DefaultSplitsCountMax = 10;
        public const int CompactSplitsCountMax = 30;

        public HitCounter() : base("Hit Counter") { }
        public override string GetVersion() => "1.1.0";
        public void OnLoadGlobal(GlobalData data) => GlobalData = data;
        public GlobalData OnSaveGlobal() => GlobalData;
        private static bool IsMenuTitleScene() => Satchel.SceneUtils.getCurrentScene().name == "Menu_Title";

        public Counter GetCurrentCounter() => _currentCounter;

        public override void Initialize()
        {
            Instance = this;

            ModHooks.SavegameLoadHook += OnSavegameLoad;
            ModHooks.AfterTakeDamageHook += OnHitTaken;
            ModHooks.HeroUpdateHook += OnHeroUpdate;

            if (_loaded)
            {
                ToggleCurrentCounter();
                return;
            }
            _loaded = true;

            ResourcesLoader.Instance.LoadResources();
            LoadCounters();
            LoadPBs();
        }

        private int OnHitTaken(int hazardType, int damage)
        {
            if (_currentCounter == null) return damage;
            _currentCounter.GetCurrentSplit().AddHit();
            CounterUI.UpdateUI(_currentCounter);
            SaveCounter();

            return damage;
        }

        private void SaveCounter(bool isPB = false)
        {
            var counter = GlobalData.counters.Find(c => c.title == _currentCounter.title);
            if (counter.splits.Count == 0)
            {
                counter.splits = new List<int>(new int[_currentCounter.splits.Count]);
                counter.session = new List<int>(new int[_currentCounter.splits.Count]);
            }

            for (var s = 0; s < _currentCounter.splits.Count; s++)
            {
                var split = _currentCounter.splits[s];
                if (isPB)
                {
                    split.hitsPb = split.hits;
                    counter.splits[s] = split.hits;
                }
                counter.session[s] = split.hits;
            }
            if (isPB) Log($"Saved PBs for {_currentCounter.title}");
        }

        public void LoadCounters()
        {
            Counters = new List<Counter>();

            var modInfo = new DirectoryInfo(CountersPath);
            var folderInfo = modInfo.GetDirectories();
            if (folderInfo.Length == 0) { LogWarn("No custom counters found!"); return; }

            var counterFolders = folderInfo.Select(f => f.FullName);
            foreach (var counterFolderPath in counterFolders)
            {
                var counterInfo = new DirectoryInfo(counterFolderPath);
                var names = ParseCounterNames(counterInfo);
                if (names == null) continue;

                Counters.Add(new Counter
                {
                    title = counterInfo.Name,
                    splits = names.Select(name => new Split(name)).ToList(),
                }
                );

                ResourcesLoader.Instance.LoadImages(counterInfo);
            }

            if (GlobalData.selectedCounter >= Counters.Count) GlobalData.selectedCounter = 0;
            _currentCounter = Counters[GlobalData.selectedCounter];

            Log("Splits loaded");
        }

        private List<string> ParseCounterNames(DirectoryInfo info)
        {
            FileInfo file = null;
            try
            {
                file = info.GetFiles().First(f => f.Name == SplitsJsonName);
            }
            catch (Exception) { /**/ }

            if (file == null) { LogWarn($"Cannot find '{SplitsJsonName}' under {info.Name}."); return null; }

            string json = null;
            try
            {
                json = JArray.Parse(File.ReadAllText(file.FullName)).ToString();
            }
            catch (Exception) { /**/ }

            if (json == null) { LogWarn($"Cannot load '{SplitsJsonName}' under {info.Name}."); return null; }

            var names = JsonConvert.DeserializeObject<List<string>>(json);
            if (names == null) LogWarn("Spits json file is empty.");

            return names;
        }

        private void LoadPBs()
        {
            if (Counters.Count <= 0) return;

            foreach (var counter in Counters)
            {
                var counterData = GlobalData.FindCounter(counter.title);
                if (counterData == null)
                {
                    counterData = new CounterData(counter.title);
                    GlobalData.counters.Add(counterData);
                }
                if (counterData.splits.Count == 0) continue;
                counterData.FillSplits(counter.splits.Count);

                for (var s = 0; s < counter.splits.Count; s++)
                {
                    counter.splits[s].hits = counterData.session[s];
                    counter.splits[s].hitsPb = counterData.splits[s];
                }

                Log($"Loaded PBs for {counter.title}");
            }

            // clean unused data
            GlobalData.counters = GlobalData.counters.Where(c => FindCounter(c.title) != null).ToList();
        }

        private Counter FindCounter(string title)
        {
            return Counters.Find(c => c.title == title);
        }

        public void ToggleCurrentCounter(bool forceToggle = false)
        {
            if (GlobalData.selectedCounter >= Counters.Count) return;
            _currentCounter = Counters[GlobalData.selectedCounter];

            if (!forceToggle && IsMenuTitleScene()) return;
            if (ResourcesLoader.Instance.canvas) ResourcesLoader.Instance.Destroy();
            ResourcesLoader.Instance.BuildMenus(_currentCounter);
        }

        private void OnSavegameLoad(int _) => ToggleCurrentCounter(true);

        private void OnHeroUpdate()
        {
            if (_currentCounter == null) return;
            if (GlobalData.Keybinds.nextSplit.WasPressed || GlobalData.Buttonbinds.nextSplit.WasPressed)
            {
                if (_currentCounter.currentSplit >= _currentCounter.splits.Count - 1) SaveCounter(true);
                _currentCounter.NextSplit();
                CounterUI.UpdateUI(_currentCounter);
            }
            if (GlobalData.Keybinds.previousSplit.WasPressed || GlobalData.Buttonbinds.previousSplit.WasPressed)
            {
                if (_currentCounter.currentSplit < 1) _currentCounter.ResetCounter();
                _currentCounter.PreviousSplit();
                CounterUI.UpdateUI(_currentCounter);
            }
            if (GlobalData.Keybinds.toggleCounter.WasPressed || GlobalData.Buttonbinds.toggleCounter.WasPressed)
            {
                CounterUI.Toggle(_currentCounter);
            }
            if (GlobalData.Keybinds.addHit.WasPressed || GlobalData.Buttonbinds.addHit.WasPressed)
            {
                _currentCounter.GetCurrentSplit().AddHit();
                CounterUI.UpdateUI(_currentCounter);
                SaveCounter();
            }
            if (GlobalData.Keybinds.removeHit.WasPressed || GlobalData.Buttonbinds.removeHit.WasPressed)
            {
                _currentCounter.GetCurrentSplit().RemoveHit();
                CounterUI.UpdateUI(_currentCounter);
                SaveCounter();
            }
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle)
        {
            return ModMenu.GetMenu(modListMenu, toggle);
        }

        public void Unload()
        {
            ModHooks.SavegameLoadHook -= OnSavegameLoad;
            ModHooks.AfterTakeDamageHook -= OnHitTaken;
            ModHooks.HeroUpdateHook -= OnHeroUpdate;

            ResourcesLoader.Instance.Destroy();
        }
    }
}