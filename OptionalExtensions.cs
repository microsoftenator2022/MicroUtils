using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;

using MicroUtils.Functional;

namespace MicroUtils.Optional;
public static class Optional
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Map<T, TResult>(this Optional<T> optional, Func<T, TResult> map) =>
        optional.HasValue ? (Optional<TResult>)map(optional.Value) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> OfObj<T>(this T? value) where T : notnull =>
        value is not null ? (Optional<T>)value : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ToObj<T>(this Optional<T> optional) => optional.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> Return<T>(T value) => value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Bind<T, TResult>(this Optional<T> optional, Func<T, Optional<TResult>> binder) =>
        optional.HasValue ? binder(optional.Value) : default;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<Optional<T>, Optional<TResult>> Lift<T, TResult>(Func<T, TResult> f) => x => x.Map(f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> OrElseWith<T>(this Optional<T> optional, Func<Optional<T>> orElseThunk) =>
        optional.HasValue ? optional.Value : orElseThunk();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> OrElse<T>(this Optional<T> optional, Optional<T> ifNoValue) =>
        optional.HasValue ? optional : ifNoValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Apply<T, TResult>(this Optional<Func<T, TResult>> lifted, Optional<T> optional) =>
        lifted.HasValue ? optional.Map(lifted.Value) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<Optional<T>, Optional<TResult>> Apply<T, TResult>(Optional<Func<T, TResult>> lifted) =>
        optional => lifted.Apply(optional);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Apply<T1, T2, TResult>(Optional<Func<T1, T2, TResult>> lifted,
        Optional<T1> optional1,
        Optional<T2> optional2) =>
        lifted.Map(F.Curry).Apply(optional1).Apply(optional2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<U> Choose<T, U>(this IEnumerable<T> source, Func<T, Optional<U>> chooser) =>
        source.Select(chooser).FirstOrDefault(optional => optional.HasValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> TryFirst<T>(this IEnumerable<T> source) =>
        source.Select((x => (Optional<T>)x)).FirstOrDefault();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> TryFind<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(predicate).TryFirst();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DefaultWith<T>(this Optional<T> optional, Func<T> defaultThunk) =>
        optional.HasValue ? optional.Value : defaultThunk();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DefaultValue<T>(this Optional<T> optional, T defaultValue) =>
        optional.HasValue ? optional.Value : defaultValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FromFSharpOption<T>(Microsoft.FSharp.Core.FSharpOption<T> fSharpOption) where T : notnull =>
        Microsoft.FSharp.Core.FSharpOption<T>.get_IsSome(fSharpOption) ? (Optional<T>)fSharpOption.Value : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FromFSharpValueOption<T>(Microsoft.FSharp.Core.FSharpValueOption<T> fSharpValueOption) where T : notnull =>
        fSharpValueOption.IsValueSome ? (Optional<T>)fSharpValueOption.Value : default;
}
