using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AnimationPlayer.Properties;
using System.Windows;

namespace AnimationPlayer.Objects
{
    public class Chrome
    {
        /// <summary>
        /// 網頁頁面
        /// </summary>
        public ChromeDriver Browser { get; set; }
        /// <summary>
        /// 取得Chrome是否初始化
        /// </summary>
        private bool IsInitial = false;

        public Chrome(bool IsPlayer = false, Point? Position = null)
        {
            Task.Run(() =>  // 使用別的執行緒初始化, 避免UI卡住
            {
                ChromeOptions options = new ChromeOptions();
                options.AddExcludedArgument("enable-automation");   // 關閉"正在受到自動化軟件控制"的訊息
                options.AddAdditionalCapability("useAutomationExtension", false);   // 關閉"停用開發人員模式"的訊息
                options.AddArguments("no-sandbox");
                if (IsPlayer)
                {
                    options.AddArguments($"--window-size={Settings.Default.Chrome_Player_Width},{Settings.Default.Chrome_Player_Height}");
                    options.AddArguments($"--window-position={Position.Value.X - int.Parse(Settings.Default.Chrome_Player_Width) / 2},{Position.Value.Y - int.Parse(Settings.Default.Chrome_Player_Height) / 2}");
                    options.AddArguments("load-extension=D:/Program Library/ChromeEx_M3u8Player/M3u8Player");
                }
                if (!IsPlayer) options.AddArguments("headless");   // 隱藏瀏覽器視窗
                //options.AddArguments("remote-debugging-port=12345");  // Debug
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);   // 不顯示圖片
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true; // 隱藏CMD視窗
                DateTime beforeChromeDriverStartTime = DateTime.Now;
                try
                {
                    this.Browser = new ChromeDriver(service, options);
                    this.BrowserClosedListenerAndHandler();
                    this.IsInitial = true;
                }
                catch (InvalidOperationException)   // 瀏覽器出現時馬上關閉, 過一陣子會觸發例外狀況
                {
                    foreach(Process chromedriver in Process.GetProcessesByName("chromedriver"))
                    {
                        double difference = (chromedriver.StartTime - beforeChromeDriverStartTime).TotalSeconds;
                        if (difference > 0 && difference < 0.1) // chromedriver啟動時間必定晚於beforeChromeDriverStartTime, 因此排除掉difference < 0的狀況
                        {
                            chromedriver.Kill();
                            break;
                        }
                    }
                }
            });
        }

        ~Chrome()
        {
            if (this.Browser != null) this.Browser.Quit();
        }

        public async Task Initial()
        {
            await Task.Run(() =>
            {
                SpinWait.SpinUntil(() => IsInitial);
            });
        }

        public async Task Load(string url)
        {
            await Task.Run(() =>
            {
                this.Browser.Navigate().GoToUrl(url);
            });
        }

        public async Task ExecuteScript(string script)
        {
            await Task.Run(() =>
            {
                this.Browser.ExecuteScript(script);
            });
        }

        public string GetSource()
        {
            return this.Browser.PageSource;
        }

        public void Quit()
        {
            this.Browser.Quit();
        }

        public void BrowserClosedListenerAndHandler()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var tmp = this.Browser.CurrentWindowHandle;
                        SpinWait.SpinUntil(() => false, 2000);
                    }
                    catch (WebDriverException)
                    {
                        this.Browser.Quit();
                        break;
                    }
                }
            });
        }
    }
}
