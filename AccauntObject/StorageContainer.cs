using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AccauntObject
{
    class StorageContainer
    {
        private List<Part> storage;

        public StorageContainer()
        {
            this.storage = null;
        }

        public StorageContainer(string name, int count, decimal price)
        {
            
        }
    }
}
