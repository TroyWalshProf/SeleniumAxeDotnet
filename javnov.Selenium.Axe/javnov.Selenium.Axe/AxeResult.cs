using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    public class AxeResult
    {
        public IReadOnlyList<AxeResultItem> Violations { get; private set; }
        public IReadOnlyList<AxeResultItem> Passes { get; private set; }

        public AxeResult(JToken violationsToken, JToken passesToken)
        {
            Violations =  violationsToken.ToObject<List<AxeResultItem>>();
            Passes = passesToken.ToObject<List<AxeResultItem>>();
        }
    }
}
