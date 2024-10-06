// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public static class NameGenerator {

        public static string GenerateName(Item item) {
            return item.HasId<Brand>() ? item.FindId<Brand>().ToString() : "";
        }

    }

}
