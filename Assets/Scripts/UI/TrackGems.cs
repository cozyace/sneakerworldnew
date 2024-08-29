// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    using Main;

    public class TrackGems : MonoBehaviour {

        public WalletValueDrawer walletValDrawer;

        // Runs once before instantiation.
        public void Awake() {
            // Listen to the wallet events.
            Player.instance.wallet.onGemsInitEvent.AddListener(walletValDrawer.DrawBalance);
            Player.instance.wallet.onGemsTransactionEvent.AddListener(walletValDrawer.DrawTransaction);
        }

    }

}
