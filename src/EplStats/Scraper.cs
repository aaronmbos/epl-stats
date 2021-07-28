using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;

namespace EplStats
{
  public interface IScraper
    {
        IEnumerable<string> ScrapeTeams();
        IEnumerable<string> ScrapePlayers();
    }

    public static class CssSelectors
    {
        public static string FplStats => @"https://fantasy.premierleague.com/statistics";
        public static string PlayerInfoButtons => "div#root div:nth-child(2) table tbody button";
        public static string TeamsFromDropdown => "#filter optgroup[label=\"By Team\"]";
        public static string PlayerModalClose => "div#root-dialog > div[role=\"presentation\"] > dialog > div div:nth-child(1) button";
    }

    public class Scraper : IScraper
    {
        public IEnumerable<string> ScrapePlayers()
        {
            return Scrape<IEnumerable<string>>((driver) => 
            {
                driver.Navigate().GoToUrl(CssSelectors.FplStats);
                foreach (var btn in driver.FindElements(By.CssSelector(CssSelectors.PlayerInfoButtons)))
                {
                    if (btn.Text.Contains("View player information")) 
                    {
                        btn.Click();
                        // Close the modal dialog
                        driver.FindElement(By.CssSelector(CssSelectors.PlayerModalClose)).Click();
                    }
                }
                return new List<string>();
            });
        }

        public IEnumerable<string> ScrapeTeams()
        {
            return Scrape<IEnumerable<string>>((driver) => 
            {
                driver.Navigate().GoToUrl(CssSelectors.FplStats);
                var teams = driver.FindElement(By.CssSelector(CssSelectors.TeamsFromDropdown)).Text;
                return teams.Split(Environment.NewLine);
            });
        }

        private T Scrape<T>(Func<IWebDriver, T> scrapeFunc)
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                try
                {
                    return scrapeFunc(driver);
                }
                finally 
                {
                    driver.Quit();
                }
            }
        }
    }
}
