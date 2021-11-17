using System.Linq;
using DerekWare.IO;
using DerekWare.IO.Serialization;

namespace DerekWare.HomeAutomation.Lifx.Lan.Devices
{
    public static class Products
    {
        public static Vendor[] Vendors;

        static Products()
        {
            Load(new Path("products.json"));
        }

        public static Product GetProduct(this Vendor vendor, int productId)
        {
            return (from product in vendor.products
                    where product.pid == productId
                    select product).FirstOrDefault();
        }

        public static Vendor GetVendor(int vendorId)
        {
            return (from vendor in Vendors
                    where vendor.vid == vendorId
                    select vendor).FirstOrDefault();
        }

        static void Load(Path path)
        {
            Vendors = JsonSerializer<Vendor[]>.Default.DeserializeFile(path);
        }

        public class Features
        {
            public bool buttons { get; set; }
            public bool chain { get; set; }
            public bool color { get; set; }
            public bool extended_multizone { get; set; }
            public bool hev { get; set; }
            public bool infrared { get; set; }
            public bool matrix { get; set; }
            public bool multizone { get; set; }
            public bool relays { get; set; }
            public int[] temperature_range { get; set; }
        }

        /*
            "products": [
                {
                    "pid": 1,
                    "name": "LIFX Original 1000",
                    "features": {
                        "color": true,
                        "chain": false,
                        "matrix": false,
                        "infrared": false,
                        "multizone": false,
                        "temperature_range": [
                            2500,
                            9000
                        ]
                    },
                    "upgrades": []
                },
         */
        public class Product
        {
            public Features features { get; set; }
            public string name { get; set; }
            public int pid { get; set; }

            public override string ToString()
            {
                return name;
            }
        }

        /*
            "vid": 1,
            "name": "LIFX",
            "defaults": {
                "hev": false,
                "color": false,
                "chain": false,
                "matrix": false,
                "relays": false,
                "buttons": false,
                "infrared": false,
                "multizone": false,
                "temperature_range": null,
                "extended_multizone": false
            },
         */
        public class Vendor
        {
            public Features defaults { get; set; }
            public string name { get; set; }
            public Product[] products { get; set; }
            public int vid { get; set; }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
