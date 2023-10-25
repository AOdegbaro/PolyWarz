using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
   private CharacterManager character;

   protected virtual void Awake()
   {
      character = GetComponent<CharacterManager>();
   }
   public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
   {
      character.animator.SetFloat("Horizontal", horizontalValue);
      character.animator.SetFloat("Vertical", verticalValue);
   }
}
