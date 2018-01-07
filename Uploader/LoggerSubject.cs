using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Model
{

    // TODO: Maybe only expose the observable rather than the subject?
    [Target("LoggerSubject")]
    public sealed class LoggerSubject : TargetWithLayout
    {
        public LoggerSubject()
        {
            this.Subject = new ReplaySubject<String>();
        }

        public ReplaySubject<String> Subject { get; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);
            this.Subject.OnNext(logMessage);
        }
    }
}
