using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Services.Caching;

namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    public class StaticControllerBase : ControllerCommon
    {
        public string GetJSFile(string FilePath, bool Minified)
        {
            string Key = string.Format(Constants.STATIC_FILE_CACHE_KEY, FilePath);
            FileInfo fileInfo = new FileInfo(FilePath);
            TextFileCache FileCache = FormsEngineCacheProxy.Cache.Get<TextFileCache>(Key);

            if (FileCache == null|| FileCache.LastModified!=fileInfo.LastWriteTime)
            {
                string  TextFile = System.IO.File.ReadAllText(FilePath);
                FileCache = new TextFileCache();
                FileCache.FileStreamMinified = new Util.HTMLExtensions.JsMinify().Compress(TextFile);
                FileCache.FileStream = TextFile;
                FileCache.LastModified = fileInfo.LastWriteTime;
                FormsEngineCacheProxy.Cache.Set(Key,FileCache);
            }
            if (Minified)
            {
                return FileCache.FileStreamMinified;
            }
            else
            {
                return FileCache.FileStream;
            }

        }
    }
}