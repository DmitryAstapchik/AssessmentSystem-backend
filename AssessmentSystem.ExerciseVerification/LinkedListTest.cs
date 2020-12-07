using System;
using System.Linq;
using AssessmentSystem.Core;
using AssessmentSystem.Factory;
using Fasterflect;
using NUnit.Framework;

namespace AssessmentSystem.ExerciseVerification
{
    [TestFixture]
    public class LinkedListTest
    {
        [Test]
        [Order(1)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodAddFirst(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;
            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddFirst(item);
            item = CreateListItem(linkedList.GetType(), 2);
            linkedList.AddFirst(item);

            item = linkedList.GetFirstItem();
            Assert.AreEqual(2, (int)item.Value);
        }

        [Test]
        [Order(2)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodAddLast(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;
            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddLast(item);
            item = CreateListItem(linkedList.GetType(), 2);
            linkedList.AddLast(item);

            item = linkedList.GetFirstItem();
            Assert.AreEqual(1, (int)item.Value);
        }

        [Test]
        [Order(3)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodInsert(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;
            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddLast(item);
            item = CreateListItem(linkedList.GetType(), 2);
            linkedList.AddLast(item);

            item = CreateListItem(linkedList.GetType(), 0);
            linkedList.Insert(item, 0);

            item = linkedList.GetFirstItem();
            Assert.AreEqual(0, (int)item.Value);
        }

        [Test]
        [Order(4)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodIsEmpty(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;

            Assert.IsTrue(linkedList.IsEmpty());

            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddLast(item);

            Assert.IsFalse(linkedList.IsEmpty());
        }

        [Test]
        [Order(5)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodClean(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;

            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddLast(item);

            Assert.IsFalse(linkedList.IsEmpty());
            linkedList.Clear();
            Assert.IsTrue(linkedList.IsEmpty());
            Assert.IsNull(linkedList.GetFirstItem());
        }

        [Test]
        [Order(6)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodGetAll(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;
            var item = CreateListItem(linkedList.GetType(), 1);
            linkedList.AddLast(item);
            item = CreateListItem(linkedList.GetType(), 2);
            linkedList.AddLast(item);
            item = CreateListItem(linkedList.GetType(), 3);
            linkedList.AddLast(item);

            var items = linkedList.GetAll();

            Assert.IsNotEmpty(items);
            Assert.AreEqual(2, items.Count());
            Assert.IsTrue(items.All(_ => (int)_.Value != 1));
        }

        [Test]
        [Order(7)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestMethodListItem(IRunnable obj)
        {
            var buff = obj.Run();
            Assert.That(buff, Is.AssignableTo<ILinkedList>());

            ILinkedList linkedList = (ILinkedList)buff;
            var item = CreateListItem(linkedList.GetType(), 1);
            var item2 = CreateListItem(linkedList.GetType(), 2, item);
            var item3 = CreateListItem(linkedList.GetType(), 3, item2);

            Assert.AreEqual(2, (int)item3.Prev().Value);
            Assert.AreEqual(3, (int)item2.Next().Value);
            Assert.IsNull(item.Prev());
            Assert.IsNotNull(item.Next());
        }

        private IListItem CreateListItem(Type obj, int data, IListItem item = null)
        {
            var type = obj.Assembly.TypesImplementing<IListItem>().FirstOrDefault();
            return (IListItem)Activator.CreateInstance(type, data, item);
        }
    }
}
