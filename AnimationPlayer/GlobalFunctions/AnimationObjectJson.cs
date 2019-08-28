using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AnimationPlayer.Objects;
using static AnimationPlayer.GlobalFunctions.Json;

namespace AnimationPlayer.GlobalFunctions
{
    public static class AnimationObjectJson
    {
        /// <summary>
        ///  動畫路徑的Key
        /// </summary>
        public enum Animation
        {
            RecentWatch
        }
        /// <summary>
        /// 動畫路徑的Dictionary
        /// </summary>
        public static Dictionary<Animation, string> FilePath = new Dictionary<Animation, string>
        {
            { Animation.RecentWatch, @"./Recent_Watch.json" },
        };

        /// <summary>
        /// 更新近期觀看的動畫至Json檔
        /// </summary>
        /// <param name="animationObject">欲更新的動畫</param>
        /// <param name="path">檔案路徑</param>
        public static void UpdateRecentWatch(AnimationObject animationObject, string path)
        {
            // 檢索速度最快, 使用的HashSet必須自定義Comparer
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);
            if (animationList == null)
            {
                animationList = new HashSet<AnimationObject>(new AnimationObjectComparer());
            }
            else
            {
                animationList = animationList.ToHashSet(new AnimationObjectComparer());
                if (animationList.Contains(animationObject)) animationList.Remove(animationObject); // 若資料已存在HashSet, 先刪除
            }
            animationList.Add(animationObject); // 添加資料進去HashSet
            WriteToFile(animationList, path);  // 儲存
        }

        /// <summary>
        /// 檢查該動畫近期是否看過
        /// </summary>
        /// <param name="href">動畫網址</param>
        /// <param name="path">檔案路徑</param>
        /// <returns>If最近看過: return欲取得的動畫; Else: return NULL</returns>
        public static AnimationObject CheckRecentWatch(string href, string path)
        {
            // 檢索速度最快, 使用的HashSet必須自定義Comparer
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);
            if (animationList == null) return null;
            else
            {
                animationList = animationList.ToHashSet(new AnimationObjectComparer());
                AnimationObject animationObject = new AnimationObject { Href = href };
                if (animationList.TryGetValue(animationObject, out animationObject)) return animationObject;
                return null;
            }            
        }

        /// <summary>
        /// 取得最近觀看動畫
        /// </summary>
        /// <param name="path">檔案路徑</param>
        /// <returns>最近所有看過的動畫</returns>
        public static HashSet<AnimationObject> GetRecentWatch(string path)
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);
            if (animationList == null) return new HashSet<AnimationObject>(new AnimationObjectComparer());
            else return animationList.ToHashSet(new AnimationObjectComparer());
        }
    }

    /// <summary>
    /// HashSet<AnimationObject> override的方法
    /// </summary>
    public class AnimationObjectComparer : IEqualityComparer<AnimationObject>
    {
        public bool Equals(AnimationObject x, AnimationObject y)
        {
            return x.Href.Equals(y.Href, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(AnimationObject obj)
        {
            return obj.Href.GetHashCode();
        }
    }
}
