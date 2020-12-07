namespace AssessmentSystem.Core
{
    public interface IListItem
    {
        IListItem Prev(); //предыдущий элемент
        IListItem Next(); //следующий элемент
        object Value { get; } //значение, хранимое в элементе
    }
}