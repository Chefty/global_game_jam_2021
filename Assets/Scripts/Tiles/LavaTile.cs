﻿using System.Collections;
using UnityEngine;

public class LavaTile : Tile
{
    public override bool CheckTileAccessibility()
    {
        return true;
    }

    public override void TileBehaviour() {
        if (!GameManager.Instance.DoesPlayerPosessAbility(typeof(Shoes))) {
            StartCoroutine(StartDeathDelayCO(.25f));
        }
    }

    private IEnumerator StartDeathDelayCO(float delay) {

        yield return new WaitForSeconds(delay);

        SoundFxManager.Instance.PlayDeathSound();
        GameManager.Instance.playerMovement.currentState = eState.death;
        GameManager.Instance.onDieOnLava();
        GameManager.Instance.DisplayDeathScreen();
        GameManager.Instance.isDead = true;
    }
}
