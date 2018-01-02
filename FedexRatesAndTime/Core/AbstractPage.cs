using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FedexRatesAndTime.Core {
    public abstract class AbstractPage : IDisposable {
        bool disposed = false;

        protected const int PageLoadTimeoutInSeconds = 10;
        protected IWebDriver _driver;

        protected AbstractPage(string pageUrl) {
            var browser = new Firefox();
            _driver = browser.Start();
            GoToUrl(pageUrl);
        }

        protected void takeScreenshotOfCurrentPage(String filename, String outputPath) {

            filename = GetSafeFilename(filename);
            var filenameWithoutExt = outputPath + @"\" + filename;
            GetEntereScreenshot().Save(filenameWithoutExt + ".jpeg");
        }

        protected void saveCurrentPage(String filename, String outputPath) {

            filename = GetSafeFilename(filename);
            var filenameWithoutExt = outputPath + @"\" + filename;
            var htmlSourceFile = filenameWithoutExt + ".html";
            File.WriteAllText(htmlSourceFile, _driver.PageSource);
        }

        protected string GetSafeFilename(string filename) {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        protected Bitmap GetEntereScreenshot() {

            Bitmap stitchedImage = null;
            try {
                long totalwidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.offsetWidth");//documentElement.scrollWidth");
                long totalHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return  document.body.parentNode.scrollHeight");

                int totalWidth = (int)totalwidth1;
                int totalHeight = (int)totalHeight1;

                // Get the Size of the Viewport
                long viewportWidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.clientWidth");//documentElement.scrollWidth");
                long viewportHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return window.innerHeight");//documentElement.scrollWidth");

                int viewportWidth = (int)viewportWidth1;
                int viewportHeight = (int)viewportHeight1;

                // Split the Screen in multiple Rectangles
                List<Rectangle> rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (int i = 0; i < totalHeight; i += viewportHeight) {
                    int newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight) {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (int ii = 0; ii < totalWidth; ii += viewportWidth) {
                        int newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth) {
                            newWidth = totalWidth - ii;
                        }

                        // Create and add the Rectangle
                        Rectangle currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }

                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                Rectangle previous = Rectangle.Empty;
                foreach (var rectangle in rectangles) {
                    // Calculate the Scrolling (if needed)
                    if (previous != Rectangle.Empty) {
                        int xDiff = rectangle.Right - previous.Right;
                        int yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        ((IJavaScriptExecutor)_driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        System.Threading.Thread.Sleep(200);
                    }

                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (MemoryStream memStream = new MemoryStream(screenshot.AsByteArray)) {
                        screenshotImage = Image.FromStream(memStream);
                    }

                    // Calculate the Source Rectangle
                    Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                    // Copy the Image
                    using (Graphics g = Graphics.FromImage(stitchedImage)) {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }

                    // Set the Previous Rectangle
                    previous = rectangle;
                }
            } catch (Exception ex) {
                // handle
            }
            return stitchedImage;
        }

        protected void GoToUrl(string url) {
            var pageLoaded = false;
            _driver.Navigate().GoToUrl(url);
            for (var i = 0; i < 60; i++) {
                Thread.Sleep(100);
                if (((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete")) {
                    pageLoaded = true;
                    break;
                }
                Thread.Sleep(100);
            }

            if (!pageLoaded) {
                throw new Exception("Page was not with complete state)!");
            }
        }

        protected void ScrollTo(int xPosition = 0, int yPosition = 0) {
            var js = String.Format("window.scrollTo({0}, {1})", xPosition, yPosition);
            ((IJavaScriptExecutor)_driver).ExecuteScript(js);
        }

        protected void ScrollTo(IWebDriver driver, IWebElement element) {
            if (element.Location.Y > 50) {
                ScrollTo(0, element.Location.Y - 200); // Make sure element is in the view but below the top navigation pane
            }
        }

        protected void WaitForAjax() {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(PageLoadTimeoutInSeconds));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver1).ExecuteScript("return document.readyState").Equals("complete"));
        }

        protected void waitForPageLoad() {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(PageLoadTimeoutInSeconds));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver1).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void Dispose() {
            if (disposed)
                return;
            _driver.Quit();
            disposed = true;
        }
    }
}
