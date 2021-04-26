using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;

namespace EplStats
{
  public interface IScraper
    {
        IEnumerable<string> ScrapeTeams();
    }

    public class Scraper : IScraper
    {
        private const string PlayersSelector = "#filter>optgroup[label=\"By Team\"]>option:nth-child(1)";
        private const string TeamsSelector = "";

        public IEnumerable<string> ScrapeTeams()
        {
            using (IWebDriver driver = new FirefoxDriver())
            {
                driver.Navigate().GoToUrl(@"https://fantasy.premierleague.com/statistics");
                var teams = driver.FindElement(By.CssSelector("#filter optgroup[label=\"By Team\"]")).Text;
                driver.Quit();
                
                return teams.Split(Environment.NewLine);
            }
        }
    }
}