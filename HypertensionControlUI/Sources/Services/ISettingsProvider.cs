using HypertensionControlUI.Properties;

namespace HypertensionControlUI.Services
{
    public interface ISettingsProvider
    {
        string ConnectionString { get; set; }
    
    }

    class SettingsProvider : ISettingsProvider
    {
        public string ConnectionString
        {
            get { return Settings.Default.ConnectionString; }
            set { Settings.Default.ConnectionString = value; }
        }
    }
}