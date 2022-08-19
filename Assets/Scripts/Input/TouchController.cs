using System;
using Managers.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class TouchController : MonoBehaviour, IPointerDownHandler
    {
        private Camera mainCam;
        private RaycastHit[] raycastHits;
        private int squareLayerMask;

        private const float RAY_MAX_DISTANCE = 150f;

        private void Start()
        {
            mainCam = Camera.main;
            raycastHits = new RaycastHit[5];

            squareLayerMask = LayerMask.GetMask("Square");
        }

        private void OnDestroy()
        {
            mainCam = null;
            raycastHits = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var screenPosV2 = eventData.pointerCurrentRaycast.screenPosition;
            Vector3 screenPosV3 = screenPosV2;
            screenPosV3.z = mainCam.nearClipPlane;

            var ray = mainCam.ScreenPointToRay(screenPosV3);

            var resultCount = Physics.RaycastNonAlloc(ray, raycastHits, RAY_MAX_DISTANCE, squareLayerMask);

            if (resultCount == 0) {return;}
            
            GridManager.Instance.HandleChosenSquare(raycastHits[0].transform.parent);
        }
    }
}