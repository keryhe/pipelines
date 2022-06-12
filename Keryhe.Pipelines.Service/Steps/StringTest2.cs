using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keryhe.Pipelines.Steps;

namespace Keryhe.Pipelines.Service.Steps
{
    internal class StringTest2 : Step<string>
    {
        public override bool Run(ref string data)
        {
            data = data.Substring(data.IndexOf("_") + 1);
            
            return true;
        }
    }
}
