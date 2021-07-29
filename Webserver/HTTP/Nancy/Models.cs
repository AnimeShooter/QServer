using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.Webserver.HTTP.Nancy
{
    public struct APIResponse<T>
    {
        public T Result;
        public string Message;
    }
}
