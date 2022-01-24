using System;

namespace SABIO.ClientApi.Types
{
    public class ApiPathAttribute : Attribute
    {
        public string Path { get; set; }

        public ApiPathAttribute(string path)
        {
            Path = path;
        }
    }
}