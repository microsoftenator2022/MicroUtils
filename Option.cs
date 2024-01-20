﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace MicroUtils.Functional
{
    public readonly record struct Option<T>() : IEquatable<T?>, IEnumerable<T> where T : notnull
    {
        public readonly T? MaybeValue = default!;
        public readonly T Value => MaybeValue ?? throw new NullReferenceException();
        public readonly bool IsSome = false;
        public bool IsNone => !IsSome;

        private Option(T value) : this()
        {
            MaybeValue = value;
            IsSome = true;
        }

        public static readonly Option<T> None = default;
        public static Option<T> Some(T value) => new(value);

        public override string ToString() => this.IsSome ? $"Some {this.Value}" : "None";

        public bool Equals(T? other) => this switch
        {
            var some when some.IsSome => some.Value.Equals(other),
            _ => other is null,
        };

        public static bool operator ==(Option<T> a, T? b) => a.Equals(b);
        public static bool operator !=(Option<T> a, T? b) => !a.Equals(b);
        public static bool operator ==(T? a, Option<T> b) => b.Equals(a);
        public static bool operator !=(T? a, Option<T> b) => !b.Equals(a);

        public IEnumerator<T> GetEnumerator()
        {
            if (IsSome)
                yield return MaybeValue!;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public static class Option
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> Some<T>(T value) where T : notnull => Option<T>.Some(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> None<T>() where T : notnull => Option<T>.None;

        public static Option<T> OfObj<T>(T? obj) where T : notnull =>
            obj switch
            {
                not null => Some(obj),
                _ => None<T>()
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> ToOption<T>(this T? obj) where T : notnull => OfObj(obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? ToObj<T>(Option<T> option) where T : notnull => option.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSome<T>(Option<T> option) where T : notnull => option.IsSome;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNone<T>(Option<T> option) where T : notnull => option.IsNone;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<A> Return<A>(A value) where A : notnull => Some(value);

        public static Func<Option<A>, Option<B>> Bind<A, B>(Func<A, Option<B>> binder)
            where A : notnull
            where B : notnull =>
            option => option switch
            {
                var some when some.IsSome => binder(some.Value),
                _ => Option<B>.None
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<B> Bind<A, B>(this Option<A> option, Func<A, Option<B>> binder)
            where A : notnull
            where B : notnull =>
            Bind(binder)(option);

        public static Func<Option<A>, Option<B>> Lift<A, B>(Func<A, B> f)
            where A : notnull
            where B : notnull =>
            option => option switch
            {
                var some when some.IsSome => ToOption(f(some.Value)),
                _ => Option<B>.None
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<B> Map<A, B>(this Option<A> option, Func<A, B> f)
            where A : notnull
            where B : notnull =>
            Lift(f)(option);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<A> OrElseWith<A>(this Option<A> option, Func<Option<A>> orElseThunk)
            where A : notnull =>
            option.IsSome ? option : orElseThunk();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<A> OrElse<A>(this Option<A> option, Option<A> orElse)
            where A : notnull =>
            option.IsSome ? option : orElse;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<Option<A>, Option<B>> Apply<A, B>(Option<Func<A, B>> lifted)
            where A : notnull
            where B : notnull =>
            option => lifted.Bind(f => option.Map(f));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<B> Apply<A, B>(this Option<Func<A, B>> lifted, Option<A> option)
            where A : notnull
            where B : notnull =>
            Apply(lifted)(option);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<C> Apply2<A, B, C>(this Option<Func<A, B, C>> lifted, Option<A> optionA, Option<B> optionB)
            where A : notnull
            where B : notnull
            where C : notnull =>
            lifted.Map(Functional.Curry).Apply(optionA).Apply(optionB);

        public static IEnumerable<U> Choose<T, U>(this IEnumerable<T> source, Func<T, Option<U>> chooser) where U : notnull =>
            source.SelectMany(x => chooser(x));

        public static Option<T> TryHead<T>(this IEnumerable<T> source) where T : notnull
        {
            foreach (var x in source)
                return Some(x);

            return None<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> TryFind<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : notnull =>
            source.Where(predicate).TryHead();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DefaultWith<T>(this Option<T> option, Func<T> defaultThunk)
            where T : notnull =>
            option switch
            {
                var some when some.IsSome => some.Value,
                _ => defaultThunk()
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DefaultValue<T>(this Option<T> option, T defaultValue) where T : notnull =>
            option switch
            {
                var some when some.IsSome => some.Value,
                _ => defaultValue
            };
    }
}
