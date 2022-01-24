using System;

namespace SABIO.ClientApi.Responses.Types
{
    [Flags]
    public enum Permission
    {
        Delete = 8,
        Update = 4,
        Create = 2,
        Read = 1,
        ReadWrite = 15,
        None = 0
    }
}