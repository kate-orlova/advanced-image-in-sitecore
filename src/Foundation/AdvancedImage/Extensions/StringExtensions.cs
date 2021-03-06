﻿using System;
using System.Collections.Generic;

namespace AdvancedImage.Extensions
{
    public static class StringExtensions
    {
        public static bool Empty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool ParseOrDefault(this string value, bool defaultValue)
        {
            if (value.Empty())
            {
                return defaultValue;
            }

            return bool.TryParse(value, out var parsedValue)
                ? parsedValue
                : defaultValue;
        }

        public static IEnumerable<TElement> ParseArray<TElement>(this string value, char separator,
            Func<string, ParseResult<TElement>> itemParser)
        {
            if (itemParser == null)
            {
                throw new ArgumentNullException(nameof(itemParser));
            }

            var result = new List<TElement>();

            if (string.IsNullOrEmpty(value)) return result;

            foreach (var item in value.Split(separator))
            {
                try
                {
                    var itemParseResult = itemParser(item);
                    if (itemParseResult.Successful)
                    {
                        result.Add(itemParseResult.Value);
                    }
                }
                catch (OverflowException)
                {
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        public static IEnumerable<int> ParseIntArray(this string value, char separator = ',')
        {
            return value.ParseArray(
                separator,
                s =>
                {
                    var itemResult = new ParseResult<int>
                    {
                        Successful = int.TryParse(s, out var parsedItem),
                        Value = parsedItem
                    };

                    return itemResult;
                });
        }

        public class ParseResult<TValue>
        {
            public TValue Value { get; set; }

            public bool Successful { get; set; }
        }
        public static string Append(this string value, string rigthValue, string separator = "")
        {
            if (string.IsNullOrEmpty(value)) return rigthValue;
            return value + separator + rigthValue;
        }
        public static bool NotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        }
        public static string OrDefault(this string value, string defaultValue)
        {
            return value.NotEmpty() ? value : defaultValue;
        }
    }
}