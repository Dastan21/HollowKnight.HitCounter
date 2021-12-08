using System;
using System.Collections.Generic;
using System.Linq;

namespace HitCounter
{
    [Serializable]
    public class Counter
    {
        public string title;
        public List<Split> splits;
        public int currentSplit;

        public int TotalHits => splits.Sum(split => split.hits);
        public int TotalHitsPb => splits.Any(split => split.hitsPb > -1) ? splits.Sum(split => Math.Max(0, split.hitsPb)) : -1;

        public void ResetCounter()
        {
            foreach (var split in splits) split.hits = 0;
        }
        public void ResetPbCounter()
        {
            foreach (var split in splits) split.hitsPb = -1;
        }

        public Split GetCurrentSplit() => splits[currentSplit];
        public void NextSplit()
        {
            if (currentSplit < splits.Count - 1) currentSplit++;
        }
        public void PreviousSplit()
        {
            if (currentSplit > 0) currentSplit--;
        }
        public bool IsPbRun() => TotalHitsPb < 0 || TotalHits < TotalHitsPb;
    }
    
    [Serializable]
    public class Split
    {
        public string name;
        public int hits;
        public int hitsPb = -1;

        public Split(string name)
        {
            this.name = name;
        }

        public void AddHit() => hits++;
    }
}