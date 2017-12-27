using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedexRatesAndTime.Core {
    public class GoogleChrome : AbstractBrowserDriver {
        private ChromeOptions _driverOptions;
        public GoogleChrome() {
            _driverOptions = new ChromeOptions();
            _driverOptions.AddUserProfilePreference("download.default_directory", Utils.GetTmpDirectory());
            _driverOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            _driverOptions.AddUserProfilePreference("profile.default_content_settings.popups", "0");
            _driverOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            _driverOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
        }

        public IWebDriver Start() {
            var driverPath = AppDomain.CurrentDomain.BaseDirectory;
            return new ChromeDriver(driverPath, _driverOptions, CommandTimeout);
        }

    }
}
