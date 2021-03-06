﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace minichain
{
    /// <summary>
    /// Slow and unstable, but easy to implement
    /// 
    /// WRITE: SingleThread
    /// READ: MultiThread
    /// </summary>
    public class FileDB : IStorageBackend, IDisposable
    {
        public string key;

        private Dictionary<string, string> memBuffer = new Dictionary<string, string>();
        private Thread flushThread;
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private bool isAlive = true;

        public FileDB()
        {
            key = Path.GetRandomFileName();
            Directory.CreateDirectory(key);

            flushThread = new Thread(FlushWorker);
            flushThread.Start();
        }
        ~FileDB()
        {
            Dispose();
        }
        public void Dispose()
        {
            isAlive = false;

            try
            {
                Directory.Delete(key, true);
            }
            catch { }
        }

        private void FlushWorker()
        {
            while (isAlive)
            {
                rwLock.EnterWriteLock();
                var localCopy = new Dictionary<string, string>(memBuffer);
                memBuffer.Clear();

                // [FIXME] PERFORMANCE SPIKE
                foreach (var pair in localCopy)
                {
                    try
                    {
                        WriteImmediateFlush(pair.Key, pair.Value);
                    }
                    catch (Exception e) { }
                }
                rwLock.ExitWriteLock();

                Thread.Sleep(1000);
            }
        }

        private string GetRelPath(string path)
        {
            return Path.Combine(key, path);
        }
        private string GetFilePath(string key)
        {
            var path = GetRelPath(key);
            if (key.Contains("/"))
            {
                key = key.Replace('/', Path.DirectorySeparatorChar);
                var pp = key.Split(Path.DirectorySeparatorChar);
                path = string.Join(Path.DirectorySeparatorChar.ToString(), pp.Take(pp.Length - 1).ToArray());

                if (Directory.Exists(GetRelPath(path)) == false)
                    Directory.CreateDirectory(GetRelPath(path));

                path = Path.Combine(GetRelPath(path), pp.Last());
            }
            return path;
        }

        public void Write(string key, object value)
        {
            rwLock.EnterWriteLock();
            memBuffer[key] = Serializer.Serialize(value, true);
            rwLock.ExitWriteLock();
        }
        private void WriteImmediateFlush(string key, string value)
        {
            File.WriteAllText(GetFilePath(key), value);
        }
        public T Read<T>(string key)
        {
            rwLock.EnterReadLock();
            try
            {
                if (memBuffer.ContainsKey(key))
                    return Serializer.Deserialize<T>(memBuffer[key]);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            if (File.Exists(GetFilePath(key)) == false)
                return default(T);

            T value;
            if (ReadFromDisk<T>(key, out value))
                return value;

            // Retry 5 times
            var spinWait = new SpinWait();
            var retry = 5;
            while (retry >= 0)
            {
                spinWait.SpinOnce();
                if (ReadFromDisk<T>(key, out value))
                    return value;

                retry--;
            }
            return default(T);
        }
        private bool ReadFromDisk<T>(string key, out T value)
        {
            try
            {
                var json = File.ReadAllText(GetFilePath(key), Encoding.UTF8);
                value = Serializer.Deserialize<T>(json);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                value = default(T);
                return false;
            }
        }
        public void Stash(string key)
        {
            try
            {
                File.Delete(GetFilePath(key));
            }
            catch (Exception e) { }
        }
    }
}
