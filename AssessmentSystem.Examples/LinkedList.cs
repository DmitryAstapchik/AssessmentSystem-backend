using System;
using System.Collections.Generic;
using System.Collections;
using AssessmentSystem.Core;

namespace AssessmentSystem.Examples
{
    //прямой связанный список
    public class LinkedList : ILinkedList
    {
        //добавить элемент в начало
        public void AddFirst(IListItem item)
        {
            throw new NotImplementedException();
        }

        //добавить элемент в конец
        public void AddLast(IListItem item)
        {
            throw new NotImplementedException();
        }

        //вставить элемент перед элементом с указанным индексом
        //если элемента нет - вставить в конец
        public void Insert(IListItem item, int index)
        {
            throw new NotImplementedException();
        }

        //проверка есть ли эелементы в списке
        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        //вернуть первый эелемент в списке
        public IListItem GetFirstItem()
        {
            throw new NotImplementedException();
        }

        //вернуть все элементы списка, кроме первого
        public IEnumerable<IListItem> GetAll()
        {
            throw new NotImplementedException();
        }

        //очистить список
        public void Clear()
        {
            throw new NotImplementedException();
        }

        //сортировка списка в обратном порядке
        public void Reverse()
        {
            throw new NotImplementedException();
        }
    }

    //элемент связанного списка
    public class ListItem : IListItem
    {
        public ListItem(object obj, IListItem prev = null)
        {
            //логика инициализации
        }

        //предыдущий связанный элемент списка
        public IListItem Prev()
        {
            throw new NotImplementedException();
        }

        //следующий связанный элемент списка
        public IListItem Next()
        {
            throw new NotImplementedException();
        }

        //хранимое значение
        public object Value { get; }
    }

    public class ListActivator: IRunnable
    {
        public object Run()
        {
            return new LinkedList();
        }
    }
}