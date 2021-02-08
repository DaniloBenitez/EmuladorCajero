using System;
using System.Configuration;
using System.Xml;

namespace EmuladorCajero
{
    public static class eCatConfig
    {
        internal static string Host()
        {
            string bRet = null;
            /*XmlDocument objDoc = new XmlDocument();
            string file = Environment.CurrentDirectory + @"NetworkConfig.xml";
            try
            {
                objDoc.Load(file);
            }
            catch (Exception ex)
            {
                //Log.Action.LogErrorFormat("NetworkConfig file load exception: {0}", ex.Message);
            }

            try
            {
                XmlNode nodes = objDoc.SelectSingleNode(@"/NetConfig/NetworkService/Sessions/Session[@name='ATMP1']/Address/@url");
                bRet = nodes.Value;
            }
            catch 
            {
                //Log.Action.LogErrorFormat("ATMP1 session is not exist in network config");
            }
            */
            bRet = GetValue("Host");
            return bRet;
        }
        internal static string Password()
        {
            string bRet = null;
            XmlDocument objDoc = new XmlDocument();
            string file = AppDomain.CurrentDomain.BaseDirectory + @"config\NetworkConfig.xml";
            try
            {
                objDoc.Load(file);
            }
            catch (Exception ex)
            {
                //Log.Action.LogErrorFormat("NetworkConfig file load exception: {0}", ex.Message);
            }

            try
            {
                XmlNode nodes = objDoc.SelectSingleNode(@"/NetConfig/NetworkService/Sessions/Session[@name='ATMP1']/Connection/@password");
                bRet = nodes.Value;
            }
            catch 
            {
                //Log.Action.LogErrorFormat("ATMP1 session is not exist in network config");
            }

            return bRet;
        }
        internal static long GetTransactionNumber()
        {
            long bRet = 0;
            XmlDocument objDoc = new XmlDocument();
            string file = AppDomain.CurrentDomain.BaseDirectory + @"config\TransactionConfig.xml";
            try
            {
                objDoc.Load(file);
            }
            catch (Exception ex)
            {
                //Log.Action.LogErrorFormat("Transaction Config file load exception: {0}", ex.Message);
            }

            try
            {
                XmlNode nodes = objDoc.SelectSingleNode(@"/Config/Transaction/@transactionNumber");
                long.TryParse(nodes.Value, out bRet);
            }
            catch (System.Exception ex)
            {
                //Log.Action.LogErrorFormat("transactionNumber is not exist in transaction config");
            }

            return bRet;
        }
        internal static void IncrementTransactionNumber()
        {
            XmlDocument objDoc = new XmlDocument();
            string file = AppDomain.CurrentDomain.BaseDirectory + @"config\TransactionConfig.xml";
            try
            {
                objDoc.Load(file);
            }
            catch (Exception ex)
            {
                //Log.Action.LogErrorFormat("Transaction Config file load exception: {0}", ex.Message);
            }

            try
            {
                XmlNode nodes = objDoc.SelectSingleNode(@"/Config/Transaction/@transactionNumber");
                long bRet = 0;
                long.TryParse(nodes.Value, out bRet);
                objDoc.SelectSingleNode(@"/Config/Transaction/@transactionNumber").Value = (bRet + 1).ToString();
                objDoc.Save(file);
            }
            catch (System.Exception ex)
            {
                //Log.Action.LogErrorFormat("transactionNumber is not exist in transaction config");
            }

        }
        internal static string GetValue(string key)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
                return config.AppSettings.Settings[key].Value;
            else
                return null;
        }
        internal static void AddValue(string key,string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {   
                config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key,value));
                config.Save(ConfigurationSaveMode.Full);
            }
        }
        internal static void ProtectConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string provider = "RsaProtectedConfigurationProvider";

            ConfigurationSection connStrings = config.AppSettings;

            if (connStrings != null)
            {
                if (!connStrings.SectionInformation.IsProtected)
                {
                    if (!connStrings.ElementInformation.IsLocked)
                    {
                        connStrings.SectionInformation.ProtectSection(provider);
                        connStrings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }
        }
    }
}
