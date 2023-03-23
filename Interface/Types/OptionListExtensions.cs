using System.Collections.Generic;
using Domain.Extensions;

namespace Interface.Types;

public static class OptionListExtensions {
  public static IList<Option<T>> AddWhen<T>(this IList<Option<T>> options, Option<T> option, bool predicate = false) =>
    options.Also(_ => options.Add(option), predicate);
}