using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelTag.NET
{
    public class TagComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            if (x.Value.ToString() == y.Value.ToString())
            {
                return true;
            }
            else return false;
        }

        public int GetHashCode(Tag tag)
        {
            return 0;
        }
    }
}
