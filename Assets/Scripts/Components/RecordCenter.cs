using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Components
{
    public class RecordCenter : Singleton<RecordCenter>
    {
        private readonly Dictionary<string, StreamWriter> _recorderDic = new Dictionary<string, StreamWriter>();

        private static string GenFile(string recorder, string monkeyName)
        {
            const string prefix = "Assets/Data/";
            var path = $"{prefix}/{DateTime.Now:yyyyMMdd}/{DateTime.Now:HHmmss}_{recorder}_{monkeyName}.csv";

            if (!File.Exists(path))
            {
                var directorPath = Path.GetDirectoryName(path);
                
                if (string.IsNullOrEmpty(directorPath))
                {
                    throw new Exception("未获得有效目录");
                }
                
                if (!Directory.Exists(directorPath))
                {
                    Directory.CreateDirectory(directorPath);
                }
                
                try
                {
                    File.Create(path).Close();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            };
            
            return path;
        }
        
        private static string GenData<T>(ref T dataList) where T : struct
        {
            var dataBuilder = new StringBuilder();
            foreach (var data in (IEnumerable)dataList)
            {
                dataBuilder.Append(data);
                dataBuilder.Append(",");
            }

            return dataBuilder.ToString();
        }
        
        public void AddRecorder(string recorder, string monkeyName, IEnumerable<string> titleName)
        {
            if (!_recorderDic.ContainsKey(recorder))
            {
                _recorderDic.Add(recorder, new StreamWriter(GenFile(recorder, monkeyName)));
                var titleBuilder = new StringBuilder();
                foreach (var item in titleName)
                {
                    titleBuilder.Append(item);
                    titleBuilder.Append(",");
                }
                _recorderDic[recorder].WriteLine(titleBuilder.ToString());
            }
        }

        public void Write<T>(string recorder, ref T dataList) where T : struct
        {
            if (_recorderDic.TryGetValue(recorder, out var value))
            {
                value.WriteLine(GenData(ref dataList));
            }
        }

        public void Close(string recorder)
        {
            if (_recorderDic.TryGetValue(recorder, out var value))
            {
               value.Close();
            }
        }

        public void Clear()
        {
            foreach (var item in _recorderDic)
            {
               Close(item.Key); 
            }
            
            _recorderDic.Clear();
        }
    }
}