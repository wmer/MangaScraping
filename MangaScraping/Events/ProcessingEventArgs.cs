using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Events {
    public class ProcessingEventArgs : EventArgs {
        public DateTime Time { get; private set; }
        public String StateMessage { get; private set; }
        public ModelBase Item { get; set; }

        public ProcessingEventArgs(DateTime time) : this(time, null) {
        }

        public ProcessingEventArgs(DateTime time, String stateMessage) : this(time, null, stateMessage) { 
        }

        public ProcessingEventArgs(DateTime time, ModelBase item, String stateMessage) {
            Time = time;
            StateMessage = stateMessage;
            Item = item;
        }
    }

    public delegate void ProcessingEventHandler(object sender, ProcessingEventArgs ev);
}
