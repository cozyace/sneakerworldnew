// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    using Main;

    public class TrackCash : MonoBehaviour {

        public WalletValueDrawer walletValDrawer;

        // Runs once before instantiation.
        public void Awake() {
            // Listen to the wallet events.
            Player.instance.wallet.onCashInitEvent.AddListener(walletValDrawer.DrawBalance);
            Player.instance.wallet.onCashTransactionEvent.AddListener(walletValDrawer.DrawTransaction);
        }

    }

}
