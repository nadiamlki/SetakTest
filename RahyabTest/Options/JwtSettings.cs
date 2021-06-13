using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Options
{
    public class JwtSettings
    {
        public TimeSpan TokenLifetime { get; set; }
        public string Secret { get; set; }  
    }
}
