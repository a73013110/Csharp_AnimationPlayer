using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

        public Chrome()
        {
            Task.Run(() =>
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("headless");   // 隱藏瀏覽器視窗
                options.AddArguments("no-sandbox");
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);   // 不顯示圖片
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true; // 隱藏CMD視窗
                this.Browser = new ChromeDriver(service, options);
                this.IsInitial = true;
            });
        }

        ~Chrome()
        {
            this.Browser.Close();
            this.Browser.Dispose();
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
    }
}
