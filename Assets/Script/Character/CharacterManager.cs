using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
   public CharacterController characterController;

   CharacterNetworkManager characterNetworkManager;
   protected virtual void Awake()
   {
      DontDestroyOnLoad(this);

      characterController = GetComponent<CharacterController>();
      characterNetworkManager = GetComponent<CharacterNetworkManager>();
   }

   protected virtual void Update()
   {
      // If this character is being controlled from our side, then assign its network position to the position of our transform
      if (IsOwner)
      {
         characterNetworkManager.networkPosition.Value = transform.position;
      }
      // If this character is being controlled from else where, then assign its position here locally by the position of its network transform
      else
      {
         transform.position = Vector3.SmoothDamp
         (transform.position, 
            characterNetworkManager.networkPosition.Value,
            ref characterNetworkManager.networkPositionVelocity, 
            characterNetworkManager.networkPositionSmoothTime);
      }
   }
}
