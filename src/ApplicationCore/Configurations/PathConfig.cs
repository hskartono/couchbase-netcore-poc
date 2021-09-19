using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Configurations
{
    public class PathConfig
    {
        // informasi konfigurasi URL
        public string BaseURI { get; set; }         // https://api.url.com/v1/api
        public string AppURI { get; set; }          // https://www.app-url.com/
        public string DownloadURI { get; set; }     // $AppURI/ DownloadFiles/
        public string UploadURI { get; set; }       // $AppURI/ UploadedFiles/

        /// <summary>
        /// Digunakan untuk menyimpan informasi path folder Storage.
        /// Jika diisi ~ maka akan di replace dengan current path.
        /// Contoh:
        ///   ~/Storage
        ///   /var/log/storage
        ///   d:/temp
        /// </summary>
        public string BaseStoragePath { get; set; }

        /// <summary>
        /// Digunakan untuk menyimpan informasi path download folder.
        /// Jika diisi ~ maka akan di replace dengan BaseStoragePath + lokasi folder download.
        /// Contoh:
        ///   ~/Downloads = ~/Storage/Downloads
        /// </summary>
        public string DownloadPath { get; set; }

        /// <summary>
        /// Digunakan untuk menyimpan informasi path upload folder.
        /// Jika diisi ~ maka akan di replace dengan BaseStoragePath + lokasi folder upload.
        /// Contoh:
        ///   ~/Uploads = ~/Storage/Uploads
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        /// Digunakan untuk menyimpan informasi path log folder.
        /// Jika diisi ~ maka akan di replace dengan BaseStoragePath + lokasi folder log.
        /// Contoh:
        ///   ~/Logs = ~/Storage/Logs
        /// </summary>
        public string LogsPath { get; set; }

        /// <summary>
        /// Digunakan untuk menyimpan informasi path temporary folder.
        /// Jika diisi ~ maka akan di replace dengan BaseStoragePath + lokasi folder temporary.
        /// Contoh:
        ///   ~/Temp = ~/Storage/Temp
        /// </summary>
        public string TempPath { get; set; }

        /// <summary>
        /// Digunakan untuk menyimpan informasi path template folder.
        /// Jika diisi ~ maka akan di replace dengan BaseStoragePath + lokasi folder template.
        /// Contoh:
        ///   ~/Templates = ~/Storage/Templates
        /// </summary>
        public string TemplatePath { get; set; }

    }
}
