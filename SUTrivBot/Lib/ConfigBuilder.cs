using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Lib
{
    public static class ConfigBuilder
    {
        // TODO: Load configuration values from appsettings.json

        private static Config _currentConfig = null;

        public static Config Build()
        {
            // TODO: Build Config or return existing

            if (_currentConfig != null)
                return _currentConfig;

            _currentConfig = new Config();
            return _currentConfig;
        }
    }
}
