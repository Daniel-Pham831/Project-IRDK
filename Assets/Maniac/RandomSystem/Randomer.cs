using UnityEngine;

namespace Maniac.RandomSystem
{
    public class Randomer
    {
        private int _seed;
        public int Seed => _seed;
        
        public void Init()
        {
            _seed = Random.Range(int.MinValue, int.MaxValue);
        }

        public void SetSeed(int seed = 0)
        {
            if(seed != 0)
                _seed = seed;
            
            Random.InitState(_seed);
        }
    }
}