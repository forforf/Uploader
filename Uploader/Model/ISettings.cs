using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    public interface ISettings
    {
        string WatchPath { get; set; }
        string S3Path { get; set; }
    }
}
