// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    // The different relationship statuses with other users.
    public enum RelationshipStatus {
        // Incoming relationships.
        IncomingRequested,
        IncomingAccepted,
        IncomingRejected,
        IncomingBlocked,
        // Outgoing relationships
        OutgoingRequested,
        OutgoingAccepted,
        OutgoingRejected,
        OutgoingBlocked,
        // The count.
        Count,
    }

}
