using System;
using System.Collections.Generic;
using System.IO;

namespace Cryoprison.Inspectors
{
    public class FileNotDestructivelyWritable : IInspector
    {
        private string id;
        private string path;

        public IInspector Init(string id, string path)
        {
            this.id = string.Format("FILE_{0}_SHOULD_NOT_BE_DESTRUCTIVELY_WRITABLE", id);
            this.path = path;

            return this;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public bool Ok
        {
            get
            {
                return !IsFileDestructivelyWritable(path);
            }
        }

        public static bool IsFileDestructivelyWritable(string path)
        {
            try
            {
                using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var bytes = new byte[] { 0, 1, 2, 3 };
                    file.Write(bytes, 0, 0);
                }
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsFileDestructivelyWritable bombed for {path}", ex);
                return false;
            }
            finally
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
