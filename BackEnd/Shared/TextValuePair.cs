using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Shared
{
    public class TextValuePair<T>(string text, T value)
    {
        public string Text { get; set; } = text;
        public T Value { get; set; } = value;

    }
}
