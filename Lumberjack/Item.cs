using System.Collections.Generic;

namespace Lumberjack
{
    public class Item
    {
        public string Name { get; set; }
        public string Material { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
    }

    public class ListItem
    {
        public List<Item> Item { get; set; }
    }
}
