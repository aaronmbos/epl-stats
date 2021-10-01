using System.Linq;
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
        public static string GetPlayerStatsDialog(bool hasAlertSection) =>
            hasAlertSection ? "div#root-dialog > div[role=\"presentation\"] > dialog > div > div + div div:nth-child(2)" :
                "div#root-dialog > div[role=\"presentation\"] > dialog > div div:nth-child(2) div";
        public static string PlayerPages => "div#root div:nth-child(2) > div > div > div div:nth-child(3)";
        public static string TableFooterButtons => "div#root div:nth-child(2) > div > div > div button";
    }

    public class Scraper : IScraper
    {
        public IEnumerable<string> ScrapePlayers()
        {
            return Scrape<IEnumerable<string>>((driver) => 
            {
                driver.Navigate().GoToUrl(CssSelectors.FplStats);
                
                var playerPageCount = GetPageCount(driver.FindElement(By.CssSelector(CssSelectors.PlayerPages)).Text);
                var rawPlayerData = new List<string>();

                for (int i = 0; i < playerPageCount; i++)
                {
                    foreach (var btn in driver.FindElements(By.CssSelector(CssSelectors.PlayerInfoButtons)))
                    {
                        if (btn.Text.Contains("View player information")) 
                        {
                            var hasAlertSection = btn.Text.Contains("chance of playing");
                            btn.Click();
                            rawPlayerData.Add(driver.FindElement(By.CssSelector(CssSelectors.GetPlayerStatsDialog(hasAlertSection))).Text);

                            // Close the modal dialog
                            driver.FindElement(By.CssSelector(CssSelectors.PlayerModalClose)).Click();
                        }
                    }
                    driver.FindElements(By.CssSelector(CssSelectors.TableFooterButtons)).First(x => x.Text == "Next").Click();
                }
                // Name
                // Position
                // Team
                // skip Form
                // skip Gameweek points
                // skip Total points
                // skip Price
                // skip skip Influence
                // skip Creativity
                // skip Threat
                // skip skip Overall ICT
                return rawPlayerData;
            });
        }

        private int GetPageCount(string rawText) => int.Parse(rawText.Split(' ', 3).Last());

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
