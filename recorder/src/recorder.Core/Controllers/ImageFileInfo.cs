using Recorder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recorder.Core.Controllers
{
    public class ImageFileInfo
    {
        public string? Title { get; set; }

        public string? LocalPath { get; private set; }

        public string PathInStorage { get; set; } = "";

        /// <summary>
        /// Type of file extension. Eg:"image/jpeg"
        /// </summary>
        public string? Extension { get; private set; }

        public ImageFileInfo(
            string? title,
            string? localPath,
            string? extension)
        {
            Title = title;

            LocalPath = localPath;

            Extension = extension;
        }
    }
}
