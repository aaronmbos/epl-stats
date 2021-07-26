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
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(FplStatsUrl);
                var players = driver.FindElement(By.CssSelector(PlayersSelector)).Text;
                driver.Quit();

                return new List<string>();
            }
        }

        public IEnumerable<string> ScrapeTeams()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(FplStatsUrl);
                var teams = driver.FindElement(By.CssSelector(TeamsSelector)).Text;
                driver.Quit();
                
                return teams.Split(Environment.NewLine);
            }
        }
    }
}