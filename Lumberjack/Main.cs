using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Lumberjack
{
    public class Main
    {
        public static Item item = new Item();
        public static ItemToOrder itemToOrder = new ItemToOrder();
        public static List<ItemToOrder> itemsForOrder = new List<ItemToOrder>();
        public static List<ItemToOrder> totalCost = new List<ItemToOrder>();
        public static Details details = new Details();
        public static TotalCost totalCostPrice = new TotalCost();

        public static string GetJsonPath()
        {
            string warehousePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return warehousePath + @"\warehouse.json";
        }

        public static string GetOrdersPath()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return path + @"\Orders";
        }

        public static void SearchItem()
        {
            int found = 0;

            AddCreatedDate();
            Console.WriteLine("Enter item name:");
            string searchString = Console.ReadLine();

            StreamReader file = new StreamReader(GetJsonPath());
            string jsonString = file.ReadToEnd();
            file.Close();
            ListItem jsonData = JsonConvert.DeserializeObject<ListItem>(jsonString);

            foreach (var jsonDataItem in jsonData.Item)
            {
                if (jsonDataItem.Name == searchString.ToUpper())
                {
                    item.Name = jsonDataItem.Name;
                    item.Material = jsonDataItem.Material;
                    item.Quantity = jsonDataItem.Quantity;
                    item.Price = jsonDataItem.Price;
                    Console.WriteLine("Item: " + searchString + " is available");
                    // Print Material, Quantity...
                    found = 1;

                    AddItemToTheOrder();
                }
            }
            if (found == 0)
            {
                Console.WriteLine("_________________");
                Console.WriteLine("|Item not found.|");
                Console.WriteLine("─────────────────");
            }

        }

        public static void AddItemToTheOrder()
        {
            string userResponse = "";
            while (userResponse != "N")
            {
                Console.WriteLine("Do you want to add item to your order? [Y / N]");
                userResponse = Console.ReadLine().ToUpper();
                if (userResponse == "Y")
                {
                    itemToOrder.name = item.Name;
                    itemToOrder.material = item.Material;
                    itemToOrder.price = item.Price;
                    itemToOrder.priceWithVat = itemToOrder.price + itemToOrder.price * 20 / 100;
                    EnterItemQuantity();
                    break;
                }
                else if (userResponse == "N")
                {
                    Console.WriteLine("Item will not be added to your order!");
                    Console.WriteLine("Search for new item? [Y / N]");
                    string newUserResponse = Console.ReadLine();
                    if (newUserResponse == "Y")
                    {
                        SearchItem();
                        EnterItemQuantity();
                    }
                    else if (newUserResponse == "N")
                    {
                        Console.WriteLine("Thank you for using Wood Worker app!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Answer must be Y or N, please try again!");
                    }
                }
                else
                {
                    Console.WriteLine("Answer must be Y or N, please try again!");
                }
            }

        }

        public static void EnterItemQuantity()
        {
            Console.WriteLine("Enter quantity for selected product:");
            itemToOrder.quantity = Int32.Parse(Console.ReadLine());
            string answer = "";

            if (itemToOrder.quantity > item.Quantity)
            {
                while (answer != "N")
                {
                    Console.WriteLine("Not enough products on stock! There are only " + item.Quantity + " products left. Do you want to continue with your order and make reduced one, " + item.Quantity + " as maximum? [Y / N ]");
                    answer = Console.ReadLine();
                    if (answer == "Y")
                    {
                        Console.WriteLine("Enter desired quantity for selected product, (maximum available number is " + item.Quantity + ").");
                        itemToOrder.quantity = Int32.Parse(Console.ReadLine());
                        itemsForOrder.Add(new ItemToOrder { name = itemToOrder.name, material = itemToOrder.material, quantity = itemToOrder.quantity, price = itemToOrder.price, priceWithVat = itemToOrder.priceWithVat });
                        UpdateStockAfterOrderCreation(itemToOrder.name, itemToOrder.quantity);
                        break;
                    }
                    else if (answer == "N")
                    {
                        Console.WriteLine("Thank you...");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Reply with Y or N");
                    }
                }
            }
            //else if(itemTo
            //
            //.quantity == 0)
            //{
            //    Console.WriteLine("Quantity must be greater than 0! Please try again.");
            //}
            else
            {
                itemsForOrder.Add(new ItemToOrder
                {
                    name = itemToOrder.name,
                    material = itemToOrder.material,
                    quantity = itemToOrder.quantity,
                    price = itemToOrder.price,
                    priceWithVat = itemToOrder.priceWithVat
                });

                var totalPrice = itemToOrder.quantity * itemToOrder.price;
                var totalPriceWithVat = itemToOrder.quantity * itemToOrder.priceWithVat;
                totalCost.Add(new ItemToOrder { price = totalPrice, priceWithVat = totalPriceWithVat });

                UpdateStockAfterOrderCreation(itemToOrder.name, itemToOrder.quantity);
            }
        }

        public static void AddMoreItemsToTheOrder()
        {
            string answer = "";
            Console.WriteLine("Add more products to your order? [Y / N]");
            answer = Console.ReadLine();
            answer.ToUpper();
            while (answer != "N")
            {
                if (answer == "Y")
                {
                    SearchItem();
                    Console.WriteLine("Thank you!");
                    break;
                }
                else if (answer == "N")
                {
                    Console.WriteLine("Thank you!");
                    break;
                }
                else
                {
                    Console.WriteLine("Answer must be Y or N, please try again!");
                }
            }
        }

        public static void AddDueDate()
        {
            Console.WriteLine("Please enter due date for your order in format YYYY/MM/DD:");
            details.dueDate = Convert.ToDateTime(Console.ReadLine());
            Console.WriteLine(details.dueDate);
        }

        public static void AddCreatedDate()
        {
            details.createdDate = DateTime.Now;
        }

        public static void AddUpdatedDate()
        {
            details.updatedDate = DateTime.Now;
        }

        public static void AssignSummayNumber()
        {
            int fCount = Directory.GetFiles(GetOrdersPath(), "*", SearchOption.AllDirectories).Length;
            details.summaryNumber = fCount + 1;
        }

        public static string GetLastFile()
        {
            var directory = new DirectoryInfo(GetOrdersPath());
            var myFile = (from f in directory.GetFiles()
                          orderby f.LastWriteTime descending
                          select f).First();

            return directory.ToString() + @"\" + myFile.Name.ToString();
        }

        public static void AddCustomerName()
        {
            Console.WriteLine("Please enter your name:");
            details.customerName = Console.ReadLine();

            AssignSummayNumber();
        }

        public static void DisplayItems()
        {
            string output = "";

            Console.WriteLine(" ");
            Console.WriteLine("_______________________________________________________");
            output = string.Format("|{0,10}|{1,9}|{2,9}|{3,6}|{4,15}|", "Item name", "Material", "Quantity", "Price", "Price with VAT");
            Console.WriteLine(output);
            Console.WriteLine("_______________________________________________________");

            foreach (var itemsList in itemsForOrder)
            {
                output = String.Format("|{0,10}|{1,9}|{2,9}|{3,6}|{4,15}|\n", itemsList.name, itemsList.material, itemsList.quantity.ToString(), itemsList.price.ToString(), itemsList.priceWithVat.ToString());
                Console.WriteLine(output);
            }
        }

        public static void DisplaySummaryDetails()
        {
            Console.WriteLine(" ");
            string output = string.Format("|{0,15}|{1,15}|{2,15}|{3,25}|{4,15}|{5,15}|{6,15}|", "Summary #", "Created date", "Updated date", "Saved as", "Customer name", "Due date", "Confirmed date");
            Console.WriteLine(output);

            output = string.Format("|{0,15}|{1,15}|{2,15}|{3,25}|{4,15}|{5,15}|{6,15}|",
                details.summaryNumber,
                String.Format("{0:yyyy/M/d}", details.createdDate),
                String.Format("{0:yyyy/M/d}", details.updatedDate),
                details.savedAs,
                details.customerName,
                String.Format("{0:yyyy/M/d}", details.dueDate),
                String.Format("{0:yyyy/M/d}", details.confirmedDate));
            Console.WriteLine(output);

        }

        public static void DisplayTotalCost()
        {
            Console.WriteLine(" ");
            string output = string.Format("|{0,11}|{1,21}|", "Total Cost", "Total cost with VAT");
            Console.WriteLine(output);
            output = string.Format("|{0,11}|{1,21}|", totalCostPrice.total.ToString(), totalCostPrice.totalWithVat.ToString());
            Console.WriteLine(output);
        }

        public static bool UpdateStockAfterOrderCreation(string product, int quantity)
        {
            int i = 0;
            int updatedQuantity = item.Quantity - itemToOrder.quantity;
            string jsonPath = GetJsonPath();

            StreamReader file = new StreamReader(jsonPath);
            string jsonString = file.ReadToEnd();
            file.Close();

            ListItem jsonData = JsonConvert.DeserializeObject<ListItem>(jsonString);

            foreach (var jsonDataItem in jsonData.Item)
            {
                if (jsonDataItem.Name == product.ToUpper())
                {
                    jsonData.Item[i].Quantity = updatedQuantity;
                }
                i++;
            }

            string jsonUpdate = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            File.WriteAllText(jsonPath, jsonUpdate);


            AddMoreItemsToTheOrder();
            return true;
        }

        public static void SaveFileToJson()
        {
            string fileName = "";
            string dueDate = "";
            string ordersPath = GetOrdersPath();

            Details detailsData = new Details()
            {
                summaryNumber = details.summaryNumber,
                createdDate = details.createdDate,
                updatedDate = details.updatedDate,
                savedAs = details.savedAs,
                customerName = details.customerName,
                dueDate = details.dueDate,
                confirmedDate = details.confirmedDate
            };

            List<ItemToOrder> items = new List<ItemToOrder>();
            for (int i = 0; i < itemsForOrder.Count; i++)
            {
                items.Add(new ItemToOrder()
                {
                    name = itemsForOrder[i].name,
                    material = itemsForOrder[i].material,
                    quantity = itemsForOrder[i].quantity,
                    price = itemsForOrder[i].price,
                    priceWithVat = itemsForOrder[i].priceWithVat
                });
            }

            TotalCost cost = new TotalCost()
            {
                total = totalCostPrice.total,
                totalWithVat = totalCostPrice.totalWithVat
            };

            var obj = new Order()
            {
                detailsSave = detailsData,
                itemToOrderSave = items,
                totalCostSave = cost
            };

            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // Type filename or if nothing entered save by default
            Console.WriteLine("Enter file name to be saved or press enter to save by default");
            fileName = Console.ReadLine();

            try
            {
                if (fileName == "")
                {
                    dueDate = String.Format("{0:yyyy.M.d}", details.dueDate);
                    details.savedAs = details.customerName + "_" + dueDate + "_" + details.summaryNumber;
                    File.WriteAllText(ordersPath + @"\" + details.savedAs + ".json", json);
                }
                else
                {
                    details.savedAs = fileName;
                    File.WriteAllText(ordersPath + @"\" + details.savedAs + ".json", json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot save file...");
                Console.WriteLine(e.Message);
                return;
            }

        }

        public static void LoadPreviousFile()
        {
            string lastFilePath = GetLastFile();
            StreamReader file = new StreamReader(lastFilePath);
            string lastFileString = file.ReadToEnd();
            file.Close();
            Order jsonData = JsonConvert.DeserializeObject<Order>(lastFileString);

            // Read data from file in memory
            details.summaryNumber = jsonData.detailsSave.summaryNumber;
            details.createdDate = jsonData.detailsSave.createdDate;
            details.updatedDate = jsonData.detailsSave.updatedDate;
            details.savedAs = jsonData.detailsSave.savedAs;
            details.confirmedDate = jsonData.detailsSave.confirmedDate;
            details.customerName = jsonData.detailsSave.customerName;
            details.dueDate = jsonData.detailsSave.dueDate;

            foreach (var jsonDataItem in jsonData.itemToOrderSave)
            {
                itemsForOrder.Add(new ItemToOrder
                {
                    name = jsonDataItem.name,
                    material = jsonDataItem.material,
                    quantity = jsonDataItem.quantity,
                    price = jsonDataItem.price,
                    priceWithVat = jsonDataItem.priceWithVat
                });
            }

            totalCostPrice.total = jsonData.totalCostSave.total;
            totalCostPrice.totalWithVat = jsonData.totalCostSave.totalWithVat;

            ConsoleDisplayFileData();
        }

        public static void ConsoleDisplayFileData()
        {
            DisplaySummaryDetails();
            DisplayItems();
            DisplayTotalCost();
        }
    }
}
