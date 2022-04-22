using System;
using System.Collections.Generic;

namespace Lumberjack
{

    public class Details
    {
        public int summaryNumber;
        public DateTime createdDate;
        public DateTime updatedDate;
        public string savedAs;
        public DateTime confirmedDate;
        public string customerName;
        public DateTime dueDate;
    }
    
    public class ItemToOrder
    {
        public string name;
        public string material;
        public int quantity;
        public double price;
        public double priceWithVat;
    }

    public class TotalCost
    {
        public double total;
        public double totalWithVat;
    }

    public class Order
    {
        public Details detailsSave { get; set; }
        public List<ItemToOrder> itemToOrderSave { get; set; }
        public TotalCost totalCostSave { get; set; }
    }
    //public class Order
    //{
    //    public Details detailsSave = new Details();
    //    public ItemToOrder dtemToOrderSave = new ItemToOrder();
    //    public TotalCost totalCostSave = new TotalCost();
    //}

    //public ItemToOrderFile(string name, string material, int quantity, double price, double priceWithVat)
    //{
    //    Name = name;
    //    Material = material;
    //    Quantity = quantity;
    //    Price = price;
    //    PriceWithVat = priceWithVat;
    //}

    //public string Name { get; }
    //public string Material { get; }
    //public int Quantity { get; }
    //public double Price { get; }
    //public double PriceWithVat { get; }
}
