using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Events {
    public class ProcessingErrorEventArgs : EventArgs {
        public DateTime Time { get; private set; }
        public string Link { get; private set; }
        public Exception Exception { get; private set; }

        public ProcessingErrorEventArgs(DateTime time, string link, Exception exception) {
            Time = time;
            Link = link;
            Exception = exception;
        }
    }

    public delegate void ProcessingErrorEventHandler(object sender, ProcessingErrorEventArgs ev);
}
