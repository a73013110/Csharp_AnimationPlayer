﻿using System;
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
        public enum Mode
        {
            RecentWatch
        }
        /// <summary>
        /// 動畫路徑的Dictionary
        /// </summary>
        public static Dictionary<Mode, string> FilePath = new Dictionary<Mode, string>
        {
            { Mode.RecentWatch, @"./Recent_Watch.json" },
        };

        /// <summary>
        /// 新增或更新動畫至Json檔
        /// </summary>
        /// <param name="animationObject">欲更新的動畫</param>
        /// <param name="path">檔案路徑</param>
        public static void SetAnimationObjectToJson(AnimationObject animationObject, string path)
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
        /// 檢查並取得該動畫
        /// </summary>
        /// <param name="href">動畫網址</param>
        /// <param name="path">檔案路徑</param>
        /// <returns>If 存在欲取得的動畫: return該動畫; Else: return NULL</returns>
        public static AnimationObject GetAnimationObjectFromJson(string href, string path)
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
        /// <param name="path">檔案路徑</param>
        /// <returns>所有動畫</returns>
        public static HashSet<AnimationObject> GetAnimationObjectHashSetFromJson(string path)
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList == null) return new HashSet<AnimationObject>(new AnimationObjectComparer());  // 回傳空的HashSet
            else return animationList.ToHashSet(new AnimationObjectComparer()); // 回傳所有動畫
        }

        /// <summary>
        /// 刪除指定動畫
        /// </summary>
        /// <param name="path"></param>
        /// <param name="animationObject"></param>
        /// <returns></returns>
        public static void RemoveAnimationObjectFromJson(string path, AnimationObject animationObject)
        {
            HashSet<AnimationObject> animationList = ReadFromFile<HashSet<AnimationObject>>(path);  // 讀取
            if (animationList != null && animationList.Contains(animationObject)) animationList.Remove(animationObject);    // 刪除
            WriteToFile(animationList, path);  // 儲存
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
