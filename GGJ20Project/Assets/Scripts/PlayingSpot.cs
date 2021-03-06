﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayingSpot : MonoBehaviour
{
    
    bool inDoTween = false;

    GameObject player = null;

    public GameObject sphere;

    Vector3 oldPos;

    private void Awake() {
        oldPos.Set(95.8f, 41.3f, -85.5f);
    }
    private void OnTriggerEnter(Collider other) {
        print("Enguiei");
        if(other.gameObject.CompareTag("Player")){
            print("Salvei");
            player = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        print("Nullers");
        player = null;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !inDoTween && player != null)
        {
            Debug.Log("Fuiiii!");
            //oldPos = player.transform.GetChild(0).transform.position;
            //print(oldPos);
            player.transform.GetChild(0).GetComponent<FPS>().enabled = false;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(player.transform.GetChild(0).DOMove(transform.GetChild(0).transform.position,  0.7f));
            
            //mySequence.Join(player.transform.DORotate(transform.GetChild(0).transform.position, 5f));
            //mySequence.Play();
        }

        if(Input.GetMouseButtonDown(1) && !inDoTween && player != null){
            Debug.Log("Fuiiii!");
            player.transform.GetChild(0).GetComponent<FPS>().enabled = true;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(player.transform.GetChild(0).DOMove(oldPos, 0.7f));
            //mySequence.Join(player.transform.DORotate(transform.GetChild(0).transform.position, 5f));
            //mySequence.PlayBackwards();
        }
    }
}
