using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keryhe.Pipelines.Steps;

namespace Keryhe.Pipelines.Service.Steps
{
    internal class StringTest1 : Step<string>
    {
        public override bool Run(ref string data)
        {
            if(data.StartsWith("Cancel"))
            {
                return false;
            }
            if(data.StartsWith("Fail"))
            {
                throw new Exception("Step Failed");
            }
            StringBuilder sb = new StringBuilder(data);
            sb.Append("_additionalData");
            sb.Append(data.Substring(data.Length - 1));
            data = sb.ToString();

            return true;
        }
    }
}
