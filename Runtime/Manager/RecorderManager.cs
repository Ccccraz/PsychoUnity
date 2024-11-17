using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PsychoUnity.Manager;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Converters;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class RecorderManager : Singleton<RecorderManager>
    {
        private readonly Dictionary<string, RecorderBase> _recordDic = new();

        public void Create(string recorderName, object data, string lab, string subject, string number,
            string collection, [CanBeNull] string filename = null, [CanBeNull] string prefix = null)
        {
            if (_recordDic.ContainsKey(recorderName))
            {
                Debug.Log($"Recorder {recorderName} already exist");
            }
            else
            {
                _recordDic.Add(recorderName,
                    filename == null
                        ? new Recorder(data, lab, subject, number, collection, recorderName)
                        : new Recorder(data, lab, subject, number, collection, filename));
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
            internal static string GenFile(string lab, string subject, string number, string collection,
                string filename, [CanBeNull] string prefix = null)
            {
                // TODO: Check prefix validity
                var path = Path.Combine(prefix ?? Application.dataPath, lab, "Subjects", subject,
                    $"{DateTime.Now:yyyy-MM-dd}", number, collection, $"{filename}.json");

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
    private readonly JsonSerializerSettings _settings = new();


    internal Recorder(object data, string lab, string subject, string number, string collection,
        string filename, [CanBeNull] string prefix = null)
    {
        _output = RecorderManager.Utils.GenFile(lab, subject, number, collection, filename, prefix);
        _data = data;
        _settings.Converters.Add(new StringEnumConverter());
    }

    private string ToJson(object data)
    {
        if (_firstLine)
        {
            _firstLine = false;
            return JsonConvert.SerializeObject(data, _settings);
        }

        var result = JsonConvert.SerializeObject(data, _settings);
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
        writer.WriteLine("\n]");
    }
}