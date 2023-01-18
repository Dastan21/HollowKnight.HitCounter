using System;
using System.Linq;
using Modding;
using Satchel;
using Satchel.BetterMenus;

namespace HitCounter
{
    public static class NewModMenu
    {
        private static Menu _menuRef;
        private static bool _toggleKeybinds;
        private static bool _toggleButtonbinds;

        private static Menu PrepareMenu(ModToggleDelegates toggle)
        {
            return new Menu(HitCounter.Instance.GetName(),new Element[]
            {
                toggle.CreateToggle("Mod toggle", "Allows disabling the mod"),
                new HorizontalOption(
                    "Selected counter", 
                    "Select the counter you want to use",
                    HitCounter.Instance.Counters.Select(s => s.title).ToArray(),
                    counter => {
                        HitCounter.Instance.GlobalData.selectedCounter = counter;
                        HitCounter.Instance.ToggleCurrentCounter();
                    },
                    () => HitCounter.Instance.GlobalData.selectedCounter,
                    "HorizontalOption-SelectedCounter"
                ),
                new CustomSlider(
                    "Maximum splits",
                    f => {
                        HitCounter.Instance.GlobalData.totalSplits = (int) f;
                        HitCounter.Instance.ToggleCurrentCounter();
                    },
                    () => HitCounter.Instance.GlobalData.totalSplits,
                    HitCounter.SplitsMin,
                    HitCounter.Instance.GlobalData.compactMode ? HitCounter.CompactSplitsCountMax : HitCounter.DefaultSplitsCountMax,
                    true,
                    "CustomSlider-MaximumSplits"
                )
                {
                    minValue = HitCounter.SplitsMin,
                    maxValue = HitCounter.Instance.GlobalData.compactMode ? HitCounter.CompactSplitsCountMax : HitCounter.DefaultSplitsCountMax,
                    wholeNumbers = true
                },
                new TextPanel("(Can be slow with lots of splits)", fontSize: 25),
                new HorizontalOption(
                    "Counter mode",
                    "Interface mode of the counter",
                    new []{ "Default", "Compact" },
                    mode => {
                        HitCounter.Instance.GlobalData.compactMode = mode == 1;
                        _menuRef?.Find("CustomSlider-MaximumSplits")?.updateAfter(el => {
                            var slider = (CustomSlider) el;
                            slider.maxValue = HitCounter.Instance.GlobalData.compactMode ? HitCounter.CompactSplitsCountMax : HitCounter.DefaultSplitsCountMax;
                            slider.value = Math.Min(HitCounter.Instance.GlobalData.totalSplits, slider.maxValue);
                        });
                        HitCounter.Instance.ToggleCurrentCounter();
                    },
                    () => HitCounter.Instance.GlobalData.compactMode ? 1 : 0
                ),
                new MenuButton(
                    "Open counters folder",
                    "To add counters, create them in your Counters folder",
                    _ => IoUtils.OpenDefault(HitCounter.CountersPath)
                ),
                new MenuButton(
                    "Refresh counters",
                    "Load the new added counters in the Counters folder",
                    _ =>
                    {
                        HitCounter.Instance.LoadCounters();
                        _menuRef?.Find("HorizontalOption-SelectedCounter")?.updateAfter(el => ((HorizontalOption)el).Values = HitCounter.Instance.Counters.Select(s => s.title).ToArray());
                    }
                ),
                new MenuButton(
                    "Keyboard bindings",
                    "Click to show keyboard bindings",
                    _ =>
                    {
                        _toggleKeybinds = !_toggleKeybinds;
                        _menuRef.Find("MenuButton-Keybinds")?.updateAfter(el => ((MenuButton)el).Description = $"Click to {(_toggleKeybinds ? "hide" : "show")} keyboard bindings");
                        if (_toggleKeybinds)
                        {
                            _menuRef?.Find("Keybind-ToggleCounter")?.Show();
                            _menuRef?.Find("Keybind-NextSplit")?.Show();
                            _menuRef?.Find("Keybind-PreviousSplit")?.Show();
                        }
                        else
                        {
                            _menuRef?.Find("Keybind-ToggleCounter")?.Hide();
                            _menuRef?.Find("Keybind-NextSplit")?.Hide();
                            _menuRef?.Find("Keybind-PreviousSplit")?.Hide();
                        }
                    },
                    Id: "MenuButton-Keybinds"
                ),
                new KeyBind("Toggle Counter", HitCounter.Instance.GlobalData.Keybinds.toggleCounter, "Keybind-ToggleCounter"){ isVisible = false },
                new KeyBind("Next Split", HitCounter.Instance.GlobalData.Keybinds.nextSplit, "Keybind-NextSplit"){ isVisible = false },
                new KeyBind("Previous Split", HitCounter.Instance.GlobalData.Keybinds.previousSplit, "Keybind-PreviousSplit"){ isVisible = false },
                new MenuButton(
                    "Controller bindings",
                    "Click to show controller bindings",
                    _ =>
                    {
                        _toggleButtonbinds = !_toggleButtonbinds;
                        _menuRef.Find("MenuButton-Buttonbinds")?.updateAfter(el => ((MenuButton)el).Description = $"Click to {(_toggleButtonbinds ? "hide" : "show")} controller bindings");
                        if (_toggleButtonbinds)
                        {
                            _menuRef?.Find("ButtonBind-ToggleCounter")?.Show();
                            _menuRef?.Find("ButtonBind-NextSplit")?.Show();
                            _menuRef?.Find("ButtonBind-PreviousSplit")?.Show();
                        }
                        else
                        {
                            _menuRef?.Find("ButtonBind-ToggleCounter")?.Hide();
                            _menuRef?.Find("ButtonBind-NextSplit")?.Hide();
                            _menuRef?.Find("ButtonBind-PreviousSplit")?.Hide();
                        }
                    },
                    Id: "MenuButton-Buttonbinds"
                ),
                new ButtonBind("Toggle Counter", HitCounter.Instance.GlobalData.Buttonbinds.toggleCounter, "ButtonBind-ToggleCounter"){ isVisible = false },
                new ButtonBind("Next Split", HitCounter.Instance.GlobalData.Buttonbinds.nextSplit, "ButtonBind-NextSplit"){ isVisible = false },
                new ButtonBind("Previous Split", HitCounter.Instance.GlobalData.Buttonbinds.previousSplit, "ButtonBind-PreviousSplit"){ isVisible = false }
            });
        }

        public static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggle)
        {
            if (toggle == null) return null;
            _menuRef = PrepareMenu((ModToggleDelegates) toggle);
            
            return _menuRef.GetMenuScreen(lastMenu);
        }
    }
}