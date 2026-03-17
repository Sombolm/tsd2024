using System;
using System.Collections.Generic;

namespace AdvancedTasks
{

    public class RandomizedList<T>
    {
        private readonly List<T> _items;
        private readonly Random _random;

        public RandomizedList()
        {
            _items = new List<T>();
            _random = new Random();
        }

        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        public void Add(T element)
        {
            if (_random.NextDouble() >= 0.5)
            {
                _items.Add(element);
            }
            else
            {
                _items.Insert(0, element);
            }
        }

        public T Get(int maxIndex)
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("The collection is empty.");
            }

            if (maxIndex >= _items.Count)
            {
                maxIndex = _items.Count - 1;
            }
            else if (maxIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIndex), "Index must be greater than or equal to 0.");
            }

            int randomIndex = _random.Next(0, maxIndex + 1);

            return _items[randomIndex];
        }

        public void PrintInternalState()
        {
            Console.WriteLine(string.Join(", ", _items));
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Write a leap year method as a lambda expression without using a separate method. ===");

            Func<int, bool> isLeapYear = year => (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);

            int[] yearsToTest = { 2020, 2023, 2024, 1900, 2000 };
            foreach (int year in yearsToTest)
            {
                Console.WriteLine($"Is {year} a leap year? {isLeapYear(year)}");
            }

            Console.WriteLine("=== Create your own generic collection, which acts as a list with some randomization. It should contain a set of methods: ===")

            RandomizedList<string> randomList = new RandomizedList<string>();

            Console.WriteLine($"Is the list empty? {randomList.IsEmpty()}");

            Console.WriteLine("Adding elements sequentially: 1, 2, 3, 4, 5");
            randomList.Add("1");
            randomList.Add("2");
            randomList.Add("3");
            randomList.Add("4");
            randomList.Add("5");

            Console.WriteLine($"Is the list empty? {randomList.IsEmpty()}");

            Console.Write("Internal order of elements: ");
            randomList.PrintInternalState();

            Console.WriteLine("\nTesting Get(maxIndex = 3):");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Attempt {i + 1}: Retrieved element -> {randomList.Get(3)}");
            }
        }
    }
}