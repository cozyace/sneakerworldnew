// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
// Firebase.
using Firebase;
using Firebase.Auth;
using Firebase.Database;
// using Firebase.Crashlytics;
// Json.
using Newtonsoft.Json;

namespace SneakerWorld {

    /// <summary>
    /// References to all the paths in the firebase back-end database.
    /// </summary>
    public static class FirebasePath {

        // User paths.
        public static string User => $"users/{FirebaseManager.CurrentUser.UserId}";

        // The user state values.
        public static string Status => $"{User}/status";
        public static string LastLoggedOut => $"{Status}/lastLoggedOut";
        public static string IsOnline => $"{Status}/isOnline";

        // Wallet paths.
        public static string Wallet => $"{User}/wallet";
        public static string Cash => $"{Wallet}/cash";

        // Inventory paths.
        public static string Inventory => $"{User}/inventory";
        
        // Sneakers from inventory.
        public static string InventorySneakers => $"{Inventory}/sneakers";
        public static string InventorySneakersWithId(string sneakerId) {
            return $"{InventorySneakers}/{sneakerId}";
        }

        // Vendor paths.
        public static string Vendor => $"{User}/vendor";

        // Sneakers from vendor.
        public static string VendorSneakers => $"{Vendor}/sneakers";
        public static string VendorSneakersWithId(string sneakerId) {
            return $"{VendorSneakers}/{sneakerId}";
        }



        // Global paths.
        public static string Global => "global";

        // Item paths.
        public static string Items => $"{Global}/items";

        // The path to all the enabled items.
        public static string EnabledItems => $"{Items}/enabledItemList";
        
        // Sneaker paths.
        public static string Sneakers => $"{Items}/sneakers";
        public static string ForSneakerWithId(string sneakerId) {
            return $"{Sneakers}/{sneakerId}";
        }

        // Crate paths.
        public static string Crates => $"{Items}/crates";
        public static string ForCrateWithId(string crateId) {
            return $"{Crates}/{crateId}";
        }

        // Market paths.
        public static string Events => $"{Global}/events";

        // Market event with id.
        public static string ForEventWithId(string eventId) {
            return $"{Events}/{eventId}";
        }

    }

}
