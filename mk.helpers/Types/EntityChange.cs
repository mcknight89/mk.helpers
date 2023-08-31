using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers.Types
{
    public class EntityChange
    {
        public string Property { get; set; }
        public Type PropertyType { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public bool NestedProperty { get; set; }
    }

}
