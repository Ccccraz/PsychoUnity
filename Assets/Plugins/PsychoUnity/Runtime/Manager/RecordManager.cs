using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class RecordManager : Singleton<RecordManager>
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

    internal class Recorder<T> where T : struct
    {
        private FieldInfo[] _members;
        private StreamWriter _writer;
        
        private T _data;
        
        internal void SetRecorder(ref T data, string recorder)
        {
            _data = data;
            _members = data.GetType().GetFields();
            
            _writer = new StreamWriter(GenFile(recorder));

            var builder = new StringBuilder();
            foreach (var variable in _members)
            {
                builder.Append(variable.Name);
                builder.Append(",");
            }
            
            _writer.WriteLine(builder.ToString());
        }

        internal void Write()
        {
            var builder = new StringBuilder();

            foreach (var variable in _members)
            {
                builder.Append(variable.GetValue(_data));
                builder.Append(",");
            }
            
            _writer.WriteLine(builder.ToString());
        }
        internal void Start(){}
        internal void Stop(){}
        internal void Pause(){}
        internal void Continue(){}
        internal void Destroy(){}
        
        private static string GenFile(string recorder, [CanBeNull] string custom = null, string prefix = "Assets/Data/")
        {
            var path = $"{prefix}/{DateTime.Now:yyyyMMdd}/{DateTime.Now:HHmmss}_{recorder}_{custom}.csv";

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
    }
}