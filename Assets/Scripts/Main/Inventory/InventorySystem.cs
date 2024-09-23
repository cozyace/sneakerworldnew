// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    public abstract class InventorySystem : PlayerSystem {

        public abstract Task<Inventory> Get();

        public abstract Task Set(Inventory inventory);

    }

}
