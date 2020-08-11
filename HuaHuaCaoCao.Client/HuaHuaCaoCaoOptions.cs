using System;
using System.Collections.Generic;
using System.Text;

namespace HuaHuaCaoCao.Client
{
    public class HuaHuaCaoCaoOptions
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int FetchCount { get; set; }
        public Uri? ApiEndpoint { get; set; }
        public string? Region { get; set; }
    }
}
