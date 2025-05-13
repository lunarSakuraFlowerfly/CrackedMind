using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ui.Utility;
using Unity.VisualScripting;

/// <summary>
/// Parallax scrolling script that should be assigned to a layer
/// </summary>
/// 
namespace Ui.Core
{
    public class ParallaxUV : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxEffectMultiplier = new Vector2(0.5f, 0.5f);
        [SerializeField] private bool infiniteHorizontal = true;
        [SerializeField] private bool infiniteVertical = true;
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;
        private void Start()
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        }
        private void LateUpdate()
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;
            if(infiniteHorizontal)
            {
                if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX/2)
                {
                    float offsetPositionX = (cameraTransform.position.x - transform.position.x) % (textureUnitSizeX/2);
                    transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
                }
            }

            if(infiniteVertical)
            {
                if(Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY/2)
                {
                    float offsetPositionY = (cameraTransform.position.y - transform.position.y) % (textureUnitSizeY/2);
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y+offsetPositionY);
                }
            }
        }
    }
}