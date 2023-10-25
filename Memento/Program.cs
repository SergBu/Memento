// Паттерн Снимок - behavioral design pattern
//
// Назначение: Позволяет делать снимки состояния объектов, не раскрывая
// подробностей их реализации. Затем снимки можно использовать, чтобы
// восстановить прошлое состояние объектов.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Memento
{
    class Program
    {
        static void Main(string[] args)
        {
            // Клиентский код.
            Originator<StateObject> current = new Originator<StateObject>();
            current.SetState(new StateObject() { Id = 1, Name = "Object 1" });
            CareTaker<StateObject>.SaveState(current);
            current.ShowState();

            current.SetState(new StateObject() { Id = 2, Name = "Object 2" });
            CareTaker<StateObject>.SaveState(current);
            current.ShowState();

            current.SetState(new StateObject() { Id = 3, Name = "Object 3" });
            CareTaker<StateObject>.SaveState(current);
            current.ShowState();

            CareTaker<StateObject>.RestoreState(current, 0);
            current.ShowState();
        }

        // это объект для хранения состояния в мементо
        // требует реализации ICloneable для обеспечения deep copy, иначе
        // мементо будет заполнен shallow copys c ссылками на эту же область памяти (на один и тот же объект)
        public class StateObject : ICloneable
        {
            public int Id;
            public string Name;

            public override string ToString()
            {
                return $"Id = {Id} Name = {Name}";
            }

            public object Clone()
            {
                return new StateObject { Id = this.Id, Name = this.Name };
            }
        }

        // это обобщённая структура мементо для обёртывания объектов для хранения состояния
        public class Memento<T> where T : ICloneable
        {
            T StateObj;

            public T GetState()
            {
                return StateObj;
            }
            public void SetState(T stateObj)
            {
                StateObj = (T)stateObj.Clone();
            }
        }

        // это обобщённая структура для обёртывания самого текущего объекта состояния с опцией восстановления объекта состояния 
        public class Originator<T> where T : ICloneable
        {
            T StateObj;

            public Memento<T> CreateMemento()
            {
                var memento = new Memento<T>();
                memento.SetState(StateObj);
                return memento;
            }

            public void RestoreMemento(Memento<T> memento)
            {
                StateObj = memento.GetState();
            }

            public void SetState(T stateObj)
            {
                StateObj = stateObj;
            }

            public void ShowState()
            {
                Console.WriteLine(StateObj);
            }
        }

        // это статическая структура которая управляет объектами состояний через список мементо
        // клиент использует эту структуру для сохранения и восстановления состояния
        public static class CareTaker<T> where T : ICloneable
        {
            static List<Memento<T>> mementos = new List<Memento<T>>();

            public static void SaveState(Originator<T> originator)
            {
                mementos.Add(originator.CreateMemento());
            }

            public static void RestoreState(Originator<T> originator, int checkpoint)
            {
                originator.RestoreMemento(mementos[checkpoint]);
            }
        }
    }
}
