using System;
using System.Diagnostics;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class RandomClass
    {
        private RandomClass()
        {
        }

        

        private static RandomClass _instance;
        private static readonly object _instanceLock = new object();

        private Random _rnd;
        private readonly object _rndLock = new object();

     
        internal static int GetRandomInt(int max)
        {
            return GetRandomInt(0, max);
        }

        internal static int GetRandomInt(int min, int max)
        {
            CheckInstance();
            return _instance.GetNextInt(min, max); 
        }

       
        private static void CheckInstance()
        {
            if (_instance == null) 
            {
                lock (_instanceLock) 
                {
                    if (_instance == null) 
                    {
                        _instance = new RandomClass(); 
                        _instance.InitInstance();
                    }
                }
            }
        }

        private void InitInstance()
        {
            lock (_rndLock) 
            {
                if (_rnd == null) 
                {
                    var tsp = new TimeSpan(DateTime.Now.Ticks);
                    var seed = (int) (tsp.TotalMilliseconds*10000%int.MaxValue%10000);
                    Debug.WriteLine($@"Random seed = {seed}");
                    _rnd = new Random(seed);
                }
            }
        }

        private int GetNextInt(int min, int max)
        {
            if (_rnd == null)
            {
                InitInstance();
            }

            lock (_rndLock)
            {
                return _rnd.Next(min, max); // Random.Next(min, max) вже повертає число в діапазоні [min, max)
            }
        }

      
    }
}