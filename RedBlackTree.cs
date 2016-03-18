using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree
{
	public abstract class SignableElement<TValue>
	{
		protected TValue _Value;
		public TValue Value
		{
			get { return _Value; }
			set 
			{
				_Value = value;
				if (_UpdateFunc != null)
					_UpdateFunc(this);				
			}
		}	
		private Guid _Sign;
		protected Action<SignableElement<TValue>> _UpdateFunc;

		public bool SetSign(Guid sign, Action<SignableElement<TValue>> fun)
		{
			if (sign == null)
			{
				_Sign = sign;
				_UpdateFunc += fun;
				return true;
			}
			return false;
		}

		public bool CheckSign(Guid sign)
		{
			return _Sign == sign;
		}

		public SignableElement(TValue value)
		{
			_Value = value;
		} 
	}
	
	public class RankElement<TKey, TValue> : SignableElement<TValue>
		where TValue : IComparable
	{
		private TKey _Key;
		public TKey Key { get { return _Key; } }
		public RankElement(TKey key, TValue value) : base(value)
		{
			_Key = key;
		}
	}

	public class RedBlackTree<TKey, TValue>
		where TValue : IComparable
	{
		private Dictionary<TKey, Node> _NodeDic = new Dictionary<TKey, Node>();
		private Node _Root;
		private Node _NIL;
		private Guid _Sign;

		public static RedBlackTree<TKey, TValue> Create()
		{
			var tree = new RedBlackTree<TKey, TValue>();
			tree._NIL = new Node();
			tree._NIL.Color = Color.BLACK;
			return tree;
		}

		public void SignUpdate(SignableElement<TValue> element) 
		{
		}

		public void Regist(RankElement<TKey, TValue> element)
		{
			element.SetSign(_Sign, SignUpdate);
		}

		public void CreateNode(RankElement<TKey, TValue> element)
		{
			var node = Node.Create(element);
		}

		#region Add
		public bool Add(RankElement<TKey, TValue> element)
		{
			if (_NodeDic.ContainsKey(element.Key))
			{
				return false;
			}
			var node = Node.Create(element);
			node.LeftSon = _NIL;
			node.RightSon = _NIL;
			_NodeDic.Add(element.Key, node);
			
			if (_Root == null)
			{
				_Root = node;
			}
			else 
			{
				Node nextNode = _Root;
				Node curNode = null;
				do
				{
					curNode = nextNode;
					nextNode = curNode.Element.Value.CompareTo(element.Value) > 0 ? curNode.LeftSon : curNode.RightSon;
				} while (nextNode != _NIL);
				node.Parent = curNode;
				if (curNode.Element.Value.CompareTo(element.Value) > 0)
				{
					curNode.LeftSon = node;
				}
				else
				{
					curNode.RightSon = node;
				}
			}
			Insert_Case1(node);
			return true;
		}

		private void Insert_Case1(Node node)
		{
			if (node.Parent == null)
			{
				node.Color = Color.BLACK;
			}
			else
			{
				Insert_Case2(node);
			}
		}

		private void Insert_Case2(Node node)
		{
			if (node.Parent.Color != Color.BLACK)
			{
				Insert_Case3(node);
			}
		}

		private void Insert_Case3(Node node)
		{
			var uncle = node.Uncle;
			if (uncle != null && uncle.Color == Color.RED)
			{
				node.Parent.Color = Color.BLACK;
				uncle.Color = Color.BLACK;
				node.GrandParent.Color = Color.RED;
				Insert_Case1(node.GrandParent);
				return;
			}
			Insert_Case4(node);
		}

		private void Insert_Case4(Node node)
		{
			var grandParent = node.GrandParent;
			if (node == node.Parent.RightSon && node.Parent == grandParent.LeftSon)
			{
				LeftRotate(node.Parent);
				node = node.LeftSon;
			}
			else if (node == node.Parent.LeftSon && node.Parent == grandParent.RightSon)
			{
				RightRotate(node.Parent);
				node = node.RightSon;
			}
			Insert_Case5(node);
		}

		private void Insert_Case5(Node node)
		{
			var grandParent = node.GrandParent;
			node.Parent.Color = Color.BLACK;
			grandParent.Color = Color.RED;
			if (node == node.Parent.LeftSon)
			{
				RightRotate(grandParent);
			}
			else
			{
				LeftRotate(grandParent);
			}
		}
		#endregion

		#region delete
		public bool Remove(RankElement<TKey, TValue> element)
		{
			if (!_NodeDic.ContainsKey(element.Key))
			{
				return false;
			}
			var node = _NodeDic[element.Key];
			//_NodeDic.Remove(element.Key);
			var replaceNode = FindMin(node.RightSon);
			if (replaceNode == null)
			{
				replaceNode = FindMax(node.LeftSon);
			}
			if (replaceNode == null)
			{
				_Root = null;
			}
			else
			{
				var tmp = node.Element;
				node.Element = replaceNode.Element;
				replaceNode.Element = tmp;
				DeleteOneChild(replaceNode);
			}			
			return true;
		}

		//删除一个节点,这个节点要满足条件:儿子节点(不包括NIL节点)数量 <= 1
		private void DeleteOneChild(Node node)
		{
			var son = node.RightSon == _NIL ? node.LeftSon : node.RightSon;			
			//交换 node son
			if (son == node.Parent.LeftSon)
			{
				node.Parent.LeftSon = son;
			}
			else
			{ 
				node.Parent.RightSon = son;
			}
			son.Parent = node.Parent;

			
			if (node.Color == Color.BLACK)
			{
				if (son.Color == Color.RED)
					son.Color = Color.BLACK;
				else
					delete_case1(son);
			}
		}

		private void delete_case1(Node node)
		{
			if (node.Parent != null)
			{
				delete_case2(node);
			}
		}

		private void delete_case2(Node node)
		{
			var sibling = node.Sibing;
			if (sibling.Color == Color.RED)
			{
				node.Parent.Color = Color.RED;
				sibling.Color = Color.BLACK;
				if (node == node.Parent.LeftSon)
				{
					LeftRotate(node.Parent);
				}
				else
				{
					RightRotate(node.Parent);
				}
			}
			delete_case3(node);
		}

		private void delete_case3(Node node)
		{
			var sibling = node.Sibing;
			if (node.Parent.Color == Color.BLACK && sibling.Color == Color.BLACK &&
				sibling.LeftSon.Color == Color.BLACK && sibling.RightSon.Color == Color.BLACK)
			{
				sibling.Color = Color.RED;
				delete_case1(node.Parent);
			}
			else
			{
				delete_case4(node);
			}
		}

		private void delete_case4(Node node)
		{
			var sibling = node.Sibing;
			if (node.Parent.Color == Color.RED && sibling.Color == Color.BLACK &&
				sibling.LeftSon.Color == Color.BLACK && sibling.RightSon.Color == Color.BLACK)
			{
				sibling.Color = Color.RED;
				node.Parent.Color = Color.BLACK;
			}
			else
			{
				delete_case5(node);
			}
		}

		private void delete_case5(Node node)
		{
			var sibling = node.Sibing;
			if (sibling.Color == Color.BLACK)
			{
				if (node == node.Parent.LeftSon && sibling.RightSon.Color == Color.BLACK
					&& sibling.LeftSon.Color == Color.RED)
				{
					sibling.Color = Color.RED;
					sibling.LeftSon.Color = Color.BLACK;
					RightRotate(sibling);
				}
				else if (node == node.Parent.RightSon && sibling.LeftSon.Color == Color.BLACK
					&& sibling.RightSon.Color == Color.RED)
				{
					sibling.Color = Color.RED;
					sibling.RightSon.Color = Color.BLACK;
					LeftRotate(sibling);
				}
			}
			delete_case6(node);
		}

		private void delete_case6(Node node)
		{
			var sibling = node.Sibing;
			sibling.Color = node.Parent.Color;
			node.Parent.Color = Color.BLACK;
			if (node == node.Parent.LeftSon)
			{
				sibling.RightSon.Color = Color.BLACK;
				LeftRotate(node.Parent);
			}
			else
			{
				sibling.LeftSon.Color = Color.BLACK;
				RightRotate(node.Parent);
			}
		}
		#endregion

		#region base operate
		private void LeftRotate(Node node)
		{
			var rightSon = node.RightSon;
			var parent = node.Parent;
			if (parent == null)
			{
				_Root = rightSon;
			}
			else if (node == parent.RightSon)
			{
				parent.RightSon = rightSon;
			}
			else
			{
				parent.LeftSon = rightSon;
			}
			rightSon.LeftSon.Parent = node;
			node.Parent = rightSon;
			node.RightSon = rightSon.LeftSon;
			rightSon.Parent = parent;
			rightSon.LeftSon = node;
		}

		private void RightRotate(Node node)
		{
			var leftSon = node.LeftSon;
			var parent = node.Parent;
			if (parent == null)
			{
				_Root = leftSon;
			}
			else if (node == parent.RightSon)
			{
				parent.RightSon = leftSon;
			}
			else
			{
				parent.LeftSon = leftSon;
			}
			leftSon.RightSon.Parent = node;
			node.Parent = leftSon;
			node.LeftSon = leftSon.RightSon;
			leftSon.Parent = parent;
			leftSon.RightSon = node;
		}

		private void SwapValue(Node node, Node other)
		{
			var color = node.Color;
			node.Color = other.Color;
			other.Color = color;
			var value = node.Element;
			node.Element = other.Element;
			other.Element = node.Element;
		}

		private Node FindMax(Node node)
		{
			if (node == null || node == _NIL)
				return null;
			while (node.RightSon != _NIL)
			{
				node = node.RightSon;
			}
			return node;
		}
		
		private Node FindMin(Node node)
		{
			if (node == null || node == _NIL)
				return null;
			while (node.LeftSon != _NIL)
			{
				node = node.LeftSon;
			}
			return node;
		}


		//private Node Find(RankElement<TKey, TValue> element)
		//{
		//	if(_Root == null)
		//	{
		//		return null;
		//	}
		//	var curNode = _Root;
		//	int cmp = curNode.Element.Value.CompareTo(element.Value);
		//	while(curNode != _NIL && cmp != 0)
		//	{
		//	}
		//}

		#endregion

		private enum Color : byte
		{ 
			RED = 0,
			BLACK = 1,
		}

		private class Node 
		{
			public Node Parent;
			public Node LeftSon;
			public Node RightSon;
			public Color Color = Color.RED;
			public RankElement<TKey, TValue> Element;

			public void Print(Node nil)
			{
				Console.WriteLine("id:{0} color{1} value {2} left {3} right {4}", Element.Key, Color, Element.Value, 
					LeftSon == nil ? "nil" : LeftSon.Element.Key.ToString(), 
					RightSon == nil ? "nil" : RightSon.Element.Key.ToString());
			}

			public void PrintValue()
			{
				Console.WriteLine(Element.Value);
			}

			public Node GrandParent
			{
				get { return Parent != null ? Parent.Parent : null; }
			}
			public Node Uncle
			{
				get 
				{
					if (GrandParent == null)
						return null;
					return Parent == GrandParent.LeftSon ? GrandParent.RightSon : GrandParent.LeftSon;
				}
			}
			public Node Sibing
			{
				get 
				{
					if (Parent == null)
						return null;
					if (this == Parent.LeftSon)
						return Parent.RightSon;
					else
						return Parent.LeftSon;
				}
			}
			public static Node Create(RankElement<TKey, TValue> value)
			{
				var node = new Node();
				node.Element = value;
				return node;
			}
			public static Node CreateNIL()
			{
				var node = new Node();
				node.Color = Color.BLACK;
				return node;
			}
		}

		#region check
		private static int _BlackDeep = 0;

		// 红色节点的儿子节点一定是黑色
		public bool CheckRedNode()
		{
			if (_Root == null)
			{
				return true; 
			}
			return CheckRedNodeSearch(_Root);
		}

		private bool CheckRedNodeSearch(Node cur_node)
		{
			if (cur_node.Color == Color.RED)
				return cur_node.LeftSon.Color == Color.BLACK && cur_node.RightSon.Color == Color.BLACK;
			return true;
		}

		public bool OrderCheck()
		{
			if (_Root == null)
			{
				return true;
			}
			return CheckOrderIterator(_Root);
		}

		private bool CheckOrderIterator(Node cur_node)
		{
			var leftSon = cur_node.LeftSon;
			if (leftSon != _NIL && cur_node.Element.Value.CompareTo(leftSon.Element.Value) < 0)
			{
				return false;	
			}
			var rightSon = cur_node.RightSon;
			if (rightSon != _NIL && cur_node.Element.Value.CompareTo(rightSon.Element.Value) > 0)
			{
				return false;
			}
			return true;
		}

		public bool BlackDeepCheck()
		{
			_BlackDeep = 0;
			if (_Root == null)
			{
				return false;
			}
			return true;
		}

		private bool BlackDeepCheckIterator(Node cur_node, int cur_deep)
		{
			if (cur_node == _NIL)
			{
				if (0 == _BlackDeep)
				{
					_BlackDeep = cur_deep;
					return true;
				}
				else
				{
					if (_BlackDeep != cur_deep)
					{
						return false;
						Console.WriteLine("black deep error");
					}
					else
					{
						return true;
					}
				}
			}

			bool ret = true;
			if (cur_node.LeftSon != _NIL)
			{
				ret &= BlackDeepCheckIterator(cur_node.LeftSon, cur_deep + 1);
			}
			if (cur_node.RightSon != _NIL)
			{
				ret &= BlackDeepCheckIterator(cur_node.RightSon, cur_deep + 1);
			}
			return ret;
		}

		public void Print()
		{
			if (_Root == _NIL)
				return;
			Iterator(_Root);
		}

		public void PrintValue()
		{
			Console.WriteLine("-------------------");
			if (_Root == _NIL)
				return;
			IteratorValue(_Root);
		}

		private void Iterator(Node node)
		{
			if (node.LeftSon != _NIL)
			{
				Iterator(node.LeftSon);
			}
			node.Print(_NIL);
			if (node.RightSon != _NIL)
			{
				Iterator(node.RightSon);
			}
		}

		private void IteratorValue(Node node)
		{
			if (node.LeftSon != _NIL)
			{
				IteratorValue(node.LeftSon);
			}
			node.PrintValue();
			if (node.RightSon != _NIL)
			{
				IteratorValue(node.RightSon);
			}
		}
		#endregion
	}
}
