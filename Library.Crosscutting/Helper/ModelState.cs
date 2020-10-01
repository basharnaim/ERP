using System;

namespace Library.Crosscutting.Helper
{
    [Flags]
    public enum ModelState
    {
        Added = 1,
        Modified = 2,
        Deleted = Modified | Added, // 0x00000003
        Unchanged = 4,
        Detached = Unchanged | Added // 0x00000005
    }
}