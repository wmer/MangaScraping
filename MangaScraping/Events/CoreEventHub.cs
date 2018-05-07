using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Events {
    public static class CoreEventHub {
        public static event ProcessingEventHandler ProcessingStart;
        public static event ProcessingEventHandler ProcessingEnd;
        public static event ProcessingEventHandler ProcessingProgress;
        public static event ProcessingErrorEventHandler ProcessingError;

        public static void OnProcessingStart(object sender, ProcessingEventArgs e) {
            ProcessingStart?.Invoke(sender, e);
        }

        public static void OnProcessingProgress(object sender, ProcessingEventArgs e) {
            ProcessingProgress?.Invoke(sender, e);
        }

        public static void OnProcessingEnd(object sender, ProcessingEventArgs e) {
            ProcessingEnd?.Invoke(sender, e);
        }

        public static void OnProcessingProgressError(object sender, ProcessingErrorEventArgs e) {
            ProcessingError?.Invoke(sender, e);
        }
    }
}
