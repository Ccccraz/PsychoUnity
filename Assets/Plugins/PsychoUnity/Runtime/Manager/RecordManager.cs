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
    /// <summary>
    /// Provide standardized recording methods
    /// Save data to csv file, support record int, float and other basic data types,
    /// support record Unity.Vector3, Unity.Vector2. Allow to create multiple instances of recorder.
    /// </summary>
    public class RecordManager : Singleton<RecordManager>
    {
        private readonly Dictionary<string, Recorder<IRecorderData>> _recorderDic = new();

        /// <summary>
        /// Creating a Recorder
        /// </summary>
        /// <param name="recorderName"> recorder name </param>
        /// <param name="data"> Data that needs to be recorded </param>
        /// <param name="custom"> Customized identifiers that will be displayed at the end of the file name,
        /// default is empty </param>
        /// <param name="prefix"> Where the data is stored, by default: Assets/Data </param>
        public void Create(string recorderName, IRecorderData data, [CanBeNull] string custom = null, string prefix = "Assets/Data")
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

        /// <summary>
        /// Manually triggered recording, Write the current state of the data to file
        /// </summary>
        /// <param name="recorderName"> target recorder name </param>
        public void Write(string recorderName)
        {
            if (Verify(recorderName))
            {
                _recorderDic[recorderName].Write();
            }
        }

        /// <summary>
        /// TODO Auto recorder
        /// </summary>
        /// <param name="recordName"> target recorder name </param>
        public void Start(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Start();
            }
        }
        
        /// <summary>
        /// TODO Auto recorder
        /// </summary>
        /// <param name="recordName"> target recorder name </param>
        public void Stop(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Stop();
            }
        }

        /// <summary>
        /// TODO Auto recorder
        /// </summary>
        /// <param name="recordName"> target recorder name </param>
        public void Pause(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Pause();
            }
        }

        /// <summary>
        /// TODO Auto recorder
        /// </summary>
        /// <param name="recordName"> target recorder name </param>
        public void Continue(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Continue();
            }
        }

        // TODO Auto recorder
        /// <summary>
        /// Destroy target recorder
        /// </summary>
        /// <param name="recordName"> target recorder name </param>
        public void Destroy(string recordName)
        {
            if (Verify(recordName))
            {
                _recorderDic[recordName].Destroy();
            }
        }

        /// <summary>
        /// Destroy all recorders
        /// </summary>
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