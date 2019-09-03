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

        public Chrome()
        {
            Task.Run(() =>  // 使用別的執行緒初始化, 避免UI卡住
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("no-sandbox");
                options.AddArguments("headless");   // 隱藏瀏覽器視窗
                //options.AddArguments("remote-debugging-port=12345");  // Debug
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);   // 不顯示圖片
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true; // 隱藏CMD視窗

                this.Browser = new ChromeDriver(service, options);
                this.IsInitial = true;
            });
        }
        /// <summary>
        /// 解構子, 關閉Chrome Drive程序
        /// </summary>
        ~Chrome()
        {
            this.Browser.Quit();
        }
        /// <summary>
        /// 等待直到初始化完畢
        /// </summary>
        /// <returns></returns>
        public async Task Initial()
        {
            await Task.Run(() =>
            {
                SpinWait.SpinUntil(() => IsInitial);
            });
        }
        /// <summary>
        /// 載入指定網頁
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task Load(string url)
        {
            await Task.Run(() =>
            {
                this.Browser.Navigate().GoToUrl(url);
            });
        }
        /// <summary>
        /// 執行JavaScript
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public async Task ExecuteScript(string script)
        {
            await Task.Run(() =>
            {
                this.Browser.ExecuteScript(script);
            });
        }
        /// <summary>
        /// 取得網頁原始碼
        /// </summary>
        /// <returns></returns>
        public string GetSource()
        {
            return this.Browser.PageSource;
        }
        /// <summary>
        /// 關閉ChromeDrive
        /// </summary>
        public void Quit()
        {
            this.Browser.Quit();
        }
    }
}
