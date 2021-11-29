using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {

    public Camera TopCamera;
    public Camera LeftCamera;
    public Camera CarCamera;
    int flag;

    void Start() {

        flag = 2;

        TopCamera.enabled = true;
        LeftCamera.enabled = false;
        CarCamera.enabled = false;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            
            if(flag==1) {
                ShowTopCamera();
            }

            if(flag==2) {
                ShowLeftCamera();
            }

            if(flag==3) {
                ShowCarCamera();
            }

            flag++;

            if(flag > 3)
                flag = 1;
        }
    }
    public void ShowTopCamera() {
        
        TopCamera.enabled = true;
        LeftCamera.enabled = false;
        CarCamera.enabled = false;
    }
    public void ShowLeftCamera() {
        
        LeftCamera.enabled = true;
        TopCamera.enabled = false;
        CarCamera.enabled = false;
    }

    public void ShowCarCamera() {
        
        CarCamera.enabled = true;
        TopCamera.enabled = false;
        LeftCamera.enabled = false;
    }
    

}
