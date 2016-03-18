using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RedBlackTree
{
	class Program
	{
		public class Data : RankElement<Guid, int>
		{
			public int CompareTo(object obj)
			{
				if (obj == null) return 1;
				Data other = obj as Data;
				if (other == null) return 1;
				return this._Value.CompareTo(other._Value);
			}

			private Data(Guid guid, int value)
				: base(guid, value)
			{ 
			}
		
			public static Data Create(int value)
			{
				return new Data(Guid.NewGuid(), value);
			}
		}

		public static List<Data> CreateRanData(int n)
		{
			Random ran = new Random();
			List<Data> list = new List<Data>();
			for (int i = 0; i < n; ++i)
			{
				list.Add(Data.Create(ran.Next(1000)));
			}
			return list;
		}

		public static List<Data> CreateFixData()
		{
			List<Data> list = new List<Data>();
			//int[] array = { 711, 963, 574, 544 };
			//int[] array = { 587, 251, 810, 256, 201, 138, 219, 181, 799, 423};
			int[] array = { 816, 444, 366, 975, 497, 696 };
			for (int i = 0; i < array.Length; ++i)
			{
				list.Add(Data.Create(array[i]));
			}
			return list;
		}

		public static void CreateNodeTimeTest(RedBlackTree<Guid, int> tree, int n)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Random ran = new Random();
			for (int i = 0; i < n; ++i)
			{
				int data = ran.Next(0, 1000);
			}
			sw.Stop();
			Console.WriteLine("{0}", sw.Elapsed);
		}

		public static void AddTest(RedBlackTree<Guid, int> tree, List<Data> datas)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Random ran = new Random();
			for (int i = 0; i < datas.Count; ++i)
			{
				tree.Add(datas[i]);
			}
			sw.Stop();
			Console.WriteLine("{0}", sw.Elapsed);
		}

		public static void RemoveTest(RedBlackTree<Guid, int> tree, List<Data> datas)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Random ran = new Random();
			for(int i = 0; i < datas.Count; ++i)
			{
				tree.Remove(datas[i]);

				//tree.PrintValue();
				//Console.WriteLine("-------\n");
			}
			sw.Stop();
			Console.WriteLine("{0}", sw.Elapsed);
		}

		public static void BlackDeepCheck(RedBlackTree<Guid, int> tree)
		{
			Console.WriteLine("黑色深度测试{0}", tree.BlackDeepCheck());
		}

		public static void CheckRedNode(RedBlackTree<Guid, int> tree)
		{
			Console.WriteLine("红色节点测试{0}", tree.CheckRedNode());
		}

		public static void OrderCheck(RedBlackTree<Guid, int> tree)
		{
			Console.WriteLine(tree.OrderCheck());
		}

		public static void PrintData(List<Data> list)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				Console.WriteLine("{0} ", list[i].Value);
			}
		}

		static void Main(string[] args)
		{

			RedBlackTree<Guid, int> tree = RedBlackTree<Guid, int>.Create();
			//var datas = CreateRanData(6);
			var datas = CreateFixData();

			PrintData(datas);
			AddTest(tree, datas);

			tree.Print();

			RemoveTest(tree, datas);

			//BlackDeepCheck(tree);
			//CheckRedNode(tree);
			//OrderCheck(tree);
		}
	}
}
