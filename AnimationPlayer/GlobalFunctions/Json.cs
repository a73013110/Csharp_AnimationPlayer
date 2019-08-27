using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AnimationPlayer.Objects;

namespace AnimationPlayer.GlobalFunctions
{
    public static class Json
    {
        /// <summary>
        /// 讀取Json檔
        /// </summary>
        /// <typeparam name="T">若指定型別, 自動將Return轉換為此型別</typeparam>
        /// <param name="path">讀取位置</param>
        /// <returns>指定型別</returns>
        public static T ReadFromFile<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
        /// <summary>
        /// 儲存Json檔
        /// </summary>
        /// <typeparam name="T">可不指定型別</typeparam>
        /// <param name="saveObject">欲儲存的物件</param>
        /// <param name="path">儲存位置</param>
        public static void WriteToFile<T>(T saveObject, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(saveObject));
        }
    }
}
