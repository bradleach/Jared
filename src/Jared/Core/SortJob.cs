using System;
using System.Collections.Generic;
using System.Configuration;

namespace Jared.Core
{
    public abstract class SortJob : Job
    {
        protected virtual string JobName { get { return this.GetType().Name; } }

        protected string GetWorkersSetting()
        {
            string setting = String.Format("{0}:Workers", JobName);
            return ""; // ConfigurationManager.AppSettings[setting];
        }

        protected string GetSkipToSetting()
        {
            string setting = String.Format("{0}:SkipTo", JobName);
            return ""; // ConfigurationManager.AppSettings[setting];
        }

        protected HashSet<string> GetExcludeSet()
        {
            string settingKey = String.Format("{0}:Exclude", JobName);
            string settingValue = ""; //ConfigurationManager.AppSettings[settingKey];

            return (String.IsNullOrWhiteSpace(settingValue))
                ? new HashSet<string>()
                : new HashSet<string>(settingValue.Split(','));
        }
    }
}
