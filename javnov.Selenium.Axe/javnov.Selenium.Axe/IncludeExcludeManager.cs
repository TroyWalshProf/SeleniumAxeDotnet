using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace javnov.Selenium.Axe
{
    /// <summary>
    /// Handle all initialization, serialization and validations for includeExclude aXe object.
    /// For more info check this: https://github.com/dequelabs/axe-core/blob/master/doc/API.md#include-exclude-object
    /// </summary>
    public class IncludeExcludeManager
    {
        private static readonly object syncObject = new object();
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private List<string[]> _includeList;
        private List<string[]> _excludeList;

        /// <summary>
        /// Include the given selectors, i.e "#foo", "ul.bar .target", "div"
        /// </summary>
        /// <param name="selectors">Selectors to include</param>
        public void Include(params string[] selectors)
        {
            ValidateParameters(selectors);
            InitializeList(_includeList);
            _includeList.Add(selectors);
        }

        /// <summary>
        /// Include the given selectors, i.e "frame", "div.foo"
        /// </summary>
        /// <param name="selectors">Selectors to exclude</param>
        public void Exclude(params string[] selectors)
        {
            ValidateParameters(selectors);
            InitializeList(_excludeList);
            _excludeList.Add(selectors);
        }

        /// <summary>
        /// Indicate if we have more than one entry on include list or we have entries on exclude list
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasMoreThanOneSelectorsToIncludeOrSomeToExclude()
        {
            bool hasMoreThanOneSelectorsToInclude = _includeList != null && _includeList.Count > 1;
            bool hasSelectorsToExclude = _excludeList != null && _excludeList.Count > 0;

            return hasMoreThanOneSelectorsToInclude || hasSelectorsToExclude;
        }

        /// <summary>
        /// Indicate we have one entry on the include list
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasOneItemToInclude() => _includeList != null && _includeList.Count == 1;

        /// <summary>
        /// Get first selector of the first entry on include list
        /// </summary>
        /// <returns></returns>
        public string GetFirstItemToInclude()
        {
            if (_includeList == null || _includeList.Count == 0)
                throw new InvalidOperationException("You must add at least one selector to include");

            return _includeList.First().First();
        }

        /// <summary>
        /// Serialize this instance on JSON format
        /// </summary>
        /// <returns>This instance serialized in JSON format</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, jsonSerializerSettings);
        }

        private void InitializeList(List<string[]> listToInitialize)
        {
            if (listToInitialize == null)
                listToInitialize = new List<string[]>();
        }

        private static void ValidateParameters(string[] selectors)
        {
            if (selectors == null)
                throw new ArgumentNullException(nameof(selectors));

            if (selectors.Any(string.IsNullOrEmpty))
                throw new ArgumentException("There is some items null or empty", nameof(selectors));
        }
    }
}
