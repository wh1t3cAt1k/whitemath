#if DEBUG

using System;
using System.Collections.Generic;

using WhiteMath.General;

namespace WhiteMath
{
    public class LinkedListTests
    {
        /// <summary>
        /// Правильно ли формируется массив узлов.
        /// </summary>
        public static void _DBM_LinkedList_NodeArrayTest()
        {
            LinkedList<int> list = new LinkedList<int>();

            for (int i = 0; i < 20; i++)
                list.AddLast(i);

            LinkedListNode<int>[] arr = list.GetNodes();

            for (int i = 0; i < arr.Length; i++)
                Console.Write("{0} ", arr[i].Value);
        }

        /// <summary>
        /// Заполняет список числами от 1 до 10.
        /// 
        /// Меняет:
        /// 
        /// 1. Первый с последним.
        /// 2. Второй с третьим.
        /// 3. Шестой с четвертым.
        /// 4. Седьмой с седьмым.
        /// 5. Восьмой с последним.
        /// </summary>
        public static void _DBM_LinkedList_NodeExchangeTest()
        {
            LinkedList<int> list = new LinkedList<int>();

            for (int i = 0; i < 10; i++)
                list.AddLast(i+1);

            LinkedListNode<int>[] array = list.GetNodes();

            list.SwapNodes(array[0], array[9]);
            list.SwapNodes(array[1], array[2]);
            list.SwapNodes(array[5], array[3]);
            list.SwapNodes(array[6], array[6]);

            array.SwapElements(0, 9);

         	list.SwapNodes(array[7], array[9]);

            Console.WriteLine(array.IsObsolete(list));

            array = list.GetNodes();

            for (int i = 0; i < array.Length; i++)
                Console.Write("{0} ", array[i].Value);
        }

        // ------------------------

        /// <summary>
        /// Проверка на то, насколько хорошо реорганизуются
        /// в соответствии с массивом узлов узлы связного списка.
        /// </summary>
        public static void _DBM_LinkedList_ReorderNodeArrayTest(int tests, int n)
        {
            Random gen = new Random();
            LinkedList<int> list = new LinkedList<int>();

            for (int i = 0; i < tests; i++)
            {
                list.Clear();

                for (int j = 0; j < n; j++)
                    list.AddLast(gen.Next());

                LinkedListNode<int>[] nodes = list.GetNodes();

                nodes.SortMerge(Comparer<int>.Default.GetLinkedListNodeComparer());

                list.ReorderAsInList(nodes);

                if (nodes.IsObsolete(list))
                    Console.WriteLine("fail");
            }
        }
    }
}

#endif