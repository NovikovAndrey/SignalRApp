using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace SignalRAppUITest
{
    public class SignalRUITest
    {
        private ChromeDriver driver;

        [Fact]
        public void ChromeDriverInitialize()
        {
            driver = new ChromeDriver(@".\");
        }
    }
}
