//
// ArrayDimensionCollection.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Generated by /CodeGen/cecil-gen.rb do not edit
// Fri Apr 21 16:46:46 CEST 2006
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil {

	using System;
	using System.Collections;

	using Mono.Cecil.Cil;

	public sealed class ArrayDimensionCollection : IArrayDimensionCollection {

		IList m_items;
		ArrayType m_container;

		public event ArrayDimensionEventHandler OnArrayDimensionAdded;
		public event ArrayDimensionEventHandler OnArrayDimensionRemoved;

		public ArrayDimension this [int index] {
			get { return m_items [index] as ArrayDimension; }
			set { m_items [index] = value; }
		}

		object IIndexedCollection.this [int index] {
			get { return m_items [index]; }
		}

		public ArrayType Container {
			get { return m_container; }
		}

		public int Count {
			get { return m_items.Count; }
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		public ArrayDimensionCollection (ArrayType container)
		{
			m_container = container;
			m_items = new ArrayList ();
		}

		public void Add (ArrayDimension value)
		{
			if (OnArrayDimensionAdded != null && !this.Contains (value))
				OnArrayDimensionAdded (this, new ArrayDimensionEventArgs (value));
			m_items.Add (value);
		}

		public void Clear ()
		{
			if (OnArrayDimensionRemoved != null)
				foreach (ArrayDimension item in this)
					OnArrayDimensionRemoved (this, new ArrayDimensionEventArgs (item));
			m_items.Clear ();
		}

		public bool Contains (ArrayDimension value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (ArrayDimension value)
		{
			return m_items.IndexOf (value);
		}

		public void Insert (int index, ArrayDimension value)
		{
			if (OnArrayDimensionAdded != null && !this.Contains (value))
				OnArrayDimensionAdded (this, new ArrayDimensionEventArgs (value));
			m_items.Insert (index, value);
		}

		public void Remove (ArrayDimension value)
		{
			if (OnArrayDimensionRemoved != null && this.Contains (value))
				OnArrayDimensionRemoved (this, new ArrayDimensionEventArgs (value));
			m_items.Remove (value);
		}

		public void RemoveAt (int index)
		{
			if (OnArrayDimensionRemoved != null)
				OnArrayDimensionRemoved (this, new ArrayDimensionEventArgs (this [index]));
			m_items.RemoveAt (index);
		}

		public void CopyTo (Array ary, int index)
		{
			m_items.CopyTo (ary, index);
		}

		public IEnumerator GetEnumerator ()
		{
			return m_items.GetEnumerator ();
		}
	}
}
