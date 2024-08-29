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
        public static string AllUsers => "users";
        public static string User => $"{AllUsers}/{FirebaseManager.CurrentUser.UserId}";

        // The user state values.
        public static string Status => $"{User}/status";

        public static string Username => $"{Status}/username";
        public static string LastLoggedOut => $"{Status}/lastLoggedOut";
        public static string IsOnline => $"{Status}/isOnline";

        // The friends of this user.
        public static string FriendsList => $"{User}/friendsList";

        public static string FriendStatusWithId(string friendId) {
            return $"users/{friendId}/status";
        }

        public static string FriendsFriendListWithId(string friendId) {
            return $"users/{friendId}/friendsList";
        }
        // public static string Friend => $"users/{FirebaseManager.CurrentUser.UserId}";


        // Wallet paths.
        public static string Wallet => $"{User}/wallet";
        public static string Cash => $"{Wallet}/cash";
        public static string Gems => $"{Wallet}/gems";

        // Inventory paths.
        public static string Inventory => $"{User}/inventory";
        
        // Sneakers from inventory.
        public static string InventoryItemWithId(string itemId) {
            return $"{Inventory}/{itemId}";
        }
        

        // Vendor paths.
        public static string Store => $"{User}/store";
        public static string DailyStore(string datePath) {
            return $"{Store}/daily/{datePath}";
        }
        // Sneakers from vendor.
        // public static string VendorSneakers => $"{Vendor}/sneakers";
        // public static string VendorSneakersWithId(string sneakerId) {
        //     return $"{VendorSneakers}/{sneakerId}";
        // }


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

        //
        public static string Featured => $"{Global}/featured";
        public static string FeaturedItemWithDate(string datePath) {
            return $"{Featured}/{datePath}";
        }

        public static string MyFeaturedItemWithDate(string datePath) {
            return $"{Store}/{Featured}/{datePath}";
        }

    }

}
