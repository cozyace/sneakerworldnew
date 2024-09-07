using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    public class ShopPanel : MonoBehaviour {

        public TextMeshProUGUI markupPct;
        public Image customer;
        public Image transaction;

        void FixedUpdate() {
            int markupPerc = (int)Mathf.Round(Player.instance.shop.markup*100f);
            markupPct.text = $"{markupPerc.ToString()}%";   
            customer.fillAmount = Player.instance.shop.customerTicks / Player.instance.shop.customerInterval;
            transaction.fillAmount = Player.instance.shop.customers == null || Player.instance.shop.customers.Count <= 0 || Player.instance.shop.customers[0] == null ? 0f : 
                1f - Player.instance.shop.customers[0].transactionTicks / Player.instance.shop.transactionInterval;
        }

    }

}
