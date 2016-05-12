using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uploader
{
    public partial class Uploader : Form
    {

        private FileDetector tempFileDetector;
        private FileSystemWatcher MyWatcher;

        public Uploader()
        {
            InitializeComponent();
        }

        private void Uploader_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Loading Form");
            //this.MyWatcher = new FileSystemWatcher();
            //MyWatcher.Path = "c:\\temp";
            //MyWatcher.IncludeSubdirectories = true;
            //MyWatcher.NotifyFilter = NotifyFilters.Attributes |
            //    NotifyFilters.CreationTime |
            //    NotifyFilters.FileName |
            //    NotifyFilters.LastAccess |
            //    NotifyFilters.LastWrite |
            //    NotifyFilters.Size |
            //    NotifyFilters.Security;
            //MyWatcher.EnableRaisingEvents = true;
            //this.MyWatcher.Changed += (watchSender, args) =>
            //{
            //    System.Diagnostics.Debug.WriteLine("Something changed FW");
            //};
            String watchPath = "c:\\temp";
            this.tempFileDetector = new FileDetector(
                new FileSystemWatcherWrapper(new FileSystemWatcher()),
                watchPath 
            );

            this.tempFileDetector.OnChanged += (source, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Something changed: " + args.FullPath + ":" + args.ChangeType);
            };
        }
    }
}
