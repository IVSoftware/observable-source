Since each item in your collection needs to be aware of its parent collection, one way to solve this objective is to define an interface like `ISourceAware` and constrain T to accept only items that implement it.

    public interface ISourceAware
    {
        ICollection Source { get; set; }
    }

The collection handles its own `CollectionChanged` notification to set itself as the `Source` of any item added to it.

    public class CustomObservableCollection<T> : ObservableCollection<T> where T: ISourceAware
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if(e.Action.Equals(NotifyCollectionChangedAction.Add)) 
            {
                foreach (Item item in e.NewItems)
                {
                    Console.WriteLine(item.Description);
                    item.Source = this;

    #if DEBUG
                    // Try a loopback test
                    Console.WriteLine(
                        $"Index of {item.Description} is {item.ParentCollection.IndexOf(item)}");
    #endif
                }
            }
        }
    } 
***
A minimal example of a class that implements `ISourceAware`:

    public class Item : ISourceAware
    {
        public string Description { get; set; } = string.Empty;
        public ICollection Source { get; set; }
        public CustomObservableCollection<Item> ParentCollection =>
            (CustomObservableCollection<Item>)Source;
    }

***
**Test**

Here's the quick console code I wrote to test this answer.

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Test ISourceAware";
            var collection = new CustomObservableCollection<Item>();

            collection.Add(new Item { Description = "Item 1" });
            collection.Add(new Item { Description = "Item 2" });

            Console.ReadKey();
        }
    }


[![console output][1]][1]


  [1]: https://i.stack.imgur.com/dCuvg.png