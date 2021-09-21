using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class CameraFocalPoint : MonoBehaviour
    {
        [SerializeField] private GameObject targetToFollow;
        [SerializeField] private float rotationSpeed;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = targetToFollow.transform.position;
            Rotate();
        }

        private void Rotate()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up, rotationSpeed * horizontalMovement * Time.deltaTime);
        }
    }
}
