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
        IEnumerable<PlayerStat> ScrapePlayerStats();
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
        public IEnumerable<PlayerStat> ScrapePlayerStats()
        {
            return Scrape<IEnumerable<PlayerStat>>((driver) => 
            {
                driver.Navigate().GoToUrl(CssSelectors.FplStats);
                var playerPageCount = GetPageCount(driver.FindElement(By.CssSelector(CssSelectors.PlayerPages)).Text);

                return GetGameweekPlayerStats(driver, playerPageCount);
            });
        }

        private int GetPageCount(string rawText) => int.Parse(rawText.Split(' ', 3).Last());

        private IEnumerable<PlayerStat> GetGameweekPlayerStats(IWebDriver driver, int pageCount)
        {
            var playerStats = new List<PlayerStat>();
            for (int i = 0; i < pageCount; i++)
            {
                foreach (var btn in driver.FindElements(By.CssSelector(CssSelectors.PlayerInfoButtons)))
                {
                    if (btn.Text.Contains("View player information")) 
                    {
                        var hasAlertSection = btn.Text.Contains("chance of playing");
                        btn.Click();
                        var rawPlayerData = driver.FindElement(By.CssSelector(CssSelectors.GetPlayerStatsDialog(hasAlertSection))).Text;
                        playerStats.Add(ParsePlayerStat(rawPlayerData));

                        // Close the modal dialog
                        driver.FindElement(By.CssSelector(CssSelectors.PlayerModalClose)).Click();
                    }
                }
                driver.FindElements(By.CssSelector(CssSelectors.TableFooterButtons)).First(x => x.Text == "Next").Click();
            }

            return playerStats;
        }

        private PlayerStat ParsePlayerStat(string rawPlayerStat)
        {
            var splitStat = rawPlayerStat.Split(Environment.NewLine);
            // "Mohamed Salah\nMidfielder\nLiverpool\nForm\n9.0\nGW6\n7pts\nTotal\n57pts\nPrice\n£12.6\nTSB\n59.1%\nICT Rank for Midfielders\nInfluence\n1 of 250\nCreativity\n3 of 250\nThreat\n1 of 250\nICT Index\n1 of 250\nOverall ICT Rank\nICT Index\n1 of 611"
            return new PlayerStat(splitStat[0], splitStat[2], splitStat[1], double.Parse(splitStat[4]), int.Parse(splitStat[6].Replace("pts", "")),
                int.Parse(splitStat[8].Replace("pts", "")), splitStat[10], splitStat[12], splitStat[15], splitStat[17], splitStat[19], splitStat[21], splitStat[24]);
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
