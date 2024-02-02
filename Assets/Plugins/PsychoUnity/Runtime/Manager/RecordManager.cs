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

        public void Create<T>(string recorderName, T data, [CanBeNull] string custom = null, string prefix = "Assets/Data") where T : IRecorderData
        {
            if (_recorderDic.ContainsKey(recorderName))
            {
                Debug.Log($"Recorder {recorderName} already exist");
            }
            else
            {
                _recorderDic.Add(recorderName, new Recorder<IRecorderData>(recorderName, data, custom, prefix));
            }
        }

        public void Write(string recorderName)
        {
            if (Verify(recorderName))
            {
                _recorderDic[recorderName].Write();
            }
        }

        // TODO Auto recorder
        public void Start(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Start();
            }
        }
        
        // TODO Auto recorder
        public void Stop(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Stop();
            }
        }

        // TODO Auto recorder
        public void Pause(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Pause();
            }
        }

        // TODO Auto recorder
        public void Continue(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Continue();
            }
        }

        // TODO Auto recorder
        public void Destroy(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Destroy();
            }
        }

        ~RecordManager()
        {
            foreach (var variable in _recorderDic)
            {
                variable.Value.Destroy();
            }
        }
        
        // TODO Use exception catching instead of Debug.Log()
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
        
        internal Recorder(string recorder, T data, [CanBeNull] string custom, string prefix)
        {
            _data = data;
            _fieldInfos = data.GetType().GetFields();
            
            // Create file
            _writer = new StreamWriter(GenFile(recorder, custom, prefix));

            // Generate and write header
            var builder = new StringBuilder();
            foreach (var variable in _fieldInfos)
            {
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
        
        // TODO Auto recorder
        internal void Start(){}

        // TODO Auto recorder
        internal void Stop()
        {
            _writer.Close();
        }
        
        // TODO Auto recorder
        internal void Pause(){}
        
        // TODO Auto recorder
        internal void Continue(){}

        internal void Destroy()
        {
            _writer.Close();
        }
        
        private static string GenFile(string recorder, [CanBeNull] string custom, string prefix)
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
        
    }
    
    /// <summary>
    /// This interface must be inherited when defining the data structure that needs to be recorded
    /// </summary>
    public interface IRecorderData
    {
        
    }
}