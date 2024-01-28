using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
// ReSharper disable CheckNamespace

namespace PsychoUnity.Manager
{
    public class RecordManager : Singleton<RecordManager>
    {
        private readonly Dictionary<string, Recorder<IRecorderData>> _recorderDic = new();

        public void Create<T>(string recorderName, T data) where T : IRecorderData
        {
            if (_recorderDic.ContainsKey(recorderName))
            {
                Debug.Log($"Recorder {recorderName} already exist");
            }
            else
            {
                _recorderDic.Add(recorderName, new Recorder<IRecorderData>(recorderName, data));
            }
        }

        public void Write(string recorderName)
        {
            if (Verify(recorderName))
            {
                _recorderDic[recorderName].Write();
            }
        }
        
        public void Start(string recordName){}
        
        public void Stop(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Stop();
            }
        }

        public void Pause(string recordName)
        {
        }
        
        public void Continue(string recordName){}
        
        public void Destroy(string recordName){}
        

        private bool Verify(string recorderName)
        {
            if (_recorderDic.ContainsKey(recorderName)) return true;
            
            Debug.Log($"Recorder {recorderName} does not exist");
            return false;
        }
    }

    internal class Recorder<T> where T : IRecorderData
    {
        private readonly T _data;
        
        private readonly FieldInfo[] _fieldInfos;
        private readonly StreamWriter _writer;
        
        internal Recorder(string recorder, T data)
        {
            _data = data;
            _fieldInfos = data.GetType().GetFields();
            
            // Create file
            _writer = new StreamWriter(GenFile(recorder));

            // Generate and write header
            var builder = new StringBuilder();
            foreach (var variable in _fieldInfos)
            {
                Debug.Log($"{variable.FieldType}");
                if (variable.FieldType == typeof(Vector3))
                {
                    var name = variable.Name;
                    builder.Append($"{name}.X, {name}.Y, {name}.Z,");
                }
                else if(variable.FieldType == typeof(Vector2))
                {
                    var name = variable.Name;
                    builder.Append($"{name}.X, {name}.Y,");
                }
                else
                {
                    builder.Append(variable.Name);
                    builder.Append(",");
                }
            }
            
            _writer.WriteLine(builder.ToString());
        }

        internal void Write()
        {
            var builder = new StringBuilder();

            foreach (var variable in _fieldInfos)
            {
                if (variable.FieldType == typeof(Vector3))
                {
                    builder.Append(GetVector3(variable.GetValue(_data)));
                }
                else if (variable.FieldType == typeof(Vector2))
                {
                    builder.Append(GetVector2(variable.GetValue(_data)));
                }
                else
                {
                    builder.Append(variable.GetValue(_data));
                    builder.Append(",");
                }
            }
            
            _writer.WriteLine(builder.ToString());
        }

        private static string GetVector3(object data)
        {
            var vector = (Vector3)data;
            return $"{vector.x}, {vector.y}, {vector.z},";
        }
        
        private static string GetVector2(object data)
        {
            var vector = (Vector2)data;
            return $"{vector.x}, {vector.y},";
        }
        
        internal void Start(){}

        internal void Stop()
        {
            _writer.Close();
        }
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
            }

            return path;
        }
        
    }
    
    public interface IRecorderData
    {
        
    }
}