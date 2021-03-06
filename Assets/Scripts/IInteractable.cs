﻿using UnityEngine;
using System.Collections;

namespace Volley
{
    public interface IInteractable
    {
        int Points { get; set; }
        void Interact(object sender, object args);
    }
}
