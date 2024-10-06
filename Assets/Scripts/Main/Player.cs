// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;

namespace SneakerWorld.Main {

    using FirebaseUser = Firebase.Auth.FirebaseUser;
    using AuthResult = Firebase.Auth.AuthResult;

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Player : MonoBehaviour {

        public string id => FirebaseManager.CurrentUser.UserId;

        // Loading events.
        public UnityEvent<float> onSystemLoaded = new UnityEvent<float>();
        public UnityEvent onFailedToLoad = new UnityEvent();
        private int systemsLoaded = 0;
        private int totalSystems = 7;
        public float percentLoaded => (float)systemsLoaded / (float)totalSystems;

        // The components this script manages.
        public FeaturedSupply featuredSupply;
        public Supply supply;
        public Stock stock;
        public Shelf shelf;
        public Wallet wallet;
        public Status status;
        public FriendsList friends;
        public PurchaseHandler purchaser;

        public static Player instance;

        void Awake() {
            instance = this;
        }

        void Start() {
            GameObject.FindFirstObjectByType<SneakerWorld.Auth.LoginHandler>().onLoginSuccessEvent.AddListener(Init);
        }

        async void Init(string message) {
            await Initialize();
        }

        // Initializes the user.
        public async Task<bool> Initialize() {
            try {

                await featuredSupply.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await supply.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await stock.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await shelf.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await wallet.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await status.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                await friends.Initialize(this);
                systemsLoaded += 1;
                onSystemLoaded.Invoke(percentLoaded);

                return true;
            }
            catch (Exception exception) {
                onFailedToLoad.Invoke();
                Debug.Log(exception.Message);
            }
            return false;
        }

    }

}
