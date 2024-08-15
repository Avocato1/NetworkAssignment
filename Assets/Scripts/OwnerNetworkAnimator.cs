using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class OwnerNetworkAnimator : NetworkAnimator
{
   //sync animation on network
   protected override bool OnIsServerAuthoritative()
   {
      return false;
   }
}
