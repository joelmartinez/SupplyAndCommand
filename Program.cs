// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Resource rice = new Resource { Name = "Rice", Price = 1.00 };
Resource iron = new Resource { Name = "Iron", Price = 3.00 };

var countries = new[]
{
    new Country
    {
        Name = "Ashtonia",
        Currency = 100.00,
        Stockpile = new Dictionary<Resource, StockPileValue> {
            { rice, new StockPileValue { CurrentQuantity = 2, DomesticProduction = 1}  },
            { iron, new StockPileValue { CurrentQuantity = 10, DomesticProduction = 3}  }
        }
    },
    new Country
    {
        Name = "Joelandia",
        Currency = 100.00,
        Stockpile = new Dictionary<Resource, StockPileValue> {
            { rice, new StockPileValue { CurrentQuantity = 10, DomesticProduction = 5} },
            { iron, new StockPileValue { CurrentQuantity = 2, DomesticProduction = 0}  }
        }
    }
};

var j = countries[0];
var a = countries[1];

j.Needs.Add(new Need { Name ="Drought", Resource = rice, Severity = 2});

// calculate trades
foreach(var country in countries)
{
    foreach(var need in country.Needs)
    {
        var countryWithResource = countries.
            Where(c => c.Stockpile.ContainsKey(need.Resource)).
            OrderByDescending(c => c.Stockpile[need.Resource].CurrentQuantity).
            FirstOrDefault();

        if (countryWithResource != null)
        {
            country.Trade(countryWithResource, need.Resource, need.Severity, need.Resource.Price);
        }
    }
}

// calculate domestic production
foreach(var country in countries)
{
    foreach(var stock in country.Stockpile.Values)
    {
        stock.CurrentQuantity += stock.DomesticProduction;
    }
}

// output
foreach(var country in countries)
{
    Console.WriteLine(country);
}

public abstract class Entity
{
    public string Name {get;set;} = Guid.NewGuid().ToString();

    public override string ToString()
    {
        return this.Name;
    }
}
public class Need : Entity
{
    public Resource Resource {get;set;}
    public int Severity {get;set;}
}

public class Resource : Entity
{
    public double Price {get;set;}
}

public class StockPileValue
{
    public int DomesticProduction {get;set;}
    public int CurrentQuantity {get;set;}
}

public class Country : Entity
{

    public double Currency {get;set;}

    public Dictionary<Resource, StockPileValue> Stockpile = new Dictionary<Resource, StockPileValue>();

    public List<Need> Needs {get;set;} = new List<Need>();

    public void Trade(Country c, Resource r, int amount, double price)
    {
        if (this.Stockpile.ContainsKey(r) && this.Stockpile[r].CurrentQuantity >= amount)
        {
            double totalPrice = amount * price;
            if (this.Currency >= totalPrice)
            {
                this.Currency -= totalPrice;
                c.Currency += totalPrice;

                this.Stockpile[r].CurrentQuantity -= amount;
                c.Stockpile[r].CurrentQuantity += amount;

                Console.WriteLine($"{this.Name} traded {amount} {r.Name} to {c.Name} for ${totalPrice}");
            }
            else
            {
                Console.WriteLine($"{this.Name} does not have enough currency to trade {amount} {r.Name}");
            }
        }
        else
        {
            Console.WriteLine($"{this.Name} does not have enough {r.Name} to trade");
        }
    }

    public override string ToString()
    {
        return $"{this.Name}: ${this.Currency}{Environment.NewLine}{string.Join(Environment.NewLine, this.Stockpile)}";
    }
}