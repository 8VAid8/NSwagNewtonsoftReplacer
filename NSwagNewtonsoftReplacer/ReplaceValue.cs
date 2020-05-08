using System;
using System.Collections.Generic;
using System.Text;

namespace NSwagNewtonsoftReplacer
{
    public class ReplaceValue
    {
        public string Property { get; set; }
        public Func<string, string> ValueReplacer { get; set; }

        public ReplaceValue(string property, Func<string, string> valueReplacer)
        {
            Property = property;
            ValueReplacer = valueReplacer;
        }
    }
}
