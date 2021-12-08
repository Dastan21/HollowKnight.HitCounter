using System;
using System.Collections.Generic;
using InControl;
using Modding.Converters;
using Newtonsoft.Json;

namespace HitCounter
{
    [Serializable]
    public class GlobalData
    {
        public static int DefaultSplitsNumber = 5;
        
        [JsonConverter(typeof(PlayerActionSetConverter))]
        public KeyBinds Keybinds = new KeyBinds();
        [JsonConverter(typeof(PlayerActionSetConverter))]
        public ButtonBinds Buttonbinds = new ButtonBinds();

        public List<CounterData> counters = new List<CounterData>();
        public int selectedCounter;
        public int totalSplits = DefaultSplitsNumber;
        public bool compactMode;

        public CounterData FindCounter(string title)
        {
            return counters.Find(c => c.title == title);
        }
    }

    [Serializable]
    public class CounterData
    {
        public string title;
        public List<int> splits = new List<int>();

        public CounterData(string title)
        {
            this.title = title;
        }

        public void FillSplits(int size)
        {
            var count = size - splits.Count;
            if (count < 0)
            {
                splits.RemoveRange(size - 1, Math.Abs(count));
                return;
            }
            for (var i = 0; i < count; i++)
                splits.Add(-1);
        }
    }

    public class KeyBinds : PlayerActionSet
    {
        public PlayerAction toggleCounter;
        public PlayerAction nextSplit;
        public PlayerAction previousSplit;

        public KeyBinds()
        {
            toggleCounter = CreatePlayerAction("hideCounterKey");
            nextSplit = CreatePlayerAction("nextSplitKey");
            previousSplit = CreatePlayerAction("previousSplitKey");
            DefaultBinds();
        }

        private void DefaultBinds()
        {
            toggleCounter.AddDefaultBinding(Key.N);
            nextSplit.AddDefaultBinding(Key.PageDown);
            previousSplit.AddDefaultBinding(Key.PageUp);
        }
    }
    
    public class ButtonBinds : PlayerActionSet
    {
        public PlayerAction toggleCounter;
        public PlayerAction nextSplit;
        public PlayerAction previousSplit;

        
        public ButtonBinds()
        {
            toggleCounter = CreatePlayerAction("hideCounterButton");
            nextSplit = CreatePlayerAction("nextSplitButton");
            previousSplit = CreatePlayerAction("previousSplitButton");
            DefaultBinds();
        }

        private void DefaultBinds()
        {
            toggleCounter.AddDefaultBinding(InputControlType.RightBumper);
            nextSplit.AddDefaultBinding(InputControlType.DPadDown);
            previousSplit.AddDefaultBinding(InputControlType.DPadUp);
        }
    }
}