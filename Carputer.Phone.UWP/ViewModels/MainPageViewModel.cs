﻿using PropertyChanged;
using ReactiveUI;
using ST.Fx.OBDII;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.ViewModels
{
    [ImplementPropertyChanged]
    public class MainPageViewModel : ReactiveObject,  IObserver<OBDIIUpdate>
    {
        public double RPM { get; set; }
        public ObservableCollection<string> Traces { get; set; } = new ObservableCollection<string>();

        private IOBDIIService _obd2service;
        public MainPageViewModel(IOBDIIService obd2service)
        {
            _obd2service = obd2service;

            var observable = obd2service as IObservable<OBDIIUpdate>;
            observable.ObserveOnDispatcher().Subscribe(this);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(OBDIIUpdate update)
        {
            if (update.Properties.ContainsKey("RPM"))
            {
                var rpmAsString = update.Properties["RPM"];
                var rpm = 0.0;
                if (double.TryParse(rpmAsString, out rpm))
                {
                    RPM = rpm;
                }
            }
        }
    }
}
