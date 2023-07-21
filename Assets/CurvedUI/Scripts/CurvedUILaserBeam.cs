using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CurvedUI
{
    /// <summary>
    /// This class contains code that controls the visuals (only!) of the laser pointer.
    /// </summary>
    public class CurvedUILaserBeam : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        Transform LaserBeamTransform;
        [SerializeField]
        Transform LaserBeamDot;
        [SerializeField]
        bool CollideWithMyLayerOnly = false;
        [SerializeField]
        bool hideWhenNotAimingAtCanvas = false;



       [SerializeField] public Canvas parentCanvas;        // the parent canvas of this UI - only needed to determine if we need the camera  
        [SerializeField] public RectTransform rect;         // the recttransform of the UI object


        // you can serialize this as well - do NOT assign it if the canvas render mode is overlay though
        public Camera UICamera;
      //  RaycastHit hitt;
#pragma warning restore 0649

        // Update is called once per frame
        protected void Update()
        {

            //get direction of the controller
            Ray myRay = new Ray(this.transform.position, this.transform.forward);


            //make laser beam hit stuff it points at.
            if(LaserBeamTransform && LaserBeamDot) {
                //change the laser's length depending on where it hits
                float length = 10000;


                //create layerMaskwe're going to use for raycasting
                int myLayerMask = -1;
                if (CollideWithMyLayerOnly)
                {
                    //lm with my own layer only.
                    myLayerMask = 1 << this.gameObject.layer;
                }


                RaycastHit hit;
                if (Physics.Raycast(myRay, out hit, length, myLayerMask))
                {
                    length = Vector3.Distance(hit.point, this.transform.position);
                    //hitt=hit;
                    Vector3 mousePos = Input.mousePosition;
                    Vector2 mp = new Vector2(mousePos.x, mousePos.y);
                //    Debug.Log("MousePos=" + mp.ToString());
                    Vector2 ray2d = new Vector2(hit.point.x, hit.point.y);
                //    Debug.Log("RayCast=" + ray2d.ToString());
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, ray2d, UICamera, out Vector2 localPos);
                //    Debug.Log("Local Position=" + localPos.ToString());
                    //Find if we hit a canvas
                    CurvedUISettings cuiSettings = hit.collider.GetComponentInParent<CurvedUISettings>();
                    if (cuiSettings != null)
                    {
                        //find if there are any canvas objects we're pointing at. we only want transforms with graphics to block the pointer. (that are drawn by canvas => depth not -1)
                        int selectablesUnderPointer = cuiSettings.GetObjectsUnderPointer().FindAll(x => x != null && x.GetComponent<Graphic>() != null && x.GetComponent<Graphic>().depth != -1).Count;

                        length = selectablesUnderPointer == 0 ? 10000 : Vector3.Distance(hit.point, this.transform.position);
                    }
                    else if (hideWhenNotAimingAtCanvas) length = 0;
                }
                else if (hideWhenNotAimingAtCanvas) length = 0;


                //set the leangth of the beam
                LaserBeamTransform.localScale = LaserBeamTransform.localScale.ModifyZ(length);
            }
           

        }

        /*
        public void OnPointerClick(PointerEventData eventData)
        {
            //Vector3 mousePos = Input.mousePosition;
            //Vector2 mp = new Vector2(mousePos.x, mousePos.y);
            Vector2 ray2d = new Vector2(hitt.point.x, hitt.point.y);
            Debug.Log("Event Data=" + eventData.position.ToString());
            Debug.Log("Raycast Position=" + hitt);
            
            // this UI element has been clicked by the mouse so determine the local position on your UI element
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, ray2d /*eventData.position, UICamera, out Vector2 localPos);

            // we now have the local click position of our rect transform, but as you want the (0,0) to be bottom-left aligned, need to adjust it
            localPos.x += rect.rect.width / 2f;
            localPos.y += rect.rect.height / 2f;

            Debug.Log("Local Position=" + localPos.ToString());
        } */
    }
}
