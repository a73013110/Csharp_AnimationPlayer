using AnimationPlayer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using static AnimationPlayer.GlobalFunctions.Json;

namespace AnimationPlayer.GlobalFunctions
{
    public static class AnimationObjectJson
    {
        private static string path = @"./AnimationObjects.json";

        /// <summary>
        /// 新增或更新動畫至Json檔
        /// </summary>
        /// <param name="animationObject">欲更新的動畫</param>
        public static void SetAnimationObjectToJson(AnimationObject animationObject)
        {
            // 檢索速度最快, 使用的HashSet必須自定義Comparer
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList == null) animationList = new HashSet<AnimationObject>(new AnimationObjectComparer()); // 創建新的HashSet
            else
            {
                animationList = animationList.ToHashSet(new AnimationObjectComparer()); // 設置Comparer
                if (animationList.Contains(animationObject)) animationList.Remove(animationObject); // 若動畫已存在HashSet, 先刪除
            }
            animationList.Add(animationObject); // 添加資料進去HashSet
            WriteToFile(animationList, path);  // 儲存
        }

        /// <summary>
        /// 透過網址檢查並取得該動畫
        /// </summary>
        /// <param name="href">動畫網址</param>
        /// <returns>If 存在欲取得的動畫: return該動畫; Else: return NULL</returns>
        public static AnimationObject GetAnimationObjectFromJson(string href)
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList != null)
            {
                animationList = animationList.ToHashSet(new AnimationObjectComparer()); // 設置Comparer
                AnimationObject animationObject = new AnimationObject { Href = href };  // New 一個欲取得的動畫物件
                if (animationList.TryGetValue(animationObject, out animationObject)) return animationObject;    // 若有該動畫就回傳
            }
            return null;
        }

        /// <summary>
        /// 取得全部動畫
        /// </summary>
        /// <returns>所有動畫</returns>
        public static HashSet<AnimationObject> GetAnimationObjectHashSetFromJson()
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList == null) return new HashSet<AnimationObject>(new AnimationObjectComparer());  // 回傳空的HashSet
            else return animationList.ToHashSet(new AnimationObjectComparer()); // 回傳所有動畫
        }

        /// <summary>
        /// 刪除指定動畫
        /// </summary>
        /// <param name="animationObject">欲刪除的動畫</param>
        public static void RemoveAnimationObjectFromJson(AnimationObject animationObject)
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList != null)
            {
                animationList = animationList.ToHashSet(new AnimationObjectComparer()); // 設置Comparer
                if (animationList.Contains(animationObject))    // 若存在該動畫
                {
                    animationList.Remove(animationObject);    // 刪除
                    WriteToFile(animationList, path);  // 儲存
                }
            }
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
