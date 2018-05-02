using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Events {
    public class ModelChangeEventArgs : EventArgs {
        public ModelBase ModelChanged { get; set; }

        public ModelChangeEventArgs(ModelBase modelChanged) {
            ModelChanged = modelChanged;
        }
    }

    public delegate void ModelChangeEventHandler(object sender, ModelChangeEventArgs ev);
}
