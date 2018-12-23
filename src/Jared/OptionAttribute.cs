using System;
using System.Collections.Generic;
using System.Text;

namespace Jared
{
    public class OptionAttribute : Attribute
    {
        public OptionAttribute()
        {
            IsDefaultJob = true;
        }

        public OptionAttribute(char shortName, string longName = "", bool isDefaultJob = false)
        {
            IsDefaultJob = isDefaultJob;
            ShortName = shortName;
            LongName = longName;
        }

        public bool IsDefaultJob { get; set; }
        public char ShortName { get; set; }
        public string LongName { get; set; }
    }
}
