using System;
using UniRx;

namespace Game.Shared.Bus {
    public interface IEvtPackage {
        Subject<object> subject { get; set; }
        Subject<bool> subjectBool { get; set; }
        IDisposable disposable { get; set; }
        IObservable<object> observable { get; }
        IObservable<bool> observableBool { get; }
        string busId { get; set; }
        int eventId { get; set; }
        string id { get; set; }
    }
}