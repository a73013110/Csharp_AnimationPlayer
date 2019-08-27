using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnimationPlayer.UserControls
{
    public partial class VideoPlayerUserControl
    {
        /// <summary>
        /// 任何地方上設定UI
        /// </summary>
        /// <param name="action"></param>
        private void SettingUI(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        /// <summary>
        /// UI Thread上設定Vlc Media Player
        /// 若有用到UI的值, 必須使用Invoke才可以執行
        /// </summary>
        /// <param name="action"></param>
        private void SettingMediaPlayerWithUIValue(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        /// <summary>
        /// 任何地方設定Vlc Media Player
        /// 防止播放器Freeze, 若有參考UI的值請使用: SettingMediaPlayerWithUIValue
        /// </summary>
        /// <param name="action"></param>
        private void SettingMediaPlayer(Action action)
        {
            ThreadPool.QueueUserWorkItem(status => action());
        }
    }
}
