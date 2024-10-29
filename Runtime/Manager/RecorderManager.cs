using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PsychoUnity.Manager;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class RecorderManager : Singleton<RecorderManager>
    {
        private readonly Dictionary<string, RecorderBase> _recordDic = new();

        public void Create(string recorderName, object data, [CanBeNull] string additional = null,
            [CanBeNull] string prefix = null)
        {
            if (_recordDic.ContainsKey(recorderName))
            {
                Debug.Log($"Recorder {recorderName} already exist");
            }
            else
            {
                _recordDic.Add(recorderName, new Recorder(data, recorderName, additional, prefix));
            }
        }

        public void Write(string recorderName)
        {
            var recorder = Verify(recorderName);
            recorder.Write();
        }

        public void Close(string recorderName)
        {
            var recorder = Verify(recorderName);
            recorder.Close();
        }

        private RecorderBase Verify(string recorderName)
        {
            return _recordDic.GetValueOrDefault(recorderName);
        }

        internal static class Utils
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
                    File.WriteAllText(path, "[\n");
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
}

internal abstract class RecorderBase
{
    internal abstract void Write();
    internal abstract void Close();
}

internal class Recorder : RecorderBase
{
    private readonly object _data;
    private readonly string _output;
    private bool _firstLine = true;

    internal Recorder(object data, string recorder, string additional, string prefix)
    {
        _output = RecorderManager.Utils.GenFile(recorder, additional, prefix);
        _data = data;
    }

    private string ToJson(object data)
    {
        if (_firstLine)
        {
            _firstLine = false;
            return JsonUtility.ToJson(data);
        }

        var result = JsonUtility.ToJson(data);
        return $",\n{result}";
    }

    internal override void Write()
    {
        var result = ToJson(_data);
        using var writer = new StreamWriter(_output, true);
        writer.Write(result);
    }

    internal override void Close()
    {
        using var writer = new StreamWriter(_output, true);
        writer.WriteLine("]");
    }
}