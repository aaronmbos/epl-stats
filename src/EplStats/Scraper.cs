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

    public class Scraper : IScraper
    {
        private const string FplStatsUrl = @"https://fantasy.premierleague.com/statistics";
        private const string PlayersSelector = "div#root div:nth-child(2)"; // Getting to the main div
        private const string TeamsSelector = "#filter optgroup[label=\"By Team\"]";

        public IEnumerable<string> ScrapePlayers()
        {
            return Scrape<IEnumerable<string>>((driver) => 
            {
                driver.Navigate().GoToUrl(FplStatsUrl);
                var players = driver.FindElement(By.CssSelector(PlayersSelector)).Text;
                return players.Split(Environment.NewLine);
            });
        }

        public IEnumerable<string> ScrapeTeams()
        {
            return Scrape<IEnumerable<string>>((driver) => 
            {
                driver.Navigate().GoToUrl(FplStatsUrl);
                var teams = driver.FindElement(By.CssSelector(TeamsSelector)).Text;                
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
