using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace observable_source
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Test ISourceAware";
            var collection = new CustomObservableCollection<Item>();

            collection.Add(new Item { Description = "Item 1" });
            collection.Add(new Item { Description = "Item 2" });

            Console.WriteLine($"Min: {collection.Min(_ => _.Id)}");
            Console.WriteLine($"Max: {collection.Max(_ => _.Id)}");

            Console.ReadKey();
        }
    }
    public interface ISourceAware
    {
        ICollection Source { get; set; }
        string Id { get; }
    }
    public class Item : ISourceAware
    {
        public string Description { get; set; } = string.Empty;
        public ICollection Source { get; set; }
        public CustomObservableCollection<Item> ParentCollection =>
            (CustomObservableCollection<Item>)Source;
        public string Id { get; set; }
            = Guid.NewGuid().ToString();    // Automatic unique initializastion if you want it.
    }
    public class CustomObservableCollection<T> : ObservableCollection<T> where T: ISourceAware
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if(e.Action.Equals(NotifyCollectionChangedAction.Add)) 
            {
                foreach (ISourceAware iitem in e.NewItems)
                {
                    iitem.Source = this;
    #if DEBUG
                    var item = (Item)iitem;
                    // Try a loopback test
                    Console.WriteLine(
                        $"Index of {item.Description} is {item.ParentCollection.IndexOf(item)}");
    #endif
                }
            }
        }
    }
}