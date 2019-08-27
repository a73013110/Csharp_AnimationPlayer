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
    public static class AnimationRecentWatchJson
    {
        public static string RecentWatchPath = @"./Recent_Watch.json";
        /// <summary>
        /// 更新近期觀看的動畫至Json檔
        /// </summary>
        /// <param name="animationObject"></param>
        public static void UpdateRecentWatch(AnimationObject animationObject)
        {
            // 檢索速度最快, 使用的HashSet必須自定義Comparer
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(RecentWatchPath).ToHashSet(new AnimationObjectComparer());
            if (animationList.Contains(animationObject)) animationList.Remove(animationObject); // 若資料已存在HashSet, 先刪除
            animationList.Add(animationObject); // 添加資料進去HashSet
            WriteToFile(animationList, RecentWatchPath);  // 儲存
        }

        /// <summary>
        /// 檢查該動畫近期是否看過
        /// </summary>
        /// <param name="href">動畫網址</param>
        /// <returns></returns>
        public static AnimationObject CheckRecentWatch(string href)
        {
            // 檢索速度最快, 使用的HashSet必須自定義Comparer
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(RecentWatchPath).ToHashSet(new AnimationObjectComparer());
            AnimationObject animationObject = new AnimationObject { Href = href };
            if (animationList.TryGetValue(animationObject, out animationObject)) return animationObject;
            return null;
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
