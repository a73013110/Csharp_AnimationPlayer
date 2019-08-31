//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using CefSharp;
//using CefSharp.OffScreen;

//namespace AnimationPlayer.Objects
//{
//    public class CefBrowser
//    {
//        /// <summary>
//        /// 網頁頁面
//        /// </summary>
//        public ChromiumWebBrowser Browser { get; private set; }

//        public CefBrowser()
//        {
//            var settings = new CefSettings()
//            {
//                CachePath = "./CefSharp/Cache",
//                //LogSeverity = LogSeverity.Verbose,
//                //RemoteDebuggingPort = 12345,
//            };
//            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
//            CefSharpSettings.ShutdownOnExit = true;
//            Browser = new ChromiumWebBrowser();
//            SpinWait.SpinUntil(() => Browser.IsBrowserInitialized);
//        }
//    }
//}
