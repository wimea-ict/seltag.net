using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelTag.NET
{
    public class Tag
    {
        public String Name
        {
            get;
            set;
        }
        public Object Value
        {
            get;
            set;
        }

        public Tag(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
