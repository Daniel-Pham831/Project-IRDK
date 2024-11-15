using System;
using System.Collections.Generic;
using System.Linq;

namespace Maniac.Utils.Extension
{
    public static class LinQExtension
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (rng == null) throw new ArgumentNullException(nameof(rng));

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
        
        public static T TakeRandomWithSeed<T>(this IEnumerable<T> list,int seed)
        {
            return list.Shuffle(new Random(seed)).FirstOrDefault();
        }

        public static IEnumerable<T> TakeRandomWithSeed<T>(this IEnumerable<T> list, int seed, int numberOfItems)
        {
            return list.Shuffle(new Random(seed)).Take(numberOfItems).ToList();
        }

        public static T TakeRandom<T>(this IEnumerable<T> list)
        {
            return list.Shuffle().FirstOrDefault();
        }

        public static IEnumerable<T> TakeRandomUnity<T>(this IEnumerable<T> list,int numOfItems)
        {
            var listCount = list.Count();
            return list.OrderBy(x => UnityEngine.Random.Range(0, listCount)).Take(numOfItems).ToList();
        }

        public static T TakeRandomUnity<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static T TakeRandomUnity<T>(this T[] list)
        {
            return list[UnityEngine.Random.Range(0, list.Length)];
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> list, int numberOfItems)
        {
            return list.Shuffle().Take(numberOfItems).ToList();
        }
    }
}