using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class RecordManager : Singleton<RecordManager>
    {
        private readonly Dictionary<string, RecordBase> _recordDic = new();

        public void Create(string recorderName, object data, [CanBeNull] string additional = null, [CanBeNull] string prefix = null)
        {
            if (_recordDic.ContainsKey(recorderName))
            {
                Debug.Log($"Recorder {recorderName} already exist");
            }
            else
            {
                _recordDic.Add(recorderName, new Record(data, recorderName, additional, prefix));
            }
        }

        public void Write(string recorderName)
        {
            if (Verify(recorderName))
            {
                _recordDic[recorderName].Write();
            }
        }
        private bool Verify(string recorderName)
        {
            if (_recordDic.ContainsKey(recorderName)) return true;
            
            Debug.Log($"Recorder {recorderName} does not exist");
            return false;
        }
    }

    internal abstract class RecordBase
    {
        internal abstract void Write();
    }
    internal class Record : RecordBase
    {
        private readonly object _data;
        private readonly string _output;

        internal Record(object data, string recorder, string additional, string prefix)
        {
            _output = UtilsForRecord.GenFile(recorder, additional, prefix);
            _data = data;
        }

        private static string ToJson(object data)
        {
            var result = JsonUtility.ToJson(data);
            return $"{result},";
        }

        internal override void Write()
        {
            var result = ToJson(_data);
            using var writer = new StreamWriter(_output, true);
            writer.WriteLine(result);
        }
    }

    internal static class UtilsForRecord
    {
        internal static string GenFile(string recorderName, [CanBeNull] string additional = null,
            [CanBeNull] string prefix = null)
        {
            // TODO: Check prefix validity
            var path = Path.Combine(prefix ?? Application.persistentDataPath, $"{DateTime.Now:yyyyMMdd}",
                $"{recorderName}_{additional}_{DateTime.Now:HHmmss}.json");

            if (File.Exists(path)) return path;

            var directory = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directory))
            {
                throw new Exception("There no valid directory");
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                File.Create(path).Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return path;
        }
    }
}