using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroUtils
{
    /// <summary>
    /// Simple stack data structure implemented as a singly-linked list
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="Head">Top of the stack/head of the list</param>
    /// <param name="Tail">Rest of the stack/list</param>
    public record class MicroStack<T>(T Head, MicroStack<T> Tail) : IEnumerable<T>
    {
        public MicroStack(T Head) : this(Head, Empty) { }

        /// <summary>
        /// Constant root list node
        /// </summary>
        public static readonly MicroStack<T> Empty = new(default!, null!);

        /// <summary>
        /// True if this is the empty node
        /// </summary>
        public bool IsEmpty => this == Empty;

        /// <summary>
        /// Add an item to the head of the list
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <returns>New <see cref="MicroStack{T}"/> with value added at the head</returns>
        public MicroStack<T> Push(T value) => new(value, this);
        
        /// <summary>
        /// Remove the top item and return this item and a new <see cref="MicroStack{T}"/> containing the remaining items
        /// </summary>
        /// <returns>Tuple containing the head of the <see cref="Stack{T}"/> and the new <see cref="Stack{T}"/></returns>
        /// <exception cref="InvalidOperationException">The stack is empty</exception>
        public (T head, MicroStack<T> tail) Pop() =>
            this.IsEmpty ?
            throw new InvalidOperationException("Cannot pop from an empty stack") :
            (this.Head, this.Tail);

        public void Deconstruct(out T head, out MicroStack<T> tail) => (head, tail) = this.Pop();

        /// <summary>
        /// Enumerable sequence of items in this <see cref="MicroStack{T}"/>
        /// </summary>
        public IEnumerable<T> Enumerable
        {
            get
            {
                if (this.IsEmpty)
                    yield break;

                yield return this.Head;

                foreach (var value in Tail)
                    yield return value;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.Enumerable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Enumerable.GetEnumerator();

        /// <summary>
        /// Constructs a <see cref="MicroStack{T}"/> from an enumerable sequence of items.
        /// This method enumerates the entire <see cref="IEnumerable{T}"/> sequence immediately
        /// </summary>
        /// <param name="source">Source items</param>
        /// <returns><see cref="MicroStack{T}"/> containing the items from the <see cref="IEnumerable{T}"/> sequence</returns>
        public static MicroStack<T> OfEnumerable(IEnumerable<T> source)
        {
            var stack = Empty;
            foreach (var item in source.Reverse())
                stack = (item, stack);

            return stack;
        }

        //public static MicroStack<T> operator +(T value, MicroStack<T> stack) => stack.Push(value);

        public static implicit operator MicroStack<T>((T head, MicroStack<T> tail) cons) => new(cons.head, cons.tail);
    }
}
