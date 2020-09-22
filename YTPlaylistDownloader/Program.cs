using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;

namespace YTPlaylistDownloader
{
    class Program
    {
        
        private static List<string> linkoviPesama;
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube playlist donwloader");
            Console.WriteLine("--- Nemanja Radoičić ---");
            Console.WriteLine("--- 2020 ---\n\n");
            bool ispravno;
            IWebDriver driver;
            do
            {
                Console.WriteLine("Please input the link of the playlist, press ENTER and wait");
                Console.WriteLine("*NOTE* Playlist must be public!");
                Console.Write("link: ");
                string link = Console.ReadLine();
                driver = new ChromeDriver();
                ispravno = preuzmiLinkove(ref driver, link);
                if (!ispravno)
                {
                    driver.Quit();
                    Console.WriteLine("This link isn't valid or playlist isn't public!\n\n");
                }
            } while (!ispravno);
            Console.WriteLine("\n\nStarting download");
            skiniPesme(ref driver);
            Console.WriteLine("If all downloads are finnished, press any key to exit!");
            Console.ReadLine();
            driver.Quit();
        }

        private static bool preuzmiLinkove(ref IWebDriver driver, string linkPlayListe)
        {
            driver.Navigate().GoToUrl(linkPlayListe);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement contents;
            try
            {
                contents = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("contents")));
            }
            catch
            {
                return false;
            }
            IWebElement brojPesama = driver.FindElement(By.XPath("//*[@id='stats']/yt-formatted-string[1]/span[1]"));
            int brPesama = Convert.ToInt32(brojPesama.Text);
            ReadOnlyCollection<IWebElement> list;
            do
            {
                list = contents.FindElements(By.Id("content"));
                Actions actions = new Actions(driver);
                actions.MoveToElement(list[list.Count - 1]);
                actions.Perform();
            } while (list.Count != brPesama);
            linkoviPesama = new List<string>();
            foreach (IWebElement l in list)
            {
                IWebElement a = l.FindElement(By.TagName("a"));
                linkoviPesama.Add(a.GetAttribute("href"));
                //Console.WriteLine(a.GetAttribute("href"));
            }
            return true;
        }

        private static bool skiniPesme(ref IWebDriver driver)
        {
            for (int i = 0; i < linkoviPesama.Count; i++)
            {
                driver.Navigate().GoToUrl("https://ytmp3.cc/en13/");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var tabovi = driver.WindowHandles;
                if (tabovi.Count > 1)
                {
                    driver.SwitchTo().DefaultContent();
                    //driver.Close();//da se ugasi onaj tab novi sto iskoci
                }
                IWebElement input;
                try
                {
                    input = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("input")));
                }
                catch
                {
                    return false;
                }
                IWebElement submit = driver.FindElement(By.Id("submit"));

                Console.Write((i + 1) + " of " + linkoviPesama.Count);

                input.SendKeys(linkoviPesama[i]);
                submit.Click();

                /*Actions actions = new Actions(driver);
                actions.SendKeys(Keys.Escape);//da se skloni ono za notifikacije
                actions.Perform();*/

                IWebElement dugmad = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("buttons")));
                IWebElement download = dugmad.FindElement(By.LinkText("Download"));
                download.Click();

                Console.WriteLine(" Pushed to download!");

                tabovi = driver.WindowHandles;
                if (tabovi.Count > 1)
                {
                    driver.SwitchTo().DefaultContent();
                    //driver.Close();//da se ugasi onaj tab novi sto iskoci
                }
            }
            return true;
        }
    }
}
