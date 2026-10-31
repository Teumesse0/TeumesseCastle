//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.Configuration;

namespace PingCastle.Healthcheck
{
    internal class HoneyPotSettings : ConfigurationSection
    {
        static HoneyPotSettings cachedSettings = null;
        public static HoneyPotSettings GetEncryptionSettings()
        {
            if (cachedSettings == null)
                cachedSettings = ConfigurationManager.GetSection("honeyPotSettings") as HoneyPotSettings;
            return cachedSettings;
        }

        [ConfigurationProperty("HoneyPots", IsRequired = false)]
        public HoneyPotsCollection HoneyPots
        {
            get
            {
                return base["HoneyPots"] as HoneyPotsCollection;
            }
        }
    }

    [ConfigurationCollection(typeof(SingleHoneyPotSettings), AddItemName = "HoneyPot")]
    internal class HoneyPotsCollection : ConfigurationElementCollection
    {
        public HoneyPotsCollection()
        {

        }

        public SingleHoneyPotSettings this[int index]
        {
            get { return (SingleHoneyPotSettings)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(SingleHoneyPotSettings pluginConfig)
        {
            BaseAdd(pluginConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SingleHoneyPotSettings();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SingleHoneyPotSettings)element).samAccountName;
        }

        public void Remove(SingleHoneyPotSettings pluginConfig)
        {
            BaseRemove(pluginConfig.samAccountName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

    }

    public class SingleHoneyPotSettings : ConfigurationElement
    {
        [ConfigurationProperty("samAccountName")]
        public string samAccountName
        {
            get
            {
                return base["samAccountName"] as string;
            }
            set
            {
                base["samAccountName"] = value;
            }
        }

        [ConfigurationProperty("distinguishedName")]
        public string distinguishedName
        {
            get
            {
                return base["distinguishedName"] as string;
            }
            set
            {
                base["distinguishedName"] = value;
            }
        }
    }
}
