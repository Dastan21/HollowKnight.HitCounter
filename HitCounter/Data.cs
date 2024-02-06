using InControl;
using Modding.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public List<int> session = new List<int>();

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
                session.RemoveRange(size - 1, Math.Abs(count));
                return;
            }
            for (var i = 0; i < count; i++)
            {
                splits.Add(-1);
                session.Add(-1);
            }
        }
    }

    public class KeyBinds : PlayerActionSet
    {
        public PlayerAction toggleCounter;
        public PlayerAction nextSplit;
        public PlayerAction previousSplit;
        public PlayerAction addHit;
        public PlayerAction removeHit;

        public KeyBinds()
        {
            toggleCounter = CreatePlayerAction("hideCounterKey");
            nextSplit = CreatePlayerAction("nextSplitKey");
            previousSplit = CreatePlayerAction("previousSplitKey");
            addHit = CreatePlayerAction("addHitKey");
            removeHit = CreatePlayerAction("removeHitKey");
            DefaultBinds();
        }

        private void DefaultBinds()
        {
            toggleCounter.AddDefaultBinding(Key.N);
            nextSplit.AddDefaultBinding(Key.PageDown);
            previousSplit.AddDefaultBinding(Key.PageUp);
            addHit.AddDefaultBinding(Key.PadPlus);
            removeHit.AddDefaultBinding(Key.PadMinus);
        }
    }

    public class ButtonBinds : PlayerActionSet
    {
        public PlayerAction toggleCounter;
        public PlayerAction nextSplit;
        public PlayerAction previousSplit;
        public PlayerAction addHit;
        public PlayerAction removeHit;


        public ButtonBinds()
        {
            toggleCounter = CreatePlayerAction("hideCounterButton");
            nextSplit = CreatePlayerAction("nextSplitButton");
            previousSplit = CreatePlayerAction("previousSplitButton");
            addHit = CreatePlayerAction("addHitButton");
            removeHit = CreatePlayerAction("removeHitButton");
            DefaultBinds();
        }

        private void DefaultBinds()
        {
            toggleCounter.AddDefaultBinding(InputControlType.RightBumper);
            nextSplit.AddDefaultBinding(InputControlType.DPadDown);
            previousSplit.AddDefaultBinding(InputControlType.DPadUp);
            addHit.AddDefaultBinding(InputControlType.RightStickButton);
            removeHit.AddDefaultBinding(InputControlType.LeftStickButton);
        }
    }
}